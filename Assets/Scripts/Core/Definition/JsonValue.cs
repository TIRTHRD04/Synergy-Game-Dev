using System;
using System.Collections.Generic;

namespace AdaptiveOpponent.Core
{
    /// <summary>The kind of a parsed JSON value.</summary>
    public enum JsonKind
    {
        /// <summary>The JSON literal null.</summary>
        Null,
        /// <summary>true or false.</summary>
        Bool,
        /// <summary>A number, held as a double.</summary>
        Number,
        /// <summary>A string.</summary>
        String,
        /// <summary>An ordered list of values.</summary>
        Array,
        /// <summary>A set of named values.</summary>
        Object,
    }

    /// <summary>
    /// One node of a parsed JSON document. Used only to read config files at load time, never on the decision path.
    /// </summary>
    public sealed class JsonValue
    {
        private readonly double number;
        private readonly string text;
        private readonly bool flag;
        private readonly List<JsonValue> items;
        private readonly Dictionary<string, JsonValue> fields;

        private JsonValue(JsonKind kind, double number, string text, bool flag,
            List<JsonValue> items, Dictionary<string, JsonValue> fields)
        {
            Kind = kind;
            this.number = number;
            this.text = text;
            this.flag = flag;
            this.items = items;
            this.fields = fields;
        }

        /// <summary>What kind of value this is.</summary>
        public JsonKind Kind { get; }

        /// <summary>The number of items in an array.</summary>
        public int Count => Expect(JsonKind.Array).items.Count;

        /// <summary>The item at <paramref name="index"/> of an array.</summary>
        public JsonValue this[int index] => Expect(JsonKind.Array).items[index];

        internal static JsonValue Null() => new JsonValue(JsonKind.Null, 0, null, false, null, null);
        internal static JsonValue FromBool(bool value) => new JsonValue(JsonKind.Bool, 0, null, value, null, null);
        internal static JsonValue FromNumber(double value) => new JsonValue(JsonKind.Number, value, null, false, null, null);
        internal static JsonValue FromString(string value) => new JsonValue(JsonKind.String, 0, value, false, null, null);
        internal static JsonValue FromArray(List<JsonValue> value) => new JsonValue(JsonKind.Array, 0, null, false, value, null);
        internal static JsonValue FromObject(Dictionary<string, JsonValue> value) => new JsonValue(JsonKind.Object, 0, null, false, null, value);

        /// <summary>Returns true and the field when this object has a field called <paramref name="name"/>.</summary>
        public bool TryGet(string name, out JsonValue value)
        {
            return Expect(JsonKind.Object).fields.TryGetValue(name, out value);
        }

        /// <summary>Returns the field called <paramref name="name"/>, or throws <see cref="JsonException"/> if it is missing.</summary>
        public JsonValue Get(string name)
        {
            if (!TryGet(name, out JsonValue value))
            {
                throw new JsonException("missing field \"" + name + "\"");
            }
            return value;
        }

        /// <summary>This number as a double.</summary>
        public double AsDouble() => Expect(JsonKind.Number).number;

        /// <summary>This number as an int; throws if it is not a whole number in range.</summary>
        public int AsInt()
        {
            double value = AsDouble();
            if (value != Math.Floor(value) || value < int.MinValue || value > int.MaxValue)
            {
                throw new JsonException("expected a whole number, got " + value);
            }
            return (int)value;
        }

        /// <summary>This number as an unsigned 64-bit integer; throws if it is not a non-negative whole number.</summary>
        public ulong AsULong()
        {
            double value = AsDouble();
            if (value != Math.Floor(value) || value < 0 || value > 9007199254740992.0)
            {
                throw new JsonException("expected a non-negative whole number up to 2^53, got " + value);
            }
            return (ulong)value;
        }

        /// <summary>This string.</summary>
        public string AsString() => Expect(JsonKind.String).text;

        /// <summary>This boolean.</summary>
        public bool AsBool() => Expect(JsonKind.Bool).flag;

        private JsonValue Expect(JsonKind kind)
        {
            if (Kind != kind)
            {
                throw new JsonException("expected " + kind + ", got " + Kind);
            }
            return this;
        }
    }

    /// <summary>Thrown when a JSON document is malformed or does not have the expected shape.</summary>
    public sealed class JsonException : FormatException
    {
        /// <summary>Creates the exception with a message that says what was wrong.</summary>
        public JsonException(string message) : base(message)
        {
        }
    }
}
