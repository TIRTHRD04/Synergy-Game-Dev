namespace AdaptiveOpponent.Core
{
    /// <summary>
    /// Level-k meta-experts in the style of Iocaine Powder (Egnor, 2000; spec section 3). Level 1 assumes the player
    /// expects the Opponent's best response to a base forecast and plays one of the two actions that beat it; level 2
    /// applies the same step to the level-1 forecast. Every forecast is smoothed toward uniform by eps = 0.05.
    /// The base expert must come earlier in the config's expert list; its forecast is read from the shared board.
    /// </summary>
    public sealed class LevelKExpert : IExpert
    {
        /// <summary>Smoothing toward uniform.</summary>
        public const double Epsilon = 0.05;

        private readonly GameDefinition definition;
        private readonly IExpert baseExpert;
        private readonly int baseIndex;
        private readonly int level;
        private readonly double[][] board;
        private readonly bool[] awakeBoard;
        private readonly double gamma;
        private readonly double[] buffer;
        private int lastTop;
        private double hits;

        /// <summary>Creates a level-<paramref name="level"/> expert over expert <paramref name="baseIndex"/>.</summary>
        public LevelKExpert(GameDefinition definition, IExpert baseExpert, int baseIndex, int level,
            double[][] board, bool[] awakeBoard, double gamma)
        {
            this.definition = definition;
            this.baseExpert = baseExpert;
            this.baseIndex = baseIndex;
            this.level = level;
            this.board = board;
            this.awakeBoard = awakeBoard;
            this.gamma = gamma;
            buffer = new double[definition.ActionCount];
        }

        /// <inheritdoc />
        public string Name => "levelK" + level + ":" + baseExpert.Name;

        /// <inheritdoc />
        public bool Predict(ExchangeHistory history, double[] forecast)
        {
            if (!awakeBoard[baseIndex])
            {
                return false;
            }
            Transform(definition, board[baseIndex], level, forecast);
            lastTop = BestResponse.TopAction(forecast, definition.ActionCount);
            return true;
        }

        /// <summary>A Thompson draw: the same transform applied to a draw from the base expert.</summary>
        public void Sample(Rng rng, double[] sample)
        {
            baseExpert.Sample(rng, buffer);
            Transform(definition, buffer, level, sample);
        }

        /// <inheritdoc />
        public void Observe(ExchangeHistory history)
        {
            hits = gamma * hits + (history.Action(history.Count - 1) == lastTop ? 1 : 0);
        }

        /// <inheritdoc />
        public void StartMatch()
        {
        }

        /// <summary>"You're trying to beat my counter", supported by the faded count of exchanges it called right.</summary>
        public bool Explain(ExchangeHistory history, int topAction, out Insight insight)
        {
            insight = default;
            insight.Kind = InsightKind.BeatMyCounter;
            insight.Action = topAction;
            insight.Support = hits;
            return true;
        }

        /// <summary>
        /// Applies the level step <paramref name="levels"/> times: the player plays, with equal chance, one of the two
        /// actions that beat the Opponent's best response to the current forecast. Then smooths by eps.
        /// <paramref name="source"/> and <paramref name="result"/> may not be the same array.
        /// </summary>
        public static void Transform(GameDefinition definition, double[] source, int levels, double[] result)
        {
            int n = definition.ActionCount;
            for (int a = 0; a < n; a++)
            {
                result[a] = source[a];
            }
            for (int l = 0; l < levels; l++)
            {
                BeatersOf(definition, BestResponse.Of(definition, result), result);
            }
            Smooth(result, n);
        }

        /// <summary>Writes the uniform forecast over the two actions that beat <paramref name="response"/>.</summary>
        public static void BeatersOf(GameDefinition definition, int response, double[] forecast)
        {
            for (int a = 0; a < definition.ActionCount; a++)
            {
                forecast[a] = 0;
            }
            forecast[definition.HeavyBeater(response)] = 0.5;
            forecast[definition.LightBeater(response)] = 0.5;
        }

        /// <summary>p &lt;- (1 - eps) p + eps / n.</summary>
        public static void Smooth(double[] forecast, int n)
        {
            for (int a = 0; a < n; a++)
            {
                forecast[a] = (1 - Epsilon) * forecast[a] + Epsilon / n;
            }
        }
    }
}
