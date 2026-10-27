namespace AdaptiveOpponent.Core
{
    /// <summary>
    /// The mirror model (spec section 3): a CTW over the Opponent's own responses, response-first context, giving q,
    /// the forecast of the Opponent's next response that a player who models the Opponent would make. Shared by the
    /// mirror experts; it predicts and learns at most once per exchange however many experts read it.
    /// </summary>
    public sealed class MirrorModel
    {
        private readonly CtwTree tree;
        private readonly double matchStartScale;
        private readonly int[] context = new int[CtwTree.Depth];
        private readonly int[] nodes = new int[CtwTree.Depth + 1];
        private readonly double[] alphaBuffer;
        private int predictedAt = -1;
        private int learnedAt = -1;
        private int startedMatch = -1;
        private int deepest;

        /// <summary>Creates the model for one session.</summary>
        public MirrorModel(GameDefinition definition, OpponentConfig config, int historyCapacity)
        {
            tree = new CtwTree(definition.ResponseCount, definition.ResponseCount + 1, config.Gamma, historyCapacity);
            matchStartScale = config.MatchStartCountScale;
            Forecast = new double[definition.ResponseCount];
            alphaBuffer = new double[definition.ResponseCount];
        }

        /// <summary>q: the forecast of the Opponent's next response, valid after <see cref="Predict"/>.</summary>
        public double[] Forecast { get; }

        /// <summary>Predicts the response for the next exchange, once per exchange.</summary>
        public void Predict(ExchangeHistory history)
        {
            if (predictedAt == history.Count)
            {
                return;
            }
            predictedAt = history.Count;
            CtwContext.Fill(history, history.Count, CtwOrder.ResponseFirst, context);
            deepest = tree.Predict(context, Forecast, nodes);
        }

        /// <summary>A Thompson draw of q along the predicted path.</summary>
        public void Sample(Rng rng, double[] sample) => tree.SamplePath(nodes, deepest, rng, alphaBuffer, sample);

        /// <summary>Learns the response of the exchange just recorded, once per exchange.</summary>
        public void Observe(ExchangeHistory history)
        {
            int last = history.Count - 1;
            if (learnedAt == last)
            {
                return;
            }
            learnedAt = last;
            CtwContext.Fill(history, last, CtwOrder.ResponseFirst, context);
            tree.Update(context, history.Response(last));
        }

        /// <summary>Scales counts and log-odds at the start of match <paramref name="match"/>, once per match.</summary>
        public void StartMatch(int match)
        {
            if (startedMatch == match)
            {
                return;
            }
            startedMatch = match;
            tree.Scale(matchStartScale, matchStartScale);
        }
    }

    /// <summary>
    /// Mirror-k experts: mirror-1 assumes the player predicts the Opponent's most likely response under q and plays
    /// one of the two actions that beat it; mirror-2 applies one more level step to that forecast. Smoothed by eps.
    /// </summary>
    public sealed class MirrorExpert : IExpert
    {
        private readonly GameDefinition definition;
        private readonly MirrorModel model;
        private readonly int level;
        private readonly double gamma;
        private readonly double[] buffer;
        private readonly double[] responseSample;
        private int lastTop;
        private int matches;
        private double hits;

        /// <summary>Creates a mirror-<paramref name="level"/> expert reading the shared model.</summary>
        public MirrorExpert(GameDefinition definition, MirrorModel model, int level, double gamma)
        {
            this.definition = definition;
            this.model = model;
            this.level = level;
            this.gamma = gamma;
            buffer = new double[definition.ActionCount];
            responseSample = new double[definition.ResponseCount];
        }

        /// <inheritdoc />
        public string Name => "mirror" + level;

        /// <inheritdoc />
        public bool Predict(ExchangeHistory history, double[] forecast)
        {
            model.Predict(history);
            FromResponseForecast(model.Forecast, forecast);
            lastTop = BestResponse.TopAction(forecast, definition.ActionCount);
            return true;
        }

        /// <summary>A Thompson draw: the transform applied to a draw of q.</summary>
        public void Sample(Rng rng, double[] sample)
        {
            model.Sample(rng, responseSample);
            FromResponseForecast(responseSample, sample);
        }

        /// <inheritdoc />
        public void Observe(ExchangeHistory history)
        {
            model.Observe(history);
            hits = gamma * hits + (history.Action(history.Count - 1) == lastTop ? 1 : 0);
        }

        /// <inheritdoc />
        public void StartMatch() => model.StartMatch(matches++);

        /// <summary>"You're reacting to my last response", supported by the faded count of exchanges it called right.</summary>
        public bool Explain(ExchangeHistory history, int topAction, out Insight insight)
        {
            insight = default;
            insight.Kind = InsightKind.ReactToMyResponse;
            insight.Action = topAction;
            insight.Support = hits;
            return true;
        }

        private void FromResponseForecast(double[] q, double[] forecast)
        {
            int predicted = BestResponse.TopAction(q, definition.ResponseCount);
            LevelKExpert.BeatersOf(definition, predicted, forecast);
            for (int l = 1; l < level; l++)
            {
                for (int a = 0; a < definition.ActionCount; a++)
                {
                    buffer[a] = forecast[a];
                }
                LevelKExpert.BeatersOf(definition, BestResponse.Of(definition, buffer), forecast);
            }
            LevelKExpert.Smooth(forecast, definition.ActionCount);
        }
    }
}
