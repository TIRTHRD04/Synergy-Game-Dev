using System.Collections.Generic;
using AdaptiveOpponent.Core;
using NUnit.Framework;

namespace AdaptiveOpponent.Tests
{
    public class OpponentV1Tests
    {
        private static readonly GameDefinition Def = TestData.Example();

        [Test]
        public void HabitSwitchIsRecoveredWithinThirtyExchanges()
        {
            Opponent opponent = TestConfigs.Make(TestConfigs.V1());
            opponent.StartMatch();
            for (int i = 0; i < 100; i++)
            {
                opponent.Decide();
                opponent.Observe(1);
            }
            int streak = 0;
            int recoveredAt = -1;
            for (int i = 0; i < 30 && recoveredAt < 0; i++)
            {
                opponent.Decide();
                streak = opponent.TopPrediction == 4 ? streak + 1 : 0;
                if (streak == 5)
                {
                    recoveredAt = i;
                }
                opponent.Observe(4);
            }
            Assert.GreaterOrEqual(recoveredAt, 0, "top prediction was not action 4 for 5 in a row within 30 exchanges");
        }

        [Test]
        public void GateOpensOnlyOnEvidence()
        {
            var gate = new EvidenceGate(System.Math.Log(1000), 6);
            for (int i = 0; i < 1000; i++)
            {
                gate.Observe(1.0 / 6);
            }
            Assert.IsFalse(gate.IsOpen);
            Assert.AreEqual(0.0, gate.Score, 1e-9);
            var strong = new EvidenceGate(System.Math.Log(1000), 6);
            int n = 0;
            while (!strong.IsOpen)
            {
                strong.Observe(0.5);
                n++;
            }
            Assert.AreEqual(7, n, "ln 1000 / ln 3 = 6.29, so the 7th exchange opens it");
        }

        [Test]
        public void GateLatchesOpen()
        {
            var gate = new EvidenceGate(1.0, 6);
            gate.Observe(0.9);
            Assert.IsTrue(gate.IsOpen);
            for (int i = 0; i < 50; i++)
            {
                gate.Observe(0.01);
            }
            Assert.IsTrue(gate.IsOpen);
        }

        [Test]
        public void NoInsightWhileTheGateIsClosed()
        {
            Opponent opponent = TestConfigs.Make(TestConfigs.V1(new Dictionary<string, string> { ["gateThreshold"] = "1e9" }));
            opponent.StartMatch();
            for (int i = 0; i < 200; i++)
            {
                opponent.Decide();
                Assert.AreEqual(CommentaryKind.None, opponent.Commentary);
                Assert.AreEqual(0.0, opponent.Lambda);
                opponent.Observe(2);
            }
        }

        [Test]
        public void InsightNeedsFiveObservedCountsInItsContext()
        {
            var config = TestConfigs.V1(new Dictionary<string, string>
            {
                ["gateThreshold"] = "-1",
                ["experts"] = "[\"order0\"]",
                ["gamma"] = "1.0",
            });
            Opponent opponent = TestConfigs.Make(config);
            opponent.StartMatch();
            var seen = new List<CommentaryKind>();
            for (int i = 0; i < 8; i++)
            {
                opponent.Decide();
                seen.Add(opponent.Commentary);
                opponent.Observe(3);
            }
            // After 3 plays, p(3) = 3.5/6 >= 0.4 but only 3 observed counts; the 6th decision has 5.
            Assert.AreEqual(CommentaryKind.None, seen[4]);
            Assert.AreEqual(CommentaryKind.Insight, seen[5]);
            Assert.AreEqual(InsightKind.Frequency, opponent.CurrentInsight.Kind);
            Assert.AreEqual(3, opponent.CurrentInsight.Action);
            Assert.AreEqual("You play Action 4 a lot.", CommentaryText.Sentence(Def, opponent.CurrentInsight));
        }

