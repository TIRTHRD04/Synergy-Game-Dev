using System;
using AdaptiveOpponent.Core;
using AdaptiveOpponent.UI;
using UnityEngine;

namespace AdaptiveOpponent.Game
{
    /// <summary>
    /// The bare-bones duel screen (IMGUI, built from script). It only draws and routes input: keys 1-6 or the six
    /// buttons pick an action; a key counts once per press (the next one needs a key-up first); after each reveal input
    /// is locked for <see cref="RevealLockoutSeconds"/>. All match flow is in <see cref="DuelSession"/>, all learning in
    /// the Opponent.
    /// </summary>
    public sealed class DuelScreen : MonoBehaviour
    {
        /// <summary>How long the reveal stays up before the next exchange (seconds).</summary>
        public float RevealLockoutSeconds = 0.4f;

        /// <summary>HP each side starts a match with (tuned on dev seeds; notes/decisions.md).</summary>
        public int StartHp = 60;

        private GameDefinition definition;
        private string error;
        private string configFile;
        private float revealStartedAt;
        private float committedAt;
        private KeyCode heldKey = KeyCode.None;
        private string[] actionLabels;
        private bool warningLogged;
        private string headerText, playerHpText, opponentHpText, promptText, summaryTitle, logPathText;

        /// <summary>The running session, or null on the config-picker screen.</summary>
        public DuelSession Session { get; private set; }

        /// <summary>Starts a session with configs/<paramref name="opponentConfigFile"/>; on failure shows the reason.</summary>
        public void BeginSession(string opponentConfigFile)
        {
            error = null;
            try
            {
                string definitionText = ConfigFiles.Read("definition.json", out string readError);
                string configText = definitionText == null ? null : ConfigFiles.Read(opponentConfigFile, out readError);
                if (configText == null)
                {
                    error = readError;
                    return;
                }
                definition = DefinitionParser.Parse(definitionText);
                OpponentConfig config = OpponentConfig.Parse(configText);
                configFile = opponentConfigFile;
                Session = new DuelSession(definition, config, configFile, Commitment.NewSessionSeed(), StartHp);
                actionLabels = new string[definition.ActionCount];
                for (int a = 0; a < actionLabels.Length; a++)
                {
                    actionLabels[a] = (a + 1) + ": " + definition.ActionName(a);
                }
                committedAt = Time.realtimeSinceStartup;
                warningLogged = false;
                RefreshTexts();
            }
            catch (Exception ex) when (ex is JsonException || ex is DefinitionException || ex is ArgumentException)
            {
                error = "Config problem: " + ex.Message;
                Session = null;
            }
        }

        private void Update()
        {
            if (Session != null && Session.Phase == DuelPhase.Revealing
                && Time.realtimeSinceStartup - revealStartedAt >= RevealLockoutSeconds)
            {
                Session.Continue();
                committedAt = Time.realtimeSinceStartup;
                RefreshTexts();
            }
        }

        private void OnApplicationQuit() => Session?.FlushLog();

        private void OnGUI()
        {
            if (Session == null)
            {
                DrawConfigPicker();
                return;
            }
            HandleKeys(Event.current);
            if (Session.Phase == DuelPhase.Summary)
            {
                DrawSummary();
                return;
            }
            DrawDuel();
        }

        private void DrawConfigPicker()
        {
            GUI.Label(new Rect(20, 20, 600, Panels.Line), "Adaptive Opponent - pick an Opponent config:");
            for (int i = 0; i < ConfigFiles.OpponentConfigs.Length; i++)
            {
                if (GUI.Button(new Rect(20, 50 + i * 34, 320, 28), ConfigFiles.OpponentConfigs[i]))
                {
                    BeginSession(ConfigFiles.OpponentConfigs[i]);
                }
            }
            if (error != null)
            {
                GUI.Label(new Rect(20, 170, 900, Panels.Line * 3), error);
            }
        }

