using System.Collections.Generic;
using AdaptiveOpponent.Core;
using NUnit.Framework;

namespace AdaptiveOpponent.Tests
{
    /// <summary>Big-hit memory tests, pinned to the numbers printed by the design check script (ADR 0011).</summary>
    public class BigHitExpertTests
    {
        private static readonly GameDefinition Def = TestData.Example();
        private const double S = 3;

        // Exact firing p after 0..3 misses, and broad, from the design check output.
        private static readonly double[] ExactP = { 0.5833, 0.5098, 0.4407, 0.3795 };
        private static readonly double[] BroadP = { 0.4444, 0.3827, 0.3307, 0.2887 };

        private sealed class Setup
        {
            public ExchangeHistory History = new ExchangeHistory(400, 6, 6);
            public BigHitMemory Memory;
            public BigHitDetector Detector = new BigHitDetector(Def);
            public BigHitSequenceExpert Sequence;
            public AvoidanceExpert Avoidance;
            public double[] Forecast = new double[6];

            public Setup(bool broad)
            {
                Memory = new BigHitMemory(Def, broad, broad);
                Sequence = new BigHitSequenceExpert(Def, Memory.OnOpponent, S, broad);
                Avoidance = new AvoidanceExpert(Def, Memory.OnPlayer, 1.0, broad);
                History.StartMatch();
            }

            public void Add(int action, int response)
            {
                History.Add(action, response, Def.Payoff(response, action));
                if (Detector.Observe(History, out BigHit hit))
                {
                    Memory.Record(History, hit);
                }
                Sequence.Observe(History);
                Avoidance.Observe(History);
            }

            public bool PredictSequence() => Sequence.Predict(History, Forecast);
        }

        // Start context (4, 4) is a draw; then the player hits 3 + 3 + 1 with the sequence 2, 3, 0.
        private static Setup WithPlayerBigHit(bool broad)
        {
            var s = new Setup(broad);
            s.Add(4, 4);
            s.Add(2, 3);
            s.Add(3, 4);
            s.Add(0, 2);
            Assert.AreEqual(1, s.Memory.RecordedOnOpponent);
            return s;
        }

        [Test]
        public void ExactFiringTableMatchesTheDesignNumbers()
        {
            Setup s = WithPlayerBigHit(false);
            s.Add(4, 4);
            for (int misses = 0; misses < 4; misses++)
            {
                Assert.IsTrue(s.PredictSequence());
                Assert.AreEqual(ExactP[misses], s.Forecast[2], 1e-3, "after " + misses + " misses");
                s.Add(4, 4);
            }
        }

        [Test]
        public void BroadFiringTableMatchesTheDesignNumbers()
        {
            Setup s = WithPlayerBigHit(true);
            s.Add(4, 1);
            Assert.AreEqual(0, Def.Payoff(1, 4), "context exchange is a draw");
            for (int misses = 0; misses < 4; misses++)
            {
                Assert.IsTrue(s.PredictSequence());
                Assert.AreEqual(BroadP[misses], s.Forecast[2], 1e-3, "after " + misses + " misses");
                s.Add(4, 1);
            }
        }

        [Test]
        public void RecallCutoffFollowsFromTheInsightProbability()
        {
            // Recall needs p >= 0.4: an exact memory still qualifies after 2 misses, not after 3; broad only before any miss.
            Assert.GreaterOrEqual(ExactP[2], 0.4);
            Assert.Less(ExactP[3], 0.4);
            Assert.GreaterOrEqual(BroadP[0], 0.4);
            Assert.Less(BroadP[1], 0.4);
        }

        [Test]
        public void SixMissesKeepTheEntrySevenDeleteItAndAHitRestartsTheCount()
        {
            Setup s = WithPlayerBigHit(false);
            s.Add(4, 4);
            for (int i = 0; i < 6; i++)
            {
                Assert.IsTrue(s.PredictSequence(), "miss " + i);
                s.Add(4, 4);
            }
            Assert.IsTrue(s.Memory.OnOpponent.Live[0], "0.7^6 = 0.118 keeps it");
            Assert.IsTrue(s.PredictSequence());
            s.Add(2, 4);
            Assert.AreEqual(1.0, s.Memory.OnOpponent.Exact[0], "a hit resets the strength");
            Setup t = WithPlayerBigHit(false);
            t.Add(4, 4);
            for (int i = 0; i < 7; i++)
            {
                t.PredictSequence();
                t.Add(4, 4);
            }
            Assert.IsFalse(t.Memory.OnOpponent.Live[0], "0.7^7 = 0.082 deletes it");
            Assert.IsFalse(t.PredictSequence(), "nothing left to fire: abstain");
        }

        [Test]
        public void FollowUpsFireWhileThePlayerFollowsAndStopAtTheFirstMiss()
        {
            Setup s = WithPlayerBigHit(false);
            s.Add(4, 4);
            Assert.IsTrue(s.PredictSequence());
            Assert.AreEqual(2, BestResponse.TopAction(s.Forecast, 6), "Opening");
            s.Add(2, 0);
            Assert.IsTrue(s.PredictSequence());
            Assert.AreEqual(3, BestResponse.TopAction(s.Forecast, 6), "Follow-up 1");
            s.Add(5, 0);
            Assert.IsFalse(s.PredictSequence(), "a wrong Follow-up ends the chain");
        }

