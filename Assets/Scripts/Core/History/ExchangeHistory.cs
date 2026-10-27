using System;

namespace AdaptiveOpponent.Core
{
    /// <summary>
    /// The session's raw exchange history: action, response and payoff of every exchange, with match boundaries.
    /// Preallocated for a fixed capacity, so recording an exchange never allocates.
    /// </summary>
    public sealed class ExchangeHistory
    {
        private readonly int[] actions;
        private readonly int[] responses;
        private readonly int[] payoffs;
        private readonly int[] matchOf;
        private readonly int[] indexInMatch;
        private int matchCount;
        private int matchStartIndex;

        /// <summary>Creates an empty history for up to <paramref name="capacity"/> exchanges.</summary>
        /// <param name="capacity">Maximum exchanges in a session.</param>
        /// <param name="actionCount">Number of actions; the Match-start context uses this value as its symbol.</param>
        /// <param name="responseCount">Number of responses; the Match-start context uses this value as its symbol.</param>
        public ExchangeHistory(int capacity, int actionCount, int responseCount)
        {
            actions = new int[capacity];
            responses = new int[capacity];
            payoffs = new int[capacity];
            matchOf = new int[capacity];
            indexInMatch = new int[capacity];
            MatchStartAction = actionCount;
            MatchStartResponse = responseCount;
        }

        /// <summary>The previous-action symbol of the Match-start context (exchange 1 of a match).</summary>
        public int MatchStartAction { get; }

        /// <summary>The previous-response symbol of the Match-start context.</summary>
        public int MatchStartResponse { get; }

        /// <summary>Exchanges recorded so far this session.</summary>
        public int Count { get; private set; }

        /// <summary>Exchanges this history can hold.</summary>
        public int Capacity => actions.Length;

        /// <summary>Matches started so far (the current match is MatchCount - 1).</summary>
        public int MatchCount => matchCount;

        /// <summary>Exchanges recorded in the current match.</summary>
        public int CountInMatch => Count - matchStartIndex;

        /// <summary>Starts a new match; the next exchange has the Match-start context.</summary>
        public void StartMatch()
        {
            matchCount++;
            matchStartIndex = Count;
        }

        /// <summary>Records one exchange and returns its session index.</summary>
        public int Add(int action, int response, int payoff)
        {
            if (matchCount == 0)
            {
                throw new InvalidOperationException("StartMatch must be called before the first exchange");
            }
            if (Count == actions.Length)
            {
                throw new InvalidOperationException("exchange history is full (" + actions.Length + ")");
            }
            int i = Count++;
            actions[i] = action;
            responses[i] = response;
            payoffs[i] = payoff;
            matchOf[i] = matchCount - 1;
            indexInMatch[i] = i - matchStartIndex;
            return i;
        }

        /// <summary>The player's action in exchange <paramref name="index"/>.</summary>
        public int Action(int index) => actions[index];

        /// <summary>The Opponent's response in exchange <paramref name="index"/>.</summary>
        public int Response(int index) => responses[index];

        /// <summary>The Opponent's payoff in exchange <paramref name="index"/>.</summary>
        public int Payoff(int index) => payoffs[index];

        /// <summary>The match (0-based) that exchange <paramref name="index"/> belongs to.</summary>
        public int MatchOf(int index) => matchOf[index];

        /// <summary>The position (0-based) of exchange <paramref name="index"/> within its match.</summary>
        public int IndexInMatch(int index) => indexInMatch[index];

        /// <summary>
        /// The previous action before exchange <paramref name="index"/> (which may equal <see cref="Count"/>, the next
        /// exchange), or <see cref="MatchStartAction"/> when it is the first exchange of its match.
        /// </summary>
        public int PrevActionBefore(int index)
        {
            return IsMatchStart(index) ? MatchStartAction : actions[index - 1];
        }

        /// <summary>
        /// The action two exchanges before exchange <paramref name="index"/> in the same match, or
        /// <see cref="MatchStartAction"/> when the match has not had that many exchanges yet.
        /// </summary>
        public int SecondPrevActionBefore(int index)
        {
            if (IsMatchStart(index) || IsMatchStart(index - 1))
            {
                return MatchStartAction;
            }
            return actions[index - 2];
        }

        /// <summary>The previous response before exchange <paramref name="index"/>, or <see cref="MatchStartResponse"/>.</summary>
        public int PrevResponseBefore(int index)
        {
            return IsMatchStart(index) ? MatchStartResponse : responses[index - 1];
        }

        /// <summary>True when exchange <paramref name="index"/> is (or would be) the first of its match.</summary>
        public bool IsMatchStart(int index)
        {
            if (index == Count)
            {
                return Count == matchStartIndex;
            }
            return indexInMatch[index] == 0;
        }
    }
}
