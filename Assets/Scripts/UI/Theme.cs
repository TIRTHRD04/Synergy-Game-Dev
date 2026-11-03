using UnityEngine;

namespace AdaptiveOpponent.UI
{
    /// <summary>
    /// Colors, text styles and the reference layout size of the duel screen. The screen is laid out at
    /// <see cref="Width"/> x <see cref="Height"/> and scaled to the window. Styles are created once, on the first
    /// OnGUI call (GUI.skin is only available there).
    /// </summary>
    public static class Theme
    {
        /// <summary>Reference layout width in pixels.</summary>
        public const float Width = 1280f;

        /// <summary>Reference layout height in pixels.</summary>
        public const float Height = 720f;

        /// <summary>Screen background.</summary>
        public static readonly Color Background = new Color32(16, 18, 26, 255);

        /// <summary>Panel background.</summary>
        public static readonly Color Panel = new Color32(30, 34, 46, 255);

        /// <summary>Raised elements inside a panel (cards, buttons).</summary>
        public static readonly Color Raised = new Color32(46, 52, 68, 255);

        /// <summary>A hovered button.</summary>
        public static readonly Color Hover = new Color32(64, 72, 94, 255);

        /// <summary>Empty part of a bar.</summary>
        public static readonly Color Track = new Color32(58, 63, 82, 255);

        /// <summary>Main text.</summary>
        public static readonly Color Text = new Color32(232, 234, 242, 255);

        /// <summary>Secondary text.</summary>
        public static readonly Color Muted = new Color32(150, 157, 178, 255);

        /// <summary>The player's color.</summary>
        public static readonly Color Player = new Color32(84, 204, 128, 255);

        /// <summary>The AI's color.</summary>
        public static readonly Color Ai = new Color32(238, 92, 92, 255);

        /// <summary>Draws and neutral bars.</summary>
        public static readonly Color Neutral = new Color32(150, 154, 170, 255);

        /// <summary>A history cell where the AI read the player.</summary>
        public static readonly Color ReadCell = new Color32(122, 40, 48, 255);

        /// <summary>Section labels and highlights.</summary>
        public static readonly Color Accent = new Color32(255, 196, 64, 255);

        private static readonly Color[] MoveColors =
        {
            new Color32(250, 205, 70, 255),
            new Color32(70, 200, 230, 255),
            new Color32(175, 115, 245, 255),
            new Color32(245, 135, 60, 255),
            new Color32(85, 135, 250, 255),
            new Color32(240, 100, 170, 255),
        };

        /// <summary>26 pt bold.</summary>
        public static GUIStyle Title { get; private set; }

        /// <summary>36 pt bold, centered (move names on the cards).</summary>
        public static GUIStyle Big { get; private set; }

        /// <summary>17 pt bold.</summary>
        public static GUIStyle Heading { get; private set; }

        /// <summary>17 pt bold, centered.</summary>
        public static GUIStyle HeadingCenter { get; private set; }

        /// <summary>14 pt, wrapping.</summary>
        public static GUIStyle Body { get; private set; }

        /// <summary>14 pt, right-aligned.</summary>
        public static GUIStyle BodyRight { get; private set; }

        /// <summary>12 pt bold (section labels).</summary>
        public static GUIStyle Label { get; private set; }

        /// <summary>12 pt, wrapping.</summary>
        public static GUIStyle Small { get; private set; }

        /// <summary>12 pt, centered, wrapping.</summary>
        public static GUIStyle SmallCenter { get; private set; }

        /// <summary>12 pt, right-aligned.</summary>
        public static GUIStyle SmallRight { get; private set; }

        /// <summary>The color of move <paramref name="index"/>.</summary>
        public static Color Move(int index) => MoveColors[index % MoveColors.Length];

        /// <summary>Creates the styles if they do not exist yet. Call at the start of OnGUI.</summary>
        public static void Ensure()
        {
            if (Body != null)
            {
                return;
            }
            Title = Make(26, FontStyle.Bold, TextAnchor.MiddleLeft, false);
            Big = Make(36, FontStyle.Bold, TextAnchor.MiddleCenter, false);
            Heading = Make(17, FontStyle.Bold, TextAnchor.MiddleLeft, true);
            HeadingCenter = Make(17, FontStyle.Bold, TextAnchor.MiddleCenter, true);
            Body = Make(14, FontStyle.Normal, TextAnchor.UpperLeft, true);
            BodyRight = Make(14, FontStyle.Normal, TextAnchor.MiddleRight, false);
            Label = Make(12, FontStyle.Bold, TextAnchor.MiddleLeft, false);
            Small = Make(12, FontStyle.Normal, TextAnchor.UpperLeft, true);
            SmallCenter = Make(12, FontStyle.Normal, TextAnchor.UpperCenter, true);
            SmallRight = Make(12, FontStyle.Normal, TextAnchor.MiddleRight, false);
        }

        // White text, so Widgets.Text can tint it with GUI.contentColor.
        private static GUIStyle Make(int size, FontStyle fontStyle, TextAnchor anchor, bool wrap)
        {
            var style = new GUIStyle(GUI.skin.label)
            {
                fontSize = size,
                fontStyle = fontStyle,
                alignment = anchor,
                wordWrap = wrap,
                richText = false,
                padding = new RectOffset(0, 0, 0, 0),
                margin = new RectOffset(0, 0, 0, 0),
            };
            style.normal.textColor = Color.white;
            style.hover.textColor = Color.white;
            return style;
        }
    }
}
