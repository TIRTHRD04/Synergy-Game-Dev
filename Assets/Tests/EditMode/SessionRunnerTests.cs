using AdaptiveOpponent.Core;
using AdaptiveOpponent.Sim;
using NUnit.Framework;

namespace AdaptiveOpponent.Tests
{
    public class SessionRunnerTests
    {
        private static readonly GameDefinition Def = TestData.Example();

        private static SessionRecord Run(string archetype, ulong seed, SessionMode mode, int hp = 60)
        {
            return SessionRunner.Run(Def, rng => new UniformOpponent(Def, rng), Archetypes.Create(archetype), seed, mode, hp);
        }

        [Test]
        public void HarnessSessionIsThreeMatchesOfOneHundred()
        {
            SessionRecord r = Run("Spammer", 1, SessionMode.Harness);
            Assert.AreEqual(300, r.Exchanges.Count);
            Assert.AreEqual(2, r.Exchanges[299].Match);
            Assert.AreEqual(99, r.Exchanges[299].IndexInMatch);
        }

        [Test]
        public void GameRulesEndAMatchAtZeroHpOrTheCap()
        {
            SessionRecord r = Run("Random", 2, SessionMode.GameRules, 10);
            Assert.AreEqual(3, r.OpponentHpAtEnd.Count);
            for (int m = 0; m < 3; m++)
            {
                Assert.IsTrue(r.OpponentHpAtEnd[m] <= 0 || r.PlayerHpAtEnd[m] <= 0, "match " + m + " ended by HP");
            }
            Assert.Less(r.Exchanges.Count, 3 * SessionRunner.ExchangeCap);
        }

        [Test]
        public void SameSeedGivesTheSameSession()
        {
            SessionRecord a = Run("OutGuesser", 7, SessionMode.Harness);
            SessionRecord b = Run("OutGuesser", 7, SessionMode.Harness);
            for (int i = 0; i < a.Exchanges.Count; i++)
            {
                Assert.AreEqual(a.Exchanges[i].Action, b.Exchanges[i].Action);
                Assert.AreEqual(a.Exchanges[i].Response, b.Exchanges[i].Response);
            }
        }

        [Test]
        public void EveryArchetypeScoresZeroAgainstUniformPlay()
        {
            // The simulator check: against sigma* every player's mean payoff is v* = 0 (tolerance 0.15).
            var seeds = new ulong[100];
            for (int i = 0; i < seeds.Length; i++)
            {
                seeds[i] = (ulong)(i + 1);
            }
            SuiteResult result = Suite.Run(Def, rng => new UniformOpponent(Def, rng), Archetypes.Suite, seeds, 99,
                SessionMode.Harness, 60);
            for (int a = 0; a < Archetypes.Suite.Length; a++)
            {
                Assert.AreEqual(0.0, result.Mean(a, s => s.MeanPayoff), 0.15, Archetypes.Suite[a]);
                Assert.AreEqual(0, result.GateOpenedSeeds(a));
            }
        }

        [Test]
        public void PairedStatsMatchHandComputedValues()
        {
            PairedComparison c = PairedStats.Compare(new[] { 0.0, 0.0, 0.0, 0.0 }, new[] { 1.0, 2.0, 3.0, 4.0 });
            Assert.AreEqual(2.5, c.MeanDifference, 1e-12);
            Assert.AreEqual(System.Math.Sqrt(5.0 / 3.0) / 2.0, c.StandardError, 1e-12);
            Assert.AreEqual(3.182, c.T, 1e-12);
            Assert.AreEqual(2.093, PairedStats.TCritical(19), 1e-12);
            Assert.AreEqual(1.980, PairedStats.TCritical(199), 1e-12);
            Assert.IsTrue(c.StrictlyBetter(true));
        }
    }
}
