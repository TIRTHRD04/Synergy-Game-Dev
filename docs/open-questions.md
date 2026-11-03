# Open questions

- **Combo rule**: damage that grows over a streak of hits (for example x1, x1.5, x2). Not built: it would make the best response depend on the current streak, so the Opponent would need lookahead and per-streak payoffs. A Big hit (ADR 0011) only detects strings of hits; it deals no extra damage.
- **End-of-match lookahead**: the selector maximises expected damage per exchange. When either side's HP is at most the maximum damage, one heavy hit decides the match, so expected damage is not win probability. Expectimax over 2-3 exchanges would fix it (ADR 0007). The harness is unaffected because HP never ends a harness match.
- **Theme**: actions, responses and their names are placeholders until a theme is chosen (GLOSSARY.md).
