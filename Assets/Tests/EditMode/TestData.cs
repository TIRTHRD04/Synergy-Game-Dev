using System;
using System.IO;
using AdaptiveOpponent.Core;

namespace AdaptiveOpponent.Tests
{
    /// <summary>Shared fixtures: the example definition built in code, and a way to find repo files.</summary>
    public static class TestData
    {
        /// <summary>The circulant example payoff, f = [0, +3, +1, 0, -1, -3] (ADR 0010).</summary>
        public static readonly int[] Circulant = { 0, 3, 1, 0, -1, -3 };

        /// <summary>Payoff rows (response by action) of the circulant example.</summary>
        public static int[][] CirculantPayoff()
        {
            var rows = new int[6][];
            for (int r = 0; r < 6; r++)
            {
                rows[r] = new int[6];
                for (int a = 0; a < 6; a++)
                {
                    rows[r][a] = Circulant[((a - r) % 6 + 6) % 6];
                }
            }
            return rows;
        }

        /// <summary>Builds a definition from signed payoff rows; authored counters default to the heavy counter.</summary>
        public static GameDefinition FromPayoff(int[][] payoff, int[] authored = null)
        {
            var outcomes = new Outcome[6][];
            var damage = new int[6][];
            for (int r = 0; r < 6; r++)
            {
                outcomes[r] = new Outcome[6];
                damage[r] = new int[6];
                for (int a = 0; a < 6; a++)
                {
                    int v = payoff[r][a];
                    outcomes[r][a] = v > 0 ? Outcome.Win : v < 0 ? Outcome.Loss : Outcome.Draw;
                    damage[r][a] = Math.Abs(v);
                }
            }
            if (authored == null)
            {
                authored = new int[6];
                for (int a = 0; a < 6; a++)
                {
                    int best = -1;
                    for (int r = 0; r < 6; r++)
                    {
                        if (payoff[r][a] > 0 && (best < 0 || payoff[r][a] > payoff[best][a]))
                        {
                            best = r;
                        }
                    }
                    authored[a] = best < 0 ? 0 : best;
                }
            }
            return new GameDefinition(Names("Action"), Names("Response"), outcomes, damage, authored, 3, 7);
        }

        /// <summary>The example definition (same data as configs/definition.json).</summary>
        public static GameDefinition Example() => FromPayoff(CirculantPayoff());

        /// <summary>Reads a file by its path from the repository root, found by walking up from the working directory.</summary>
        public static string ReadRepoFile(string relativePath)
        {
            string dir = Directory.GetCurrentDirectory();
            while (dir != null)
            {
                string candidate = Path.Combine(dir, relativePath);
                if (File.Exists(candidate))
                {
                    return File.ReadAllText(candidate);
                }
                dir = Path.GetDirectoryName(dir);
            }
            throw new FileNotFoundException("not found above the working directory: " + relativePath);
        }

        private static string[] Names(string prefix)
        {
            var names = new string[6];
            for (int i = 0; i < 6; i++)
            {
                names[i] = prefix + " " + (i + 1);
            }
            return names;
        }
    }
}
