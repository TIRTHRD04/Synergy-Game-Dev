using System;

namespace AdaptiveOpponent.Sim
{
    /// <summary>Creates archetypes by name, and lists the suite.</summary>
    public static class Archetypes
    {
        /// <summary>The seven archetypes of the harness suite, in report order.</summary>
        public static readonly string[] Suite =
        {
            "Spammer", "Cycler", "WinStayLoseShift", "OutGuesser", "Random", "BigHitRepeater", "BurnedAvoider",
        };

        /// <summary>The scripted players used only by the level-k/mirror keep rule.</summary>
        public static readonly string[] Scripted = { "Level1", "Mirror1" };

        /// <summary>Creates a fresh archetype by its logged name.</summary>
        public static IPlayer Create(string name)
        {
            switch (name)
            {
                case "Spammer": return new Spammer();
                case "Cycler": return new Cycler();
                case "WinStayLoseShift": return new WinStayLoseShift();
                case "OutGuesser": return new OutGuesser();
                case "Random": return new RandomPlayer();
                case "BigHitRepeater": return new BigHitRepeater();
                case "BurnedAvoider": return new BurnedAvoider();
                case "Level1": return new Level1Player();
                case "Mirror1": return new Mirror1Player();
                default: throw new ArgumentException("unknown archetype: " + name);
            }
        }
    }
}
