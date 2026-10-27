# Spec: Opponent v2 (maximum strength)

Status: approved after grilling (2026-10-27). Source: the team lead's brief, with every grilling correction applied. Decisions: notes/decisions.md. Reasons: docs/adr/0007-v2-stronger-opponent.md.

## Context
v1 (notes/decisions.md, "Opponent v1") is built first: order 0/1/2 and (previous action, previous response) experts with per-visit fading, Fixed Share mixing, the evidence gate, and Thompson Sampling once the gate is open. v2 adds the components below behind the same interfaces, one at a time. Each is kept only if it beats the version without it on paired dev seeds (ADR 0004: the simpler model wins ties). One experiments/log.csv row per ablation.

Data volume (about 100-300 exchanges per real session; 300 in the fixed-length harness) is the binding limit, not compute. Every component is chosen for sample efficiency.

## 1. Context Tree Weighting (CTW) experts
Two CTW experts, differing only in context order:
- Action-first: a(t-1), r(t-1), a(t-2), r(t-2), ... "Repeats the last action" is a depth-1 node.
- Response-first: r(t-1), a(t-1), r(t-2), a(t-2), ... "Reacts to my last response" is a depth-1 node.

Depth D = 6 symbols (3 exchanges). Branching alternates action set / response set. r(t), the current response, is never in the context: the player has not seen it.

Each node:
- KT estimator over actions: Pe(a | node) = (c_a + 1/2) / (n + |A|/2).
- Stores log Pe_seq, log Pw_seq and logProd = log of the product of all children's Pw_seq.
- Pw_seq = 1/2 Pe_seq + 1/2 Prod. On an update only the path child changes, so logProd += logPw_child_new - logPw_child_old.
- Mixing weight as log-odds L = log(Pe_seq / Prod). beta = sigmoid(L).

Prediction, leaf to root along the current path: p_node(a) = beta Pe(a | node) + (1 - beta) p_child(a). The children off the path cancel in this ratio. Work in log space.

Update: increment counts on the path, then update each node leaf to root. After each update, L <- gamma L, then clamp L to [-6, 6]. Without this, L grows without bound and beta freezes, whatever the counts do. With it, CTW is a switching-style approximation, not exact Bayes; this is deliberate.

Fading is per visit: a node's counts are multiplied by gamma (start 0.98, tuned on dev) each time the node is on the path. Global-time fading is one ablation row.

Preallocated node pool (<= 400 exchanges x D x 3 trees: action-first, response-first and the mirror CTW over responses). No per-decision allocation.

Unit test: one node is fed 50 symbols favouring its own estimate, then 50 favouring the child; beta crosses 0.5 within 15 symbols of the flip.

## 2. Longest-match expert
- Finds the longest suffix (<= 20 exchanges) of the joint (action, response) history that occurred earlier in the session.
- Predicts a Dirichlet(1/2) posterior over the actions that followed every earlier occurrence of that suffix. No match: uniform.
- O(n L) per exchange, about 8k comparisons.
- Memory keeps the session's raw (action, response) history.

## 3. Level-k / mirror experts (Iocaine Powder style)
For each base forecast p from the CTW experts and the longest-match expert:
- Level 1: the player expects our best response BR(p) and plays an action that beats it. Forecast: uniform over the two actions that beat BR(p).
- Level 2: the same step applied to the level-1 forecast.

Mirror (the player models us): a CTW over the Opponent's own responses gives q.
- Mirror-1: uniform over the two actions that beat our most likely response under q.
- Mirror-2: one more level.

Every forecast is smoothed: p <- (1 - eps) p + eps / |A|, eps = 0.05. Each meta-expert has a unit test with a scripted player of its type.

Keep rule (its own, because the archetype set barely tests it): keep level-k/mirror if (a) not worse on Spammer, Cycler, Win-stay/lose-shift and Random by more than the margin in prediction accuracy, (b) >= 0.3 higher accuracy than without them on the scripted level-1 and mirror-1 players, (c) not worse against Out-guesser by more than the margin, on payoff. If they do not improve the Out-guesser result, they are kept on the scripted tests alone; that weaker evidence is checked against humans through the manual play log.

