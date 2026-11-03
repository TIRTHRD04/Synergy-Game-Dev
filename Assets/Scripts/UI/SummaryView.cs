using System.Collections.Generic;

namespace AdaptiveOpponent.UI
{
    /// <summary>
    /// The end-of-session summary, built once from what the session recorded: results, the read rate per match, the
    /// learning curve, when the evidence gate opened, and the patterns the AI put into words, most frequent first.
    /// </summary>
    public sealed class SummaryView
    {
        private const int PatternsShown = 5;

        /// <summary>Builds the summary.</summary>
        public SummaryView(HistoryView history, int matches, int damageDealt, int damageTaken, int opponentWins,
            int playerWins, int gateOpenedAt, int bigHitsOnOpponent, int bigHitsOnPlayer,
            IReadOnlyDictionary<string, int> patterns)
        {
            Results = new[]
            {
                "Matches: you won " + playerWins + ", the AI won " + opponentWins + ".",
                "Damage: you dealt " + damageTaken + ", the AI dealt " + damageDealt + ".",
                "Big hits: you landed " + bigHitsOnOpponent + ", the AI landed " + bigHitsOnPlayer + ".",
                gateOpenedAt < 0
                    ? "It never found enough evidence that you have habits: to it, you looked random."
                    : "It found enough evidence that you have habits after exchange " + gateOpenedAt + " and started acting on its read.",
                history.SessionText + " " + history.ChanceText + ".",
            };
            MatchRates = new float[matches];
            MatchLabels = new string[matches];
            for (int m = 0; m < matches; m++)
            {
                MatchRates[m] = history.MatchRate(m);
                MatchLabels[m] = "Match " + (m + 1) + ": " + HistoryView.Percent(MatchRates[m]) +
                    " (" + history.MatchSeen(m) + " exchanges)";
            }
            Curve = new float[history.Curve.Count];
            for (int i = 0; i < Curve.Length; i++)
            {
                Curve[i] = history.Curve[i];
            }
            var sorted = new List<KeyValuePair<string, int>>(patterns);
            sorted.Sort((x, y) => y.Value != x.Value ? y.Value.CompareTo(x.Value) : string.CompareOrdinal(x.Key, y.Key));
            int shown = System.Math.Min(PatternsShown, sorted.Count);
            Patterns = new string[shown == 0 ? 1 : shown];
            if (shown == 0)
            {
                Patterns[0] = "None: it never had enough evidence to describe a habit of yours.";
            }
            for (int i = 0; i < shown; i++)
            {
                Patterns[i] = sorted[i].Value + "x  \"" + sorted[i].Key + "\"";
            }
        }

        /// <summary>The result lines.</summary>
        public string[] Results { get; }

        /// <summary>The read rate of each match, 0-1.</summary>
        public float[] MatchRates { get; }

        /// <summary>"Match 1: 21% (87 exchanges)" for each match.</summary>
        public string[] MatchLabels { get; }

        /// <summary>The learning curve: read rate per <see cref="HistoryView.Bucket"/> exchanges.</summary>
        public float[] Curve { get; }

        /// <summary>The patterns it described, with how many exchanges each was shown on.</summary>
        public string[] Patterns { get; }
    }
}
