using System.Collections.Generic;
using AdaptiveOpponent.Core;
using AdaptiveOpponent.UI;

namespace AdaptiveOpponent.Game
{
    /// <summary>Where a session is in its flow.</summary>
    public enum DuelPhase
    {
        /// <summary>The Opponent has committed; waiting for the player's action.</summary>
        Choosing,
        /// <summary>Both choices are revealed; the next exchange starts after the lockout.</summary>
        Revealing,
        /// <summary>All three matches are over; the summary is shown.</summary>
        Summary,
    }

    /// <summary>
    /// The match flow of one session (3 matches, HP, 120-exchange cap) around the Opponent. Pure flow: every learning,
    /// prediction and decision happens inside <see cref="Opponent"/>. The view models are rebuilt only when an exchange
    /// changes, never per frame.
    /// </summary>
    public sealed class DuelSession
    {
        /// <summary>Matches per session.</summary>
        public const int Matches = 3;

        /// <summary>The exchange cap of a match.</summary>
        public const int ExchangeCap = 120;

        private readonly GameDefinition definition;
        private readonly Opponent opponent;
        private readonly BigHitDetector detector;
        private readonly PlayLog log;
        private readonly List<bool> correct = new List<bool>();
        private int committed;
        private Commitment commitment;
        private int exchangeInMatch;
        private int gateOpenedAt = -1;
        private int damageDealt;
        private int damageTaken;

        /// <summary>Starts a session and commits the first exchange.</summary>
        public DuelSession(GameDefinition definition, OpponentConfig config, string configFile, ulong seed, int startHp)
        {
            this.definition = definition;
            StartHp = startHp;
            Seed = seed;
            opponent = new Opponent(definition, config, Rng.ForStream(seed, RngStream.Opponent), Matches * ExchangeCap);
            detector = new BigHitDetector(definition);
            log = new PlayLog(configFile, seed, config.Name);
            Belief = new BeliefView(definition);
            Reveal = new RevealView();
            Label = config.Name;
            StartMatch();
        }

        /// <summary>The current phase.</summary>
        public DuelPhase Phase { get; private set; }

        /// <summary>The config's name, shown on screen.</summary>
        public string Label { get; }

        /// <summary>The session seed (also in the play log header).</summary>
        public ulong Seed { get; }

        /// <summary>HP each side starts every match with.</summary>
        public int StartHp { get; }

        /// <summary>The current match (0-based).</summary>
        public int Match { get; private set; }

        /// <summary>The exchange number within the match (1-based) of the committed exchange.</summary>
        public int ExchangeNumber => exchangeInMatch + 1;

        /// <summary>The Opponent's HP.</summary>
        public int OpponentHp { get; private set; }

        /// <summary>The player's HP.</summary>
        public int PlayerHp { get; private set; }

        /// <summary>Matches the player won and lost so far.</summary>
        public int PlayerMatchWins { get; private set; }

        /// <summary>Matches the Opponent won so far.</summary>
        public int OpponentMatchWins { get; private set; }

        /// <summary>The first 16 hex characters of the current commitment.</summary>
        public string CommitmentShort => commitment.ShortHash;

        /// <summary>The Belief and Commentary for the committed exchange.</summary>
        public BeliefView Belief { get; }

        /// <summary>The last revealed exchange.</summary>
        public RevealView Reveal { get; }

        /// <summary>The summary, built when the session ends.</summary>
        public SummaryView Summary { get; private set; }

        /// <summary>A play-log write failure to show once, or null.</summary>
        public string LogWarning => log.Warning;

        /// <summary>Where the play log is written.</summary>
        public string LogPath => log.FilePath;

        /// <summary>Plays the player's action for the committed exchange. Ignored outside the Choosing phase.</summary>
        public void SubmitAction(int action, long reactionMs)
        {
            if (Phase != DuelPhase.Choosing || action < 0 || action >= definition.ActionCount)
            {
                return;
            }
            int top = opponent.TopPrediction;
            double pAction = opponent.ForecastOf(action);
            bool gate = opponent.GateOpen;
            double lambda = opponent.Lambda;
            string leading = opponent.LeadingExpert >= 0 ? opponent.ExpertName(opponent.LeadingExpert) : "none";
            opponent.Observe(action);
            bool bigHit = detector.Observe(opponent.History, out BigHit hit);
            int payoff = definition.Payoff(committed, action);
            int damage = payoff < 0 ? -payoff : payoff;
            if (payoff > 0)
            {
                PlayerHp -= damage;
                damageDealt += damage;
            }
            else if (payoff < 0)
            {
                OpponentHp -= damage;
                damageTaken += damage;
            }
            correct.Add(top == action);
            if (gate && gateOpenedAt < 0)
            {
                gateOpenedAt = correct.Count;
            }
            Outcome outcome = definition.OutcomeOf(committed, action);
            log.Add(Match + 1, ExchangeNumber, action, committed, outcome.ToString(), damage, bigHit, top, pAction,
                leading, gate, lambda, reactionMs, commitment.Hash, commitment.Nonce);
            Reveal.Set(definition, action, committed, outcome, damage, bigHit, bigHit ? hit.Side : BigHitSide.OnOpponent,
                commitment.Nonce);
            exchangeInMatch++;
            Phase = DuelPhase.Revealing;
        }

        /// <summary>After the reveal lockout: commits the next exchange, starts the next match, or ends the session.</summary>
        public void Continue()
        {
            if (Phase != DuelPhase.Revealing)
            {
                return;
            }
            if (OpponentHp > 0 && PlayerHp > 0 && exchangeInMatch < ExchangeCap)
            {
                Commit();
                return;
            }
            if (PlayerHp <= 0 || (OpponentHp > 0 && OpponentHp > PlayerHp))
            {
                OpponentMatchWins++;
            }
            else if (OpponentHp <= 0 || PlayerHp > OpponentHp)
            {
                PlayerMatchWins++;
            }
            log.Flush();
            if (Match + 1 < Matches)
            {
                Match++;
                StartMatch();
                return;
            }
            Summary = new SummaryView(correct, damageDealt, damageTaken, OpponentMatchWins, PlayerMatchWins,
                gateOpenedAt, opponent.BigHits.RecordedOnOpponent, opponent.BigHits.RecordedOnPlayer);
            Phase = DuelPhase.Summary;
        }

        /// <summary>Writes any buffered play-log rows (on quit).</summary>
        public void FlushLog() => log.Flush();

        private void StartMatch()
        {
            OpponentHp = StartHp;
            PlayerHp = StartHp;
            exchangeInMatch = 0;
            opponent.StartMatch();
            Commit();
        }

        private void Commit()
        {
            committed = opponent.Decide();
            commitment = Commitment.For(committed);
            Belief.Update(opponent);
            Phase = DuelPhase.Choosing;
        }
    }
}
