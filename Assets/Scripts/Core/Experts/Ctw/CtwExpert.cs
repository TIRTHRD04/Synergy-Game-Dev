namespace AdaptiveOpponent.Core
{
    /// <summary>The order of context symbols for a CTW tree.</summary>
    public enum CtwOrder
    {
        /// <summary>a(t-1), r(t-1), a(t-2), r(t-2), ...: "repeats the last action" is a depth-1 node.</summary>
        ActionFirst,
        /// <summary>r(t-1), a(t-1), r(t-2), a(t-2), ...: "reacts to my last response" is a depth-1 node.</summary>
        ResponseFirst,
    }

    /// <summary>Builds CTW contexts from the exchange history. The current response is never in the context.</summary>
    public static class CtwContext
    {
        /// <summary>Fills <paramref name="context"/> (most recent first) for exchange <paramref name="index"/>.</summary>
        public static void Fill(ExchangeHistory history, int index, CtwOrder order, int[] context)
        {
            for (int k = 0; k < CtwTree.Depth / 2; k++)
            {
                int action = history.ActionBack(index, k + 1);
                int response = history.ResponseBack(index, k + 1);
                context[2 * k] = order == CtwOrder.ActionFirst ? action : response;
                context[2 * k + 1] = order == CtwOrder.ActionFirst ? response : action;
            }
        }
    }

    /// <summary>
    /// A v2 CTW expert over the player's actions (ADR 0007). Thompson Sampling draws a depth on the current path with
    /// the beta weights, then samples that node's KT posterior.
    /// </summary>
    public sealed class CtwExpert : IExpert
    {
        private readonly CtwOrder order;
        private readonly CtwTree tree;
        private readonly double matchStartScale;
        private readonly double insightMinCount;
        private readonly int[] context = new int[CtwTree.Depth];
        private readonly int[] nodes = new int[CtwTree.Depth + 1];
        private readonly double[] alphaBuffer;
        private int deepest;

        /// <summary>Creates the expert with the config's fading.</summary>
        public CtwExpert(CtwOrder order, GameDefinition definition, OpponentConfig config, int historyCapacity)
        {
            this.order = order;
            int actions = definition.ActionCount;
            tree = new CtwTree(actions, actions + 1, config.Gamma, historyCapacity);
            matchStartScale = config.MatchStartCountScale;
            insightMinCount = config.InsightMinCount;
            alphaBuffer = new double[actions];
        }

        /// <inheritdoc />
        public string Name => order == CtwOrder.ActionFirst ? "ctwActionFirst" : "ctwResponseFirst";

        /// <inheritdoc />
        public bool Predict(ExchangeHistory history, double[] forecast)
        {
            CtwContext.Fill(history, history.Count, order, context);
            deepest = tree.Predict(context, forecast, nodes);
            return true;
        }

        /// <inheritdoc />
        public void Sample(Rng rng, double[] sample) => tree.SamplePath(nodes, deepest, rng, alphaBuffer, sample);

        /// <inheritdoc />
        public void Observe(ExchangeHistory history)
        {
            int last = history.Count - 1;
            CtwContext.Fill(history, last, order, context);
            tree.Update(context, history.Action(last));
        }

        /// <inheritdoc />
        public void StartMatch() => tree.Scale(matchStartScale, matchStartScale);

        /// <summary>Describes the deepest node of depth 2 or less with enough observed counts.</summary>
        public bool Explain(ExchangeHistory history, int topAction, out Insight insight)
        {
            insight = default;
            int depth = System.Math.Min(2, deepest);
            while (depth > 0 && tree.Total(nodes[depth]) < insightMinCount)
            {
                depth--;
            }
            insight.Action = topAction;
            insight.Support = tree.Total(nodes[depth]);
            int now = history.Count;
            insight.Prev1 = history.ActionBack(now, 1);
            insight.PrevResponse = history.ResponseBack(now, 1);
            if (depth == 0)
            {
                insight.Kind = InsightKind.Frequency;
            }
            else if (history.IsMatchStart(now))
            {
                insight.Kind = InsightKind.MatchStart;
            }
            else if (depth == 2)
            {
                insight.Kind = InsightKind.AfterActionAndResponse;
            }
            else
            {
                insight.Kind = order == CtwOrder.ActionFirst ? InsightKind.AfterAction : InsightKind.AfterResponse;
            }
            return true;
        }
    }
}
