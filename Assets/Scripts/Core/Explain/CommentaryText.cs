namespace AdaptiveOpponent.Core
{
    /// <summary>
    /// Builds player-facing sentences from Insight data. Every word comes from what the Opponent learned; there are
    /// no canned lines. Called by the UI only when the Commentary changes, never per frame.
    /// </summary>
    public static class CommentaryText
    {
        /// <summary>The sentence for an Insight or Recall.</summary>
        public static string Sentence(GameDefinition definition, Insight insight)
        {
            string action = definition.ActionName(insight.Action);
            switch (insight.Kind)
            {
                case InsightKind.Frequency:
                    return "You play " + action + " a lot.";
                case InsightKind.AfterAction:
                    return "After " + definition.ActionName(insight.Prev1) + ", you usually play " + action + ".";
                case InsightKind.AfterTwoActions:
                    return "After " + definition.ActionName(insight.Prev2) + " then " + definition.ActionName(insight.Prev1) +
                        ", you usually play " + action + ".";
                case InsightKind.AfterActionAndResponse:
                    return "After you play " + definition.ActionName(insight.Prev1) + " and I play " +
                        definition.ResponseName(insight.PrevResponse) + ", you usually play " + action + ".";
                case InsightKind.MatchStart:
                    return "At the start of a match, you usually play " + action + ".";
                case InsightKind.AfterResponse:
                    return "After I play " + definition.ResponseName(insight.PrevResponse) + ", you usually play " + action + ".";
                case InsightKind.RepeatSequence:
                    return "You're repeating a sequence from earlier.";
                case InsightKind.BeatMyCounter:
                    return "You're trying to beat my counter.";
                case InsightKind.ReactToMyResponse:
                    return "You're reacting to my last response.";
                case InsightKind.RecallOpening:
                    return "Last time we were here you hit me hard, starting with " + action + ".";
                case InsightKind.RecallFollowUp:
                    return "Last time you followed " + definition.ActionName(insight.Prev1) + " with " + action + ".";
                default:
                    return "";
            }
        }

        /// <summary>The leading expert in plain words, for the Belief line. No expert or gate jargon reaches the player.</summary>
        public static string PlainExpertName(string expert)
        {
            switch (expert)
            {
                case "order0": return "which moves you favour";
                case "order1": return "what you play after each move";
                case "order2": return "your last two moves";
                case "actionResponse": return "how you react to its moves";
                case "bigHitSequence": return "a big hit you landed earlier";
                case "avoidance": return "where you got hit hard";
                case "ctwActionFirst": return "your recent moves";
                case "ctwResponseFirst": return "its recent moves";
                case "longestMatch": return "a sequence you repeated";
                default:
                    if (expert.StartsWith("levelK", System.StringComparison.Ordinal))
                    {
                        return "you trying to beat its counter";
                    }
                    if (expert.StartsWith("mirror", System.StringComparison.Ordinal))
                    {
                        return "you reading its moves";
                    }
                    return expert;
            }
        }
    }
}
