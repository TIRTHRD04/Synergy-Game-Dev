using System;

namespace AdaptiveOpponent.Core
{
    /// <summary>Which slice of recent history a count expert looks at (ADR 0004).</summary>
    public enum CountContext
    {
        /// <summary>No context: plain action frequencies.</summary>
        Order0,
        /// <summary>The previous action.</summary>
        Order1,
        /// <summary>The two previous actions.</summary>
        Order2,
        /// <summary>The previous action and the previous response.</summary>
        ActionResponse,
    }

    /// <summary>A v1 expert: faded Dirichlet counts of the next action in one kind of context.</summary>
    public sealed class CountExpert : IExpert
    {
        private readonly CountContext kind;
        private readonly CountTable table;
        private readonly double matchStartScale;
        private readonly double[] alphaBuffer;
        private readonly int symbols;
        private int lastContext;

        /// <summary>Creates the expert for <paramref name="kind"/> with the config's prior and fading.</summary>
        public CountExpert(CountContext kind, GameDefinition definition, OpponentConfig config)
        {
            this.kind = kind;
            symbols = definition.ActionCount + 1;
            int contexts;
            switch (kind)
            {
                case CountContext.Order0: contexts = 1; break;
                case CountContext.Order1: contexts = symbols; break;
                case CountContext.Order2: contexts = symbols * symbols; break;
                case CountContext.ActionResponse: contexts = symbols * (definition.ResponseCount + 1); break;
                default: throw new ArgumentOutOfRangeException(nameof(kind));
            }
            table = new CountTable(contexts, definition.ActionCount, config.Dirichlet, config.Gamma, config.GlobalFading);
            matchStartScale = config.MatchStartCountScale;
            alphaBuffer = new double[definition.ActionCount];
        }

        /// <inheritdoc />
        public string Name
        {
            get
            {
                switch (kind)
                {
                    case CountContext.Order0: return "order0";
                    case CountContext.Order1: return "order1";
                    case CountContext.Order2: return "order2";
                    default: return "actionResponse";
                }
            }
        }

        /// <inheritdoc />
        public bool Predict(ExchangeHistory history, double[] forecast)
        {
            lastContext = ContextBefore(history, history.Count);
            table.Mean(lastContext, forecast);
            return true;
        }

        /// <inheritdoc />
        public void Sample(Rng rng, double[] sample) => table.Sample(lastContext, rng, alphaBuffer, sample);

        /// <inheritdoc />
        public void Observe(ExchangeHistory history)
        {
            int last = history.Count - 1;
            table.Add(ContextBefore(history, last), history.Action(last));
        }

        /// <inheritdoc />
        public void StartMatch() => table.Scale(matchStartScale);

        /// <inheritdoc />
        public bool Explain(ExchangeHistory history, int topAction, out Insight insight)
        {
            insight = default;
            insight.Action = topAction;
            insight.Support = table.Total(lastContext);
            int now = history.Count;
            insight.Prev1 = history.PrevActionBefore(now);
            insight.Prev2 = history.SecondPrevActionBefore(now);
            insight.PrevResponse = history.PrevResponseBefore(now);
            bool atStart = history.IsMatchStart(now);
            switch (kind)
            {
                case CountContext.Order0: insight.Kind = InsightKind.Frequency; break;
                case CountContext.Order1: insight.Kind = atStart ? InsightKind.MatchStart : InsightKind.AfterAction; break;
                case CountContext.Order2:
                    insight.Kind = atStart ? InsightKind.MatchStart
                        : insight.Prev2 == history.MatchStartAction ? InsightKind.AfterAction : InsightKind.AfterTwoActions;
                    break;
                default: insight.Kind = atStart ? InsightKind.MatchStart : InsightKind.AfterActionAndResponse; break;
            }
            return true;
        }

        private int ContextBefore(ExchangeHistory history, int index)
        {
            switch (kind)
            {
                case CountContext.Order0: return 0;
                case CountContext.Order1: return history.PrevActionBefore(index);
                case CountContext.Order2: return history.SecondPrevActionBefore(index) * symbols + history.PrevActionBefore(index);
                default: return history.PrevActionBefore(index) * (history.MatchStartResponse + 1) + history.PrevResponseBefore(index);
            }
        }
    }
}
