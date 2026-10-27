using System;

namespace AdaptiveOpponent.Core
{
    /// <summary>How the Opponent picks its response once the evidence gate is open.</summary>
    public enum SelectorKind
    {
        /// <summary>Thompson Sampling over the experts' posteriors (ADR 0006).</summary>
        Thompson,
        /// <summary>Safe exploitation: lambda BR + (1 - lambda) sigma* with a budget of expected gifts (ADR 0007).</summary>
        SafeExploitation,
    }

    /// <summary>
    /// One Opponent config file (configs/*.json with "kind": "adaptive"). Every tuned value is explicit in the file,
    /// so a run is reproducible from the config and the git hash.
    /// </summary>
    public sealed class OpponentConfig
    {
        /// <summary>The config schema version this parser reads.</summary>
        public const int SchemaVersion = 1;

        /// <summary>The config's name, as logged.</summary>
        public string Name { get; private set; }

        /// <summary>The experts in mixing order, by name.</summary>
        public string[] Experts { get; private set; }

        /// <summary>Symmetric Dirichlet prior strength per action.</summary>
        public double Dirichlet { get; private set; }

        /// <summary>Fading factor per visit (or per exchange with global fading).</summary>
        public double Gamma { get; private set; }

        /// <summary>When true, every context fades every exchange (ablation row) instead of per visit.</summary>
        public bool GlobalFading { get; private set; }

        /// <summary>Counts are multiplied by this at match start (0.5).</summary>
        public double MatchStartCountScale { get; private set; }

        /// <summary>Fixed Share alpha.</summary>
        public double FixedShareAlpha { get; private set; }

        /// <summary>At match start, weights become (1 - mix) w + mix / K.</summary>
        public double MatchStartWeightMix { get; private set; }

        /// <summary>Evidence gate threshold, in nats.</summary>
        public double GateThreshold { get; private set; }

        /// <summary>An Insight needs the leading expert's top probability at least this.</summary>
        public double InsightMinProbability { get; private set; }

        /// <summary>An Insight needs at least this observed faded count in its context.</summary>
        public double InsightMinCount { get; private set; }

        /// <summary>The response selector once the gate is open.</summary>
        public SelectorKind Selector { get; private set; }

        /// <summary>Test mode: use the posterior mean instead of a posterior draw (same code path).</summary>
        public bool MeanMode { get; private set; }

        /// <summary>Safe exploitation: the starting budget k0, in damage units.</summary>
        public double SafeBudgetStart { get; private set; }

        /// <summary>Big-hit sequence expert: pseudo-counts per unit of strength on an exact match (s).</summary>
        public double BigHitStrength { get; private set; }

        /// <summary>Avoidance expert: how strongly a recorded big hit lowers the Opening's probability.</summary>
        public double AvoidanceBeta { get; private set; }

        /// <summary>Whether the Big-hit sequence expert also fires on broad matches (previous action only).</summary>
        public bool SequenceBroadMatches { get; private set; }

        /// <summary>Whether the Avoidance expert also fires on broad matches (previous action only).</summary>
        public bool AvoidanceBroadMatches { get; private set; }

        /// <summary>Returns true when <paramref name="expert"/> is in the expert list.</summary>
        public bool Uses(string expert) => Array.IndexOf(Experts, expert) >= 0;

        /// <summary>Parses config JSON; throws <see cref="JsonException"/> on a missing or malformed field.</summary>
        public static OpponentConfig Parse(string json)
        {
            JsonValue root = JsonReader.Parse(json);
            if (root.Get("schemaVersion").AsInt() != SchemaVersion)
            {
                throw new JsonException("opponent config schema " + root.Get("schemaVersion").AsInt() + " is not supported");
            }
            if (root.Get("kind").AsString() != "adaptive")
            {
                throw new JsonException("not an adaptive opponent config");
            }
            JsonValue experts = root.Get("experts");
            var config = new OpponentConfig
            {
                Name = root.Get("name").AsString(),
                Experts = new string[experts.Count],
                Dirichlet = root.Get("dirichlet").AsDouble(),
                Gamma = root.Get("gamma").AsDouble(),
                GlobalFading = root.Get("fading").AsString() == "global",
                MatchStartCountScale = root.Get("matchStartCountScale").AsDouble(),
                FixedShareAlpha = root.Get("fixedShareAlpha").AsDouble(),
                MatchStartWeightMix = root.Get("matchStartWeightMix").AsDouble(),
                GateThreshold = root.Get("gateThreshold").AsDouble(),
                InsightMinProbability = root.Get("insightMinProbability").AsDouble(),
                InsightMinCount = root.Get("insightMinCount").AsDouble(),
                Selector = ParseSelector(root.Get("selector").AsString()),
                MeanMode = root.Get("meanMode").AsBool(),
            };
            for (int i = 0; i < experts.Count; i++)
            {
                config.Experts[i] = experts[i].AsString();
            }
            if (config.Selector == SelectorKind.SafeExploitation)
            {
                config.SafeBudgetStart = root.Get("safeBudgetStart").AsDouble();
            }
            if (config.Uses("bigHitSequence"))
            {
                config.BigHitStrength = root.Get("bigHitStrength").AsDouble();
                config.SequenceBroadMatches = root.Get("sequenceBroadMatches").AsBool();
            }
            if (config.Uses("avoidance"))
            {
                config.AvoidanceBeta = root.Get("avoidanceBeta").AsDouble();
                config.AvoidanceBroadMatches = root.Get("avoidanceBroadMatches").AsBool();
            }
            return config;
        }

        private static SelectorKind ParseSelector(string name)
        {
            switch (name)
            {
                case "thompson": return SelectorKind.Thompson;
                case "safeExploitation": return SelectorKind.SafeExploitation;
                default: throw new JsonException("unknown selector \"" + name + "\"");
            }
        }
    }
}
