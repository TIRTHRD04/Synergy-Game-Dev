using UnityEngine;

namespace AdaptiveOpponent.UI
{
    /// <summary>
    /// The "AI Brain" panel: the live meters (safe while the player chooses) and, after each reveal, what the AI
    /// expected, whether it read the player, how it chose its move and the pattern it can back with evidence.
    /// </summary>
    public static class BrainPanel
    {
        private const float Pad = 14f;
        private const string Subtitle = "Live view of the AI's model of you. Nothing here is scripted.";
        private const string Empty = "Make your first move. After each reveal this shows what the AI expected you to play.";

        /// <summary>Draws the panel into <paramref name="area"/>.</summary>
        public static void Draw(Rect area, MetersView meters, BeliefView belief)
        {
            Widgets.Fill(area, Theme.Panel);
            float x = area.x + Pad;
            float w = area.width - 2 * Pad;
            float y = area.y + 10;

            Widgets.Text(new Rect(x, y, w, 26), "AI BRAIN", Theme.Heading, Theme.Accent);
            Widgets.Text(new Rect(x, y + 24, w, 18), Subtitle, Theme.Small, Theme.Muted);
            y += 50;

            y = Meter(x, y, w, "CONFIDENCE YOU HAVE HABITS", meters.ConfidenceFraction,
                meters.LockedOn ? Theme.Ai : Theme.Accent, meters.ConfidenceText);
            y = Meter(x, y, w, "HOW OFTEN IT ACTS ON ITS READ", meters.LambdaFraction, Theme.Ai, meters.LambdaText);
            Widgets.Text(new Rect(x, y, w, 20), meters.ReadingText, Theme.Small, Theme.Text);
            y += 26;
            Widgets.Fill(new Rect(x, y, w, 1), Theme.Track);
            y += 10;

            if (!belief.HasData)
            {
                Widgets.Text(new Rect(x, y, w, 60), Empty, Theme.Body, Theme.Muted);
                return;
            }
            Widgets.Text(new Rect(x, y, w, 20), belief.Heading, Theme.Label, Theme.Accent);
            y += 24;
            y = ForecastBars(x, y, w, belief);
            Widgets.Text(new Rect(x, y, w, 42), belief.Verdict, Theme.Heading, belief.Read ? Theme.Ai : Theme.Player);
            y += 44;
            Widgets.Text(new Rect(x, y, w, 40), belief.Choice, Theme.Small, Theme.Text);
            y += 44;
            Widgets.Text(new Rect(x, y, w, 18), "THE AI SAYS", Theme.Label, Theme.Accent);
            Widgets.Text(new Rect(x, y + 20, w, 40), belief.Pattern, Theme.Body, Theme.Text);
        }

        private static float Meter(float x, float y, float w, string label, float fraction, Color color, string text)
        {
            Widgets.Text(new Rect(x, y, w, 18), label, Theme.Label, Theme.Muted);
            Widgets.Bar(new Rect(x, y + 20, w, 12), fraction, color);
            Widgets.Text(new Rect(x, y + 36, w, 34), text, Theme.Small, Theme.Text);
            return y + 72;
        }

        // One bar per move: its forecast, a marker at chance, the top prediction in the AI's color, the move the player
        // actually played tagged "YOU".
        private static float ForecastBars(float x, float y, float w, BeliefView belief)
        {
            const float nameWidth = 62f;
            const float pctWidth = 40f;
            const float tagWidth = 38f;
            float barWidth = w - nameWidth - pctWidth - tagWidth - 12;
            for (int a = 0; a < belief.Names.Length; a++)
            {
                Color nameColor = belief.IsActual[a] ? Theme.Move(a) : Theme.Text;
                Widgets.Text(new Rect(x, y, nameWidth, 18), belief.Names[a], Theme.Small, nameColor);
                var bar = new Rect(x + nameWidth, y + 3, barWidth, 12);
                Widgets.Bar(bar, belief.Fractions[a], belief.IsTop[a] ? Theme.Ai : Theme.Neutral);
                Widgets.Marker(bar, belief.ChanceFraction, Theme.Accent);
                Widgets.Text(new Rect(bar.xMax + 4, y, pctWidth, 18), belief.Percents[a], Theme.SmallRight, Theme.Text);
                if (belief.IsActual[a])
                {
                    Widgets.Text(new Rect(bar.xMax + pctWidth + 10, y, tagWidth, 18), "< YOU", Theme.Label, Theme.Move(a));
                }
                y += 22;
            }
            Widgets.Text(new Rect(x + nameWidth, y, barWidth, 16), belief.ChanceText, Theme.Small, Theme.Muted);
            return y + 20;
        }
    }
}
