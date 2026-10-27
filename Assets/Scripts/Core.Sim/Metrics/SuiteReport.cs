using System.Globalization;
using System.Text;

namespace AdaptiveOpponent.Sim
{
    /// <summary>Formats a suite run per archetype and per phase, with learning curves (CLAUDE.md, "Harness").</summary>
    public static class SuiteReport
    {
        /// <summary>The human-readable report printed by the CLI.</summary>
        public static string Format(SuiteResult result, string configName, int seedCount)
        {
            var sb = new StringBuilder();
            sb.AppendLine("config " + configName + ", " + seedCount + " seeds, chance = " + F(1.0 / 6));
            sb.AppendLine("archetype         early   late    all     oracle  payoff  winrate bits/ex gate    insights recalls lambda>0");
            for (int a = 0; a < result.ArchetypeNames.Length; a++)
            {
                sb.Append(result.ArchetypeNames[a].PadRight(18));
                sb.Append(Col(result.Mean(a, s => s.EarlyAccuracy)));
                sb.Append(Col(result.Mean(a, s => s.LateAccuracy)));
                sb.Append(Col(result.Mean(a, s => s.Accuracy)));
                sb.Append(Col(result.Mean(a, s => s.Oracle)));
                sb.Append(Col(result.Mean(a, s => s.MeanPayoff)));
                sb.Append(Col(result.Mean(a, s => s.WinRate)));
                sb.Append(Col(result.Mean(a, s => s.BitsGainedPerExchange)));
                sb.Append((result.GateOpenedSeeds(a) + "/" + seedCount).PadRight(8));
                sb.Append(result.Total(a, s => s.Insights).ToString(CultureInfo.InvariantCulture).PadRight(9));
                sb.Append(result.Total(a, s => s.Recalls).ToString(CultureInfo.InvariantCulture).PadRight(8));
                sb.AppendLine(result.Total(a, s => s.LambdaNonZero).ToString(CultureInfo.InvariantCulture));
            }
            for (int a = 0; a < result.ArchetypeNames.Length; a++)
            {
                if (result.ArchetypeNames[a] == "BigHitRepeater")
                {
                    sb.Append("BigHitRepeater replay accuracy (pooled):");
                    string[] names = { " opening ", " follow-up 1 ", " follow-up 2 " };
                    for (int p = 0; p < SessionSummary.ReplayPositions; p++)
                    {
                        double acc = result.PooledReplayAccuracy(a, p, out int n);
                        sb.Append(names[p] + F(acc) + " (n=" + n + ")");
                    }
                    sb.AppendLine();
                }
                if (result.ArchetypeNames[a] == "BurnedAvoider")
                {
                    double loss = result.PooledBurnedLogLoss(a, out int n);
                    sb.AppendLine("BurnedAvoider log-loss in burned contexts (pooled): " + F(loss) + " nats (n=" + n + ")");
                }
            }
            sb.AppendLine("learning curve (accuracy per " + SuiteResult.CurveBucket + " exchanges):");
            for (int a = 0; a < result.ArchetypeNames.Length; a++)
            {
                sb.Append("  " + result.ArchetypeNames[a].PadRight(18));
                foreach (double v in result.LearningCurve(a))
                {
                    sb.Append(F(v) + " ");
                }
                sb.AppendLine();
            }
            sb.AppendLine("decision time (Decide + Observe): p50 " + F(result.DecisionMicroseconds(50)) + " us, p99 " +
                F(result.DecisionMicroseconds(99)) + " us; runtime " + F(result.RuntimeSeconds) + " s");
            return sb.ToString();
        }

        /// <summary>The compact per-archetype metrics written to the metrics column of experiments/log.csv.</summary>
        public static string MetricsField(SuiteResult result, int seedCount)
        {
            var sb = new StringBuilder();
            for (int a = 0; a < result.ArchetypeNames.Length; a++)
            {
                if (a > 0)
                {
                    sb.Append("; ");
                }
                sb.Append(result.ArchetypeNames[a]).Append(": early=").Append(F(result.Mean(a, s => s.EarlyAccuracy)))
                    .Append(" late=").Append(F(result.Mean(a, s => s.LateAccuracy)))
                    .Append(" payoff=").Append(F(result.Mean(a, s => s.MeanPayoff)))
                    .Append(" win=").Append(F(result.Mean(a, s => s.WinRate)))
                    .Append(" bits=").Append(F(result.Mean(a, s => s.BitsGainedPerExchange)))
                    .Append(" gate=").Append(result.GateOpenedSeeds(a)).Append('/').Append(seedCount)
                    .Append(" insights=").Append(result.Total(a, s => s.Insights))
                    .Append(" recalls=").Append(result.Total(a, s => s.Recalls));
            }
            sb.Append("; p50us=").Append(F(result.DecisionMicroseconds(50)))
                .Append(" p99us=").Append(F(result.DecisionMicroseconds(99)));
            return sb.ToString();
        }

        private static string Col(double value) => F(value).PadRight(8);

        /// <summary>Formats a number with three decimals, invariant culture; NaN prints as "-".</summary>
        public static string F(double value)
        {
            return double.IsNaN(value) ? "-" : value.ToString("0.000", CultureInfo.InvariantCulture);
        }
    }
}
