namespace AdaptiveOpponent.Core
{
    /// <summary>
    /// One of the predictor's sub-models. Each exchange: <see cref="Predict"/> for the next exchange (or Abstain),
    /// then, once the action is known, <see cref="Observe"/>. All buffers are the caller's, so nothing allocates.
    /// </summary>
    public interface IExpert
    {
        /// <summary>The expert's name in logs and configs, e.g. "order1".</summary>
        string Name { get; }

        /// <summary>
        /// Fills <paramref name="forecast"/> with a probability per action for the next exchange (history.Count).
        /// Returns false to Abstain; the forecast is then left unspecified.
        /// </summary>
        bool Predict(ExchangeHistory history, double[] forecast);

        /// <summary>
        /// Fills <paramref name="sample"/> with one draw from the posterior behind the last <see cref="Predict"/>
        /// (Thompson Sampling). Only called when the expert did not Abstain.
        /// </summary>
        void Sample(Rng rng, double[] sample);

        /// <summary>Learns from the exchange just added to <paramref name="history"/>.</summary>
        void Observe(ExchangeHistory history);

        /// <summary>Called when a new match starts.</summary>
        void StartMatch();

        /// <summary>
        /// Describes the last <see cref="Predict"/> as Insight data for its top action, with the observed (not prior)
        /// faded count of its context. Returns false when this expert has no Insight to offer.
        /// </summary>
        bool Explain(ExchangeHistory history, int topAction, out Insight insight);
    }

    /// <summary>The kinds of statement an Insight or Recall can make; each maps to one sentence template.</summary>
    public enum InsightKind
    {
        /// <summary>"You play {Action} a lot."</summary>
        Frequency,
        /// <summary>"After {Prev1}, you usually play {Action}."</summary>
        AfterAction,
        /// <summary>"After {Prev2} then {Prev1}, you usually play {Action}."</summary>
        AfterTwoActions,
        /// <summary>"After you play {Prev1} and I play {PrevResponse}, you usually play {Action}."</summary>
        AfterActionAndResponse,
        /// <summary>"At the start of a match, you usually play {Action}."</summary>
        MatchStart,
        /// <summary>"After I play {PrevResponse}, you usually play {Action}."</summary>
        AfterResponse,
        /// <summary>"You're repeating a sequence from earlier."</summary>
        RepeatSequence,
        /// <summary>"You're trying to beat my counter."</summary>
        BeatMyCounter,
        /// <summary>"You're reacting to my last response."</summary>
        ReactToMyResponse,
        /// <summary>Recall: "Last time we were here you hit me hard, starting with {Action}."</summary>
        RecallOpening,
        /// <summary>Recall: "Last time you followed {Prev1} with {Action}."</summary>
        RecallFollowUp,
    }

    /// <summary>The data behind one Insight or Recall. Text is built from it only when it is shown.</summary>
    public struct Insight
    {
        /// <summary>Which sentence this is.</summary>
        public InsightKind Kind;
        /// <summary>The predicted action the statement is about.</summary>
        public int Action;
        /// <summary>The previous action in the context, where the sentence uses one.</summary>
        public int Prev1;
        /// <summary>The action before that, where the sentence uses one.</summary>
        public int Prev2;
        /// <summary>The previous response in the context, where the sentence uses one.</summary>
        public int PrevResponse;
        /// <summary>The expert's forecast probability for <see cref="Action"/> (filled in by the Opponent).</summary>
        public double Probability;
        /// <summary>The observed faded count behind the statement.</summary>
        public double Support;
    }
}
