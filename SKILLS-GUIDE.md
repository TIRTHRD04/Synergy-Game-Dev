# Claude Code setup for the adaptive-opponent game (Unity): what each skill is and how to install it

Written for Claude Code (desktop app Code tab or terminal), a Unity project, and a Pro plan. Adjust if yours differs.
Each entry says what the item does, how to install it, how to use it, whether to leave it auto or make it manual,
and what it costs.

---

## 0. Read this first

**What I verified, and what I did not (checked Oct 20, 2026)**
- **Verified on the web today:** Unity's official Claude Code plugin exists and ships 29 skills (names below come
  from Unity's launch post, Sep 9, 2026). Unity's docs page (updated about 5 days ago) says the plugin ships
  **skills only and no MCP server**. Skills that control the Editor run Unity CLI commands, and the CLI and the
  Pipeline package must be set up separately (see Step 1). A separate Unity MCP server is optional. Some press
  coverage and the launch post's wording suggest the plugin includes MCP; the docs page is the more specific
  source, so I went with it. The install commands are from the docs page.
- **Not verified:** the Unity Editor version the CLI and Pipeline package require, whether the 29 skills can be
  installed one by one, and whether each skill is model-invoked (I did not read the skills' `SKILL.md` files).
  Check Unity's docs before committing to a version.
- **Copied from your Pokerbots guide** (which says it verified them on Oct 20, 2026 by cloning the repos): the
  Pocock skill names and install command, `shadcn/improve`, thermo-nuclear, ponytail, and the ECC cherry-pick
  method. I did not re-clone those repos.
- **ECC names for this project come from the inventory file's tables.** That file is internally inconsistent (its
  tables list 460 skills while the headline says 311), so the copy loop below prints `missing:` for any name
  that is not in the repo. Skip those.
- **ECC's `csharp-reviewer`** is described as a .NET reviewer (async/await, nullable references, LINQ). It does
  not know Unity's rules (GC allocations, MonoBehaviour lifecycle, serialization). Those rules live in `CLAUDE.md`.

**Carried over from your Pokerbots guide**
- `benchmark-methodology` is a competitor-analysis step, not a performance tool. Not installed.
- `eval-harness` evaluates verification loops. It is not your archetype simulator. Your harness is your own code.

**What none of this covers.** None of the Unity skills or inventory items implements the learning AI, the
archetype simulator, or the commentary logic. Those are your code, built with the test-first workflow below.

**Safety**
- Skills and plugins are instructions plus sometimes scripts that run with your permissions. Skim each `SKILL.md`.
- Unity's skills run Unity CLI commands against your open Editor. Use them only on this project and read the
  permission prompts. If you also set up the optional Unity MCP server, the same applies. Unity says its skills
  were security reviewed; that is their claim, not mine.
- ECC's README says to install only from the official GitHub repo, the `ecc-universal` npm package, or
  `ecc.tools`. No mirrors.
- `santa-method` was flagged "Risky" by one scanner for credential handling. It is not in this guide.

---

## 1. Before you start

1. Update the app (Check for Updates). Start a session on **Local** with the Unity project repo as the folder.
   The plugin browser does not work in cloud sessions.
2. Prerequisites: `node --version` (18+), `git --version`, Unity Hub with the Editor version recorded in `CLAUDE.md`.
3. Create the repo with a Unity `.gitignore` (Library, Temp, Obj, Logs, UserSettings, Builds) **before the first
   commit**. Commit `.meta` files. Put `CLAUDE.md` and this guide in the repo root.
4. On Windows use Git Bash for the bash snippets, or the PowerShell versions where given.

---

## 2. What to install, in order

### Step 1: Unity's official plugin

**What it is:** Unity's first-party plugin. It teaches Claude how Unity projects are laid out and how engine
systems are meant to be set up. Skills that control the Editor do it by running Unity CLI commands. The plugin is
skills only; it does not include an MCP server.

