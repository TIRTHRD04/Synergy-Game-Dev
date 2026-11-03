using System.Collections.Generic;
using System.Globalization;

namespace AdaptiveOpponent.UI
{
    /// <summary>
    /// The recent exchanges and how often the AI read the player, as cached strings: the last
    /// <see cref="Shown"/> exchanges (player's move, the AI's top prediction, read or not), the read rate per match,
    /// and the live learning curve (read rate per <see cref="Bucket"/> exchanges). Updated once per exchange.
    /// </summary>
    public sealed class HistoryView
    {
        /// <summary>How many recent exchanges the strip shows.</summary>
        public const int Shown = 12;

        /// <summary>Exchanges per point of the learning curve.</summary>
        public const int Bucket = 10;

        private readonly string[] names;
        private readonly int[] matchReads;
        private readonly int[] matchSeen;
        private readonly List<float> curve = new List<float>();
        private int bucketHits;
        private int bucketSeen;
        private int totalReads;
        private int totalSeen;

        /// <summary>Creates an empty history for a session of <paramref name="matches"/> matches.</summary>
        public HistoryView(string[] moveNames, int matches)
        {
            names = moveNames;
            matchReads = new int[matches];
            matchSeen = new int[matches];
            PlayerMoves = new string[Shown];
            Guesses = new string[Shown];
            Reads = new bool[Shown];
            Winners = new ExchangeWinner[Shown];
            ChanceFraction = 1f / moveNames.Length;
            ChanceText = "Chance: " + (100.0 / moveNames.Length).ToString("0", CultureInfo.InvariantCulture) + "%";
        }

        /// <summary>How many strip cells are filled (oldest first).</summary>
        public int Count { get; private set; }

        /// <summary>The player's move in each strip cell.</summary>
        public string[] PlayerMoves { get; }

        /// <summary>The AI's top prediction in each strip cell.</summary>
        public string[] Guesses { get; }

        /// <summary>Whether the AI's top prediction matched in each strip cell.</summary>
        public bool[] Reads { get; }

        /// <summary>Who won each exchange in the strip.</summary>
        public ExchangeWinner[] Winners { get; }

        /// <summary>The read rate per <see cref="Bucket"/> exchanges, the last one possibly partial.</summary>
        public IReadOnlyList<float> Curve => curve;

        /// <summary>"This match: it read you 9 of 30 times (30%)."</summary>
        public string MatchText { get; private set; } = "No exchanges yet.";

        /// <summary>"Whole session: 42 of 150 (28%)."</summary>
        public string SessionText { get; private set; } = "";

        /// <summary>Chance level for one move (1 / number of moves).</summary>
        public float ChanceFraction { get; }

        /// <summary>"Chance: 17%".</summary>
        public string ChanceText { get; }

        /// <summary>Exchanges played in each match so far.</summary>
        public int MatchSeen(int match) => matchSeen[match];

        /// <summary>The read rate of a match, 0-1 (0 when it has no exchanges).</summary>
        public float MatchRate(int match) => matchSeen[match] > 0 ? (float)matchReads[match] / matchSeen[match] : 0f;

        /// <summary>Records one revealed exchange; <paramref name="predicted"/> is -1 when it had no prediction.</summary>
        public void Add(int match, int action, int predicted, bool read, ExchangeWinner winner)
        {
            if (Count == Shown)
            {
                for (int i = 1; i < Shown; i++)
                {
                    PlayerMoves[i - 1] = PlayerMoves[i];
                    Guesses[i - 1] = Guesses[i];
                    Reads[i - 1] = Reads[i];
                    Winners[i - 1] = Winners[i];
                }
                Count--;
            }
            PlayerMoves[Count] = names[action];
            Guesses[Count] = predicted >= 0 ? names[predicted] : "-";
            Reads[Count] = read;
            Winners[Count] = winner;
            Count++;

            int hit = read ? 1 : 0;
            matchReads[match] += hit;
            matchSeen[match]++;
            totalReads += hit;
            totalSeen++;
            if (bucketSeen == Bucket)
            {
                bucketHits = 0;
                bucketSeen = 0;
            }
            bucketHits += hit;
            bucketSeen++;
            float rate = (float)bucketHits / bucketSeen;
            if (bucketSeen == 1)
            {
                curve.Add(rate);
            }
            else
            {
                curve[curve.Count - 1] = rate;
            }
            MatchText = "This match: it read you " + matchReads[match] + " of " + matchSeen[match] + " times (" +
                Percent(MatchRate(match)) + ").";
            SessionText = "Whole session: " + totalReads + " of " + totalSeen + " (" +
                Percent((float)totalReads / totalSeen) + ").";
        }

        /// <summary>Formats a 0-1 rate as "30%".</summary>
        public static string Percent(float rate) => (rate * 100).ToString("0", CultureInfo.InvariantCulture) + "%";
    }
}
