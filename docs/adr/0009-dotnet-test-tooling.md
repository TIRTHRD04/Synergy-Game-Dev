# 0009: Core and the harness are built and tested with dotnet; NUnit is test-only

**Status:** Accepted (2026-10-27)

Unity cannot run in the cloud container, but the Opponent core and the harness are pure C# and must be tested on every change (CLAUDE.md, "Before saying done"). The container has the .NET 8 SDK (Ubuntu package `dotnet-sdk-8.0`, 8.0.132).

## Decision

- `tools/dotnet/` holds four projects that compile the sources straight from `Assets/`, so there is one copy of the code:
  - `Core` (netstandard2.1): `Assets/Scripts/Core`.
  - `Sim` (netstandard2.1): `Assets/Scripts/Core.Sim`.
  - `SimCli` (net8.0, executable `sim`): the command-line entry point. It lives outside `Assets/` and is not part of the Unity build.
  - `Tests` (net8.0): `Assets/Tests/EditMode`. The same test files run in Unity's Test Runner.
- C# language version 9.0 and netstandard2.1, the level Unity 6 compiles, so code that builds here builds in Unity. Warnings are errors, and XML documentation is generated (a missing doc comment on a public member is an error).
- Test-only packages, pinned, never shipped in the game:
  - NUnit 3.14.0. The 3.x classic API (`Assert.AreEqual`, `Assert.That`) matches the NUnit 3 that Unity's Test Framework bundles; NUnit 4 removed the classic asserts, so test files would not compile in both.
  - NUnit3TestAdapter 4.5.0 and Microsoft.NET.Test.Sdk 17.9.0, to run NUnit under `dotnet test`.
- The shipped game adds no packages beyond the Unity 6 default template. The teammate who creates the Unity project lists that template's packages (from `Packages/manifest.json`) in this ADR.

## Consequences

- `dotnet test tools/dotnet/Tests` is the test command in the container (docs/COMMANDS.md). Unity EditMode and PlayMode runs, and the smoke check, are done by a teammate with Unity 6.
- Core and Core.Sim must not use APIs missing from netstandard2.1 or language features newer than C# 9 (file-scoped namespaces, `record struct`, `init` accessors on netstandard2.1).
