using System;

namespace AdaptiveOpponent.Core
{
    /// <summary>
    /// A Context Tree Weighting tree (Willems, Shtarkov and Tjalkens, 1995) in log-odds form, with per-visit fading
    /// (spec .scratch/opponent-v2/spec.md section 1). Each node keeps faded KT counts and L = log(Pe / Prod), the
    /// log-odds of its own estimate against its children. An update adds log Pe(x) - log p_child(x) to L (the child's
    /// change in weighted probability is its predictive probability), then L &lt;- gamma L, clamped to [-6, 6]. The
    /// prediction mixes leaf to root: p_node = beta Pe + (1 - beta) p_child, beta = sigmoid(L). Nodes come from a
    /// preallocated pool, so updates never allocate.
    /// </summary>
    public sealed class CtwTree
    {
        /// <summary>Context depth in symbols (3 exchanges of action and response).</summary>
        public const int Depth = 6;

        /// <summary>The log-odds clamp.</summary>
        public const double Clamp = 6.0;

        /// <summary>The KT estimator's prior per symbol.</summary>
        public const double Kt = 0.5;

        private readonly int symbols;
        private readonly int branching;
        private readonly double gamma;
        private readonly double[] counts;
        private readonly double[] totals;
        private readonly double[] logOdds;
        private readonly int[] children;
        private readonly int[] path = new int[Depth + 1];
        private readonly double[] childPredictive = new double[Depth + 1];
        private int nodeCount;

        /// <summary>Creates a tree predicting one of <paramref name="symbols"/> symbols.</summary>
        /// <param name="symbols">Size of the predicted alphabet (actions, or responses for the mirror tree).</param>
        /// <param name="branching">Context symbols per level, including the Match-start symbol.</param>
        /// <param name="gamma">Per-visit fading of counts and log-odds.</param>
        /// <param name="maxUpdates">The most updates the tree will see (sizes the node pool).</param>
        public CtwTree(int symbols, int branching, double gamma, int maxUpdates)
        {
            this.symbols = symbols;
            this.branching = branching;
            this.gamma = gamma;
            int capacity = maxUpdates * Depth + 1;
            counts = new double[capacity * symbols];
            totals = new double[capacity];
            logOdds = new double[capacity];
            children = new int[capacity * branching];
            for (int i = 0; i < children.Length; i++)
            {
                children[i] = -1;
            }
            nodeCount = 1;
        }

        /// <summary>Nodes in use, including the root.</summary>
        public int NodeCount => nodeCount;

        /// <summary>
        /// Writes the weighted prediction for the context <paramref name="context"/> (most recent symbol first) into
        /// <paramref name="forecast"/>, and the node path into <paramref name="nodes"/> (-1 past the deepest existing
        /// node). Returns the depth of the deepest existing node. Does not change the tree.
        /// </summary>
        public int Predict(int[] context, double[] forecast, int[] nodes)
        {
            int deepest = Walk(context, nodes, false);
            int start;
            if (deepest == Depth)
            {
                Estimate(nodes[Depth], forecast);
                start = Depth - 1;
            }
            else
            {
                // Below the deepest existing node every node is fresh, and a fresh subtree predicts uniform.
                for (int x = 0; x < symbols; x++)
                {
                    forecast[x] = 1.0 / symbols;
                }
                start = deepest;
            }
            for (int d = start; d >= 0; d--)
            {
                int node = nodes[d];
                double beta = Sigmoid(logOdds[node]);
                double denominator = totals[node] + symbols * Kt;
                int row = node * symbols;
                for (int x = 0; x < symbols; x++)
                {
                    forecast[x] = beta * (counts[row + x] + Kt) / denominator + (1 - beta) * forecast[x];
                }
            }
            return deepest;
        }

