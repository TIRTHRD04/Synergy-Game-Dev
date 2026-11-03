using UnityEngine;

namespace AdaptiveOpponent.UI
{
    /// <summary>Full-screen layers of the duel screen: the how-to-play overlay and the end-of-session summary.</summary>
    public static class Screens
    {
        private const string HelpIntro =
            "Each exchange, you and the AI both pick one of six moves. The AI locks its move in first and shows a proof " +
            "code, so after the reveal you can check it never changed its move after seeing yours.";
        private const string HelpRing =
            "The moves form a ring. Each move beats the next move cleanly (3 damage) and the one after that with a " +
            "glancing hit (1 damage). It ties with itself and with the move opposite it.";
        private static readonly string CurveLegend = "LEARNING CURVE: read rate per " + HistoryView.Bucket +
            " exchanges over the whole session (dashed line = chance)";
        private const string HelpAi =
            "HOW THE AI LEARNS: it watches your moves, predicts your next one and plays the move that beats it. It only " +
            "starts acting on its read once it has statistical evidence that you have habits; against truly random play it never " +
            "does. After every reveal, the AI Brain panel shows what it expected, whether it read you and why it played " +
            "its move. The learning curve at the bottom shows how often it reads you as the session goes on.";

        /// <summary>
        /// Draws the how-to-play overlay; <paramref name="matchRules"/> states HP, exchange cap and matches. Returns
        /// true when "Got it" is clicked.
        /// </summary>
        public static bool Help(MovesView moves, string matchRules)
        {
            Widgets.Fill(new Rect(0, 0, Theme.Width, Theme.Height), new Color(0f, 0f, 0f, 0.82f));
            var box = new Rect(170, 50, 940, 620);
            Widgets.Fill(box, Theme.Panel);
            Widgets.Outline(box, Theme.Accent, 2f);
            float x = box.x + 30;
            float w = box.width - 60;
            float y = box.y + 20;
            Widgets.Text(new Rect(x, y, w, 34), "HOW TO PLAY", Theme.Title, Theme.Accent);
            y += 44;
            Widgets.Text(new Rect(x, y, w, 44), HelpIntro, Theme.Body, Theme.Text);
            y += 50;
            Widgets.Text(new Rect(x, y, w, 44), HelpRing, Theme.Body, Theme.Text);
            y += 50;
            for (int a = 0; a < moves.HelpLines.Length; a++)
            {
                Widgets.Fill(new Rect(x, y + 4, 10, 10), Theme.Move(a));
                Widgets.Text(new Rect(x + 18, y, w - 18, 20), moves.HelpLines[a], Theme.Body, Theme.Text);
                Widgets.Text(new Rect(x + 18, y + 19, w - 18, 18), moves.Hints[a], Theme.Small, Theme.Muted);
                y += 42;
            }
            y += 4;
            Widgets.Text(new Rect(x, y, w, 20), matchRules, Theme.Body, Theme.Text);
            y += 28;
            Widgets.Text(new Rect(x, y, w, 80), HelpAi, Theme.Body, Theme.Accent);
            return Widgets.Button(new Rect(box.center.x - 110, box.yMax - 52, 220, 38), "Got it  (H)", Theme.Raised);
        }

        /// <summary>Draws the end-of-session summary; returns true when "New session" is clicked.</summary>
        public static bool Summary(SummaryView summary, float chance, string title, string logPath)
        {
            Widgets.Fill(new Rect(0, 0, Theme.Width, Theme.Height), Theme.Background);
            Widgets.Text(new Rect(40, 20, 1200, 40), title, Theme.Title, Theme.Accent);
            var left = new Rect(40, 76, 580, 300);
            Widgets.Fill(left, Theme.Panel);
            Widgets.Text(new Rect(left.x + 16, left.y + 10, 540, 18), "RESULTS", Theme.Label, Theme.Accent);
            for (int i = 0; i < summary.Results.Length; i++)
            {
                Widgets.Text(new Rect(left.x + 16, left.y + 36 + i * 50, 548, 46), summary.Results[i], Theme.Body, Theme.Text);
            }

            var right = new Rect(640, 76, 600, 300);
            Widgets.Fill(right, Theme.Panel);
            Widgets.Text(new Rect(right.x + 16, right.y + 10, 560, 18),
                "HOW OFTEN IT READ YOU, PER MATCH (yellow tick = chance)", Theme.Label, Theme.Accent);
            for (int m = 0; m < summary.MatchRates.Length; m++)
            {
                float y = right.y + 44 + m * 76;
                Widgets.Text(new Rect(right.x + 16, y, 560, 20), summary.MatchLabels[m], Theme.Heading, Theme.Text);
                var bar = new Rect(right.x + 16, y + 28, 560, 22);
                Widgets.Bar(bar, summary.MatchRates[m], Theme.Ai);
                Widgets.Marker(bar, chance, Theme.Accent);
            }

            var curve = new Rect(40, 392, 1200, 150);
            Widgets.Fill(curve, Theme.Panel);
            Widgets.Text(new Rect(curve.x + 16, curve.y + 10, 1160, 18), CurveLegend, Theme.Label, Theme.Accent);
            Widgets.Columns(new Rect(curve.x + 16, curve.y + 34, 1168, 104), summary.Curve, summary.Curve.Length, chance,
                Theme.Ai);

            var patterns = new Rect(40, 556, 860, 150);
            Widgets.Fill(patterns, Theme.Panel);
            Widgets.Text(new Rect(patterns.x + 16, patterns.y + 10, 820, 18),
                "HABITS IT SPOTTED (how many exchanges it said so)", Theme.Label, Theme.Accent);
            for (int i = 0; i < summary.Patterns.Length; i++)
            {
                Widgets.Text(new Rect(patterns.x + 16, patterns.y + 34 + i * 22, 828, 20), summary.Patterns[i], Theme.Body,
                    Theme.Text);
            }

            Widgets.Text(new Rect(920, 560, 320, 80), logPath, Theme.Small, Theme.Muted);
            return Widgets.Button(new Rect(920, 656, 320, 44), "New session", Theme.Raised);
        }
    }
}
