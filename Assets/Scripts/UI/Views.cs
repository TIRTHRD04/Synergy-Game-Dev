using System.Collections.Generic;
using System.Globalization;
using System.Text;
using AdaptiveOpponent.Core;

namespace AdaptiveOpponent.UI
{
    /// <summary>
    /// The Belief and Commentary for the committed exchange, as cached strings. Rebuilt once per exchange from what the
    /// Opponent actually computed; the panels only draw these strings.
    /// </summary>
    public sealed class BeliefView
    {
        private const int Shown = 3;
        private readonly GameDefinition definition;
        private readonly int[] order;

        /// <summary>Creates an empty view.</summary>
        public BeliefView(GameDefinition definition)
        {
            this.definition = definition;
            order = new int[definition.ActionCount];
            Lines = new string[Shown];
        }

        /// <summary>The top predictions, one line each ("Action 3  54%").</summary>
        public string[] Lines { get; }

        /// <summary>The heading: what the Opponent is basing its guess on, and whether it is only a guess.</summary>
        public string Heading { get; private set; } = "";

        /// <summary>The Commentary line, or empty when there is nothing the evidence supports.</summary>
        public string Commentary { get; private set; } = "";

        /// <summary>Rebuilds the strings from the Opponent's committed forecast.</summary>
        public void Update(Opponent opponent)
        {
            for (int a = 0; a < order.Length; a++)
            {
                order[a] = a;
            }
            for (int i = 1; i < order.Length; i++)
            {
                int key = order[i];
                int j = i - 1;
                while (j >= 0 && opponent.ForecastOf(order[j]) < opponent.ForecastOf(key))
                {
                    order[j + 1] = order[j];
                    j--;
                }
                order[j + 1] = key;
            }
            for (int i = 0; i < Shown; i++)
            {
                int a = order[i];
                Lines[i] = definition.ActionName(a) + "  " +
                    (opponent.ForecastOf(a) * 100).ToString("0", CultureInfo.InvariantCulture) + "%";
            }
            string basis = opponent.LeadingExpert >= 0
                ? CommentaryText.PlainExpertName(opponent.ExpertName(opponent.LeadingExpert))
                : "nothing yet";
            Heading = (opponent.GateOpen ? "I expect" : "My guess (not enough evidence yet)") + ", based on " + basis + ":";
            Commentary = opponent.Commentary == CommentaryKind.None ? "" : CommentaryText.Sentence(definition, opponent.CurrentInsight);
        }
    }

    /// <summary>The last revealed exchange, as cached strings.</summary>
    public sealed class RevealView
    {
        /// <summary>"You played X. I played Y."</summary>
        public string Choices { get; private set; } = "";

        /// <summary>The result and the damage dealt.</summary>
        public string Result { get; private set; } = "";

        /// <summary>"BIG HIT" text, or empty.</summary>
        public string BigHit { get; private set; } = "";

        /// <summary>The nonce that opens the commitment.</summary>
        public string Nonce { get; private set; } = "";

        /// <summary>Sets the strings for one revealed exchange.</summary>
        public void Set(GameDefinition definition, int action, int response, Outcome outcome, int damage, bool bigHit,
            BigHitSide side, string nonce)
        {
            Choices = "You played " + definition.ActionName(action) + ". I played " + definition.ResponseName(response) + ".";
            Result = outcome == Outcome.Draw ? "Draw: no damage."
                : outcome == Outcome.Win ? "I hit you for " + damage + "." : "You hit me for " + damage + ".";
            BigHit = !bigHit ? "" : side == BigHitSide.OnOpponent ? "BIG HIT: three hits in a row on me!" : "BIG HIT: three hits in a row on you!";
            Nonce = "Commitment nonce: " + nonce;
        }
    }

    /// <summary>The end-of-session summary, built once.</summary>
    public sealed class SummaryView
    {
        /// <summary>Builds the summary from the session's record.</summary>
        public SummaryView(IReadOnlyList<bool> correct, int damageDealt, int damageTaken, int opponentWins, int playerWins,
            int gateOpenedAt, int bigHitsOnOpponent, int bigHitsOnPlayer)
        {
            int hits = 0;
            foreach (bool c in correct)
            {
                hits += c ? 1 : 0;
            }
            CultureInfo ci = CultureInfo.InvariantCulture;
            double accuracy = correct.Count > 0 ? (double)hits / correct.Count : 0;
            var lines = new List<string>
            {
                "Matches: you won " + playerWins + ", I won " + opponentWins + ".",
                "Damage: you dealt " + damageTaken + ", I dealt " + damageDealt + ".",
                "I predicted your action " + (accuracy * 100).ToString("0", ci) + "% of the time over " + correct.Count +
                    " exchanges (chance is 17%).",
                gateOpenedAt < 0 ? "I never found enough evidence that you were predictable."
                    : "I had enough evidence that you were predictable after exchange " + gateOpenedAt + ".",
                "Big hits: you landed " + bigHitsOnOpponent + " on me, I landed " + bigHitsOnPlayer + " on you.",
            };
            Lines = lines.ToArray();
            Curve = BuildCurve(correct, ci);
        }

        /// <summary>The summary lines.</summary>
        public string[] Lines { get; }

        /// <summary>The learning curve: prediction accuracy per 25 exchanges, as text.</summary>
        public string Curve { get; }

        private static string BuildCurve(IReadOnlyList<bool> correct, CultureInfo ci)
        {
            const int bucket = 25;
            var sb = new StringBuilder("Learning curve (my accuracy per " + bucket + " exchanges): ");
            for (int start = 0; start < correct.Count; start += bucket)
            {
                int end = System.Math.Min(correct.Count, start + bucket), hits = 0;
                for (int i = start; i < end; i++)
                {
                    hits += correct[i] ? 1 : 0;
                }
                sb.Append(((double)hits / (end - start) * 100).ToString("0", ci)).Append("% ");
            }
            return sb.ToString();
        }
    }
}