        /// <summary>Learns that <paramref name="symbol"/> followed <paramref name="context"/>.</summary>
        public void Update(int[] context, int symbol)
        {
            Walk(context, path, true);
            childPredictive[Depth] = EstimateOf(path[Depth], symbol);
            for (int d = Depth - 1; d >= 0; d--)
            {
                int node = path[d];
                double pe = EstimateOf(node, symbol);
                double beta = Sigmoid(logOdds[node]);
                childPredictive[d] = beta * pe + (1 - beta) * childPredictive[d + 1];
                double updated = gamma * (logOdds[node] + Math.Log(pe) - Math.Log(childPredictive[d + 1]));
                logOdds[node] = Math.Max(-Clamp, Math.Min(Clamp, updated));
            }
            for (int d = 0; d <= Depth; d++)
            {
                int node = path[d];
                int row = node * symbols;
                for (int x = 0; x < symbols; x++)
                {
                    counts[row + x] *= gamma;
                }
                totals[node] = gamma * totals[node] + 1;
                counts[row + symbol] += 1;
            }
        }

        /// <summary>At match start: counts x <paramref name="countScale"/>, log-odds x <paramref name="oddsScale"/>.</summary>
        public void Scale(double countScale, double oddsScale)
        {
            for (int i = 0; i < nodeCount * symbols; i++)
            {
                counts[i] *= countScale;
            }
            for (int n = 0; n < nodeCount; n++)
            {
                totals[n] *= countScale;
                logOdds[n] *= oddsScale;
            }
        }

        /// <summary>The mixing weight beta of a node.</summary>
        public double Beta(int node) => Sigmoid(logOdds[node]);

        /// <summary>The observed faded count of all symbols at a node.</summary>
        public double Total(int node) => totals[node];

        /// <summary>The KT estimate of every symbol at a node.</summary>
        public void Estimate(int node, double[] forecast)
        {
            double denominator = totals[node] + symbols * Kt;
            int row = node * symbols;
            for (int x = 0; x < symbols; x++)
            {
                forecast[x] = (counts[row + x] + Kt) / denominator;
            }
        }

        /// <summary>One draw from a node's KT (Dirichlet 1/2) posterior.</summary>
        public void Sample(int node, Rng rng, double[] alphaBuffer, double[] sample)
        {
            int row = node * symbols;
            for (int x = 0; x < symbols; x++)
            {
                alphaBuffer[x] = counts[row + x] + Kt;
            }
            rng.NextDirichlet(alphaBuffer, sample, symbols);
        }

        /// <summary>
        /// One Thompson draw along a predicted path: a depth is drawn with the beta weights the prediction used, then
        /// that node's KT posterior is sampled (a fresh subtree below the deepest node samples the bare KT prior).
        /// </summary>
        public void SamplePath(int[] nodes, int deepest, Rng rng, double[] alphaBuffer, double[] sample)
        {
            double u = rng.NextDouble();
            double remaining = 1;
            for (int d = 0; d <= deepest; d++)
            {
                double take = d == Depth ? remaining : remaining * Beta(nodes[d]);
                if (u < take)
                {
                    Sample(nodes[d], rng, alphaBuffer, sample);
                    return;
                }
                u -= take;
                remaining -= take;
            }
            for (int x = 0; x < symbols; x++)
            {
                alphaBuffer[x] = Kt;
            }
            rng.NextDirichlet(alphaBuffer, sample, symbols);
        }

        /// <summary>The log-odds update of one node, exposed for the beta-crossing unit test.</summary>
        public static double UpdateLogOdds(double logOdds, double logOwn, double logChild, double gamma)
        {
            return Math.Max(-Clamp, Math.Min(Clamp, gamma * (logOdds + logOwn - logChild)));
        }

        private double EstimateOf(int node, int symbol)
        {
            return (counts[node * symbols + symbol] + Kt) / (totals[node] + symbols * Kt);
        }

        private int Walk(int[] context, int[] nodes, bool create)
        {
            nodes[0] = 0;
            int deepest = 0;
            for (int d = 1; d <= Depth; d++)
            {
                int slot = nodes[d - 1] * branching + context[d - 1];
                if (children[slot] < 0)
                {
                    if (!create)
                    {
                        for (int rest = d; rest <= Depth; rest++)
                        {
                            nodes[rest] = -1;
                        }
                        return deepest;
                    }
                    children[slot] = nodeCount++;
                }
                nodes[d] = children[slot];
                deepest = d;
            }
            return deepest;
        }

        private static double Sigmoid(double x) => 1.0 / (1.0 + Math.Exp(-x));
    }
}
