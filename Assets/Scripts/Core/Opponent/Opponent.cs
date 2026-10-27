using System;

namespace AdaptiveOpponent.Core
{
    /// <summary>
    /// The learning Opponent (ADR 0001): experts predict the player's next action, Fixed Share mixes them, the
    /// evidence gate decides whether any pattern may be claimed, and the response selector commits a response before
    /// the action is revealed. Pure C#, deterministic for a given seed, no allocation per exchange.
    /// </summary>
    public sealed class Opponent : IOpponent
    {
        private readonly GameDefinition definition;
        private readonly OpponentConfig config;
        private readonly Rng rng;
        private readonly IExpert[] experts;
        private readonly double[][] forecasts;
        private readonly bool[] awake;
        private readonly double[] mixed;
        private readonly double[] sample;
        private readonly double[] drawBuffer;
        private readonly FixedShareMixer mixer;
        private readonly EvidenceGate gate;
        private readonly SafeExploitation safe;
        private readonly BigHitDetector detector;
        private readonly BigHitMemory bigHits;
        private readonly int recallExpert;
        private int committed = -1;
        private int bestResponse;
        private Insight insight;

        /// <summary>Creates a fresh Opponent for one session.</summary>
        /// <param name="definition">The game data.</param>
        /// <param name="config">The Opponent config.</param>
        /// <param name="rng">The Opponent's own random stream (derived from the session seed).</param>
        /// <param name="historyCapacity">The most exchanges the session can have.</param>
        public Opponent(GameDefinition definition, OpponentConfig config, Rng rng, int historyCapacity)
        {
            this.definition = definition;
            this.config = config;
            this.rng = rng;
            History = new ExchangeHistory(historyCapacity, definition.ActionCount, definition.ResponseCount);
            detector = new BigHitDetector(definition);
            bigHits = new BigHitMemory(definition, config.SequenceBroadMatches, config.AvoidanceBroadMatches);
            int count = config.Experts.Length;
            forecasts = new double[count][];
            for (int i = 0; i < count; i++)
            {
                forecasts[i] = new double[definition.ActionCount];
            }
            awake = new bool[count];
            experts = ExpertFactory.Create(definition, config, bigHits, historyCapacity, forecasts, awake);
            recallExpert = Array.IndexOf(config.Experts, "bigHitSequence");
            mixed = new double[definition.ActionCount];
            sample = new double[definition.ActionCount];
            drawBuffer = new double[Math.Max(experts.Length, 1)];
            mixer = new FixedShareMixer(experts.Length, definition.ActionCount, config.FixedShareAlpha, config.MatchStartWeightMix);
            gate = new EvidenceGate(config.GateThreshold, definition.ActionCount);
            if (config.Selector == SelectorKind.SafeExploitation)
            {
                safe = new SafeExploitation(definition, config.SafeBudgetStart);
            }
        }

        /// <summary>The session so far, as the Opponent recorded it.</summary>
        public ExchangeHistory History { get; }

        /// <summary>The config this Opponent runs.</summary>
        public OpponentConfig Config => config;

        /// <inheritdoc />
        public int TopPrediction { get; private set; }

        /// <inheritdoc />
        public bool GateOpen => gate.IsOpen;

        /// <summary>The evidence gate's score so far, in nats.</summary>
        public double GateScore => gate.Score;

        /// <inheritdoc />
        public double Lambda { get; private set; }

        /// <inheritdoc />
        public CommentaryKind Commentary { get; private set; }

        /// <summary>The data behind the current Commentary (valid when <see cref="Commentary"/> is not None).</summary>
        public Insight CurrentInsight => insight;

        /// <summary>The awake expert with the most weight for the committed exchange, or -1.</summary>
        public int LeadingExpert { get; private set; }

        /// <summary>The name of expert <paramref name="index"/>.</summary>
        public string ExpertName(int index) => experts[index].Name;

        /// <summary>The number of experts.</summary>
        public int ExpertCount => experts.Length;

        /// <summary>The weight of expert <paramref name="index"/>.</summary>
        public double ExpertWeight(int index) => mixer.Weight(index);

        /// <summary>Whether expert <paramref name="index"/> gave a forecast for the committed exchange.</summary>
        public bool ExpertAwake(int index) => awake[index];

        /// <summary>Big hits the player landed on the Opponent, and those it landed on the player.</summary>
        public BigHitMemory BigHits => bigHits;

        /// <inheritdoc />
        public void StartMatch()
        {
            History.StartMatch();
            mixer.StartMatch();
            for (int i = 0; i < experts.Length; i++)
            {
                experts[i].StartMatch();
            }
        }

        /// <inheritdoc />
        public int Decide()
        {
            for (int i = 0; i < experts.Length; i++)
            {
                awake[i] = experts[i].Predict(History, forecasts[i]);
            }
            mixer.Mix(awake, forecasts, mixed);
            TopPrediction = BestResponse.TopAction(mixed, definition.ActionCount);
            LeadingExpert = mixer.Leading(awake);
            UpdateCommentary();
            committed = SelectResponse();
            return committed;
        }

        /// <inheritdoc />
        public void Observe(int action)
        {
            if (committed < 0)
            {
                throw new InvalidOperationException("Decide must be called before Observe");
            }
            gate.Observe(mixed[action]);
            mixer.Update(awake, forecasts, mixed, action);
            safe?.Observe(bestResponse, action, Lambda);
            History.Add(action, committed, definition.Payoff(committed, action));
            if (detector.Observe(History, out BigHit hit))
            {
                bigHits.Record(History, hit);
            }
            for (int i = 0; i < experts.Length; i++)
            {
                experts[i].Observe(History);
            }
            committed = -1;
        }

        /// <inheritdoc />
        public double ForecastOf(int action) => mixed[action];

        private int SelectResponse()
        {
            bestResponse = BestResponse.Of(definition, mixed);
            if (!gate.IsOpen || LeadingExpert < 0)
            {
                Lambda = 0;
                return rng.NextInt(definition.ResponseCount);
            }
            if (safe != null)
            {
                Lambda = safe.Lambda(bestResponse);
                return config.MeanMode || rng.NextDouble() < Lambda ? bestResponse : rng.NextInt(definition.ResponseCount);
            }
            Lambda = 1;
            if (config.MeanMode)
            {
                return bestResponse;
            }
            int drawn = mixer.DrawAwake(awake, rng, drawBuffer);
            experts[drawn].Sample(rng, sample);
            return BestResponse.Of(definition, sample);
        }

        // A Recall needs the gate open and the Big-hit sequence expert firing with its prediction at p >= the
        // Insight cutoff; it takes precedence. Otherwise an Insight needs the leading expert's top action at
        // p >= the cutoff and enough observed faded counts behind it.
        private void UpdateCommentary()
        {
            Commentary = CommentaryKind.None;
            if (!gate.IsOpen)
            {
                return;
            }
            if (recallExpert >= 0 && awake[recallExpert] && TryExplain(recallExpert))
            {
                Commentary = CommentaryKind.Recall;
                return;
            }
            if (LeadingExpert >= 0 && LeadingExpert != recallExpert && TryExplain(LeadingExpert)
                && insight.Support >= config.InsightMinCount)
            {
                Commentary = CommentaryKind.Insight;
            }
        }

        private bool TryExplain(int expert)
        {
            double[] forecast = forecasts[expert];
            int top = BestResponse.TopAction(forecast, definition.ActionCount);
            if (forecast[top] < config.InsightMinProbability || !experts[expert].Explain(History, top, out insight))
            {
                return false;
            }
            insight.Probability = forecast[top];
            return true;
        }
    }
}
