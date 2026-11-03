namespace AdaptiveOpponent.Core
{
    /// <summary>
    /// Reads <c>configs/definition.json</c> text into a validated <see cref="GameDefinition"/>. The caller reads the
    /// file; Core never does I/O (ADR 0002).
    /// </summary>
    public static class DefinitionParser
    {
        /// <summary>The definition schema version this parser reads.</summary>
        public const int SchemaVersion = 1;

        /// <summary>
        /// Parses and validates definition JSON. Throws <see cref="JsonException"/> on malformed JSON and
        /// <see cref="DefinitionException"/> on data that breaks a rule.
        /// </summary>
        public static GameDefinition Parse(string json)
        {
            JsonValue root = JsonReader.Parse(json);
            int version = root.Get("schemaVersion").AsInt();
            if (version != SchemaVersion)
            {
                throw new DefinitionException("definition schema " + version + " is not supported (expected " + SchemaVersion + ")");
            }
            string[] actions = ReadStrings(root.Get("actions"));
            string[] responses = ReadStrings(root.Get("responses"));
            JsonValue outcomeRows = root.Get("outcomes");
            var outcomes = new Outcome[outcomeRows.Count][];
            for (int r = 0; r < outcomes.Length; r++)
            {
                outcomes[r] = ReadOutcomeRow(outcomeRows[r].AsString(), r);
            }
            JsonValue damageRows = root.Get("damage");
            var damage = new int[damageRows.Count][];
            for (int r = 0; r < damage.Length; r++)
            {
                damage[r] = ReadInts(damageRows[r]);
            }
            int[] counters = ReadInts(root.Get("authoredCounters"));
            JsonValue bigHit = root.Get("bigHit");
            return new GameDefinition(actions, responses, outcomes, damage, counters,
                bigHit.Get("window").AsInt(), bigHit.Get("threshold").AsInt());
        }

        private static Outcome[] ReadOutcomeRow(string row, int index)
        {
            var result = new Outcome[row.Length];
            for (int a = 0; a < row.Length; a++)
            {
                switch (row[a])
                {
                    case 'W': result[a] = Outcome.Win; break;
                    case 'D': result[a] = Outcome.Draw; break;
                    case 'L': result[a] = Outcome.Loss; break;
                    default: throw new DefinitionException("outcome row " + index + " may only contain W, D and L");
                }
            }
            return result;
        }

        private static string[] ReadStrings(JsonValue array)
        {
            var result = new string[array.Count];
            for (int i = 0; i < result.Length; i++)
            {
                result[i] = array[i].AsString();
            }
            return result;
        }

        private static int[] ReadInts(JsonValue array)
        {
            var result = new int[array.Count];
            for (int i = 0; i < result.Length; i++)
            {
                result[i] = array[i].AsInt();
            }
            return result;
        }
    }
}
