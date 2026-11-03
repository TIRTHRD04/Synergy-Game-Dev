using AdaptiveOpponent.Core;

namespace AdaptiveOpponent.Sim
{
    /// <summary>
    /// Scripted level-1 player, used only for the level-k/mirror keep rule. It is exactly the player the level-1
    /// expert over order 0 models: it predicts itself the way the Opponent's order-0 expert does (counts faded by 0.98
    /// each exchange, Dirichlet 0.5, halved at match start), takes the Opponent's best response to that, and plays one
    /// of the two actions that beat it, picked at random, with 10% uniform noise.
    /// </summary>
    public sealed class Level1Player : ArchetypeBase
    {
        private const double Fade = 0.98;
        private const double Prior = 0.5;
        private const double Noise = 0.1;
        private double[] ownCounts;
        private double[] forecast;
        private int matchesSeen;

        /// <inheritdoc />
        public override string Name => "Level1";

        /// <inheritdoc />
        protected override void Setup(Rng setup)
        {
            ownCounts = new double[Definition.ActionCount];
            forecast = new double[Definition.ActionCount];
            matchesSeen = 0;
        }

        /// <inheritdoc />
        public override int Act(ExchangeHistory history)
        {
            if (history.MatchCount > matchesSeen)
            {
                matchesSeen = history.MatchCount;
                for (int a = 0; a < ownCounts.Length; a++)
                {
                    ownCounts[a] *= 0.5;
                }
            }
            if (Chance(Noise))
            {
                return RandomAction();
            }
            double total = 0;
            for (int a = 0; a < ownCounts.Length; a++)
            {
                total += ownCounts[a] + Prior;
            }
            for (int a = 0; a < ownCounts.Length; a++)
            {
                forecast[a] = (ownCounts[a] + Prior) / total;
            }
            return EitherBeater(BestResponse.Of(Definition, forecast));
        }

        /// <inheritdoc />
        public override void Observe(ExchangeHistory history)
        {
            for (int a = 0; a < ownCounts.Length; a++)
            {
                ownCounts[a] *= Fade;
            }
            ownCounts[history.Action(history.Count - 1)] += 1;
        }
    }

    /// <summary>
    /// Scripted mirror-1 player, used only for the level-k/mirror keep rule. It is exactly the player the mirror-1
    /// expert models: it predicts the Opponent's next response with the same response-first CTW the Opponent's mirror
    /// model uses, and plays one of the two actions that beat the most likely response, picked at random, with 10%
    /// uniform noise.
    /// </summary>
    public sealed class Mirror1Player : ArchetypeBase
    {
        private const double Noise = 0.1;
        private const double Fade = 0.98;
        private readonly int[] context = new int[CtwTree.Depth];
        private readonly int[] nodes = new int[CtwTree.Depth + 1];
        private CtwTree tree;
        private double[] q;
        private int matchesSeen;

        /// <inheritdoc />
        public override string Name => "Mirror1";

        /// <inheritdoc />
        protected override void Setup(Rng setup)
        {
            int r = Definition.ResponseCount;
            tree = new CtwTree(r, r + 1, Fade, SessionRunner.Matches * SessionRunner.ExchangeCap);
            q = new double[r];
            matchesSeen = 0;
        }

        /// <inheritdoc />
        public override int Act(ExchangeHistory history)
        {
            if (history.MatchCount > matchesSeen)
            {
                matchesSeen = history.MatchCount;
                tree.Scale(0.5, 0.5);
            }
            if (Chance(Noise))
            {
                return RandomAction();
            }
            CtwContext.Fill(history, history.Count, CtwOrder.ResponseFirst, context);
            tree.Predict(context, q, nodes);
            return EitherBeater(BestResponse.TopAction(q, Definition.ResponseCount));
        }

        /// <inheritdoc />
        public override void Observe(ExchangeHistory history)
        {
            int last = history.Count - 1;
            CtwContext.Fill(history, last, CtwOrder.ResponseFirst, context);
            tree.Update(context, history.Response(last));
        }
    }
}
