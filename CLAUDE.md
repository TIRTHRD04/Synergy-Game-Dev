# <GAME NAME> - 1v1 game with a learning AI opponent (Unity)

Goal: the AI opponent learns the player's habits during a match, predicts their next action, picks a counter from
a fixed response set, and carries what it learned into later matches of the same session (3 matches, about
100-300 exchanges).
Success = (1) the AI measurably adapts (test harness), (2) the player can SEE it adapt (commentary, belief
overlay, end-of-session summary), (3) the source holds up under review by technical judges.
Unity version: Unity 6 (6000.x LTS).
Design is not fixed in this file. Opus proposes it in Plan Mode; the approved design goes in docs/adr/.

Team: <n> people, coordinating outside Claude. Claude cannot see that chat. Anything decided there that affects
code or rules must be pasted into notes/decisions.md (one dated line) or it does not exist.

Read first: GLOSSARY.md, docs/CONTEXT.md (game rules, action set, response set), notes/decisions.md,
notes/lessons.md, docs/COMMANDS.md (build/test commands).
If a fact (actions, responses, rules, Unity version, API) is not in those files, ask. Never guess.

## Hard rules (never break)
- NEVER put learning, prediction or decision logic in MonoBehaviours or scene code. The AI core is pure C#
  (its own asmdef, no UnityEngine reference) so it runs in plain unit tests and the headless simulator.
- NEVER tune the AI on the seeds you report. Tune on seeds/dev/, report on seeds/heldout/ (committed once, never
  edited). "Feels right in a playtest" is not evidence on its own.
- NEVER claim "tests pass", an accuracy number or an adaptation result without showing the raw command output.
- NEVER show commentary or overlay text that is not derived from what the AI actually learned. No canned lines
  without data behind them.
- NEVER let the AI claim a pattern against random play (random is a negative control in the harness).
- NEVER script or fake adaptation in any mode, demo included. A demo profile may change parameters only. It must
  pass the same harness, be logged as its own config, and be labeled on screen.
- NEVER guess Unity APIs, package versions or file contents. Read the file or the docs first.
- NEVER start research detours (doc crawls, new packages, asset-store downloads) without asking first.
- NEVER hand-edit .unity/.prefab YAML; use the Unity plugin's unity-cli skill. NEVER commit Library/, Temp/,
  Obj/, Logs/, UserSettings/, builds or keys. ALWAYS commit .meta files together with their assets.
- NEVER add a package or dependency without a short entry in docs/adr/ (judges read the dependency list).
- NEVER make network calls from the AI. A pretrained table (for example the population prior, ADR 0008) may ship
  only as a bundled config file, and only if it beats the no-prior config in the harness and on teammates' play
  logs. The human report set is only ever used to report, never to decide. The AI must run offline and be deterministic for a given seed.

## Conventions
- Reproducibility: one global seed in configs/. All randomness is seeded. Every run is reproducible from
  config + git hash.
- A missing, old or corrupt saved profile must never crash the game. (Dormant: no saved profiles for now.)
- Performance: the AI decision path has a budget of 1 ms per decision (Decide + Observe) and no per-frame heap allocations in
  per-frame code. Measure with the profiler before claiming anything.
- Harness: simulated player archetypes (listed in docs/CONTEXT.md) validate adaptation; pass criteria live there
  too. Report per archetype and per match phase, with learning curves (accuracy vs actions seen), never only a
  global average.
- Every sim run appends one row to experiments/log.csv: name, git hash, config, seeds, per-archetype metrics,
  runtime, notes.
- Code for reviewers: files under ~400 lines, no dead or commented-out code, no stale TODOs at submission, XML
  docs on public types. Use the terms in GLOSSARY.md exactly; no synonyms for the same concept.

## Workflow
- Plan Mode first for anything non-trivial. One task per session; commit, then /clear. Run /context early.
  Compact at milestones, not mid-refactor.
- The learning code and the harness are test-first (/tdd). Scene and UI work is
  validated by docs/playtest.md and a run in the editor, not unit tests.
- After a non-obvious bug, add a line to notes/lessons.md before closing the session.
- Decisions go in docs/adr/. Report the file path as proof. "Remember this" is not a record.
- Never trust "it works": get an independent review (csharp-reviewer or code-reviewer) before merging big changes.

## Before saying "done"
1. EditMode + PlayMode tests, using the command in docs/COMMANDS.md
   (batchmode example: Unity -batchmode -runTests -testPlatform EditMode -projectPath . -testResults results.xml)
2. Archetype suite on seeds/heldout/, raw output shown
3. Smoke check: a full match runs in the editor, the summary screen shows after 3 matches
4. A row exists in experiments/log.csv for this run; no new compiler warnings

## Repo map
Assets/Scripts/Core/      pure C#: definition, experts, predictor, evidence gate, response selector (own asmdef, no UnityEngine)
Assets/Scripts/Core.Sim/  archetypes and runner (usable headless)
Assets/Scripts/Game/      match flow, input, scene wiring
Assets/Scripts/UI/        Belief, Commentary, end-of-session summary
Assets/Tests/             EditMode/ PlayMode/
configs/  experiments/  seeds/  docs/ (CONTEXT.md, COMMANDS.md, playtest.md, adr/, agents/)  notes/
GLOSSARY.md  .scratch/<feature>/ (specs and issues)

## Skills
See SKILLS-GUIDE.md. One owner per phase. Do not invoke overlapping workflows.

## Agent skills

### Issue tracker

Local markdown: issues and specs live as files under `.scratch/<feature>/` in this repo. See `docs/agents/issue-tracker.md`.

### Domain docs

Single-context: one `GLOSSARY.md` and `docs/adr/` at the repo root. See `docs/agents/domain.md`.
