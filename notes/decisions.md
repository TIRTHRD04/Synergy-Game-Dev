# Decisions

One dated line per decision, final version only. When a decision changes, its line is rewritten in place, not appended. The reasons behind the larger decisions are in docs/adr/.

## Project and tooling
- 2026-10-27: Unity 6 (6000.x LTS), bare-bones test screen built from script (IMGUI), no scene/prefab files, no UI packages.
- 2026-10-27: Decision budget 1 ms per Decide+Observe, p50/p99 measured with Stopwatch in the sim. No deadline or freeze date in CLAUDE.md.
- 2026-10-27: Neural networks, Q-learning, MCTS, epsilon-greedy and UCB1 are explained in docs/CONTEXT.md, not built.
- 2026-10-27: No handoff note. .NET 8 is installed in the cloud container only to run core tests and the sim.
- 2026-10-27: Work is pushed to branch claude/vigilant-ramanujan-3t7kvy.

## Game rules
- 2026-10-27: Theme-agnostic: actions, responses, outcome matrix and authored counters are data (configs/definition.json). 6 actions, 6 responses, balanced matrix: each response beats 2 actions, draws with 2, loses to 2.
- 2026-10-27: Nash without an LP: the loader checks every row and column of the outcome matrix sums to 0, so sigma* = uniform and v* = 0; a unit test checks uniform scores exactly 0 against every column. An LP comes back, with an ADR, only if an unbalanced theme needs it.
- 2026-10-27: The Opponent commits its response before it reads the player's action.
- 2026-10-27: Match ends at HP 0 or a 120-exchange cap; a lost exchange costs 1 HP; start HP about 30, tuned on dev seeds.
- 2026-10-27: No saved profiles, no leaderboard, no storage interface. Memory lasts for one session (3 matches), in memory only. The corrupt-profile rule in CLAUDE.md stays, dormant.
- 2026-10-27: Prediction is history-only; no state snapshot (ADR 0003 deferred).

## Archetypes and harness
- 2026-10-27: Five archetypes, built before the Opponent: Spammer, Cycler, Win-stay/lose-shift, Out-guesser, Random (negative control).
- 2026-10-27: Win-stay/lose-shift: a win repeats the action; a draw or loss shifts to one of the two actions that beat the Opponent's last response (seeded Rng picks). Its oracle ceiling is logged next to its prediction accuracy.
- 2026-10-27: Out-guesser: faded counts (0.9 per exchange) of the Opponent's responses, predicts the most likely response, plays one of the two actions that beat it, 70/30 with a per-seed preference, plus 10% uniform noise. Its oracle ceiling (about 0.65) is logged.
- 2026-10-27: Heldout set = 10 seeds. Pass criteria apply to the mean across seeds; per-seed rows are logged. Early = exchanges 1-50 of the session, late = last 100.
- 2026-10-27: Pass: late prediction accuracy >= chance + 0.20 for Spammer, Cycler and Win-stay/lose-shift. Random: mean prediction accuracy in 1/6 +/- 0.05, 0 claims summed over all seeds, and lambda = 0 on every exchange; its mean payoff within v* +/- 0.05 is kept as a simulator check, not an AI check.
- 2026-10-27: Habit-switch unit test: a sequence switches from spamming action 1 to spamming action 4; recovered = top prediction is action 4 for 5 exchanges in a row, within 30 exchanges of the switch. Fading tuning on dev must keep it passing.
- 2026-10-27: All tuning (HP, gate threshold, Insight limits, fading, alpha, k0) is on dev seeds. The gate threshold is fixed with Random on >= 100 dev seeds before any heldout run and never changed after seeing heldout results.
- 2026-10-27: One heldout run at the end reports v1, freq and v2 (whatever survived the ablations) together.

