namespace AdaptiveOpponent.Core
{
    /// <summary>
    /// The Big-hit sequence expert (ADR 0011): when the Start context of a big hit the player landed comes back
    /// (exact, or broad when allowed), it expects the Opening again, then each Follow-up while the player follows.
    /// Forecast: Dirichlet 0.5 plus s x strength (s/2 x strength when broad) on each predicted action. A miss
    /// multiplies the chain's strength by 0.7 and ends the chain; a hit resets it to 1. Abstains when nothing fires.
    /// </summary>
    public sealed class BigHitSequenceExpert : IExpert
    {
        /// <summary>Strength multiplier on a miss.</summary>
        public const double MissFactor = 0.7;

        /// <summary>Broad matches add this fraction of the exact pseudo-counts.</summary>
        public const double BroadRatio = 0.5;

        /// <summary>The symmetric Dirichlet prior per action.</summary>
        public const double Prior = 0.5;

        private readonly BigHitPool pool;
        private readonly double strength;
        private readonly bool broad;
        private readonly int actions;
        private readonly double[] pseudo;
        private readonly double[] alphaBuffer;

        /// <summary>Creates the expert over the Opponent's Big-hit memory.</summary>
        public BigHitSequenceExpert(GameDefinition definition, BigHitPool pool, double strength, bool broadMatches)
        {
            this.pool = pool;
            this.strength = strength;
            broad = broadMatches;
            actions = definition.ActionCount;
            pseudo = new double[actions];
            alphaBuffer = new double[actions];
        }

        /// <inheritdoc />
        public string Name => "bigHitSequence";

        /// <inheritdoc />
        public bool Predict(ExchangeHistory history, double[] forecast)
        {
            int now = history.Count;
            int prevAction = history.PrevActionBefore(now);
            int prevResponse = history.PrevResponseBefore(now);
            bool any = false;
            for (int a = 0; a < actions; a++)
            {
                pseudo[a] = 0;
            }
            for (int s = 0; s < BigHitPool.Size; s++)
            {
                pool.FiredType[s] = MatchType.None;
                if (!pool.Live[s])
                {
                    continue;
                }
                MatchType type;
                int position;
                if (pool.ChainPosition[s] > 0)
                {
                    type = pool.ChainType[s];
                    position = pool.ChainPosition[s];
                }
                else
                {
                    type = pool.MatchOf(s, prevAction, prevResponse, broad);
                    position = 0;
                }
                if (type == MatchType.None)
                {
                    continue;
                }
                int predicted = pool.ActionAt(s, position);
                pool.FiredType[s] = type;
                pool.FiredAction[s] = predicted;
                pool.FiredPosition[s] = position;
                pseudo[predicted] += Weight(s, type);
                any = true;
            }
            if (!any)
            {
                return false;
            }
            double total = actions * Prior;
            for (int a = 0; a < actions; a++)
            {
                total += pseudo[a];
            }
            for (int a = 0; a < actions; a++)
            {
                forecast[a] = (Prior + pseudo[a]) / total;
            }
            return true;
        }

        /// <inheritdoc />
        public void Sample(Rng rng, double[] sample)
        {
            for (int a = 0; a < actions; a++)
            {
                alphaBuffer[a] = Prior + pseudo[a];
            }
            rng.NextDirichlet(alphaBuffer, sample, actions);
        }

        /// <inheritdoc />
        public void Observe(ExchangeHistory history)
        {
            int action = history.Action(history.Count - 1);
            for (int s = 0; s < BigHitPool.Size; s++)
            {
                MatchType type = pool.FiredType[s];
                if (type == MatchType.None)
                {
                    continue;
                }
                pool.FiredType[s] = MatchType.None;
                if (action == pool.FiredAction[s])
                {
                    pool.SetStrength(s, type, 1.0);
                    int next = pool.FiredPosition[s] + 1;
                    bool more = next < pool.StoredActions;
                    pool.ChainPosition[s] = more ? next : 0;
                    pool.ChainType[s] = type;
                }
                else
                {
                    pool.ChainPosition[s] = 0;
                    pool.SetStrength(s, type, pool.Strength(s, type) * MissFactor);
                }
            }
        }

        /// <inheritdoc />
        public void StartMatch() => pool.EndChains();

        /// <inheritdoc />
        public bool Explain(ExchangeHistory history, int topAction, out Insight insight)
        {
            insight = default;
            int best = -1;
            for (int s = 0; s < BigHitPool.Size; s++)
            {
                if (pool.FiredType[s] != MatchType.None && pool.FiredAction[s] == topAction
                    && (best < 0 || Weight(s, pool.FiredType[s]) > Weight(best, pool.FiredType[best])))
                {
                    best = s;
                }
            }
            if (best < 0)
            {
                return false;
            }
            int position = pool.FiredPosition[best];
            insight.Kind = position == 0 ? InsightKind.RecallOpening : InsightKind.RecallFollowUp;
            insight.Action = topAction;
            insight.Prev1 = position == 0 ? pool.PrevAction[best] : pool.ActionAt(best, position - 1);
            insight.PrevResponse = pool.PrevResponse[best];
            insight.Support = pool.Strength(best, pool.FiredType[best]);
            return true;
        }

        private double Weight(int slot, MatchType type)
        {
            double scale = type == MatchType.Exact ? strength : strength * BroadRatio;
            return scale * pool.Strength(slot, type);
        }
    }
}
