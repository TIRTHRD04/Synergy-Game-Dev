using System;

namespace AdaptiveOpponent.Core
{
    /// <summary>
    /// The Avoidance expert (ADR 0011): where the Opponent landed a big hit on the player, it expects the player to
    /// avoid that big hit's Opening when the Start context comes back (exact, or broad when allowed). Forecast:
    /// p(a1) proportional to exp(-beta x strength), the other actions equal, smoothed by eps = 0.05. Playing a1
    /// anyway multiplies the strength by 0.7; avoiding it leaves the strength unchanged. Abstains when nothing fires.
    /// </summary>
    public sealed class AvoidanceExpert : IExpert
    {
        /// <summary>Strength multiplier when the player plays the Opening anyway.</summary>
        public const double MissFactor = 0.7;

        /// <summary>Smoothing toward uniform.</summary>
        public const double Epsilon = 0.05;

        private readonly BigHitPool pool;
        private readonly double beta;
        private readonly bool broad;
        private readonly int actions;
        private readonly double[] last;

        /// <summary>Creates the expert over the Opponent's Avoidance record.</summary>
        public AvoidanceExpert(GameDefinition definition, BigHitPool pool, double beta, bool broadMatches)
        {
            this.pool = pool;
            this.beta = beta;
            broad = broadMatches;
            actions = definition.ActionCount;
            last = new double[actions];
        }

        /// <inheritdoc />
        public string Name => "avoidance";

        /// <inheritdoc />
        public bool Predict(ExchangeHistory history, double[] forecast)
        {
            int now = history.Count;
            int prevAction = history.PrevActionBefore(now);
            int prevResponse = history.PrevResponseBefore(now);
            bool any = false;
            for (int a = 0; a < actions; a++)
            {
                forecast[a] = 1;
            }
            for (int s = 0; s < BigHitPool.Size; s++)
            {
                MatchType type = pool.MatchOf(s, prevAction, prevResponse, broad);
                pool.FiredType[s] = type;
                if (type == MatchType.None)
                {
                    continue;
                }
                int opening = pool.ActionAt(s, 0);
                pool.FiredAction[s] = opening;
                forecast[opening] *= Math.Exp(-beta * pool.Strength(s, type));
                any = true;
            }
            if (!any)
            {
                return false;
            }
            double total = 0;
            for (int a = 0; a < actions; a++)
            {
                total += forecast[a];
            }
            for (int a = 0; a < actions; a++)
            {
                forecast[a] = (1 - Epsilon) * forecast[a] / total + Epsilon / actions;
                last[a] = forecast[a];
            }
            return true;
        }

        /// <summary>The forecast is a fixed transform of the record, so a Thompson draw is the forecast itself.</summary>
        public void Sample(Rng rng, double[] sample)
        {
            for (int a = 0; a < actions; a++)
            {
                sample[a] = last[a];
            }
        }

        /// <inheritdoc />
        public void Observe(ExchangeHistory history)
        {
            int action = history.Action(history.Count - 1);
            for (int s = 0; s < BigHitPool.Size; s++)
            {
                MatchType type = pool.FiredType[s];
                pool.FiredType[s] = MatchType.None;
                if (type != MatchType.None && action == pool.FiredAction[s])
                {
                    pool.SetStrength(s, type, pool.Strength(s, type) * MissFactor);
                }
            }
        }

        /// <inheritdoc />
        public void StartMatch()
        {
        }

        /// <summary>The Avoidance expert predicts what the player will not do, so it offers no Insight.</summary>
        public bool Explain(ExchangeHistory history, int topAction, out Insight insight)
        {
            insight = default;
            return false;
        }
    }
}
