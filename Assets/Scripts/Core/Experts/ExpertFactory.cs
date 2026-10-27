using System;

namespace AdaptiveOpponent.Core
{
    /// <summary>Builds the experts a config lists, in its order.</summary>
    public static class ExpertFactory
    {
        /// <summary>Creates one expert per name in <paramref name="config"/>; throws on an unknown name.</summary>
        public static IExpert[] Create(GameDefinition definition, OpponentConfig config, BigHitMemory bigHits)
        {
            var experts = new IExpert[config.Experts.Length];
            for (int i = 0; i < experts.Length; i++)
            {
                experts[i] = Create(config.Experts[i], definition, config, bigHits);
            }
            return experts;
        }

        private static IExpert Create(string name, GameDefinition definition, OpponentConfig config, BigHitMemory bigHits)
        {
            switch (name)
            {
                case "order0": return new CountExpert(CountContext.Order0, definition, config);
                case "order1": return new CountExpert(CountContext.Order1, definition, config);
                case "order2": return new CountExpert(CountContext.Order2, definition, config);
                case "actionResponse": return new CountExpert(CountContext.ActionResponse, definition, config);
                case "bigHitSequence":
                    return new BigHitSequenceExpert(definition, bigHits.OnOpponent, config.BigHitStrength, config.BroadMatches);
                case "avoidance":
                    return new AvoidanceExpert(definition, bigHits.OnPlayer, config.AvoidanceBeta, config.BroadMatches);
                default: throw new ArgumentException("unknown expert \"" + name + "\"");
            }
        }
    }
}
