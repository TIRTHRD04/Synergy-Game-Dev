using System;
using System.Diagnostics;
using AdaptiveOpponent.Core;

namespace AdaptiveOpponent.Sim
{
    /// <summary>How a simulated session is played.</summary>
    public enum SessionMode
    {
        /// <summary>Fixed length: 3 matches of 100 exchanges; HP never ends a match.</summary>
        Harness,
        /// <summary>Game rules: a match ends at HP 0 or after 120 exchanges.</summary>
        GameRules,
    }

    /// <summary>Plays one session between an Opponent and a scripted player and records every exchange.</summary>
    public static class SessionRunner
    {
        /// <summary>Matches per session.</summary>
        public const int Matches = 3;

        /// <summary>Exchanges per match in harness mode.</summary>
        public const int HarnessMatchLength = 100;

        /// <summary>The exchange cap of a match under game rules.</summary>
        public const int ExchangeCap = 120;

        /// <summary>
        /// Plays one session. <paramref name="makeOpponent"/> receives the Opponent's random stream, derived from
        /// <paramref name="sessionSeed"/>, so paired runs of two configs on the same seed face the same player draws.
        /// </summary>
        public static SessionRecord Run(GameDefinition definition, Func<Rng, IOpponent> makeOpponent, IPlayer player,
            ulong sessionSeed, SessionMode mode, int startHp)
        {
            int capacity = Matches * (mode == SessionMode.Harness ? HarnessMatchLength : ExchangeCap);
            var history = new ExchangeHistory(capacity, definition.ActionCount, definition.ResponseCount);
            var record = new SessionRecord(capacity, player.Name);
            IOpponent opponent = makeOpponent(Rng.ForStream(sessionSeed, RngStream.Opponent));
            player.StartSession(definition, Rng.ForStream(sessionSeed, RngStream.PlayerSetup),
                Rng.ForStream(sessionSeed, RngStream.Player));
            var oracle = player as IOracle;
            var watch = new Stopwatch();
            for (int match = 0; match < Matches; match++)
            {
                history.StartMatch();
                opponent.StartMatch();
                int opponentHp = startHp, playerHp = startHp;
                for (int e = 0; ; e++)
                {
                    if (mode == SessionMode.Harness ? e == HarnessMatchLength
                        : e == ExchangeCap || opponentHp <= 0 || playerHp <= 0)
                    {
                        break;
                    }
                    watch.Restart();
                    int response = opponent.Decide();
                    watch.Stop();
                    long decideTicks = watch.ElapsedTicks;
                    int top = opponent.TopPrediction;
                    bool gate = opponent.GateOpen;
                    double lambda = opponent.Lambda;
                    double oracleP = oracle != null ? oracle.OracleProbability(history) : double.NaN;
                    int action = player.Act(history);
                    double pAction = opponent.ForecastOf(action);
                    int payoff = definition.Payoff(response, action);
                    history.Add(action, response, payoff);
                    watch.Restart();
                    opponent.Observe(action);
                    watch.Stop();
                    player.Observe(history);
                    if (payoff > 0)
                    {
                        playerHp -= payoff;
                    }
                    else
                    {
                        opponentHp += payoff;
                    }
                    record.Add(new ExchangeRecord
                    {
                        Match = match,
                        IndexInMatch = e,
                        Action = action,
                        Response = response,
                        Payoff = payoff,
                        TopPrediction = top,
                        ForecastOfAction = pAction,
                        GateOpen = gate,
                        Lambda = lambda,
                        Tag = player.Tag,
                        Commentary = opponent.Commentary,
                        OracleProbability = oracleP,
                        DecisionTicks = decideTicks + watch.ElapsedTicks,
                    });
                }
                record.EndMatch(opponentHp, playerHp);
            }
            return record;
        }
    }

    /// <summary>An archetype that can say how well a perfect predictor of it could do on the next exchange.</summary>
    public interface IOracle
    {
        /// <summary>The probability of the archetype's single most likely next action, given its state.</summary>
        double OracleProbability(ExchangeHistory history);
    }
}
