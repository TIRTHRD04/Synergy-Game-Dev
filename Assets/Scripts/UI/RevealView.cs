using AdaptiveOpponent.Core;

namespace AdaptiveOpponent.UI
{
    /// <summary>Who won the last revealed exchange, from the player's side.</summary>
    public enum ExchangeWinner
    {
        /// <summary>Nobody took damage.</summary>
        Draw,
        /// <summary>The player's move beat the AI's.</summary>
        Player,
        /// <summary>The AI's move beat the player's.</summary>
        Ai,
    }

    /// <summary>The last revealed exchange, as cached strings: both moves, who won, why, and the commitment nonce.</summary>
    public sealed class RevealView
    {
        /// <summary>Whether an exchange has been revealed yet this session.</summary>
        public bool HasData { get; private set; }

        /// <summary>The player's move (index).</summary>
        public int PlayerMove { get; private set; }

        /// <summary>The AI's move (index).</summary>
        public int AiMove { get; private set; }

        /// <summary>Who won the exchange.</summary>
        public ExchangeWinner Winner { get; private set; }

        /// <summary>"Exchange 12", the exchange this reveal belongs to.</summary>
        public string Title { get; private set; } = "";

        /// <summary>The player's move name.</summary>
        public string PlayerMoveName { get; private set; } = "";

        /// <summary>The AI's move name.</summary>
        public string AiMoveName { get; private set; } = "";

        /// <summary>The headline, e.g. "AI WINS: Dodge beats Grab (clean hit). You take 3."</summary>
        public string Banner { get; private set; } = "";

        /// <summary>Why: the winning move's description, or why a draw is a draw.</summary>
        public string Reason { get; private set; } = "";

        /// <summary>"BIG HIT" text, or empty.</summary>
        public string BigHit { get; private set; } = "";

        /// <summary>The nonce that opens the commitment.</summary>
        public string Nonce { get; private set; } = "";

        /// <summary>Sets the strings for one revealed exchange.</summary>
        public void Set(GameDefinition definition, int exchangeNumber, int action, int response, Outcome outcome,
            int damage, bool bigHit, BigHitSide side, string nonce)
        {
            HasData = true;
            PlayerMove = action;
            AiMove = response;
            Title = "Exchange " + exchangeNumber;
            PlayerMoveName = definition.ActionName(action);
            AiMoveName = definition.ResponseName(response);
            string hit = damage >= definition.MaxDamage ? "clean hit" : "glancing hit";
            switch (outcome)
            {
                case Outcome.Win:
                    Winner = ExchangeWinner.Ai;
                    Banner = "AI WINS: " + AiMoveName + " beats " + PlayerMoveName + " (" + hit + "). You take " + damage + ".";
                    Reason = AiMoveName + ": " + definition.ActionHint(response);
                    break;
                case Outcome.Loss:
                    Winner = ExchangeWinner.Player;
                    Banner = "YOU WIN: " + PlayerMoveName + " beats " + AiMoveName + " (" + hit + "). AI takes " + damage + ".";
                    Reason = PlayerMoveName + ": " + definition.ActionHint(action);
                    break;
                default:
                    Winner = ExchangeWinner.Draw;
                    Banner = "DRAW: " + PlayerMoveName + " vs " + AiMoveName + ". No damage.";
                    Reason = action == response ? "Same move: they cancel out." : "These two moves trade evenly.";
                    break;
            }
            BigHit = !bigHit ? "" : side == BigHitSide.OnOpponent
                ? "BIG HIT! You hit the AI three times in a row."
                : "BIG HIT! The AI hit you three times in a row.";
            Nonce = "Proof key: " + nonce;
        }
    }
}
