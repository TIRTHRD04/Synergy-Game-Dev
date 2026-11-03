# 0010: Damage varies per pairing; the Damage table stays balanced

**Status:** Accepted (2026-10-27)

Every lost exchange used to cost 1 HP. The team wants some pairings to hit harder. Damage is fixed per (response, action) pair, so it is game data like the outcome matrix, and the Opponent never learns it.

## Decision

- `configs/definition.json` holds a Damage table D(r, a) >= 1 on every win and loss cell; draws are 0. The payoff to the Opponent is U(r, a) = +D(r, a) when the response wins, -D(r, a) when it loses, and 0 for a draw. A lost exchange costs the loser D HP.
- The loader runs two checks, each with its own job:
  - (b) Every row and every column of U sums to 0. This alone makes uniform play an equilibrium (sigma* = uniform, v* = 0), so no LP is needed. A unit test checks that uniform play scores exactly 0 against every column.
  - (a) Every row and every column has exactly 2 wins, 2 draws and 2 losses. Nash does not need this; Win-stay/lose-shift, level-k, mirror and the population prior's relations do, because they rely on every action having exactly two counters.
- Example table (circulant): U(r, a) = f((a - r) mod 6) with f = [0, +3, +1, 0, -1, -3]. Each action has a Heavy counter (3 damage) and a Light counter (1 damage). The authored counter is data; in the example it is the Heavy counter.
- Scale: payoffs now range over +/-3, not +/-1.
  - Safe exploitation: lambda = clamp(k / (v* - min_a U(BR, a)), 0, 1). Every row of the example contains -3, so lambda = min(k / 3, 1). k is in damage units: k0 = 0.75 (was 0.25), tuned on dev in [0.3, 3]. Under sigma = lambda BR + (1 - lambda) sigma*, the expected gift is lambda U(BR, a), so k doubles only when the best response lands its heavy win; against a fully predictable action it does.
  - The payoff margin floor becomes 0.06 (0.02 x max damage) and Random's payoff check v* +/- 0.15. Accuracy and log-loss margins, and every t x paired SE rule, are scale-free and do not change.
- Start HP is retuned on dev seeds (rough start 60).

## Consequences

- The selector maximises expected damage per exchange. When either side's HP is at most the maximum damage, one heavy hit decides the match, so expected damage is not the same as win probability. This is a lookahead trigger, recorded in docs/open-questions.md and ADR 0007 next to the deferred combo rule. The harness is unaffected because HP never ends a harness match.
- A rule where damage grows over a streak (a combo) is not part of this ADR; it would make the best response depend on state and needs lookahead.
- Numbers checked by a script (output, 2026-10-27): row sums and column sums all 0; every row and column 2/2/2; uniform scores 0.0 against every column; the lambda denominator is 3 for every response.
