using AdaptiveOpponent.Core;

namespace AdaptiveOpponent.Sim
{
    /// <summary>
    /// The Nash strategy as an opponent: a uniformly random response and a uniform forecast, never learning.
    /// It is the simulator's reference: any player's mean payoff against it must be v* = 0.
    /// </summary>
    public sealed class UniformOpponent : IOpponent
    {
        private readonly GameDefinition definition;
        private readonly Rng rng;

        /// <summary>Creates the uniform opponent with its own random stream.</summary>
        public UniformOpponent(GameDefinition definition, Rng rng)
        {
            this.definition = definition;
            this.rng = rng;
        }

        /// <inheritdoc />
        public int TopPrediction => 0;

        /// <inheritdoc />
        public bool GateOpen => false;

        /// <inheritdoc />
        public double Lambda => 0;

        /// <inheritdoc />
        public CommentaryKind Commentary => CommentaryKind.None;

        /// <inheritdoc />
        public void StartMatch()
        {
        }

        /// <inheritdoc />
        public int Decide() => rng.NextInt(definition.ResponseCount);

        /// <inheritdoc />
        public void Observe(int action)
        {
        }

        /// <inheritdoc />
        public double ForecastOf(int action) => 1.0 / definition.ActionCount;
    }
}
