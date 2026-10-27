# Decisions

One dated line per decision made outside Claude or during design sessions.

- 2026-10-27: Unity 6 (6000.x LTS), bare-bones test screen built from script (IMGUI), no scene/prefab files, no UI packages.
- 2026-10-27: Theme-agnostic: actions, responses, outcome matrix and authored counters are data (configs/definition.json); 6 actions, 6 responses, balanced matrix (each response beats 2, draws 2, loses to 2).
- 2026-10-27: The Opponent commits its response before it reads the player's action.
- 2026-10-27: No saved profiles, no leaderboard, no storage interface for now. Learning lasts for one session (3 matches), in memory only. Corrupt-profile rule in CLAUDE.md stays, dormant.
- 2026-10-27: Match ends at HP 0 or a 120-exchange cap; a lost exchange costs 1 HP; start HP about 30, tuned later on dev seeds.
- 2026-10-27: Four archetypes (Spammer, Cycler, Win-stay/lose-shift, Random) are built before the Opponent. Random is the negative control.
- 2026-10-27: Predictor: N-gram experts with Dirichlet priors and fading counts, combined by Bayesian model averaging; compared against frequency counts (ADR 0004). Selector: Thompson Sampling over predicted actions with fixed payoffs (ADR 0006). Epsilon-greedy, UCB1 and Q-learning are documented, not built.
- 2026-10-27: Prediction is history-only; no state snapshot (ADR 0003 deferred).
- 2026-10-27: Decision budget 1 ms per Decide+Observe. No deadline or freeze date in CLAUDE.md.
- 2026-10-27: No handoff note. .NET 8 is installed in the cloud container only to run core tests and the sim.
- 2026-10-27: Work is pushed to branch claude/vigilant-ramanujan-3t7kvy.
- 2026-10-27: Win-stay/lose-shift archetype: win repeats; draw or loss shifts to one of the two actions that beat the Opponent's last response (seeded Rng picks). Its oracle ceiling (a predictor that knows its rule) is logged next to its prediction accuracy.
- 2026-10-27: The fourth expert's context is (previous action, previous response), not (previous action, previous outcome).
- 2026-10-27: Heldout set = 10 seeds. Pass criteria apply to the mean across seeds; per-seed rows are logged. Early = exchanges 1-50 of the session, late = last 100. Random passes with mean prediction accuracy in 1/6 +/- 0.05 and 0 claims summed over all seeds.
- 2026-10-27: Evidence gate: one test on the averaged predictor's cumulative log-likelihood ratio vs uniform, over the session, starting threshold ln(1000). The threshold is fixed on dev seeds (Random on >= 100 dev seeds, false-claim rate checked) before the first heldout run and never changed after seeing heldout results.
- 2026-10-27: An Insight also needs a current, strong prediction: top probability >= 0.4 and >= 5 faded counts in its context (both tuned on dev). Gate open alone is not enough.
- 2026-10-27: Fading 0.98 per exchange. At match start: counts x 0.5; expert weights become 0.5*w + 0.5*(1/K), not w x 0.5 (a no-op after normalising).
- 2026-10-27: Model averaging uses Fixed Share (rate ~0.01) so an expert's weight can recover.
- 2026-10-27: Habit-change unit test: a sequence switches from spamming action 1 to spamming action 4; recovered = top prediction is action 4 for 5 exchanges in a row, within 30 exchanges of the switch. Fading tuning on dev must keep this test passing.
- 2026-10-27: Unity screen: no timer per exchange. One exchange per key-down; the next needs a key-up first, plus a 0.4 s lockout during the reveal.
- 2026-10-27: The round-2 design is v1. A v2 (CTW predictor, longest-match expert, level-k meta-strategies, safe exploitation mixed with the Nash strategy) is proposed in ADR 0007 and replaces v1 only if it beats v1 on dev seeds.
