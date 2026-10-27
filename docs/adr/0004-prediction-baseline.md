# 0004: Prediction baseline is an order-k Markov model, compared against frequency counts

**Status:** Proposed. The sim runs a `full` config (all experts, averaged) and a `freq` config (order-0 frequency counts alone) on the same seeds; the result decides this ADR (2026-10-27).

This is a proposed baseline, not a final choice. The headless simulator will compare it against frequency-count prediction before anything is selected.

## Context

The predictor must estimate the player's next action from observed history. It has to adapt to habits, respond to changes in behavior, and stay deterministic and offline.

## Decision (proposed)

Use an order-k Markov model with a configurable k and recency decay as the baseline. When there is not enough data, fall back to frequency counts. Break ties deterministically, for example by fixed action order.

The final choice will be made by comparing this baseline with frequency-count prediction in the headless simulator, against the same scripted player sequences.

## Consequences

- k and the decay rate are configuration parameters, not constants.
- A "sufficient data" threshold must be defined before the fallback can be used.
- The simulator must exist before this decision can be finalized.
- If frequency counts perform as well as the Markov model, the simpler model should be chosen.

## Unresolved dependencies

- The action set size and session length affect how much data is available for a Markov model. Both depend on the TBD gameplay. See [open-questions.md](../open-questions.md).
