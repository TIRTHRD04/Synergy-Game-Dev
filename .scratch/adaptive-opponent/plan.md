# Plan: adaptive Opponent (v1 then v2), archetype harness, bare-bones Unity screen

Status: approved after grilling (2026-10-27). Decisions: notes/decisions.md. ADRs 0002-0008. v2 spec: .scratch/opponent-v2/spec.md.

## Build order (one commit per milestone, branch claude/kind-mccarthy-ckci1c)
1. Setup: .NET 8 in the container, tools/dotnet csproj files, docs/CONTEXT.md (actions, responses, rules, Damage table, 7 archetypes, pass criteria, methods and "not used"), docs/COMMANDS.md, docs/open-questions.md (combo rule, end-of-match lookahead), ADR 0009 (test-only NUnit, dotnet tooling; written in this step), .gitignore.
2. Definition loader with the two-part balance check (ADR 0010), seeded Rng, big-hit detector, IPlayer, 7 archetypes (Spammer, Cycler, Win-stay/lose-shift, Out-guesser, Random, Big-hit repeater, Burned avoider) plus scripted Level-1 and Mirror-1 players, with oracle ceilings, Session loop (fixed-length harness mode and game-rules mode), sim runner against a uniform Opponent. Test-first.
3. v1 predictor: order 0/1/2 and (action, response) experts with per-visit fading, Fixed Share mixing with abstaining experts, evidence gate, habit-switch test. Then ADR 0011: Big-hit memory and the Avoidance record (fixed pools of 16), the Big-hit sequence expert, the Avoidance expert, Recall, with tests pinned to the ADR's numbers.
4. v1 selector and Opponent: uniform while the gate is closed, Thompson Sampling once open; Opponent facade, Belief, Insights, Commentary, end-of-session summary.
5. Tune on dev seeds (start HP, gamma, alpha, k0, s, beta_avoid); fix the gate threshold with Random on >= 100 dev seeds once the final expert set is chosen.
6. Unity: Bootstrap, DuelScreen (IMGUI, key-up rule, commitment hash), HumanPlayer, asmdefs, play log, docs/playtest.md. Compiled and played by the team in Unity 6, not in the container. From here on, teammates play it so play logs collect while the next steps are built.
7. v2, one component at a time on paired dev seeds, one log.csv row each: CTW x2 (action-first, response-first, clamped log-odds), longest-match, level-k/mirror (own keep rule), safe exploitation (k0 = 0.75), optional CTW at 3 gammas, global-time fading row. p50/p99 decision time reported.
8. Population prior (ADR 0008): build configs/prior.json from teammates' play logs; decide by leave-one-player-out on teammates only.
9. One heldout run reporting v1, freq, v2 and the prior (whatever survived), run twice to show identical numbers, raw output shown.
10. Human data: analyze-human command in Core.Sim reads play logs and reports the learning curve (bootstrap over players), accuracy from exchange 31, log-loss vs uniform, gate-open rate and a per-player table; one log.csv row per run. Fresh players = report set, >= 8 players before any claim, used once.
11. Independent review, fixes.
