using AdaptiveOpponent.Core;
using AdaptiveOpponent.Sim;
using NUnit.Framework;

namespace AdaptiveOpponent.Tests
{
    public class ArchetypeTests
    {
        private static readonly GameDefinition Def = TestData.Example();

        private static T Start<T>(T player, ulong seed = 1) where T : IPlayer
        {
            player.StartSession(Def, new Rng(seed, 1), new Rng(seed, 2));
            return player;
        }

        private static ExchangeHistory NewHistory()
        {
            var h = new ExchangeHistory(600, 6, 6);
            h.StartMatch();
            return h;
        }

        // Records an exchange and lets the player see it.
        private static void Play(ExchangeHistory h, IPlayer p, int action, int response)
        {
            h.Add(action, response, Def.Payoff(response, action));
            p.Observe(h);
        }

        [Test]
        public void SpammerPlaysItsFavouriteAboutNinetyPercent()
        {
            var p = Start(new Spammer());
            ExchangeHistory h = NewHistory();
            var counts = new int[6];
            for (int i = 0; i < 2000; i++)
            {
                counts[p.Act(h)]++;
            }
            int top = 0;
            for (int a = 1; a < 6; a++)
            {
                top = counts[a] > counts[top] ? a : top;
            }
            Assert.AreEqual(0.9 + 0.1 / 6, counts[top] / 2000.0, 0.03);
        }

        [Test]
        public void CyclerRepeatsThreeDistinctActions()
        {
            var p = Start(new Cycler());
            ExchangeHistory h = NewHistory();
            var seq = new int[300];
            for (int i = 0; i < seq.Length; i++)
            {
                seq[i] = p.Act(h);
            }
            int agree = 0;
            for (int i = 3; i < seq.Length; i++)
            {
                agree += seq[i] == seq[i - 3] ? 1 : 0;
            }
            Assert.Greater(agree / 297.0, 0.75);
        }

        [Test]
        public void WinStayLoseShiftRepeatsAfterAWinAndShiftsToABeaterOtherwise()
        {
            var p = Start(new WinStayLoseShift());
            ExchangeHistory h = NewHistory();
            Play(h, p, 0, 1);
            Assert.Less(Def.Payoff(1, 0), 0, "response 1 loses to action 0");
            Assert.AreEqual(0, p.Act(h));
            Play(h, p, 0, 5);
            Assert.Greater(Def.Payoff(5, 0), 0, "response 5 beats action 0");
            for (int i = 0; i < 50; i++)
            {
                int a = p.Act(h);
                Assert.IsTrue(a == Def.HeavyBeater(5) || a == Def.LightBeater(5));
            }
            Assert.AreEqual(1.0, p.OracleProbability(NewHistoryWithWin()), 1e-12);
        }

        private static ExchangeHistory NewHistoryWithWin()
        {
            ExchangeHistory h = NewHistory();
            h.Add(0, 1, Def.Payoff(1, 0));
            return h;
        }

        [Test]
        public void OutGuesserPlaysABeaterOfTheResponseItExpects()
        {
            var p = Start(new OutGuesser());
            ExchangeHistory h = NewHistory();
            for (int i = 0; i < 10; i++)
            {
                Play(h, p, 0, 2);
            }
            int beaters = 0;
            for (int i = 0; i < 1000; i++)
            {
                int a = p.Act(h);
                beaters += a == Def.HeavyBeater(2) || a == Def.LightBeater(2) ? 1 : 0;
            }
            Assert.AreEqual(0.9 + 0.1 * 2 / 6, beaters / 1000.0, 0.04);
            Assert.AreEqual(0.9 * 0.7 + 0.1 / 6, p.OracleProbability(h), 1e-12);
        }

        [Test]
        public void BigHitRepeaterReplaysItsSequenceAfterTheGap()
        {
            var p = Start(new BigHitRepeater());
            ExchangeHistory h = NewHistory();
            // Context: previous action 4 (a draw). Then three exchanges in which the Opponent takes 3 + 3 + 1 damage.
            Play(h, p, 4, 4);
            Assert.AreEqual(0, Def.Payoff(4, 4));
            int[] seq = { 2, 3, 0 };
            int[] responses = { 3, 4, 2 };
            for (int i = 0; i < 3; i++)
            {
                Play(h, p, seq[i], responses[i]);
            }
            Assert.AreEqual(-3, Def.Payoff(3, 2));
            Assert.AreEqual(-3, Def.Payoff(4, 3));
            Assert.AreEqual(-1, Def.Payoff(2, 0));
            for (int i = 0; i < BigHitRepeater.Gap; i++)
            {
                Play(h, p, 1, 1);
            }
            Play(h, p, 4, 4);
            for (int i = 0; i < 3; i++)
            {
                Assert.AreEqual(seq[i], p.Act(h), "replay position " + i);
                Assert.AreEqual(i == 0 ? PlayerTag.ReplayOpening : i == 1 ? PlayerTag.ReplayFollowUp1 : PlayerTag.ReplayFollowUp2, p.Tag);
                Play(h, p, seq[i], 4);
            }
            Play(h, p, 4, 4);
            p.Act(h);
            Assert.AreEqual(PlayerTag.None, p.Tag, "the gap starts again after a replay");
        }

        [Test]
        public void BurnedAvoiderAvoidsTheOpeningExactlyAndHalfAsMuchBroadly()
        {
            var p = Start(new BurnedAvoider(), 3);
            ExchangeHistory h = NewHistory();
            var counts = new int[6];
            for (int i = 0; i < 1000; i++)
            {
                counts[p.Act(h)]++;
            }
            int favourite = 0;
            for (int a = 1; a < 6; a++)
            {
                favourite = counts[a] > counts[favourite] ? a : favourite;
            }
            // Burn: context (5, 5), then the Opponent hits the player's favourite three times for 3 damage each.
            Play(h, p, 5, 5);
            int heavy = Def.AuthoredCounter(favourite);
            for (int i = 0; i < 3; i++)
            {
                Play(h, p, favourite, heavy);
            }
            Play(h, p, 5, 5);
            Assert.AreEqual(PlayerTag.None, p.Tag);
            int exact = CountOpening(p, h, favourite);
            Assert.AreEqual(PlayerTag.BurnedContext, p.Tag);
            Assert.Less(exact / 2000.0, 0.03);
            Play(h, p, 5, 0);
            int broad = CountOpening(p, h, favourite);
            Assert.AreEqual(0.667 * 0.5, broad / 2000.0, 0.05);
        }

        private static int CountOpening(IPlayer p, ExchangeHistory h, int opening)
        {
            int n = 0;
            for (int i = 0; i < 2000; i++)
            {
                n += p.Act(h) == opening ? 1 : 0;
            }
            return n;
        }

        [Test]
        public void EverySuiteNameCreatesThatArchetype()
        {
            foreach (string name in Archetypes.Suite)
            {
                Assert.AreEqual(name, Archetypes.Create(name).Name);
            }
            foreach (string name in Archetypes.Scripted)
            {
                Assert.AreEqual(name, Archetypes.Create(name).Name);
            }
        }
    }
}
