namespace AdaptiveOpponent.Core
{
    /// <summary>The result of one exchange from the Opponent's side.</summary>
    public enum Outcome
    {
        /// <summary>The player's action beat the Opponent's response.</summary>
        Loss = -1,
        /// <summary>Neither side takes damage.</summary>
        Draw = 0,
        /// <summary>The Opponent's response beat the player's action.</summary>
        Win = 1,
    }

    /// <summary>
    /// The game data: action and response names, the outcome matrix, the Damage table, authored counters and the
    /// big-hit rule. Immutable and validated on construction (ADR 0010), so every other part can rely on it.
    /// </summary>
    public sealed class GameDefinition
    {
        private readonly string[] actionNames;
        private readonly string[] responseNames;
        private readonly string[] actionHints;
        private readonly int[] payoff;
        private readonly int[] authoredCounter;
        private readonly int[] lightCounter;
        private readonly int[] heavyBeater;
        private readonly int[] lightBeater;

        /// <summary>
        /// Builds and validates a definition. <paramref name="outcomes"/>[r][a] and <paramref name="damage"/>[r][a]
        /// are for response r against action a. <paramref name="actionHints"/> is optional (one line per action).
        /// Throws <see cref="DefinitionException"/> if the data is not balanced.
        /// </summary>
        public GameDefinition(string[] actionNames, string[] responseNames, Outcome[][] outcomes, int[][] damage,
            int[] authoredCounters, int bigHitWindow, int bigHitThreshold, string[] actionHints = null)
        {
            DefinitionValidator.Validate(actionNames, responseNames, outcomes, damage, authoredCounters,
                bigHitWindow, bigHitThreshold);
            if (actionHints != null && actionHints.Length != actionNames.Length)
            {
                throw new DefinitionException("need one action hint per action, got " + actionHints.Length);
            }
            this.actionNames = (string[])actionNames.Clone();
            this.responseNames = (string[])responseNames.Clone();
            this.actionHints = actionHints != null ? (string[])actionHints.Clone() : new string[actionNames.Length];
            for (int a = 0; a < this.actionHints.Length; a++)
            {
                this.actionHints[a] = this.actionHints[a] ?? "";
            }
            ActionCount = actionNames.Length;
            ResponseCount = responseNames.Length;
            BigHitWindow = bigHitWindow;
            BigHitThreshold = bigHitThreshold;
            payoff = new int[ResponseCount * ActionCount];
            for (int r = 0; r < ResponseCount; r++)
            {
                for (int a = 0; a < ActionCount; a++)
                {
                    int value = (int)outcomes[r][a] * damage[r][a];
                    payoff[r * ActionCount + a] = value;
                    if (damage[r][a] > MaxDamage)
                    {
                        MaxDamage = damage[r][a];
                    }
                }
            }
            authoredCounter = (int[])authoredCounters.Clone();
            lightCounter = new int[ActionCount];
            for (int a = 0; a < ActionCount; a++)
            {
                lightCounter[a] = OtherCounter(a, authoredCounter[a]);
            }
            heavyBeater = new int[ResponseCount];
            lightBeater = new int[ResponseCount];
            for (int r = 0; r < ResponseCount; r++)
            {
                FindBeaters(r, out heavyBeater[r], out lightBeater[r]);
            }
        }

        /// <summary>Number of actions the player can pick from.</summary>
        public int ActionCount { get; }

        /// <summary>Number of responses the Opponent can pick from.</summary>
        public int ResponseCount { get; }

        /// <summary>The largest damage in the Damage table.</summary>
        public int MaxDamage { get; }

        /// <summary>How many consecutive exchanges a big hit spans (ADR 0011).</summary>
        public int BigHitWindow { get; }

        /// <summary>The total damage a big hit must reach over its window.</summary>
        public int BigHitThreshold { get; }

        /// <summary>The display name of action <paramref name="action"/>.</summary>
        public string ActionName(int action) => actionNames[action];

        /// <summary>The display name of response <paramref name="response"/>.</summary>
        public string ResponseName(int response) => responseNames[response];

        /// <summary>A one-line description of action <paramref name="action"/>, or "" when the data has none.</summary>
        public string ActionHint(int action) => actionHints[action];

        /// <summary>The Opponent's payoff: +damage on a win, -damage on a loss, 0 on a draw.</summary>
        public int Payoff(int response, int action) => payoff[response * ActionCount + action];

        /// <summary>The damage the loser of this exchange takes (0 on a draw).</summary>
        public int Damage(int response, int action)
        {
            int value = payoff[response * ActionCount + action];
            return value < 0 ? -value : value;
        }

        /// <summary>The outcome of this exchange from the Opponent's side.</summary>
        public Outcome OutcomeOf(int response, int action)
        {
            int value = payoff[response * ActionCount + action];
            return value > 0 ? Outcome.Win : value < 0 ? Outcome.Loss : Outcome.Draw;
        }

        /// <summary>The designer's preferred counter for an action, used to break ties.</summary>
        public int AuthoredCounter(int action) => authoredCounter[action];

        /// <summary>The action's other counter (the one that is not the authored counter).</summary>
        public int OtherCounterOf(int action) => lightCounter[action];

        /// <summary>The action that beats <paramref name="response"/> for the most damage.</summary>
        public int HeavyBeater(int response) => heavyBeater[response];

        /// <summary>The action that beats <paramref name="response"/> for the least damage.</summary>
        public int LightBeater(int response) => lightBeater[response];

        /// <summary>The worst payoff response <paramref name="response"/> can get, over all actions.</summary>
        public int MinPayoff(int response)
        {
            int min = 0;
            for (int a = 0; a < ActionCount; a++)
            {
                int value = payoff[response * ActionCount + a];
                if (value < min)
                {
                    min = value;
                }
            }
            return min;
        }

        private int OtherCounter(int action, int counter)
        {
            for (int r = 0; r < ResponseCount; r++)
            {
                if (r != counter && payoff[r * ActionCount + action] > 0)
                {
                    return r;
                }
            }
            throw new DefinitionException("action " + action + " has only one counter");
        }

        private void FindBeaters(int response, out int heavy, out int light)
        {
            heavy = -1;
            light = -1;
            for (int a = 0; a < ActionCount; a++)
            {
                int value = payoff[response * ActionCount + a];
                if (value >= 0)
                {
                    continue;
                }
                if (heavy < 0)
                {
                    heavy = a;
                }
                else if (value < payoff[response * ActionCount + heavy])
                {
                    light = heavy;
                    heavy = a;
                }
                else
                {
                    light = a;
                }
            }
        }
    }
}
