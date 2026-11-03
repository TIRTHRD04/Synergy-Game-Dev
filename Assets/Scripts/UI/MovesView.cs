using System.Text;
using AdaptiveOpponent.Core;

namespace AdaptiveOpponent.UI
{
    /// <summary>
    /// The rules of each move as text, generated from the payoff table so the screen can never disagree with the game
    /// data: what a move beats and loses to, and for how much. Built once per session.
    /// </summary>
    public sealed class MovesView
    {
        /// <summary>Builds the strings for every move of <paramref name="definition"/>.</summary>
        public MovesView(GameDefinition definition)
        {
            int n = definition.ActionCount;
            Names = new string[n];
            Keys = new string[n];
            Beats = new string[n];
            LosesTo = new string[n];
            Hints = new string[n];
            HelpLines = new string[n];
            for (int a = 0; a < n; a++)
            {
                Names[a] = definition.ActionName(a);
                Keys[a] = (a + 1).ToString(System.Globalization.CultureInfo.InvariantCulture);
                Beats[a] = "Beats " + List(definition, a, true);
                LosesTo[a] = "Loses to " + List(definition, a, false);
                Hints[a] = definition.ActionHint(a);
                HelpLines[a] = "[" + Keys[a] + "] " + Names[a] + ": beats " + List(definition, a, true) +
                    "; loses to " + List(definition, a, false) + ".";
            }
        }

        /// <summary>Move names, in key order.</summary>
        public string[] Names { get; }

        /// <summary>The key that plays each move ("1" to "6").</summary>
        public string[] Keys { get; }

        /// <summary>"Beats Dodge (3), Grab (1)" for each move.</summary>
        public string[] Beats { get; }

        /// <summary>"Loses to Parry (3), Guard (1)" for each move.</summary>
        public string[] LosesTo { get; }

        /// <summary>The one-line description of each move from the game data (may be empty).</summary>
        public string[] Hints { get; }

        /// <summary>One full rules line per move, for the how-to-play overlay.</summary>
        public string[] HelpLines { get; }

        // The moves that move `a` beats (wins) or loses to, heaviest damage first, as "Dodge (3), Grab (1)".
        private static string List(GameDefinition definition, int a, bool wins)
        {
            var sb = new StringBuilder();
            for (int damage = definition.MaxDamage; damage >= 1; damage--)
            {
                for (int r = 0; r < definition.ResponseCount; r++)
                {
                    int payoff = definition.Payoff(r, a);
                    bool match = wins ? payoff == -damage : payoff == damage;
                    if (match)
                    {
                        sb.Append(sb.Length > 0 ? ", " : "").Append(definition.ResponseName(r))
                            .Append(" (").Append(damage).Append(')');
                    }
                }
            }
            return sb.Length > 0 ? sb.ToString() : "nothing";
        }
    }
}
