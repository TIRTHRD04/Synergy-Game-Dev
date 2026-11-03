using System;
using System.Globalization;
using AdaptiveOpponent.Core;

namespace AdaptiveOpponent.UI
{
    /// <summary>
    /// The AI's live state as meters, rebuilt each time it commits: how much evidence it has that the player has
    /// habits (the evidence gate), how likely it is to act on its read this exchange (lambda), and which pattern it leans on.
    /// None of these say which move it expects, so they are safe to show while the player chooses (ADR 0012).
    /// </summary>
    public sealed class MetersView
    {
        /// <summary>Evidence gate progress, 0-1 (1 once the gate is open).</summary>
        public float ConfidenceFraction { get; private set; }

        /// <summary>Whether the evidence gate is open.</summary>
        public bool LockedOn { get; private set; }

        /// <summary>What the confidence meter means right now.</summary>
        public string ConfidenceText { get; private set; } = "";

        /// <summary>The chance it plays its best response this exchange (lambda), 0-1.</summary>
        public float LambdaFraction { get; private set; }

        /// <summary>What the lambda meter means right now.</summary>
        public string LambdaText { get; private set; } = "";

        /// <summary>"Relying most on: what you play after each move".</summary>
        public string ReadingText { get; private set; } = "";

        /// <summary>Rebuilds the strings after the Opponent commits. <paramref name="lockedAt"/> is -1 until known.</summary>
        public void Update(Opponent opponent, int lockedAt)
        {
            CultureInfo ci = CultureInfo.InvariantCulture;
            LockedOn = opponent.GateOpen;
            double threshold = opponent.Config.GateThreshold;
            ConfidenceFraction = LockedOn ? 1f : (float)Math.Max(0, Math.Min(1, opponent.GateScore / threshold));
            if (LockedOn)
            {
                ConfidenceText = "LOCKED ON: it is confident you have habits" +
                    (lockedAt > 0 ? " (since exchange " + lockedAt + " of the session)." : ".");
            }
            else
            {
                ConfidenceText = "Evidence you have habits: " + (ConfidenceFraction * 100).ToString("0", ci) +
                    "% of what it needs. Until then it plays at random.";
            }
            LambdaFraction = (float)opponent.Lambda;
            LambdaText = !LockedOn
                ? "This exchange: random move. It does not act on its read until it is confident."
                : opponent.Lambda >= 0.995
                    ? "This exchange: it acts on its read (100%)."
                    : "This exchange: it acts on its read " + (opponent.Lambda * 100).ToString("0", ci) +
                        "% of the time and plays a random move otherwise.";
            ReadingText = opponent.LeadingExpert >= 0
                ? "Relying most on: " + CommentaryText.PlainExpertName(opponent.ExpertName(opponent.LeadingExpert))
                : "Relying most on: nothing yet";
        }
    }
}
