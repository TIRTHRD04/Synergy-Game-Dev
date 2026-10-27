using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using AdaptiveOpponent.Core;
using AdaptiveOpponent.Sim;

namespace AdaptiveOpponent.SimCli
{
    /// <summary>Command-line entry point for the headless simulator (docs/COMMANDS.md).</summary>
    public static class Program
    {
        private const string Usage =
            "usage: sim suite --config <file> --seeds <file> --name <row> [--mode harness|game] [--hp <n>]\n" +
            "                 [--archetypes a,b,...] [--notes <text>] [--no-log]\n" +
            "       sim compare --base <file> --new <file> --seeds <file> --name <row> [--notes <text>] [--no-log]";

        /// <summary>Runs one simulator command and returns the process exit code.</summary>
        public static int Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.Error.WriteLine(Usage);
                return 2;
            }
            try
            {
                Dictionary<string, string> options = ParseOptions(args);
                switch (args[0])
                {
                    case "suite": return RunSuite(options);
                    case "compare": return Commands.Compare(options);
                    default:
                        Console.Error.WriteLine(Usage);
                        return 2;
                }
            }
            catch (Exception ex) when (ex is ArgumentException || ex is IOException || ex is FormatException
                || ex is DefinitionException)
            {
                Console.Error.WriteLine("error: " + ex.Message);
                return 1;
            }
        }

        private static int RunSuite(Dictionary<string, string> options)
        {
            var run = new RunSetup(options);
            string configPath = Require(options, "config");
            Func<Rng, IOpponent> factory = OpponentFactory.FromConfig(run.Definition, File.ReadAllText(configPath), out string configName);
            SuiteResult result = Suite.Run(run.Definition, factory, run.Archetypes, run.Seeds, run.GlobalSeed, run.Mode, run.StartHp);
            Console.Write(SuiteReport.Format(result, configName, run.Seeds.Length));
            run.Log(Require(options, "name"), configPath, SuiteReport.MetricsField(result, run.Seeds.Length), result.RuntimeSeconds);
            return 0;
        }

        internal static Dictionary<string, string> ParseOptions(string[] args)
        {
            var options = new Dictionary<string, string>();
            for (int i = 1; i < args.Length; i++)
            {
                if (!args[i].StartsWith("--", StringComparison.Ordinal))
                {
                    throw new ArgumentException("unexpected argument " + args[i]);
                }
                string key = args[i].Substring(2);
                bool isFlag = i + 1 >= args.Length || args[i + 1].StartsWith("--", StringComparison.Ordinal);
                options[key] = isFlag ? "true" : args[++i];
            }
            return options;
        }

        internal static string Require(Dictionary<string, string> options, string key)
        {
            if (!options.TryGetValue(key, out string value))
            {
                throw new ArgumentException("missing --" + key);
            }
            return value;
        }

        internal static string GitHash()
        {
            string hash = Git("rev-parse --short HEAD").Trim();
            string dirty = Git("status --porcelain --untracked-files=no").Trim();
            return dirty.Length > 0 ? hash + "-dirty" : hash;
        }

        private static string Git(string arguments)
        {
            var info = new ProcessStartInfo("git", arguments) { RedirectStandardOutput = true, UseShellExecute = false };
            using (Process process = Process.Start(info))
            {
                string output = process.StandardOutput.ReadToEnd();
                process.WaitForExit();
                return output;
            }
        }
    }

    /// <summary>The inputs every run shares: definition, seeds, global seed, mode and log destination.</summary>
    internal sealed class RunSetup
    {
        private readonly Dictionary<string, string> options;

        public RunSetup(Dictionary<string, string> options)
        {
            this.options = options;
            Definition = DefinitionParser.Parse(File.ReadAllText("configs/definition.json"));
            GlobalSeed = OpponentFactory.GlobalSeed(File.ReadAllText("configs/seed.json"));
            SeedsPath = Program.Require(options, "seeds");
            Seeds = ReadSeeds(SeedsPath);
            Mode = options.TryGetValue("mode", out string mode) && mode == "game" ? SessionMode.GameRules : SessionMode.Harness;
            StartHp = options.TryGetValue("hp", out string hp) ? int.Parse(hp, CultureInfo.InvariantCulture) : 60;
            Archetypes = options.TryGetValue("archetypes", out string list) ? list.Split(',') : Sim.Archetypes.Suite;
        }

        public GameDefinition Definition { get; }
        public ulong GlobalSeed { get; }
        public string SeedsPath { get; }
        public ulong[] Seeds { get; }
        public SessionMode Mode { get; }
        public int StartHp { get; }
        public string[] Archetypes { get; }

        public void Log(string name, string config, string metrics, double runtimeSeconds)
        {
            if (options.ContainsKey("no-log"))
            {
                return;
            }
            options.TryGetValue("notes", out string notes);
            string modeNote = "mode=" + (Mode == SessionMode.Harness ? "harness" : "game hp=" + StartHp);
            ExperimentLog.Append("experiments/log.csv", name, Program.GitHash(), config, SeedsPath, metrics, runtimeSeconds,
                string.IsNullOrEmpty(notes) ? modeNote : modeNote + "; " + notes);
            Console.WriteLine("logged row \"" + name + "\" to experiments/log.csv");
        }

        private static ulong[] ReadSeeds(string path)
        {
            var seeds = new List<ulong>();
            foreach (string line in File.ReadAllLines(path))
            {
                string trimmed = line.Trim();
                if (trimmed.Length > 0 && !trimmed.StartsWith("#", StringComparison.Ordinal))
                {
                    seeds.Add(ulong.Parse(trimmed, CultureInfo.InvariantCulture));
                }
            }
            return seeds.ToArray();
        }
    }
}
