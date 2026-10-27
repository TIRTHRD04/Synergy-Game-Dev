# Lessons

One line per non-obvious problem, so it is not repeated.

- 2026-10-27: A `cat >` heredoc silently replaced the existing .gitignore (which ignored data/, models/ and secrets). Read a file before writing it, and append with `>>` when adding entries.
- 2026-10-27: The Microsoft .NET download host is blocked by the container's network policy; Ubuntu's archive has `dotnet-sdk-8.0`, and NuGet (api.nuget.org) is reachable.
- 2026-10-27: With broad matches off, the broad strength never decays, so "delete when both strengths < 0.1" kept entries forever. Deletion counts only the match types in use.
- 2026-10-27: When every expert abstains, Thompson Sampling drew a sleeping expert and sampled a stale posterior. The selector now plays sigma* when no expert is awake.
