using System.Collections.Generic;
using AdaptiveOpponent.Core;
using AdaptiveOpponent.Sim;
using NUnit.Framework;

namespace AdaptiveOpponent.Tests
{
    public class OpponentV2Tests
    {
        private static readonly GameDefinition Def = TestData.Example();

        [Test]
        public void BetaCrossesOneHalfWithinFifteenSymbolsOfTheFlip()
        {
            double l = 0;
            for (int i = 0; i < 50; i++)
            {
                l = CtwTree.UpdateLogOdds(l, System.Math.Log(0.5), System.Math.Log(0.25), 0.98);
            }
            Assert.AreEqual(CtwTree.Clamp, l, 1e-12, "log-odds are clamped at 6");
            int crossed = -1;
            for (int i = 0; i < 50 && crossed < 0; i++)
            {
                l = CtwTree.UpdateLogOdds(l, System.Math.Log(0.25), System.Math.Log(0.5), 0.98);
                if (l < 0)
                {
                    crossed = i + 1;
                }
            }
            Assert.That(crossed, Is.InRange(1, 15));
        }

        [Test]
        public void CtwForecastIsAProbabilityDistribution()
        {
            var config = TestConfigs.V1();
            var expert = new CtwExpert(CtwOrder.ActionFirst, Def, config, 400);
            var h = new ExchangeHistory(400, 6, 6);
            var rng = new Rng(3, 3);
            var f = new double[6];
            h.StartMatch();
            for (int i = 0; i < 200; i++)
            {
                expert.Predict(h, f);
                double total = 0;
                foreach (double p in f)
                {
                    Assert.Greater(p, 0);
                    total += p;
                }
                Assert.AreEqual(1.0, total, 1e-9);
                int a = rng.NextInt(6), r = rng.NextInt(6);
                h.Add(a, r, Def.Payoff(r, a));
                expert.Observe(h);
            }
        }

        [Test]
        public void ResponseFirstCtwLearnsAReactionToMyLastResponse()
        {
            var expert = new CtwExpert(CtwOrder.ResponseFirst, Def, TestConfigs.V1(), 400);
            var h = new ExchangeHistory(400, 6, 6);
            var rng = new Rng(4, 4);
            var f = new double[6];
            h.StartMatch();
            int correct = 0;
            for (int i = 0; i < 150; i++)
            {
                expert.Predict(h, f);
                int a = h.Count == 0 ? 0 : (h.Response(h.Count - 1) + 1) % 6;
                if (i >= 100 && BestResponse.TopAction(f, 6) == a)
                {
                    correct++;
                }
                int r = rng.NextInt(6);
                h.Add(a, r, Def.Payoff(r, a));
                expert.Observe(h);
            }
            Assert.Greater(correct / 50.0, 0.9);
        }

        [Test]
        public void LongestMatchPredictsWhatFollowedTheEarlierOccurrence()
        {
            var expert = new LongestMatchExpert(Def);
            var h = new ExchangeHistory(100, 6, 6);
            var f = new double[6];
            h.StartMatch();
            int[] actions = { 0, 3, 3, 1, 5, 2, 4 };
            int[] responses = { 2, 2, 5, 0, 1, 3, 4 };
            for (int i = 0; i < 7 + 3; i++)
            {
                h.Add(actions[i % 7], responses[i % 7], 0);
            }
            expert.Predict(h, f);
            Assert.AreEqual(3, expert.MatchLength);
            Assert.AreEqual(actions[3], BestResponse.TopAction(f, 6));
            Assert.AreEqual((1 + 0.5) / (1 + 3.0), f[actions[3]], 1e-12);
        }

        // A perfect model of either scripted player splits 0.9 between two beaters, so its top-1 ceiling is
        // 0.9 x 0.5 + 0.1 / 6 = 0.467; the meta expert must get close to that and well past order 0 alone.
        [Test]
        public void LevelOneExpertCatchesTheScriptedLevelOnePlayer()
        {
            double with = Late("[\"order0\", \"levelK1:order0\"]", "Level1", s => s.LateAccuracy);
            double without = Late("[\"order0\"]", "Level1", s => s.LateAccuracy);
            Assert.Greater(with, 0.40);
            Assert.Greater(with - without, 0.15);
        }

