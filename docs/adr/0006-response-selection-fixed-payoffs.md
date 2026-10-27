# 0006: Response selection samples predicted actions; payoffs come from the fixed outcome matrix

**Status:** Accepted (2026-10-27)

The outcome matrix is game data that the Opponent already knows exactly, so learning payoffs (Beta posteriors, outcome-adjusted counters as in ADR 0005) would only re-estimate known numbers and blur the "no reinforcement learning" line in ADR 0001. Instead the response selector uses Thompson Sampling over the predicted action probabilities only: it samples p(action) from the predictor's Dirichlet posterior and commits the response with the highest expected payoff, `EV(r) = Σ p(a)·payoff(r,a)`, with payoffs (+1 win, 0 draw, −1 loss) read from the outcome matrix. Outcomes still matter, as input to the predictor, not to the selector. When no pattern may be claimed, the selector uses the authored counter.

## Considered options

- Beta posteriors on payoffs with the outcome matrix as prior: rejected, payoffs are not uncertain.
- Greedy argmax on the mean prediction: rejected for play, because a fully predictable Opponent is easy to exploit; kept as the test-only mean mode below.

## Consequences

- Determinism (ADR 0002): the Rng seed comes from `configs/` and is an input. Ties break by lowest index. Tests may switch, by config flag, to the posterior mean instead of a sample; this is the same code path.
- The outcome matrix must be balanced (each response beats 2 actions, draws with 2, loses to 2), validated on load, so every action has exactly two counters.
