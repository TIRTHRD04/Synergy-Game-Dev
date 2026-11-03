using System.Collections;
using System.IO;
using AdaptiveOpponent.Game;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace AdaptiveOpponent.Tests
{
    /// <summary>The smoke check as a PlayMode test: the screen boots, a full session plays to the summary, the log is written.</summary>
    public class SmokeTests
    {
        [UnityTest]
        public IEnumerator FullSessionReachesTheSummaryAndWritesThePlayLog()
        {
            yield return null;
            DuelScreen screen = Object.FindFirstObjectByType<DuelScreen>();
            Assert.IsNotNull(screen, "Bootstrap creates the duel screen without any scene edits");
            screen.BeginSession("v2.json");
            Assert.IsNotNull(screen.Session, "configs/definition.json and configs/v2.json load");
            DuelSession session = screen.Session;
            int guard = 0;
            while (session.Phase != DuelPhase.Summary && guard < 1000)
            {
                guard++;
                session.SubmitAction(guard % 5 == 0 ? 3 : 1, 0);
                session.Continue();
                if (guard % 50 == 0)
                {
                    yield return null;
                }
            }
            Assert.AreEqual(DuelPhase.Summary, session.Phase, "three matches end within the exchange caps");
            Assert.IsNull(session.LogWarning);
            Assert.IsTrue(File.Exists(session.LogPath), "the play log was written at match end");
            StringAssert.Contains("# schema_version=1", File.ReadAllText(session.LogPath));
        }
    }
}
