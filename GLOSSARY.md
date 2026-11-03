# Glossary

The language of the adaptive-opponent project. **Working definitions** are settled or provisional terms the project uses now. **Game-domain definitions** name the fighting theme (ADR 0012).

## Working definitions

### Play

**Opponent**:
The in-game AI that learns a player's habits and responds to them. See [ADR 0001](docs/adr/0001-in-game-opponent-role.md). Status: accepted.
_Avoid_: bot, enemy AI, agent

**Action**:
One of the player's fixed set of mutually exclusive choices; the player picks exactly one per exchange.
_Avoid_: move (except in on-screen text, see Move), input

**Response**:
One of the Opponent's fixed set of choices; the Opponent picks exactly one per exchange.
_Avoid_: move (except in on-screen text, see Move), reply, counter-move

**Exchange**:
One unit of play: the Opponent commits a response, then the player picks an action, then both are revealed and the outcome is resolved.
_Avoid_: step, turn, round

**Outcome matrix**:
The rule table that gives the outcome of every response against every action. It is game data, not something the Opponent learns. With the Damage table it gives the payoff of every exchange.

**Damage**:
The HP the loser of an exchange loses. It depends on the (response, action) pair. See [ADR 0010](docs/adr/0010-variable-damage.md).
_Avoid_: crit, hit points lost

**Damage table**:
The game data that gives the damage of every winning or losing (response, action) pair; draws deal no damage. It is balanced so that uniform play is an equilibrium.

**Outcome**:
The result of one exchange from the Opponent's side: win, draw or loss.
_Avoid_: outcome signal, reward

**Counter**:
A response that beats a given action under the outcome matrix. Every action has exactly two counters: a Heavy counter and a Light counter.

**Heavy counter**:
The counter of an action that deals more damage.

**Light counter**:
The counter of an action that deals less damage.

**Authored counter**:
The designer's preferred counter for an action, used to break ties between equally good responses.
_Avoid_: authored table, default response

**HP**:
The health each side has in a match; a side that loses an exchange loses the exchange's damage in HP.

**Match**:
A contest that ends when either side's HP reaches zero or the exchange cap is reached.

**Session**:
Three matches played in one run of the game. The Opponent's learning lasts for one session and is lost when the game closes or a new session starts.
_Avoid_: run, game

**Archetype**:
A scripted behaviour that plays in place of a human player, used to check that the Opponent adapts. The seven archetypes are Spammer, Cycler, Win-stay/lose-shift, Out-guesser, Random, Big-hit repeater and Burned avoider.
_Avoid_: simulated player, bot, scripted player

**Big-hit repeater**:
The archetype that replays a Big-hit sequence it landed earlier when the same previous action comes up again, at least 30 exchanges later.

**Burned avoider**:
The archetype that avoids the Opening of a big hit it took, at the same spot, and slowly forgets to.

**Big hit**:
Three consecutive exchanges in which the same side takes damage every time, totalling at least the big-hit threshold. A flag on the exchange that completes it, not an Outcome; the game rules do not change. See [ADR 0011](docs/adr/0011-big-hit-memory.md).
_Avoid_: combo (reserved for a deferred rule where damage grows over a streak), burst, streak, crit

**Start context**:
The previous action and previous response just before the first exchange of a big hit.

**Broad context**:
The previous action alone, before the first exchange of a big hit.

**Match-start context**:
The context of exchange 1 of a match, which has no previous exchange.

**Big-hit sequence**:
The player's three actions during a big hit: the Opening, then two Follow-ups.
_Avoid_: run, combo

**Opening**:
The first action of a Big-hit sequence.

**Follow-up**:
The second or third action of a Big-hit sequence.

**Play log**:
The record of one human session's exchanges, written by the game for later analysis. The Opponent never reads it.
_Avoid_: profile, telemetry, save file

**Population prior**:
The Opponent's starting guess about how people in general play, learned from earlier play logs before it has seen this player.
_Avoid_: pretrained model, default habits

### The Opponent's parts

**Predictor**:
The component that estimates the probability of each action in the next exchange. See [ADR 0004](docs/adr/0004-prediction-baseline.md). Status: proposed.

**Response selector**:
The component that turns the predicted action probabilities into one committed response. See [ADR 0006](docs/adr/0006-response-selection-fixed-payoffs.md).
_Avoid_: counter selector

**Expert**:
One of the predictor's sub-models; each looks at a different slice of recent history and gives its own probability for every action.
_Avoid_: model, sub-predictor

**Memory**:
What the Opponent has learned in the current session: its experts' counts, their weights, the evidence gate's score, Big-hit memory, the Avoidance record and, in v2, the session's raw exchange history. It carries across the matches of a session and is dropped at session end.
_Avoid_: observation log, profile (reserved for Memory saved to disk, not built)

**Abstain**:
What an expert does when it has nothing to say this exchange. Its weight is left unchanged, and it is not drawn by Thompson Sampling.
_Avoid_: sleep (as a term), skip

**Big-hit memory**:
The Opponent's record of the big hits the player landed on it: start context, Big-hit sequence and two strengths, one for exact and one for broad matches.
_Avoid_: scar, grudge

**Big-hit sequence expert**:
The expert that predicts a Big-hit sequence again when its start context or broad context comes back.

**Avoidance record**:
The Opponent's record of the big hits it landed on the player: start context, Opening and two strengths.
_Avoid_: scar, burn list

**Avoidance expert**:
The expert that predicts the player will avoid the Opening of a big hit they took, at the same spot.

**Evidence gate**:
The test that decides whether the Opponent has enough evidence that the player is predictable to claim any pattern at all.
_Avoid_: confidence check, threshold

**Insight**:
A statement about the player's pattern that the Opponent may show: the evidence gate is open and the current prediction is strong and well supported.
_Avoid_: claim (as a noun), observation

**Recall**:
A factual Commentary line about a recorded big hit, shown only while the evidence gate is open and the Big-hit sequence expert's prediction is strong.
_Avoid_: callback, taunt

**Commentary**:
Player-facing lines built only from Insights and Recalls.
_Avoid_: banter, flavour text

**Belief**:
The Opponent's forecast for an exchange, its leading expert and its confidence, as shown on screen in the AI Brain panel after the reveal (ADR 0012).
_Avoid_: belief snapshot, belief overlay

**Prediction accuracy**:
The fraction of exchanges in which the predictor's single most likely action, fixed before the player acted, matched the action the player actually picked. Chance is one over the number of actions.
_Avoid_: AI accuracy, hit rate

**State snapshot**:
Game context beyond action history that the game could supply to the Opponent. Not used: prediction is history-only. See [ADR 0003](docs/adr/0003-state-snapshot-input.md). Status: deferred.

## Game-domain definitions

- **Genre / theme**: a 1v1 fighting duel. See [ADR 0012](docs/adr/0012-fighting-move-theme.md).
- **Action names** and **Response names**: the same six moves, in ring order: Strike, Dodge, Grab, Heavy, Guard, Parry.

**Move**:
The on-screen word for an action or a response in the fighting theme; both sides pick from the same six moves. Used in player-facing text and in the UI code that builds it; everywhere else, say action or response.

**Read**:
On screen: an exchange where the Opponent had a prediction (its forecast was not flat) and its top prediction was the action the player picked. The read rate can be slightly below Prediction accuracy, which counts a flat forecast's tie-broken action.

**Planned response**:
The best response to the mixed forecast for the committed exchange. The Opponent plays it unless it plays a random response (gate closed, or safe exploitation plays safe) or a sampled best response (Thompson Sampling).
