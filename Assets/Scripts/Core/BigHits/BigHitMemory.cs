namespace AdaptiveOpponent.Core
{
    /// <summary>How a big-hit entry matched the current context.</summary>
    public enum MatchType
    {
        /// <summary>No match.</summary>
        None,
        /// <summary>Previous action and previous response both match the Start context.</summary>
        Exact,
        /// <summary>Only the previous action matches (the Broad context).</summary>
        Broad,
    }

    /// <summary>
    /// A preallocated pool of big-hit entries (ADR 0011). A new big hit with the same Start context and the same
    /// stored actions as an entry resets that entry's strengths to 1; a full pool replaces the entry with the lowest
    /// max(exact, broad) strength, oldest first. An entry is freed when its strengths are below
    /// <see cref="DeleteBelow"/> for every match type in use (only exact when broad matches are off).
    /// </summary>
    public sealed class BigHitPool
    {
        /// <summary>Entries per pool.</summary>
        public const int Size = 16;

        /// <summary>An entry is deleted when both strengths are below this.</summary>
        public const double DeleteBelow = 0.1;

        private readonly int length;
        private readonly bool broadInUse;
        private readonly int[] actions;
        private long nextOrder;

        /// <summary>Creates an empty pool whose entries store <paramref name="storedActions"/> actions each.</summary>
        public BigHitPool(int storedActions, bool broadMatches)
        {
            length = storedActions;
            broadInUse = broadMatches;
            actions = new int[Size * storedActions];
            Live = new bool[Size];
            PrevAction = new int[Size];
            PrevResponse = new int[Size];
            Exact = new double[Size];
            Broad = new double[Size];
            Order = new long[Size];
            ChainPosition = new int[Size];
            ChainType = new MatchType[Size];
            FiredType = new MatchType[Size];
            FiredAction = new int[Size];
            FiredPosition = new int[Size];
        }

        /// <summary>How many actions each entry stores (the Big-hit sequence, or just the Opening).</summary>
        public int StoredActions => length;

        /// <summary>Whether a slot holds an entry.</summary>
        public bool[] Live { get; }
        /// <summary>a0 of each entry's Start context.</summary>
        public int[] PrevAction { get; }
        /// <summary>r0 of each entry's Start context.</summary>
        public int[] PrevResponse { get; }
        /// <summary>Strength for exact matches.</summary>
        public double[] Exact { get; }
        /// <summary>Strength for broad matches.</summary>
        public double[] Broad { get; }
        /// <summary>Insertion order, for the oldest-first tie rule.</summary>
        public long[] Order { get; }
        /// <summary>Sequence expert: the next position of an active chain (0 = no chain).</summary>
        public int[] ChainPosition { get; }
        /// <summary>Sequence expert: the match type of an active chain.</summary>
        public MatchType[] ChainType { get; }
        /// <summary>The match type the entry fired with for the committed exchange (None if it did not fire).</summary>
        public MatchType[] FiredType { get; }
        /// <summary>The action the entry predicted (or, for avoidance, expects to be avoided).</summary>
        public int[] FiredAction { get; }
        /// <summary>The stored position the entry fired at.</summary>
        public int[] FiredPosition { get; }

        /// <summary>The stored action at <paramref name="position"/> of entry <paramref name="slot"/>.</summary>
        public int ActionAt(int slot, int position) => actions[slot * length + position];

        /// <summary>The strength of entry <paramref name="slot"/> for a match type.</summary>
        public double Strength(int slot, MatchType type) => type == MatchType.Exact ? Exact[slot] : Broad[slot];

        /// <summary>Sets one strength, then frees the entry if every strength in use fell below the limit.</summary>
        public void SetStrength(int slot, MatchType type, double value)
        {
            if (type == MatchType.Exact)
            {
                Exact[slot] = value;
            }
            else
            {
                Broad[slot] = value;
            }
            if (Exact[slot] < DeleteBelow && (!broadInUse || Broad[slot] < DeleteBelow))
            {
                Live[slot] = false;
                ChainPosition[slot] = 0;
            }
        }

        /// <summary>How entry <paramref name="slot"/> matches the context (previous action and response).</summary>
        public MatchType MatchOf(int slot, int prevAction, int prevResponse, bool broadAllowed)
        {
            if (!Live[slot] || PrevAction[slot] != prevAction)
            {
                return MatchType.None;
            }
            if (PrevResponse[slot] == prevResponse)
            {
                return MatchType.Exact;
            }
            return broadAllowed ? MatchType.Broad : MatchType.None;
        }

        /// <summary>Stores a big hit read from <paramref name="history"/>, merging with an equal entry.</summary>
        public void Add(ExchangeHistory history, BigHit hit)
        {
            for (int s = 0; s < Size; s++)
            {
                if (Live[s] && Same(s, history, hit))
                {
                    Exact[s] = 1;
                    Broad[s] = 1;
                    return;
                }
            }
            int slot = FreeOrWeakest();
            Live[slot] = true;
            PrevAction[slot] = hit.StartPrevAction;
            PrevResponse[slot] = hit.StartPrevResponse;
            for (int i = 0; i < length; i++)
            {
                actions[slot * length + i] = history.Action(hit.StartIndex + i);
            }
            Exact[slot] = 1;
            Broad[slot] = 1;
            Order[slot] = nextOrder++;
            ChainPosition[slot] = 0;
            FiredType[slot] = MatchType.None;
        }

        /// <summary>Ends every active chain (match start).</summary>
        public void EndChains()
        {
            for (int s = 0; s < Size; s++)
            {
                ChainPosition[s] = 0;
            }
        }

        private bool Same(int slot, ExchangeHistory history, BigHit hit)
        {
            if (PrevAction[slot] != hit.StartPrevAction || PrevResponse[slot] != hit.StartPrevResponse)
            {
                return false;
            }
            for (int i = 0; i < length; i++)
            {
                if (actions[slot * length + i] != history.Action(hit.StartIndex + i))
                {
                    return false;
                }
            }
            return true;
        }

        private int FreeOrWeakest()
        {
            int weakest = 0;
            for (int s = 0; s < Size; s++)
            {
                if (!Live[s])
                {
                    return s;
                }
                double strength = broadInUse ? System.Math.Max(Exact[s], Broad[s]) : Exact[s];
                double best = broadInUse ? System.Math.Max(Exact[weakest], Broad[weakest]) : Exact[weakest];
                if (strength < best || (strength == best && Order[s] < Order[weakest]))
                {
                    weakest = s;
                }
            }
            return weakest;
        }
    }

    /// <summary>The Opponent's two big-hit records: Big-hit memory (hits on it) and the Avoidance record (hits it landed).</summary>
    public sealed class BigHitMemory
    {
        /// <summary>Creates both pools for the definition's big-hit window.</summary>
        public BigHitMemory(GameDefinition definition, bool sequenceBroadMatches, bool avoidanceBroadMatches)
        {
            OnOpponent = new BigHitPool(definition.BigHitWindow, sequenceBroadMatches);
            OnPlayer = new BigHitPool(1, avoidanceBroadMatches);
        }

        /// <summary>Big-hit memory: big hits the player landed on the Opponent, with the whole Big-hit sequence.</summary>
        public BigHitPool OnOpponent { get; }

        /// <summary>The Avoidance record: big hits the Opponent landed on the player, with the Opening.</summary>
        public BigHitPool OnPlayer { get; }

        /// <summary>Total big hits recorded this session, per side.</summary>
        public int RecordedOnOpponent { get; private set; }

        /// <summary>Total big hits the Opponent landed this session.</summary>
        public int RecordedOnPlayer { get; private set; }

        /// <summary>Records a detected big hit in the pool for its side.</summary>
        public void Record(ExchangeHistory history, BigHit hit)
        {
            if (hit.Side == BigHitSide.OnOpponent)
            {
                OnOpponent.Add(history, hit);
                RecordedOnOpponent++;
            }
            else
            {
                OnPlayer.Add(history, hit);
                RecordedOnPlayer++;
            }
        }
    }
}
