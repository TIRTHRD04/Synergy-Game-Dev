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
