# Lessons

One line per non-obvious problem, so it is not repeated.

- 2026-10-27: A `cat >` heredoc silently replaced the existing .gitignore (which ignored data/, models/ and secrets). Read a file before writing it, and append with `>>` when adding entries.
- 2026-10-27: The Microsoft .NET download host is blocked by the container's network policy; Ubuntu's archive has `dotnet-sdk-8.0`, and NuGet (api.nuget.org) is reachable.
- 2026-10-27: With broad matches off, the broad strength never decays, so "delete when both strengths < 0.1" kept entries forever. Deletion counts only the match types in use.
- 2026-10-27: When every expert abstains, Thompson Sampling drew a sleeping expert and sampled a stale posterior. The selector now plays sigma* when no expert is awake.
- 2026-10-27: Scripted level-1/mirror-1 players that modelled the Opponent slightly differently (fade 0.9 vs 0.98, order-1 counts vs CTW) flipped the best response often enough to make a correct meta expert look broken (0.27 vs a 0.467 ceiling). A test player "of its type" must use exactly the expert's model.
- 2026-10-27: `dotnet run --no-build` runs the last build of the CLI, not the current sources. Rebuild before measuring.
