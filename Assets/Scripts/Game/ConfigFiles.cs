using System;
using System.IO;
using UnityEngine;

namespace AdaptiveOpponent.Game
{
    /// <summary>
    /// Reads config files for the game. A build reads StreamingAssets/configs; the editor falls back to the repository's
    /// configs/ folder next to Assets/, so the harness and the game read the same files. Never throws: a missing or
    /// unreadable file comes back as null with a message to show on screen.
    /// </summary>
    public static class ConfigFiles
    {
        /// <summary>The Opponent configs offered at session start, in order.</summary>
        public static readonly string[] OpponentConfigs = { "v1.json", "v2.json", "v2-full-experimental.json" };

        /// <summary>Returns the text of configs/<paramref name="fileName"/>, or null and a reason.</summary>
        public static string Read(string fileName, out string error)
        {
            error = null;
            string[] folders =
            {
                Path.Combine(Application.streamingAssetsPath, "configs"),
                Path.Combine(Path.GetDirectoryName(Application.dataPath) ?? ".", "configs"),
            };
            foreach (string folder in folders)
            {
                string path = Path.Combine(folder, fileName);
                try
                {
                    if (File.Exists(path))
                    {
                        return File.ReadAllText(path);
                    }
                }
                catch (Exception ex) when (ex is IOException || ex is UnauthorizedAccessException)
                {
                    error = "Could not read " + path + ": " + ex.Message;
                    return null;
                }
            }
            error = "Config " + fileName + " not found in StreamingAssets/configs or configs/.";
            return null;
        }
    }
}
