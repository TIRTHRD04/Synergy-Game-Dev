using UnityEngine;

namespace AdaptiveOpponent.UI
{
    /// <summary>
    /// The duel screen's main column: top bar, HP bars, the arena with the last reveal, the move buttons and the
    /// read-history strip with the live learning curve. Draws cached view strings only.
    /// </summary>
    public static class DuelPanels
    {
        private const float CardWidth = 230f;
        private const float CardHeight = 110f;
        private static readonly string StripLegend = "DID IT READ YOU?  Last " + HistoryView.Shown +
            " exchanges. Top: your move. Bottom: the AI's prediction. Red = it predicted right.";
        private static readonly string CurveLegend = "LEARNING CURVE: how often it read you, per " + HistoryView.Bucket +
            " exchanges. Dashed line = chance.";

        /// <summary>Draws the top bar: title, match and exchange, and the right-hand info.</summary>
        public static void TopBar(Rect area, string center, string right)
        {
            Widgets.Fill(area, Theme.Panel);
            Widgets.Text(new Rect(area.x + 20, area.y, 300, area.height), "ADAPTIVE DUEL", Theme.Title, Theme.Accent);
            Widgets.Text(new Rect(area.x + 300, area.y, 520, area.height), center, Theme.HeadingCenter, Theme.Text);
            Widgets.Text(new Rect(area.xMax - 460, area.y, 440, area.height), right, Theme.BodyRight, Theme.Muted);
        }

        /// <summary>Draws both HP bars. A flash value above 0 outlines a bar that just took damage.</summary>
        public static void Fighters(Rect area, string playerText, float playerFraction, float playerFlash,
            string aiText, float aiFraction, float aiFlash)
        {
            float half = (area.width - 20) / 2;
            Fighter(new Rect(area.x, area.y, half, area.height), playerText, playerFraction, playerFlash, Theme.Player);
            Fighter(new Rect(area.x + half + 20, area.y, half, area.height), aiText, aiFraction, aiFlash, Theme.Ai);
        }

        /// <summary>Draws the last revealed exchange: both moves, who won and why.</summary>
        public static void Arena(Rect area, RevealView reveal, string waiting)
        {
            Widgets.Fill(area, Theme.Panel);
            if (!reveal.HasData)
            {
                Widgets.Text(area, waiting, Theme.HeadingCenter, Theme.Muted);
                return;
            }
            Widgets.Text(new Rect(area.x + 14, area.y + 6, 300, 18), reveal.Title, Theme.Label, Theme.Muted);
            var left = new Rect(area.x + 20, area.y + 28, CardWidth, CardHeight);
            var right = new Rect(area.xMax - 20 - CardWidth, area.y + 28, CardWidth, CardHeight);
            Card(left, "YOU", reveal.PlayerMoveName, reveal.PlayerMove, reveal.Winner == ExchangeWinner.Player, Theme.Player);
            Card(right, "AI", reveal.AiMoveName, reveal.AiMove, reveal.Winner == ExchangeWinner.Ai, Theme.Ai);
            var middle = new Rect(left.xMax, left.y, right.x - left.xMax, CardHeight);
            switch (reveal.Winner)
            {
                case ExchangeWinner.Player:
                    Widgets.Text(middle, "BEATS  >>", Theme.HeadingCenter, Theme.Player);
                    break;
                case ExchangeWinner.Ai:
                    Widgets.Text(middle, "<<  BEATS", Theme.HeadingCenter, Theme.Ai);
                    break;
                default:
                    Widgets.Text(middle, "=  DRAW  =", Theme.HeadingCenter, Theme.Neutral);
                    break;
            }
            Color bannerColor = reveal.Winner == ExchangeWinner.Player ? Theme.Player
                : reveal.Winner == ExchangeWinner.Ai ? Theme.Ai : Theme.Neutral;
            float x = area.x + 20;
            float w = area.width - 40;
            Widgets.Text(new Rect(x, area.y + 146, w, 24), reveal.Banner, Theme.Heading, bannerColor);
            Widgets.Text(new Rect(x, area.y + 172, w, 20), reveal.Reason, Theme.Body, Theme.Muted);
            Widgets.Text(new Rect(x, area.y + 194, 420, 18), reveal.BigHit, Theme.Label, Theme.Accent);
            Widgets.Text(new Rect(area.xMax - 20 - 380, area.y + 194, 380, 18), reveal.Nonce, Theme.SmallRight, Theme.Muted);
        }

