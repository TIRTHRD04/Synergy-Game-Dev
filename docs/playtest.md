# Playtest guide

The game is an IMGUI screen built from script, laid out at 1280 x 720 and scaled to the window. No scene or prefab is edited; `Bootstrap` creates the screen when Play starts.

## One-time setup (a teammate with Unity 6)

The repository has `Assets/` but no Unity project settings yet. To make it a Unity project:

1. In Unity Hub, create a new project with Unity 6 (6000.x LTS) and the default 3D template, in any temporary folder.
2. Copy that project's `ProjectSettings/` and `Packages/` folders into the repository root, next to `Assets/`.
3. In Unity Hub, add the repository root as a project and open it. Unity creates `.meta` files for every file under `Assets/`.
4. Check the Console: there must be no compiler errors or warnings. Paste anything you see into a note for review.
5. Commit `ProjectSettings/`, `Packages/` and every new `.meta` file (never `Library/`, `Temp/`, `Logs/` or `UserSettings/`).
6. Record the exact Unity version (from `ProjectSettings/ProjectVersion.txt`) as a dated line in `notes/decisions.md`, and list the packages in `Packages/manifest.json` in `docs/adr/0009-dotnet-test-tooling.md`.

## Tests in Unity

Run both from the repository root (see docs/COMMANDS.md) and paste the raw result files or the Test Runner output:

- EditMode: the same Core and harness tests that run under dotnet, plus the commitment tests.
- PlayMode: `SmokeTests.FullSessionReachesTheSummaryAndWritesThePlayLog` boots the screen, plays a full session on `configs/v2.json` and checks the play log was written.

## Smoke check by hand

1. Press Play. Pick an Opponent config: `v1.json` (baseline), `v2.json` (the tuned Opponent) or `v2-full-experimental.json` (every component, including the ones that lost their ablations; labelled EXPERIMENTAL on screen).
2. The how-to-play overlay opens at the start of a session (H toggles it). Check the six moves and what each beats.
3. Each exchange: the Opponent locks in its move and shows the first 16 characters of its commitment ("proof code"). Press 1-6 (or click a move button). A key counts once per press: release it before the next exchange.
4. After the reveal, the arena shows both moves, who won, the damage and why (the winning move's hint). Check one commitment: take the AI's move number (0-based in ring order: Strike 0, Dodge 1, Grab 2, Heavy 3, Guard 4, Parry 5) and the proof key shown, and run `printf '%s' "<number>|<key>" | sha256sum`. The first 16 characters must match the proof code shown before you acted. The full hash and nonce are also in the play log.
5. While you choose, the AI Brain panel shows only its meters (evidence gate, how often it acts on its read, the pattern it relies on). After each reveal it shows what it expected (forecast bars, chance marked, your move tagged), READ YOU / MISSED / NO READ YET, how it chose its move, and "The AI says" (Commentary, only once the gate is open and the prediction is strong and supported).
6. Play a habit on purpose (for example Grab, Heavy, Strike over and over): the confidence meter should fill, the strip at the bottom should turn red more often, and the learning curve should rise. Then play randomly: the AI should stay near chance.
7. After three matches the summary shows match results, damage, big hits, when the evidence gate opened, the read rate per match against chance, the learning curve and the habits it put into words most often.

## Play logs

Every session writes one CSV to `Application.persistentDataPath/play-logs/` (its full path is shown on the summary screen). On Windows that is under `%USERPROFILE%\AppData\LocalLow\<company>\<product>\`, on macOS under `~/Library/Application Support/<company>/<product>/`.

- Copy logs out into a folder outside the repository, or into `play-logs/` at the repository root, which git ignores. Play logs never go in the repository.
- Teammates' sessions are dev data: they may inform tuning and build the population prior. Sessions by fresh players (interviewers, friends) are the report set: keep them in a separate folder, never use them for tuning, and use them once, to report.
- If the log cannot be written, the screen shows one warning and play continues.

## Builds

A player build reads configs from `StreamingAssets/configs/`. Before building, copy `configs/definition.json` and the Opponent configs into `Assets/StreamingAssets/configs/`. The editor reads `configs/` directly, so no copy is needed for editor play.
