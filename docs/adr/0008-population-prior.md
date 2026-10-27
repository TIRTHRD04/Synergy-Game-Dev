# 0008: The Opponent starts from a population prior learned from play logs

**Status:** Proposed (2026-10-27). Shipped only if it passes the acceptance rule below.

With no prior, the Opponent knows nothing for the first ~30 exchanges, which is exactly what a demo shows first. People share habits that do not depend on the theme: they avoid repeating a move, shift after a loss, and go for what beats what just beat them (Wang et al., 2014, on rock-paper-scissors). We learn those habits once and ship them as a small table, so the Opponent starts with a sensible guess about people in general.

## Decision

- The prior is relational, so it survives any theme: for each outcome of the last exchange (win, draw, loss), the probability that the next action is the same as the last action, beats the Opponent's last response, is beaten by it, and so on. Each relation is defined by the outcome matrix, not by action names.
- It is turned into Dirichlet pseudo-counts for the order-1 and (action, response) experts (and CTW depth-1/2 nodes if v2 ships), with total strength w pseudo-counts per context. w is tuned on dev in [0, 20]; w = 0 means no prior.
- It is learned only from teammates' play logs (dev data) and shipped as configs/prior.json. Fresh players' logs (the report set) are never used to build it.
- Commentary and Insights never cite the prior; they describe only this player's evidence.

## Acceptance

The decision is made on teammates' play logs only, by leave-one-player-out: build the prior from all teammates but one, test on that one, repeat for each. Ship it if early prediction accuracy (exchanges 1-30) beats w = 0 on average across those folds, no archetype is worse by more than the max(0.02, 2 x paired SE) margin, and Random still makes 0 claims. The fresh players' report set is then used once, to report the result, never to decide or tune. Otherwise configs/prior.json stays out and this ADR records why.

## Considered options

- No prior (the default before this ADR): the Opponent is weakest exactly when the demo starts.
- Absolute priors ("people pick action 3"): meaningless until the theme fixes what actions are, and fragile after a reskin.
- Public rock-paper-scissors datasets: a 3-move game with different relations and unknown licences; using one is a research detour that needs the team's approval first.
- A neural network pretrained on play logs: far too little data, and it cannot be explained in Commentary.
