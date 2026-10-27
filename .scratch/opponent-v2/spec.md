# Spec: Opponent v2 (maximum strength)

Status: grilled (2026-10-27); round 4 corrections recorded in notes/decisions.md override the text below where they differ. Source: brief from the team lead, pasted into the design session. Open points are listed at the end; the decisions that resolve them go to notes/decisions.md and ADR 0007.

## Context
Rounds 1 and 2 define v1: 4 N-gram experts (order 0, 1, 2, and previous action + previous response), model averaging, the evidence gate, and a response selector over the fixed outcome matrix. The team wants the strongest Opponent that is still theoretically sound. Build v1 first, then add each v2 component behind the existing interfaces, one at a time. Keep a component only if it beats the version without it on dev seeds (ADR 0004: the simpler model wins ties). Record v2 in an ADR, one line in notes/decisions.md, one experiments/log.csv row per ablation.

Data volume (200-400 actions) is the binding limit, not compute. Every component is chosen for sample efficiency.

## 1. Context Tree Weighting (CTW) predictor
- Context, most recent first: r(t-1), a(t-1), r(t-2), a(t-2), ... Depth D = 6 symbols (3 exchanges). Branching alternates response set / action set.
- Each node: KT estimator over actions, Pe(a | node) = (c_a + 1/2) / (n + |A|/2).
- Node mixture: Pw = 1/2 Pe + 1/2 (product of children's Pw); only the path child changes on an update.
- Prediction leaf to root: p_node(a) = beta Pe(a | node) + (1 - beta) p_child(a), beta = (1/2 Pe_seq) / Pw_seq before the update. Log space.
- Update: increment counts on the path, recompute log Pe and log Pw leaf to root.
- Fading: counts x gamma per exchange (start 0.98, tune on dev).
- Preallocated node pool (<= 400 exchanges x D, about 2.4k nodes). No per-decision allocation.
- v1 stays in the sim as the baseline.

## 2. Longest-match expert
- Longest suffix (<= 20 exchanges) of the joint (action, response) history that occurred earlier in the session.
- Dirichlet(1/2) posterior over the actions that followed every earlier occurrence. No match: uniform.
- O(n L) per exchange (about 8k comparisons).
- Memory keeps the raw (action, response) history of the session.

## 3. Level-k / mirror experts (Iocaine Powder style)
For each base predictor B in {CTW, longest-match}, forecast p:
- Level 0: p. Level 1: uniform over actions that beat BR(p). Level 2: the same step applied to level 1.
- Mirror: a CTW over the Opponent's own responses gives q. Mirror-1: an action that beats our most likely response under q. Mirror-2: one more level.
- Smoothing: p <- (1 - eps) p + eps / |A|, eps = 0.05. About 8 meta-experts, each with a unit test using a scripted player of its type.

## 4. Fixed Share mixing
- After action a: w_i <- w_i p_i(a), normalise, w_i <- (1 - alpha) w_i + alpha / K. alpha starts at 0.01.
- Mixed forecast p = sum_i w_i p_i. Optional: CTW at gamma in {1.0, 0.98, 0.9} as three experts.
- Match start: w <- 0.5 w + 0.5 / K.
- The evidence gate stays on the single mixed forecast.

## 5. Safe-exploitation response selector
- Nash equilibrium sigma* and value v* of the zero-sum outcome matrix (+1/0/-1) by a small deterministic LP. Tests: RPS gives uniform; one asymmetric matrix with a known answer.
- BR(p) = argmax_r sum_a p(a) U(r, a); ties to the authored counter, then lower index.
- Budget k_t = k0 + sum of (payoff - v*) over past exchanges. k0 starts at 2.
- sigma = lambda BR + (1 - lambda) sigma*, lambda = clamp(k_t / (v* - min_a U(BR, a)), 0, 1). Gate closed: lambda = 0.
- Response drawn from sigma with the seeded Rng; test mode uses argmax.
- Property test: worst-case expected payoff >= v* - k_t at every step.

## 6. Lookahead: deferred
Expectimax over 2-3 exchanges with the mixed forecast, only if HP or round state changes the best response. Record in docs/open-questions.md.

## 7. Insights and Commentary
Insights describe the leading expert, only when the gate is open, top probability >= 0.4 and supporting faded count >= N.
- CTW: "After you <action> and I <response>, you usually <action>", from the deepest node on the path with enough counts.
- Longest-match: "You're repeating a sequence from earlier."
- Level-1/2: "You're trying to beat my counter."
- Mirror: "You're reacting to my last response."
The Belief line names the leading expert in plain words, no jargon.

## 8. Constraints
Pure C#, deterministic per seed, ties by fixed index, offline. Under 1 ms per decision, p50/p99 measured with Stopwatch in the sim. No per-decision heap allocation, no LINQ on the decision path. Files under 400 lines, XML docs on public types.

## 9. Acceptance
- All gate parameters (threshold, k0, alpha, gamma) fixed on dev before the single heldout run.
- v2 >= v1 on mean late prediction accuracy for all 4 archetypes; v2 > v1 on Opponent win rate vs Win-stay/lose-shift.
- Random: 0 claims over the heldout seeds; Opponent win rate within +/- 0.03 of v*.
- Habit-switch unit test passes (5 in a row within 30 exchanges).
- Ablation rows: v1, +CTW, +longest-match, +level-k/mirror, +Fixed Share, +safe exploitation.

## Not used (for docs/CONTEXT.md)
Neural networks/LSTMs (too little data, no pretrained models); Q-learning/deep RL (one-step rewards reduce to a bandit, too data-hungry, assume a non-adapting player); MCTS (no state depth); epsilon-greedy/UCB1 (exploitable exploration, ignore the forecast).

## References
Willems, Shtarkov & Tjalkens (1995); Herbster & Warmuth (1998); Ganzfried & Sandholm (2015); Egnor (2000); Ville (1939) / Shafer & Vovk.
