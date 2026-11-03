using System;

namespace AdaptiveOpponent.Core
{
    /// <summary>Thrown when game data breaks a rule from ADR 0010 or ADR 0011.</summary>
    public sealed class DefinitionException : Exception
    {
        /// <summary>Creates the exception with a message naming the broken rule.</summary>
        public DefinitionException(string message) : base(message)
        {
        }
    }

    /// <summary>
    /// The load-time checks on game data (ADR 0010). Check (b), every row and column of the payoff sums to 0, makes
    /// uniform play an equilibrium with value 0. Check (a), every row and column has exactly 2 wins, 2 draws and
    /// 2 losses, gives every action exactly two counters, which the archetypes and level-k experts rely on.
    /// </summary>
    public static class DefinitionValidator
    {
        /// <summary>The number of wins, draws and losses every row and column must have.</summary>
        public const int PerKind = 2;

        /// <summary>Throws <see cref="DefinitionException"/> on the first broken rule.</summary>
        public static void Validate(string[] actionNames, string[] responseNames, Outcome[][] outcomes, int[][] damage,
            int[] authoredCounters, int bigHitWindow, int bigHitThreshold)
        {
            if (actionNames == null || responseNames == null || outcomes == null || damage == null || authoredCounters == null)
            {
                throw new DefinitionException("definition is missing a field");
            }
            int actions = actionNames.Length;
            int responses = responseNames.Length;
            if (actions != 3 * PerKind || responses != 3 * PerKind)
            {
                throw new DefinitionException("need " + 3 * PerKind + " actions and responses, got " + actions + " and " + responses);
            }
            if (outcomes.Length != responses || damage.Length != responses)
            {
                throw new DefinitionException("outcome and damage tables need one row per response");
            }
            for (int r = 0; r < responses; r++)
            {
                if (outcomes[r] == null || damage[r] == null || outcomes[r].Length != actions || damage[r].Length != actions)
                {
                    throw new DefinitionException("row " + r + " needs one entry per action");
                }
                for (int a = 0; a < actions; a++)
                {
                    CheckCell(r, a, outcomes[r][a], damage[r][a]);
                }
            }
            for (int r = 0; r < responses; r++)
            {
                CheckLine("response " + r, i => outcomes[r][i], i => Signed(outcomes[r][i], damage[r][i]), actions);
            }
            for (int a = 0; a < actions; a++)
            {
                CheckLine("action " + a, i => outcomes[i][a], i => Signed(outcomes[i][a], damage[i][a]), responses);
            }
            if (authoredCounters.Length != actions)
            {
                throw new DefinitionException("need one authored counter per action");
            }
            for (int a = 0; a < actions; a++)
            {
                int c = authoredCounters[a];
                if (c < 0 || c >= responses || outcomes[c][a] != Outcome.Win)
                {
                    throw new DefinitionException("authored counter of action " + a + " is not a response that beats it");
                }
            }
            if (bigHitWindow < 1 || bigHitThreshold < 1)
            {
                throw new DefinitionException("big-hit window and threshold must be at least 1");
            }
        }

        private static void CheckCell(int r, int a, Outcome outcome, int dmg)
        {
            if (outcome != Outcome.Win && outcome != Outcome.Draw && outcome != Outcome.Loss)
            {
                throw new DefinitionException("bad outcome at response " + r + ", action " + a);
            }
            if (outcome == Outcome.Draw ? dmg != 0 : dmg < 1)
            {
                throw new DefinitionException("damage at response " + r + ", action " + a +
                    " must be 0 on a draw and at least 1 on a win or loss");
            }
        }

        private static int Signed(Outcome outcome, int dmg) => (int)outcome * dmg;

        private static void CheckLine(string name, Func<int, Outcome> outcome, Func<int, int> payoff, int length)
        {
            int wins = 0, draws = 0, losses = 0, sum = 0;
            for (int i = 0; i < length; i++)
            {
                switch (outcome(i))
                {
                    case Outcome.Win: wins++; break;
                    case Outcome.Draw: draws++; break;
                    default: losses++; break;
                }
                sum += payoff(i);
            }
            if (sum != 0)
            {
                throw new DefinitionException(name + ": payoffs sum to " + sum + ", not 0");
            }
            if (wins != PerKind || draws != PerKind || losses != PerKind)
            {
                throw new DefinitionException(name + ": needs " + PerKind + " wins, draws and losses, got " +
                    wins + "/" + draws + "/" + losses);
            }
        }
    }
}
