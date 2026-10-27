# 0007: v2 Opponent: CTW, longest match, level-k and safe exploitation, only if it beats v1

**Status:** Accepted in part (2026-10-27), after the dev ablations in notes/decisions.md ("Dev ablation results"):
- Kept: longest-match (a negligible gain, kept by the rule) and safe exploitation with k0 = 3 (better payoff against Cycler, Win-stay/lose-shift and Out-guesser; both strict wins).
- Rejected: CTW x2 (worse beyond the margin on Big-hit repeater), level-k/mirror (failed keep rule (b): +0.126 and +0.213, below +0.3), global-time fading.
- So v2 (configs/v2.json) is v1's experts plus longest-match, the ADR 0011 experts and safe exploitation. The rejected code stays only for configs/v2-full-experimental.json in the playtest build and is deleted before submission.

v1 (fixed-order N-gram experts with per-visit fading and Fixed Share mixing, and Thompson Sampling over predicted actions per ADR 0006) is built first. v2 adds: two Context Tree Weighting experts (action-first and response-first context, per-visit fading, clamped mixing log-odds) in place of the fixed-order N-gram experts, keeping v1's order-1 expert as a hedge; a longest-match expert (find the longest earlier repeat of the recent history and predict what followed it); level-k meta-strategies in the style of Iocaine Powder, so the Opponent can counter players who adapt to it; and safe exploitation (Ganzfried & Sandholm), mixing the best response with the Nash strategy (uniform under the balanced outcome matrix) by a budget of expected gifts that starts at k0 = 0.75 damage units (0.25 before variable damage, ADR 0010). v2 replaces v1 only if it beats v1 on dev seeds; otherwise v1 stays, and this ADR records why.

## Consequences

- The longest-match expert needs the raw exchange history of the session, so Memory must keep it, not only faded counts.
- v1 and v2 run as separate configs in the sim on the same seeds, and each run gets its own experiments/log.csv row.
- Level-k and mirror experts can only earn their place against a player who models the Opponent, so the Out-guesser archetype is added to the suite.
- Each component is kept only if it beats the row above on paired dev seeds (rules in notes/decisions.md); the full spec is .scratch/opponent-v2/spec.md.
- Lookahead (expectimax over 2-3 exchanges) is deferred. Two things would bring it back: a combo rule (damage that grows over a streak), or a match where either side's HP is at most the maximum damage, so one heavy hit decides it (ADR 0010). Both are in docs/open-questions.md.
- With fading and clamped mixing log-odds, CTW is a switching-style approximation, not exact Bayes; this is deliberate so it can track habit changes.
- Level-k/mirror experts have their own keep rule (notes/decisions.md): no worse on the other archetypes and Out-guesser within max(0.02, t x paired SE), and >= 0.3 higher accuracy on scripted level-1 and mirror-1 players. If they do not improve the Out-guesser result they are kept on the scripted tests alone; that is weaker evidence, checked against humans through the logged manual play.
