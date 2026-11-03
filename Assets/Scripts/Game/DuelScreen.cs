using System;
using AdaptiveOpponent.Core;
using AdaptiveOpponent.UI;
using UnityEngine;

namespace AdaptiveOpponent.Game
{
    /// <summary>
    /// The duel screen (IMGUI, built from script, laid out at 1280 x 720 and scaled to the window). It only draws and
    /// routes input: keys 1-6 or the six buttons pick a move; a key counts once per press (the next one needs a
    /// key-up first); after each reveal input is locked for <see cref="RevealLockoutSeconds"/>; H toggles the
    /// how-to-play overlay. All match flow is in <see cref="DuelSession"/>, all learning in the Opponent.
    /// </summary>
    public sealed class DuelScreen : MonoBehaviour
    {
        /// <summary>How long the reveal locks input before the next exchange (seconds).</summary>
        public float RevealLockoutSeconds = 0.4f;

        /// <summary>HP each side starts a match with (tuned on dev seeds; notes/decisions.md).</summary>
        public int StartHp = 60;

        private const float FlashSeconds = 0.6f;
        private static readonly string[] ConfigTitles =
        {
            "Baseline AI (v1)",
            "Tuned AI (v2) - recommended",
            "Experimental AI (every component)",
        };
        private static readonly string[] ConfigNotes =
        {
            "Counts your habits and mixes the counts. The reference version.",
            "Also spots repeated sequences and big hits you landed. The version the dev tests chose.",
            "Includes components that lost their dev tests. Labelled EXPERIMENTAL on screen.",
        };
        private const string Pitch = "A 1v1 duel of six fighting moves against an AI that learns your habits while you play.";
        private const string Waiting = "Pick your first move below.\nThe AI has already locked in its move.";

        private string error;
        private string configFile;
        private float revealStartedAt;
        private float committedAt;
        private float playerHitAt = -10f;
        private float aiHitAt = -10f;
        private bool showHelp;
        private KeyCode heldKey = KeyCode.None;
        private bool warningLogged;
        private string headerText, rightText, playerHpText, aiHpText, statusText, summaryTitle, logPathText, matchRules;

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
                GameDefinition definition = DefinitionParser.Parse(definitionText);
                OpponentConfig config = OpponentConfig.Parse(configText);
                configFile = opponentConfigFile;
                Session = new DuelSession(definition, config, configFile, Commitment.NewSessionSeed(), StartHp);
                matchRules = "Each match starts at " + StartHp + " HP and ends at 0 HP or after " + DuelSession.ExchangeCap +
                    " exchanges. A session is " + DuelSession.Matches + " matches.";
                committedAt = Time.realtimeSinceStartup;
                playerHitAt = aiHitAt = -10f;
                warningLogged = false;
                showHelp = true;
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
            Theme.Ensure();
            Widgets.Fill(new Rect(0, 0, Screen.width, Screen.height), Theme.Background);
            float scale = Mathf.Min(Screen.width / Theme.Width, Screen.height / Theme.Height);
            var offset = new Vector3((Screen.width - Theme.Width * scale) / 2, (Screen.height - Theme.Height * scale) / 2, 0);
            Matrix4x4 previous = GUI.matrix;
            GUI.matrix = Matrix4x4.TRS(offset, Quaternion.identity, new Vector3(scale, scale, 1));
            if (Session == null)
            {
                DrawConfigPicker();
            }
            else
            {
                HandleKeys(Event.current);
                if (Session.Phase == DuelPhase.Summary)
                {
                    DrawSummary();
                }
                else
                {
                    DrawDuel();
                }
            }
            GUI.matrix = previous;
        }

        private void DrawConfigPicker()
        {
            Widgets.Text(new Rect(80, 60, 1120, 50), "ADAPTIVE DUEL", Theme.Title, Theme.Accent);
            Widgets.Text(new Rect(80, 110, 1120, 24), Pitch, Theme.Body, Theme.Text);
            Widgets.Text(new Rect(80, 160, 1120, 20), "CHOOSE YOUR OPPONENT", Theme.Label, Theme.Muted);
            for (int i = 0; i < ConfigFiles.OpponentConfigs.Length; i++)
            {
                var row = new Rect(80, 188 + i * 92, 700, 80);
                if (Widgets.Button(new Rect(row.x, row.y, 320, row.height), ConfigTitles[i], Theme.Raised))
                {
                    BeginSession(ConfigFiles.OpponentConfigs[i]);
                }
                Widgets.Text(new Rect(row.x + 340, row.y + 10, 440, 22), ConfigFiles.OpponentConfigs[i], Theme.Label, Theme.Muted);
                Widgets.Text(new Rect(row.x + 340, row.y + 32, 440, 44), ConfigNotes[i], Theme.Body, Theme.Text);
            }
            if (error != null)
            {
                Widgets.Text(new Rect(80, 480, 1120, 80), error, Theme.Body, Theme.Ai);
            }
        }