## 4. Fixed Share mixing (already in v1)
- After action a: w_i <- w_i p_i(a), normalise, then w_i <- (1 - alpha) w_i + alpha / K. alpha starts at 0.01.
- Mixed forecast p = sum_i w_i p_i. v1's order-1 expert stays in the mix as a hedge.
- Match start: w <- 0.5 w + 0.5 / K.
- Optional ablation row: the action-first CTW at gamma in {1.0, 0.98, 0.9} as three experts.
- The evidence gate stays on the single mixed forecast, made before the action, so Ville's bound holds.

## 5. Safe-exploitation response selector
- Nash: the balanced outcome matrix gives sigma* = uniform and v* = 0 (checked at load; no LP).
- BR(p) = argmax_r sum_a p(a) U(r, a). Ties go to the authored counter, then the lower index.
- Budget: k starts at k0 = 0.25 (tuned on dev in [0.1, 1]); after each exchange k += sum_r sigma_t(r) U(r, a_t) - v* (expected gift, not realised payoff).
- Strategy: sigma = lambda BR + (1 - lambda) sigma*, lambda = clamp(k / (v* - min_a U(BR, a)), 0, 1). With v* = 0 and min U = -1, lambda = min(k, 1); against a real pattern it roughly doubles per correct exchange.
- Gate closed: lambda = 0, so the Opponent plays sigma*.
- The response is drawn from sigma with the seeded Rng; test mode uses argmax.
- Property test: across random forecasts, worst-case expected payoff >= v* - k at every step.
- Judged on payoff against v1's Thompson Sampling. ADR 0006 is superseded only if this wins.

## 6. Lookahead: deferred
Expectimax over 2-3 exchanges with the mixed forecast, only if HP or round state changes the best response. Recorded in docs/open-questions.md.

## 7. Insights and Commentary
An Insight describes the leading expert, only when the gate is open, the top probability is >= 0.4 and the supporting faded count is >= 5.
- CTW: "After you <action> and I <response>, you usually <action>", from the deepest node on the path with enough counts.
- Longest-match: "You're repeating a sequence from earlier."
- Level-1/2: "You're trying to beat my counter."
- Mirror: "You're reacting to my last response."

The Belief line names the leading expert in plain words. No expert or gate jargon reaches the player.

## 8. Constraints
Pure C#, deterministic per seed, ties by fixed index, offline. Under 1 ms per decision, p50/p99 measured with Stopwatch in the sim. No per-decision heap allocation, no LINQ on the decision path. Files under 400 lines, XML docs on public types.

## 9. Acceptance
- All tuned parameters (gate threshold, k0, alpha, gamma) are fixed on dev seeds before the single heldout run, never changed after it.
- Comparisons use paired seeds; margin = max(0.02, t x paired SE), t = Student-t 95% critical value for (seeds - 1) df.
- v2 >= v1 - margin on mean late prediction accuracy for every archetype.
- Strict wins: v2 beats v1 by more than t x paired SE on Win-stay/lose-shift win rate and on Out-guesser payoff.
- Random: the evidence gate never opens on any heldout seed (so no Insights) and lambda = 0 on every exchange. Mean payoff within v* +/- 0.05 is a simulator check.
- Habit-switch unit test passes.
- Ablation rows (dev, greedy, in order): v1, v1 without Fixed Share, +CTW x2, +longest-match, +level-k/mirror, +safe exploitation, optional +CTW at 3 gammas, global-time fading. Predictor rows are judged on prediction accuracy, selector rows on payoff. A row beats the one above if it improves at least one archetype by more than t x paired SE and is not worse on any by more than the margin.

## Not used (for docs/CONTEXT.md)
- Neural networks and LSTMs: far too little data; pretrained models are not allowed.
- Q-learning and deep RL: rewards are one-step, so the Bellman update reduces to a bandit; they need far more data and assume a player who does not adapt.
- MCTS: the game state has no depth to search.
- Epsilon-greedy and UCB1: fixed-rate or deterministic exploration a human can exploit, and they ignore the forecast. Safe exploitation replaces them.

## References
- Willems, Shtarkov & Tjalkens (1995), "The Context-Tree Weighting Method: Basic Properties".
- Herbster & Warmuth (1998), "Tracking the Best Expert".
- Ganzfried & Sandholm (2015), "Safe Opponent Exploitation".
- Egnor (2000), "Iocaine Powder", ICGA Journal.
- Ville (1939); Shafer & Vovk: test martingales.