        /// <summary>Draws one button per move; returns the clicked move, or -1.</summary>
        public static int MoveButtons(Rect area, MovesView moves, bool enabled)
        {
            const float gap = 10f;
            int n = moves.Names.Length;
            float w = (area.width - gap * (n - 1)) / n;
            int clicked = -1;
            for (int a = 0; a < n; a++)
            {
                var r = new Rect(area.x + a * (w + gap), area.y, w, area.height);
                Widgets.Fill(r, enabled && Widgets.Hovered(r) ? Theme.Hover : Theme.Raised);
                Widgets.Fill(new Rect(r.x, r.y, r.width, 5), Theme.Move(a));
                Widgets.Fill(new Rect(r.x + 6, r.y + 11, 18, 18), Theme.Track);
                Widgets.Text(new Rect(r.x + 6, r.y + 11, 18, 18), moves.Keys[a], Theme.SmallCenter, Theme.Text);
                Widgets.Text(new Rect(r.x, r.y + 9, r.width, 24), moves.Names[a], Theme.HeadingCenter, Theme.Move(a));
                Widgets.Text(new Rect(r.x + 6, r.y + 38, r.width - 12, 32), moves.Beats[a], Theme.SmallCenter, Theme.Player);
                Widgets.Text(new Rect(r.x + 6, r.y + 70, r.width - 12, 32), moves.LosesTo[a], Theme.SmallCenter, Theme.Ai);
                if (!enabled)
                {
                    Widgets.Fill(r, new Color(0f, 0f, 0f, 0.35f));
                }
                else if (Widgets.Clicked(r))
                {
                    clicked = a;
                }
            }
            return clicked;
        }

        /// <summary>Draws the last exchanges (read or not) and the live learning curve.</summary>
        public static void HistoryStrip(Rect area, HistoryView history)
        {
            Widgets.Fill(area, Theme.Panel);
            float x = area.x + 12;
            float w = area.width - 24;
            float y = area.y + 8;
            Widgets.Text(new Rect(x, y, w, 18), StripLegend, Theme.Label, Theme.Accent);
            const float gap = 4f;
            float cw = (w - gap * (HistoryView.Shown - 1)) / HistoryView.Shown;
            for (int i = 0; i < HistoryView.Shown; i++)
            {
                var r = new Rect(x + i * (cw + gap), y + 24, cw, 50);
                if (i >= history.Count)
                {
                    Widgets.Fill(r, Theme.Raised);
                    continue;
                }
                Widgets.Fill(r, history.Reads[i] ? Theme.ReadCell : Theme.Raised);
                Color winner = history.Winners[i] == ExchangeWinner.Player ? Theme.Player
                    : history.Winners[i] == ExchangeWinner.Ai ? Theme.Ai : Theme.Neutral;
                Widgets.Fill(new Rect(r.x, r.yMax - 3, r.width, 3), winner);
                Widgets.Text(new Rect(r.x + 2, r.y + 6, r.width - 4, 16), history.PlayerMoves[i], Theme.SmallCenter, Theme.Text);
                Widgets.Text(new Rect(r.x + 2, r.y + 26, r.width - 4, 16), history.Guesses[i], Theme.SmallCenter, Theme.Muted);
            }
            Widgets.Text(new Rect(x, y + 80, w / 2, 18), history.MatchText, Theme.Small, Theme.Text);
            Widgets.Text(new Rect(x + w / 2, y + 80, w / 2, 18), history.SessionText, Theme.SmallRight, Theme.Text);
            Widgets.Text(new Rect(x, y + 100, w, 18), CurveLegend, Theme.Label, Theme.Accent);
            Widgets.Columns(new Rect(x, y + 120, w, 84), history.Curve, history.Curve.Count, history.ChanceFraction, Theme.Ai);
        }

        private static void Fighter(Rect area, string text, float fraction, float flash, Color color)
        {
            Widgets.Text(new Rect(area.x, area.y, area.width, 22), text, Theme.Heading, color);
            var bar = new Rect(area.x, area.y + 26, area.width, 20);
            Widgets.Bar(bar, fraction, color);
            if (flash > 0f)
            {
                Widgets.Outline(bar, new Color(1f, 1f, 1f, flash), 3f);
            }
        }

        private static void Card(Rect area, string who, string move, int moveIndex, bool won, Color winColor)
        {
            Widgets.Fill(area, Theme.Raised);
            Widgets.Fill(new Rect(area.x, area.y, area.width, 6), Theme.Move(moveIndex));
            if (won)
            {
                Widgets.Outline(area, winColor, 3f);
            }
            Widgets.Text(new Rect(area.x + 12, area.y + 12, area.width - 24, 18), who, Theme.Label, Theme.Muted);
            Widgets.Text(new Rect(area.x, area.y + 34, area.width, 56), move, Theme.Big, Theme.Move(moveIndex));
        }
    }
}
