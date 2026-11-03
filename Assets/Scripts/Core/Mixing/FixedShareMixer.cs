namespace AdaptiveOpponent.Core
{
    /// <summary>
    /// Mixes expert forecasts by Fixed Share (Herbster and Warmuth, 1998) with abstaining experts (Freund, Schapire,
    /// Singer and Warmuth, 1997). The mixed forecast averages the awake experts by weight. After the action, awake
    /// weights are multiplied by p_i(a) / p_mix(a), which keeps their total; abstaining weights do not change. Then
    /// w &lt;- (1 - alpha) w + alpha / K over all K experts.
    /// </summary>
    public sealed class FixedShareMixer
    {
        private readonly double[] weights;
        private readonly double alpha;
        private readonly double matchStartMix;
        private readonly int actions;

        /// <summary>Creates a mixer over <paramref name="experts"/> experts with equal starting weights.</summary>
        public FixedShareMixer(int experts, int actions, double alpha, double matchStartMix)
        {
            weights = new double[experts];
            for (int i = 0; i < experts; i++)
            {
                weights[i] = 1.0 / experts;
            }
            this.alpha = alpha;
            this.matchStartMix = matchStartMix;
            this.actions = actions;
        }

        /// <summary>Number of experts.</summary>
        public int Count => weights.Length;

        /// <summary>The current weight of expert <paramref name="i"/> (all weights sum to 1).</summary>
        public double Weight(int i) => weights[i];

        /// <summary>
        /// Writes the weighted average of the awake experts' forecasts into <paramref name="mixed"/>. If every expert
        /// abstains, the mixed forecast is uniform.
        /// </summary>
        public void Mix(bool[] awake, double[][] forecasts, double[] mixed)
        {
            double total = 0;
            for (int a = 0; a < actions; a++)
            {
                mixed[a] = 0;
            }
            for (int i = 0; i < weights.Length; i++)
            {
                if (!awake[i])
                {
                    continue;
                }
                total += weights[i];
                double[] f = forecasts[i];
                for (int a = 0; a < actions; a++)
                {
                    mixed[a] += weights[i] * f[a];
                }
            }
            for (int a = 0; a < actions; a++)
            {
                mixed[a] = total > 0 ? mixed[a] / total : 1.0 / actions;
            }
        }

        /// <summary>Updates the weights after <paramref name="action"/>, given the forecasts used by <see cref="Mix"/>.</summary>
        public void Update(bool[] awake, double[][] forecasts, double[] mixed, int action)
        {
            double pMix = mixed[action];
            if (pMix > 0)
            {
                for (int i = 0; i < weights.Length; i++)
                {
                    if (awake[i])
                    {
                        weights[i] *= forecasts[i][action] / pMix;
                    }
                }
            }
            Normalise();
            double share = alpha / weights.Length;
            for (int i = 0; i < weights.Length; i++)
            {
                weights[i] = (1 - alpha) * weights[i] + share;
            }
        }

        /// <summary>At match start: w &lt;- (1 - mix) w + mix / K.</summary>
        public void StartMatch()
        {
            double share = matchStartMix / weights.Length;
            for (int i = 0; i < weights.Length; i++)
            {
                weights[i] = (1 - matchStartMix) * weights[i] + share;
            }
        }

        /// <summary>The awake expert with the largest weight (ties to the lower index), or -1 if all abstain.</summary>
        public int Leading(bool[] awake)
        {
            int best = -1;
            for (int i = 0; i < weights.Length; i++)
            {
                if (awake[i] && (best < 0 || weights[i] > weights[best]))
                {
                    best = i;
                }
            }
            return best;
        }

        /// <summary>Draws an awake expert with probability proportional to its weight (Thompson Sampling).</summary>
        public int DrawAwake(bool[] awake, Rng rng, double[] buffer)
        {
            for (int i = 0; i < weights.Length; i++)
            {
                buffer[i] = awake[i] ? weights[i] : 0;
            }
            return rng.NextWeighted(buffer, weights.Length);
        }

        private void Normalise()
        {
            double total = 0;
            for (int i = 0; i < weights.Length; i++)
            {
                total += weights[i];
            }
            for (int i = 0; i < weights.Length; i++)
            {
                weights[i] = total > 0 ? weights[i] / total : 1.0 / weights.Length;
            }
        }
    }
}