        [Test]
        public void MirrorOneExpertCatchesTheScriptedMirrorOnePlayer()
        {
            double with = Late("[\"order0\", \"mirror1\"]", "Mirror1", s => s.LateAccuracy);
            double without = Late("[\"order0\"]", "Mirror1", s => s.LateAccuracy);
            Assert.Greater(with, 0.40);
            Assert.Greater(with - without, 0.15);
        }

        private static double Late(string experts, string player, System.Func<SessionSummary, double> metric)
        {
            OpponentConfig config = TestConfigs.V1(new Dictionary<string, string> { ["experts"] = experts });
            double sum = 0;
            for (ulong seed = 1; seed <= 10; seed++)
            {
                SessionRecord record = SessionRunner.Run(Def, rng => new Opponent(Def, config, rng, 400),
                    Archetypes.Create(player), seed, SessionMode.Harness, 60);
                sum += metric(new SessionSummary(record, 6));
            }
            return sum / 10;
        }

        [Test]
        public void LevelTransformPlaysTheBeatersOfMyBestResponse()
        {
            var p = new double[] { 0, 0, 1, 0, 0, 0 };
            var result = new double[6];
            LevelKExpert.Transform(Def, p, 1, result);
            int br = BestResponse.Of(Def, p);
            Assert.AreEqual(0.95 * 0.5 + 0.05 / 6, result[Def.HeavyBeater(br)], 1e-12);
            Assert.AreEqual(0.95 * 0.5 + 0.05 / 6, result[Def.LightBeater(br)], 1e-12);
            Assert.AreEqual(0.05 / 6, result[2], 1e-12);
        }

        [Test]
        public void SafeExploitationNeverRisksMoreThanItsBudget()
        {
            var safe = new SafeExploitation(Def, 0.75);
            var rng = new Rng(8, 8);
            var p = new double[6];
            var alpha = new double[] { 0.3, 0.3, 0.3, 0.3, 0.3, 0.3 };
            for (int step = 0; step < 5000; step++)
            {
                rng.NextDirichlet(alpha, p, 6);
                int br = BestResponse.Of(Def, p);
                double lambda = safe.Lambda(br);
                double worst = double.PositiveInfinity;
                for (int a = 0; a < 6; a++)
                {
                    double uniform = 0;
                    for (int r = 0; r < 6; r++)
                    {
                        uniform += Def.Payoff(r, a) / 6.0;
                    }
                    worst = System.Math.Min(worst, lambda * Def.Payoff(br, a) + (1 - lambda) * uniform);
                }
                Assert.GreaterOrEqual(worst, 0 - safe.Budget - 1e-12, "step " + step);
                Assert.AreEqual(System.Math.Max(0, System.Math.Min(1, safe.Budget / 3)), lambda, 1e-12);
                safe.Observe(br, rng.NextInt(6), lambda);
            }
        }

        [Test]
        public void ClosedGateLeavesTheBudgetUnchanged()
        {
            var safe = new SafeExploitation(Def, 0.75);
            for (int a = 0; a < 6; a++)
            {
                safe.Observe(0, a, 0);
            }
            Assert.AreEqual(0.75, safe.Budget, 1e-12);
        }

        [Test]
        public void FullV2ConfigRunsWithinTheDecisionBudget()
        {
            OpponentConfig config = OpponentConfig.Parse(TestData.ReadRepoFile("configs/v2.json"));
            var seeds = new ulong[] { 1, 2, 3 };
            SuiteResult result = Suite.Run(Def, rng => new Opponent(Def, config, rng, 400), Archetypes.Suite, seeds, 5,
                SessionMode.Harness, 60);
            Assert.Less(result.DecisionMicroseconds(99), 1000.0, "p99 of Decide + Observe under 1 ms");
            Assert.AreEqual(0, result.GateOpenedSeeds(System.Array.IndexOf(Archetypes.Suite, "Random")));
        }

        [Test]
        public void LevelKBaseMustComeEarlier()
        {
            var config = TestConfigs.V1(new Dictionary<string, string> { ["experts"] = "[\"levelK1:order0\", \"order0\"]" });
            Assert.Throws<System.ArgumentException>(() => TestConfigs.Make(config));
        }
    }
}
