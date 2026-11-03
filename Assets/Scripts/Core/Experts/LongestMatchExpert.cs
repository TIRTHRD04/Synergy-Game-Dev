namespace AdaptiveOpponent.Core
{
    /// <summary>
    /// The longest-match expert (spec section 2): finds the longest suffix (at most 20 exchanges) of the session's
    /// joint (action, response) history that occurred earlier, and predicts a Dirichlet(1/2) posterior over the actions
    /// that followed every earlier occurrence. With no match it predicts uniform. About n x 20 comparisons per exchange.
    /// </summary>
    public sealed class LongestMatchExpert : IExpert
    {
        /// <summary>The longest suffix compared, in exchanges.</summary>
        public const int MaxLength = 20;

        /// <summary>The Dirichlet prior per action.</summary>
        public const double Prior = 0.5;

        private readonly int actions;
        private readonly double[] counts;
        private readonly double[] alphaBuffer;
        private double occurrences;

        /// <summary>Creates the expert.</summary>
        public LongestMatchExpert(GameDefinition definition)
        {
            actions = definition.ActionCount;
            counts = new double[actions];
            alphaBuffer = new double[actions];
        }

        /// <inheritdoc />
        public string Name => "longestMatch";

        /// <summary>The length of the suffix matched for the last prediction (0 = none).</summary>
        public int MatchLength { get; private set; }

        /// <inheritdoc />
        public bool Predict(ExchangeHistory history, double[] forecast)
        {
            for (int a = 0; a < actions; a++)
            {
                counts[a] = 0;
            }
            occurrences = 0;
            MatchLength = 0;
            int last = history.Count - 1;
            for (int j = 0; j < last; j++)
            {
                int length = 0;
                while (length < MaxLength && j - length >= 0
                    && history.Action(j - length) == history.Action(last - length)
                    && history.Response(j - length) == history.Response(last - length))
                {
                    length++;
                }
                if (length == 0 || length < MatchLength)
                {
                    continue;
                }
                if (length > MatchLength)
                {
                    MatchLength = length;
                    for (int a = 0; a < actions; a++)
                    {
                        counts[a] = 0;
                    }
                    occurrences = 0;
                }
                counts[history.Action(j + 1)] += 1;
                occurrences += 1;
            }
            double total = occurrences + actions * Prior;
            for (int a = 0; a < actions; a++)
            {
                forecast[a] = (counts[a] + Prior) / total;
            }
            return true;
        }

        /// <inheritdoc />
        public void Sample(Rng rng, double[] sample)
        {
            for (int a = 0; a < actions; a++)
            {
                alphaBuffer[a] = counts[a] + Prior;
            }
            rng.NextDirichlet(alphaBuffer, sample, actions);
        }

        /// <inheritdoc />
        public void Observe(ExchangeHistory history)
        {
        }

        /// <inheritdoc />
        public void StartMatch()
        {
        }

        /// <summary>"You're repeating a sequence from earlier", supported by the earlier occurrences of the suffix.</summary>
        public bool Explain(ExchangeHistory history, int topAction, out Insight insight)
        {
            insight = default;
            if (MatchLength == 0)
            {
                return false;
            }
            insight.Kind = InsightKind.RepeatSequence;
            insight.Action = topAction;
            insight.Support = occurrences;
            return true;
        }
    }
}
