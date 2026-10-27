# 0007: v2 Opponent: CTW, longest match, level-k and safe exploitation, only if it beats v1

**Status:** Proposed (2026-10-27)

v1 (fixed-order N-gram experts with Bayesian model averaging plus Fixed Share, and Thompson Sampling over predicted actions per ADR 0006) is built first. v2 adds: a Context Tree Weighting predictor in place of the fixed-order N-gram experts; a longest-match expert (find the longest earlier repeat of the recent history and predict what followed it); level-k meta-strategies in the style of Iocaine Powder, so the Opponent can counter players who adapt to it; and safe exploitation, mixing the best response with the Nash strategy (uniform over responses under the balanced outcome matrix) in proportion to confidence. v2 replaces v1 only if it beats v1 on dev seeds; otherwise v1 stays, and this ADR records why.

## Consequences

- The longest-match expert needs the raw exchange history of the session, so Memory must keep it, not only faded counts.
- v1 and v2 run as separate configs in the sim on the same seeds, and each run gets its own experiments/log.csv row.