        private void DrawDuel()
        {
            DuelSession s = Session;
            DuelPanels.TopBar(new Rect(0, 0, Theme.Width, 44), headerText, rightText);
            float now = Time.realtimeSinceStartup;
            DuelPanels.Fighters(new Rect(20, 56, 800, 48),
                playerHpText, (float)s.PlayerHp / s.StartHp, Flash(now - playerHitAt),
                aiHpText, (float)s.OpponentHp / s.StartHp, Flash(now - aiHitAt));
            DuelPanels.Arena(new Rect(20, 112, 800, 218), s.Reveal, Waiting);
            bool choosing = s.Phase == DuelPhase.Choosing && !showHelp;
            Widgets.Text(new Rect(20, 336, 800, 26), statusText, Theme.Body, choosing ? Theme.Text : Theme.Muted);
            int clicked = DuelPanels.MoveButtons(new Rect(20, 364, 800, 108), s.Moves, choosing);
            if (clicked >= 0)
            {
                Submit(clicked);
            }
            DuelPanels.HistoryStrip(new Rect(20, 484, 800, 224), s.History);
            BrainPanel.Draw(new Rect(840, 56, 420, 652), s.Meters, s.Belief);
            if (s.LogWarning != null)
            {
                Widgets.Text(new Rect(840, 20, 420, 20), s.LogWarning, Theme.Small, Theme.Ai);
            }
            if (showHelp && Screens.Help(s.Moves, matchRules))
            {
                showHelp = false;
            }
        }

        private void DrawSummary()
        {
            if (Screens.Summary(Session.Summary, Session.History.ChanceFraction, summaryTitle, logPathText))
            {
                Session = null;
            }
        }

        private float Flash(float elapsed) => elapsed < FlashSeconds ? 1f - elapsed / FlashSeconds : 0f;

        private void HandleKeys(Event e)
        {
            if (e == null || (e.type != EventType.KeyDown && e.type != EventType.KeyUp))
            {
                return;
            }
            if (e.keyCode == KeyCode.H || (e.keyCode == KeyCode.Escape && showHelp))
            {
                if (e.type == EventType.KeyDown)
                {
                    showHelp = e.keyCode == KeyCode.H && !showHelp;
                }
                e.Use();
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
                if (!showHelp)
                {
                    Submit(action);
                }
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
            if (Session.Reveal.Winner == ExchangeWinner.Ai)
            {
                playerHitAt = revealStartedAt;
            }
            else if (Session.Reveal.Winner == ExchangeWinner.Player)
            {
                aiHitAt = revealStartedAt;
            }
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
            headerText = "Match " + (s.Match + 1) + " / " + DuelSession.Matches + "      Exchange " +
                Math.Min(s.ExchangeNumber, DuelSession.ExchangeCap) + " / " + DuelSession.ExchangeCap;
            rightText = "Matches won: you " + s.PlayerMatchWins + " - " + s.OpponentMatchWins + " AI      AI: " + s.Label +
                (configFile.Contains("experimental") ? " [EXPERIMENTAL CONFIG]" : "") + "      [H] How to play";
            playerHpText = "YOU   " + Math.Max(0, s.PlayerHp) + " / " + s.StartHp + " HP";
            aiHpText = "AI   " + Math.Max(0, s.OpponentHp) + " / " + s.StartHp + " HP";
            statusText = s.Phase == DuelPhase.Choosing
                ? "The AI has locked in its move (proof code " + s.CommitmentShort + "...). Pick yours: click a move or press 1-6."
                : "Revealing...";
            summaryTitle = "SESSION OVER   (" + s.Label + ", seed " + s.Seed + ")";
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
