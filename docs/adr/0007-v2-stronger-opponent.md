# 0007: v2 Opponent: CTW, longest match, level-k and safe exploitation, only if it beats v1

**Status:** Proposed (2026-10-27)

v1 (fixed-order N-gram experts with Bayesian model averaging plus Fixed Share, and Thompson Sampling over predicted actions per ADR 0006) is built first. v2 adds: two Context Tree Weighting experts (action-first and response-first context, per-visit fading, clamped mixing log-odds) in place of the fixed-order N-gram experts; a longest-match expert (find the longest earlier repeat of the recent history and predict what followed it); level-k meta-strategies in the style of Iocaine Powder, so the Opponent can counter players who adapt to it; and safe exploitation (Ganzfried & Sandholm), mixing the best response with the Nash strategy (uniform under the balanced outcome matrix) by a budget of expected gifts that starts at k0 = 0.25. v2 replaces v1 only if it beats v1 on dev seeds; otherwise v1 stays, and this ADR records why.

## Consequences

- The longest-match expert needs the raw exchange history of the session, so Memory must keep it, not only faded counts.
- v1 and v2 run as separate configs in the sim on the same seeds, and each run gets its own experiments/log.csv row.
- Level-k and mirror experts can only earn their place against a player who models the Opponent, so the Out-guesser archetype is added to the suite.
- Each component is kept only if it beats the row above on paired dev seeds (rules in notes/decisions.md); the full spec is .scratch/opponent-v2/spec.md.
- Lookahead (expectimax over 2-3 exchanges) is deferred until HP or round state changes the best response.
- With fading and clamped mixing log-odds, CTW is a switching-style approximation, not exact Bayes; this is deliberate so it can track habit changes.
- Level-k/mirror experts have their own keep rule (notes/decisions.md): no worse on the other archetypes and Out-guesser within max(0.02, 2 x paired SE), and >= 0.3 higher accuracy on scripted level-1 and mirror-1 players. If they do not improve the Out-guesser result they are kept on the scripted tests alone; that is weaker evidence, checked against humans through the logged manual play.
