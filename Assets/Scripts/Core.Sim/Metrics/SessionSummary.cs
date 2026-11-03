using System;
using AdaptiveOpponent.Core;

namespace AdaptiveOpponent.Sim
{
    /// <summary>The metrics of one simulated session (ADR 0004, notes/decisions.md "Archetypes and harness").</summary>
    public sealed class SessionSummary
    {
        /// <summary>First exchange index (0-based, session order) of the late phase in harness mode.</summary>
        public const int LateStart = 200;

        /// <summary>Exchanges in the early phase (exchanges 1-50).</summary>
        public const int EarlyLength = 50;

        /// <summary>Number of replay positions tracked (Opening and two Follow-ups).</summary>
        public const int ReplayPositions = 3;

        /// <summary>Summarises a session record.</summary>
        public SessionSummary(SessionRecord record, int actionCount)
        {
            PlayerName = record.PlayerName;
            var x = record.Exchanges;
            Exchanges = x.Count;
            Correct = new bool[x.Count];
            int correct = 0, early = 0, earlyN = 0, late = 0, lateN = 0, wins = 0;
            double logLoss = 0, payoff = 0, oracle = 0;
            int oracleN = 0;
            GateOpenIndex = -1;
            ReplayHits = new int[ReplayPositions];
            ReplayCounts = new int[ReplayPositions];
            for (int i = 0; i < x.Count; i++)
            {
                ExchangeRecord e = x[i];
                bool hit = e.TopPrediction == e.Action;
                Correct[i] = hit;
                correct += hit ? 1 : 0;
                if (i < EarlyLength)
                {
                    early += hit ? 1 : 0;
                    earlyN++;
                }
                if (i >= LateStart)
                {
                    late += hit ? 1 : 0;
                    lateN++;
                }
                double nll = -Math.Log(Math.Max(e.ForecastOfAction, 1e-300));
                logLoss += nll;
                payoff += e.Payoff;
                wins += e.Payoff > 0 ? 1 : 0;
                if (e.GateOpen && GateOpenIndex < 0)
                {
                    GateOpenIndex = i;
                }
                LambdaNonZero += e.Lambda > 0 ? 1 : 0;
                Insights += e.Commentary == CommentaryKind.Insight ? 1 : 0;
                Recalls += e.Commentary == CommentaryKind.Recall ? 1 : 0;
                if (!double.IsNaN(e.OracleProbability))
                {
                    oracle += e.OracleProbability;
                    oracleN++;
                }
                int position = ReplayIndex(e.Tag);
                if (position >= 0)
                {
                    ReplayCounts[position]++;
                    ReplayHits[position] += hit ? 1 : 0;
                }
                if (e.Tag == PlayerTag.BurnedContext)
                {
                    BurnedLogLossSum += nll;
                    BurnedCount++;
                }
            }
            Accuracy = Ratio(correct, x.Count);
            EarlyAccuracy = Ratio(early, earlyN);
            LateAccuracy = Ratio(late, lateN);
            LogLoss = x.Count > 0 ? logLoss / x.Count : double.NaN;
            BitsGainedPerExchange = (Math.Log(actionCount) - LogLoss) / Math.Log(2);
            MeanPayoff = x.Count > 0 ? payoff / x.Count : double.NaN;
            WinRate = Ratio(wins, x.Count);
            Oracle = oracleN > 0 ? oracle / oracleN : double.NaN;
            for (int m = 0; m < record.OpponentHpAtEnd.Count; m++)
            {
                int mine = record.OpponentHpAtEnd[m], theirs = record.PlayerHpAtEnd[m];
                bool won = theirs <= 0 || (mine > 0 && mine > theirs);
                bool lost = mine <= 0 || (theirs > 0 && theirs > mine);
                MatchesWon += won && !lost ? 1 : 0;
                MatchesLost += lost && !won ? 1 : 0;
            }
        }

        /// <summary>Matches the Opponent won (the player's HP hit 0, or the Opponent had more HP at the cap).</summary>
        public int MatchesWon { get; }

        /// <summary>Matches the Opponent lost.</summary>
        public int MatchesLost { get; }

        /// <summary>The archetype that played.</summary>
        public string PlayerName { get; }

        /// <summary>Exchanges in the session.</summary>
        public int Exchanges { get; }

        /// <summary>Whether the top prediction was right, per exchange.</summary>
        public bool[] Correct { get; }

        /// <summary>Prediction accuracy over the whole session.</summary>
        public double Accuracy { get; }

        /// <summary>Prediction accuracy over exchanges 1-50.</summary>
        public double EarlyAccuracy { get; }

        /// <summary>Prediction accuracy over exchanges 201-300 (NaN if the session is shorter).</summary>
        public double LateAccuracy { get; }

        /// <summary>Mean negative log-likelihood of the actual action, in nats.</summary>
        public double LogLoss { get; }

        /// <summary>How many bits per exchange the forecast beats a uniform forecast by.</summary>
        public double BitsGainedPerExchange { get; }

        /// <summary>The Opponent's mean payoff per exchange (net damage).</summary>
        public double MeanPayoff { get; }

        /// <summary>The fraction of exchanges the Opponent won.</summary>
        public double WinRate { get; }

        /// <summary>The first exchange (0-based) on which the gate was open, or -1.</summary>
        public int GateOpenIndex { get; }

        /// <summary>Exchanges whose committed strategy put weight on the best response.</summary>
        public int LambdaNonZero { get; }

        /// <summary>Exchanges after which an Insight was shown.</summary>
        public int Insights { get; }

        /// <summary>Exchanges after which a Recall was shown.</summary>
        public int Recalls { get; }

        /// <summary>The archetype's mean oracle probability (NaN when it has none).</summary>
        public double Oracle { get; }

        /// <summary>Correct top predictions on replay exchanges, by position.</summary>
        public int[] ReplayHits { get; }

        /// <summary>Replay exchanges, by position.</summary>
        public int[] ReplayCounts { get; }

        /// <summary>Top-prediction accuracy over every replay exchange of this session (NaN when there were none).</summary>
        public double ReplayAccuracy
        {
            get
            {
                int hits = 0, n = 0;
                for (int p = 0; p < ReplayPositions; p++)
                {
                    hits += ReplayHits[p];
                    n += ReplayCounts[p];
                }
                return Ratio(hits, n);
            }
        }

        /// <summary>Mean log-loss over exchanges in burned contexts (NaN when there were none).</summary>
        public double BurnedLogLoss => BurnedCount > 0 ? BurnedLogLossSum / BurnedCount : double.NaN;

        /// <summary>Summed log-loss over exchanges in burned contexts.</summary>
        public double BurnedLogLossSum { get; }

        /// <summary>Exchanges in burned contexts.</summary>
        public int BurnedCount { get; }

        private static int ReplayIndex(PlayerTag tag)
        {
            switch (tag)
            {
                case PlayerTag.ReplayOpening: return 0;
                case PlayerTag.ReplayFollowUp1: return 1;
                case PlayerTag.ReplayFollowUp2: return 2;
                default: return -1;
            }
        }

        private static double Ratio(int hits, int n) => n > 0 ? (double)hits / n : double.NaN;
    }
}
