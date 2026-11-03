using AdaptiveOpponent.Core;
using NUnit.Framework;

namespace AdaptiveOpponent.Tests
{
    public class DefinitionTests
    {
        [Test]
        public void ConfigFileParsesToTheExampleTable()
        {
            GameDefinition fromFile = DefinitionParser.Parse(TestData.ReadRepoFile("configs/definition.json"));
            GameDefinition fromCode = TestData.Example();
            for (int r = 0; r < 6; r++)
            {
                for (int a = 0; a < 6; a++)
                {
                    Assert.AreEqual(fromCode.Payoff(r, a), fromFile.Payoff(r, a), "cell " + r + "," + a);
                }
            }
            for (int a = 0; a < 6; a++)
            {
                Assert.AreEqual(fromCode.AuthoredCounter(a), fromFile.AuthoredCounter(a));
            }
            Assert.AreEqual(3, fromFile.BigHitWindow);
            Assert.AreEqual(7, fromFile.BigHitThreshold);
            Assert.AreEqual(3, fromFile.MaxDamage);
        }

        [Test]
        public void UniformPlayScoresExactlyZeroAgainstEveryAction()
        {
            GameDefinition def = TestData.Example();
            for (int a = 0; a < 6; a++)
            {
                int sum = 0;
                for (int r = 0; r < 6; r++)
                {
                    sum += def.Payoff(r, a);
                }
                Assert.AreEqual(0, sum, "action " + a);
            }
        }

        [Test]
        public void EveryActionHasAHeavyAndALightCounter()
        {
            GameDefinition def = TestData.Example();
            for (int a = 0; a < 6; a++)
            {
                int heavy = def.AuthoredCounter(a);
                int light = def.OtherCounterOf(a);
                Assert.AreEqual(3, def.Payoff(heavy, a));
                Assert.AreEqual(1, def.Payoff(light, a));
                Assert.AreEqual((a + 5) % 6, heavy);
            }
        }

        [Test]
        public void BeatersOfAResponseAreOrderedHeavyFirst()
        {
            GameDefinition def = TestData.Example();
            for (int r = 0; r < 6; r++)
            {
                Assert.AreEqual(-3, def.Payoff(r, def.HeavyBeater(r)));
                Assert.AreEqual(-1, def.Payoff(r, def.LightBeater(r)));
                Assert.AreEqual(-3, def.MinPayoff(r));
            }
        }

        [Test]
        public void SumsBalancedButOneWinOneLossFourDrawsColumnIsRejected()
        {
            // Every row and column sums to 0, but column 0 holds +3, 0, 0, 0, 0, -3.
            int[][] payoff =
            {
                new[] { 3, -1, 0, 1, 0, -3 },
                new[] { 0, 3, 1, -1, 0, -3 },
                new[] { 0, 3, -3, -1, 1, 0 },
                new[] { 0, -3, 0, 1, -1, 3 },
                new[] { 0, -3, -1, 0, 1, 3 },
                new[] { -3, 1, 3, 0, -1, 0 },
            };
            var ex = Assert.Throws<DefinitionException>(() => TestData.FromPayoff(payoff));
            StringAssert.Contains("action 0", ex.Message);
            StringAssert.Contains("1/4/1", ex.Message);
        }

        [Test]
        public void UnbalancedRowSumIsRejected()
        {
            int[][] payoff = TestData.CirculantPayoff();
            payoff[0][1] = 2;
            var ex = Assert.Throws<DefinitionException>(() => TestData.FromPayoff(payoff));
            StringAssert.Contains("sum to", ex.Message);
        }

        [Test]
        public void DamageOnADrawIsRejected()
        {
            string json = TestData.ReadRepoFile("configs/definition.json").Replace("[0, 3, 1, 0, 1, 3]", "[1, 3, 1, 0, 1, 3]");
            Assert.Throws<DefinitionException>(() => DefinitionParser.Parse(json));
        }

        [Test]
        public void AuthoredCounterThatDoesNotBeatTheActionIsRejected()
        {
            var authored = new[] { 0, 0, 1, 2, 3, 4 };
            Assert.Throws<DefinitionException>(() => TestData.FromPayoff(TestData.CirculantPayoff(), authored));
        }

        [Test]
        public void WrongSchemaVersionIsRejected()
        {
            string json = TestData.ReadRepoFile("configs/definition.json").Replace("\"schemaVersion\": 1", "\"schemaVersion\": 2");
            Assert.Throws<DefinitionException>(() => DefinitionParser.Parse(json));
        }

        [Test]
        public void MalformedJsonThrowsJsonException()
        {
            Assert.Throws<JsonException>(() => DefinitionParser.Parse("{ \"schemaVersion\": 1, "));
        }

        [Test]
        public void ConfigFileNamesTheFightingMovesInRingOrder()
        {
            GameDefinition def = DefinitionParser.Parse(TestData.ReadRepoFile("configs/definition.json"));
            string[] ring = { "Strike", "Dodge", "Grab", "Heavy", "Guard", "Parry" };
            for (int m = 0; m < 6; m++)
            {
                Assert.AreEqual(ring[m], def.ActionName(m), "action " + m);
                Assert.AreEqual(ring[m], def.ResponseName(m), "both sides pick from the same moves (ADR 0012)");
                Assert.AreEqual(3, def.Payoff(m, (m + 1) % 6), ring[m] + " beats the next move cleanly");
                Assert.AreEqual(1, def.Payoff(m, (m + 2) % 6), ring[m] + " beats the move after that glancingly");
                Assert.IsNotEmpty(def.ActionHint(m), "every move has a hint");
            }
        }

        [Test]
        public void MissingActionHintsGiveEmptyStrings()
        {
            GameDefinition def = TestData.Example();
            for (int a = 0; a < 6; a++)
            {
                Assert.AreEqual("", def.ActionHint(a));
            }
        }

        [Test]
        public void WrongNumberOfActionHintsIsRejected()
        {
            string json = TestData.ReadRepoFile("configs/definition.json");
            int start = json.IndexOf("\"actionHints\"", System.StringComparison.Ordinal);
            int end = json.IndexOf(']', start);
            string broken = json.Substring(0, start) + "\"actionHints\": [\"only one\"" + json.Substring(end);
            var ex = Assert.Throws<DefinitionException>(() => DefinitionParser.Parse(broken));
            StringAssert.Contains("hint", ex.Message);
        }
    }
}
