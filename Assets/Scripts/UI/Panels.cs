using UnityEngine;

namespace AdaptiveOpponent.UI
{
    /// <summary>
    /// IMGUI drawing for the bare-bones duel screen. Every method draws cached strings into fixed rectangles, so a frame
    /// allocates nothing. Layout is for a 960 x 600 or larger window.
    /// </summary>
    public static class Panels
    {
        /// <summary>Line height in pixels.</summary>
        public const float Line = 22f;

        /// <summary>Draws a labelled HP bar.</summary>
        public static void HpBar(Rect area, string label, int hp, int maxHp)
        {
            GUI.Label(new Rect(area.x, area.y, area.width, Line), label);
            var bar = new Rect(area.x, area.y + Line, area.width, Line);
            GUI.Box(bar, GUIContent.none);
            float fraction = maxHp > 0 ? Mathf.Clamp01((float)hp / maxHp) : 0f;
            GUI.Box(new Rect(bar.x, bar.y, bar.width * fraction, bar.height), GUIContent.none);
        }

        /// <summary>Draws lines of text from the top of <paramref name="area"/>.</summary>
        public static void Lines(Rect area, string[] lines)
        {
            for (int i = 0; i < lines.Length; i++)
            {
                GUI.Label(new Rect(area.x, area.y + i * Line, area.width, Line), lines[i]);
            }
        }

        /// <summary>Draws the Belief: heading, then the top predictions.</summary>
        public static void Belief(Rect area, BeliefView belief)
        {
            GUI.Box(area, GUIContent.none);
            GUI.Label(new Rect(area.x + 8, area.y + 4, area.width - 16, Line * 2), belief.Heading);
            Lines(new Rect(area.x + 8, area.y + 4 + Line * 2, area.width - 16, Line * belief.Lines.Length), belief.Lines);
        }

        /// <summary>Draws the reveal of the last exchange.</summary>
        public static void Reveal(Rect area, RevealView reveal)
        {
            GUI.Label(new Rect(area.x, area.y, area.width, Line), reveal.Choices);
            GUI.Label(new Rect(area.x, area.y + Line, area.width, Line), reveal.Result);
            GUI.Label(new Rect(area.x, area.y + Line * 2, area.width, Line), reveal.BigHit);
            GUI.Label(new Rect(area.x, area.y + Line * 3, area.width, Line), reveal.Nonce);
        }

        /// <summary>Draws one action button; returns true when it was clicked.</summary>
        public static bool ActionButton(Rect area, string text) => GUI.Button(area, text);
    }
}
