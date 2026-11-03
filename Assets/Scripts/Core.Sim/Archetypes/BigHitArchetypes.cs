using System;
using System.Collections.Generic;
using AdaptiveOpponent.Core;

namespace AdaptiveOpponent.Sim
{
    /// <summary>
    /// Big-hit repeater: plays uniformly at random, except when the previous action equals the a0 of a big hit it
    /// landed earlier and at least 30 exchanges have passed since it last used that big hit; then it replays that
    /// Big-hit sequence. A replay is cut short at a match boundary.
    /// </summary>
    public sealed class BigHitRepeater : ArchetypeBase
    {
        /// <summary>Exchanges that must pass before the same big hit is replayed again.</summary>
        public const int Gap = 30;

        private readonly List<Entry> entries = new List<Entry>();
        private BigHitDetector detector;
        private int replaying = -1;
        private int replayPosition;

        /// <inheritdoc />
        public override string Name => "BigHitRepeater";

        /// <inheritdoc />
        protected override void Setup(Rng setup)
        {
            entries.Clear();
            detector = new BigHitDetector(Definition);
            replaying = -1;
        }

        /// <inheritdoc />
        public override int Act(ExchangeHistory history)
        {
            int now = history.Count;
            if (replaying >= 0 && history.IsMatchStart(now))
            {
                replaying = -1;
            }
            if (replaying < 0)
            {
                int prevAction = history.PrevActionBefore(now);
                for (int i = 0; i < entries.Count; i++)
                {
                    if (entries[i].PrevAction == prevAction && now - entries[i].LastUsed >= Gap)
                    {
                        replaying = i;
                        replayPosition = 0;
                        break;
                    }
                }
            }
            if (replaying < 0)
            {
                Tag = PlayerTag.None;
                return RandomAction();
            }
            Entry entry = entries[replaying];
            entry.LastUsed = now;
            entries[replaying] = entry;
            int action = entry.Sequence[replayPosition];
            Tag = replayPosition == 0 ? PlayerTag.ReplayOpening
                : replayPosition == 1 ? PlayerTag.ReplayFollowUp1 : PlayerTag.ReplayFollowUp2;
            replayPosition++;
            if (replayPosition == entry.Sequence.Length)
            {
                replaying = -1;
            }
            return action;
        }

        /// <inheritdoc />
        public override void Observe(ExchangeHistory history)
        {
            if (!detector.Observe(history, out BigHit hit) || hit.Side != BigHitSide.OnOpponent)
            {
                return;
            }
            var sequence = new int[hit.EndIndex - hit.StartIndex + 1];
            for (int i = 0; i < sequence.Length; i++)
            {
                sequence[i] = history.Action(hit.StartIndex + i);
            }
            entries.Add(new Entry { PrevAction = hit.StartPrevAction, Sequence = sequence, LastUsed = hit.EndIndex });
        }

        private struct Entry
        {
            public int PrevAction;
            public int[] Sequence;
            public int LastUsed;
        }
    }

    /// <summary>
    /// Burned avoider: spams one per-seed action with 40% uniform noise. In a start context where it took a big hit,
    /// it avoids that big hit's Opening with probability 0.5^(dt/100), dt = exchanges since the hit; when only the
    /// previous action matches (broad), with half that. Avoiding redraws uniformly from the other actions.
    /// </summary>
    public sealed class BurnedAvoider : ArchetypeBase
    {
        private const double Noise = 0.4;
        private const double HalfLife = 100.0;
        private readonly List<Burn> burns = new List<Burn>();
        private BigHitDetector detector;
        private int favourite;

        /// <inheritdoc />
        public override string Name => "BurnedAvoider";

        /// <inheritdoc />
        protected override void Setup(Rng setup)
        {
            favourite = setup.NextInt(Definition.ActionCount);
            burns.Clear();
            detector = new BigHitDetector(Definition);
        }

        /// <inheritdoc />
        public override int Act(ExchangeHistory history)
        {
            int now = history.Count;
            int prevAction = history.PrevActionBefore(now);
            int prevResponse = history.PrevResponseBefore(now);
            int action = Chance(Noise) ? RandomAction() : favourite;
            Tag = PlayerTag.None;
            for (int i = 0; i < burns.Count; i++)
            {
                Burn burn = burns[i];
                if (burn.PrevAction != prevAction)
                {
                    continue;
                }
                Tag = PlayerTag.BurnedContext;
                if (burn.Opening != action)
                {
                    continue;
                }
                double p = Math.Pow(0.5, (now - burn.Index) / HalfLife);
                if (burn.PrevResponse != prevResponse)
                {
                    p *= 0.5;
                }
                if (Chance(p))
                {
                    action = OtherThan(action);
                }
            }
            return action;
        }

        /// <inheritdoc />
        public override void Observe(ExchangeHistory history)
        {
            if (detector.Observe(history, out BigHit hit) && hit.Side == BigHitSide.OnPlayer)
            {
                burns.Add(new Burn
                {
                    PrevAction = hit.StartPrevAction,
                    PrevResponse = hit.StartPrevResponse,
                    Opening = history.Action(hit.StartIndex),
                    Index = hit.EndIndex,
                });
            }
        }

        private int OtherThan(int action)
        {
            int pick = Play.NextInt(Definition.ActionCount - 1);
            return pick >= action ? pick + 1 : pick;
        }

        private struct Burn
        {
            public int PrevAction;
            public int PrevResponse;
            public int Opening;
            public int Index;
        }
    }
}
