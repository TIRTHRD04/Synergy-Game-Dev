using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using AdaptiveOpponent.Core;
using AdaptiveOpponent.Sim;

namespace AdaptiveOpponent.SimCli
{
    /// <summary>Commands beyond a single suite run.</summary>
    internal static class Commands
    {
        private struct Metric
        {
            public string Name;
            public Func<SessionSummary, double> Value;
            public bool HigherIsBetter;
            public double Floor;
        }

        private static readonly Metric[] Metrics =
        {
            new Metric { Name = "late accuracy", Value = s => s.LateAccuracy, HigherIsBetter = true, Floor = 0.02 },
            new Metric { Name = "accuracy", Value = s => s.Accuracy, HigherIsBetter = true, Floor = 0.02 },
            new Metric { Name = "log-loss", Value = s => s.LogLoss, HigherIsBetter = false, Floor = 0.02 },
            new Metric { Name = "payoff", Value = s => s.MeanPayoff, HigherIsBetter = true, Floor = 0.06 },
            new Metric { Name = "win rate", Value = s => s.WinRate, HigherIsBetter = true, Floor = 0.02 },
            new Metric { Name = "replay accuracy", Value = s => s.ReplayAccuracy, HigherIsBetter = true, Floor = 0.02 },
            new Metric { Name = "burned log-loss", Value = s => s.BurnedLogLoss, HigherIsBetter = false, Floor = 0.02 },
        };

        /// <summary>
        /// Runs two configs on the same seeds and prints the paired difference per archetype and metric, with the
        /// verdict under the rules in notes/decisions.md (strict win: more than t x SE; worse: beyond the margin).
        /// </summary>
        public static int Compare(Dictionary<string, string> options)
        {
            var run = new RunSetup(options);
            string basePath = Program.Require(options, "base");
            string newPath = Program.Require(options, "new");
            Func<Rng, IOpponent> baseFactory = OpponentFactory.FromConfig(run.Definition, File.ReadAllText(basePath), out string baseName);
            Func<Rng, IOpponent> newFactory = OpponentFactory.FromConfig(run.Definition, File.ReadAllText(newPath), out string newName);
            SuiteResult baseResult = Suite.Run(run.Definition, baseFactory, run.Archetypes, run.Seeds, run.GlobalSeed, run.Mode, run.StartHp);
            SuiteResult newResult = Suite.Run(run.Definition, newFactory, run.Archetypes, run.Seeds, run.GlobalSeed, run.Mode, run.StartHp);
            Console.WriteLine("base:");
            Console.Write(SuiteReport.Format(baseResult, baseName, run.Seeds.Length));
            Console.WriteLine("new:");
            Console.Write(SuiteReport.Format(newResult, newName, run.Seeds.Length));
            Console.WriteLine("paired comparison (new - base), " + run.Seeds.Length + " seeds:");
            Console.WriteLine("archetype         metric          base    new     diff    SE      margin  verdict");
            var field = new StringBuilder();
            for (int a = 0; a < run.Archetypes.Length; a++)
            {
                foreach (Metric m in Metrics)
                {
                    if (!Paired(baseResult.PerSeed(a, m.Value), newResult.PerSeed(a, m.Value), out double[] b, out double[] n))
                    {
                        continue;
                    }
                    PairedComparison c = PairedStats.Compare(b, n);
                    string verdict = c.StrictlyBetter(m.HigherIsBetter) ? "BETTER"
                        : c.WorseBeyondMargin(m.HigherIsBetter, m.Floor) ? "WORSE" : "within margin";
                    Console.WriteLine(run.Archetypes[a].PadRight(18) + m.Name.PadRight(16) + Col(Mean(b)) + Col(Mean(n)) +
                        Col(c.MeanDifference) + Col(c.StandardError) + Col(c.Margin(m.Floor)) + verdict);
                    field.Append(run.Archetypes[a]).Append(' ').Append(m.Name).Append(" diff=")
                        .Append(SuiteReport.F(c.MeanDifference)).Append(" se=").Append(SuiteReport.F(c.StandardError))
                        .Append(' ').Append(verdict).Append("; ");
                }
            }
            run.Log(Program.Require(options, "name"), basePath + " vs " + newPath, field.ToString(),
                baseResult.RuntimeSeconds + newResult.RuntimeSeconds);
            return 0;
        }

        // Keeps the seeds where both runs have a value (a seed with no replays has none); needs two such seeds.
        private static bool Paired(double[] baseAll, double[] newAll, out double[] b, out double[] n)
        {
            var keepBase = new List<double>();
            var keepNew = new List<double>();
            for (int i = 0; i < baseAll.Length; i++)
            {
                if (!double.IsNaN(baseAll[i]) && !double.IsNaN(newAll[i]))
                {
                    keepBase.Add(baseAll[i]);
                    keepNew.Add(newAll[i]);
                }
            }
            b = keepBase.ToArray();
            n = keepNew.ToArray();
            return b.Length >= 2;
        }

        private static double Mean(double[] values)
        {
            double sum = 0;
            foreach (double v in values)
            {
                sum += v;
            }
            return sum / values.Length;
        }

        private static string Col(double value) => SuiteReport.F(value).PadRight(8);
    }
}
