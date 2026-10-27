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
- 2026-10-27: The team lead's v2 brief (.scratch/opponent-v2/spec.md) is the change to the plan; v2 components are added one at a time after v1 and kept only if they earn their place.
- 2026-10-27: CTW runs as two experts: action-first context a(t-1), r(t-1), ... and response-first r(t-1), a(t-1), ...
- 2026-10-27: CTW nodes store log Pw and the log product of children's Pw, updated by one factor when the path child changes. Each node's beta log-odds L fades by gamma and is clamped to [-6, 6]; unit test: 50 symbols favouring the node, then 50 favouring the child, beta crosses 0.5 within 15 symbols of the flip.
- 2026-10-27: Fading is per visit: a context's counts fade by gamma once each time that context is visited, not by global time. Applies to v1's order-1/2 and (action, response) experts and to CTW nodes (order 0 is unchanged). Global-time fading is one dev ablation row. This replaces "fading 0.98 per exchange" for every expert except order 0.
- 2026-10-27: Nash: keep the balanced outcome matrix, no LP. Load checks every row and column sums to 0; sigma* = uniform, v* = 0. Add an LP (with an ADR) only if an unbalanced theme ever needs it.
- 2026-10-27: Gate closed means play sigma* (uniform) in both v1 and v2. v1 plays Thompson Sampling once the gate is open; v2 plays lambda*BR + (1-lambda)*sigma* on the mean mixed forecast. ADR 0006 is superseded only if that ablation wins.
- 2026-10-27: Safe-exploitation budget uses expected gifts: k += sum_r sigma_t(r) U(r, a_t) - v*. k0 starts at 0.25, tuned on dev in [0.1, 1].
- 2026-10-27: Fifth archetype, Out-guesser: faded counts (0.9 per exchange) of the Opponent's responses, predicts the most likely response, plays one of the two actions that beat it, 70/30 with a per-seed preference, plus 10% uniform noise. Its oracle ceiling (about 0.65) is logged. It joins dev and heldout suites. Replaces "four archetypes".
- 2026-10-27: Random's payoff check (mean within v* +/- 0.05) tests the simulator, not the AI; also check lambda = 0 on every exchange against Random.
- 2026-10-27: v1 vs v2 (and every ablation) is compared on paired seeds with the SE of the paired difference reported. Non-inferiority margin = max(0.02, 2 x paired SE). Strict wins (WSLS win rate, Out-guesser payoff) must exceed v1 by more than 2 paired SE.
- 2026-10-27: Ablation rows (dev only, one log.csv row each, greedy in this order): v1, v1 without Fixed Share, +CTW x2, +longest-match, +level-k/mirror, +safe exploitation, optional +CTW at 3 gammas, plus global-time fading. A row "beats the row above" if it improves at least one archetype by more than 2 paired SE and is not worse on any by more than the margin. Predictor rows are judged on prediction accuracy, selector rows on payoff.
- 2026-10-27: Commitment: the Game layer (not Core) shows the first 8 hex chars of SHA-256("response|nonce") while locked in, and the nonce after the reveal. The nonce comes from a separate Rng so the Opponent's seeded stream is unchanged.
- 2026-10-27: No hedged gate (the gate latches, so hedging would only help before the first opening).
- 2026-10-27: v1 Thompson Sampling: pick an expert with probability equal to its weight, sample from its Dirichlet posterior for the current context, play the best response to the sample. Test mode uses the mixture mean.
- 2026-10-27: One heldout run at the end reports v1, freq and v2 (whatever survived the ablations) together.
