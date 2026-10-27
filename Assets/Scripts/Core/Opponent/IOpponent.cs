namespace AdaptiveOpponent.Core
{
    /// <summary>What the Commentary currently shows.</summary>
    public enum CommentaryKind
    {
        /// <summary>Nothing: the gate is closed or no prediction is strong enough.</summary>
        None,
        /// <summary>An Insight about the player's pattern.</summary>
        Insight,
        /// <summary>A Recall of a recorded big hit.</summary>
        Recall,
    }

    /// <summary>
    /// The Opponent as the session loop sees it. Each exchange: <see cref="Decide"/> commits a response and fixes
    /// the forecast, then the player acts, then <see cref="Observe"/> reveals the action. The Opponent never sees the
    /// action before it commits.
    /// </summary>
    public interface IOpponent
    {
        /// <summary>Starts a new match of the session.</summary>
        void StartMatch();

        /// <summary>Commits the response for the next exchange and fixes the forecast for it.</summary>
        int Decide();

        /// <summary>Reveals the player's action for the exchange committed by the last <see cref="Decide"/>.</summary>
        void Observe(int action);

        /// <summary>The forecast probability of <paramref name="action"/>, fixed by the last <see cref="Decide"/>.</summary>
        double ForecastOf(int action);

        /// <summary>The single most likely action in the current forecast (ties to the lower index).</summary>
        int TopPrediction { get; }

        /// <summary>Whether the evidence gate is open.</summary>
        bool GateOpen { get; }

        /// <summary>The weight on the best response in the committed strategy (0 while the gate is closed).</summary>
        double Lambda { get; }

        /// <summary>What the Commentary shows for the exchange committed by the last <see cref="Decide"/>.</summary>
        CommentaryKind Commentary { get; }
    }
}