## Opponent v1
- 2026-10-27: Experts: order 0, order 1, order 2 and (previous action, previous response), each a table of counts with a symmetric Dirichlet prior. A freq config (order 0 alone) runs beside the full config on the same seeds (ADR 0004).
- 2026-10-27: Fading is per visit: a context's counts fade by gamma (start 0.98) once each time that context is visited. Order 0 is visited every exchange. Global-time fading is one dev ablation row.
- 2026-10-27: Experts are mixed by Fixed Share (alpha start 0.01): w_i <- w_i p_i(a), normalise, w_i <- (1 - alpha) w_i + alpha / K.
- 2026-10-27: At match start: counts x 0.5; expert weights become 0.5*w + 0.5*(1/K).
- 2026-10-27: Evidence gate: one test on the mixed forecast's cumulative log-likelihood ratio vs uniform over the session, starting threshold ln(1000). It latches open for the rest of the session. No hedged gate.
- 2026-10-27: An Insight needs the gate open, top probability >= 0.4 and >= 5 faded counts in its context (both tuned on dev).
- 2026-10-27: Response selection: while the gate is closed, play sigma* (uniform). Once open, Thompson Sampling: pick an expert with probability equal to its weight, sample from its Dirichlet posterior for the current context, play the best response to the sample. Test mode uses the mixture mean. Ties go to the authored counter, then the lower index.

## Opponent v2 (ADR 0007, spec .scratch/opponent-v2/spec.md)
- 2026-10-27: v2 components are added one at a time after v1 and kept only if they earn their place on paired dev seeds.
- 2026-10-27: CTW runs as two experts: action-first context a(t-1), r(t-1), ... and response-first r(t-1), a(t-1), ..., depth 6. v1's order-1 expert stays in the mix as a hedge.
- 2026-10-27: CTW nodes store log Pw and the log product of the children's Pw, updated by one factor when the path child changes. Each node's beta log-odds fades by gamma and is clamped to [-6, 6]; unit test: 50 symbols favouring the node, then 50 favouring the child, beta crosses 0.5 within 15 symbols of the flip.
- 2026-10-27: Selector: while the gate is closed, play sigma*; once open, play lambda*BR + (1-lambda)*sigma* on the mean mixed forecast. Budget k starts at k0 = 0.25 (tuned on dev in [0.1, 1]) and grows by expected gifts, sum_r sigma_t(r) U(r, a_t) - v*. ADR 0006 is superseded only if this ablation wins.
- 2026-10-27: Comparisons use paired seeds with the SE of the paired difference reported. Non-inferiority margin = max(0.02, 2 x paired SE). Strict wins (Win-stay/lose-shift win rate, Out-guesser payoff) must exceed v1 by more than 2 paired SE.
- 2026-10-27: Ablation rows (dev only, one log.csv row each, greedy in this order): v1, v1 without Fixed Share, +CTW x2, +longest-match, +level-k/mirror, +safe exploitation, optional +CTW at 3 gammas, global-time fading. A row beats the row above if it improves at least one archetype by more than 2 paired SE and is not worse on any by more than the margin. Predictor rows are judged on prediction accuracy, selector rows on payoff.
- 2026-10-27: Level-k/mirror experts have their own keep rule: (a) not worse on Spammer, Cycler, Win-stay/lose-shift and Random by more than the margin in prediction accuracy, (b) >= 0.3 higher accuracy than without them on the scripted level-1 and mirror-1 players, (c) not worse against Out-guesser by more than the margin, on payoff.

## Unity screen
- 2026-10-27: No timer per exchange. One exchange per key-down; the next needs a key-up first, plus a 0.4 s lockout during the reveal.
- 2026-10-27: Commitment: the Game layer (not Core) shows the first 8 hex chars of SHA-256("response|nonce") while locked in, and the nonce after the reveal. The nonce comes from a separate Rng so the Opponent's seeded stream is unchanged.
- 2026-10-27: Manual play log: the Game layer writes one timestamped CSV per session to Application.persistentDataPath. Header: schema version, config file name, seed, AI version (v1/v2). Rows per exchange: session, match and exchange index, action, response, outcome, predicted top action, forecast probability of the actual action, leading expert, gate open, lambda, reaction time ms (data only, never used by the AI). Rows are buffered and written at match end and on quit; all file I/O is in try/catch, and a failure shows one on-screen warning while play continues. The AI never reads these files; they never go in the repo; docs/playtest.md explains how to copy one out.
