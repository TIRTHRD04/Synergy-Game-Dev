using System.Collections.Generic;
using System.Text;
using AdaptiveOpponent.Core;

namespace AdaptiveOpponent.Tests
{
    /// <summary>Opponent configs for tests: configs/v1.json with some fields replaced.</summary>
    public static class TestConfigs
    {
        /// <summary>
        /// configs/v1.json with <paramref name="overrides"/> (field name to raw JSON value) replacing or adding fields.
        /// </summary>
        public static OpponentConfig V1(Dictionary<string, string> overrides = null)
        {
            JsonValue v1 = JsonReader.Parse(TestData.ReadRepoFile("configs/v1.json"));
            string[] fields =
            {
                "schemaVersion", "name", "kind", "experts", "dirichlet", "gamma", "fading", "matchStartCountScale",
                "fixedShareAlpha", "matchStartWeightMix", "gateThreshold", "insightMinProbability", "insightMinCount",
                "selector", "meanMode",
            };
            var values = new Dictionary<string, string>();
            foreach (string f in fields)
            {
                values[f] = Raw(v1.Get(f));
            }
            if (overrides != null)
            {
                foreach (var kv in overrides)
                {
                    values[kv.Key] = kv.Value;
                }
            }
            var sb = new StringBuilder("{");
            bool first = true;
            foreach (var kv in values)
            {
                sb.Append(first ? "" : ",").Append('"').Append(kv.Key).Append("\":").Append(kv.Value);
                first = false;
            }
            return OpponentConfig.Parse(sb.Append('}').ToString());
        }

        /// <summary>An Opponent for one test session, on a fixed stream.</summary>
        public static Opponent Make(OpponentConfig config, ulong seed = 1)
        {
            return new Opponent(TestData.Example(), config, new Rng(seed, 9), 400);
        }

        private static string Raw(JsonValue v)
        {
            switch (v.Kind)
            {
                case JsonKind.String: return "\"" + v.AsString() + "\"";
                case JsonKind.Bool: return v.AsBool() ? "true" : "false";
                case JsonKind.Number: return v.AsDouble().ToString("R", System.Globalization.CultureInfo.InvariantCulture);
                case JsonKind.Array:
                    var parts = new string[v.Count];
                    for (int i = 0; i < v.Count; i++)
                    {
                        parts[i] = Raw(v[i]);
                    }
                    return "[" + string.Join(",", parts) + "]";
                default: return "null";
            }
        }
    }
}
