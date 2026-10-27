using System;

namespace AdaptiveOpponent.Core
{
    /// <summary>
    /// Builds the experts a config lists, in its order. Level-k experts are named "levelK1:base" or "levelK2:base"
    /// and must come after their base expert; they read its forecast from the shared board.
    /// </summary>
    public static class ExpertFactory
    {
        /// <summary>Creates one expert per name in <paramref name="config"/>; throws on an unknown or misordered name.</summary>
        /// <param name="definition">The game data.</param>
        /// <param name="config">The Opponent config.</param>
        /// <param name="bigHits">The Opponent's big-hit records.</param>
        /// <param name="historyCapacity">The most exchanges in the session (sizes CTW node pools).</param>
        /// <param name="board">Per-expert forecast buffers, written by the Opponent each exchange.</param>
        /// <param name="awakeBoard">Per-expert awake flags, written by the Opponent each exchange.</param>
        public static IExpert[] Create(GameDefinition definition, OpponentConfig config, BigHitMemory bigHits,
            int historyCapacity, double[][] board, bool[] awakeBoard)
        {
            var experts = new IExpert[config.Experts.Length];
            MirrorModel mirror = null;
            for (int i = 0; i < experts.Length; i++)
            {
                string name = config.Experts[i];
                if (name.StartsWith("levelK", StringComparison.Ordinal))
                {
                    experts[i] = LevelK(name, i, experts, definition, config, board, awakeBoard);
                    continue;
                }
                if (name == "mirror1" || name == "mirror2")
                {
                    mirror = mirror ?? new MirrorModel(definition, config, historyCapacity);
                    experts[i] = new MirrorExpert(definition, mirror, name == "mirror1" ? 1 : 2, config.Gamma);
                    continue;
                }
                experts[i] = Simple(name, definition, config, bigHits, historyCapacity);
            }
            return experts;
        }

        private static IExpert Simple(string name, GameDefinition definition, OpponentConfig config, BigHitMemory bigHits,
            int historyCapacity)
        {
            switch (name)
            {
                case "order0": return new CountExpert(CountContext.Order0, definition, config);
                case "order1": return new CountExpert(CountContext.Order1, definition, config);
                case "order2": return new CountExpert(CountContext.Order2, definition, config);
                case "actionResponse": return new CountExpert(CountContext.ActionResponse, definition, config);
                case "ctwActionFirst": return new CtwExpert(CtwOrder.ActionFirst, definition, config, historyCapacity);
                case "ctwResponseFirst": return new CtwExpert(CtwOrder.ResponseFirst, definition, config, historyCapacity);
                case "longestMatch": return new LongestMatchExpert(definition);
                case "bigHitSequence":
                    return new BigHitSequenceExpert(definition, bigHits.OnOpponent, config.BigHitStrength, config.SequenceBroadMatches);
                case "avoidance":
                    return new AvoidanceExpert(definition, bigHits.OnPlayer, config.AvoidanceBeta, config.AvoidanceBroadMatches);
                default: throw new ArgumentException("unknown expert \"" + name + "\"");
            }
        }

        private static IExpert LevelK(string name, int index, IExpert[] built, GameDefinition definition,
            OpponentConfig config, double[][] board, bool[] awakeBoard)
        {
            int colon = name.IndexOf(':');
            if (colon < 0 || (name.Substring(0, colon) != "levelK1" && name.Substring(0, colon) != "levelK2"))
            {
                throw new ArgumentException("level-k experts are named levelK1:<base> or levelK2:<base>, got \"" + name + "\"");
            }
            string baseName = name.Substring(colon + 1);
            int baseIndex = Array.IndexOf(config.Experts, baseName);
            if (baseIndex < 0 || baseIndex >= index)
            {
                throw new ArgumentException("\"" + name + "\" needs \"" + baseName + "\" earlier in the expert list");
            }
            int level = name[6] - '0';
            return new LevelKExpert(definition, built[baseIndex], baseIndex, level, board, awakeBoard, config.Gamma);
        }
    }
}
