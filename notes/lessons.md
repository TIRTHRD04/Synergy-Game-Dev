# Lessons

One line per non-obvious problem, so it is not repeated.

- 2026-10-27: A `cat >` heredoc silently replaced the existing .gitignore (which ignored data/, models/ and secrets). Read a file before writing it, and append with `>>` when adding entries.
- 2026-10-27: The Microsoft .NET download host is blocked by the container's network policy; Ubuntu's archive has `dotnet-sdk-8.0`, and NuGet (api.nuget.org) is reachable.
