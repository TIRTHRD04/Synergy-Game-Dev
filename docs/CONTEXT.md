# Context: game rules, action set, response set, archetypes, pass criteria

Decisions behind every line are in notes/decisions.md and docs/adr/. Terms are defined in GLOSSARY.md.

## Game rules

- One Exchange: the Opponent commits a response (a SHA-256 commitment is shown), then the player picks one of 6 actions, then both are revealed and the outcome is resolved.
- 6 actions and 6 responses with the same names: the fighting moves **Strike, Dodge, Grab, Heavy, Guard, Parry** (indices 0-5, in ring order; ADR 0012). On screen both are called moves. All of it is data in `configs/definition.json`, including a one-line hint per move.
- Payoff to the Opponent: U(r, a) = f((a - r) mod 6) with f = [0, +3, +1, 0, -1, -3] (indices 0-5). So:
  - response r beats action r+1 for 3 damage (Heavy counter) and action r+2 for 1 damage (Light counter);
  - it draws with actions r and r+3;
  - it loses to action r+4 for 1 damage and to action r+5 for 3 damage (all mod 6).
  - In move terms: each move beats the next one cleanly (3) and the one after that glancingly (1), draws with itself and the opposite move.

  | Move | Beats (3) | Beats (1) | Draws | Loses to (1) | Loses to (3) |
  |---|---|---|---|---|---|
  | Strike | Dodge | Grab | Heavy | Guard | Parry |
  | Dodge | Grab | Heavy | Guard | Parry | Strike |
  | Grab | Heavy | Guard | Parry | Strike | Dodge |
  | Heavy | Guard | Parry | Strike | Dodge | Grab |
  | Guard | Parry | Strike | Dodge | Grab | Heavy |
  | Parry | Strike | Dodge | Grab | Heavy | Guard |
- Every row and column of U sums to 0 and has 2 wins, 2 draws and 2 losses, so playing uniformly at random is optimal (sigma* = uniform, v* = 0). The authored counter of action a is its Heavy counter, response a-1.
- A lost exchange costs the loser its damage in HP. A Match ends at HP 0 or after 120 exchanges. A Session is 3 matches. Start HP is 60 (tuned on dev seeds).
- Big hit: 3 consecutive exchanges in which the same side takes damage every time, totalling >= 7. It is a flag only; no extra damage.

## Harness mode

- Fixed length: 3 matches of 100 exchanges; HP never ends a harness match. Early = exchanges 1-50, late = 201-300.
- Dev seeds: `seeds/dev/seeds.txt` (tuning). Heldout seeds: `seeds/heldout/seeds.txt` (20, committed once, used once at the end).

## Archetypes

| Archetype | Behaviour |
|---|---|
| Spammer | Plays one action (per-seed), with 10% uniform noise. |
| Cycler | Plays actions in a fixed per-seed cycle of length 3, with 10% uniform noise. |
| Win-stay/lose-shift | After a win it repeats its action; after a draw or loss it plays one of the two actions that beat the Opponent's last response. |
| Out-guesser | Faded counts (0.9 per exchange) of the Opponent's responses; predicts the most likely response and plays one of the two actions that beat it, 70/30 with a per-seed preference between the heavy and light one, plus 10% uniform noise. Oracle ceiling about 0.65. |
| Random | Uniform at random (negative control). |
| Big-hit repeater | Uniform, except it replays a Big-hit sequence it landed earlier when the previous action equals that big hit's a0, at least 30 exchanges after it last did so. |
| Burned avoider | Spams one action with 40% noise; avoids the Opening of a big hit it took at that start context with probability 0.5^(dt/100) (half that on a broad match). |
| Level-1, Mirror-1 (scripted) | Only for the level-k/mirror keep rule: they beat the Opponent's predicted best response / most likely response. |

## Pass criteria (heldout, mean over seeds)

- Late prediction accuracy >= chance + 0.20 (chance = 1/6) for Spammer, Cycler and Win-stay/lose-shift.
- Random: mean prediction accuracy in 1/6 +/- 0.05; the evidence gate never opens on any seed (so no Insights and no Recalls); lambda = 0 on every exchange. Mean payoff within 0 +/- 0.15 is a simulator check.
- Big-hit repeater: mixed-forecast top-prediction accuracy on replay exchanges (>= 100 per position, pooled) >= chance + 0.20, for the Opening and each Follow-up.
- Burned avoider: log-loss in burned contexts (>= 100 pooled) beats the config without the Avoidance expert by more than t x paired SE.
- Habit-switch unit test passes.

## Methods used

v1: order 0/1/2 and (action, response) count experts with per-visit fading, mixed by Fixed Share; an evidence gate (test martingale vs uniform, threshold ln 1000); Thompson Sampling once the gate is open. ADR 0011: Big-hit sequence expert and Avoidance expert. v2 (ADR 0007): CTW, longest-match, level-k/mirror, safe exploitation. Each component is kept only if it wins its ablation row on dev seeds.

## Methods not used

- Neural networks and LSTMs: far too little data (100-300 exchanges per session); pretrained models are not allowed.
- Q-learning and deep RL: rewards are one-step, so the Bellman update reduces to a bandit; they need far more data and assume a player who does not adapt.
- MCTS: the game state has no depth to search.
- Epsilon-greedy and UCB1: fixed-rate or deterministic exploration that a human can exploit, and they ignore the forecast. Safe exploitation replaces them.
