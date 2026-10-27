# Plan: adaptive Opponent (v1 then v2), archetype harness, bare-bones Unity screen

Status: approved after grilling (2026-10-27). Decisions: notes/decisions.md. ADRs 0003-0007. v2 spec: .scratch/opponent-v2/spec.md.

## Build order (one commit per step, branch claude/vigilant-ramanujan-3t7kvy)
1. Setup: .NET 8 in the container, tools/dotnet csproj files, docs/CONTEXT.md (actions, responses, rules, 5 archetypes, pass criteria, methods and "not used"), docs/COMMANDS.md, docs/open-questions.md (lookahead), ADR 0008 (test-only NUnit, dotnet tooling), .gitignore.
2. Definition loader with balanced-matrix and Nash checks, seeded Rng, IPlayer, 5 archetypes (Spammer, Cycler, Win-stay/lose-shift, Out-guesser, Random) with oracle ceilings, Session loop, sim runner against a uniform Opponent. Test-first.
3. v1 predictor: order 0/1/2 and (action, response) experts with per-visit fading, Fixed Share mixing, evidence gate, habit-switch test.
4. v1 selector and Opponent: uniform while the gate is closed, Thompson Sampling once open; Opponent facade, Belief, Insights, Commentary, end-of-session summary.
5. Tune v1 on dev seeds; fix the gate threshold with Random on >= 100 dev seeds.
6. v2, one component at a time on paired dev seeds, one log.csv row each: CTW x2 (action-first, response-first, clamped log-odds), longest-match, level-k/mirror (own keep rule), safe exploitation (k0 = 0.25), optional CTW at 3 gammas, global-time fading row. p50/p99 decision time reported.
7. One heldout run reporting v1, freq and v2, run twice to show identical numbers, raw output shown.
8. Unity: Bootstrap, DuelScreen (IMGUI, key-up rule, commit hash), HumanPlayer, asmdefs, manual play CSV log, docs/playtest.md. Compiled and played by the team in Unity 6, not in the container.
9. Independent review, fixes.
