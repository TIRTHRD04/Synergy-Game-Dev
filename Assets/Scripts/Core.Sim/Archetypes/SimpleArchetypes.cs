using AdaptiveOpponent.Core;

namespace AdaptiveOpponent.Sim
{
    /// <summary>Shared plumbing for archetypes: the definition, the play stream and uniform noise.</summary>
    public abstract class ArchetypeBase : IPlayer
    {
        /// <summary>The game data for this session.</summary>
        protected GameDefinition Definition { get; private set; }

        /// <summary>The random stream for in-play choices.</summary>
        protected Rng Play { get; private set; }

        /// <inheritdoc />
        public abstract string Name { get; }

        /// <inheritdoc />
        public PlayerTag Tag { get; protected set; }

        /// <inheritdoc />
        public void StartSession(GameDefinition definition, Rng setup, Rng play)
        {
            Definition = definition;
            Play = play;
            Tag = PlayerTag.None;
            Setup(setup);
        }

        /// <inheritdoc />
        public abstract int Act(ExchangeHistory history);

        /// <inheritdoc />
        public virtual void Observe(ExchangeHistory history)
        {
        }

        /// <summary>Makes the per-seed choices for a new session.</summary>
        protected virtual void Setup(Rng setup)
        {
        }

        /// <summary>A uniformly random action.</summary>
        protected int RandomAction() => Play.NextInt(Definition.ActionCount);

        /// <summary>True with probability <paramref name="p"/>.</summary>
        protected bool Chance(double p) => Play.NextDouble() < p;

        /// <summary>One of the two actions that beat <paramref name="response"/>, with equal chance.</summary>
        protected int EitherBeater(int response) => Chance(0.5) ? Definition.HeavyBeater(response) : Definition.LightBeater(response);
    }

    /// <summary>Plays uniformly at random: the negative control.</summary>
    public sealed class RandomPlayer : ArchetypeBase
    {
        /// <inheritdoc />
        public override string Name => "Random";

        /// <inheritdoc />
        public override int Act(ExchangeHistory history) => RandomAction();
    }

    /// <summary>Plays one per-seed action, with 10% uniform noise.</summary>
    public sealed class Spammer : ArchetypeBase
    {
        private const double Noise = 0.1;
        private int favourite;

        /// <inheritdoc />
        public override string Name => "Spammer";

        /// <inheritdoc />
        protected override void Setup(Rng setup) => favourite = setup.NextInt(Definition.ActionCount);

        /// <inheritdoc />
        public override int Act(ExchangeHistory history) => Chance(Noise) ? RandomAction() : favourite;
    }

    /// <summary>Plays a per-seed cycle of three distinct actions, with 10% uniform noise; the cycle keeps its place across matches.</summary>
    public sealed class Cycler : ArchetypeBase
    {
        private const double Noise = 0.1;
        private readonly int[] cycle = new int[3];
        private int position;

        /// <inheritdoc />
        public override string Name => "Cycler";

        /// <inheritdoc />
        protected override void Setup(Rng setup)
        {
            position = 0;
            cycle[0] = setup.NextInt(Definition.ActionCount);
            do
            {
                cycle[1] = setup.NextInt(Definition.ActionCount);
            }
            while (cycle[1] == cycle[0]);
            do
            {
                cycle[2] = setup.NextInt(Definition.ActionCount);
            }
            while (cycle[2] == cycle[0] || cycle[2] == cycle[1]);
        }

        /// <inheritdoc />
        public override int Act(ExchangeHistory history)
        {
            int action = cycle[position];
            position = (position + 1) % cycle.Length;
            return Chance(Noise) ? RandomAction() : action;
        }
    }

    /// <summary>
    /// Win-stay/lose-shift: after a win it repeats its action; after a draw or loss it plays one of the two actions
    /// that beat the Opponent's last response, picked at random. The first exchange of the session is random.
    /// </summary>
    public sealed class WinStayLoseShift : ArchetypeBase, IOracle
    {
        /// <inheritdoc />
        public override string Name => "WinStayLoseShift";

        /// <inheritdoc />
        public double OracleProbability(ExchangeHistory history)
        {
            int last = history.Count - 1;
            if (last < 0)
            {
                return 1.0 / Definition.ActionCount;
            }
            return history.Payoff(last) < 0 ? 1.0 : 0.5;
        }

        /// <inheritdoc />
        public override int Act(ExchangeHistory history)
        {
            int last = history.Count - 1;
            if (last < 0)
            {
                return RandomAction();
            }
            if (history.Payoff(last) < 0)
            {
                return history.Action(last);
            }
            return EitherBeater(history.Response(last));
        }
    }

    /// <summary>
    /// Out-guesser: keeps faded counts (0.9 per exchange) of the Opponent's responses, predicts the most likely one,
    /// and plays one of the two actions that beat it, 70/30 with a per-seed preference between the heavy and light
    /// beater, plus 10% uniform noise.
    /// </summary>
    public sealed class OutGuesser : ArchetypeBase, IOracle
    {
        private const double Fade = 0.9;
        private const double Noise = 0.1;
        private const double Preferred = 0.7;
        private double[] counts;

        /// <summary>True when this seed prefers the heavy beater (logged with the run).</summary>
        public bool PrefersHeavy { get; private set; }

        /// <inheritdoc />
        public override string Name => "OutGuesser";

        /// <inheritdoc />
        public double OracleProbability(ExchangeHistory history) => (1 - Noise) * Preferred + Noise / Definition.ActionCount;

        /// <inheritdoc />
        protected override void Setup(Rng setup)
        {
            counts = new double[Definition.ResponseCount];
            PrefersHeavy = setup.NextInt(2) == 0;
        }

        /// <inheritdoc />
        public override int Act(ExchangeHistory history)
        {
            if (Chance(Noise))
            {
                return RandomAction();
            }
            int predicted = 0;
            for (int r = 1; r < counts.Length; r++)
            {
                if (counts[r] > counts[predicted])
                {
                    predicted = r;
                }
            }
            bool heavy = Chance(Preferred) == PrefersHeavy;
            return heavy ? Definition.HeavyBeater(predicted) : Definition.LightBeater(predicted);
        }

        /// <inheritdoc />
        public override void Observe(ExchangeHistory history)
        {
            for (int r = 0; r < counts.Length; r++)
            {
                counts[r] *= Fade;
            }
            counts[history.Response(history.Count - 1)] += 1;
        }
    }
}
