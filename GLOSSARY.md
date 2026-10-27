# Glossary

The language of the adaptive-opponent project. **Working definitions** are settled or provisional terms the project uses now. **Game-domain definitions** stay TBD until the game's theme is chosen. Do not treat a TBD entry as settled.

## Working definitions

### Play

**Opponent**:
The in-game AI that learns a player's habits and responds to them. See [ADR 0001](docs/adr/0001-in-game-opponent-role.md). Status: accepted.
_Avoid_: bot, enemy AI, agent

**Action**:
One of the player's fixed set of mutually exclusive choices; the player picks exactly one per exchange.
_Avoid_: move, input

**Response**:
One of the Opponent's fixed set of choices; the Opponent picks exactly one per exchange.
_Avoid_: move, reply, counter-move

**Exchange**:
One unit of play: the Opponent commits a response, then the player picks an action, then both are revealed and the outcome is resolved.
_Avoid_: step, turn, round

**Outcome matrix**:
The rule table that gives the outcome of every response against every action. It is game data, not something the Opponent learns.

**Outcome**:
The result of one exchange from the Opponent's side: win, draw or loss.
_Avoid_: outcome signal, reward

**Counter**:
A response that beats a given action under the outcome matrix. Every action has exactly two counters.

**Authored counter**:
The designer's default counter for an action, used when the Opponent has no pattern it may claim.
_Avoid_: authored table, default response

**HP**:
The health each side has in a match; a side that loses an exchange loses HP.

**Match**:
A contest that ends when either side's HP reaches zero or the exchange cap is reached.

**Session**:
Three matches played in one run of the game. The Opponent's learning lasts for one session and is lost when the game closes or a new session starts.
_Avoid_: run, game

**Archetype**:
A scripted behaviour that plays in place of a human player, used to check that the Opponent adapts. The four archetypes are Spammer, Cycler, Win-stay/lose-shift and Random.
_Avoid_: simulated player, bot, scripted player

### The Opponent's parts

**Predictor**:
The component that estimates the probability of each action in the next exchange. See [ADR 0004](docs/adr/0004-prediction-baseline.md). Status: proposed.

**Response selector**:
The component that turns the predicted action probabilities into one committed response. See [ADR 0006](docs/adr/0006-response-selection-fixed-payoffs.md).
_Avoid_: counter selector

**Prediction accuracy**:
The fraction of exchanges in which the predictor's single most likely action, fixed before the player acted, matched the action the player actually picked. Chance is one over the number of actions.
_Avoid_: AI accuracy, hit rate

**State snapshot**:
Game context beyond action history that the game could supply to the Opponent. Not used: prediction is history-only. See [ADR 0003](docs/adr/0003-state-snapshot-input.md). Status: deferred.

## Game-domain definitions (TBD)

- **Genre / theme**: TBD. Actions, responses and the outcome matrix are placeholders until it is chosen.
- **Action names**: TBD
- **Response names**: TBD
