using System.Collections.Generic;
using UnityEngine;

namespace AdaptiveOpponent.UI
{
    /// <summary>
    /// IMGUI drawing primitives for the duel screen: filled rectangles, tinted text, bars, buttons and a column chart.
    /// They draw cached strings and numbers only, so a frame allocates nothing.
    /// </summary>
    public static class Widgets
    {
        /// <summary>Fills <paramref name="area"/> with <paramref name="color"/>.</summary>
        public static void Fill(Rect area, Color color)
        {
            Color old = GUI.color;
            GUI.color = color;
            GUI.DrawTexture(area, Texture2D.whiteTexture);
            GUI.color = old;
        }

        /// <summary>Draws a one-pixel-thick (or <paramref name="thickness"/>) outline.</summary>
        public static void Outline(Rect area, Color color, float thickness)
        {
            Fill(new Rect(area.x, area.y, area.width, thickness), color);
            Fill(new Rect(area.x, area.yMax - thickness, area.width, thickness), color);
            Fill(new Rect(area.x, area.y, thickness, area.height), color);
            Fill(new Rect(area.xMax - thickness, area.y, thickness, area.height), color);
        }

        /// <summary>Draws <paramref name="text"/> in <paramref name="style"/>, tinted <paramref name="color"/>.</summary>
        public static void Text(Rect area, string text, GUIStyle style, Color color)
        {
            Color old = GUI.contentColor;
            GUI.contentColor = color;
            GUI.Label(area, text, style);
            GUI.contentColor = old;
        }

        /// <summary>Draws a horizontal bar filled to <paramref name="fraction"/> (0-1).</summary>
        public static void Bar(Rect area, float fraction, Color fill)
        {
            Fill(area, Theme.Track);
            Fill(new Rect(area.x, area.y, area.width * Mathf.Clamp01(fraction), area.height), fill);
        }

        /// <summary>Draws a thin vertical marker across a bar at <paramref name="fraction"/> of its width.</summary>
        public static void Marker(Rect bar, float fraction, Color color)
        {
            Fill(new Rect(bar.x + bar.width * Mathf.Clamp01(fraction) - 1, bar.y - 3, 2, bar.height + 6), color);
        }

        /// <summary>An invisible button over <paramref name="area"/>; returns true when clicked.</summary>
        public static bool Clicked(Rect area) => GUI.Button(area, GUIContent.none, GUIStyle.none);

        /// <summary>Whether the mouse is over <paramref name="area"/>.</summary>
        public static bool Hovered(Rect area) => Event.current != null && area.Contains(Event.current.mousePosition);

        /// <summary>A filled text button; returns true when clicked.</summary>
        public static bool Button(Rect area, string text, Color fill)
        {
            Fill(area, Hovered(area) ? Theme.Hover : fill);
            Text(area, text, Theme.HeadingCenter, Theme.Text);
            return Clicked(area);
        }

        /// <summary>
        /// A column chart of rates (0-1, one column each, the newest at the right) with a dashed line at
        /// <paramref name="chance"/>. Shows the last columns that fit when there are too many.
        /// </summary>
        public static void Columns(Rect area, IReadOnlyList<float> values, int count, float chance, Color color)
        {
            Fill(area, Theme.Raised);
            const float minWidth = 8f;
            const float gap = 2f;
            int fit = Mathf.Max(1, (int)((area.width + gap) / (minWidth + gap)));
            int start = Mathf.Max(0, count - fit);
            float width = Mathf.Max(minWidth, (area.width + gap) / Mathf.Max(1, count - start) - gap);
            width = Mathf.Min(width, 40f);
            for (int i = start; i < count; i++)
            {
                float h = area.height * Mathf.Clamp01(values[i]);
                float x = area.x + (i - start) * (width + gap);
                Fill(new Rect(x, area.yMax - h, width, h), color);
            }
            float y = area.yMax - area.height * Mathf.Clamp01(chance);
            for (float x = area.x; x < area.xMax; x += 10f)
            {
                Fill(new Rect(x, y, Mathf.Min(6f, area.xMax - x), 2f), Theme.Accent);
            }
        }
    }
}
