using System;
using System.Globalization;
using System.IO;
using System.Text;
using UnityEngine;

namespace AdaptiveOpponent.Game
{
    /// <summary>
    /// The play log (notes/decisions.md, "Unity screen"): one timestamped CSV per session in
    /// Application.persistentDataPath/play-logs. Rows are buffered and written at match end and on quit. All file I/O
    /// is caught; a failure sets <see cref="Warning"/> once and play continues. The Opponent never reads these files.
    /// </summary>
    public sealed class PlayLog
    {
        /// <summary>The play-log schema version.</summary>
        public const int SchemaVersion = 1;

        /// <summary>The column header row.</summary>
        public const string Columns = "session,match,exchange,action,response,outcome,damage,big_hit,predicted_top," +
            "p_action,leading_expert,gate_open,lambda,reaction_ms,commitment_hash,nonce";

        private readonly StringBuilder buffer = new StringBuilder();
        private readonly string path;
        private bool headerWritten;

        /// <summary>Starts a log for one session; nothing is written until the first flush.</summary>
        public PlayLog(string configFile, ulong seed, string aiVersion)
        {
            string stamp = DateTime.UtcNow.ToString("yyyyMMdd-HHmmss", CultureInfo.InvariantCulture);
            SessionId = stamp;
            path = Path.Combine(Application.persistentDataPath, "play-logs", "session-" + stamp + ".csv");
            buffer.Append("# schema_version=").Append(SchemaVersion).Append('\n')
                .Append("# config=").Append(configFile).Append('\n')
                .Append("# seed=").Append(seed.ToString(CultureInfo.InvariantCulture)).Append('\n')
                .Append("# ai_version=").Append(aiVersion).Append('\n')
                .Append(Columns).Append('\n');
        }

        /// <summary>The session id used in every row (the UTC start time).</summary>
        public string SessionId { get; }

        /// <summary>Where the log is written.</summary>
        public string FilePath => path;

        /// <summary>Set once when a write fails; shown on screen.</summary>
        public string Warning { get; private set; }

        /// <summary>Buffers one exchange row.</summary>
        public void Add(int match, int exchange, int action, int response, string outcome, int damage, bool bigHit,
            int predictedTop, double pAction, string leadingExpert, bool gateOpen, double lambda, long reactionMs,
            string hash, string nonce)
        {
            CultureInfo c = CultureInfo.InvariantCulture;
            buffer.Append(SessionId).Append(',').Append(match).Append(',').Append(exchange).Append(',')
                .Append(action).Append(',').Append(response).Append(',').Append(outcome).Append(',')
                .Append(damage).Append(',').Append(bigHit ? 1 : 0).Append(',').Append(predictedTop).Append(',')
                .Append(pAction.ToString("0.######", c)).Append(',').Append(leadingExpert).Append(',')
                .Append(gateOpen ? 1 : 0).Append(',').Append(lambda.ToString("0.####", c)).Append(',')
                .Append(reactionMs).Append(',').Append(hash).Append(',').Append(nonce).Append('\n');
        }

        /// <summary>Appends the buffered rows to the file. Never throws.</summary>
        public void Flush()
        {
            if (buffer.Length == 0 || Warning != null)
            {
                return;
            }
            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(path));
                File.AppendAllText(path, buffer.ToString());
                buffer.Clear();
                headerWritten = true;
            }
            catch (Exception ex) when (ex is IOException || ex is UnauthorizedAccessException || ex is NotSupportedException)
            {
                Warning = "Play log could not be written (" + ex.Message + "). Play continues; this session is not logged.";
            }
        }

        /// <summary>True once the header and at least one flush reached the file.</summary>
        public bool Written => headerWritten;
    }
}
