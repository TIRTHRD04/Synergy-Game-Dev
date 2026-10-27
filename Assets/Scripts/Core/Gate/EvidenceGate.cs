using System;

namespace AdaptiveOpponent.Core
{
    /// <summary>
    /// The evidence gate: the cumulative log-likelihood ratio of the mixed forecast against a uniform forecast over
    /// the session. Each forecast is fixed before the action, so exp(score) is a test martingale under a uniformly
    /// random player, and by Ville's inequality the chance it ever reaches exp(threshold) is at most exp(-threshold).
    /// Once open, it stays open for the rest of the session.
    /// </summary>
    public sealed class EvidenceGate
    {
        private readonly double threshold;
        private readonly double logUniform;

        /// <summary>Creates a closed gate with the given threshold (natural log; ln 1000 by default in configs).</summary>
        public EvidenceGate(double threshold, int actions)
        {
            this.threshold = threshold;
            logUniform = Math.Log(1.0 / actions);
        }

        /// <summary>The cumulative log-likelihood ratio so far.</summary>
        public double Score { get; private set; }

        /// <summary>Whether the gate has opened this session.</summary>
        public bool IsOpen { get; private set; }

        /// <summary>Adds the evidence of one exchange: the mixed forecast's probability of the action played.</summary>
        public void Observe(double forecastOfAction)
        {
            Score += Math.Log(Math.Max(forecastOfAction, 1e-300)) - logUniform;
            if (Score >= threshold)
            {
                IsOpen = true;
            }
        }
    }
}
