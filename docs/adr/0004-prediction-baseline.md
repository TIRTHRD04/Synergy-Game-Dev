# 0004: The predictor is a Fixed Share mix of count-based experts, checked against frequency counts

**Status:** Proposed (2026-10-27). The sim decides it: the `full` config (all v1 experts, mixed by Fixed Share) runs against the `freq` config (order-0 frequency counts alone) on the same paired dev seeds.

## Context

The predictor must estimate the player's next action from about 100-300 exchanges per session, adapt to habits, follow changes in behaviour, and stay deterministic and offline. The action set has 6 actions.

## Decision (proposed)

Four count-based experts, each with a symmetric Dirichlet prior and per-visit fading: order 0, order 1, order 2, and (previous action, previous response). Fixed Share mixes them, so an expert whose weight fell can recover when the player changes. There is no "sufficient data" switch: the Dirichlet prior handles sparse contexts and Fixed Share moves weight toward whichever expert predicts best. Ties break by lower index.

## Consequences

- Fading rate, Dirichlet strength and Fixed Share alpha are configuration, tuned on dev seeds.
- If `freq` is not worse than `full` beyond the paired margin (notes/decisions.md), the simpler `freq` model is chosen and this ADR records it.
- v2 (ADR 0007) and the population prior (ADR 0008) build on this mix and are judged against it.
