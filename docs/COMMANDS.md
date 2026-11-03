# Commands

All commands run from the repository root.

## In the cloud container (dotnet, no Unity)

| What | Command |
|---|---|
| Build everything | `dotnet build tools/dotnet/SimCli` and `dotnet build tools/dotnet/Tests` |
| EditMode tests (Core and Core.Sim) | `dotnet test tools/dotnet/Tests` |
| Archetype suite on dev seeds | `dotnet run --project tools/dotnet/SimCli -- suite --config configs/v1.json --seeds seeds/dev/seeds.txt --name <row-name>` |
| Archetype suite on heldout seeds (once, at the end) | `dotnet run --project tools/dotnet/SimCli -- suite --config <config> --seeds seeds/heldout/seeds.txt --name <row-name>` |
| Paired comparison of two configs | `dotnet run --project tools/dotnet/SimCli -- compare --base <config> --new <config> --seeds seeds/dev/seeds.txt --name <row-name>` |
| Gate threshold check on Random | `dotnet run --project tools/dotnet/SimCli -- gate-check --config <config> --seeds seeds/dev/seeds.txt` |

Every `suite` and `compare` run appends one row to `experiments/log.csv`.

## In Unity 6 (a teammate)

| What | Command |
|---|---|
| EditMode tests | `Unity -batchmode -runTests -testPlatform EditMode -projectPath . -testResults results-editmode.xml` |
| PlayMode tests | `Unity -batchmode -runTests -testPlatform PlayMode -projectPath . -testResults results-playmode.xml` |
| Smoke check | Open the project, press Play, follow docs/playtest.md |
