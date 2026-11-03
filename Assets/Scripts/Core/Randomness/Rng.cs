using System;

namespace AdaptiveOpponent.Core
{
    /// <summary>
    /// The seeded random generator every random choice in Core and the harness uses (PCG32, XSH-RR variant).
    /// Same seed and stream, same sequence, on every platform. Allocation-free after construction.
    /// </summary>
    public sealed class Rng
    {
        private const ulong Multiplier = 6364136223846793005UL;
        private ulong state;
        private readonly ulong increment;

        /// <summary>Creates a generator for <paramref name="seed"/> on stream <paramref name="stream"/>.</summary>
        public Rng(ulong seed, ulong stream)
        {
            increment = (stream << 1) | 1UL;
            state = 0;
            NextUInt();
            state += seed;
            NextUInt();
        }

        /// <summary>
        /// Derives an independent stream for one consumer (the Opponent, one archetype, ...) from a session seed,
        /// so no two consumers share draws.
        /// </summary>
        public static Rng ForStream(ulong sessionSeed, RngStream stream)
        {
            ulong id = (ulong)stream;
            return new Rng(SplitMix64(sessionSeed ^ (id * 0x9E3779B97F4A7C15UL)), SplitMix64(id + 0xD1B54A32D192ED03UL));
        }

        /// <summary>Derives a session seed from the global seed in configs/ and one run seed from a seeds file.</summary>
        public static ulong DeriveSeed(ulong globalSeed, ulong runSeed)
        {
            return SplitMix64(SplitMix64(globalSeed) ^ runSeed);
        }

        /// <summary>The next uniformly distributed 32-bit value.</summary>
        public uint NextUInt()
        {
            ulong old = state;
            state = unchecked(old * Multiplier + increment);
            uint xorShifted = (uint)(((old >> 18) ^ old) >> 27);
            int rot = (int)(old >> 59);
            return (xorShifted >> rot) | (xorShifted << ((-rot) & 31));
        }

        /// <summary>A uniform integer in [0, <paramref name="n"/>), without modulo bias.</summary>
        public int NextInt(int n)
        {
            if (n <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(n));
            }
            uint bound = (uint)n;
            uint threshold = unchecked(0u - bound) % bound;
            while (true)
            {
                uint value = NextUInt();
                if (value >= threshold)
                {
                    return (int)(value % bound);
                }
            }
        }

        /// <summary>A uniform double in [0, 1) with 53 random bits.</summary>
        public double NextDouble()
        {
            ulong high = NextUInt() >> 5;
            ulong low = NextUInt() >> 6;
            return ((high << 26) | low) * (1.0 / 9007199254740992.0);
        }

        /// <summary>Draws an index with probability proportional to <paramref name="weights"/> (non-negative, not all 0).</summary>
        public int NextWeighted(double[] weights, int count)
        {
            double total = 0;
            for (int i = 0; i < count; i++)
            {
                total += weights[i];
            }
            double target = NextDouble() * total;
            double running = 0;
            int last = 0;
            for (int i = 0; i < count; i++)
            {
                if (weights[i] <= 0)
                {
                    continue;
                }
                last = i;
                running += weights[i];
                if (target < running)
                {
                    return i;
                }
            }
            return last;
        }

        /// <summary>A standard normal draw (Box-Muller, no cached second value, so the stream stays simple).</summary>
        public double NextNormal()
        {
            double u1 = 1.0 - NextDouble();
            double u2 = NextDouble();
            return Math.Sqrt(-2.0 * Math.Log(u1)) * Math.Cos(2.0 * Math.PI * u2);
        }

        /// <summary>A Gamma(<paramref name="shape"/>, 1) draw (Marsaglia and Tsang, 2000).</summary>
        public double NextGamma(double shape)
        {
            if (shape <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(shape));
            }
            if (shape < 1.0)
            {
                double u = 1.0 - NextDouble();
                return NextGamma(shape + 1.0) * Math.Pow(u, 1.0 / shape);
            }
            double d = shape - 1.0 / 3.0;
            double c = 1.0 / Math.Sqrt(9.0 * d);
            while (true)
            {
                double x = NextNormal();
                double v = 1.0 + c * x;
                if (v <= 0)
                {
                    continue;
                }
                v = v * v * v;
                double u = 1.0 - NextDouble();
                if (Math.Log(u) < 0.5 * x * x + d - d * v + d * Math.Log(v))
                {
                    return d * v;
                }
            }
        }

        /// <summary>
        /// Fills <paramref name="result"/> with a Dirichlet(<paramref name="alpha"/>) draw over the first
        /// <paramref name="count"/> entries. Every alpha must be positive.
        /// </summary>
        public void NextDirichlet(double[] alpha, double[] result, int count)
        {
            double total = 0;
            for (int i = 0; i < count; i++)
            {
                result[i] = NextGamma(alpha[i]);
                total += result[i];
            }
            if (total <= 0)
            {
                for (int i = 0; i < count; i++)
                {
                    result[i] = 1.0 / count;
                }
                return;
            }
            for (int i = 0; i < count; i++)
            {
                result[i] /= total;
            }
        }

        private static ulong SplitMix64(ulong x)
        {
            unchecked
            {
                x += 0x9E3779B97F4A7C15UL;
                x = (x ^ (x >> 30)) * 0xBF58476D1CE4E5B9UL;
                x = (x ^ (x >> 27)) * 0x94D049BB133111EBUL;
                return x ^ (x >> 31);
            }
        }
    }

    /// <summary>The consumers that each get their own random stream from the session seed.</summary>
    public enum RngStream : ulong
    {
        /// <summary>The Opponent's own draws (Thompson Sampling, mixed strategies).</summary>
        Opponent = 1,
        /// <summary>The archetype or scripted player standing in for the human.</summary>
        Player = 2,
        /// <summary>Per-seed archetype set-up (preferred action, cycle, preference).</summary>
        PlayerSetup = 3,
    }
}