        private void DrawDuel()
        {
            DuelSession s = Session;
            GUI.Label(new Rect(20, 10, 900, Panels.Line), headerText);
            Panels.HpBar(new Rect(20, 40, 400, 44), playerHpText, s.PlayerHp, s.StartHp);
            Panels.HpBar(new Rect(460, 40, 400, 44), opponentHpText, s.OpponentHp, s.StartHp);
            bool choosing = s.Phase == DuelPhase.Choosing;
            GUI.Label(new Rect(20, 100, 900, Panels.Line), promptText);
            GUI.enabled = choosing;
            for (int a = 0; a < actionLabels.Length; a++)
            {
                if (Panels.ActionButton(new Rect(20 + a * 140, 130, 130, 36), actionLabels[a]))
                {
                    Submit(a);
                }
            }
            GUI.enabled = true;
            Panels.Reveal(new Rect(20, 180, 600, Panels.Line * 4), s.Reveal);
            Panels.Belief(new Rect(640, 180, 300, Panels.Line * 6), s.Belief);
            GUI.Label(new Rect(20, 290, 920, Panels.Line * 2), s.Belief.Commentary);
            if (s.LogWarning != null)
            {
                GUI.Label(new Rect(20, 560, 920, Panels.Line * 2), s.LogWarning);
            }
        }

        private void DrawSummary()
        {
            SummaryView summary = Session.Summary;
            GUI.Label(new Rect(20, 20, 900, Panels.Line), summaryTitle);
            Panels.Lines(new Rect(20, 50, 920, Panels.Line * summary.Lines.Length), summary.Lines);
            GUI.Label(new Rect(20, 60 + Panels.Line * summary.Lines.Length, 920, Panels.Line * 3), summary.Curve);
            GUI.Label(new Rect(20, 140 + Panels.Line * summary.Lines.Length, 920, Panels.Line), logPathText);
            if (GUI.Button(new Rect(20, 180 + Panels.Line * summary.Lines.Length, 200, 30), "New session"))
            {
                Session = null;
            }
        }

        private void HandleKeys(Event e)
        {
            if (e == null || (e.type != EventType.KeyDown && e.type != EventType.KeyUp))
            {
                return;
            }
            int action = ActionForKey(e.keyCode);
            if (action < 0)
            {
                return;
            }
            if (e.type == EventType.KeyUp)
            {
                if (e.keyCode == heldKey)
                {
                    heldKey = KeyCode.None;
                }
                return;
            }
            if (heldKey == KeyCode.None)
            {
                heldKey = e.keyCode;
                Submit(action);
            }
            e.Use();
        }

        private void Submit(int action)
        {
            if (Session.Phase != DuelPhase.Choosing)
            {
                return;
            }
            long reactionMs = (long)((Time.realtimeSinceStartup - committedAt) * 1000f);
            Session.SubmitAction(action, reactionMs);
            revealStartedAt = Time.realtimeSinceStartup;
            RefreshTexts();
        }

        // Rebuilds the on-screen strings when the session changes, so OnGUI allocates nothing per frame.
        private void RefreshTexts()
        {
            DuelSession s = Session;
            if (s == null)
            {
                return;
            }
            headerText = "Opponent: " + s.Label + (configFile.Contains("experimental") ? "  [EXPERIMENTAL CONFIG]" : "") +
                "   Match " + (s.Match + 1) + "/" + DuelSession.Matches + "   Exchange " + s.ExchangeNumber;
            playerHpText = "You: " + s.PlayerHp + " HP";
            opponentHpText = "Opponent: " + s.OpponentHp + " HP";
            promptText = s.Phase == DuelPhase.Choosing
                ? "I have locked in my response. Commitment: " + s.CommitmentShort + "...  Pick an action (keys 1-6)."
                : "Revealed.";
            summaryTitle = "Session over (" + s.Label + ", seed " + s.Seed + ")";
            logPathText = "Play log: " + s.LogPath;
            if (s.LogWarning != null && !warningLogged)
            {
                warningLogged = true;
                Debug.LogWarning(s.LogWarning);
            }
        }

        private int ActionForKey(KeyCode key)
        {
            if (key >= KeyCode.Alpha1 && key <= KeyCode.Alpha6)
            {
                return key - KeyCode.Alpha1;
            }
            if (key >= KeyCode.Keypad1 && key <= KeyCode.Keypad6)
            {
                return key - KeyCode.Keypad1;
            }
            return -1;
        }
    }
}
