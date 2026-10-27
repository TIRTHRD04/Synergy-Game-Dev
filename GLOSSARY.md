# Glossary

This glossary has two sections. **Working definitions** are architectural terms the project can use now. **Game-domain definitions** stay TBD until the game is specified. Do not treat a TBD entry as settled.

## Working definitions

- **Opponent**: the in-game AI that adapts to a player's habits across matches. See [ADR 0001](docs/adr/0001-in-game-opponent-role.md). Status: accepted.
- **Action**: one of the player's discrete choices in a step.
- **Action set**: the fixed set of 3–8 named, mutually exclusive actions. Exactly one action is chosen per step. The names are TBD (see Game-domain definitions).
- **Step**: one unit of play in which the player chooses exactly one action. Turn-based play is provisional, so whether a step equals a turn is not yet confirmed.
- **State snapshot**: a minimal, game-agnostic value the game supplies to the AI each step. Its fields are TBD. See [ADR 0003](docs/adr/0003-state-snapshot-input.md). Status: provisional.
- **Observation log**: the record of (action, snapshot) pairs the AI has seen, kept across matches.
- **Session**: the span over which learning accumulates, roughly 200–400 actions. Its boundary is TBD.
- **Predictor**: the component that estimates the player's next action. The proposed baseline is an order-k Markov model with recency decay. The fallback is frequency counts. See [ADR 0004](docs/adr/0004-prediction-baseline.md). Status: proposed.
- **Counter**: a response the AI chooses in reply to a predicted action. Concrete counters are TBD.
- **Authored table**: the designer-defined default counter for each predicted action.
- **Counter selector**: the component that chooses a counter from the authored table, adjusted by outcome signals. See [ADR 0005](docs/adr/0005-hybrid-counter-selection.md). Status: provisional.
- **Outcome signal**: an observable result the game reports after a counter is used. It is optional. When it is absent, the selector falls back to the authored table. What counts as success is TBD.

## Game-domain definitions (TBD)

- **Genre**: TBD
- **Player objective**: TBD
- **Match**: TBD, including its ending conditions
- **Action names**: TBD
- **Counter responses**: TBD
- **Snapshot fields**: TBD
- **Success** (the meaning of a positive outcome signal): TBD

See [docs/open-questions.md](docs/open-questions.md) for the blockers on each TBD item.
