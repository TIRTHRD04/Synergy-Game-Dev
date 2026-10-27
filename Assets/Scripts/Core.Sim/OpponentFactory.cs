using System;
using AdaptiveOpponent.Core;

namespace AdaptiveOpponent.Sim
{
    /// <summary>Turns an Opponent config file's text into a factory the session runner can call once per session.</summary>
    public static class OpponentFactory
    {
        /// <summary>Parses the config and returns its name and a factory taking the Opponent's random stream.</summary>
        public static Func<Rng, IOpponent> FromConfig(GameDefinition definition, string json, out string name)
        {
            JsonValue root = JsonReader.Parse(json);
            name = root.Get("name").AsString();
            string kind = root.Get("kind").AsString();
            switch (kind)
            {
                case "uniform":
                    return rng => new UniformOpponent(definition, rng);
                default:
                    throw new JsonException("unknown opponent kind \"" + kind + "\"");
            }
        }

        /// <summary>Reads the global seed from configs/seed.json text.</summary>
        public static ulong GlobalSeed(string json) => JsonReader.Parse(json).Get("globalSeed").AsULong();
    }
}
