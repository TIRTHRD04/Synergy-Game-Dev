using AdaptiveOpponent.Core;
using NUnit.Framework;

namespace AdaptiveOpponent.Tests
{
    public class JsonReaderTests
    {
        [Test]
        public void ParsesNestedValues()
        {
            JsonValue v = JsonReader.Parse("{\"a\": [1, -2.5, 3e2], \"b\": {\"c\": \"x\\ny\\u0041\"}, \"t\": true, \"n\": null}");
            Assert.AreEqual(3, v.Get("a").Count);
            Assert.AreEqual(1, v.Get("a")[0].AsInt());
            Assert.AreEqual(-2.5, v.Get("a")[1].AsDouble());
            Assert.AreEqual(300, v.Get("a")[2].AsInt());
            Assert.AreEqual("x\nyA", v.Get("b").Get("c").AsString());
            Assert.IsTrue(v.Get("t").AsBool());
            Assert.AreEqual(JsonKind.Null, v.Get("n").Kind);
        }

        [TestCase("")]
        [TestCase("{")]
        [TestCase("[1, 2,]")]
        [TestCase("{\"a\": 1,}")]
        [TestCase("{\"a\": 1, \"a\": 2}")]
        [TestCase("01")]
        [TestCase("\"unterminated")]
        [TestCase("[1] 2")]
        [TestCase("tru")]
        public void RejectsMalformedJson(string text)
        {
            Assert.Throws<JsonException>(() => JsonReader.Parse(text));
        }

        [Test]
        public void WrongShapeThrowsJsonException()
        {
            JsonValue v = JsonReader.Parse("{\"a\": 1.5}");
            Assert.Throws<JsonException>(() => v.Get("a").AsInt());
            Assert.Throws<JsonException>(() => v.Get("missing"));
            Assert.Throws<JsonException>(() => v.Get("a").AsString());
        }
    }
}