        [Test]
        public void ExactTakesPrecedenceOverBroad()
        {
            Setup s = WithPlayerBigHit(true);
            s.Add(4, 4);
            s.PredictSequence();
            Assert.AreEqual(MatchType.Exact, s.Memory.OnOpponent.FiredType[0]);
            s.Add(4, 4);
            Assert.AreEqual(0.7, s.Memory.OnOpponent.Exact[0], 1e-12);
            Assert.AreEqual(1.0, s.Memory.OnOpponent.Broad[0], "an exact visit changes m_exact only");
        }

        [Test]
        public void RepeatedBigHitMergesAndAFullPoolReplacesTheWeakest()
        {
            Setup s = WithPlayerBigHit(false);
            s.Add(4, 4);
            s.PredictSequence();
            s.Add(4, 4);
            s.Add(2, 3);
            s.Add(3, 4);
            s.Add(0, 2);
            Assert.AreEqual(1.0, s.Memory.OnOpponent.Exact[0], "same start context and sequence: strengths reset");
            int live = 0;
            foreach (bool b in s.Memory.OnOpponent.Live)
            {
                live += b ? 1 : 0;
            }
            Assert.AreEqual(1, live);
            var pool = new BigHitPool(1, true);
            var h = new ExchangeHistory(64, 6, 6);
            h.StartMatch();
            for (int i = 0; i < BigHitPool.Size + 1; i++)
            {
                h.Add(i % 6, 0, 0);
                pool.Add(h, new BigHit { StartIndex = i, EndIndex = i, StartPrevAction = i % 7, StartPrevResponse = i / 7 });
                if (i == 5)
                {
                    pool.SetStrength(5, MatchType.Exact, 0.2);
                    pool.SetStrength(5, MatchType.Broad, 0.2);
                }
            }
            Assert.AreEqual(BigHitPool.Size % 7, pool.PrevAction[5], "the weakest slot took the 17th big hit");
        }

        [Test]
        public void StrengthsCarryOverAMatchBoundaryAndChainsEnd()
        {
            Setup s = WithPlayerBigHit(false);
            s.Add(4, 4);
            s.PredictSequence();
            s.Add(2, 0);
            s.Sequence.StartMatch();
            s.History.StartMatch();
            Assert.AreEqual(0, s.Memory.OnOpponent.ChainPosition[0]);
            Assert.AreEqual(1.0, s.Memory.OnOpponent.Exact[0]);
        }

        [Test]
        public void ABigHitOnExchangeOneIsRememberedForTheNextMatchStart()
        {
            var s = new Setup(false);
            s.Add(2, 3);
            s.Add(3, 4);
            s.Add(0, 2);
            s.History.StartMatch();
            s.Sequence.StartMatch();
            Assert.IsTrue(s.PredictSequence(), "exchange 1 of the next match has the Match-start context");
            Assert.AreEqual(2, BestResponse.TopAction(s.Forecast, 6));
        }

        [Test]
        public void AvoidanceHasNoResetAndDecaysOnlyWhenTheOpeningIsPlayedAnyway()
        {
            var s = new Setup(false);
            s.Add(4, 4);
            int heavy = Def.AuthoredCounter(1);
            s.Add(1, heavy);
            s.Add(1, heavy);
            s.Add(1, heavy);
            Assert.AreEqual(1, s.Memory.RecordedOnPlayer);
            s.Add(4, 4);
            Assert.IsTrue(s.Avoidance.Predict(s.History, s.Forecast));
            Assert.Less(s.Forecast[1], s.Forecast[0]);
            s.Add(5, 4);
            Assert.AreEqual(1.0, s.Memory.OnPlayer.Exact[0], "avoiding leaves the strength unchanged");
            s.Add(4, 4);
            for (int i = 0; i < 7; i++)
            {
                Assert.IsTrue(s.Avoidance.Predict(s.History, s.Forecast), "play " + i);
                s.Add(1, 1);
                s.Add(4, 4);
            }
            Assert.IsFalse(s.Memory.OnPlayer.Live[0], "7 plays of the Opening at that spot delete the entry");
        }

        [Test]
        public void OpponentShowsARecallWhenTheStartContextComesBack()
        {
            var config = TestConfigs.V1(new Dictionary<string, string>
            {
                ["gateThreshold"] = "-1",
                ["experts"] = "[\"bigHitSequence\"]",
                ["bigHitStrength"] = "3",
                ["sequenceBroadMatches"] = "false",
            });
            Opponent opponent = TestConfigs.Make(config, 2);
            opponent.StartMatch();
            int r0 = opponent.Decide();
            opponent.Observe(r0);
            for (int i = 0; i < 3; i++)
            {
                int response = opponent.Decide();
                opponent.Observe(Def.HeavyBeater(response));
            }
            Assert.AreEqual(1, opponent.BigHits.RecordedOnOpponent);
            int opening = opponent.BigHits.OnOpponent.ActionAt(0, 0);
            bool recalled = false;
            for (int i = 0; i < 200 && !recalled; i++)
            {
                int response = opponent.Decide();
                if (opponent.Commentary == CommentaryKind.Recall)
                {
                    recalled = true;
                    Assert.AreEqual(InsightKind.RecallOpening, opponent.CurrentInsight.Kind);
                    Assert.AreEqual(opening, opponent.CurrentInsight.Action);
                    StringAssert.StartsWith("Last time we were here you hit me hard", CommentaryText.Sentence(Def, opponent.CurrentInsight));
                }
                opponent.Observe(r0);
            }
            Assert.IsTrue(recalled);
        }
    }
}
