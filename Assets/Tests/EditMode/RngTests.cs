using AdaptiveOpponent.Core;
using NUnit.Framework;

namespace AdaptiveOpponent.Tests
{
    public class RngTests
    {
        [Test]
        public void SameSeedAndStreamGiveTheSameSequence()
        {
            var a = Rng.ForStream(42, RngStream.Opponent);
            var b = Rng.ForStream(42, RngStream.Opponent);
            for (int i = 0; i < 1000; i++)
            {
                Assert.AreEqual(a.NextUInt(), b.NextUInt());
            }
        }

        [Test]
        public void DifferentStreamsDiffer()
        {
            var a = Rng.ForStream(42, RngStream.Opponent);
            var b = Rng.ForStream(42, RngStream.Player);
            int same = 0;
            for (int i = 0; i < 1000; i++)
            {
                if (a.NextUInt() == b.NextUInt())
                {
                    same++;
                }
            }
            Assert.Less(same, 3);
        }

        [Test]
        public void FirstValuesArePinned()
        {
            // Pins the generator so a change to it cannot silently change every logged run.
            var rng = new Rng(42, 54);
            Assert.AreEqual(0xa15c02b7u, rng.NextUInt());
            Assert.AreEqual(0x7b47f409u, rng.NextUInt());
            Assert.AreEqual(0xba1d3330u, rng.NextUInt());
        }

        [Test]
        public void NextIntIsRoughlyUniform()
        {
            var rng = new Rng(7, 1);
            var counts = new int[6];
            const int n = 60000;
            for (int i = 0; i < n; i++)
            {
                counts[rng.NextInt(6)]++;
            }
            double chi2 = 0;
            for (int i = 0; i < 6; i++)
            {
                double d = counts[i] - n / 6.0;
                chi2 += d * d / (n / 6.0);
            }
            Assert.Less(chi2, 20.5, "chi-square with 5 df, p < 0.001 bound");
        }

        [Test]
        public void NextDoubleStaysInUnitInterval()
        {
            var rng = new Rng(9, 3);
            for (int i = 0; i < 10000; i++)
            {
                double x = rng.NextDouble();
                Assert.GreaterOrEqual(x, 0.0);
                Assert.Less(x, 1.0);
            }
        }

        [TestCase(0.5)]
        [TestCase(1.0)]
        [TestCase(4.0)]
        public void GammaMeanMatchesShape(double shape)
        {
            var rng = new Rng(11, 5);
            double sum = 0;
            const int n = 40000;
            for (int i = 0; i < n; i++)
            {
                sum += rng.NextGamma(shape);
            }
            Assert.AreEqual(shape, sum / n, 4 * System.Math.Sqrt(shape / n));
        }

        [Test]
        public void DirichletDrawSumsToOne()
        {
            var rng = new Rng(13, 2);
            var alpha = new[] { 0.5, 0.5, 0.5, 3.5, 0.5, 0.5 };
            var result = new double[6];
            double meanHeavy = 0;
            for (int i = 0; i < 20000; i++)
            {
                rng.NextDirichlet(alpha, result, 6);
                double total = 0;
                foreach (double x in result)
                {
                    Assert.GreaterOrEqual(x, 0.0);
                    total += x;
                }
                Assert.AreEqual(1.0, total, 1e-9);
                meanHeavy += result[3];
            }
            Assert.AreEqual(3.5 / 6.0, meanHeavy / 20000, 0.01);
        }
    }
}
