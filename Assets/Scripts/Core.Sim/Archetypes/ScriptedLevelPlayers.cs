using AdaptiveOpponent.Core;

namespace AdaptiveOpponent.Sim
{
    /// <summary>
    /// Scripted level-1 player, used only for the level-k/mirror keep rule: it assumes the Opponent predicts it from
    /// its own action frequencies (faded 0.9), works out the Opponent's best response to that, and plays the heavy
    /// beater of that response, with 10% uniform noise.
    /// </summary>
    public sealed class Level1Player : ArchetypeBase
    {
        private const double Fade = 0.9;
        private const double Noise = 0.1;
        private double[] ownCounts;
        private double[] forecast;

        /// <inheritdoc />
        public override string Name => "Level1";

        /// <inheritdoc />
        protected override void Setup(Rng setup)
        {
            ownCounts = new double[Definition.ActionCount];
            forecast = new double[Definition.ActionCount];
        }

        /// <inheritdoc />
        public override int Act(ExchangeHistory history)
        {
            if (Chance(Noise))
            {
                return RandomAction();
            }
            double total = 0;
            for (int a = 0; a < ownCounts.Length; a++)
            {
                total += ownCounts[a] + 0.5;
            }
            for (int a = 0; a < ownCounts.Length; a++)
            {
                forecast[a] = (ownCounts[a] + 0.5) / total;
            }
            return Definition.HeavyBeater(BestResponse.Of(Definition, forecast));
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
    /// Scripted mirror-1 player, used only for the level-k/mirror keep rule: it predicts the Opponent's next response
    /// from counts of what followed the Opponent's previous response, and plays the heavy beater of the most likely
    /// one, with 10% uniform noise.
    /// </summary>
    public sealed class Mirror1Player : ArchetypeBase
    {
        private const double Noise = 0.1;
        private double[] counts;

        /// <inheritdoc />
        public override string Name => "Mirror1";

        /// <inheritdoc />
        protected override void Setup(Rng setup)
        {
            int r = Definition.ResponseCount;
            counts = new double[(r + 1) * r];
        }

        /// <inheritdoc />
        public override int Act(ExchangeHistory history)
        {
            if (Chance(Noise))
            {
                return RandomAction();
            }
            int r = Definition.ResponseCount;
            int row = history.PrevResponseBefore(history.Count) * r;
            int predicted = 0;
            for (int i = 1; i < r; i++)
            {
                if (counts[row + i] > counts[row + predicted])
                {
                    predicted = i;
                }
            }
            return Definition.HeavyBeater(predicted);
        }

        /// <inheritdoc />
        public override void Observe(ExchangeHistory history)
        {
            int last = history.Count - 1;
            int row = history.PrevResponseBefore(last) * Definition.ResponseCount;
            counts[row + history.Response(last)] += 1;
        }
    }
}
