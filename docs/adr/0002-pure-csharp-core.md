# 0002: The AI core is pure C#, deterministic, and offline

**Status:** Accepted

## Context

The AI must run inside a Unity game but also be testable without the editor. Its behavior has to be reproducible so tests and simulations give the same results.

## Decision

The AI core is pure C#. It has no UnityEngine dependency and no network access. It is deterministic: the same inputs always produce the same outputs, with fixed tie-breaking rules. The Unity integration lives in a separate adapter assembly that references UnityEngine. The core never reads or writes files directly; persistence goes through a storage interface.

## Consequences

- The core can be tested headlessly with a simulator and standard unit tests.
- Every input the core uses must come through an interface: the action stream, the state snapshot, the outcome signal, and storage.
- Ties in prediction and counter selection must break deterministically, for example by fixed action order.
- The Unity adapter stays thin, and all decisions live in the core.

## Unresolved dependencies

- The storage format is not yet chosen. It is an implementation detail, not a blocker.
