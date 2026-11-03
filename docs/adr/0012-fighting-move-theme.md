# 0012: Fighting-move theme, and the Belief shown only after the reveal

**Status:** Accepted (2026-11-03).

Playtesting showed that nobody could follow the game: the six actions and six responses were placeholders ("Action 1" to "Response 6"), and the screen did not show why an exchange was won or how the Opponent was adapting. This ADR gives the game a theme and changes what the screen shows, and when. It changes no rule and no number in the Opponent.

## Decision

### Theme: six fighting moves on the existing ring
- Both sides pick from the same six moves, in ring order: **Strike, Dodge, Grab, Heavy, Guard, Parry** (indices 0-5). `configs/definition.json` uses these names for both `actions` and `responses`.
- The outcome matrix and Damage table are unchanged (ADR 0010): move m beats move m+1 for 3 damage ("clean hit") and move m+2 for 1 damage ("glancing hit"), draws with itself and with move m+3, and loses to moves m-1 and m-2 (mod 6). Because the data is unchanged, every test, tuning result and experiments/log.csv row stays valid; the dev suite row `fighting-theme-check` reproduces `v2-dev-suite` exactly (timing aside).
- The order was chosen so each matchup reads as a fighting-game rule: Dodge slips a Grab, Grab catches a slow Heavy, Heavy breaks a Guard, Guard leaves a Parry nothing to deflect, Parry punishes a Strike, Strike catches a Dodge as it recovers.
- The sixth move is named Parry, not Counter: "Counter" is already a glossary term (a response that beats an action), and screen text such as "its counter: Counter" would be ambiguous.
- Because both sides use the same move set, a later player-vs-player mode needs no rule change.
- `definition.json` gains an optional `actionHints` array (one line per move). `DefinitionParser` reads it; a missing array gives empty hints, a wrong length is a `DefinitionException`.

### What the screen shows, and when
- **While the player chooses**, the screen shows only meters that do not name a move: evidence gate progress, lambda (how likely the Opponent is to play its best response this exchange) and the leading expert in plain words.
- **After the reveal**, the screen shows the Belief for the exchange just played: the full forecast with chance marked, whether its top prediction matched (a "read"), how it chose its response, and the Commentary. This replaces the 2026-10-27 decision to show the Belief from the first exchange while the player chooses: a visible forecast tells the player which response is coming (the best response to the top prediction), so a player could beat the Opponent by reading the panel rather than by playing.
- The Opponent reports how it chose each response (`ResponseKind`: Random, BestResponse, SampledBestResponse) and its `PlannedResponse` (the best response to the mixed forecast), so the "how it chose" line is read from the Opponent, never inferred by the UI.
- A forecast where every move is equally likely is shown as "no read yet", not as a prediction of the tie-broken move.
- The end-of-session summary shows the read rate per match against chance, the learning curve per 10 exchanges, when the gate opened, and the Commentary lines shown most often.

## Consequences
- No new packages; the screen is still IMGUI built from script, laid out at 1280 x 720 and scaled to the window.
- The on-screen read rate counts an exchange with a flat forecast as not read, so it can be slightly below Prediction accuracy (which the harness and play log keep as defined).
- Play logs keep action and response indices, so analysis is unaffected by the names.
