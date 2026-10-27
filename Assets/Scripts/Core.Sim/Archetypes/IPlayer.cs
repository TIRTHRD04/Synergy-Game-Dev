using AdaptiveOpponent.Core;

namespace AdaptiveOpponent.Sim
{
    /// <summary>What the last action of a player was for, so metrics can score the exchanges that matter.</summary>
    public enum PlayerTag
    {
        /// <summary>An ordinary action.</summary>
        None,
        /// <summary>Big-hit repeater: the Opening of a replayed Big-hit sequence.</summary>
        ReplayOpening,
        /// <summary>Big-hit repeater: the first Follow-up of a replay.</summary>
        ReplayFollowUp1,
        /// <summary>Big-hit repeater: the second Follow-up of a replay.</summary>
        ReplayFollowUp2,
        /// <summary>Burned avoider: the current context matches a big hit it took (exact or broad).</summary>
        BurnedContext,
    }

    /// <summary>A scripted player that stands in for a human in the harness (an archetype).</summary>
    public interface IPlayer
    {
        /// <summary>The archetype's name as logged.</summary>
        string Name { get; }

        /// <summary>The tag of the action returned by the last <see cref="Act"/>.</summary>
        PlayerTag Tag { get; }

        /// <summary>Starts a session; per-seed choices come from <paramref name="setup"/>, play draws from <paramref name="play"/>.</summary>
        void StartSession(GameDefinition definition, Rng setup, Rng play);

        /// <summary>Picks the action for the next exchange. <paramref name="history"/> holds every earlier exchange.</summary>
        int Act(ExchangeHistory history);

        /// <summary>Called after the exchange is recorded in <paramref name="history"/>.</summary>
        void Observe(ExchangeHistory history);
    }
}
