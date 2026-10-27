using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace AdaptiveOpponent.Core
{
    /// <summary>
    /// A small strict JSON parser (RFC 8259 values, no comments, no trailing commas), so Core needs no package
    /// and no UnityEngine. It only reads config text; the caller does any file I/O.
    /// </summary>
    public static class JsonReader
    {
        /// <summary>Parses <paramref name="text"/> or throws <see cref="JsonException"/> naming the position.</summary>
        public static JsonValue Parse(string text)
        {
            if (text == null)
            {
                throw new JsonException("no JSON text");
            }
            var parser = new Parser(text);
            JsonValue value = parser.ParseValue();
            parser.SkipWhitespace();
            if (!parser.AtEnd)
            {
                throw parser.Error("unexpected text after the value");
            }
            return value;
        }

        private sealed class Parser
        {
            private readonly string text;
            private int pos;

            public Parser(string text)
            {
                this.text = text;
            }

            public bool AtEnd => pos >= text.Length;

            public JsonException Error(string message) => new JsonException(message + " at position " + pos);

            public void SkipWhitespace()
            {
                while (!AtEnd && (text[pos] == ' ' || text[pos] == '\t' || text[pos] == '\n' || text[pos] == '\r'))
                {
                    pos++;
                }
            }

            public JsonValue ParseValue()
            {
                SkipWhitespace();
                if (AtEnd)
                {
                    throw Error("unexpected end of JSON");
                }
                char c = text[pos];
                switch (c)
                {
                    case '{': return ParseObject();
                    case '[': return ParseArray();
                    case '"': return JsonValue.FromString(ParseString());
                    case 't': ExpectWord("true"); return JsonValue.FromBool(true);
                    case 'f': ExpectWord("false"); return JsonValue.FromBool(false);
                    case 'n': ExpectWord("null"); return JsonValue.Null();
                    default:
                        if (c == '-' || (c >= '0' && c <= '9'))
                        {
                            return JsonValue.FromNumber(ParseNumber());
                        }
                        throw Error("unexpected character '" + c + "'");
                }
            }

            private JsonValue ParseObject()
            {
                pos++;
                var fields = new Dictionary<string, JsonValue>();
                SkipWhitespace();
                if (!AtEnd && text[pos] == '}')
                {
                    pos++;
                    return JsonValue.FromObject(fields);
                }
                while (true)
                {
                    SkipWhitespace();
                    if (AtEnd || text[pos] != '"')
                    {
                        throw Error("expected a field name");
                    }
                    string name = ParseString();
                    if (fields.ContainsKey(name))
                    {
                        throw Error("duplicate field \"" + name + "\"");
                    }
                    SkipWhitespace();
                    Expect(':');
                    fields[name] = ParseValue();
                    SkipWhitespace();
                    if (AtEnd)
                    {
                        throw Error("unterminated object");
                    }
                    if (text[pos] == ',')
                    {
                        pos++;
                        continue;
                    }
                    Expect('}');
                    return JsonValue.FromObject(fields);
                }
            }

            private JsonValue ParseArray()
            {
                pos++;
                var items = new List<JsonValue>();
                SkipWhitespace();
                if (!AtEnd && text[pos] == ']')
                {
                    pos++;
                    return JsonValue.FromArray(items);
                }
                while (true)
                {
                    items.Add(ParseValue());
                    SkipWhitespace();
                    if (AtEnd)
                    {
                        throw Error("unterminated array");
                    }
                    if (text[pos] == ',')
                    {
                        pos++;
                        continue;
                    }
                    Expect(']');
                    return JsonValue.FromArray(items);
                }
            }

            private string ParseString()
            {
                pos++;
                var sb = new StringBuilder();
                while (true)
                {
                    if (AtEnd)
                    {
                        throw Error("unterminated string");
                    }
                    char c = text[pos++];
                    if (c == '"')
                    {
                        return sb.ToString();
                    }
                    if (c < ' ')
                    {
                        throw Error("control character in string");
                    }
                    if (c != '\\')
                    {
                        sb.Append(c);
                        continue;
                    }
                    if (AtEnd)
                    {
                        throw Error("unterminated escape");
                    }
                    char e = text[pos++];
                    switch (e)
                    {
                        case '"': sb.Append('"'); break;
                        case '\\': sb.Append('\\'); break;
                        case '/': sb.Append('/'); break;
                        case 'b': sb.Append('\b'); break;
                        case 'f': sb.Append('\f'); break;
                        case 'n': sb.Append('\n'); break;
                        case 'r': sb.Append('\r'); break;
                        case 't': sb.Append('\t'); break;
                        case 'u': sb.Append(ParseHex4()); break;
                        default: throw Error("bad escape '\\" + e + "'");
                    }
                }
            }

            private char ParseHex4()
            {
                if (pos + 4 > text.Length)
                {
                    throw Error("short \\u escape");
                }
                string hex = text.Substring(pos, 4);
                if (!int.TryParse(hex, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out int code))
                {
                    throw Error("bad \\u escape");
                }
                pos += 4;
                return (char)code;
            }

            private double ParseNumber()
            {
                int start = pos;
                if (text[pos] == '-')
                {
                    pos++;
                }
                if (AtEnd || !IsDigit(text[pos]))
                {
                    throw Error("expected a digit");
                }
                if (text[pos] == '0')
                {
                    pos++;
                }
                else
                {
                    SkipDigits();
                }
                if (!AtEnd && text[pos] == '.')
                {
                    pos++;
                    if (AtEnd || !IsDigit(text[pos]))
                    {
                        throw Error("expected a digit after '.'");
                    }
                    SkipDigits();
                }
                if (!AtEnd && (text[pos] == 'e' || text[pos] == 'E'))
                {
                    pos++;
                    if (!AtEnd && (text[pos] == '+' || text[pos] == '-'))
                    {
                        pos++;
                    }
                    if (AtEnd || !IsDigit(text[pos]))
                    {
                        throw Error("expected a digit in the exponent");
                    }
                    SkipDigits();
                }
                return double.Parse(text.Substring(start, pos - start), NumberStyles.Float, CultureInfo.InvariantCulture);
            }

            private void SkipDigits()
            {
                while (!AtEnd && IsDigit(text[pos]))
                {
                    pos++;
                }
            }

            private static bool IsDigit(char c) => c >= '0' && c <= '9';

            private void Expect(char c)
            {
                if (AtEnd || text[pos] != c)
                {
                    throw Error("expected '" + c + "'");
                }
                pos++;
            }

            private void ExpectWord(string word)
            {
                if (pos + word.Length > text.Length || string.CompareOrdinal(text, pos, word, 0, word.Length) != 0)
                {
                    throw Error("expected " + word);
                }
                pos += word.Length;
            }
        }
    }
}
