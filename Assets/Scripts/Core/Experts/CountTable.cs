namespace AdaptiveOpponent.Core
{
    /// <summary>
    /// Faded action counts per context with a symmetric Dirichlet prior (ADR 0004). Per-visit fading multiplies a
    /// context's counts by gamma each time that context is visited, before the new count is added; global fading
    /// multiplies every context each exchange (an ablation row).
    /// </summary>
    public sealed class CountTable
    {
        private readonly double[] counts;
        private readonly double[] totals;
        private readonly int actions;
        private readonly double dirichlet;
        private readonly double gamma;
        private readonly bool globalFading;

        /// <summary>Creates an all-zero table.</summary>
        public CountTable(int contexts, int actions, double dirichlet, double gamma, bool globalFading)
        {
            counts = new double[contexts * actions];
            totals = new double[contexts];
            this.actions = actions;
            this.dirichlet = dirichlet;
            this.gamma = gamma;
            this.globalFading = globalFading;
        }

        /// <summary>The observed faded count of <paramref name="action"/> in <paramref name="context"/>.</summary>
        public double Count(int context, int action) => counts[context * actions + action];

        /// <summary>The observed faded count of all actions in <paramref name="context"/>.</summary>
        public double Total(int context) => totals[context];

        /// <summary>The posterior mean: (count + dirichlet) / (total + actions x dirichlet).</summary>
        public void Mean(int context, double[] forecast)
        {
            double denominator = totals[context] + actions * dirichlet;
            int row = context * actions;
            for (int a = 0; a < actions; a++)
            {
                forecast[a] = (counts[row + a] + dirichlet) / denominator;
            }
        }

        /// <summary>One draw from the Dirichlet posterior of <paramref name="context"/>.</summary>
        public void Sample(int context, Rng rng, double[] alphaBuffer, double[] sample)
        {
            int row = context * actions;
            for (int a = 0; a < actions; a++)
            {
                alphaBuffer[a] = counts[row + a] + dirichlet;
            }
            rng.NextDirichlet(alphaBuffer, sample, actions);
        }

        /// <summary>Fades (per visit, or everything when fading is global) and then counts the action.</summary>
        public void Add(int context, int action)
        {
            if (globalFading)
            {
                Scale(gamma);
            }
            else
            {
                ScaleContext(context, gamma);
            }
            counts[context * actions + action] += 1;
            totals[context] += 1;
        }

        /// <summary>Multiplies every count by <paramref name="factor"/> (used at match start).</summary>
        public void Scale(double factor)
        {
            for (int i = 0; i < counts.Length; i++)
            {
                counts[i] *= factor;
            }
            for (int c = 0; c < totals.Length; c++)
            {
                totals[c] *= factor;
            }
        }

        private void ScaleContext(int context, double factor)
        {
            int row = context * actions;
            for (int a = 0; a < actions; a++)
            {
                counts[row + a] *= factor;
            }
            totals[context] *= factor;
        }
    }
}
