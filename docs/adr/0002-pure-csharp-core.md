# 0002: The AI core is pure C#, deterministic, and offline

**Status:** Accepted

## Context

The AI must run inside a Unity game but also be testable without the editor. Its behavior has to be reproducible so tests and simulations give the same results.

## Decision

The AI core is pure C#. It has no UnityEngine dependency and no network access. It is deterministic: the same inputs always produce the same outputs, with fixed tie-breaking rules. The Unity integration lives in a separate adapter assembly that references UnityEngine. The core never reads or writes files. Nothing is persisted: Memory lasts one session (see notes/decisions.md); a storage interface comes back only if saved profiles do.

## Consequences

- The core can be tested headlessly with a simulator and standard unit tests.
- Every input the core uses comes through an interface: the definition (actions, responses, outcome matrix), the exchange stream (action, response, outcome) and the seeded Rng. There is no state snapshot (ADR 0003, deferred) and no storage.
- Ties in prediction and response selection break deterministically: the authored counter, then the lower index.
- The Unity adapter stays thin, and all decisions live in the core.

## Notes

- Updated 2026-10-27: storage and the state snapshot were dropped from the interface list. Randomness in the Game layer that never reaches the core (the commitment nonce) may use an unseeded secure source.
