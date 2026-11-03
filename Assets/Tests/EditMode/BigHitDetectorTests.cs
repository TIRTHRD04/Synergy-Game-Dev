using System.Collections.Generic;
using AdaptiveOpponent.Core;
using NUnit.Framework;

namespace AdaptiveOpponent.Tests
{
    public class BigHitDetectorTests
    {
        // Payoffs from the Opponent's side: negative = the Opponent took damage.
        private static List<BigHit> Run(params int[][] matches)
        {
            var history = new ExchangeHistory(64, 6, 6);
            var detector = new BigHitDetector(3, 7);
            var hits = new List<BigHit>();
            foreach (int[] match in matches)
            {
                history.StartMatch();
                foreach (int payoff in match)
                {
                    history.Add(history.Count % 6, (history.Count + 1) % 6, payoff);
                    if (detector.Observe(history, out BigHit hit))
                    {
                        hits.Add(hit);
                    }
                }
            }
            return hits;
        }

        [Test]
        public void ThreeHitsOnTheOpponentTotallingSevenIsABigHit()
        {
            List<BigHit> hits = Run(new[] { 0, -3, -3, -1 });
            Assert.AreEqual(1, hits.Count);
            Assert.AreEqual(BigHitSide.OnOpponent, hits[0].Side);
            Assert.AreEqual(1, hits[0].StartIndex);
            Assert.AreEqual(3, hits[0].EndIndex);
            Assert.AreEqual(7, hits[0].TotalDamage);
        }

        [Test]
        public void BelowThresholdIsNotABigHit()
        {
            Assert.AreEqual(0, Run(new[] { -1, -3, -1, -1, -1, -3 }).Count);
        }

        [Test]
        public void ADrawOrASideChangeBreaksTheWindow()
        {
            Assert.AreEqual(0, Run(new[] { -3, -3, 0, -3, 3, -3 }).Count);
        }

        [Test]
        public void HitsOnThePlayerAreTheirOwnSide()
        {
            List<BigHit> hits = Run(new[] { 3, 1, 3 });
            Assert.AreEqual(1, hits.Count);
            Assert.AreEqual(BigHitSide.OnPlayer, hits[0].Side);
        }

        [Test]
        public void WindowsDoNotOverlap()
        {
            Assert.AreEqual(1, Run(new[] { -3, -3, -3, -3, -3 }).Count);
            List<BigHit> two = Run(new[] { -3, -3, -3, -3, -3, -3 });
            Assert.AreEqual(2, two.Count);
            Assert.AreEqual(3, two[1].StartIndex);
        }

        [Test]
        public void ALateStartWindowIsFoundAfterAFailedOne()
        {
            List<BigHit> hits = Run(new[] { -1, -1, -3, -3 });
            Assert.AreEqual(1, hits.Count);
            Assert.AreEqual(1, hits[0].StartIndex);
        }

        [Test]
        public void WindowsNeverCrossAMatchBoundary()
        {
            Assert.AreEqual(0, Run(new[] { 0, -3, -3 }, new[] { -3, 0 }).Count);
        }

        [Test]
        public void ABigHitOnExchangeOneHasTheMatchStartContext()
        {
            List<BigHit> hits = Run(new[] { 0, 0 }, new[] { -3, -3, -3 });
            Assert.AreEqual(1, hits.Count);
            Assert.AreEqual(6, hits[0].StartPrevAction);
            Assert.AreEqual(6, hits[0].StartPrevResponse);
        }

        [Test]
        public void StartContextIsTheExchangeBeforeTheOpening()
        {
            List<BigHit> hits = Run(new[] { 0, 0, -3, -3, -1 });
            Assert.AreEqual(2, hits[0].StartIndex);
            Assert.AreEqual(1, hits[0].StartPrevAction);
            Assert.AreEqual(2, hits[0].StartPrevResponse);
        }

        [Test]
        public void MatchesTheOfflineLeftToRightScanOnRandomData()
        {
            var rng = new Rng(5, 5);
            int[] values = { 0, 0, 0, 0, 1, 3, -1, -3 };
            for (int trial = 0; trial < 200; trial++)
            {
                var matches = new int[3][];
                var expected = new List<int>();
                int offset = 0;
                for (int m = 0; m < 3; m++)
                {
                    matches[m] = new int[20];
                    for (int i = 0; i < 20; i++)
                    {
                        matches[m][i] = values[rng.NextInt(values.Length)];
                    }
                    foreach (int start in OfflineScan(matches[m]))
                    {
                        expected.Add(offset + start);
                    }
                    offset += 20;
                }
                List<BigHit> hits = Run(matches);
                Assert.AreEqual(expected.Count, hits.Count, "trial " + trial);
                for (int i = 0; i < hits.Count; i++)
                {
                    Assert.AreEqual(expected[i], hits[i].StartIndex, "trial " + trial);
                }
            }
        }

        private static List<int> OfflineScan(int[] payoffs)
        {
            var starts = new List<int>();
            int t = 0;
            while (t + 3 <= payoffs.Length)
            {
                int a = payoffs[t], b = payoffs[t + 1], c = payoffs[t + 2];
                bool sameSide = (a > 0 && b > 0 && c > 0) || (a < 0 && b < 0 && c < 0);
                if (sameSide && System.Math.Abs(a) + System.Math.Abs(b) + System.Math.Abs(c) >= 7)
                {
                    starts.Add(t);
                    t += 3;
                }
                else
                {
                    t++;
                }
            }
            return starts;
        }
    }
}
