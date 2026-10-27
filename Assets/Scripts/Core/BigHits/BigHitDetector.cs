namespace AdaptiveOpponent.Core
{
    /// <summary>Who took the damage in a big hit.</summary>
    public enum BigHitSide
    {
        /// <summary>The player landed the big hit; the Opponent took the damage.</summary>
        OnOpponent,
        /// <summary>The Opponent landed the big hit; the player took the damage.</summary>
        OnPlayer,
    }

    /// <summary>One detected big hit (ADR 0011). The Big-hit sequence is read from the history by index.</summary>
    public struct BigHit
    {
        /// <summary>Who took the damage.</summary>
        public BigHitSide Side;
        /// <summary>Session index of the first exchange of the big hit (the Opening).</summary>
        public int StartIndex;
        /// <summary>Session index of the exchange that completed it.</summary>
        public int EndIndex;
        /// <summary>Previous action of the Start context (a0); the Match-start symbol on exchange 1.</summary>
        public int StartPrevAction;
        /// <summary>Previous response of the Start context (r0); the Match-start symbol on exchange 1.</summary>
        public int StartPrevResponse;
        /// <summary>Total damage over the window.</summary>
        public int TotalDamage;
    }

    /// <summary>
    /// Detects big hits as exchanges are recorded: <c>window</c> consecutive exchanges in which the same side takes
    /// damage every time, totalling at least <c>threshold</c>. Windows are scanned left to right and never overlap or
    /// cross a match boundary. The online check gives exactly the same big hits as the offline left-to-right scan.
    /// </summary>
    public sealed class BigHitDetector
    {
        private readonly int window;
        private readonly int threshold;
        private int nextAllowedStart;
        private int lastMatch = -1;

        /// <summary>Creates a detector for the given window length and damage threshold.</summary>
        public BigHitDetector(int window, int threshold)
        {
            this.window = window;
            this.threshold = threshold;
        }

        /// <summary>Creates a detector using the definition's big-hit rule.</summary>
        public BigHitDetector(GameDefinition definition) : this(definition.BigHitWindow, definition.BigHitThreshold)
        {
        }

        /// <summary>
        /// Call after each exchange is added to <paramref name="history"/>. Returns true, with the big hit, when the
        /// latest exchange completes one.
        /// </summary>
        public bool Observe(ExchangeHistory history, out BigHit hit)
        {
            hit = default;
            int end = history.Count - 1;
            int match = history.MatchOf(end);
            if (match != lastMatch)
            {
                lastMatch = match;
                nextAllowedStart = end;
            }
            int start = end - window + 1;
            if (start < nextAllowedStart || history.MatchOf(start) != match)
            {
                return false;
            }
            int sign = Sign(history.Payoff(start));
            if (sign == 0)
            {
                return false;
            }
            int total = 0;
            for (int i = start; i <= end; i++)
            {
                int payoff = history.Payoff(i);
                if (Sign(payoff) != sign)
                {
                    return false;
                }
                total += payoff < 0 ? -payoff : payoff;
            }
            if (total < threshold)
            {
                return false;
            }
            nextAllowedStart = end + 1;
            hit.Side = sign < 0 ? BigHitSide.OnOpponent : BigHitSide.OnPlayer;
            hit.StartIndex = start;
            hit.EndIndex = end;
            hit.StartPrevAction = history.PrevActionBefore(start);
            hit.StartPrevResponse = history.PrevResponseBefore(start);
            hit.TotalDamage = total;
            return true;
        }

        private static int Sign(int value) => value > 0 ? 1 : value < 0 ? -1 : 0;
    }
}