**What it needs to actually drive the Editor (from Unity's docs):**
1. The Unity CLI installed on your machine.
2. The Pipeline package in your project (`unity pipeline install`).
3. The Unity Editor open.
4. Check with `unity status`. The separate, optional MCP server is set up with `unity mcp configure`.

**Unity's own limitations note:** for simple uGUI work the plugin makes little difference, and on Sonnet 5 it mainly
supplies correctness, because Sonnet 5 builds most Unity features without it but defaults to older approaches. So
it is a "use the modern API" helper, not a requirement.

**Install (prompt box or terminal):**
```
/plugin marketplace add Unity-Technologies/unity-agent-plugin
/plugin install unity@unity-agent-plugin
```
Pick a scope when asked: **project** if your team should share it, **local** for yourself in this repo only (user
scope is the default). In the desktop app you can also use Plugins, then Browse, and search for Unity; if it is not
listed, add the marketplace by URL `https://github.com/Unity-Technologies/unity-agent-plugin`, then Sync.
To verify, type `/unity:` to list the skills and `/plugin` to confirm `unity` is enabled. The skills appear under
the `unity:` prefix, so the exact name may be `/unity:unity-cli`; the short names below are from Unity's launch post.
To update: `/plugin marketplace update unity-agent-plugin`. If skills do not appear, Unity says to clear
`~/.claude/plugins/cache`, restart and reinstall (or run `/reload-plugins`).

**The 29 skills, sorted for this project**

| Skill | What it does (short) | For this game |
|---|---|---|
| `/new-unity-project` | Guided new-project flow (concept, platforms, packages, source control). Does not scaffold gameplay code | **Use once** at the start |
| `/unity-cli` | Drives the Editor from the terminal: scenes, GameObjects, assets, run C#, builds, logs | **Core.** Main build/verify tool |
| `/unity-package-management` | Add/remove/upgrade UPM packages, headless and CI installs | **Use** (Test Framework, UI packages) |
| `/ui` | Detects the project's UI system, hands off to the right one | **Use** for overlay, HUD, summary |
| `/ui-uitk` | UI Toolkit (UXML/USS, UIDocument, runtime binding), Unity 6.0+ | **Use** if UI Toolkit; best fit for the overlay |
| `/ui-ugui` | Canvas-based uGUI | Only if you choose uGUI |
| `/ui-imgui` | IMGUI for editor tooling | Skip |
| `/optimize-text-mesh-pro` | TextMeshPro fonts, atlases, SDF, CJK | Maybe, if commentary text looks bad |
| `/optimize-web` | Smaller/faster WebGL and WebGPU builds | Maybe, if judges play in a browser |
| `/build-live-game` | Unity Services backend: leaderboards, cloud save, remote config, analytics | Maybe (see warning) |
| `/physics-3d-collision` | Fixes PhysX collision/trigger problems | Only if 3D and you hit such bugs |
| `/urp-postprocessing` | URP Volume effects | Polish only |
| `/audio-setup-mixers`, `/optimize-audio` | Mixer routing, audio import settings | Polish only |
| `/initialize-ai-navigation` | NavMesh surfaces/agents (pathfinding, not learning) | Skip unless the game needs pathing |
| `/2d-pixel-perfect`, `/sprite-editor`, `/sprite-segment-3x3grid`, `/manage-sprite-atlas`, `/tilemap-palette-create`, `/tilemap-ruletile-createempty`, `/tilemap-ruletile-createfromsegment` | 2D sprite and tilemap tooling | Skip unless pixel-art 2D |
| `/shader-graph-create-custom-node`, `/validate-urp-render-graph-renderer-feature` | Shader and renderer-feature work | Skip |
| `/implement-in-app-purchases`, `/levelplay-unity-integration` | IAP and ads | Skip |
| `/setup-vivox-voice-chat`, `/setup-multiplayer-services` | Voice chat, multiplayer services | Skip (you play against an AI) |
| `/localization` | Unity Localization | Skip |

**Warning on `/build-live-game`:** it adds accounts, a backend and setup time, and a backend that is down during
judging breaks your demo. Leave it out unless Opus's plan calls for it, and then add it only after everything
else works.

**Auto or manual:** by default a Claude Code skill's description sits in context and Claude invokes it when your
request matches, so I expect these to behave as auto. I did not read their `SKILL.md` files to confirm, and I could
not confirm you can install them one by one. Files managed by a plugin may be overwritten on update (also not
confirmed), so do not rely on editing them. In a Unity-only project, auto is what you want: they fire on Unity
tasks and stay quiet otherwise.
**Cost:** the descriptions for all 29 skills, every session. Run `/context` right after installing and again after
a long session. If the overhead hurts, disable the plugin in sessions that do not touch the engine.

---

### Step 2: Matt Pocock's skills (the backbone)

**What it is:** small composable engineering skills: sort out what you want before building (grilling), then
test-first loops and structured debugging instead of letting the agent guess.

**Install (terminal):**
```
npx skills@latest add mattpocock/skills
```
- Choose **Claude Code** as the agent and **project scope**.
- **Pick only the skills in the table.** Installing everything adds descriptions Claude scans every session.

**Run once per repo:** `/setup-matt-pocock-skills`. When it asks where docs go, keep the glossary at the repo
root (`GLOSSARY.md`) and decisions in `docs/adr/`, where `CLAUDE.md` expects them. Choose local files for the issue tracker.

| Skill | What it does | Recommended |
|---|---|---|
| `setup-matt-pocock-skills` | One-time repo configuration | Install, run once |
| `grill-with-docs` | Relentless interview, and it writes a glossary (`GLOSSARY.md`) and decision records as it goes | **Use once at the start** to lock the action set, response set, pass criteria and glossary |
| `grill-me` | Same interview without the doc side effects | Alternative to the above |
| `tdd` | Red, green, refactor | **Make manual.** Use for the learning code, the harness, saved-profile handling |
| `diagnosing-bugs` | Reproduce, minimise, hypothesise, instrument, fix | Auto is fine; triggers on bug reports |
| `codebase-design` | Vocabulary for deep modules and seams | Auto, low cost |
| `code-review` | Review of changes against standards and spec | **Make manual** |
| `handoff` | Compacts the conversation into a handoff document | Manual. Writes to `notes/handoff.md` |
| `research` | Investigates a question against sources and saves findings | **Make manual** (conflicts with the "no research detours" rule) |
| `improve-codebase-architecture` | Scans for architecture improvements, HTML report, grills you through one | Manual, after the game works |
| `git-guardrails-claude-code` | Hooks that block dangerous git commands | Recommended, cheap safety net |
| `ask-matt` | Router that tells you which skill fits | Optional |

**Skip:** `setup-pre-commit` and `migrate-to-shoehorn` (JS/TS), `scaffold-exercises`, `wizard`, `pr`, `triage`,
`wayfinder`, `implement`, `implement-spec`, `teach`, `to-questionnaire`, `wait-what`, `writing-for-agents`,
`retro`, `prototype`, `domain-modeling`, `grilling`, and everything in `in-progress`. Also skip `to-spec` and
`to-tickets` unless the team splits work across more people and wants tickets; if so, run them once.

**Make a skill manual-only:** add `disable-model-invocation: true` to the frontmatter of its `SKILL.md`. Do this
for `tdd`, `code-review` and `research`.
**Cost:** grilling is many back-and-forth turns by design. Use it once, not per task.

---

### Step 3: `shadcn/improve` (audit and plan)

**What it is:** audits a codebase and writes plans for other agents to execute. It writes plan files only and
never changes source.

**Install:** `npx skills add shadcn/improve`

**Use:** `/improve quick` once the core works, `/improve branch` before submission. Skip `deep` and `execute` on
a Pro plan. I have not checked how well it handles Unity-specific problems (serialization, lifecycle), so verify
its findings.
**Auto or manual:** manual. **Cost:** `quick` is about 5 minutes per the author's own estimate.

---

### Step 4: The review stack for judges

**Thermo-nuclear (from Cursor's plugins repo):**
```
npx skills add https://github.com/cursor/plugins --skill thermo-nuclear-code-quality-review
npx skills add https://github.com/cursor/plugins --skill thermo-nuclear-review
```
- `thermo-nuclear-code-quality-review`: very strict maintainability review of your branch (abstractions, giant
  files, tangled conditions). Treat its findings as a menu, not a to-do list. It is manual by design.
- `thermo-nuclear-review`: audit of a branch's diff for bugs, breaking changes and security problems. The better
  pre-submission check.
- Skip `thermos` (runs both in parallel, most expensive). Whether these work well on C# is unverified.

**ponytail (write less code):**
```
/plugin marketplace add DietrichGebert/ponytail
/plugin install ponytail@ponytail
```
If the commands are not recognized in the desktop app, add it through Customize, then Plugins, then add a
marketplace from repository: `DietrichGebert/ponytail`.
- Auto, at lite or full. Use `/ponytail-review` manually for a deletion pass before submission.
- Keep it off ultra; it will fight you when you write the real model and harness.
- Its benchmark numbers are the author's own.

---

### Step 5: ECC cherry-picks (copy files, do not install the plugin)

**Why copy:** installing the whole plugin advertises the full catalog every session, and a known desktop bug means
plugin `commands/*.md` files do not run as slash commands in the Code tab (skills do). Skills must sit directly
under `~/.claude/skills/`. Do not also run ECC's full installer.

**macOS, Linux, or Git Bash:**
```
git clone --depth 1 https://github.com/affaan-m/ECC.git && cd ECC
mkdir -p ~/.claude/skills ~/.claude/agents
for s in verification-loop strategic-compact coding-standards benchmark-optimization-loop; do [ -d skills/$s ] && cp -r skills/$s ~/.claude/skills/ || echo "missing: $s"; done
for a in planner architect csharp-reviewer code-reviewer silent-failure-hunter performance-optimizer refactor-cleaner; do [ -f agents/$a.md ] && cp agents/$a.md ~/.claude/agents/ || echo "missing: $a"; done
```

**Windows PowerShell:**
```
git clone --depth 1 https://github.com/affaan-m/ECC.git; cd ECC
New-Item -ItemType Directory -Force $HOME\.claude\skills,$HOME\.claude\agents | Out-Null
foreach($s in 'verification-loop','strategic-compact','coding-standards','benchmark-optimization-loop'){ if(Test-Path skills\$s){Copy-Item -Recurse skills\$s $HOME\.claude\skills\} else {"missing: $s"} }
foreach($a in 'planner','architect','csharp-reviewer','code-reviewer','silent-failure-hunter','performance-optimizer','refactor-cleaner'){ if(Test-Path agents\$a.md){Copy-Item agents\$a.md $HOME\.claude\agents\} else {"missing: $a"} }
```
Any `missing:` line means that name is not in the repo. Skip it.

**Skills (4)**

| Skill | What it does | Auto or manual |
|---|---|---|
| `coding-standards` | General language best-practice guidance | Auto, low cost |
| `verification-loop` | Build, lint, test until green, with proof | **Manual**, medium cost |
| `strategic-compact` | Suggests when to compact context | **Manual**, low cost |
| `benchmark-optimization-loop` | Baseline, one change at a time, correctness gate, keep the fastest safe version | **Manual**. Use when the decision path misses its ms budget |

**Agents (7)**, invoked by name only (do not mention them in prompts you do not want them used in):

| Agent | When to use |
|---|---|
| `planner` | Big or risky changes |
| `architect` | Before restructuring |
| `csharp-reviewer` | Review of new C# code (it does not know Unity-specific rules; those are in `CLAUDE.md`) |
| `code-reviewer` | Pre-submission review |
| `silent-failure-hunter` | Pre-submission. Swallowed exceptions and anything that could crash mid-demo |
| `performance-optimizer` | When the decision path misses its budget or allocates per frame. Part of its prompt is about JS bundles; ignore it |
| `refactor-cleaner` | Only after the game works: dead code before judges read it |

**Optional, names from the inventory only (unverified):** `pr-test-analyzer` (audits tests for missing assertions;
useful for the harness tests) and `type-design-analyzer` (reviews type design).

---

### Step 6: Restart and verify
1. Quit and reopen the app. Start a **new** Local session on the repo.
2. Type `/` in the prompt box. You should see the Pocock skills, `/improve`, the ponytail commands and the ECC
   skills. Type `/unity:` for the Unity skills.
3. If something is missing, check that it landed directly under `~/.claude/skills/` or `.claude/skills/`.
4. Run `/context` and note the baseline.

---

## 3. Which skill owns which phase (do not mix owners)

| Phase | Owner | Notes |
|---|---|---|
| 0. Setup | `/new-unity-project`, `/setup-matt-pocock-skills`, `git-guardrails-claude-code` | Once |
| 1. Requirements | `grill-with-docs` | Produces `docs/CONTEXT.md` (actions, responses, pass criteria), `GLOSSARY.md` and ADRs |
| 2. Plan | Opus in Plan Mode (`planner` / `architect` agents optional) | Opus designs; the approved plan goes in `docs/adr/` |
| 3. Learning code + harness | Pocock `tdd`, `codebase-design`, `csharp-reviewer` | Test-first, held-out seeds |
| 4. Game scene + UI | `/unity-cli`, `/ui` then `/ui-uitk`, `/unity-package-management` | Validate with `docs/playtest.md` |
| 5. Bugs | `diagnosing-bugs` | One owner. Skip ECC `root-cause-analysis` |
| 6. Speed | `performance-optimizer`, `benchmark-optimization-loop` | Only if the budget is missed |
| 7. Pre-submission | `/improve branch`, `thermo-nuclear-review`, `code-reviewer`, `silent-failure-hunter`, `/ponytail-review`, `refactor-cleaner` | Then `gitleaks detect` |
| Session handoff | Pocock `handoff` into `notes/handoff.md` | |

**Overlaps resolved:** TDD is Pocock `tdd` (skip ECC `tdd-workflow` and `tdd-guide`); handoff is Pocock `handoff`
(skip `truongfelix/workflow`); bug diagnosis is `diagnosing-bugs`; reviews go in the order above.

---

## 4. Left out on purpose

- **`truongfelix/workflow` (`adopt.sh`):** built for many parallel sessions. `CLAUDE.md` already has `notes/` files and
  Pocock has `handoff`. If you want it anyway, run `adopt.sh` after `CLAUDE.md` is in place (it never overwrites
  existing files) and drop Pocock `handoff`.
- **`santa-method`, GAN loops, multi-model pipelines, `/loop` overnight runs:** token-heavy, and the project is
  small.
- **`search-first`, Pocock `research`:** conflict with the no-research-detours rule. Run manually if needed.
- **ML skills (PyTorch, LoRA, TensorRT and similar):** not installed. Revisit only if Opus's plan needs them.
- **Headroom:** only if your harness logs make sessions huge. I have not verified it for this setup.

---

## 5. Auto vs manual summary

**Auto (leave as installed):** `CLAUDE.md`, the Unity skills (they trigger from their descriptions), ponytail
(lite or full), `coding-standards`, Pocock `diagnosing-bugs` and `codebase-design`.

**Manual (invoke by name only when needed):**
- Pocock: `grill-with-docs`, `grill-me`, `tdd`, `code-review`, `research`, `handoff`, `improve-codebase-architecture`
- `/improve quick`, `/improve branch`
- thermo-nuclear code quality review, `thermo-nuclear-review`
- `verification-loop`, `strategic-compact`, `benchmark-optimization-loop`
- Agents: all seven above

---

## 6. If you are on the desktop app

- **Plan Mode:** mode selector (Ctrl/Cmd+Shift+M). Shift+Tab does not cycle modes in Desktop.
- **Model:** model picker (Ctrl/Cmd+Shift+I). Unconfirmed whether `opusplan` exists there. If not, plan on Opus, then
  switch to Sonnet to build.
- **`!`, Ctrl+Z, Ctrl+S:** CLI shortcuts. Use the terminal pane (Ctrl+`) instead.
- **Parallel work:** each session has a worktree option for an isolated copy of the project. A Unity project makes
  heavy worktrees (Library is rebuilt per copy), so only use it when you need it.

## 7. Undoing things
- **ECC cherry-picks:** delete the folders from `~/.claude/skills/` and the `.md` files from `~/.claude/agents/`.
- **Pocock, `shadcn/improve`, thermo-nuclear:** delete the folders from the skills directory the installer used.
- **Unity plugin and ponytail:** remove them from the plugin list in the app (Customize, Plugins).
- **Switching engines:** I did not research Godot plugins. If you switch, Step 1 and the engine-specific parts of
  `CLAUDE.md` (`.meta`, the unity-cli skill) are what change; the rest carries over.

## Sources
- Unity plugin docs: https://docs.unity.com/en-us/ai/unity-plugin/claude-code
- Unity launch post (Sep 9, 2026): https://unity.com/blog/unity-plugin-for-claude-code
- Your Pokerbots SKILLS-GUIDE.md (Pocock, improve, thermo-nuclear, ponytail, ECC methods)
- CLAUDE_AGENT_ECOSYSTEM_INVENTORY.md (ECC names and descriptions)
- Claude Code desktop docs: https://code.claude.com/docs/en/desktop