        [Test]
        public void BestResponseTiesGoToTheAuthoredCounterThenTheLowerIndex()
        {
            var uniform = new double[] { 1, 1, 1, 1, 1, 1 };
            for (int a = 0; a < 6; a++)
            {
                uniform[a] /= 6;
            }
            Assert.AreEqual(Def.AuthoredCounter(0), BestResponse.Of(Def, uniform), "all responses tie at 0");
            var onTwo = new double[] { 0, 0, 1, 0, 0, 0 };
            Assert.AreEqual(Def.AuthoredCounter(2), BestResponse.Of(Def, onTwo));
            Assert.AreEqual(3, BestResponse.ExpectedPayoff(Def, onTwo, Def.AuthoredCounter(2)), 1e-12);
        }

        [Test]
        public void ObserveBeforeDecideIsRejected()
        {
            Opponent opponent = TestConfigs.Make(TestConfigs.V1());
            opponent.StartMatch();
            Assert.Throws<System.InvalidOperationException>(() => opponent.Observe(0));
        }

        [Test]
        public void TheCommittedResponseDoesNotDependOnTheComingAction()
        {
            // Two sessions on the same seed see the same actions up to exchange 50, then diverge: every response up to
            // and including exchange 50 must match, because it is fixed before that exchange's action is revealed.
            Opponent a = TestConfigs.Make(TestConfigs.V1(), 3);
            Opponent b = TestConfigs.Make(TestConfigs.V1(), 3);
            a.StartMatch();
            b.StartMatch();
            for (int i = 0; i <= 50; i++)
            {
                Assert.AreEqual(a.Decide(), b.Decide(), "exchange " + i);
                a.Observe(i % 2);
                b.Observe(i < 50 ? i % 2 : 5);
            }
        }

        [Test]
        public void SameSeedSameResponses()
        {
            Opponent a = TestConfigs.Make(TestConfigs.V1(), 4);
            Opponent b = TestConfigs.Make(TestConfigs.V1(), 4);
            a.StartMatch();
            b.StartMatch();
            for (int i = 0; i < 300; i++)
            {
                Assert.AreEqual(a.Decide(), b.Decide());
                a.Observe(i % 3);
                b.Observe(i % 3);
            }
        }

        [Test]
        public void AbstainingExpertKeepsItsWeightExactlyWithoutTheShareUpdate()
        {
            var mixer = new FixedShareMixer(3, 6, 0.0, 0.0);
            var forecasts = new[] { new double[6], new double[6], new double[6] };
            for (int a = 0; a < 6; a++)
            {
                forecasts[0][a] = a == 0 ? 0.5 : 0.1;
                forecasts[1][a] = 1.0 / 6;
                forecasts[2][a] = 0.9;
            }
            var awake = new[] { true, true, false };
            var mixed = new double[6];
            mixer.Mix(awake, forecasts, mixed);
            mixer.Update(awake, forecasts, mixed, 0);
            Assert.AreEqual(1.0 / 3, mixer.Weight(2), 1e-15);
            Assert.AreEqual(2.0 / 3, mixer.Weight(0) + mixer.Weight(1), 1e-15);
            Assert.Greater(mixer.Weight(0), mixer.Weight(1));
        }

        [Test]
        public void ShareUpdateKeepsEveryWeightAboveAlphaOverK()
        {
            var mixer = new FixedShareMixer(2, 6, 0.01, 0.5);
            var forecasts = new[] { new double[6], new double[6] };
            for (int a = 0; a < 6; a++)
            {
                forecasts[0][a] = a == 0 ? 0.95 : 0.01;
                forecasts[1][a] = a == 0 ? 0.01 : 0.198;
            }
            var awake = new[] { true, true };
            var mixed = new double[6];
            for (int i = 0; i < 500; i++)
            {
                mixer.Mix(awake, forecasts, mixed);
                mixer.Update(awake, forecasts, mixed, 0);
            }
            Assert.GreaterOrEqual(mixer.Weight(1), 0.01 / 2 - 1e-15);
            Assert.AreEqual(1.0, mixer.Weight(0) + mixer.Weight(1), 1e-12);
        }
    }
}
