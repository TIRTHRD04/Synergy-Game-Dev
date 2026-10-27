using System;

namespace AdaptiveOpponent.Sim
{
    /// <summary>The result of comparing two configs on the same seeds.</summary>
    public struct PairedComparison
    {
        /// <summary>Mean of (new - base) over seeds.</summary>
        public double MeanDifference;
        /// <summary>Standard error of the paired difference.</summary>
        public double StandardError;
        /// <summary>Two-sided 95% Student-t critical value for (seeds - 1) degrees of freedom.</summary>
        public double T;
        /// <summary>Number of paired seeds.</summary>
        public int Seeds;

        /// <summary>The non-inferiority margin, max(<paramref name="floor"/>, t x SE).</summary>
        public double Margin(double floor) => Math.Max(floor, T * StandardError);

        /// <summary>True when the new config is better by more than t x SE (a strict win).</summary>
        public bool StrictlyBetter(bool higherIsBetter)
        {
            double d = higherIsBetter ? MeanDifference : -MeanDifference;
            return d > T * StandardError;
        }

        /// <summary>True when the new config is worse by more than the margin.</summary>
        public bool WorseBeyondMargin(bool higherIsBetter, double floor)
        {
            double d = higherIsBetter ? MeanDifference : -MeanDifference;
            return d < -Margin(floor);
        }
    }

    /// <summary>Paired-seed statistics (notes/decisions.md: margin = max(floor, t x paired SE)).</summary>
    public static class PairedStats
    {
        private static readonly int[] Df = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20,
            21, 22, 23, 24, 25, 26, 27, 28, 29, 30, 40, 60, 120 };

        private static readonly double[] Critical = { 12.706, 4.303, 3.182, 2.776, 2.571, 2.447, 2.365, 2.306, 2.262,
            2.228, 2.201, 2.179, 2.160, 2.145, 2.131, 2.120, 2.110, 2.101, 2.093, 2.086, 2.080, 2.074, 2.069, 2.064,
            2.060, 2.056, 2.052, 2.048, 2.045, 2.042, 2.021, 2.000, 1.980 };

        /// <summary>
        /// The two-sided 95% Student-t critical value. Between table rows the next lower df is used, which is
        /// slightly conservative.
        /// </summary>
        public static double TCritical(int degreesOfFreedom)
        {
            if (degreesOfFreedom < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(degreesOfFreedom));
            }
            double t = Critical[0];
            for (int i = 0; i < Df.Length; i++)
            {
                if (Df[i] <= degreesOfFreedom)
                {
                    t = Critical[i];
                }
            }
            return t;
        }

        /// <summary>Compares per-seed values of a new config against a base config (same seeds, same order).</summary>
        public static PairedComparison Compare(double[] baseValues, double[] newValues)
        {
            if (baseValues.Length != newValues.Length || baseValues.Length < 2)
            {
                throw new ArgumentException("need at least two paired seeds of equal length");
            }
            int n = baseValues.Length;
            double mean = 0;
            for (int i = 0; i < n; i++)
            {
                mean += newValues[i] - baseValues[i];
            }
            mean /= n;
            double ss = 0;
            for (int i = 0; i < n; i++)
            {
                double d = newValues[i] - baseValues[i] - mean;
                ss += d * d;
            }
            return new PairedComparison
            {
                MeanDifference = mean,
                StandardError = Math.Sqrt(ss / (n - 1) / n),
                T = TCritical(n - 1),
                Seeds = n,
            };
        }
    }
}
