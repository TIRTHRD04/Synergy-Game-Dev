using System.Globalization;
using AdaptiveOpponent.Core;

namespace AdaptiveOpponent.UI
{
    /// <summary>
    /// What the AI expected on the exchange just revealed, and how it chose its move: the Belief and Commentary as
    /// cached strings. Captured at the reveal (after the player has acted), so the screen never shows the AI's
    /// prediction while the player is choosing (ADR 0012). Every value is read from the Opponent; nothing is canned.
    /// </summary>
    public sealed class BeliefView
    {
        private const double FlatTolerance = 1e-9;
        private readonly GameDefinition definition;

        /// <summary>Creates an empty view.</summary>
        public BeliefView(GameDefinition definition)
        {
            this.definition = definition;
            int n = definition.ActionCount;
            Names = new string[n];
            Percents = new string[n];
            Fractions = new float[n];
            IsTop = new bool[n];
            IsActual = new bool[n];
            ChanceFraction = 1f / n;
            ChanceText = "yellow tick = chance (" + Percent(1.0 / n) + ")";
            for (int a = 0; a < n; a++)
            {
                Names[a] = definition.ActionName(a);
                Percents[a] = "";
            }
        }

        /// <summary>Whether an exchange has been captured yet.</summary>
        public bool HasData { get; private set; }

        /// <summary>Move names, one per forecast bar.</summary>
        public string[] Names { get; }

        /// <summary>The forecast of each move as "54%".</summary>
        public string[] Percents { get; }

        /// <summary>The forecast of each move, 0-1.</summary>
        public float[] Fractions { get; }

        /// <summary>Which move was its top prediction.</summary>
        public bool[] IsTop { get; }

        /// <summary>Which move the player actually played.</summary>
        public bool[] IsActual { get; }

        /// <summary>Chance level for one move (1 / number of moves).</summary>
        public float ChanceFraction { get; }

        /// <summary>The legend for the chance marker.</summary>
        public string ChanceText { get; }

        /// <summary>Whether it had a prediction at all (false when every move was equally likely).</summary>
        public bool HadPrediction { get; private set; }

        /// <summary>Whether it had a prediction and it was the move the player played.</summary>
        public bool Read { get; private set; }

        /// <summary>"ITS PREDICTION FOR EXCHANGE 12".</summary>
        public string Heading { get; private set; } = "";

        /// <summary>"READ YOU: ..." or "MISSED: ...".</summary>
        public string Verdict { get; private set; } = "";

        /// <summary>How it chose its move, from the Opponent's ResponseKind.</summary>
        public string Choice { get; private set; } = "";

        /// <summary>The Commentary line in quotes, or a note that there is none yet.</summary>
        public string Pattern { get; private set; } = "";

        /// <summary>The Commentary sentence, or null when the evidence supports none (used to count patterns).</summary>
        public string PatternSentence { get; private set; }

        /// <summary>
        /// Captures the committed exchange. Call after the player acts and before the Opponent observes the action, so
        /// the forecast and Commentary are the ones fixed before the player chose.
        /// </summary>
        public void Capture(Opponent opponent, int action, int response, int exchangeNumber)
        {
            HasData = true;
            int top = opponent.TopPrediction;
            double low = 1;
            for (int a = 0; a < Names.Length; a++)
            {
                low = System.Math.Min(low, opponent.ForecastOf(a));
                Fractions[a] = (float)opponent.ForecastOf(a);
                Percents[a] = Percent(opponent.ForecastOf(a));
                IsTop[a] = a == top;
                IsActual[a] = a == action;
            }
            HadPrediction = opponent.ForecastOf(top) - low > FlatTolerance;
            Read = HadPrediction && top == action;
            Heading = "ITS PREDICTION FOR EXCHANGE " + exchangeNumber;
            Verdict = !HadPrediction ? "NO READ YET: every move looked equally likely to it."
                : Read ? "READ YOU: it predicted " + Names[top] + " and you played it."
                : "MISSED: it predicted " + Names[top] + ", you played " + Names[action] + ".";
            Choice = ChoiceText(opponent, top, response);
            PatternSentence = opponent.Commentary == CommentaryKind.None
                ? null
                : CommentaryText.Sentence(definition, opponent.CurrentInsight);
            Pattern = PatternSentence == null ? "No pattern it can back with evidence yet." : "\"" + PatternSentence + "\"";
        }

        private string ChoiceText(Opponent opponent, int top, int response)
        {
            string played = definition.ResponseName(response);
            string expected = HadPrediction ? Names[top] + " (" + Percents[top] + ")" : "no move in particular";
            switch (opponent.ResponseKind)
            {
                case ResponseKind.BestResponse:
                    return "It expected " + expected + ", so it played " + played +
                        ": the move that scores best against its forecast.";
                case ResponseKind.SampledBestResponse:
                    return "It expected " + expected + " and played " + played +
                        ": the move that scores best against one of its pattern guesses.";
                default:
                    if (!opponent.GateOpen)
                    {
                        return "Not confident yet, so it played " + played + " at random.";
                    }
                    return opponent.LeadingExpert < 0
                        ? "None of its patterns fit this spot, so it played " + played + " at random."
                        : "It played " + played + " at random to stay unpredictable (acting on its read, it would have played " +
                            definition.ResponseName(opponent.PlannedResponse) + ").";
            }
        }

        private static string Percent(double p) => (p * 100).ToString("0", CultureInfo.InvariantCulture) + "%";
    }
}
