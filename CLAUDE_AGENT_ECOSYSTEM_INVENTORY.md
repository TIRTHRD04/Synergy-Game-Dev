# The Unified Claude Code & AI Agent Ecosystem Inventory
*A Comprehensive, Self-Contained Reference Manual of Architectures, Frameworks, Skills, Commands, and Context Systems*

---

## Table of Contents
1. [Executive Architectural Overview: How the Pieces Connect](#1-executive-architectural-overview-how-the-pieces-connect)
2. [Claude Code Native Engine & Core Primitives](#2-claude-code-native-engine--core-primitives)
   - 2.1 The Agent Harness Runtime
   - 2.2 Model Routing & `opusplan`
   - 2.3 Native Plan Mode (`Shift + Tab`), The "Test-First Plan Gate" & Type-Check Mandate
   - 2.4 Prompt Caching Mechanics & The 1-Hour Cold Cache Window
   - 2.5 Subagent Delegation (`CLAUDE_CODE_SUBAGENT_MODEL`)
   - 2.6 Dynamic Custom Slash Commands (`.claude/commands/`)
   - 2.7 Automated Permission Controls (`settings.json`)
   - 2.8 Native Shell Mode (`!` Prefix, `Ctrl + Z` Suspend & `Ctrl + S` Stash)
   - 2.9 Context Hygiene Primitives: `/context`, `/compact`, `/clear` & The 100k–150k Ceiling Rule
   - 2.10 Autonomous Scheduled Loops: `/loop` & `/goal` (Mechanics, Traps & Relevance)
3. [ECC (Everything Claude Code) Framework Deep-Dive](#3-ecc-everything-claude-code-framework-deep-dive)
   - 3.1 What ECC Is and Its Architecture
   - 3.2 Modular Install Profiles & Deployment Topologies
   - 3.3 The Complete 71 Specialized Subagents Directory
   - 3.4 The 311 Skills Complete Inventory & Domain Taxonomy
   - 3.5 The Complete 97 Slash Commands Catalog
   - 3.6 The Modular Rules Engine (122 Rules Across 22 Stacks)
   - 3.7 The Lifecycle Hooks Runtime & AgentShield Security System
   - 3.8 Multi-Agent Verification Loops & Cross-Model Workflows
   - 3.9 Memory Persistence, Instincts & Continuous Learning Engine
   - 3.10 Infrastructure Bridges & Orchestration
4. [Matt Pocock's Agent Skills Suite (`aihero.dev`)](#4-matt-pococks-agent-skills-suite-aiherodev)
   - 4.1 The Core Thesis: Overcoming the 4 Agent Failure Modes
   - 4.2 The "Idea-to-Ship" Lifecycle Pipeline
   - 4.3 Socratic Alignment: `/grill-me` and `/grill-with-docs`
   - 4.4 Domain Modeling & Ubiquitous Language (`GLOSSARY.md` & ADRs)
   - 4.5 Spec-Driven Development: `/to-spec` and `/to-tickets`
   - 4.6 Verification & Diagnostics: `/tdd` and `/diagnosing-bugs`
   - 4.7 Codebase Design & Deep Modules
   - 4.8 Complete Catalog of Matt Pocock's 38 Skills
5. [The "Thermo-Nuclear Code Quality Review" Skill](#5-the-thermo-nuclear-code-quality-review-skill)
   - 5.1 Origin and Philosophy
   - 5.2 The Concept of "Code Judo"
   - 5.3 Deepening Opportunities & The Deletion Test
   - 5.4 The Architecture Review HTML Dashboard
   - 5.5 Execution Workflow & Grilling Loop
6. [Headroom Context Compression Layer](#6-headroom-context-compression-layer)
   - 6.1 Purpose & "Live Zone" Optimization
   - 6.2 Deployment Modes (`headroom wrap claude`, Proxy, MCP Server)
   - 6.3 Compression Mechanics (AST-Aware Code Compression, JSON/Log Crushing)
   - 6.4 The Breadcrumb Hash Retrieval System
7. [Anthropic's Official Agent Skills Standard (6 Core Insights)](#7-anthropics-official-agent-skills-standard-6-core-insights)
   - 7.1 Insight 1: Gerund Naming Conventions
   - 7.2 Insight 2: Single-Level Deep References
   - 7.3 Insight 3: Front-Loaded Table of Contents
   - 7.4 Insight 4: Evaluation-Driven Development (EDD)
   - 7.5 Insight 5: Description as the Discovery Product
   - 7.6 Insight 6: Progressive Disclosure (The 3-Tier Context Hierarchy)
   - 7.7 Cross-Model Parity Testing
8. [The September 2026 Skills Leaderboard Catalog](#8-the-september-2026-skills-leaderboard-catalog)
   - 01. `DietrichGebert/ponytail` (The Pragmatic Senior / YAGNI Ladder)
   - 02. `mattpocock/skills` (Engineering Rigor Suite)
   - 03. `ayghri/i-have-adhd` (High-Signal, Action-First Output Format)
   - 04. `cloudflare/security-audit-skill` (Vulnerability Scanning)
   - 05. `obra/superpowers` (Disciplined SDLC & Subagent Orchestration)
   - 06. `cathrynlavery/diagram-design` (Mermaid & Visual Systems)
   - 07. `blader/humanizer` (De-AI Prose Filter)
   - 08. `Graphify-Labs/graphify` (Graph-Based Knowledge Persistence)
   - 09. `addyosmani/agent-skills` (Production-Grade Lifecycle Management)
   - 10. `pbakaus/impeccable` (Frontend Precision & Craft Polish)
   - 11. `shadcn/improve` (The "Plan is the Product" / Brain-Worker Engine)
9. [Community Session Persistence, Heuristics & Multi-Model Workflows](#9-community-session-persistence-heuristics--multi-model-workflows)
   - 9.1 `truongfelix/workflow` (Session Continuity, Questions Archive & Agent Collision Control)
   - 9.2 The 13 Battle-Tested Heuristics (1-Year Daily Power User Protocol)
   - 9.3 Caveman Mode: Ultra-Terse Token Pruning (`JuliusBrussee/caveman`)
   - 9.4 Multi-Model Division of Labor: Gemini + Claude Symbiosis
   - 9.5 The Hackathon Velocity Playbook & Zero-Crash Mock Fallback Strategy
   - 9.6 Lean `CLAUDE.md` Architecture vs. The "Boil the Ocean" Mega-Prompt
10. [Specialized MCP Servers & Precomputed Lookup Architectures (Databases, State Engines & Browser Automation)](#10-specialized-mcp-servers--precomputed-lookup-architectures-databases-state-engines--browser-automation)
   - 10.1 Database MCPs for Agentic Workflows (SQLite, Postgres, Supabase, Redis)
   - 10.2 Architectural Pattern: The Precomputed State-Action Engine (Game Bot Case Study)
   - 10.3 Browser & UI Verification MCPs (Playwright, Puppeteer, Chrome DevTools)
11. [Developer Tooling & Rapid Prototyping Infrastructure](#11-developer-tooling--rapid-prototyping-infrastructure)
   - 11.1 Rapid UI Scaffolding & Component Tools (Shadcn CLI, Tailwind v4, Lucide)
   - 11.2 Multimodal Wireframe-to-Code Pipeline (Vision-to-JSX Generation)
   - 11.3 Local Tunneling & Webhook Tools (`cloudflared`, `ngrok`)
   - 11.4 Synthetic Data & API Mocking Tools (`@faker-js/faker`, MSW)
   - 11.5 Deployment CLIs & Secret Detection (`vercel`, `railway`, `gitleaks`)
   - 11.6 Voice-to-Text Terminal Tools (Whisper CLI, Local Speech-to-Text)
12. [Cross-System Capability & Taxonomy Matrix](#12-cross-system-capability--taxonomy-matrix)

---

## 1. Executive Architectural Overview: How the Pieces Connect

The modern AI coding ecosystem consists of multiple distinct layers operating simultaneously. When an engineer executes a prompt in a terminal, that prompt flows through a structured hierarchy:

```
┌────────────────────────────────────────────────────────────────────────┐
│                        USER / TERMINAL PROMPT                          │
└───────────────────────────────────┬────────────────────────────────────┘
                                    │
                                    ▼
┌────────────────────────────────────────────────────────────────────────┐
│ LAYER 1: THE AGENT HARNESS (Runtime Platform)                         │
│ • Claude Code / Codex / Cursor / OpenCode                             │
│ • Shell execution, file editing, tool execution, session storage      │
└───────────────────────────────────┬────────────────────────────────────┘
                                    │
                                    ▼
┌────────────────────────────────────────────────────────────────────────┐
│ LAYER 2: TRAFFIC, CONTEXT & CACHE OPTIMIZATION                         │
│ • Anthropic Native Prompt Caching (Ephemeral prefix reuse)            │
│ • Headroom (Local proxy / AST-aware log/JSON compressor)              │
│ • Permission bypass rules (`~/.claude/settings.json`)                 │
└───────────────────────────────────┬────────────────────────────────────┘
                                    │
                                    ▼
┌────────────────────────────────────────────────────────────────────────┐
│ LAYER 3: PERSISTENT MEMORY & GOVERNANCE                                │
│ • CLAUDE.md (Timeless project constraints & command index)            │
│ • GLOSSARY.md (Ubiquitous language domain dictionary)                  │
│ • ADRs (Architecture Decision Records in docs/adr/)                    │
│ • Session Handoffs (PROGRESS.md, lessons.md, notes.md)                 │
└───────────────────────────────────┬────────────────────────────────────┘
                                    │
                                    ▼
┌────────────────────────────────────────────────────────────────────────┐
│ LAYER 4: PROGRESSIVE DISCLOSURE SKILLS & COMMANDS                      │
│ • Anthropic Skills Standard (SKILL.md loaded only on demand)           │
│ • Custom Slash Commands (.claude/commands/ with dynamic variables)     │
│ • Community Skills (Ponytail, ADHD, Matt Pocock, Addy Osmani)          │
└───────────────────────────────────┬────────────────────────────────────┘
                                    │
                                    ▼
┌────────────────────────────────────────────────────────────────────────┐
│ LAYER 5: META-FRAMEWORKS & ORCHESTRATION                               │
│ • ECC (280+ skills, 67 subagents, AgentShield, Santa Loop, Hermes)     │
│ • Matt Pocock Pipeline (/grill-me → /to-spec → /to-tickets → /tdd)     │
│ • Thermo-Nuclear Review (Deep vs. shallow refactoring via "Code Judo") │
└───────────────────────────────────┬────────────────────────────────────┘
                                    │
                                    ▼
┌────────────────────────────────────────────────────────────────────────┐
│ LAYER 6: MODEL ROUTING & INFERENCE                                     │
│ • Opus 4.6 / 3.7 (Deep planning, architecture, Socratic grilling)     │
│ • Sonnet 3.7 / 4.6 (Primary high-velocity code execution)             │
│ • Haiku 3.5 / 4.5 (Subagent file sweeps, simple lookups, tests)       │
└────────────────────────────────────────────────────────────────────────┘
```

---

## 2. Claude Code Native Engine & Core Primitives

### 2.1 The Agent Harness Runtime
An **agent harness** is the host environment executing inside the developer's workstation. In Anthropic's official tool, **Claude Code**, the harness provides:
- Access to standard POSIX shell tools (`bash`, `git`, `grep`, `file-view`, `file-edit`).
- Native plugin and skills discovery via `.claude/`, `~/.claude/`, and plugin registries.
- Session state persistence in `~/.claude.json`.

### 2.2 Model Routing & `opusplan`
Claude Code supports runtime model selection via the `/model` slash command:
- **`/model sonnet`**: Default for daily implementation, balancing speed and reasoning.
- **`/model opus`**: High-compute model used for subtle bugs and complex architecture.
- **`/model haiku`**: Fast, lightweight model for background tasks.
- **`/model opusplan`**: An automated hybrid routing alias. When toggled into Plan Mode, the harness automatically invokes **Opus** for high-level architectural reasoning. Once the plan is approved and enters implementation, it seamlessly switches to **Sonnet** to write and edit code.

### 2.3 Native Plan Mode (`Shift + Tab`), The "Test-First Plan Gate" & Type-Check Mandate
Plan Mode is a built-in state switch inside Claude Code toggled using the keyboard shortcut `Shift + Tab`:
- **Read-Only Gate:** Claude enters a non-modifying state. It can inspect files, search repositories, and read documentation, but is forbidden from making file edits or executing mutating bash commands.
- **Structured Artifact Generation:** It outputs a concrete plan of action, identifying dependencies and risks.
- **Explicit User Sign-Off:** The harness pauses execution until the developer explicitly approves the plan, at which point the harness switches back to execution mode.

#### The "Tautological Test Trap":
A critical failure mode in AI-assisted coding occurs when an engineer asks an agent to *"write tests"* **after** the implementation is already written:
- **Confirmation Bias:** The model reads its own completed code and writes test assertions that precisely mirror what it just built—including edge-case omissions, logical flaws, and schema misunderstandings.
- **The Result:** The tests pass 100% of the time, creating a dangerous false sense of security while proving almost nothing.

#### The Plan-Mode Test-First Protocol:
To eliminate tautological testing, enforce test specification **during Plan Mode itself**:
1. While in `Shift + Tab` Plan Mode, require Claude to draft the explicit test assertions, expected input/output shapes, and edge-case failure modes into the plan artifact **before any source file is touched**.
2. Once the plan is approved, Claude must write the test file first, execute it to observe the failure (Red), and only then write the implementation (Green).

#### The Mandatory Static Type-Check Gate:
Runtime tests frequently miss subtle interface mismatches and null-pointer hazards. In modern TypeScript or typed Python environments, enforce a mandatory standing gate:
- Claude must run a full static type check (`npx tsc --noEmit`, `mypy`, or `pyright`) and confirm **zero type errors** before reporting that any task is complete.

### 2.4 Prompt Caching Mechanics & The 1-Hour Cold Cache Window
Anthropic APIs utilize prompt caching to avoid re-computing attention on static prefixes (such as system prompts, base tools, and loaded files):
- **Cache Hit Pricing:** Cached tokens receive an approximate 90% discount in cost and significant latency reduction.
- **The 1-Hour Expiry Window:** Cache lifetimes are typically valid for 5 minutes of inactivity on standard endpoints, but within Claude Code sessions, interactive sessions hold a warm cache. If an interactive session is paused or abandoned for **longer than 60 minutes**, the cache goes cold. The next interaction triggers a full re-computation of the entire context history.

### 2.5 Subagent Delegation (`CLAUDE_CODE_SUBAGENT_MODEL`)
Claude Code includes a native `Task` tool that spawns nested, isolated subagents to perform background or exploration tasks:
- **Configuration:** Configured via `~/.claude/settings.json`:
  ```json
  {
    "env": {
      "CLAUDE_CODE_SUBAGENT_MODEL": "haiku"
    }
  }
  ```
- **Context Isolation:** A subagent can inspect 40 files across a repository within its own disposable context window. When its task finishes, it returns only a 10-line condensed summary back to the parent session, shielding the parent context window from token bloat.

#### The Economic Reality: The $20/Month Plan Ruleset for Subagents
On a **$20/month subscription** (Claude Pro / standard rate limits), multi-agent swarms and unconstrained subagent spawning are the fastest way to hit rate-limit lockout walls (*"You have reached your limit until 8:00 PM"*). Every spawned agent initializes its own system prompt, tools, and message history, multiplying token burn 3x–5x.

**As a rule: Never use multi-agents for daily coding on a $20 plan.**

#### The Only 3 Rare Moments Where a Subagent Is Justified:
1. **The Context Poisoning Shield (Wide Codebase Sweeps):**
   - *The Scenario:* You need to track down where an obscure API header or database call originates across 40 files in an unfamiliar repo.
   - *The Trap:* If your *main* session reads those 40 files, your primary context balloons by 80,000 tokens. From that point on, *every single prompt you type* in that session bills for an 80k input payload.
   - *The Subagent Fix:* Spawn a single **Haiku subagent** (`CLAUDE_CODE_SUBAGENT_MODEL=haiku`). The cheap subagent reads the 40 files in a disposable bubble, and returns a 2-line summary: *"The header is injected in `src/auth/interceptor.ts:45`"*. Your main session stays lean at 5k tokens.
2. **The "Clean Room" Sanity Check (Pre-Submission Diff Audit):**
   - *The Scenario:* You have spent 4 hours hacking together a complex feature. Your main chat has seen 30 revisions, errors, and fixes, developing "tunnel vision" (anchoring bias on its own previous mistakes).
   - *The Subagent Fix:* Spawn a single, isolated reviewer subagent on your diff: *"Audit this git diff against main with fresh eyes. Does this break the build, leak secrets, or miss an edge case?"* Because it has zero conversational baggage, it gives an objective, unpolluted audit.
3. **The Disposable Spike (Throwaway Prototype Testing):**
   - *The Scenario:* You are debating whether an unfamiliar third-party library actually works with your framework.
   - *The Subagent Fix:* Dispatch a subagent to a throwaway git branch to build a 20-line proof-of-concept. If it fails, the subagent and its context are deleted. Your main session never sees the failure logs.

#### When to NEVER Use Subagents on a Budget:
- ❌ Regular feature implementation or standard refactoring (just prompt directly).
- ❌ Routine bug fixes or syntax corrections (run `!npm run build` or paste the error).
- ❌ Autonomous multi-agent debates (token bonfires designed for enterprise corporate budgets).

### 2.6 Dynamic Custom Slash Commands (`.claude/commands/`)
Developers can create custom CLI commands by saving Markdown files into `.claude/commands/` (repository-scoped) or `~/.claude/commands/` (user-scoped):
- **Zero Token Overhead:** The prompt template stored in the markdown file is never loaded into context until the user explicitly invokes the command.
- **Dynamic Variable Injection (`$ARGUMENTS`):** Anything typed after the slash command is substituted into the prompt.
  - Command: `/project:fix 401 error on auth`
  - Template: `Investigate this error: $ARGUMENTS`
- **Shell Output Injection (``!`command``` syntax):** Executes terminal commands inline and embeds the live terminal output directly into the prompt payload before sending:
  ```markdown
  Analyze our current git status:
  !`git status --short`
  !`git diff --stat`
  ```

### 2.7 Automated Permission Controls (`settings.json`)
By default, Claude Code requests user confirmation before executing bash commands or modifying files. To enable autonomous operation without repetitive prompts, permissions are configured in `~/.claude/settings.json`:
```json
{
  "permissions": {
    "allow": [
      "Bash(npm run *)",
      "Bash(npx *)",
      "Bash(git *)",
      "Bash(ls *)",
      "Write(*)"
    ]
  }
}
```

### 2.8 Native Shell Mode (`!` Prefix, `Ctrl + Z` Suspend & `Ctrl + S` Stash)
Claude Code includes low-level terminal bypass features that grant direct shell access without triggering expensive LLM reasoning cycles:

#### 1. The `!` Command Execution Prefix (Zero-Token Bypass):
- When typing into the Claude Code prompt, prefixing any string with an exclamation mark (`!`) executes that command directly on your host machine's POSIX shell.
- *Examples:* `!git status`, `!npm test`, `!docker compose ps`, `!curl -I localhost:3000`.
- *Why it's essential:*
  - **Zero Token Burn:** If you prompt Claude *"can you check git status and tell me modified files"*, Claude sends input tokens, consumes reasoning tokens, calls the bash tool, and returns prose (burning 2,000–5,000 tokens). Typing `!git status` executes locally in 5ms at **0 token cost**.
  - **Live Buffer Injection:** The terminal output of the `!` command remains in the active terminal feed. If a build fails with `!npm run build`, your next natural prompt to Claude (*"fix the build error above"*) already has the exact compiler output in its immediate visual buffer.

#### 2. Process Suspension (`Ctrl + Z` & `fg`):
- Instead of opening multiple terminal tabs or killing an active Claude Code session to run complex git rebases, long scripts, or package installs, press **`Ctrl + Z`** when Claude is idle.
- The session is suspended in the background, dropping you cleanly into your native operating system shell.
- Run your manual commands (`git checkout -b feature`, `htop`, etc.).
- When ready, type **`fg`** and press Enter: your Claude Code session instantly resumes with its full context cache and conversation state 100% intact.

#### 3. Prompt Stashing (`Ctrl + S`):
- If you have typed a half-finished prompt or complex command and need to clear your input line to inspect terminal logs or run an inline `!` check, press **`Ctrl + S`**.
- The line is stashed into an ephemeral buffer. Pressing **`Ctrl + S`** again restores your exact draft.

---

### 2.9 Context Hygiene Primitives: `/context`, `/compact`, `/clear` & The 100k–150k Ceiling Rule
Managing the size of the active context window is the single highest-leverage factor determining agent reasoning accuracy, latency, and token billing.

#### 1. The `/context` Inspection Command:
- Running `/context` (or `/context-budget`) outputs an immediate, visual telemetry breakdown of your current session's token consumption:
  - System prompt & core instructions
  - Loaded MCP tool schemas
  - Active conversation history & user prompts
  - Accumulated tool outputs, terminal logs, and file read dumps
- **The 100k–150k Ceiling Rule:**
  - Standard frontier models operate with massive context windows (200k to 1M+), but attention sharpness degrades as context swells past 100,000 tokens.
  - When sessions reach **100,000 to 150,000 tokens**, "needle-in-a-haystack" retrieval friction increases, turn latency doubles, and cost per turn compounds significantly.
  - *Best Practice:* Regularly run `/context`. The moment token usage crosses 100k–150k, enforce a mandatory hygiene checkpoint: either run `/compact` (if continuing the current feature) or `/clear` (if the task is completed).

#### 2. What Is `/compact` and How Does It Work?
- The `/compact` command prunes and summarizes the conversation history down to an executive state.
- **What it discards:** Raw terminal logs, verbose compiler error dumps from earlier iterations, intermediate file reads, and conversational pleasantries.
- **What it preserves:** High-level architectural decisions, modified file paths, established conventions, and active pending tasks.
- **Typical Compression Ratio:** Safely collapses a bloated 120,000-token session back down to 15,000–25,000 tokens without losing project continuity.

#### 3. Strategic Compaction vs. The "Auto-Compact Fallacy":
- **The Auto-Compact Fallacy:** Claude Code includes an automatic compaction threshold that triggers autonomously when context reaches ~75–80% of capacity. When triggered *involuntary in the middle of writing complex code*, auto-compaction frequently strips recent variable names, subtle test edge cases, and in-flight debugging traces, causing the agent to experience sudden amnesia.
- **The Strategic Compaction Rule:** Never rely on auto-compaction. Trigger `/compact` **deliberately at logical milestones**:
  - *When to run `/compact`:* Right after planning is approved (before coding begins); right after backend tests pass (before switching to frontend UI); right after solving a stubborn bug.
  - *When NOT to run `/compact`:* In the middle of an active multi-file refactor or while an active compiler error is being diagnosed.

#### 4. The "Auto-Compact Canary" Rule (The Session Reset Signal):
- **The Heuristic:** 
  - *If you manually type `/compact`:* It is controlled, deliberate, and preserves momentum across a known milestone.
  - *If Claude triggers auto-compaction on its own:* **This is your canary in the coal mine.** It indicates the context has crossed 80% capacity (~160k+ tokens), memory degradation has already begun, and subtle active variables were silently discarded.
- **The Action:** Whenever Claude announces an automatic compaction, immediately finish the current thought, commit your working files (`git commit`), and run `/clear` or open a fresh terminal session.

#### 5. The Context Hygiene Triangle:
| Command | Action | Impact on Memory | When to Use |
| :--- | :--- | :--- | :--- |
| **`/context`** | Inspection only | 0 modification | Run every few prompts or whenever responses feel slow to check token weight. |
| **`/compact`** | Condenses history | Keeps goals & files, dumps logs | At major task milestones when context reaches 100k–150k. |
| **`/clear`** | Wipes 100% of context | Total reset (clean slate) | Between distinct, unrelated features (the "1 Task = 1 Chat" rule). |

---

### 2.10 Autonomous Scheduled Loops: `/loop` & `/goal` (Mechanics, Traps & Relevance)

#### 1. What Is the `/loop` Command?
Claude Code includes a built-in session-level scheduler that repeats instructions at fixed or dynamic intervals without manual prompt re-entry:
- **Fixed Interval Syntax:**
  ```bash
  /loop 5m "check if the backend deployment succeeded and run smoke tests"
  ```
  *(Units: `s`, `m`, `h`, `d`. Re-evaluates repository state on every iteration).*
- **Dynamic Interval:**
  ```bash
  /loop "poll git status and fix any linting failures that appear"
  ```
  *(Claude dynamically calculates delays between 1 minute and 1 hour based on the task complexity).*

#### 2. Coupling `/loop` with `/goal` for Autonomous Convergence:
- **`/goal`**: Defines a strict, measurable completion target (e.g., `/goal Zero TypeScript errors and all integration tests green`).
- **`/loop`**: Executes repeated autonomous turns, attempting fixes and running builds until the `/goal` evaluation function returns `true`.

#### 3. Is `/loop` Relevant for You Right Now?
- **🟢 When It Is Highly Useful:**
  - **Background Polling:** Waiting for long CI/CD pipelines, Docker container builds, or remote database migrations to complete while you work elsewhere.
  - **Overnight Convergence:** Running an autonomous bug-fixing loop against a suite of 30 failing unit tests before going to sleep.
- **🔴 When It Is a Massive Token Hazard (The Warning):**
  - **Never use during active interactive coding:** If you leave `/loop` active while thinking or designing, it will poll the LLM every few minutes, burning tens of thousands of tokens on static repository checks.
  - For hackathons and rapid feature sprints, **avoid open background loops**; you want immediate, deterministic, turn-by-turn execution under direct supervision.

---

## 3. ECC (Everything Claude Code) Framework Deep-Dive

### 3.1 What ECC Is and Its Architecture
**ECC** (`affaan-m/ECC`, published on npm as `ecc-universal` and in the plugin registry as `ecc@ecc`) is an open-source performance optimization system and agent harness operating layer created by Affaan Mustafa. It transforms an AI coding tool into a structured, production-grade engineering system by standardizing the engineering lifecycle:
$$\text{plan} \longrightarrow \text{test} \longrightarrow \text{implement} \longrightarrow \text{review} \longrightarrow \text{verify} \longrightarrow \text{remember} \longrightarrow \text{improve}$$

> *"Optimize the context window. Persist everything else."*

Instead of rebuilding workflows into every prompt, ECC installs a cohesive suite containing **71 specialized subagents**, **311 progressive disclosure skills**, **97 slash command shims**, **122 modular rule packs across 22 language stacks**, an automated **lifecycle hooks runtime**, and the **AgentShield security engine**.

#### Core Principles of ECC:
1. **Agent-First:** Delegate complex domain tasks to specialized subagent identities with scoped tools and constraints.
2. **Test-Driven (TDD):** Enforce writing automated tests before implementation; require 80%+ test coverage.
3. **Security-First:** Never compromise on security boundaries; sanitize inputs, guard configs, and scan for secret leakage.
4. **Immutability:** Encourage creating new objects and pure functional transformations over destructive mutations.
5. **Plan Before Execute:** Prohibit blind code modification; enforce explicit architectural planning before editing source files.

#### Multi-Harness Compatibility Matrix (Release 2.2.3):
| Harness | Integration Mode | Supported Surface & Capabilities |
| :--- | :--- | :--- |
| **Claude Code** | Native Plugin (`ecc@ecc`) | 100% full parity: all 71 agents, 311 skills, 97 commands, 8 lifecycle hooks, rules, memory. |
| **Codex (CLI/App)** | Native Marketplace Plugin | Full parity: discovers root `AGENTS.md`, native skills, tools, and Codex-managed hooks. |
| **Qoder (CLI/IDE)** | Native Plugin (`.qoder-plugin`) | Reuses canonical skills, agents, 95 command shims, Chrome DevTools MCP, and session bootstrap. |
| **Cursor** | Local Adapter (`.cursor/`) | Managed agent definitions under `.cursor/agents/ecc-*.md` and project-local rules. |
| **OpenCode** | Built Plugin (`npm run build:opencode`)| Full agent and skill integration with background hook execution. |
| **Gemini CLI / Antigravity**| Project Config (`.gemini/`) | Skill execution and workflow prompt compatibility via project-scoped configuration. |
| **Zed / Qwen / Kimi** | Managed Local Adapter | Project-local rules, skills, and prompt shims mapped to host conventions. |
| **Hermes / OpenClaw** | Headless Adapter | Headless operator runner with CI/CD guard hooks and automated batch workflows. |

---

### 3.2 Modular Install Profiles & Deployment Topologies
To eliminate token bloat and allow developers to tailor context usage to their hardware and subscription tiers, ECC utilizes a modular package architecture defined in `manifests/install-profiles.json`:

```
                       ┌──────────────────────┐
                       │   ECC Profile Root   │
                       └──────────┬───────────┘
                                  │
         ┌────────────────────────┼────────────────────────┐
         │                        │                        │
         ▼                        ▼                        ▼
┌──────────────────┐    ┌──────────────────┐    ┌──────────────────┐
│     minimal      │    │       core       │    │    developer     │
│ • rules-core     │    │ • minimal +      │    │ • core +         │
│ • agents-core    │    │ • hooks-runtime  │    │ • framework-lang │
│ • commands-core  │    │                  │    │ • database       │
│ • workflow-qual. │    │                  │    │ • orchestration  │
└──────────────────┘    └──────────────────┘    └──────────────────┘
         │                        │                        │
         ▼                        ▼                        ▼
┌──────────────────┐    ┌──────────────────┐    ┌──────────────────┐
│     security     │    │     research     │    │       full       │
│ • core +         │    │ • core +         │    │ • All 311 skills │
│ • security agent │    │ • research-apis  │    │ • All 71 agents  │
│ • AgentShield    │    │ • business-cont. │    │ • All modules    │
└──────────────────┘    └──────────────────┘    └──────────────────┘
```

#### Install Profiles Breakdown:
- **`minimal`**: Low-context profile. Installs core rules, base planning agents, quality workflows, and command shims, completely omitting the background Node.js hook runtime. Zero background CPU overhead.
- **`core`**: Baseline engineering harness. Adds the active lifecycle hooks runtime (`hooks-runtime`), cost trackers, and platform configurations.
- **`developer`**: Standard production profile. Adds database skills, language/framework patterns (React, Python, Go, Rust), and worktree orchestration.
- **`security`**: Hardened security profile. Activates AgentShield pre-execution command scanning, SAST audits, and compliance checks (HIPAA, OWASP, PHI).
- **`research`**: Research & synthesis profile. Adds deep web search APIs (SerpApi, Exa), scientific literature querying (PubMed, USPTO), and market synthesis.
- **`full`**: Unrestricted installation containing all 71 agents, all 311 skills, 97 commands, and domain modules for ML, DevOps, Swift, and prediction markets.

#### Universal Setup Commands:
```bash
# Recommended universal guided installer (Claude Code, Codex, Kimi)
npx ecc-universal@2.2.3 setup

# In-Claude Code native plugin installation
/plugin marketplace add https://github.com/affaan-m/ECC
/plugin install ecc@ecc

# Selective component installation via advisor
npx ecc-universal@2.2.3 consult "security reviews" --target claude
npx ecc-universal@2.2.3 install --profile minimal --target claude --with capability:machine-learning
```

---

### 3.3 The Complete 71 Specialized Subagents Directory
ECC provides **71 dedicated autonomous agents**. Each agent operates under a tailored system persona with strict tool permissions, output constraints, and model routing parameters:

| Agent Name | Primary Specialization & Purpose | When to Trigger / Recommended Use |
| :--- | :--- | :--- |
| **`a11y-architect`** | WCAG 2.2 accessibility architecture | Designing UI components, accessibility audits, screen reader optimization |
| **`agent-evaluator`** | Agent benchmark and evaluation specialist | Running regression evaluations, tool accuracy benchmarks, and trajectory audits |
| **`architect`** | System design and scalability architect | High-level architectural decisions, cross-service contracts, scaling bottlenecks |
| **`bengali-reviewer`** | Bengali text and NLP review | Unicode normalization, grapheme clusters, localized search, and locale correctness |
| **`build-error-resolver`** | Universal build and type error resolver | When compiler, linter, or type checks fail across any supported language |
| **`chief-of-staff`** | High-level engineering project orchestrator | Milestone tracking, ticket delegation, multi-agent status reporting, and unblocking |
| **`code-architect`** | Feature architecture blueprints from codebase patterns | Designing new features adhering strictly to existing codebase architectural patterns |
| **`code-explorer`** | Fast, read-only codebase explorer | Mapping module dependencies, inheritance hierarchies, and call graphs without token bloat |
| **`code-reviewer`** | General code quality and maintainability reviewer | Auditing newly written or refactored code for idioms, readability, and performance |
| **`code-simplifier`** | Complexity reducer and refactoring specialist | Simplifying verbose functions, eradicating dead branches, and improving clarity |
| **`comment-analyzer`** | Documentation and comment auditor | Verifying accuracy of docstrings, detecting outdated comments, and flagging TODO rot |
| **`commit-reviewer`** | Git commit and diff quality reviewer | Enforcing conventional commit standards, atomic changes, and descriptive messages |
| **`conversation-analyzer`** | Session transcript and context hygiene auditor | Analyzing conversation drift, token waste, and extracting key settled decisions |
| **`cpp-build-resolver`** | C and C++ compilation resolver | Resolving CMake, GCC, Clang, linker, and header inclusion errors |
| **`cpp-reviewer`** | C and C++ code review specialist | Auditing modern C++20/23, RAII, memory safety, pointer bounds, and concurrency |
| **`csharp-reviewer`** | C# and .NET code reviewer | Auditing async/await patterns, nullable references, LINQ efficiency, and security |
| **`dart-build-resolver`** | Dart and Flutter build resolver | Fixing pub dependency conflicts, Gradle/CocoaPods integration, and compile errors |
| **`database-reviewer`** | PostgreSQL and Supabase specialist | Schema design, index tuning, query plan analysis, RLS policies, and migrations |
| **`django-build-resolver`** | Django build and migration resolver | Resolving Django startup errors, migration collisions, collectstatic, and settings bugs |
| **`django-reviewer`** | Django and DRF code reviewer | Auditing Django ORM queries (N+1), REST framework serializers, signals, and security |
| **`doc-updater`** | Documentation and codemap synchronizer | Updating READMEs, API docs, system codemaps, and architecture diagrams |
| **`docs-lookup`** | Context7 documentation lookup assistant | Fetching and referencing verified framework, library, and third-party API docs |
| **`e2e-runner`** | Playwright end-to-end testing specialist | Executing, diagnosing, and repairing end-to-end browser test suites |
| **`fastapi-reviewer`** | FastAPI and Pydantic reviewer | Auditing async endpoints, dependency injection, Pydantic v2 schemas, and OpenAPI specs |
| **`flutter-reviewer`** | Flutter and Dart widget/state reviewer | Auditing widget tree rebuilds, BLoC/Riverpod state, and platform channels |
| **`fsharp-reviewer`** | F# functional programming reviewer | Auditing functional idioms, immutability, pattern matching, and type safety |
| **`gan-evaluator`** | GAN discriminator evaluator agent | Critiquing and stress-testing candidate code and architectures against failure modes |
| **`gan-generator`** | GAN generator agent | Producing candidate solutions, code variants, and test cases under adversarial guidance |
| **`gan-planner`** | GAN adversarial loop planner | Setting convergence thresholds, eval criteria, and loss functions for iterative generation |
| **`go-build-resolver`** | Go build and module resolver | Resolving go mod conflicts, cgo errors, build tag issues, and compiler failures |
| **`go-reviewer`** | Go idioms and concurrency reviewer | Auditing goroutine leaks, channel synchronization, error handling, and interfaces |
| **`harmonyos-app-resolver`** | HarmonyOS ArkTS/ArkUI specialist | Auditing OpenHarmony/HarmonyOS apps, ArkTS V2 state, and Stage model APIs |
| **`harness-optimizer`** | Harness configuration and cost tuner | Optimizing model routing, token expenditure, tool selection, and execution speed |
| **`healthcare-reviewer`** | Clinical safety and PHI compliance auditor | Auditing HIPAA/HITECH compliance, PHI sanitization, and CDSS clinical accuracy |
| **`homelab-architect`** | Home and small-lab infrastructure architect | Designing Proxmox, VLAN segmentation, TrueNAS, and local container topologies |
| **`java-build-resolver`** | Java, Maven, and Gradle build resolver | Fixing Maven dependency trees, Gradle tasks, classpath issues, and JDK incompatibilities |
| **`java-reviewer`** | Java and Spring Boot reviewer | Auditing Spring Boot services, JPA/Hibernate mappings, concurrency, and OOP idioms |
| **`kotlin-build-resolver`** | Kotlin and Gradle build resolver | Resolving Kotlin multiplatform, Android Gradle plugin, and compiler flags |
| **`kotlin-reviewer`** | Kotlin and KMP code reviewer | Auditing coroutines, Flow pipelines, Exposed ORM, and Compose Multiplatform code |
| **`loop-operator`** | Autonomous loop execution monitor | Supervising long-running `/loop` tasks, detecting stalls, and preventing runaway loops |
| **`marketing-agent`** | Product marketing and launch copywriter | Drafting release notes, changelogs, feature announcement copy, and value propositions |
| **`mle-reviewer`** | Production ML engineering reviewer | Auditing training data contracts, reproducible pipelines, model serving, and rollbacks |
| **`network-architect`** | Enterprise multi-site network architect | Designing SD-WAN, BGP routing, MPLS overlays, and datacenter network fabrics |
| **`network-config-reviewer`** | Router and switch configuration auditor | Auditing Cisco IOS, Junos, and Arista configs for security and syntax correctness |
| **`network-troubleshooter`** | OSI-layer network diagnostic specialist | Diagnosing packet loss, MTU mismatches, routing loops, and DNS resolution failures |
| **`opensource-forker`** | Open-source fork manager | Managing upstream sync, conflict resolution, and customized patches on forked repos |
| **`opensource-packager`** | Open-source release packaging specialist | Setting up licenses, CI release workflows, npm/PyPI publish configs, and security policies |
| **`opensource-sanitizer`** | Open-source repository sanitizer | Scrubbing internal URLs, credentials, proprietary comments, and internal IPs |
| **`performance-optimizer`** | System performance and bottleneck optimizer | Profiling CPU hot loops, memory leaks, bundle sizes, and GC pressure |
| **`php-reviewer`** | PHP and modern PSR-12 reviewer | Auditing Laravel/Symfony code, PHP 8.x types, Eloquent queries, and security |
| **`planner`** | Feature implementation planner | Proactively breaking down complex features, risk analysis, and implementation sequences |
| **`pr-test-analyzer`** | PR test quality and coverage analyzer | Auditing pull request test suites for mutation survivability and missing assertions |
| **`python-reviewer`** | Pythonic idioms and style reviewer | Auditing type hints, generators, asyncio event loops, and PEP 8 compliance |
| **`pytorch-build-resolver`** | PyTorch and CUDA error resolver | Resolving CUDA OOM, tensor shape mismatches, DDP distributed training errors |
| **`rag-pipeline-reviewer`** | RAG pipeline and retrieval auditor | Auditing chunking strategies, vector embeddings, rerankers, and RAGAS evals |
| **`react-build-resolver`** | React, Vite, Next.js, and Webpack resolver | Resolving JSX compilation, hydration mismatches, Webpack plugins, and Vite builds |
| **`react-reviewer`** | React and JSX architecture reviewer | Auditing React 19 patterns, Server Components, hook dependencies, and re-renders |
| **`refactor-cleaner`** | Dead code and technical debt cleaner | Safely removing unused exports, deprecated functions, and dead dependencies |
| **`ruby-reviewer`** | Ruby and Rails code reviewer | Auditing Ruby idioms, Rails 8 Solid adapters, ActiveRecord queries, and RuboCop rules |
| **`rust-build-resolver`** | Rust borrow-checker and build resolver | Fixing borrow checker errors, lifetime annotations, macro expansions, and Cargo bugs |
| **`rust-reviewer`** | Rust safety and idioms reviewer | Auditing unsafe blocks, trait bounds, error handling (anyhow/thiserror), and zero-copy |
| **`security-reviewer`** | Security vulnerability and SAST reviewer | Auditing OWASP Top 10, auth boundaries, input validation, and secret exposure |
| **`seo-specialist`** | Technical SEO and web discoverability auditor | Auditing SSR metadata, OpenGraph, JSON-LD schemas, sitemaps, and Core Web Vitals |
| **`silent-failure-hunter`** | Swallowed error and exception auditor | Hunts empty catch blocks, unhandled promise rejections, and silent error drops |
| **`spec-miner`** | Brownfield specification extractor | Extracting formal specs, domain models, and API contracts from legacy codebases |
| **`swift-build-resolver`** | Swift, Xcode, and SPM build resolver | Fixing Swift Package Manager dependencies, Xcode scheme errors, and compiler flags |
| **`swift-reviewer`** | Swift and iOS architecture reviewer | Auditing Swift 6 Concurrency, actors, SwiftUI layouts, and Apple platform standards |
| **`tdd-guide`** | Test-driven development mentor | Guiding strict Red-Green-Refactor cycles and enforcing 80%+ test coverage gates |
| **`type-design-analyzer`** | Type safety and invariant design analyzer | Auditing type encapsulation, branded types, and compile-time illegal state prevention |
| **`typescript-reviewer`** | TypeScript and JavaScript code reviewer | Auditing strict null checks, generics, type narrowing, and modern ES features |
| **`vue-reviewer`** | Vue 3 and Nuxt code reviewer | Auditing Composition API, reactivity triggers, Pinia stores, and Nuxt SSR hydration |

#### Key Architectural Subagent Clusters:

1. **Feature Planning & Blueprints (Opus-Routed):**
   - `planner`: Automatically invoked when users prompt for complex features or architectural rewrites. Produces a phased roadmap and pauses for human confirmation.
   - `architect`: High-level system scalability specialist. Evaluates service boundaries, cross-module seams, and throughput constraints.
   - `code-architect`: Scans existing codebase idioms to generate blueprints that match established internal abstractions rather than external generic code.

2. **The Build Repair Fleet (11 Specialized Resolvers):**
   - Instead of a generalist model guessing at build failures, ECC routes compiler errors to dedicated language resolvers:
   - `build-error-resolver` (Universal fallback), `cpp-build-resolver` (CMake/Clang), `go-build-resolver` (Go modules), `java-build-resolver` (Maven/Gradle), `kotlin-build-resolver` (KMP/Gradle), `react-build-resolver` (Vite/Next.js hydration), `rust-build-resolver` (Cargo/borrow-checker), `swift-build-resolver` (Xcode/SPM), `dart-build-resolver` (Flutter pub), `django-build-resolver` (Django migrations), `pytorch-build-resolver` (CUDA/tensor shapes).

3. **Code Quality, Simplification & Refactoring:**
   - `code-reviewer`: Fresh-context audit inspecting newly modified files for maintainability and bugs.
   - `code-simplifier`: Dedicated refactorer that simplifies complex logic, removes redundant intermediate variables, and reduces cyclomatic complexity.
   - `silent-failure-hunter`: Scans for swallowed exceptions, empty catch blocks, unhandled promise rejections, and silent error fallbacks.
   - `type-design-analyzer`: Enforces type encapsulation, branded primitives, and compile-time invariant validation.
   - `refactor-cleaner`: Safely deletes dead exports, unused methods, and orphaned dependencies.

4. **Autonomous Supervisors & Adversarial Loops:**
   - `loop-operator`: Monitors background `/loop` tasks, detecting infinite loops, token exhaustion, and algorithmic stalls.
   - `chief-of-staff`: Orchestrates multi-agent timelines, manages ticket dependencies, and updates project status boards.
   - `gan-planner`, `gan-generator`, `gan-evaluator`: Triad executing iterative adversarial generation where the generator writes solutions and the evaluator aggressively tests them until convergence.

---

### 3.4 The 311 Skills Complete Inventory & Domain Taxonomy
Skills in ECC follow Anthropic's progressive disclosure standard. They reside in `.agents/skills/` and `skills/`, remaining dormant at **zero token overhead** until their YAML `description` trigger matches a user prompt or subagent request.

Here is the complete inventory of all **311 skills**, organized across 10 functional domains:

#### 1. Core SDLC, Planning, TDD & Verification Loops (55 Skills)
| Skill Identifier | Capability & Operational Purpose |
| :--- | :--- |
| **`accessibility`** | WCAG 2.2 accessibility checklist and automated audit workflows |
| **`agent-eval`** | Regression evaluation harness for measuring agent code output quality |
| **`ai-regression-testing`** | Automated baseline generation and regression detection for AI features |
| **`architecture-decision-records`** | Formal ADR drafting, numbering, and architecture locking workflow |
| **`benchmark`** | Standardized micro-benchmarking and execution profiling framework |
| **`benchmark-methodology`** | Scientific benchmarking standards: warmups, statistical confidence, p99 metrics |
| **`benchmark-optimization-loop`** | Iterative profiling, hot-spot optimization, and verified speedup loops |
| **`blueprint`** | End-to-end technical blueprint authoring for greenfield systems |
| **`brownfield-retrospective`** | Analyzing legacy codebase evolution, tech debt hotspots, and refactor ROI |
| **`browser-qa`** | Headless browser validation and automated visual regression checks |
| **`canary-watch`** | Automated canary release monitoring and rollback trigger analysis |
| **`change-impact`** | Static dependency graphing to predict downstream effects of a code change |
| **`ck`** | Codebase knowledge extraction and cross-module relationship mapping |
| **`code-tour`** | Guided interactive walkthroughs of complex subsystems for developers |
| **`codebase-onboarding`** | Automated repository indexing, dev setup validation, and architecture briefs |
| **`codehealth-mcp`** | MCP server integration for continuous code health and linting audits |
| **`coding-standards`** | Language-agnostic coding standards, formatting rules, and style consistency |
| **`continuous-learning`** | Legacy v1 Stop-hook session pattern extraction and rule synthesis |
| **`continuous-learning-v2`** | Continuous learning engine: turns debugging victories into persistent rules |
| **`conventional-commit-review`** | Auditing git commit history for conventional commit specifications |
| **`decision-ledger`** | Immutable record of architectural and product trade-offs made in a repo |
| **`delivery-gate`** | Automated pre-deployment quality checks and merge blocking rules |
| **`doc-file-guard`** | Pre-execution hook preventing accidental clobbering of documentation |
| **`docs-governance`** | Documentation freshness auditing, dead link detection, and drift control |
| **`documentation-lookup`** | Context7 and verified API documentation ingestion engine |
| **`dynamic-workflow-mode`** | Adaptive workflow state machine shifting between prototype, TDD, and production |
| **`e2e-testing`** | Playwright end-to-end test authoring, fixture management, and execution |
| **`error-handling`** | Standardized error hierarchy, custom exception classes, and error bubbling |
| **`eval-harness`** | Evaluation harness for AI code generation correctness and robustness |
| **`gateguard`** | Standing pre-edit gate enforcing test-first verification before source modification |
| **`git-workflow`** | Git branching strategies, atomic commits, PR hygiene, and merge protocols |
| **`inherit-legacy-style`** | Style extraction engine matching legacy codebase idioms without refactoring |
| **`intent-driven-development`** | Aligning code changes directly with user intent and product goals |
| **`legacy-code-scaffolding`** | Wrapping legacy modules in safety seams before refactoring |
| **`living-docs-governance`** | Self-updating documentation linked directly to code symbol changes |
| **`module-regression`** | Module-level regression testing ensuring backward-compatible interfaces |
| **`plan-canvas`** | Browser review canvas for interactive plan artifact annotation and approval |
| **`plan-orchestrate`** | Multi-stage project planner coordinating parallel work streams |
| **`plankton-code-quality`** | High-density code quality scoring and cognitive complexity metrics |
| **`quality-gate`** | Multi-axis quality evaluation blocking merges that fail criteria |
| **`recursive-decision-ledger`** | Hierarchical decision tracking linking system architecture to line-level diffs |
| **`refactor-cleaner`** | Dead code identification, unused export pruning, and dependency tree cleanup |
| **`release-checklist`** | Pre-release audit checklist verifying migrations, secrets, and telemetry |
| **`root-cause-analysis`** | Hypothesis-driven diagnostic tree isolating bugs before code edits |
| **`santa-method`** | Adversarial review protocol enforcing two opposing perspectives on diffs |
| **`smoke-test`** | Fast end-to-end sanity verification running critical path smoke checks |
| **`spec-driven-dev`** | Formal specification-first development translating PRDs into atomic code |
| **`story-lifecycle`** | Agile story lifecycle management from user story to deployed feature |
| **`strategic-compact`** | Controlled session context compaction preserving active mental state |
| **`tdd-workflow`** | Strict Red-Green-Refactor test-driven development workflow |
| **`test-collaboration`** | Multi-agent collaborative test authoring across unit and integration levels |
| **`test-first`** | Standing test-first protocol authoring test assertions before implementation |
| **`unified-memory`** | Cross-harness persistent project memory requiring the ecc-universal CLI |
| **`verification-loop`** | Autonomous build, lint, and test verification loop ensuring 100% green code |
| **`windows-desktop-e2e`** | E2E testing for Windows native desktop apps via pywinauto and UI Automation |

#### 2. Agentic Systems, Multi-Agent & Harness Infrastructure (59 Skills)
| Skill Identifier | Capability & Operational Purpose |
| :--- | :--- |
| **`agent-architecture-audit`** | Auditing agent system prompts, tool schemas, and token efficiency |
| **`agent-harness-construction`** | Scaffolding custom agent host environments with tool/permission sandboxes |
| **`agent-introspection-debugging`** | Inspecting agent reasoning traces, hidden scratchpads, and failure loops |
| **`agent-payment-x402`** | Agent-to-agent HTTP 402 micro-payment and credit settlement protocols |
| **`agent-runtime-gateway-smoke-test`** | Smoke testing runtime gateways and LLM proxy routers for agent sessions |
| **`agent-security-hardening`** | Hardening agent harnesses against prompt injection and tool manipulation |
| **`agent-self-evaluation`** | Meta-evaluation prompts enabling models to critique their own output quality |
| **`agent-sort`** | Categorizing and routing complex user prompts to the optimal specialized subagent |
| **`agentic-engineering`** | Production engineering principles for multi-turn autonomous agent loops |
| **`agentic-os`** | Agentic operating system abstractions: process lifecycle, memory, and IO |
| **`ai-first-engineering`** | Re-architecting software architectures to maximize LLM code generation speed |
| **`autonomous-agent-harness`** | Headless autonomous runner executing queued tasks with minimal human input |
| **`autonomous-loops`** | Safe scheduled loop design with termination guarantees and budget ceilings |
| **`claude-devfleet`** | Orchestrating fleets of Claude Code instances across multiple terminal windows |
| **`configure-ecc`** | Interactive wizard configuring ECC scopes, profiles, and hook runtimes |
| **`connections-optimizer`** | Optimizing connection pooling and multiplexing for agent API calls |
| **`context-and-decisions`** | Persistent context management linking architectural decisions to prompts |
| **`context-budget`** | Visual telemetry inspecting session token consumption across tools and chats |
| **`continuous-agent-loop`** | Long-running autonomous agent loops with self-healing and stall detection |
| **`cost-aware-llm-pipeline`** | Routing prompts dynamically to the lowest-cost model meeting quality gates |
| **`cost-tracking`** | Real-time session spend tracking in USD with cache hit/miss breakdown |
| **`council`** | Multi-agent consensus voting engine resolving ambiguous technical designs |
| **`council-multi-model`** | Multi-model deliberation polling Claude, OpenAI, and Gemini for consensus |
| **`deepseek-harness-setup`** | Configuring DeepSeek R1/V3 models as reasoning backends in agent harnesses |
| **`dev-team`** | Multi-agent simulated software engineering team with defined roles |
| **`dmux-workflows`** | Distributed multiplexer workflows managing parallel agent processes |
| **`ecc-guide`** | Comprehensive interactive reference manual for all ECC capabilities and commands |
| **`ecc-recipes`** | Curated prompt templates and workflows for common engineering challenges |
| **`ecc-tools-cost-audit`** | Auditing total token and financial expenditure across installed ECC tools |
| **`enterprise-agent-ops`** | Enterprise compliance, audit logging, and RBAC for autonomous agents |
| **`gan-style-harness`** | Adversarial GAN-style dual-agent evaluation loop for high-rigor tasks |
| **`harness-audit`** | Diagnosing environment variables, tool permissions, and PATH configs for harnesses |
| **`hermes-imports`** | Importing and converting skills from the Hermes agent format to ECC |
| **`hookify-rules`** | Dynamic user-defined hook generator authoring custom pre/post tool hooks |
| **`llm-trading-agent-security`** | Security constraints, circuit breakers, and sandboxes for financial agents |
| **`loop-design-check`** | Pre-flight validation ensuring `/loop` commands have deterministic exit conditions |
| **`mcp-dependency-review`** | Auditing MCP servers for latency, schema bloat, and dependency safety |
| **`mcp-server-patterns`** | Best practices for authoring performant, schema-compact MCP servers |
| **`model-route`** | Intelligent model selection routing simple queries to Haiku and complex to Opus |
| **`multi-agent-orchestration`** | Worktree and tmux-based multi-agent coordination with file locks |
| **`multi-agent-supervisor`** | Supervisor agent delegating tasks, reviewing sub-diffs, and merging outputs |
| **`nanoclaw-repl`** | Lightweight sandboxed REPL executing throwaway code snippets safely |
| **`openclaw-persona-forge`** | Configuring agent personas, system prompt baselines, and tone guidelines |
| **`operator-approval-loop`** | Deterministic human-in-the-loop approval gates for sensitive operations |
| **`orch-pipeline`** | Full orchestration pipeline managing epic decomposition, coding, and review |
| **`parallel-execution-optimizer`** | Optimizing parallel tool execution and subagent task scheduling |
| **`prompt-caching-strategy`** | Optimizing prompt structure to maximize 90% discount prompt cache hits |
| **`prompt-optimizer`** | Refactoring system prompts to reduce token size while increasing adherence |
| **`safety-guard`** | Standing safety filter intercepting potentially destructive system commands |
| **`skill-comply`** | Auditing custom skills against Anthropic's official 6-insight authoring standard |
| **`skill-host-compat`** | Verifying skill compatibility across Claude Code, Codex, Cursor, and Zed |
| **`skill-scout`** | Scanning public registries to discover and recommend relevant agent skills |
| **`skill-stocktake`** | Auditing installed skills, removing duplicates, and resolving command collisions |
| **`team-agent-orchestration`** | Coordinating specialized agents in defined sequence: Plan -> Build -> Audit |
| **`team-builder`** | Scaffolding custom multi-agent team manifests for specific project domains |
| **`terminal-opener`** | Cross-platform terminal tab launcher spawning isolated agent instances |
| **`terminal-ops`** | Automating shell commands, tmux splits, and process monitoring for agents |
| **`token-budget-advisor`** | Recommending compaction or session reset based on active token velocity |
| **`token-card`** | Visual breakdown card displaying session token consumption by category |

#### 3. Frontend, UI/UX, Design Systems & React/Vue/Web (43 Skills)
| Skill Identifier | Capability & Operational Purpose |
| :--- | :--- |
| **`angular-developer`** | Modern Angular 17+ standalone components, signals, and RxJS patterns |
| **`compose-multiplatform-patterns`** | Kotlin Compose Multiplatform declarative UI patterns for desktop/mobile |
| **`css-architecture`** | Scalable CSS architecture using modern container queries and CSS variables |
| **`dashboard-builder`** | Rapid scaffolding of analytics dashboards with charts, tables, and KPIs |
| **`design-system`** | Design system token architecture, component libraries, and consistency audits |
| **`design-token-management`** | Synchronizing design tokens between design tools and CSS variables |
| **`figma-tokens-sync`** | Automated pipeline translating Figma design variables into Tailwind classes |
| **`flutter-dart-code-review`** | Deep review of Flutter widgets, state management, and render trees |
| **`frontend-a11y`** | Frontend accessibility testing: ARIA attributes, keyboard navigation, contrast |
| **`frontend-design-direction`** | Establishing aesthetic visual guidelines, color palettes, and typography |
| **`frontend-patterns`** | Modern React patterns: hooks, context, compound components, and render props |
| **`frontend-performance`** | Optimizing Core Web Vitals, FID, LCP, CLS, and bundle size reduction |
| **`frontend-slides`** | Generating interactive presentation slide decks using HTML, CSS, and Markdown |
| **`liquid-glass-design`** | Advanced UI styling with glassmorphism, blur backdrops, and modern micro-effects |
| **`make-interfaces-feel-better`** | Micro-interaction polish: spring physics, subtle hover states, transitions |
| **`motion-advanced`** | Complex choreography, scroll-linked animations, and SVG morphing |
| **`motion-foundations`** | Foundational animation curves, timing scales, and reduced-motion fallbacks |
| **`motion-patterns`** | Framer Motion declarative layout transitions and exit animations |
| **`nextjs-turbopack`** | Next.js App Router patterns, Turbopack optimizations, and Server Actions |
| **`nuxt4-patterns`** | Nuxt 4 / Vue 3 Composition API, server routes, SSR hydration, and Pinia |
| **`radix-primitives`** | Accessible, unstyled UI primitives using Radix UI for custom design systems |
| **`react-native-patterns`** | React Native cross-platform patterns, Hermes engine, and gesture handling |
| **`react-patterns`** | React 19 Server Components, Actions, `use` hook, and optimistic UI updates |
| **`react-performance`** | Diagnosing React re-renders, React Profiler, and memoization strategies |
| **`react-testing`** | React Testing Library best practices: userEvent, queries, and async assertions |
| **`remotion-video-creation`** | Programmatic video rendering using React and Remotion |
| **`responsive-design`** | Mobile-first responsive layouts, flexbox, grid, and fluid typography |
| **`shadcn-patterns`** | Integrating and customizing accessible Tailwind components via Shadcn UI |
| **`svelte-runes`** | Modern Svelte 5 reactivity using runes (`$state`, `$derived`, `$effect`) |
| **`tailwind-v4`** | Tailwind CSS v4 CSS-first configuration, theme variables, and utility styling |
| **`taste`** | Visual aesthetic taste evaluation ensuring high-end UI craftsmanship |
| **`taste-application`** | Applying refined aesthetic taste principles to existing crude UI components |
| **`taste-distillation`** | Extracting design tokens and visual styles from reference screenshot URLs |
| **`tasteforge-video`** | Generating polished UI video demos and micro-interaction walkthroughs |
| **`threejs-webgl`** | 3D scene construction, shaders, lighting, and WebGL rendering with Three.js |
| **`ui-demo`** | Interactive UI component demonstration harnesses for developer testing |
| **`ui-to-vue`** | Transpiling and adapting React or HTML design wireframes into clean Vue 3 code |
| **`vite-patterns`** | Vite build configuration, plugin authoring, and HMR optimization |
| **`vue-patterns`** | Vue 3 Composition API, script setup, ref/reactive, and component composition |
| **`web-accessibility-wcag`** | WCAG 2.2 AA/AAA compliance auditing and remediation workflows |
| **`web-components`** | Custom Elements, Shadow DOM, and framework-agnostic Web Components |
| **`web-vitals-optimization`** | Profiling and optimizing Largest Contentful Paint and Cumulative Layout Shift |
| **`zustand-state-management`** | Lightweight React global state management using Zustand stores |

#### 4. Backend, Microservices, APIs & Language Idioms (74 Skills)
| Skill Identifier | Capability & Operational Purpose |
| :--- | :--- |
| **`api-connector-builder`** | Scaffolding robust third-party API connectors with retries and rate limits |
| **`api-design`** | RESTful and GraphQL API contract design with OpenAPI 3.1 specifications |
| **`backend-patterns`** | Clean Architecture, repository patterns, and domain service isolation |
| **`bun-runtime`** | High-performance JavaScript/TypeScript development using the Bun runtime |
| **`clean-architecture`** | Hexagonal architecture, ports & adapters, and domain-centric design |
| **`contract-first`** | OpenAPI and protobuf contract-first development enforcing interface locks |
| **`cpp-coding-standards`** | Modern C++20/23 coding guidelines: RAII, smart pointers, zero-overhead |
| **`cpp-testing`** | C++ testing frameworks: GoogleTest, Catch2, ctest, and mock objects |
| **`csharp-testing`** | xUnit and NUnit testing for .NET with FluentAssertions and Moq |
| **`data-throughput-accelerator`** | Optimizing backend data ingestion pipelines and batch serialization |
| **`deno-runtime`** | Deno secure runtime development with native TypeScript and Web APIs |
| **`django-celery`** | Asynchronous task queue architecture using Django, Celery, and Redis |
| **`django-patterns`** | Idiomatic Django app structure, models, managers, and service layers |
| **`django-security`** | Django security hardening: CSRF, clickjacking, SQL injection, and auth |
| **`django-tdd`** | Test-driven development for Django apps and Django REST Framework APIs |
| **`django-verification`** | Django verification suites: migration consistency, checks, and test suites |
| **`dotnet-patterns`** | ASP.NET Core Web API architecture, dependency injection, and Entity Framework |
| **`elixir-otp`** | Fault-tolerant distributed systems using Elixir, Erlang OTP, and GenServer |
| **`express-patterns`** | Production Express.js architecture: middleware pipelines, validation, error gates |
| **`fastapi-patterns`** | FastAPI async endpoints, Pydantic v2 validation, and OpenAPI documentation |
| **`fiber-go`** | Ultra-fast Go web services using the Fiber HTTP framework |
| **`fsharp-testing`** | F# property-based testing and unit testing using Expecto and FsCheck |
| **`golang-patterns`** | Idiomatic Go concurrency, channel patterns, interfaces, and error handling |
| **`golang-testing`** | Go table-driven unit tests, benchmarking, race detector, and mock interfaces |
| **`graphql-api-design`** | GraphQL schema design, resolvers, dataloader batching, and Apollo Server |
| **`grpc-services`** | High-throughput gRPC microservice communication using Protocol Buffers |
| **`hexagonal-architecture`** | Isolating business logic from databases and UI through explicit ports |
| **`java-coding-standards`** | Modern Java 21+ coding standards, records, virtual threads, and pattern matching |
| **`jpa-patterns`** | Hibernate/JPA entity relationships, lazy loading, and query optimization |
| **`kotlin-coroutines-flows`** | Asynchronous Kotlin programming using Coroutines, Flow, and Channels |
| **`kotlin-exposed-patterns`** | SQL database access using JetBrains Exposed Kotlin ORM |
| **`kotlin-ktor-patterns`** | Lightweight asynchronous microservices using the Kotlin Ktor framework |
| **`kotlin-patterns`** | Idiomatic Kotlin: extension functions, sealed interfaces, and data classes |
| **`kotlin-testing`** | Kotlin testing using Kotest, MockK, and coroutine test dispatchers |
| **`laravel-patterns`** | Modern Laravel 11 patterns: service providers, actions, and Eloquent |
| **`laravel-plugin-discovery`** | Building and distributing modular packages for the Laravel ecosystem |
| **`laravel-security`** | Laravel security review: mass assignment, authorization policies, validation |
| **`laravel-tdd`** | Test-driven development using Pest PHP and PHPUnit for Laravel applications |
| **`laravel-verification`** | Laravel verification loops: artisan test, pint formatting, and phpstan |
| **`latency-critical-systems`** | Zero-copy, lock-free, and cache-friendly designs for low-latency systems |
| **`microservices-design`** | Microservice domain decomposition, saga patterns, and event choreography |
| **`mybatis-patterns`** | MyBatis SQL mapper XML and annotation patterns for Java applications |
| **`nestjs-patterns`** | Enterprise TypeScript backend development using NestJS modules and decorators |
| **`nodejs-keccak256`** | High-speed cryptographic hashing and Keccak256 computations in Node.js |
| **`nodejs-streams`** | Memory-efficient data pipelines using Node.js readable/writable streams |
| **`perl-patterns`** | Modern Perl 5.36+ patterns, Moose/Moo object systems, and CPAN modules |
| **`perl-security`** | Taint mode, input sanitization, and SQL injection prevention in Perl |
| **`perl-testing`** | Test::More, Test2, and unit testing suites for Perl codebases |
| **`php-modern-patterns`** | PHP 8.2+ modern features: enums, readonly classes, fibers, and match |
| **`python-asyncio`** | Python asyncio event loops, tasks, semaphores, and async context managers |
| **`python-patterns`** | Pythonic idioms: context managers, decorators, dataclasses, and typing |
| **`python-testing`** | Pytest best practices: fixtures, parameterization, mocks, and coverage plugins |
| **`quarkus-patterns`** | Supersonic Subatomic Java microservice architecture using Quarkus |
| **`quarkus-security`** | Quarkus OIDC security, JWT validation, and RBAC authorization |
| **`quarkus-tdd`** | Test-driven development for Quarkus with @QuarkusTest and Testcontainers |
| **`quarkus-verification`** | Native image compilation verification and GraalVM build testing |
| **`rails-patterns`** | Rails 8 conventions, ActiveSupport, Hotwire, and Solid Queue patterns |
| **`regex-vs-llm-structured-text`** | Benchmarking deterministic regex parsing vs. LLM extraction for text |
| **`rest-api-design`** | Idempotent REST API architecture, HTTP status codes, and HATEOAS |
| **`ruby-dry-rb`** | Clean Ruby business logic using the dry-rb gem collection |
| **`rust-concurrency`** | Rust fearless concurrency: Arc, Mutex, RwLock, channels, and rayon |
| **`rust-patterns`** | Idiomatic Rust: pattern matching, trait objects, smart pointers, and iterators |
| **`rust-testing`** | Rust unit and integration tests, cargo test, proptest, and cargo-fuzz |
| **`serverless-patterns`** | AWS Lambda, Cloudflare Workers, and stateless serverless event handling |
| **`springboot-patterns`** | Spring Boot 3 architecture, auto-configuration, and dependency injection |
| **`springboot-security`** | Spring Security 6 filters, OAuth2 resource server, and method security |
| **`springboot-tdd`** | Test-driven development with SpringBootTest, MockMvc, and Testcontainers |
| **`springboot-verification`** | Spring Boot verification: actuator health checks, metrics, and profiles |
| **`swift-actor-persistence`** | Thread-safe data persistence using Swift 6 actors and structured concurrency |
| **`swift-concurrency-6-2`** | Modern Swift 6 concurrency: Sendable types, task groups, and isolation |
| **`swift-protocol-di-testing`** | Protocol-oriented dependency injection and mock testing in Swift |
| **`swift-vapor`** | High-performance server-side Swift development using the Vapor framework |
| **`tinystruct-patterns`** | Lightweight Java web application architecture using the TinyStruct framework |
| **`websocket-streaming`** | Real-time bidirectional WebSocket streaming architecture with reconnection |

#### 5. Database, Storage, Caches & Persistence (21 Skills)
| Skill Identifier | Capability & Operational Purpose |
| :--- | :--- |
| **`clickhouse-io`** | Real-time analytical clickstream and event storage with ClickHouse |
| **`database-migration`** | Zero-downtime database schema migration strategies: expand and contract |
| **`database-migrations`** | Automated migration authoring, rollback scripts, and integrity checks |
| **`database-sharding`** | Horizontal database partitioning, shard keys, and cross-shard queries |
| **`dbt-data-modeling`** | Data transformation pipelines and analytics engineering using dbt |
| **`duckdb-analytics`** | Embedded analytical queries on Parquet and CSV data using DuckDB |
| **`dynamodb-patterns`** | Single-table design and secondary indexing patterns for Amazon DynamoDB |
| **`elasticsearch-search`** | Full-text search, fuzzy matching, and inverted indexing with Elasticsearch |
| **`indexeddb-offline`** | Client-side offline data persistence using browser IndexedDB APIs |
| **`mongodb-aggregation`** | MongoDB document modeling and multi-stage aggregation pipelines |
| **`mysql-patterns`** | MySQL schema optimization, InnoDB indexing, and transaction isolation |
| **`neo4j-graph-queries`** | Graph database modeling and Cypher query optimization using Neo4j |
| **`orm-optimization`** | Diagnosing and eliminating N+1 query bottlenecks in ORMs |
| **`planetscale-patterns`** | Serverless MySQL database architecture using PlanetScale and Vitess |
| **`postgres-indexes`** | PostgreSQL B-tree, GIN, GiST, and BRIN indexing strategies for query speed |
| **`postgres-patterns`** | PostgreSQL advanced features: JSONB, window functions, and CTEs |
| **`prisma-patterns`** | Prisma schema modeling, relations, migrations, and typed client queries |
| **`redis-patterns`** | Redis caching strategies, cache stampede prevention, and sorted sets |
| **`sqlite-embedded`** | Embedded SQLite database architecture, WAL mode, and fast lookup tables |
| **`supabase-rls-policies`** | Supabase PostgreSQL Row Level Security (RLS) policy authoring |
| **`timescaledb-telemetry`** | Time-series telemetry storage, hypertables, and continuous aggregates |

#### 6. Cloud, DevOps, Networks, Homelab & Infrastructure (42 Skills)
| Skill Identifier | Capability & Operational Purpose |
| :--- | :--- |
| **`ansible-automation`** | Infrastructure configuration management and provisioning with Ansible |
| **`aws-cdk`** | Defining AWS cloud infrastructure as code using TypeScript and AWS CDK |
| **`aws-well-architected`** | Auditing cloud architectures against the AWS Well-Architected Framework |
| **`cisco-ios-patterns`** | Cisco router and switch configuration standards, VLANs, and ACLs |
| **`cloud-cost-finops`** | Cloud infrastructure cost optimization and idle resource elimination |
| **`cloudflare-workers`** | Edge computing serverless deployment using Cloudflare Workers and KV |
| **`container-hardening`** | Docker container security: non-root users, minimal base images, scanning |
| **`datadog-observability`** | Application performance monitoring, traces, and metrics with Datadog |
| **`deployment-patterns`** | Blue-green, canary, and rolling deployment strategies for zero downtime |
| **`docker-compose-patterns`** | Multi-container local development environments using Docker Compose |
| **`docker-multi-stage-builds`** | Optimizing container image sizes using multi-stage Dockerfiles |
| **`docker-patterns`** | Production Docker containerization best practices and security |
| **`flox-environments`** | Reproducible developer environments using Flox and Nix packages |
| **`git-ops-argo`** | GitOps continuous delivery for Kubernetes using ArgoCD and Git repositories |
| **`github-actions-ci`** | CI/CD workflow automation, matrix builds, and caching in GitHub Actions |
| **`github-ops`** | Managing GitHub repository settings, branch protection, and team permissions |
| **`grafana-dashboards`** | Designing Prometheus and Loki observability dashboards in Grafana |
| **`homelab-network-readiness`** | Pre-flight validation of local homelab server readiness and storage |
| **`homelab-network-setup`** | Initial configuration of home server networks, gateways, and subnets |
| **`homelab-pihole-dns`** | Local network-wide ad blocking and private DNS resolution via Pi-hole |
| **`homelab-vlan-segmentation`** | Isolating IoT devices and servers via managed switch VLAN trunks |
| **`homelab-wireguard-vpn`** | Secure remote homelab access using WireGuard VPN peer tunnels |
| **`istio-service-mesh`** | Kubernetes service mesh traffic routing, mTLS, and observability via Istio |
| **`kubernetes-deployments`** | Kubernetes manifests: Deployments, StatefulSets, Services, and Ingress |
| **`kubernetes-patterns`** | Kubernetes cloud-native design patterns: sidecar, adapter, and ambassador |
| **`netmiko-ssh-automation`** | Multi-vendor network device automation using Python and Netmiko |
| **`network-bgp-diagnostics`** | Diagnosing BGP neighbor peering, route flapping, and AS-path policies |
| **`network-config-validation`** | Validating network router configuration syntax before deployment |
| **`network-interface-health`** | Monitoring physical and virtual interface packet drops, CRC, and duplex |
| **`nginx-reverse-proxy`** | Nginx reverse proxy configuration: SSL termination, caching, load balancing |
| **`opentelemetry-tracing`** | Distributed tracing instrumentation across microservices with OpenTelemetry |
| **`packer-image-build`** | Automated golden machine image creation using HashiCorp Packer |
| **`pm2-process-management`** | Node.js application process supervision, clustering, and auto-restart |
| **`prometheus-alerting`** | Configuring Prometheus metric collection and Alertmanager rules |
| **`pulumi-iac`** | Multi-cloud infrastructure as code using Pulumi with TypeScript or Python |
| **`sentry-error-tracking`** | Configuring Sentry crash reporting, sourcemaps, and error triage |
| **`terraform-patterns`** | HashiCorp Terraform module architecture, state locking, and workspaces |
| **`uncloud`** | De-clouding workflows migrating cloud dependencies back to self-hosted servers |
| **`vault-secret-management`** | Dynamic secret leasing, encryption-as-a-service, and HashiCorp Vault |
| **`wireguard-vpn-config`** | Point-to-point and mesh VPN configuration using WireGuard protocols |
| **`workspace-surface-audit`** | Scanning repository workspace for untracked files, temp logs, and bloat |
| **`zero-trust-network`** | Zero-trust network architecture: micro-segmentation and identity verification |

#### 7. Machine Learning, RAG, Vision & Audio (36 Skills)
| Skill Identifier | Capability & Operational Purpose |
| :--- | :--- |
| **`bengali-nlp`** | Bengali language tokenization, POS tagging, and sentiment analysis |
| **`blender-motion-state-inspection`** | Automated inspection of Blender 3D animation rigs and keyframe curves |
| **`cuda-memory-optimization`** | Managing GPU VRAM allocation, gradient checkpointing, and FP16/BF16 |
| **`data-contracts-ml`** | Validating training data schemas, feature drift, and distribution shifts |
| **`diffusers-pipeline`** | Image generation pipelines using HuggingFace Diffusers and Stable Diffusion |
| **`fal-ai-media`** | High-speed cloud image and media generation using the Fal.ai API |
| **`fine-tuning-lora`** | Parameter-efficient fine-tuning (PEFT) of LLMs using LoRA and QLoRA |
| **`foundation-models-on-device`** | Deploying and running quantized SLMs locally on edge devices |
| **`huggingface-transformers`** | Loading, fine-tuning, and evaluating models using HuggingFace Transformers |
| **`iterative-retrieval`** | Multi-hop and iterative query expansion for complex RAG tasks |
| **`langchain-evals`** | Benchmarking LangChain agent chains and tool usage using LangSmith |
| **`llamaindex-patterns`** | Document indexing, hierarchical chunking, and querying with LlamaIndex |
| **`manim-video`** | Generating programmatic mathematical animations using Manim |
| **`ml-adoption-playbook`** | Framework for assessing and executing enterprise AI/ML adoption |
| **`mle-workflow`** | Production machine learning lifecycle: data contracts, training, eval, and rollout |
| **`mlflow-tracking`** | Tracking model hyperparameters, metrics, and artifact artifacts with MLflow |
| **`model-quantization`** | Quantizing deep neural networks to INT8 and INT4 using AWQ and GGUF |
| **`onnx-runtime`** | Cross-platform model optimization and high-performance inference with ONNX |
| **`opencv-vision-pipeline`** | Computer vision pipelines: edge detection, contours, and object tracking |
| **`pandas-data-wrangling`** | High-throughput tabular data cleaning, transformations, and joins with Pandas |
| **`pytorch-ddp-training`** | Multi-GPU distributed training using PyTorch DistributedDataParallel (DDP) |
| **`pytorch-patterns`** | Idiomatic PyTorch: custom Dataset, DataLoader, nn.Module, and training loops |
| **`rag-chunking-strategies`** | Optimizing text chunking: semantic, sentence-window, and parent-child chunks |
| **`rag-reranking`** | Improving vector search recall using Cohere and BGE cross-encoder rerankers |
| **`ragas-evaluations`** | Automated evaluation of RAG systems: faithfulness, answer relevancy, recall |
| **`recsys-pipeline-architect`** | Designing personalized recommendation engines: two-tower models and ranking |
| **`scientific-db-pubmed-database`** | Querying and extracting medical citations from the NCBI PubMed database |
| **`scientific-db-uspto-database`** | Searching and parsing patent records from the USPTO patent database |
| **`scientific-pkg-gget`** | Automated retrieval of genomic sequences and annotations using gget |
| **`scientific-thinking-literature-review`** | Structured synthesis and critical analysis of academic literature |
| **`scientific-thinking-scholar-evaluation`** | Evaluating scientific paper methodologies and statistical significance |
| **`tensorrt-inference`** | Compiling PyTorch models to NVIDIA TensorRT engines for sub-millisecond inference |
| **`triton-inference-server`** | Deploying scalable multi-model inference pipelines with NVIDIA Triton |
| **`video-editing`** | Programmatic video trimming, concatenation, and audio synchronization via FFmpeg |
| **`videodb`** | Indexing, searching, and extracting clips from video archives using VideoDB |
| **`whisper-audio-transcription`** | Speech-to-text audio transcription and subtitle generation using Whisper |

#### 8. Security, AgentShield, Cryptography & Compliance (29 Skills)
| Skill Identifier | Capability & Operational Purpose |
| :--- | :--- |
| **`api-security-audit`** | Auditing API endpoints for BOLA/IDOR, broken authentication, and rate limits |
| **`auth-oauth2-oidc`** | OAuth 2.0 and OpenID Connect flows: PKCE, authorization code, and token refresh |
| **`cryptographic-patterns`** | Industry-standard encryption: AES-GCM, RSA, Ed25519, and secure random |
| **`csp-headers`** | Configuring Content Security Policy headers to mitigate XSS and clickjacking |
| **`customs-trade-compliance`** | Auditing international import/export tariff codes and trade compliance |
| **`defi-amm-security`** | Auditing decentralized exchange AMM smart contracts for flash loan exploits |
| **`dependency-audit-snyk`** | Scanning third-party dependencies for known CVE vulnerabilities |
| **`evm-token-decimals`** | Preventing decimal precision loss and overflow bugs in EVM token math |
| **`gdpr-data-privacy`** | Enforcing GDPR compliance: right to be forgotten, data portability, consent |
| **`gitleaks-scanning`** | Automated pre-commit scanning blocking credential and API key leaks |
| **`healthcare-cdss-patterns`** | Designing clinical decision support systems adhering to safety guidelines |
| **`healthcare-emr-patterns`** | Interfacing with electronic medical record systems via HL7 and FHIR standards |
| **`healthcare-eval-harness`** | Clinical benchmark evaluations assessing medical AI recommendation safety |
| **`healthcare-phi-compliance`** | Sanitizing Protected Health Information (PHI) to meet HIPAA regulations |
| **`hipaa-compliance`** | Technical controls, audit logs, and encryption required for HIPAA compliance |
| **`jwt-security-hygiene`** | Preventing JWT vulnerabilities: algorithm confusion, weak secrets, token replay |
| **`owasp-top-10`** | Systematic auditing of codebases against the current OWASP Top 10 vulnerabilities |
| **`pci-dss-compliance`** | Payment card industry data security standards: tokenization and isolation |
| **`penetration-testing-checklist`** | Structured penetration testing methodology across web applications |
| **`prompt-injection-defense`** | Sanitizing external untrusted inputs to prevent prompt injection in agents |
| **`rbac-abac-authz`** | Implementing Role-Based and Attribute-Based Access Control policies |
| **`saml-enterprise-sso`** | Configuring enterprise single sign-on using SAML 2.0 assertions |
| **`secrets-rotation-workflow`** | Automated credential and API key rotation without service downtime |
| **`security-bounty-hunter`** | Proactive vulnerability hunting discovering high-severity bounty exploits |
| **`security-review`** | Comprehensive multi-axis security audit checklist for pull requests |
| **`security-scan`** | Automated static analysis scanning repository for vulnerabilities and credentials |
| **`sql-injection-prevention`** | Enforcing parameterized queries and prepared statements across all database calls |
| **`threat-modeling-stride`** | Identifying system threats using the STRIDE threat modeling methodology |
| **`xss-csrf-prevention`** | Input sanitization, output encoding, and SameSite cookie configurations |

#### 9. Mobile, iOS/Swift, Android/Kotlin & Multiplatform (16 Skills)
| Skill Identifier | Capability & Operational Purpose |
| :--- | :--- |
| **`android-clean-architecture`** | Android Clean Architecture: Domain, Data, Presentation, and UseCases |
| **`android-ndk`** | Integrating native C/C++ code into Android apps via JNI and Android NDK |
| **`apple-watchos-patterns`** | Developing complications, background sessions, and watchOS apps in SwiftUI |
| **`arkts-state-management`** | State management in HarmonyOS: `@State`, `@Prop`, `@Link`, and `@Provide` |
| **`dart-flutter-patterns`** | Dart idioms, asynchronous streams, and Flutter UI tree best practices |
| **`harmonyos-arkui`** | Declarative UI layout construction for HarmonyOS using ArkUI components |
| **`ios-icon-gen`** | Automated generation of complete iOS app icon assets from master SVG wireframes |
| **`ios-memory-profiling`** | Instruments profiling detecting memory leaks and retain cycles in Swift |
| **`jetpack-compose`** | Modern declarative Android UI development using Jetpack Compose components |
| **`kotlin-multiplatform-mobile`** | Sharing business logic across iOS and Android using Kotlin Multiplatform |
| **`react-native-hermes`** | Optimizing React Native JavaScript execution using the Hermes engine |
| **`react-native-turbomodules`** | Building high-performance native modules using React Native TurboModules |
| **`swift-swiftdata`** | Declarative local data modeling and persistence using Apple SwiftData |
| **`swiftui-design-patterns`** | SwiftUI view composition, navigation stacks, and environment injection |
| **`swiftui-patterns`** | Modern SwiftUI view lifecycles, state observation, and layout geometry |
| **`xcode-spm-build`** | Configuring Xcode projects, schemes, and Swift Package Manager targets |

#### 10. Operators, Desk Workflows, Domain Logistics & Media (85 Skills)
| Skill Identifier | Capability & Operational Purpose |
| :--- | :--- |
| **`article-writing`** | Authoring authoritative, technical engineering articles and tutorials |
| **`automation-audit-ops`** | Auditing manual enterprise processes and designing automation scripts |
| **`brand-discovery`** | Extracting core brand positioning, value propositions, and customer personas |
| **`brand-voice`** | Codifying brand tone of voice, stylistic guidelines, and vocabulary rules |
| **`carrier-relationship-management`** | Managing freight carrier performance, SLA contracts, and scorecards |
| **`click-path-audit`** | Analyzing user funnel click paths, identifying drop-offs, and optimizing UX |
| **`competitive-platform-analysis`** | Systematic feature, pricing, and capability benchmarking of competitors |
| **`competitive-report-structure`** | Structuring executive competitive intelligence reports for stakeholders |
| **`config-gc`** | Garbage-collecting orphaned configuration files and obsolete environment variables |
| **`content-engine`** | Automated multi-channel content generation and publishing pipeline |
| **`content-hash-cache-pattern`** | Content-addressable hashing for deterministic asset and data caching |
| **`counterparty-channel-discipline`** | Protocol for formal, auditable communication in external deal channels |
| **`crosspost`** | Automated cross-posting of technical articles to Dev.to, Medium, and Hashnode |
| **`customer-billing-ops`** | Managing subscription billing cycles, invoicing, and dunning workflows |
| **`data-scraper-agent`** | Resilient web scraping pipelines with retry mechanisms and proxy rotation |
| **`deep-research`** | Multi-stage iterative web research and technical document synthesis |
| **`desk-channel-discipline`** | Maintaining formal separation between internal operations and client comms |
| **`document-translation-pipeline`** | Automated translation pipeline for technical docs preserving code blocks |
| **`email-ops`** | Transactional email delivery, SPF/DKIM authentication, and bounce tracking |
| **`energy-procurement`** | Analyzing commercial energy consumption profiles and utility tariffs |
| **`epic-breakdown`** | Decomposing complex business epics into atomic engineering tickets |
| **`esign-field-placement`** | Deterministic placement of e-signature fields and audit trails on PDFs |
| **`esports-tournament-ops`** | Tournament bracket generation, live match scheduling, and dispute rules |
| **`exa-search`** | Neural web search querying Exa.ai for technical papers and domain lookups |
| **`finance-billing-ops`** | Reconciling merchant payment gateways, chargebacks, and financial ledgers |
| **`freight-rate-procurement`** | Benchmarking and negotiating freight shipping rates across logistics lanes |
| **`generating-python-installer`** | Packaging Python applications into standalone cross-platform installers |
| **`google-workspace-ops`** | Automating Google Drive, Docs, and Sheets integration via Google APIs |
| **`growth-log`** | Tracking marketing experiments, growth hypotheses, and conversion metrics |
| **`i18n-sync`** | Synchronizing localization keys across frontend and backend translation catalogs |
| **`inventory-demand-planning`** | Forecasting inventory replenishment cycles using historical demand velocity |
| **`investor-materials`** | Authoring investor update letters, financial metrics, and pitch collateral |
| **`investor-outreach`** | Structuring investor pipeline tracking and outbound fundraising communications |
| **`ito-baskets`** | Consolidated read-only market baskets and prediction pricing via the Itô API |
| **`ito-compute`** | Provisioning and querying authenticated GPU inventory and RFQs on Itô |
| **`ito-inference`** | Executing remote model inference workloads across Itô compute clusters |
| **`ito-training`** | Orchestrating distributed training jobs across Itô high-performance compute nodes |
| **`jira-integration`** | Two-way synchronization between agent issue trackers and corporate Jira |
| **`knowledge-ops`** | Enterprise knowledge base curation, documentation hygiene, and search indexing |
| **`lead-intelligence`** | Enriching sales leads with firmographic data and intent signals |
| **`logistics-exception-management`** | Automating freight delay alerts, rerouting, and carrier notifications |
| **`mailtrap-email-integration`** | Safe email testing and sandbox delivery verification using Mailtrap |
| **`market-maker-surveillance`** | Monitoring liquidity provider spreads, order books, and depth of market |
| **`market-research`** | Synthesizing market size, TAM/SAM/SOM, and customer segment dynamics |
| **`marketing-campaign`** | Planning and executing multi-week product marketing launch campaigns |
| **`master-agreement-generator`** | Drafting Master Services Agreements (MSA) with rolling schedule addendums |
| **`media-generation`** | Coordinating AI-assisted image, voiceover, and video media creation |
| **`messages-ops`** | SMS and WhatsApp transactional messaging pipelines via Twilio and Meta APIs |
| **`nasiko-control-plane`** | Experimental Nasiko CLI lifecycle bridge for pinned installs and status |
| **`newsletter-publishing`** | Curating, formatting, and publishing engineering newsletter editions |
| **`nutrient-document-processing`** | Processing, merging, and signing complex PDF documents via Nutrient SDK |
| **`opensource-pipeline`** | Automated pipeline managing open-source contribution review and triage |
| **`orch-add-feature`** | Guided orchestration pipeline adding new feature modules to codebases |
| **`orch-build-mvp`** | Rapid orchestration pipeline scaffolding a complete functional MVP |
| **`orch-change-feature`** | Orchestration workflow modifying existing feature behavior with tests |
| **`orch-fix-defect`** | Targeted orchestration pipeline isolating and resolving software defects |
| **`orch-refine-code`** | Orchestration workflow refining code readability without changing behavior |
| **`osint-investigation`** | Public-source intelligence gathering, entity resolution, and falsification |
| **`pdf-form-automation`** | Programmatically filling and extracting data from AcroForm PDF templates |
| **`pitch-deck-structure`** | Structuring 10-slide startup pitch decks for venture capital presentations |
| **`prediction-market-oracle-research`** | Researching decentralized oracle resolution mechanisms and truth feeds |
| **`prediction-market-risk-review`** | Auditing prediction market contracts for settlement ambiguity and exploit vectors |
| **`procurement-rfp-evaluation`** | Evaluating vendor RFP submissions against quantitative technical criteria |
| **`product-capability`** | Mapping product feature capabilities to customer workflow requirements |
| **`product-lens`** | Evaluating code changes through user experience and product value perspectives |
| **`production-audit`** | Comprehensive operational audit of production systems before going live |
| **`production-scheduling`** | Factory production scheduling, job sequencing, and machine utilization |
| **`project-flow-ops`** | Optimizing developer flow state and removing blocking process friction |
| **`quality-nonconformance`** | Tracking manufacturing and software quality non-conformance reports (NCR) |
| **`ralphinho-rfc-pipeline`** | Formal RFC proposal drafting, commenting, and architectural review pipeline |
| **`repo-scan`** | Fast high-level scanning of foreign repositories to summarize architecture |
| **`research-ops`** | Organizing, synthesizing, and archiving technical research findings |
| **`returns-reverse-logistics`** | Managing customer return workflows, RMA authorization, and restocking |
| **`rules-distill`** | Distilling repeated conversation corrections into permanent project rules |
| **`sales-funnel-attribution`** | Tracking multi-touch customer attribution across sales and marketing funnels |
| **`search-first`** | Mandatory search protocol: check docs and existing code before writing imports |
| **`seo`** | Search engine optimization audit: meta tags, crawl budget, and keyword hierarchy |
| **`social-graph-ranker`** | Ranking social engagement and influence within developer communities |
| **`social-publisher`** | Automating multi-platform social media announcements and thread scheduling |
| **`supply-chain-wms`** | Warehouse Management System (WMS) workflows: picking, packing, and bin locations |
| **`unified-notifications-ops`** | Aggregating push, email, and Slack alerts into a single unified queue |
| **`visa-doc-translate`** | Certified translation and formatting of official immigration and visa documents |
| **`warehouse-capacity-planning`** | Optimizing warehouse cubic capacity utilization and aisle layouts |
| **`x-api`** | Publishing, monitoring, and analyzing developer threads via the X (Twitter) API |
| **`youtube-script-structuring`** | Structuring engaging technical YouTube video scripts with hooks and demos |

#### Master Alphabetical Directory of All 311 ECC Skills:
```text
accessibility                       agent-architecture-audit            agent-eval                          
agent-harness-construction          agent-introspection-debugging       agent-payment-x402                  
agent-runtime-gateway-smoke-test    agent-security-hardening            agent-self-evaluation               
agent-sort                          agentic-engineering                 agentic-os                          
ai-first-engineering                ai-regression-testing               android-clean-architecture          
angular-developer                   api-connector-builder               api-design                          
architecture-decision-records       article-writing                     automation-audit-ops                
autonomous-agent-harness            autonomous-loops                    backend-patterns                    
benchmark                           benchmark-methodology               benchmark-optimization-loop         
bengali-nlp                         blender-motion-state-inspection     blueprint                           
brand-discovery                     brand-voice                         brownfield-retrospective            
browser-qa                          bun-runtime                         canary-watch                        
carrier-relationship-management     change-impact                       cisco-ios-patterns                  
ck                                  claude-devfleet                     click-path-audit                    
clickhouse-io                       code-tour                           codebase-onboarding                 
codehealth-mcp                      coding-standards                    competitive-platform-analysis       
competitive-report-structure        compose-multiplatform-patterns      config-gc                           
configure-ecc                       connections-optimizer               content-engine                      
content-hash-cache-pattern          context-and-decisions               context-budget                      
continuous-agent-loop               continuous-learning                 continuous-learning-v2              
contract-first                      conventional-commit-review          cost-aware-llm-pipeline             
cost-tracking                       council                             council-multi-model                 
counterparty-channel-discipline     cpp-coding-standards                cpp-testing                         
crosspost                           csharp-testing                      customer-billing-ops                
customs-trade-compliance            dart-flutter-patterns               dashboard-builder                   
data-scraper-agent                  data-throughput-accelerator         database-migrations                 
deep-research                       deepseek-harness-setup              defi-amm-security                   
delivery-gate                       deployment-patterns                 design-system                       
dev-team                            django-celery                       django-patterns                     
django-security                     django-tdd                          django-verification                 
dmux-workflows                      docker-patterns                     docs-governance                     
documentation-lookup                dotnet-patterns                     dynamic-workflow-mode               
e2e-testing                         ecc-guide                           ecc-recipes                         
ecc-tools-cost-audit                email-ops                           energy-procurement                  
enterprise-agent-ops                error-handling                      esign-field-placement               
eval-harness                        evm-token-decimals                  exa-search                          
fal-ai-media                        fastapi-patterns                    finance-billing-ops                 
flox-environments                   flutter-dart-code-review            foundation-models-on-device         
frontend-a11y                       frontend-design-direction           frontend-patterns                   
frontend-slides                     fsharp-testing                      gan-style-harness                   
gateguard                           generating-python-installer         git-workflow                        
github-ops                          golang-patterns                     golang-testing                      
google-workspace-ops                growth-log                          healthcare-cdss-patterns            
healthcare-emr-patterns             healthcare-eval-harness             healthcare-phi-compliance           
hermes-imports                      hexagonal-architecture              hipaa-compliance                    
homelab-network-readiness           homelab-network-setup               homelab-pihole-dns                  
homelab-vlan-segmentation           homelab-wireguard-vpn               hookify-rules                       
i18n-sync                           inherit-legacy-style                intent-driven-development           
inventory-demand-planning           investor-materials                  investor-outreach                   
ios-icon-gen                        iterative-retrieval                 ito-baskets                         
ito-compute                         ito-inference                       ito-training                        
java-coding-standards               jira-integration                    jpa-patterns                        
knowledge-ops                       kotlin-coroutines-flows             kotlin-exposed-patterns             
kotlin-ktor-patterns                kotlin-patterns                     kotlin-testing                      
kubernetes-patterns                 laravel-patterns                    laravel-plugin-discovery            
laravel-security                    laravel-tdd                         laravel-verification                
latency-critical-systems            lead-intelligence                   liquid-glass-design                 
living-docs-governance              llm-trading-agent-security          logistics-exception-management      
loop-design-check                   mailtrap-email-integration          make-interfaces-feel-better         
manim-video                         market-research                     marketing-campaign                  
master-agreement-generator          mcp-dependency-review               mcp-server-patterns                 
messages-ops                        ml-adoption-playbook                mle-workflow                        
module-regression                   motion-advanced                     motion-foundations                  
motion-patterns                     mybatis-patterns                    mysql-patterns                      
nanoclaw-repl                       nasiko-control-plane                nestjs-patterns                     
netmiko-ssh-automation              network-bgp-diagnostics             network-config-validation           
network-interface-health            nextjs-turbopack                    nodejs-keccak256                    
nutrient-document-processing        nuxt4-patterns                      openclaw-persona-forge              
opensource-pipeline                 operator-approval-loop              orch-add-feature                    
orch-build-mvp                      orch-change-feature                 orch-fix-defect                     
orch-pipeline                       orch-refine-code                    osint-investigation                 
parallel-execution-optimizer        perl-patterns                       perl-security                       
perl-testing                        plan-canvas                         plan-orchestrate                    
plankton-code-quality               postgres-patterns                   prediction-market-oracle-research   
prediction-market-risk-review       prisma-patterns                     product-capability                  
product-lens                        production-audit                    production-scheduling               
project-flow-ops                    prompt-caching-strategy             prompt-optimizer                    
python-patterns                     python-testing                      pytorch-patterns                    
quality-nonconformance              quarkus-patterns                    quarkus-security                    
quarkus-tdd                         quarkus-verification                rails-patterns                      
ralphinho-rfc-pipeline              react-native-patterns               react-patterns                      
react-performance                   react-testing                       recsys-pipeline-architect           
recursive-decision-ledger           redis-patterns                      regex-vs-llm-structured-text        
remotion-video-creation             repo-scan                           research-ops                        
returns-reverse-logistics           rules-distill                       rust-patterns                       
rust-testing                        safety-guard                        santa-method                        
scientific-db-pubmed-database       scientific-db-uspto-database        scientific-pkg-gget                 
scientific-thinking-literature-reviewscientific-thinking-scholar-evaluationsearch-first                        
security-bounty-hunter              security-review                     security-scan                       
seo                                 skill-comply                        skill-host-compat                   
skill-scout                         skill-stocktake                     social-graph-ranker                 
social-publisher                    springboot-patterns                 springboot-security                 
springboot-tdd                      springboot-verification             story-lifecycle                     
strategic-compact                   swift-actor-persistence             swift-concurrency-6-2               
swift-protocol-di-testing           swiftui-patterns                    taste                               
taste-application                   taste-distillation                  tasteforge-video                    
tdd-workflow                        team-agent-orchestration            team-builder                        
terminal-opener                     terminal-ops                        test-collaboration                  
tinystruct-patterns                 token-budget-advisor                token-card                          
ui-demo                             ui-to-vue                           uncloud                             
unified-memory                      unified-notifications-ops           verification-loop                   
video-editing                       videodb                             visa-doc-translate                  
vite-patterns                       vue-patterns                        windows-desktop-e2e                 
workspace-surface-audit             x-api                               
```

---

### 3.5 The Complete 97 Slash Commands Catalog
ECC exposes 97 slash command shims mapping multi-step engineering procedures into single terminal keystrokes. Each command executes with dynamic argument injection (`$ARGUMENTS`) and live shell output buffers:

| Command | Functional Domain | Operational Execution & Behavior |
| :--- | :--- | :--- |
| **`/aside`** | Context | Opens an ephemeral scratchpad query without polluting active conversation context |
| **`/astra-review`** | Cross-Model | Routes local diffs via Codex CLI to GPT-6-Astra for an out-of-band external model audit |
| **`/auto-update`** | Maintenance | Checks for ECC package updates and synchronizes skills with the upstream repository |
| **`/build-fix`** | Repair | Automatically parses compiler/linter error output and routes fixes to specialized resolver subagents |
| **`/checkpoint`** | Memory | Creates an instant, persistent snapshot of current working state, files, and thoughts |
| **`/code-review`** | Review | Audits uncommitted git changes or remote PRs with fresh-context objectivity |
| **`/cost-report`** | Telemetry | Generates a detailed session spend report in USD with token cache hit/miss analysis |
| **`/cpp-build`** | C++ | Runs CMake/Clang/GCC builds and parses compiler and linker diagnostic flags |
| **`/cpp-review`** | C++ | Audits C++ code for RAII compliance, pointer bounds, modern C++20/23 features |
| **`/cpp-test`** | C++ | Executes GoogleTest or Catch2 test suites with address sanitizer checks |
| **`/ecc-guide`** | Docs | Launches the interactive ECC documentation guide and command reference manual |
| **`/epic-claim`** | Orchestration | Claims an unassigned backlog epic, locking it across multi-agent workspaces |
| **`/epic-decompose`** | Orchestration | Decomposes a major business epic into atomic, dependency-sequenced task tickets |
| **`/epic-publish`** | Orchestration | Publishes decomposed epics and tickets to project backlog files or remote issue trackers |
| **`/epic-review`** | Orchestration | Audits completed epic work against the original acceptance criteria and test coverage |
| **`/epic-sync`** | Orchestration | Synchronizes epic execution status with local git worktrees and remote issue trackers |
| **`/epic-unblock`** | Orchestration | Diagnoses blocked tickets in an active epic, reassigning tasks or resolving dependencies |
| **`/epic-validate`** | Orchestration | Runs formal acceptance criteria validation across all tickets in a completed epic |
| **`/evolve`** | Learning | Self-refinement loop updating agent prompts and instincts based on recent performance logs |
| **`/fastapi-review`** | FastAPI | Audits FastAPI routes, Pydantic schemas, dependency injection, and OpenAPI specs |
| **`/feature-dev`** | Feature Dev | End-to-end guided feature development emphasizing architectural fit and dependency checks |
| **`/flutter-build`** | Flutter | Runs Flutter pub get and flutter build, diagnosing cross-platform compile errors |
| **`/flutter-review`** | Flutter | Audits Flutter widget trees, BLoC/Riverpod state flow, and animation lifecycles |
| **`/flutter-test`** | Flutter | Executes Flutter widget and unit test suites with coverage thresholds |
| **`/gan-build`** | GAN | Executes the adversarial GAN implementation loop until the discriminator verifies all tests |
| **`/gan-design`** | GAN | Initializes an adversarial GAN loop defining design constraints, generators, and evaluators |
| **`/go-build`** | Go | Runs Go build checks, verifies module dependencies, and diagnoses compilation failures |
| **`/go-review`** | Go | Audits Go code for goroutine safety, channel leaks, error handling, and idioms |
| **`/go-test`** | Go | Executes Go table-driven unit tests, race detector (`-race`), and coverage reports |
| **`/gradle-build`** | Java/Gradle | Diagnoses and repairs complex multi-project Gradle build tasks and dependency trees |
| **`/harness-audit`** | Diagnostics | Audits agent harness configuration, tool permissions, PATH integrity, and environment |
| **`/hookify`** | Hooks | Interactive interactive builder creating custom lifecycle hooks for tool executions |
| **`/hookify-configure`** | Hooks | Configures hook triggers, execution timeouts, and blocking vs. async modes |
| **`/hookify-help`** | Hooks | Displays syntax reference, matcher regex examples, and available hook lifecycle events |
| **`/hookify-list`** | Hooks | Lists all currently registered hooks with their matchers, targets, and active states |
| **`/instinct-export`** | Instincts | Exports learned project instincts to a portable JSON file for team sharing |
| **`/instinct-import`** | Instincts | Imports team-shared behavioral instincts into the local agent harness |
| **`/instinct-status`** | Instincts | Displays active behavioral instincts and heuristic preferences learned by the agent |
| **`/jira`** | Integration | Interacts with Atlassian Jira to fetch tickets, update sprint status, and add worklogs |
| **`/kotlin-build`** | Kotlin | Runs Gradle builds for Kotlin JVM and Kotlin Multiplatform targets |
| **`/kotlin-review`** | Kotlin | Audits Kotlin coroutines, Flow pipelines, Exposed ORM queries, and null-safety |
| **`/kotlin-test`** | Kotlin | Runs Kotest and JUnit test suites with coroutine test dispatchers |
| **`/learn`** | Learning | Parses solved debugging challenges from current session and synthesizes permanent rules |
| **`/learn-eval`** | Learning | Evaluates session learning effectiveness and verifies synthesized rules against regressions |
| **`/loop-start`** | Loops | Starts an autonomous execution loop with fixed or dynamic interval polling |
| **`/loop-status`** | Loops | Inspects running autonomous loops, iteration count, resource spend, and stall health |
| **`/marketing-campaign`** | Marketing | Generates multi-stage product launch campaigns, social threads, and blog drafts |
| **`/model-route`** | Routing | Manages dynamic model routing thresholds, aliases, and fallback providers |
| **`/multi-backend`** | CCG Multi-Model | Routes backend service implementation to specialized backend model personas |
| **`/multi-execute`** | CCG Multi-Model | Dispatches parallel implementation slices across multiple model endpoints |
| **`/multi-frontend`** | CCG Multi-Model | Routes UI/UX implementation to specialized frontend design model personas |
| **`/multi-plan`** | CCG Multi-Model | Collaborative multi-model architectural planning session using the CCG workflow runtime |
| **`/multi-workflow`** | CCG Multi-Model | End-to-end multi-model orchestration pipeline combining CCG planners and workers |
| **`/orch-add-feature`** | Orchestration | Orchestrates adding a complete feature module from spec to verified tests |
| **`/orch-build-mvp`** | Orchestration | Rapid orchestration pipeline bootstrapping an MVP from scratch in defined sequence |
| **`/orch-change-feature`** | Orchestration | Orchestrates modifying existing feature behavior with regression safety nets |
| **`/orch-fix-defect`** | Orchestration | Targeted orchestration isolating a bug, creating a failing repro test, and fixing it |
| **`/orch-refine-code`** | Orchestration | Orchestrates structural refactoring across modules without altering public interfaces |
| **`/orch-review`** | Orchestration | Runs an architectural orchestration review over newly landed multi-agent PRs |
| **`/plan`** | Architecture | Enforces requirement restatement, risk analysis, and step-by-step roadmap before coding |
| **`/plan-canvas`** | Architecture | Renders an interactive HTML plan artifact in the browser for visual annotation and signoff |
| **`/plan-prd`** | Architecture | Generates a lean, problem-first Product Requirements Document (PRD) before planning |
| **`/pm2`** | Process Ops | Manages local background Node.js/Python server daemons and log streams via PM2 |
| **`/pr`** | Git | Automates drafting, formatting, and submitting pull requests with verified test summaries |
| **`/project-init`** | Setup | Analyzes repo structure, tech stack, and dev environment to scaffold initial governance files |
| **`/projects`** | Setup | Lists and switches between active project workspaces and tracked contexts |
| **`/promote`** | Learning | Promotes project-local learned rules into global user rules across all repositories |
| **`/prp-commit`** | PRP Pipeline | Product Requirement Package: drafts atomic git commits verified against the PRP spec |
| **`/prp-implement`** | PRP Pipeline | Product Requirement Package: executes code changes against the approved PRP ticket |
| **`/prp-plan`** | PRP Pipeline | Product Requirement Package (PRP): drafts the architectural implementation plan |
| **`/prp-pr`** | PRP Pipeline | Product Requirement Package: compiles verified diffs into a complete pull request |
| **`/prp-prd`** | PRP Pipeline | Product Requirement Package: formalizes user requirements into a structured PRD |
| **`/prune`** | Maintenance | Cleans obsolete, redundant, or conflicting learned rules and instincts from storage |
| **`/python-review`** | Python | Audits Python code for PEP 8 compliance, type annotations, and asyncio efficiency |
| **`/quality-gate`** | Quality | Evaluates code against explicit project quality metrics before merges |
| **`/react-build`** | React | Runs React build toolchains (Vite/Next.js/Turbopack) and fixes hydration mismatches |
| **`/react-review`** | React | Audits React components for React 19 patterns, hook dependencies, and re-renders |
| **`/react-test`** | React | Runs Vitest/Jest unit tests with React Testing Library assertions |
| **`/refactor-clean`** | Maintenance | Safely cleans dead code, unused exports, and deprecated dependencies across the codebase |
| **`/resume-session`** | Memory | Restores full mental context, uncommitted goals, and task state from previous session |
| **`/review-pr`** | Review | Runs a multi-perspective review across security, performance, and naming conventions |
| **`/rust-build`** | Rust | Runs `cargo check` and `cargo build`, resolving borrow checker and lifetime errors |
| **`/rust-review`** | Rust | Audits Rust code for safety invariants, unsafe blocks, trait bounds, and zero-cost idioms |
| **`/rust-test`** | Rust | Executes `cargo test` with property-based testing and integration benchmarks |
| **`/santa-loop`** | Multi-Agent | Spawns dual independent reviewer subagents with conflicting prompts until convergence |
| **`/save-session`** | Memory | Extracts session decisions, file churn, and next steps into persistent dotfiles |
| **`/security-scan`** | Security | Executes comprehensive security scan for hardcoded secrets and OWASP vulnerabilities |
| **`/sessions`** | Memory | Lists all saved historical sessions with timestamps, token usage, and summary tags |
| **`/setup-pm`** | Process Ops | Initializes and configures PM2 process supervisor files for local development servers |
| **`/skill-create`** | Skills | Scaffolds a new custom skill conforming to Anthropic's official 6-insight standard |
| **`/skill-health`** | Skills | Audits all installed skills for missing files, schema errors, and outdated dependencies |
| **`/skill-host-compat`** | Skills | Validates installed skills across Claude Code, Codex, Cursor, and Zed harnesses |
| **`/test-coverage`** | Testing | Identifies untested execution paths and automatically generates missing test cases |
| **`/token-card`** | Telemetry | Displays a visual terminal card summarizing active token consumption by category |
| **`/update-codemaps`** | Docs | Scans repository and regenerates architecture codemaps and component relationship diagrams |
| **`/update-docs`** | Docs | Synchronizes documentation, READMEs, and inline docstrings with current codebase state |
| **`/vue-review`** | Vue | Audits Vue 3 code for Composition API best practices, reactivity triggers, and Pinia stores |

---

### 3.6 The Modular Rules Engine (`rules/` - 122 Rules Across 22 Stacks)
Rather than stuffing hundreds of instructions into a single bloated `CLAUDE.md` that suffers from attention dilution, ECC pioneers the **modular rules directory** (`.claude/rules/ecc/<stack>/`). Rules are selectively loaded into context only when relevant files are touched.

#### 1. The `rules/common/` Baseline (10 Universal Rules):
Every ECC setup begins with `rules/common/`, establishing universal engineering standards across all languages:
- **`development-workflow.md`**: Enforces the 7-step cycle (`plan -> test -> implement -> review -> verify -> remember -> improve`).
- **`testing.md`**: Enforces strict test-first development, minimum 80% coverage, and Red-Green-Refactor cycles.
- **`security.md`**: Enforces OWASP Top 10 mitigation, secret scanning, parameterized queries, and input validation.
- **`coding-style.md`**: Universal formatting, naming conventions, small function depth, and immutability rules.
- **`code-review.md`**: Self-audit checklist before pull requests and commits.
- **`git-workflow.md`**: Conventional commit standards, branch naming, and atomic diff policies.
- **`performance.md`**: Performance profiling guidelines, avoiding premature optimization while blocking N+1 queries.
- **`patterns.md`**: Clean architecture, dependency injection, and separation of concerns.
- **`agents.md`**: Subagent delegation boundaries and task handoff contracts.
- **`hooks.md`**: Lifecycle hook standards, timeout budgets, and non-blocking safety rules.

#### 2. The 21 Language & Framework Rule Packs (112 Specialized Rule Files):
ECC includes 21 dedicated language and framework packs. Each pack typically provides 5 standardized files (`coding-style.md`, `hooks.md`, `patterns.md`, `security.md`, `testing.md`):

| Language / Framework Stack | Rule Pack Path | Rules Included | Key Focus & Enforced Patterns |
| :--- | :--- | :---: | :--- |
| **TypeScript** | `rules/typescript/` | 5 | Strict null checks, generics, branded types, ESLint rules, Zod validation. |
| **Python** | `rules/python/` | 6 | Type hints, PEP 8, `fastapi.md` async patterns, Pytest fixtures, Pydantic v2. |
| **React** | `rules/react/` | 5 | React 19 Server Components, Actions, hook dependencies, memoization, hydration. |
| **React Native** | `rules/react-native/` | 8 | Hermes optimization, TurboModules, accessibility, production readiness, Gestures. |
| **Web / Frontend** | `rules/web/` | 7 | Core Web Vitals, semantic HTML5, design quality, container queries, CSS variables. |
| **Go (Golang)** | `rules/golang/` | 5 | Goroutine leaks, channel patterns, table-driven tests, error wrapping (`%w`). |
| **Rust** | `rules/rust/` | 5 | Borrow checker idioms, unsafe block isolation, `thiserror`/`anyhow`, Cargo tests. |
| **Swift / Apple** | `rules/swift/` | 5 | Swift 6 Concurrency, actors, Sendable checks, SwiftUI state, Apple HIG. |
| **Kotlin / Android** | `rules/kotlin/` | 5 | Coroutines, Flow, Compose Multiplatform, Jetpack Compose, Exposed ORM. |
| **Java / Spring** | `rules/java/` | 5 | Java 21 virtual threads, Spring Boot 3 auto-config, JPA/Hibernate performance. |
| **C / C++** | `rules/cpp/` | 5 | Modern C++20/23, RAII, pointer bounds, GoogleTest, memory sanitizers. |
| **C# / .NET** | `rules/csharp/` | 5 | ASP.NET Core Web API, async/await patterns, nullable reference types, xUnit. |
| **F#** | `rules/fsharp/` | 5 | Functional programming idioms, immutability, pattern matching, Expecto testing. |
| **PHP / Laravel** | `rules/php/` | 5 | Modern PHP 8.2+, PSR-12, Laravel 11 action patterns, Pest PHP, Eloquent queries. |
| **Ruby / Rails** | `rules/ruby/` | 5 | Ruby 3.x patterns, Rails 8 conventions, Solid Queue/Cache, RuboCop rules. |
| **Vue.js** | `rules/vue/` | 5 | Vue 3 Composition API, `<script setup>`, Pinia store patterns, reactivity. |
| **Nuxt** | `rules/nuxt/` | 5 | Nuxt SSR hydration, nitro server routes, auto-imports, SEO head management. |
| **Angular** | `rules/angular/` | 5 | Angular 17+ standalone components, signals, inject-based DI, RxJS interop. |
| **HarmonyOS (ArkTS)** | `rules/arkts/` | 5 | ArkTS V2 state management (`@State`, `@Prop`), ArkUI declarative layouts, Stage model. |
| **Dart / Flutter** | `rules/dart/` | 5 | Flutter widget tree rebuilds, BLoC/Riverpod state, platform channels. |
| **Perl** | `rules/perl/` | 5 | Modern Perl 5.36+, Moose/Moo object systems, taint mode security, Test::More. |

---

### 3.7 The Lifecycle Hooks Runtime & AgentShield Security System
ECC's most technically sophisticated layer is its **Lifecycle Hooks Runtime** (`hooks/hooks.json`), executing local Node.js interceptor scripts at exact event moments during an agent turn:

```
┌────────────────────────────────────────────────────────────────────────┐
│                       ECC HOOK LIFECYCLE PIPELINE                      │
└───────────────────────────────────┬────────────────────────────────────┘
                                    │
                                    ▼
1. SessionStart  ──► `plugin-hook-bootstrap.js` (Verify environment, PATH & scope)
                     `plan-canvas-sessions.js` (Load active interactive plan canvas)
                                    │
                                    ▼
2. UserPrompt    ──► `prompt:hookify-runtime` (Scan user prompt for custom triggers)
                                    │
                                    ▼
3. PreToolUse    ──► AgentShield Command Scanner: Block `rm -rf`, force-push, token leaks
                     `pre:config-protection`: Prevent clobbering `.env`, `.claude.json`
                     `pre:doc-file-warning`: Prevent accidental doc overwrites
                     `pre:edit-write:gateguard`: Block code edits if tests aren't written
                     `pre:suggest-compact`: Warn when file churn crosses context ceiling
                     `pre:mcp-health-check`: Ping MCP servers before tool invocation
                                    │
                                    ▼
4. [ TOOL EXECUTES IN TERMINAL / POSIX ENVIRONMENT ]
                                    │
                                    ▼
5. PostToolUse   ──► Track file diffs, update session memory ledger, record tool latency
   (On Failure)  ──► `post:mcp-health-check` & `post:skill:track` (Route to error resolver)
                                    │
                                    ▼
6. Stop (Turn)   ──► `stop-format-typecheck.js` (Auto-run linter/typecheck on finish)
                     `check-console-log.js` (Flag forgotten debugging statements)
                     `cost-tracker.js` (Calculate exact turn USD cost & token delta)
                     `evaluate-session.js` (Evaluate turn quality against instincts)
                     `desktop-notify.js` (Trigger OS desktop notification on completion)
                                    │
                                    ▼
7. PreCompact    ──► `pre:compact` (Capture active mental state & decisions before prune)
                                    │
                                    ▼
8. SessionEnd    ──► `session-end-marker.js` (Write historical progress log and handoff)
```

#### The 4 Hook Enforcement Profiles:
Developers configure enforcement intensity via `npx ecc-universal setup`:
- **`none`**: Hooks disabled entirely (identical to `--profile minimal`).
- **`minimal`**: Lightweight safety only. Intercepts catastrophic destructive commands and executes session end markers.
- **`standard`**: Recommended default. Enforces AgentShield command protection, config protection, desktop notifications, and cost tracking.
- **`strict`**: Maximum engineering rigor. Enforces `gateguard` (blocks code editing unless failing tests exist), blocks forgotten `console.log` statements, and enforces immediate type-checking on every tool completion.

#### AgentShield Security Architecture (`ecc-agentshield`):
Published as a standalone security scanner on npm (`ecc-agentshield`), AgentShield operates both as a real-time hook and a standalone CLI scanner:
- **Pre-Execution Command Scanning:** Intercepts shell execution before bash runs. Blocks dangerous regex patterns: recursive deletions (`rm -rf /`), destructive git commands (`git push --force`, `git reset --hard HEAD~10`), unencrypted credential dumps (`cat ~/.aws/credentials`, `export | grep KEY`), and curl-to-bash pipelines (`curl ... | bash`).
- **Prompt Injection Sanitizer:** Scans ingested web pages, issue bodies, and tool outputs for adversarial prompt injections, invisible Unicode zero-width tags, and role-override attempts.
- **Configuration Protection Gate:** Blocks models from silently editing sensitive project infrastructure files (`.env`, `.claude.json`, SSH keys, and CI workflow secrets).

---

### 3.8 Multi-Agent Verification Loops & Cross-Model Workflows
ECC moves beyond single-prompt generation by formalizing multi-agent verification loops:

#### 1. The Santa Loop (`/santa-loop`):
Spawns an adversarial dual-agent verification cycle. Two reviewer subagents are initialized with conflicting priorities:
- **Reviewer Alpha (Stability & Correctness):** Focuses on edge cases, regression risk, type safety, and defensive checks.
- **Reviewer Beta (Performance & Simplicity):** Focuses on code judo, eliminating shallow abstractions, throughput, and minimizing bundle size.
- **Convergence:** Code cannot merge until both agents sign off on the same diff.

#### 2. Cross-Provider Astra Review (`/astra-review`):
Eliminates single-model confirmation bias. Packages the local uncommitted git diff and pipes it through the **Codex CLI** to **GPT-6-Astra** using your existing ChatGPT login. GPT-6 audits the Claude-generated diff from an entirely independent model family, returning findings back to Claude Code to implement corrections.

#### 3. The Adversarial GAN Cycle (`/gan-design` & `/gan-build`):
Applies Generative Adversarial Network architecture to software engineering:
- `gan-planner`: Establishes the target feature specification, loss functions (test criteria, latency limits), and convergence thresholds.
- `gan-generator`: Proposes code implementations, refactoring candidate patches, and optimization variants.
- `gan-evaluator`: Acts as the discriminator, ruthlessly attacking candidate code with mutation tests, edge cases, and fuzzing inputs until the solution passes all hurdles.

#### 4. The CCG Multi-Model Pipeline (`/multi-plan`, `/multi-execute`, `/multi-workflow`):
Integrates with the `ccg-workflow` runtime to orchestrate multi-model development streams:
- `/multi-plan`: Orchestrates multi-model architecture reviews combining Claude Opus and OpenAI reasoning models.
- `/multi-backend` & `/multi-frontend`: Dispatches frontend UI work and backend API work to separate model workers simultaneously, synchronizing their outputs via typed OpenAPI schemas.

---

### 3.9 Memory Persistence, Instincts & Continuous Learning Engine
ECC solves agent amnesia across multi-session projects through a 3-tier memory engine:

#### 1. Session State Checkpointing (`/checkpoint`, `/save-session`, `/resume-session`):
- `/checkpoint`: Captures an instant snapshot of modified files, pending tickets, and reasoning context into `.claude/sessions/checkpoint.json`.
- `/save-session`: Summarizes completed tasks, settled architectural locks, and open questions before closing the terminal.
- `/resume-session`: Restores context into a new terminal session, allowing the agent to pick up exactly where it left off.

#### 2. The Continuous Learning Engine (`continuous-learning-v2`):
- Analyzes multi-turn debugging sessions. When an agent resolves a tricky bug after multiple failed attempts, the continuous learning hook extracts the root cause and synthesizes a concise, positive/negative rule.
- The synthesized rule is automatically saved into project rules or `lessons.md`, preventing the agent from repeating the same mistake in future sessions.

#### 3. The Instincts Engine (`/instinct-status`, `/instinct-export`, `/instinct-import`):
- Tracks developer preferences and coding idioms over time (e.g., preferred test assertion libraries, indentation styles, variable naming).
- Encodes preferences as lightweight behavioral "instincts" that subtly bias future agent decisions without consuming thousands of prompt tokens.
- Can be exported (`/instinct-export`) and shared across engineering teams (`/instinct-import`) to ensure uniform AI coding habits across an organization.

---

### 3.10 Infrastructure Bridges & Orchestration
ECC provides production infrastructure bridges connecting agent sessions directly to cloud compute, CI/CD, and multi-workspace environments:

#### 1. Hermes Headless CI/CD Operator (`docs/HERMES-SETUP.md`):
- Enables running ECC in headless, non-interactive mode within CI/CD pipelines (GitHub Actions, GitLab CI).
- Automatically reviews incoming pull requests, runs `/build-fix` against failing build logs, and submits PR corrections autonomously.

#### 2. Itô Compute CLI Bridge (`ito-compute`, `ito-baskets`):
- Interfaces directly with high-performance GPU compute capacity and prediction market feeds from inside Claude Code.
- Enables agents to query live GPU cluster inventory (`ito-gpu-inventory-rfq`), provision remote compute for training jobs (`ito-training`), and run distributed inference benchmarks (`ito-inference`).

#### 3. Git Worktree & Tmux Orchestration (`worktree-management`, `tmux-orchestration`):
- Provides automated provisioning of physical git worktrees (`git worktree add ../project-worker-1`).
- Allows running multiple parallel subagent sessions on the same local repository without branch collisions, file locking, or git merge corruption.


---

## 4. Matt Pocock's Agent Skills Suite (`aihero.dev`)

### 4.1 The Core Thesis: Overcoming the 4 Agent Failure Modes
TypeScript educator Matt Pocock structured his skills catalog (`mattpocock/skills`) around four specific breakdown points common in agentic coding:

```
┌──────────────────────────────────────┐     ┌──────────────────────────────────────┐
│       AGENT FAILURE MODE             │     │         MATT POCOCK SOLUTION         │
├──────────────────────────────────────┤     ├──────────────────────────────────────┤
│ 1. MISALIGNMENT                      │ ──► │ Socratic Grilling (/grill-me)        │
│    Agent builds the wrong feature.   │     │ Force questions before writing code. │
├──────────────────────────────────────┤     ├──────────────────────────────────────┤
│ 2. VERBOSITY & JARGON DRIFT          │ ──► │ Domain Modeling (GLOSSARY.md)        │
│    Agent talks in 20 words vs. 1.    │     │ Ubiquitous language & ADR registry.  │
├──────────────────────────────────────┤     ├──────────────────────────────────────┤
│ 3. CODE DOESN'T WORK                 │ ──► │ Strict TDD & Root-Cause Tree         │
│    Agent claims tests pass blindly.  │     │ Red-Green-Refactor loop & diagnostic │
├──────────────────────────────────────┤     ├──────────────────────────────────────┤
│ 4. BALL OF MUD ENTROPY               │ ──► │ Deep Module Design (/codebase-design)│
│    Fast AI coding creates spaghetti. │     │ Large logic hidden behind tiny APIs. │
└──────────────────────────────────────┘     └──────────────────────────────────────┘
```

### 4.2 The "Idea-to-Ship" Lifecycle Pipeline
Matt Pocock's engineering skills connect into a sequential assembly line:

```
[ Unrefined Idea ]
       │
       ▼
1. /grill-with-docs ──► Interviews developer; extracts GLOSSARY.md & ADR records
       │
       ▼
2. /to-spec         ──► Formalizes the agreed requirements into a technical specification
       │
       ▼
3. /to-tickets      ──► Slices specification into atomic, isolated ticket items
       │
       ▼
4. /implement       ──► Executes code changes ticket-by-ticket
       │
       ▼
5. /tdd             ──► Enforces red-green test cycle (write failing test -> make green)
       │
       ▼
6. /code-review     ──► Multi-axis audit against domain glossary and architectural rules
       │
       ▼
7. /retro           ──► Post-merge review analyzing mistakes to update team guardrails
```

### 4.3 Socratic Alignment: `/grill-me` and `/grill-with-docs`
- **`/grill-me`**: Claude stops being a passive executor and acts as an aggressive interrogator. When given an idea, it asks 3–6 probing questions regarding failure states, edge cases, UX assumptions, and data relationships. No code is generated until alignment is reached.
- **`/grill-with-docs`**: The same Socratic interview loop, but with active side effects: it automatically extracts domain terminology into `GLOSSARY.md` and generates Architecture Decision Records (ADRs) in `docs/adr/`.

### 4.4 Domain Modeling & Ubiquitous Language (`GLOSSARY.md` & ADRs)
- **Ubiquitous Language (from Domain-Driven Design):** Enforces strict naming conventions. If a concept is designated `OrderBatch`, Claude is strictly forbidden from using synonyms like `BatchGroup` or `OrderCollection`. This reduces context drift and prevents naming confusion across modules.
- **Architecture Decision Records (ADRs):** Short markdown files documenting non-obvious technical decisions (e.g., *"Why we use WebSockets instead of SSE"*). Prevents future agent sessions from re-litigating settled discussions.

### 4.5 Spec-Driven Development: `/to-spec` and `/to-tickets`
- **`/to-spec`**: Converts conversational brainstorming into a structured functional specification with explicit acceptance criteria.
- **`/to-tickets`**: Converts specifications into discrete task cards with defined dependencies, ready to be linked to issue trackers (GitHub Issues, GitLab, Jira).

### 4.6 Verification & Diagnostics: `/tdd` and `/diagnosing-bugs`
- **`/tdd`**: Enforces the Red/Green/Refactor loop:
  1. Write an automated test covering the requirement.
  2. Run the test to ensure it fails for the expected reason (Red).
  3. Write the minimum implementation code required to pass (Green).
  4. Refactor for clarity without breaking the test surface.
- **`/diagnosing-bugs`**: A phased diagnostic framework that prevents an agent from making speculative edits. It forces the agent to formulate hypotheses, isolate variables, gather telemetry, and reproduce bugs before modifying source files.

### 4.7 Codebase Design & Deep Modules
Built on John Ousterhout's *A Philosophy of Software Design* and Michael Feathers' *Working Effectively with Legacy Code*:

```
┌─────────────────────────────────┐       ┌─────────────────────────────────┐
│     DEEP MODULE (DESIRABLE)     │       │    SHALLOW MODULE (ANTI-PATTERN)│
├─────────────────────────────────┤       ├─────────────────────────────────┤
│       Small Interface           │       │        Wide Interface           │
│     (simple parameters)         │       │    (many methods, leak types)   │
├─────────────────────────────────┤       ├─────────────────────────────────┤
│                                 │       │                                 │
│                                 │       │      Thin Implementation        │
│       Deep Implementation       │       │    (just passes data through)   │
│   (hides massive internal logic │       │                                 │
│      behind a simple facade)    │       │                                 │
│                                 │       │                                 │
└─────────────────────────────────┘       └─────────────────────────────────┘
```

#### Core Design Vocabulary:
- **Module:** Any unit with an interface and an implementation (scale-agnostic).
- **Interface:** The complete contract a caller must understand to use the module correctly (types, invariants, errors, ordering).
- **Depth:** The ratio of internal capability to interface surface area. Deep = high capability behind a small interface. Shallow = interface is as complicated as the code inside.
- **Seam:** A boundary where behavior can be altered without editing the source code at that location.
- **Adapter:** A concrete component satisfying an interface at a seam.
- **Leverage:** The operational benefit callers gain from depth (learning a small interface gives massive functionality).
- **Locality:** The operational benefit maintainers gain (bugs, logic, and tests concentrate in one place rather than leaking into callers).
- **The Deletion Test:** If deleting a module causes complexity to vanish, it was a shallow pass-through. If deleting it causes complexity to scatter across callers, it was earning its keep.

### 4.8 Complete Catalog of Matt Pocock's 38 Skills

#### Engineering Category:
1. **`setup-matt-pocock-skills`**: Interactive setup wizard configuring issue trackers, labels, and doc paths.
2. **`grill-with-docs`**: Socratic interview building ADRs and domain glossaries.
3. **`codebase-design`**: Deep module vocabulary and architectural principles.
4. **`improve-codebase-architecture`**: Deepening scanner with visual HTML reporting and candidate grilling.
5. **`domain-modeling`**: Domain glossary extraction and terminology management.
6. **`to-spec`**: Specification generation from requirements.
7. **`to-tickets`**: Spec-to-issue breakdown for task tracking.
8. **`implement`**: Ticket execution engine.
9. **`implement-spec`**: Full-spec implementation orchestrator.
10. **`tdd`**: Red-Green-Refactor test cycle enforcement.
11. **`diagnosing-bugs`**: Gated root-cause diagnostic tree.
12. **`code-review`**: Multi-perspective branch and diff auditor.
13. **`retro`**: Post-implementation lessons-learned extractor.
14. **`wayfinder`**: Codebase exploration and navigation guide for agents.
15. **`prototype`**: Rapid exploratory scratch-pad builder.
16. **`research`**: Structured codebase and web research orchestrator.
17. **`pr`**: Automated pull request drafting and verification.
18. **`triage`**: Issue tracker triage and label categorization.
19. **`wizard`**: Step-by-step guided task assistant.
20. **`ask-matt`**: Expert TypeScript and architecture advice shim.

#### Productivity Category:
21. **`grill-me`**: Core Socratic requirement interrogation (code-agnostic).
22. **`grilling`**: The reusable questioning engine underlying all grill skills.
23. **`handoff`**: Session state extraction and continuity packager.
24. **`teach`**: Interactive educational explainer of complex topics.
25. **`to-questionnaire`**: Converts fuzzy problems into structured multiple-choice surveys.
26. **`wait-what`**: Sanity-check command for unraveling confusing code paths.
27. **`writing-for-agents`**: Formats documentation specifically optimized for LLM reading.
*(Plus 11 internal sub-skills and helpers facilitating triage, ticket formatting, and glossary validation).*

---

## 5. The "Thermo-Nuclear Code Quality Review" Skill

### 5.1 Origin and Philosophy
The **"Thermo-Nuclear Code Quality Review"** is an ultra-strict, high-rigor architectural review skill found across top skill directories. While standard code reviews check for syntax errors, missing null-checks, or formatting inconsistencies, a Thermo-Nuclear review evaluates code through the lens of a Chief Architect, seeking structural transformations.

### 5.2 The Concept of "Code Judo"
The foundational mandate of the Thermo-Nuclear review is discovering **"Code Judo"**:
- **Definition:** High-leverage refactoring maneuvers that preserve 100% of external functionality while dramatically simplifying, consolidating, or eliminating internal implementation complexity.
- **Objective:** Eradicate shallow abstractions, redundant indirection layers, and accidental state machines.

### 5.3 Deepening Opportunities & The Deletion Test
The review scans for **architectural friction** and surfaces **deepening opportunities**:
- Identifies shallow modules (e.g., classes or services that merely forward parameters to another library).
- Applies the **Deletion Test**: Evaluates whether removing a module would simplify the system or scatter complexity into callers.
- Enforces the rule: *"One adapter represents a hypothetical seam. Two adapters represent a real seam."* It flags speculative abstractions that only have one concrete implementation.

### 5.4 The Architecture Review HTML Dashboard
Instead of dumping walls of markdown text into the CLI, the skill generates a standalone, interactive HTML report saved to the system temp directory (`<tmpdir>/architecture-review-<timestamp>.html`) and opens it in the default browser:
- **Visual Tech Stack:** Styled using Tailwind CSS (via CDN) with interactive structural diagrams rendered using Mermaid.js.
- **Candidate Cards:** Every proposed architectural improvement is presented as a card containing:
  - **Involved Modules:** File paths and touched components.
  - **Friction Diagnosis:** Plain-English explanation of why the current structure creates cognitive overhead.
  - **Proposed Judo Maneuver:** Direct restructuring strategy.
  - **Side-by-Side Visuals:** Before vs. After architectural diagrams showing module consolidation.
  - **Confidence Rating:** Ranked as `Strong`, `Worth exploring`, or `Speculative`.
- **Top Recommendation:** Explicit guidance on which refactor delivers the highest ROI.

### 5.5 The Twin Pairing: `codebase-design` + `improve-codebase-architecture`
In your screenshot, notice the instruction:
> *"Run the `/codebase-design` skill for the architecture vocabulary (module, interface, depth, seam, adapter, leverage, locality) and its principles..."*

These two skills are designed as an inseparable **Twin Pair**:
```
┌────────────────────────────────────────────────────────┐
│ MENTAL MODEL & VOCABULARY: `codebase-design`           │
│ • Defines Deep vs. Shallow modules                     │
│ • Enforces exact terms: seam, adapter, depth, leverage │
│ • Establishes principles: deletion test, test surfaces │
└───────────────────────────┬────────────────────────────┘
                            │ (Informs & Governs)
                            ▼
┌────────────────────────────────────────────────────────┐
│ EXECUTION & SCANNER ENGINE: `improve-codebase-arch`    │
│ • Walks git churn hotspots via git log                 │
│ • Hunts for "Code Judo" simplification opportunities   │
│ • Renders interactive HTML report (Tailwind + Mermaid) │
│ • Runs the Socratic Grilling Loop on refactor picks    │
└────────────────────────────────────────────────────────┘
```
Without `codebase-design`, the Thermo-Nuclear review drifts into vague cosmetic advice (*"make this component cleaner"*). Without `improve-codebase-architecture`, `codebase-design` is just a glossary of design principles. Together, they form an automated, visual architectural refactoring powerhouse.

### 5.6 Execution Workflow & Grilling Loop
1. **Targeted Scan:** The agent walks the git history (`git log --oneline`) to identify hot spots (frequently changed files).
2. **Report Generation:** Generates and launches the HTML visual report (`architecture-review-<timestamp>.html`).
3. **Interactive Grilling:** The developer selects an architectural candidate from the dashboard. The agent immediately enters a `/grill-me` interrogation loop to debate constraints and verify that test surfaces survive the refactoring.

---

## 6. Headroom Context Compression Layer

### 6.1 Purpose & "Live Zone" Optimization
Created by Tejas Chopra, **Headroom** (`extraheadroom.com`) is an open-source, local-first context compression layer. It operates under the observation that the majority of context window consumption in agentic workflows does not come from user chat prompts, but from the **"Live Zone"**: verbose tool executions, package manager outputs, build errors, raw JSON responses, and entire file read dumps.

```
┌────────────────────────────────────────────────────────┐
│  Claude Code / Agent Terminal                          │
└───────────┬────────────────────────────────────────────┘
            │ Tool Output (e.g., 500-line npm error log)
            ▼
┌────────────────────────────────────────────────────────┐
│  HEADROOM LOCAL COMPRESSION ENGINE                     │
│  • AST Code Pruner                                     │
│  • JSON Structure Flattener                            │
│  • Terminal Log Deduplicator                           │
│  • Breadcrumb Hash Generator                           │
└───────────┬────────────────────────────────────────────┘
            │ Compressed Summary + Breadcrumb Hash (35 lines)
            ▼
┌────────────────────────────────────────────────────────┐
│  LLM Provider API (Anthropic / OpenAI)                 │
└────────────────────────────────────────────────────────┘
```

### 6.2 Deployment Modes
Headroom integrates into development environments via three primary interfaces:
1. **Terminal Wrapper (`headroom wrap`):**
   ```bash
   headroom wrap claude
   ```
   Wraps the `claude` executable, establishing a local interceptor that compresses incoming tool data before it hits the network layer. Can be deactivated via `headroom unwrap claude`.
2. **Local Proxy Mode:**
   Runs a local HTTP proxy on `localhost:8787`. Standard SDKs redirect traffic by setting `ANTHROPIC_BASE_URL=http://localhost:8787`.
3. **MCP Server Integration:**
   Exposes compression tools directly to MCP clients:
   - `headroom_compress`: Manual payload reduction.
   - `headroom_retrieve`: Retrieves the original raw content associated with a breadcrumb hash.
   - `headroom_stats`: Displays live session token metrics.

### 6.3 Compression Mechanics
- **AST-Aware Code Compression:** Parses code files into Abstract Syntax Trees, pruning internal function bodies while retaining interface declarations, signatures, and exported types when files are read for contextual understanding.
- **Log and JSON Crushing:** Strips redundant timestamps, deduplicates repetitive stack trace lines, and flattens verbose JSON trees down to essential schema structures.
- **Semantic Intactness:** Targets 60% to 90% size reduction on raw tool text while keeping error codes, line numbers, and core paths verbatim.

### 6.4 The Breadcrumb Hash Retrieval System
To eliminate the risk of lossy compression breaking an agent's reasoning, Headroom attaches a unique **Breadcrumb Hash** to every compressed block:
- If Claude realizes that an omitted detail is required to complete an edit, it calls `headroom_retrieve(hash)`.
- Headroom fetches the original uncompressed text from its local cache, providing safety against over-compression.

---

## 7. Anthropic's Official Agent Skills Standard (6 Core Insights)

Based on Anthropic’s engineering guide for authoring skills within Claude Code and agent architectures:

### 7.1 Insight 1: Gerund Naming Conventions
- **Guideline:** Skill names should ideally use gerunds (`-ing` verb forms) to describe active processes clearly:
  - *Recommended:* `processing-pdfs`, `analyzing-spreadsheets`, `diagnosing-bugs`.
  - *Acceptable alternatives:* Noun phrases (`pdf-processing`) or action verbs (`process-pdfs`), provided naming remains uniform across the library.
  - *Anti-patterns:* Generic labels like `utils`, `data`, or `helpers`.

### 7.2 Insight 2: Single-Level Deep References
- **Guideline:** All supporting documentation, schemas, and references must link directly from the root `SKILL.md`.
- **The Mechanism:** Agents frequently sample or preview referenced files (reading only initial characters). If `SKILL.md` links to `fileA.md`, which in turn links to `fileB.md`, the agent may drop the chain and fail to ingest `fileB.md`. Subdirectories are supported, but reference chains must not exceed a depth of one link.

### 7.3 Insight 3: Front-Loaded Table of Contents
- **Guideline:** When reference files exceed 100 lines, provide a clear Table of Contents at the very top.
- **The Mechanism:** Agents scanning files for relevance often read the first 100 lines. If critical procedures appear at line 300, a front-loaded Table of Contents signals to the agent that it must continue reading.

### 7.4 Insight 4: Evaluation-Driven Development (EDD)
- **Guideline:** Author skills through empirical evaluations rather than subjective testing:
  1. Run Claude on the target task **without** the skill installed.
  2. Document the specific failure modes, missing parameters, or logical errors.
  3. Establish a baseline benchmark.
  4. Author the minimum skill instructions required to resolve the failure modes.
  5. Re-run the evaluation to measure deterministic improvement.

### 7.5 Insight 5: Description as the Discovery Product
- **Guideline:** The `description` field in the YAML frontmatter is the most critical line of code in the entire skill.
- **The Mechanism:** When an agent boots, the body of `SKILL.md` is **not** loaded. Only the `name` and `description` are loaded into the primary system prompt. If the description is vague, the agent will never select the skill:
  - Must be written in the **third person**.
  - Must state precisely **what** the skill does and **when** it should be triggered.
  - Must include explicit keyword triggers that users are likely to say.

### 7.6 Insight 6: Progressive Disclosure (The 3-Tier Context Hierarchy)
The cornerstone of context efficiency in modern agent architectures:

```
┌────────────────────────────────────────────────────────────────────────┐
│ TIER 1: STARTUP (System Prompt Footprint)                              │
│ • Only skill NAME + DESCRIPTION loaded into memory.                    │
│ • Token cost: ~20-40 tokens per skill.                                 │
└───────────────────────────────────┬────────────────────────────────────┘
                                    │
                         User prompt matches description
                                    │
                                    ▼
┌────────────────────────────────────────────────────────────────────────┐
│ TIER 2: RELEVANCE ACTIVATION (Skill Body)                              │
│ • Root SKILL.md is dynamically read into the context window.           │
│ • Token cost: 100-500 lines of actionable procedures.                  │
└───────────────────────────────────┬────────────────────────────────────┘
                                    │
                          Complex edge case encountered
                                    │
                                    ▼
┌────────────────────────────────────────────────────────────────────────┐
│ TIER 3: DEEP DRILLDOWN (Supporting Reference Files)                   │
│ • Specific referenced files (schemas, API specs, templates) read.      │
│ • Token cost: Consumed only on demand for specific sub-tasks.          │
└────────────────────────────────────────────────────────────────────────┘
```

### 7.7 Cross-Model Parity Testing
Skills must be verified across the entire model family intended for deployment:
- Instructions sufficient for **Opus** (which exhibits high inference and implicit reasoning) may be too concise for **Haiku**, which requires explicit, step-by-step procedural constraints.

---

## 8. The September 2026 Skills Leaderboard Catalog

The fastest-growing open-source Claude skills ranked by GitHub stars gained:

```
┌──────┬──────────────────────────────────────────┬──────────────┐
│ RANK │ REPOSITORY                               │ STARS GAINED │
├──────┼──────────────────────────────────────────┼──────────────┤
│  01  │ DietrichGebert/ponytail                  │   +30,432    │
│  02  │ mattpocock/skills                        │   +29,787    │
│  03  │ ayghri/i-have-adhd                       │   +26,258    │
│  04  │ cloudflare/security-audit-skill          │   +20,148    │
│  05  │ obra/superpowers                         │   +13,841    │
│  06  │ cathrynlavery/diagram-design             │   +13,795    │
│  07  │ blader/humanizer                         │   +13,769    │
│  08  │ Graphify-Labs/graphify                   │   +9,651     │
│  09  │ addyosmani/agent-skills                  │   +8,880     │
│  10  │ pbakaus/impeccable                       │   +8,505     │
└──────┴──────────────────────────────────────────┴──────────────┘
```

### 01. `DietrichGebert/ponytail` (The Pragmatic Senior Developer)
- **Philosophy:** *"The best code is the code you never wrote."* Instills the mindset of a pragmatic, minimalist senior engineer.
- **The YAGNI Decision Ladder:** Forces the agent through an elimination ladder before writing any new code:
  1. Does this feature actually need to exist? (YAGNI)
  2. Does an equivalent helper or utility already exist in this repository?
  3. Can the programming language's standard library solve this?
  4. Can native platform features handle this (e.g., standard HTML5 vs. external npm packages)?
  5. Does an already-installed dependency solve it?
  6. Can it be expressed cleanly in a single line?
  7. *Only then:* Author new custom code.
- **Key Commands:**
  - `/ponytail`: Adjusts lazy-evaluation intensity (`lite`, `full`, `ultra`).
  - `/ponytail-review`: Audits git diffs specifically targeting over-engineering and bloated abstractions.
  - `/ponytail-audit`: Scans the entire repository and returns a ranked list of code that can be safely deleted.
  - `/ponytail-gain`: Outputs metrics showing deleted lines of code, token savings, and bundle reductions.

### 02. `mattpocock/skills` (Engineering Rigor Suite)
- Comprehensive 38-skill suite covering the entire lifecycle: `/grill-me`, `/to-spec`, `/to-tickets`, `/implement`, `/tdd`, `/codebase-design`, `/improve-codebase-architecture`, and `/retro` (detailed in Section 4).

### 03. `ayghri/i-have-adhd` (High-Signal, Action-First Output Format)
- **Philosophy:** Eliminates conversational preamble, throat-clearing, and verbose filler. Designed around behavioral research from *The Adult ADHD Tool Kit*.
- **The 10 Operational Rules:**
  1. Lead with the immediate next action (no *"Sure, I can help with that!"*).
  2. Number multi-step execution paths.
  3. Conclude every response with exactly one concrete next step.
  4. Suppress conversational tangents.
  5. Explicitly restate project state on every turn.
  6. Provide specific time estimates in minutes rather than vague qualifiers.
  7. Celebrate progress by making completed steps visible.
  8. State errors neutrally without apologies.
  9. Cap all bulleted lists to a maximum of 5 items.
  10. Strict ban on preamble, conversational recaps, and closing pleasantries.

### 04. `cloudflare/security-audit-skill` (Vulnerability Scanning)
- Automated security analysis scanning codebases and diffs for OWASP Top 10 vulnerabilities, credential leaks, improper CORS setups, unsafe deserialization, and missing authentication boundaries.

### 05. `obra/superpowers` (Disciplined SDLC & Subagent Orchestration)
- Created by Jesse Vincent (*obra*). Replaces unstructured "vibe coding" with disciplined engineering habits:
  - Enforces Socratic brainstorming before spec authoring.
  - Implements strict Red/Green Test-Driven Development.
  - Orchestrates subagents to handle complex multi-hour tasks without losing focus.

### 06. `cathrynlavery/diagram-design` (Visual & Mermaid Systems)
- Teaches the agent to communicate architecture, data flows, and state machines via structured diagrams (Mermaid, SVG, ASCII), optimizing layout choices for readability.

### 07. `blader/humanizer` (De-AI Prose Filter)
- Content and documentation filter that detects and strips stereotypical AI writing tropes (e.g., *"delve"*, *"testament"*, *"tapestry"*, unnecessary rhetorical questions, symmetrical sentence structures).

### 08. `Graphify-Labs/graphify` (Graph-Based Knowledge Persistence)
- Transforms codebases, documentation, and agent memory into interconnected knowledge graphs, allowing agents to navigate complex dependency graphs through semantic relationships rather than flat grep searches.

### 09. `addyosmani/agent-skills` (Production-Grade Lifecycle Management)
- Created by Addy Osmani (Google/Anthropic staff engineering). Packages production engineering workflows across the software lifecycle:
  - `/spec`: Requirements definition.
  - `/plan`: Atomic task slicing.
  - `/build`: Incremental slice-by-slice implementation.
  - `/test`: Proof-of-correctness testing.
  - `/review`: Multi-axis health checks.
  - `/ship`: Safe deployment pipelines.
  - `/code-simplify`: Code clarity optimizer.

### 10. `pbakaus/impeccable` (Frontend Precision & Craft Polish)
- Frontend design and craft evaluation skill ensuring UI implementations adhere to high visual standards: checks spacing scales, typography hierarchies, micro-interactions, accessibility contrast, and responsive layout stability.

### 11. `shadcn/improve` (The "Plan is the Product" / Brain-Worker Engine)
- **Author:** shadcn (creator of Shadcn UI).
- **Core Philosophy:** *"Use your most capable model for the part where intelligence compounds — understanding the codebase, judging what's worth doing, writing the spec — and hand execution to cheaper models. The skill never implements anything itself. The plan is the product."*
- **Installation:**
  ```bash
  npx skills add shadcn/improve
  ```
- **How It Works (The 3-Step Relay):**
  1. **You $\rightarrow$ `/improve`:** Using your most powerful model (e.g., Opus, Claude 3.7 Thinking), you run an audit of the codebase. It acts in a **read-only** capacity—mapping technical debt, N+1 query bottlenecks, performance issues, or architectural friction.
  2. **`plans/` $\rightarrow$ `001-fix-n-plus-one.md`:** Instead of modifying code, it outputs self-contained, atomic specification documents into a `plans/` directory with explicit instructions and boundaries.
  3. **Other Agent $\rightarrow$ Execution:** A cheaper, fast model (e.g., Haiku, Sonnet, Gemini Flash, or an autonomous worker) reads the isolated plan file, executes the implementation, runs tests, and submits the PR.
- **Key Commands:**
  - `/improve` or `/improve quick`: Maps repository structure and outputs a prioritized findings table.
  - `/improve security` / `/improve perf`: Scoped audits focusing strictly on vulnerability paths or database/compute bottlenecks.
  - `/improve plan <description>`: Directly generates an execution plan for a specific user-requested feature or fix.
  - `/improve execute <plan>`: Dispatches an execution agent targeted exclusively at the specified plan artifact.
- **Why It Matters:** Formulates the **Brain-Worker Orchestrator pattern** into a standardized, file-based contract. It isolates expensive reasoning tokens to where they matter most (planning), shielding the execution phase from heavy token burn.

---

## 9. Community Session Persistence, Heuristics & Multi-Model Workflows

### 9.1 `truongfelix/workflow` (Session Continuity, Questions Archive & Agent Collision Control)
An open-source workflow framework (`https://github.com/truongfelix/workflow.git`) designed to solve session amnesia across multi-week, multi-agent projects.

#### Adoption & CLI Commands:
```bash
git clone https://github.com/truongfelix/workflow.git
workflow/bin/adopt.sh ~/code/my-project
```
Then instruct Claude: *"Set up the workflow: follow 'Adopt in a new project' in workflow/README.md"*.

#### The 4 Core Session Questions:
Every session begins with a mandatory brief answering:
1. **What was done recently:** Git logs and verified progress.
2. **What is being built right now:** Active work in progress.
3. **What was already decided:** Architecture locks so they don't get "rediscovered" or accidentally reverted.
4. **What tasks are in the queue:** Immediate priority backlog.

#### The Dual-Command Engine:
- **`/session-start`**: Executed upon opening a session. Cross-checks session notes against live `git status`, open pull requests, and background CI jobs, highlighting any stale assumptions before work begins.
- **`/handoff`**: Executed before closing a session. Summarizes achievements, settled decisions, and remaining tickets, automatically archiving previous session notes into historical logs to prevent context window bloat.

#### Knowledge Repositories & Collision Prevention:
- **`lessons.md`**: A rolling register of project-specific quirks, compiler bugs, and tool failures. Loaded at the start of every session so Claude never repeats a solved error.
- **`questions.md`**: Tracks open architectural inquiries and client requirements, complete with an archival mechanism once resolved.
- **Backlog & Collision Dashboard**: A standardized format for backlog items with an automated parser script. Multiple agents claim work items via `git push`, updating a lightweight local HTML status page to prevent two concurrent agents from modifying the same files.

---

### 9.2 The 13 Battle-Tested Heuristics (1-Year Daily Power User Protocol)
Synthesized from experienced engineers running Claude Code daily on multi-month production codebases:

1. **Git & Instant Remote Pushes:** Always initialize Git immediately. Push commits to GitHub continuously (using private repositories for proprietary code) so rollback points exist for every working state.
2. **Git Worktrees vs. Standard GitHub Branching (Do you really need them?):**
   - **If you work sequentially (1 terminal, 1 task at a time):** **NO, you do NOT need worktrees.** Standard Git branching (`git checkout -b feature`, commit, push) is simpler, cleaner, and 100% sufficient.
   - **When worktrees ARE needed:** ONLY when running **simultaneous parallel agents on the same computer**:
     - *The Problem:* With standard Git, switching branches changes the physical files on disk in your single working directory. If Claude in Terminal 1 is writing code on `feature-auth` and you switch to `feature-ui` in Terminal 2, Terminal 1 will crash or corrupt files.
     - *The Worktree Fix:* `git worktree add ../project-ui feature-ui` creates a separate physical folder on disk linked to the same `.git` database. Terminal 1 and Terminal 2 can code simultaneously on different branches without file collision.
   - **Bottom Line:** If you aren't running two active Claude sessions at the exact same minute on the same repository, stick to regular GitHub branches.
3. **The "1 Task = 1 Chat" Rule (< 500k Context Ceiling):**
   - *Why auto-compact fails:* Context degradation accelerates past 500k tokens. Built-in `/compact` and auto-compaction frequently lose active state, subtle type definitions, and debugging threads.
   - *The Rule:* Treat conversations as disposable execution pods. Run exactly **1 task per chat session**. Commit the feature, close the terminal, and start fresh.
4. **Decoupled Brain / Worker Multi-Window Topology:**
   - Create a dedicated **"Brain" chat** running on **Opus** that analyzes requirements, designs systems, and drafts clear technical briefs.
   - Create 2–4 **"Worker" chats** running on **Sonnet**. Name them and initialize them with the prompt `"wait for task"`.
   - The Brain distributes concrete briefs into the Worker chats, verifying diffs when workers complete their runs.
5. **High-Quality `CLAUDE.md` + `Rules.md`:** Maintain clean, explicit governance files in repository roots defining build scripts, framework idioms, and forbidden anti-patterns.
6. **Skeptical Verification ("Never Believe Claude"):** Never accept Claude's claim that *"all tests pass and everything works"* without seeing raw terminal execution proof. Always command it to perform an independent audit from "different points of view."
7. **Research Escalation via Ultracode / Multi-Agent Sweeps:** When Claude claims a task is impossible, hallucinates missing capabilities, or suggests an antipattern, force it into research mode: run multi-agent sweeps restricted strictly to trusted official documentation.
8. **Standing Rule: Full Smoke Check:** Enforce a mandatory standing rule in `CLAUDE.md`: before declaring any feature or bugfix complete, Claude must execute a full end-to-end smoke check (build, lint, integration verification).
9. **The Clean-Room / Off-Project Debugger Chat:**
   - *The Problem:* When Claude gets stuck in an erroneous reasoning loop, trying to correct it inside the same chat further pollutes the context with defensive or confused tokens.
   - *The Solution:* Open a completely clean, unlinked chat window. Explain what the project worker is doing and what went wrong. The clean model will diagnose the flaw objectively and generate an unpolluted correction prompt to paste back into the project terminal.
10. **The Anti-Guessing Mandate:** Claude default behavior often gambles on file contents from memory. Add an explicit standing rule:
    > *"Never guess at file contents, imports, or project state. Always perform concrete file reads and audits before proposing modifications."*
11. **Document-Verified Memory:** If you instruct Claude to remember something, it will inevitably scroll out of the context window. Force physical artifact creation:
    > *"Write this decision into `docs/decisions/` and output the exact file path as verification."*
12. **Live Lessons Capture:** Whenever an unexpected error is resolved, update `lessons.md` or `CLAUDE.md` immediately during that session before closing the terminal.
13. **The Research Tangent Gate (The Anti-Rabbit-Hole Mandate):**
    - *The Problem:* When Claude encounters ambiguity or subtle bugs, it often embarks on unprompted research tangents—reading dozens of unfamiliar documentation pages, spidering external links, or scanning distant modules. This burns thousands of tokens without advancing the task.
    - *The Rule:* Claude is strictly forbidden from initiating autonomous research detours. Whenever it believes external research or reading files outside the active feature boundary is necessary, it must explicitly halt and request permission:
      > *"I recommend researching [Topic/Library] to solve this issue. Would you like me to proceed with this research detour, or would you prefer a specific approach?"*

---

### 9.3 Caveman Mode: Ultra-Terse Token Pruning (`JuliusBrussee/caveman`)
Created by Julius Brussee, **Caveman Mode** (`@caveman-ai/cli` or `JuliusBrussee/caveman`) is an output optimization prompt technique built on the meme: *"Why use many token when few token do trick?"*

#### Core Mechanics:
Standard models generate polite conversational filler, preamble (*"Certainly! I'd be happy to help..."*), problem restatements, and sign-offs. In long terminal loops, these filler tokens burn budget and slow down CLI responses.

Caveman Mode strips:
- Articles (*"the"*, *"a"*, *"an"*)
- Pleasantries, conversational hedging, and throat-clearing
- Restating questions or code back to the developer
- Conversational sign-offs (*"Let me know if you need anything else!"*)

It keeps code blocks, error logs, and technical diffs completely verbatim.

#### Installation & Direct Prompting:
```bash
claude plugin marketplace add JuliusBrussee/caveman
claude plugin install caveman@caveman
```
*Manual system instruction variant:*
> *"Follow these rules on every response: Speak in short sentence fragments only. Drop filler words, articles (the, a, an), pleasantries, and hedging. Tool work first, results first. Do not restate the problem or explain code unless asked. Keep code, errors, and technical terms exact and verbatim."*

---

### 9.4 Multi-Model Division of Labor: Gemini + Claude Symbiosis
For projects requiring heavy documentation intake and rapid UI/logic iteration, pairing distinct frontier models creates a high-leverage division of labor:

```
┌────────────────────────────────────────────────────────┐
│ GEMINI (Pro / Flash) ── The Research Scout             │
│ • Massive 1M–2M context window                         │
│ • Ingests 200-page sponsor SDKs, raw PDFs, sample repos│
│ • Parses whiteboard sketches & UI wireframe images     │
│ • Output: Condenses massive docs into 20-line snippets │
└───────────┬────────────────────────────────────────────┘
            │ Clean, concise code contract & parameters
            ▼
┌────────────────────────────────────────────────────────┐
│ CLAUDE CODE (Sonnet / Opus) ── The Tactical Builder    │
│ • Best-in-class frontend aesthetics & React/Tailwind   │
│ • Direct terminal tool execution, git, and file writes │
│ • Executes tight logic loops without hallucinating APIs│
└────────────────────────────────────────────────────────┘
```

#### Operational Rule:
Avoid circular, back-and-forth orchestration between models (which wastes time). Maintain a **one-way relay**: Gemini ingests and compresses external documentation; Claude consumes the compressed brief to write and edit project code.

---

### 9.5 The Hackathon Velocity Playbook & Zero-Crash Mock Fallback Strategy

#### Sprint (48-Hour) vs. Marathon (3-Week) Dynamics:
- **48-Hour Sprint:** Ban TDD, skip formal PRDs, skip refactoring. Focus 100% on a working demo path, high visual polish, and pitch clarity.
- **3-Week Marathon:** Architecture matters. Use `/grill-me` in Week 1 to lock database schemas, run Headroom to contain 21 days of tool logs, maintain `lessons.md` for session persistence, and freeze backend features by Day 17 for pure UI polish.

#### Pre-Baked Stack Selection:
Never build authentication, databases, or UI primitives from scratch. Standardize on rapid frameworks: **Next.js 15 (App Router) + Tailwind v4 + Shadcn UI + Supabase / Convex**.

#### The Zero-Crash Fallback Mock Pattern (Crucial):
The most common point of failure in live hackathon presentations is network latency, third-party API rate limits, or expired bearer tokens.

**Mandatory Hackathon Rule:** Wrap every external API integration in a silent fallback toggle:
```typescript
try {
  const res = await fetchLiveSponsorApi(params);
  if (!res.ok) throw new Error("API unreachable");
  return await res.json();
} catch (error) {
  console.warn("Live API failed, engaging silent demo fallback:", error);
  return MOCK_DEMO_DATA; // Seamlessly returns realistic data so live demo never crashes
}
```

---

### 9.6 Lean `CLAUDE.md` Architecture vs. The "Boil the Ocean" Mega-Prompt

#### The Failure of 500-Line Mega-Prompts:
Large language models experience **attention dilution** across long system prompts:
- **Lines 1–50:** High adherence (~95%).
- **Lines 50–150:** Moderate adherence (~70%).
- **Lines 150–500+:** Severe dilution (~30%); rules get ignored or half-heartedly executed.

Attempting to "boil the ocean" by cramming language textbooks, styling encyclopedias, and 50 contradictory roles into `CLAUDE.md` degrades performance while burning thousands of tokens per prompt.

#### The Lean Index-Router Standard (< 100 Lines):
High-efficiency `CLAUDE.md` files follow three structural principles:
1. **High-Compliance Negative Constraints:** Explicit negative boundaries stick significantly better than vague positive advice (`NEVER use useEffect for data fetching` vs. `prefer modern data patterns`).
2. **Pointer Indexing (Progressive Disclosure):** Never explain code conventions in prose; point directly to canonical files (`For error handling, copy the exact shape in src/lib/errors.ts`).
3. **Dynamic Bug Registry:** Keep the file concise (~50 lines) and use the remaining space to record live lessons as errors occur during active development.

---

## 10. Specialized MCP Servers & Precomputed Lookup Architectures (Databases, State Engines & Browser Automation)

### 10.1 Database MCPs for Agentic Workflows
Model Context Protocol (MCP) servers allow Claude Code to interact directly with local or remote databases, schema migrations, and table inspections:

- **SQLite MCP (`@modelcontextprotocol/server-sqlite`):**
  - *Best for:* Local embedded stores, lightweight lookup tables, offline state tracking, and rapid test environments.
  - *Zero Setup:* Runs directly in Node against a local `.db` or `.sqlite` file without provisioning a background daemon.
  - *Install:* `claude mcp add sqlite -- npx -y @modelcontextprotocol/server-sqlite /path/to/database.db`
- **Postgres MCP (`@modelcontextprotocol/server-postgres`) & Supabase MCP (`@supabase/mcp-server`):**
  - *Best for:* Relational production applications, RLS policies, migrations, and complex schema relationships.
  - *Safety:* Initialize with `--read-only` flags when using agents to explore table schemas without accidental mutation risks.
- **Redis / Key-Value MCPs:**
  - *Best for:* Low-latency state caches, ephemeral session tokens, and pub/sub message brokers.

---

### 10.2 Architectural Pattern: The Precomputed State-Action Engine (Game Bot Case Study)
When building interactive applications (e.g., a real-time game bot that learns or adapts to player behavior), running real-time ML fine-tuning or full-model inference on every tick is a technical bottleneck:

```
┌────────────────────────────────────────────────────────────────────────┐
│ THE REAL-TIME BOT BOTTLENECK (ANTI-PATTERN)                           │
│ • Player makes move ──► Send 10k tokens to LLM / Run local GPU train   │
│ • Result: Massive latency (500ms-3000ms), huge cost, frame stutters.  │
└────────────────────────────────────────────────────────────────────────┘
                                    │
                         RESOLVED AT ARCHITECTURE STAGE VIA
                                    │
                                    ▼
┌────────────────────────────────────────────────────────────────────────┐
│ THE PRECOMPUTED STATE-ACTION LOOKUP ENGINE                             │
│                                                                        │
│ 1. OFFLINE BATCH PRECOMPUTATION:                                       │
│    • Simulate/calculate state transitions & optimal responses.         │
│    • Store combinations into an embedded database (SQLite / DuckDB).   │
│    • Formats: Transposition Tables, Q-Tables, or Vector Embeddings.    │
│                                                                        │
│ 2. LIVE GAME RUNTIME:                                                  │
│    • Player makes Move X at Game State Y.                              │
│    • Bot queries SQLite index via primary key hash `(state_id, move)`. │
│    • Response retrieved in < 2ms without live LLM inference overhead.  │
│                                                                        │
│ 3. AGENTIC ADAPTATION LOOP:                                            │
│    • Claude Code / Subagent runs background analysis on game logs.     │
│    • Periodically updates the SQLite database with new edge cases.     │
└────────────────────────────────────────────────────────────────────────┘
```

#### Why This Belongs in Your Agent Architecture:
- By using an embedded SQLite database accessed via MCP or local ORM, the agent harness can generate, populate, and query deterministic lookup tables during development, giving live bots instant reaction speed with zero API costs during gameplay.

---

### 10.3 Browser & UI Verification MCPs (Playwright, Puppeteer, Chrome DevTools)
Testing full-stack web applications and visual user interfaces requires tools that interact with the live DOM:

- **Playwright MCP / Chrome DevTools MCP:**
  - *Capabilities:* Headless or visual browser execution, taking full-page screenshots, clicking elements, inspecting console error logs, checking network response status codes, and verifying responsive viewport layouts.
  - *Hackathon / Demo Verification:* Allows Claude Code to open the local development server (`http://localhost:3000`), navigate through the user journey, verify that charts and animations render cleanly, and capture proof before the developer presents to judges.
- **Puppeteer MCP (`@modelcontextprotocol/server-puppeteer`):**
  - *Capabilities:* Fast scraping, automated form submission, PDF rendering, and visual regression checks.

---

## 11. Developer Tooling & Rapid Prototyping Infrastructure

### 11.1 Rapid UI Scaffolding & Component Tools
Tooling that enables instantaneous, production-grade frontend creation without manual CSS wrestling:
- **Shadcn CLI (`npx shadcn@latest add <component>`):** Copies fully accessible, unstyled Radix-based components directly into your codebase as editable source code rather than locked npm dependencies.
- **Tailwind CSS v4 & Lucide React (`lucide-react`):** Utility-first styling engine with a standardized icon library (1,400+ vector icons), eliminating design friction.
- **UI Micro-Interaction Libraries:**
  - `sonner`: Instant toast notification dispatcher requiring zero configuration (`toast.success("Action completed")`).
  - `canvas-confetti`: Lightweight canvas-based celebratory particle trigger.
  - `framer-motion`: Declarative layout and exit animations for tabs, dialogs, and route transitions.

---

### 11.2 Multimodal Wireframe-to-Code Pipeline (Vision-to-JSX Generation)
Tooling that accepts visual artifacts and outputs functional JSX/TSX components:
- **Mechanics:** Passing screenshots, Excalidraw exports, or paper sketches to multimodal models (Claude Sonnet or Gemini Flash).
- **Execution CLI:**
  - Using Claude Code's native image handling or web interface to inject `.png`/`.jpg` wireframes directly into prompts.
  - The model maps layout hierarchies, forms, buttons, and navigation bars directly into Tailwind classes and Shadcn primitives.

---

### 11.3 Local Tunneling & Webhook Tools (`cloudflared`, `ngrok`)
Essential networking utilities for testing external integrations and local servers:
- **Cloudflare Tunnels (`cloudflared tunnel`):**
  - Provides free, secure, persistent public URLs pointing to local ports (`localhost:3000`) without public IP exposure or port forwarding.
  - *Install:* `cloudflared tunnel --url http://localhost:3000`
- **ngrok (`ngrok http 3000`):**
  - Standard tunnel utility with built-in request/response inspector UI (`localhost:4040`) for debugging incoming webhooks (e.g., Stripe, Clerk, GitHub webhooks) and testing live mobile views.

---

### 11.4 Synthetic Data & API Mocking Tools (`@faker-js/faker`, MSW)
Tooling that populates empty databases and mocks external third-party dependencies:
- **Mock Service Worker (`msw`):**
  - Intercepts requests at the network level (via Service Workers in browser or node interceptors), allowing seamless switching between live APIs and mocked fallbacks without altering production code.
- **Faker.js (`@faker-js/faker`):**
  - Programmatic generation of realistic mock data: user avatars, names, enterprise transaction logs, dates, and addresses to pre-seed databases and prevent empty-state bugs during agent runs.

---

### 11.5 Deployment CLIs & Secret Detection (`vercel`, `railway`, `gitleaks`)
Command-line utilities for continuous cloud synchronization and credential security:
- **Hosting CLIs:**
  - `vercel` / `railway` / `flyctl`: Headless CLI tools that enable direct terminal deployments, build log inspection, and environment variable synchronization (`vercel env pull`).
- **Gitleaks (`gitleaks`):**
  - Fast, local-first SAST tool that scans git repositories for hardcoded secrets, API keys, and tokens:
    ```bash
    gitleaks detect --source . --verbose
    ```
  - Prevents fatal credential exposure to public repositories during rapid git commits.

---

### 11.6 Voice-to-Text Terminal Tools (Whisper CLI, Local Speech-to-Text)
Input acceleration tooling enabling hands-free, high-velocity prompt input:
- **Whisper CLI (`whisper.cpp` / OpenAI Whisper):**
  - Local, offline speech-to-text binaries that transcribe audio from microphone inputs into text files or direct stdin pipe payloads.
- **OS-Level Voice Layers (Wispr Flow, Superwhisper, macOS Dictation):**
  - Allows dictating complex technical briefs, edge cases, and file constraints into the Claude Code terminal at 150+ words per minute, eliminating typing bottleneck during marathon sessions.

---

## 12. Cross-System Capability & Taxonomy Matrix

| System / Tool | Primary Focus | Token Footprint | Key Commands / Trigger | Ideal Deployment Stage |
| :--- | :--- | :--- | :--- | :--- |
| **Claude Code Engine** | Agent Harness Runtime | Baseline | `/model`, `Shift+Tab`, `/clear` | Universal execution layer |
| **Test-First Plan Gate** | Tautological Test Prevention | Zero Overhead | Enforced during Plan Mode | Feature planning & spec authoring |
| **Context Hygiene (`/context`, `/compact`)** | Active Window Optimization | Zero to Negative Tokens | `/context`, `/compact` (100k-150k)| Milestone transitions & memory pruning |
| **ECC (Everything Claude Code)** | Enterprise Suite (71 Agents, 311 Skills, 97 Cmds, 122 Rules)| Modular (Zero idle overhead for skills) | `/plan`, `/santa-loop`, `/build-fix` | Universal full-lifecycle engineering |
| **Matt Pocock Suite** | Alignment, Spec & Architecture | Low (Progressive Disclosure) | `/grill-me`, `/to-spec`, `/codebase-design`| Pre-build & structural refactoring |
| **Thermo-Nuclear Review** | Radical Code Simplification | Low (On-Demand Activation) | `thermonuclear`, `/codebase-design` | Pre-merge architecture audits |
| **Headroom** | Live-Zone Context Compression | Negative (Reduces tokens) | `headroom wrap claude` | Log/test-heavy development loops |
| **Anthropic Skills Std** | Architectural Skill Standard | Near-Zero Idle Overhead | Automatic discovery via descriptions | Universal authoring guideline |
| **Ponytail** | Anti-Over-Engineering / YAGNI | Near-Zero Idle Overhead | `/ponytail`, `/ponytail-audit` | Prototyping & complexity control |
| **I Have ADHD** | Output Conciseness & Formatting | Near-Zero Idle Overhead | Automatic prompt filtering | Token-saving daily development |
| **Addy Osmani Skills** | Structured SDLC Quality Gates | Low (Progressive Disclosure) | `/spec`, `/plan`, `/build`, `/test` | Team codebases & PR pipelines |
| **`shadcn/improve`** | Brain-Worker Spec Delegation | Low (Read-only planning) | `/improve`, `/improve plan` | Token-optimized architecture & execution |
| **`truongfelix/workflow`**| Multi-Session State Persistence| Low (Markdown artifacts) | `/session-start`, `/handoff` | Multi-week development projects |
| **Native Shell Mode (`!`)**| Zero-Token Direct Shell Bypass | Zero Token Cost | `!<command>`, `Ctrl+Z`, `Ctrl+S`| Routine git, build, test & CLI commands |
| **Database MCPs (SQLite/Supabase)**| Fast State-Action & Schema Lookup| Low (Tool Schema only) | `claude mcp add sqlite ...` | State storage, game lookups & migrations |
| **Browser MCPs (Playwright/Chrome)**| Live DOM, Canvas & UI Inspection| Low (Tool Schema only) | `claude mcp add playwright ...`| UI smoke tests & visual demo verification |
| **Tunneling Tools (`cloudflared`/`ngrok`)**| Local Webhook & External Access | Zero Token Cost | CLI: `cloudflared tunnel`, `ngrok` | Webhook verification & mobile inspection |
| **Mocking Tools (MSW / Faker.js)**| Synthetic Data & Network Mocking| Zero Token Cost | `npm install msw @faker-js/faker`| Offline state & pre-seeding databases |
| **Deploy & Secret CLIs (`gitleaks`/Vercel)**| Fast Sync & Credential Scanning | Zero Token Cost | `gitleaks detect`, `vercel` CLI | Automated deployments & token security |




