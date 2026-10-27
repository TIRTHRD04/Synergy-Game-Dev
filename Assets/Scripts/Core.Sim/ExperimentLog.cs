using System.IO;
using System.Text;

namespace AdaptiveOpponent.Sim
{
    /// <summary>Appends one row per sim run to experiments/log.csv (CLAUDE.md, "Conventions").</summary>
    public static class ExperimentLog
    {
        /// <summary>The CSV header, written when the file is new.</summary>
        public const string Header = "name,git_hash,config,seeds,metrics,runtime_s,notes";

        /// <summary>Appends a row; fields are quoted so commas and quotes inside them are safe.</summary>
        public static void Append(string path, string name, string gitHash, string config, string seeds,
            string metrics, double runtimeSeconds, string notes)
        {
            bool isNew = !File.Exists(path) || new FileInfo(path).Length == 0;
            var sb = new StringBuilder();
            if (isNew)
            {
                sb.AppendLine(Header);
            }
            sb.Append(Quote(name)).Append(',').Append(Quote(gitHash)).Append(',').Append(Quote(config)).Append(',')
                .Append(Quote(seeds)).Append(',').Append(Quote(metrics)).Append(',')
                .Append(SuiteReport.F(runtimeSeconds)).Append(',').Append(Quote(notes ?? "")).AppendLine();
            File.AppendAllText(path, sb.ToString());
        }

        private static string Quote(string field) => "\"" + (field ?? "").Replace("\"", "\"\"") + "\"";
    }
}
