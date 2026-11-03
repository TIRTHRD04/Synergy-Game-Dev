# 0003: Input is action history plus a minimal state snapshot

**Status:** Deferred: history-only chosen (2026-10-27)

The Opponent predicts from action history alone, the fallback this ADR allows. The predictor interface takes no snapshot. If a snapshot is added later, HP is the first candidate field.

## Context

The predictor could use only the player's action history, or the game could also supply context. Supplying the game's internal state directly would couple the AI to game code and break the pure-C# requirement. Adding a snapshot also adds complexity that may not be needed.

## Decision

The AI observes the action history and a minimal, game-agnostic state snapshot. The game supplies the snapshot through a pure-C# interface. The AI never accesses UnityEngine objects or game internals directly. The snapshot's fields are not yet defined.

If the snapshot adds more complexity than it gives back, the AI falls back to history-only prediction. This is an allowed outcome, not a failure.

## Consequences

- The snapshot interface must stay minimal until the game's mechanics are known.
- The predictor must work with history alone, so the fallback path needs its own tests.
- The Unity adapter is responsible for building each snapshot from game state.

## Unresolved dependencies

- The snapshot's fields are TBD and depend on the game's mechanics. See [open-questions.md](../open-questions.md).
