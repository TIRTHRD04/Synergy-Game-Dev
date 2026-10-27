# 0011: The Opponent remembers big hits, and that the player avoids theirs

**Status:** Accepted in part (2026-10-27). Kept: the Big-hit sequence expert with exact (replay accuracy +0.049) and broad matches (+0.244, to 0.829), and the Avoidance expert with exact matches (burned-context log-loss -0.048). Rejected: broad matches for the Avoidance expert (log-loss worse by 0.020). Raw output: experiments/runs/ablation-08 to ablation-11.

A player who takes a nasty string of hits remembers the situation it started from for the rest of the game. This ADR gives the Opponent the same kind of memory, in both directions: it remembers the big hits the player landed on it, and it learns that the player avoids the spots where the player was hit hard. Detecting a big hit is not a rule change; no extra damage is dealt.

## Decision

### Big hit
- A Big hit is three consecutive exchanges in which the same side takes damage every time, totalling at least the big-hit threshold. The window (3) and threshold (7) are game data in `configs/definition.json`. It is flagged on the exchange that completes it; Outcome stays win, draw or loss.
- Counting: scan left to right; after a big hit the next window starts on the following exchange, so windows never overlap. Windows never cross a match boundary.
- Exchange 1 of a match has the Match-start context (one symbol for both the previous action and the previous response).
- Start context: (a0, r0), the previous action and response before the first exchange of the big hit. Broad context: a0 alone. Big-hit sequence: the player's three actions (a1, a2, a3); a1 is the Opening, a2 and a3 the Follow-ups.

### Big-hit sequence expert
- Big-hit memory is a preallocated pool of 16 entries for big hits the player lands on the Opponent: start context, Big-hit sequence, and strengths m_exact = m_broad = 1. A repeat of the same start context and sequence resets the entry's strengths to 1. A full pool replaces the entry with the lowest max(m_exact, m_broad), oldest first on ties.
- It fires on the Opening when the current context equals the start context (exact) or only a0 matches (broad); exact takes precedence, so one entry gives one match per exchange. Follow-ups fire only if the player followed the previous prediction; the first wrong prediction stops the chain. Follow-ups key on the player's own actions and use the Opening's match type.
- Forecast while firing: Dirichlet 0.5 on every action plus s x m_exact pseudo-counts (exact) or (s / 2) x m_broad (broad) on the predicted action; matching entries add up. s is tuned on dev in [0, 5].
- A miss multiplies the chain's strength by 0.7; a hit resets it to 1. An entry is deleted when its strengths are below 0.1 for every match type in use (only m_exact when broad matches are off), which takes 7 misses in a row for each. Nothing fades with time or halves at match start.
- When nothing matches, the expert Abstains. If every expert abstains, the Opponent plays sigma* (uniform) for that exchange: Fixed Share's loss update renormalises only the awake experts, the share update (1 - alpha) w + alpha / K runs over all K experts, and Thompson Sampling draws only among awake experts. The evidence gate still scores the full mixed forecast made before the action, so Ville's bound holds.
- Recall is a Commentary line built from an entry ("Last time we were here you hit me hard, starting with <a1>"; "Last time you followed <a1> with <a2>"). It needs the gate open, the expert firing, and its predicted action as top prediction with p >= 0.4. The miss count at which a Recall stops follows from that cutoff, not from a hard-coded number. Random never opens the gate, so it never gets a Recall.

### Avoidance expert
- The Avoidance record is a pool of 16 entries for big hits the Opponent lands on the player: start context, Opening a1, m_exact, m_broad, with the same merge, replacement and precedence rules.
- While firing it forecasts p(a1) proportional to exp(-beta_avoid x m) with the other actions equal, smoothed by eps = 0.05; beta_avoid is tuned on dev in [0, 3]. It abstains otherwise.
- Wash-out has no reset: when the player plays a1 there anyway, the strength is multiplied by 0.7; when the player avoids a1, it is unchanged. A reset would fire by chance five times in six against random play, so the entry would never fade.

### Population prior buckets
- Five buckets, named by who took the hit: big hit on the player, win, draw, loss, big hit on the Opponent. The big-hit buckets have about 4 events per session, so each shrinks toward its plain bucket (on the player toward win, on the Opponent toward loss): estimate = (counts + 20 x parent) / (n + 20).

## Acceptance

Four dev ablation rows, after the best surviving config (v2 if it ships, v1 otherwise), judged by the rules in notes/decisions.md: +Big-hit sequence expert (exact only), +broad matches for it, +Avoidance expert (exact only), +broad matches for it. A row that does not win is removed from the code; Recall goes with the sequence expert. This ADR then records why.

## Numbers (script output, 2026-10-27)

```
P(window is a big hit) = 0.01852 (4/216 = 0.01852)
per side per 300-exchange session: non-overlapping 4.18, overlapping 5.53, old any-3-damage rule 50
visits per context per session: order-1 50, (action, response) 8.3
m      after     exact   broad
1.000  0 misses  0.5833  0.4444
0.700  1 misses  0.5098  0.3827
0.490  2 misses  0.4407  0.3307
0.343  3 misses  0.3795  0.2887
Recall (p >= 0.4) stops after 3 exact misses, 1 broad misses
deletion: 0.7^6 = 0.118 (kept), 0.7^7 = 0.082 (deleted, < 0.1)
exact: mean 555, median 434 exchanges, mean visits 15.4
broad: mean 111, median 87 exchanges, mean visits 15.4
```

Under uniform play the exact memory has a median life of 434 exchanges, longer than a 300-exchange session; the broad memory washes out sooner (median 87).

## Considered options

- A single heavy hit as the big hit: with the example table that happens on 1 exchange in 6, about 50 times per side per session, so nothing about it would be memorable.
- Extra pseudo-counts in the existing count tables: the memory would fade by visits and halve at match start, so it could not last a session, and it could not follow a sequence.
- A time-based fade: lifetimes would differ wildly between context types (8 versus 50 visits per session) and would not respond to evidence.
