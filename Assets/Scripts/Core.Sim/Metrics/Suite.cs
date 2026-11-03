using System;
using System.Collections.Generic;
using System.Diagnostics;
using AdaptiveOpponent.Core;

namespace AdaptiveOpponent.Sim
{
    /// <summary>Runs one Opponent config against a list of archetypes on a list of seeds.</summary>
    public static class Suite
    {
        /// <summary>
        /// Plays every archetype on every seed. The session seed is derived from the global seed and the run seed,
        /// so two configs run on the same seeds face identical player draws (paired seeds).
        /// </summary>
        public static SuiteResult Run(GameDefinition definition, Func<Rng, IOpponent> makeOpponent, string[] archetypes,
            ulong[] seeds, ulong globalSeed, SessionMode mode, int startHp)
        {
            var result = new SuiteResult(archetypes, seeds.Length);
            var watch = Stopwatch.StartNew();
            for (int a = 0; a < archetypes.Length; a++)
            {
                for (int s = 0; s < seeds.Length; s++)
                {
                    ulong sessionSeed = Rng.DeriveSeed(globalSeed, seeds[s]);
                    SessionRecord record = SessionRunner.Run(definition, makeOpponent, Archetypes.Create(archetypes[a]),
                        sessionSeed, mode, startHp);
                    result.Add(a, new SessionSummary(record, definition.ActionCount), record);
                }
            }
            result.RuntimeSeconds = watch.Elapsed.TotalSeconds;
            return result;
        }
    }

    /// <summary>The per-archetype results of a suite run, with pooled and per-seed views.</summary>
    public sealed class SuiteResult
    {
        /// <summary>Exchanges per bucket of the learning curve.</summary>
        public const int CurveBucket = 25;

        private readonly List<SessionSummary>[] sessions;
        private readonly List<long> decisionTicks = new List<long>();

        /// <summary>Creates an empty result for the given archetypes.</summary>
        public SuiteResult(string[] archetypes, int seedCount)
        {
            ArchetypeNames = archetypes;
            sessions = new List<SessionSummary>[archetypes.Length];
            for (int i = 0; i < archetypes.Length; i++)
            {
                sessions[i] = new List<SessionSummary>(seedCount);
            }
        }

        /// <summary>The archetypes, in report order.</summary>
        public string[] ArchetypeNames { get; }

        /// <summary>Wall-clock seconds the run took.</summary>
        public double RuntimeSeconds { get; set; }

        /// <summary>Adds one session's summary and its decision times.</summary>
        public void Add(int archetype, SessionSummary summary, SessionRecord record)
        {
            sessions[archetype].Add(summary);
            foreach (ExchangeRecord e in record.Exchanges)
            {
                decisionTicks.Add(e.DecisionTicks);
            }
        }

        /// <summary>The sessions of one archetype, one per seed in seed order.</summary>
        public IReadOnlyList<SessionSummary> Sessions(int archetype) => sessions[archetype];

        /// <summary>One value per seed for an archetype.</summary>
        public double[] PerSeed(int archetype, Func<SessionSummary, double> metric)
        {
            var list = sessions[archetype];
            var values = new double[list.Count];
            for (int i = 0; i < values.Length; i++)
            {
                values[i] = metric(list[i]);
            }
            return values;
        }

        /// <summary>The mean over seeds of a metric for an archetype (NaN values are skipped).</summary>
        public double Mean(int archetype, Func<SessionSummary, double> metric)
        {
            double sum = 0;
            int n = 0;
            foreach (SessionSummary s in sessions[archetype])
            {
                double v = metric(s);
                if (!double.IsNaN(v))
                {
                    sum += v;
                    n++;
                }
            }
            return n > 0 ? sum / n : double.NaN;
        }

        /// <summary>Seeds on which the evidence gate opened at least once.</summary>
        public int GateOpenedSeeds(int archetype)
        {
            int n = 0;
            foreach (SessionSummary s in sessions[archetype])
            {
                n += s.GateOpenIndex >= 0 ? 1 : 0;
            }
            return n;
        }

        /// <summary>A summed count over seeds.</summary>
        public int Total(int archetype, Func<SessionSummary, int> count)
        {
            int n = 0;
            foreach (SessionSummary s in sessions[archetype])
            {
                n += count(s);
            }
            return n;
        }

        /// <summary>Pooled replay accuracy at a position and the number of replay exchanges behind it.</summary>
        public double PooledReplayAccuracy(int archetype, int position, out int count)
        {
            int hits = 0;
            count = 0;
            foreach (SessionSummary s in sessions[archetype])
            {
                hits += s.ReplayHits[position];
                count += s.ReplayCounts[position];
            }
            return count > 0 ? (double)hits / count : double.NaN;
        }

        /// <summary>Pooled mean log-loss in burned contexts and the number of exchanges behind it.</summary>
        public double PooledBurnedLogLoss(int archetype, out int count)
        {
            double sum = 0;
            count = 0;
            foreach (SessionSummary s in sessions[archetype])
            {
                sum += s.BurnedLogLossSum;
                count += s.BurnedCount;
            }
            return count > 0 ? sum / count : double.NaN;
        }

        /// <summary>Learning curve: mean prediction accuracy per bucket of <see cref="CurveBucket"/> exchanges.</summary>
        public double[] LearningCurve(int archetype)
        {
            int length = 0;
            foreach (SessionSummary s in sessions[archetype])
            {
                length = Math.Max(length, s.Exchanges);
            }
            int buckets = (length + CurveBucket - 1) / CurveBucket;
            var hits = new double[buckets];
            var counts = new double[buckets];
            foreach (SessionSummary s in sessions[archetype])
            {
                for (int i = 0; i < s.Exchanges; i++)
                {
                    hits[i / CurveBucket] += s.Correct[i] ? 1 : 0;
                    counts[i / CurveBucket]++;
                }
            }
            for (int b = 0; b < buckets; b++)
            {
                hits[b] = counts[b] > 0 ? hits[b] / counts[b] : double.NaN;
            }
            return hits;
        }

        /// <summary>A percentile (0-100) of Decide + Observe time over every exchange, in microseconds.</summary>
        public double DecisionMicroseconds(double percentile)
        {
            if (decisionTicks.Count == 0)
            {
                return double.NaN;
            }
            var sorted = decisionTicks.ToArray();
            Array.Sort(sorted);
            int index = (int)Math.Min(sorted.Length - 1, Math.Floor(percentile / 100.0 * sorted.Length));
            return sorted[index] * 1e6 / Stopwatch.Frequency;
        }
    }
}
