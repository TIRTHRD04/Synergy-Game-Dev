# 0001: The AI is an in-game adaptive opponent

**Status:** Accepted

## Context

The project needs an AI with a defined role. Two other roles were considered: a playtesting agent that plays the game to measure balance, and a reinforcement-learning agent. The requirements call for an opponent that adapts to a player during play.

## Decision

The AI is an in-game opponent. It learns a player's habits across matches, predicts the player's next action, and chooses a counter from a predefined response set. It is not a playtesting agent and does not use reinforcement learning.

## Consequences

- Learning must run during play, at a cost the game can afford, without an offline training pipeline.
- The headless simulator is needed for testing and comparison, not for training a policy.
- The AI's behavior is judged by how well it adapts, not by how well it plays.

## Unresolved dependencies

- Resolved 2026-10-27: the core is theme-agnostic (actions, responses and the outcome matrix are data), so the genre no longer blocks this decision. The surprise jam theme only decides names and presentation.
