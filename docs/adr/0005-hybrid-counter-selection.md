# 0005: Counter selection is hybrid: authored defaults adjusted by outcomes

**Status:** Superseded by [ADR 0006](0006-response-selection-fixed-payoffs.md) (2026-10-27)

The outcome matrix is fixed game data, so payoffs are rules, not unknowns, and no outcome signal is needed to learn them.

## Context

The AI needs to choose a counter for each predicted action. A fixed table is predictable but cannot adapt to a particular player. Learning counters from scratch is harder to test and depends on a reliable success signal, which the game may not provide.

## Decision

Designers author a default counter for each predicted action. When the game provides a reliable outcome signal, the AI adjusts its counter preferences from those outcomes. When no reliable signal exists, the selector falls back to the authored table. Selection is deterministic.

Action prediction and counter selection are separate components. Only the counter selector uses outcome signals.

## Consequences

- An outcome signal is optional. The authored-table fallback must be complete and tested on its own.
- The authored table must cover every action in the action set.
- What counts as a successful outcome must be defined before the adjustment logic can be implemented.

## Unresolved dependencies

- The definition of success is TBD.
- The concrete counter responses are TBD.
- Whether the game can provide a reliable outcome signal is unknown. See [open-questions.md](../open-questions.md).
