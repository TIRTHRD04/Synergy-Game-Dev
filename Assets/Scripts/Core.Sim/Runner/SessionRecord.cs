using System.Collections.Generic;
using AdaptiveOpponent.Core;

namespace AdaptiveOpponent.Sim
{
    /// <summary>Everything recorded about one simulated exchange.</summary>
    public struct ExchangeRecord
    {
        /// <summary>Match index (0-based).</summary>
        public int Match;
        /// <summary>Exchange index within the match (0-based).</summary>
        public int IndexInMatch;
        /// <summary>The player's action.</summary>
        public int Action;
        /// <summary>The Opponent's committed response.</summary>
        public int Response;
        /// <summary>The Opponent's payoff.</summary>
        public int Payoff;
        /// <summary>The Opponent's top prediction, fixed before the player acted.</summary>
        public int TopPrediction;
        /// <summary>The forecast probability of the action actually played.</summary>
        public double ForecastOfAction;
        /// <summary>Whether the evidence gate was open when the response was committed.</summary>
        public bool GateOpen;
        /// <summary>The weight on the best response in the committed strategy.</summary>
        public double Lambda;
        /// <summary>What the player's action was for.</summary>
        public PlayerTag Tag;
        /// <summary>What the Commentary showed after this exchange.</summary>
        public CommentaryKind Commentary;
        /// <summary>The archetype's oracle probability for this exchange (NaN when it has none).</summary>
        public double OracleProbability;
        /// <summary>Stopwatch ticks spent in Decide plus Observe.</summary>
        public long DecisionTicks;
    }

    /// <summary>The full record of one simulated session.</summary>
    public sealed class SessionRecord
    {
        private readonly List<ExchangeRecord> exchanges;
        private readonly List<int> opponentHpAtEnd = new List<int>();
        private readonly List<int> playerHpAtEnd = new List<int>();

        /// <summary>Creates an empty record.</summary>
        public SessionRecord(int capacity, string playerName)
        {
            exchanges = new List<ExchangeRecord>(capacity);
            PlayerName = playerName;
        }

        /// <summary>The archetype that played.</summary>
        public string PlayerName { get; }

        /// <summary>The recorded exchanges in order.</summary>
        public IReadOnlyList<ExchangeRecord> Exchanges => exchanges;

        /// <summary>The Opponent's HP at the end of each match.</summary>
        public IReadOnlyList<int> OpponentHpAtEnd => opponentHpAtEnd;

        /// <summary>The player's HP at the end of each match.</summary>
        public IReadOnlyList<int> PlayerHpAtEnd => playerHpAtEnd;

        /// <summary>Appends one exchange.</summary>
        public void Add(ExchangeRecord exchange) => exchanges.Add(exchange);

        /// <summary>Records the HP of both sides when a match ends.</summary>
        public void EndMatch(int opponentHp, int playerHp)
        {
            opponentHpAtEnd.Add(opponentHp);
            playerHpAtEnd.Add(playerHp);
        }
    }
}
