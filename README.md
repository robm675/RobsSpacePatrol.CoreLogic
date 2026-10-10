# Rob's Space Patrol — CoreLogic

The shared C# game engine and data-transfer objects for Rob's Space Patrol, a
command-line space strategy game and personal learning project. The engine
models an 8-by-8 galaxy of 8-by-8 sector grids, including navigation, combat,
ship resources, damage, repairs, and mission state.

This repository is a library rather than a directly runnable game. The
[CSharp command-line application](https://github.com/robm675/RobsSpacePatrol.CSharp)
is one consumer.

I independently wrote the original C# implementation, inspired by classic
text-based space games, and later ported my own implementation to other
languages.

## Requirements

- Windows with the .NET 10 SDK. The checked-in `global.json` selects SDK
  10.0.401 and allows later 10.0.4xx patches.
- PowerShell for the optional package-update helper.
- Internet access on the first restore for public NuGet dependencies.

Visual Studio is optional.

## Build and test

From the repository directory:

```powershell
dotnet restore .\RobsSpacePatrol.CoreLogic.sln
dotnet build .\RobsSpacePatrol.CoreLogic.sln --configuration Release --no-restore
dotnet test .\RobsSpacePatrol.CoreLogic.sln --configuration Release --no-restore
```

The tests use xUnit, Moq, and deterministic random-factory fixtures. They focus
on game rules and state transitions rather than depending on live services.

## Creating the private packages

The projects are versioned as private NuGet packages. They do not need to be
published to NuGet.org:

```powershell
dotnet pack .\RobsSpacePatrol.DTO\RobsSpacePatrol.DTO.csproj `
    --configuration Release --output .\artifacts\packages

dotnet pack .\RobsSpacePatrol.Library\RobsSpacePatrol.Library.csproj `
    --configuration Release --output .\artifacts\packages
```

`RobsSpacePatrol.Library` declares the matching `RobsSpacePatrol.DTO` package as
a dependency. Consumer repositories can commit these packages to their own
`local-packages` directory and restore them through a repository-level
`NuGet.config`. Generated packages under this repository's `artifacts`
directory are intentionally ignored by Git.

For the maintainer's sibling-folder workflow, `update_coreLogic_Nuget.ps1`
increments package versions, tests CoreLogic, packs into the adjacent
`RobsSpacePatrol.CSharp` repository, updates that solution's package references,
and tests the consumer. Use `-IncludeDto` when DTO changes also require a new DTO
package version. The helper is not required to build or test this repository.

## Project layout

- `RobsSpacePatrol.DTO/`: serializable DTOs, shared enums, and API request and
  response contracts.
- `RobsSpacePatrol.Library/`: game rules, models, factories, navigation,
  persistence helpers, and model/DTO mapping.
- `RobsSpacePatrol.Library.Tests/`: xUnit tests and deterministic fixtures.
- `update_coreLogic_Nuget.ps1`: optional maintainer package-update workflow for
  the adjacent CSharp repository.
- `Directory.Build.props`: deterministic-build and source-path settings.
- `global.json`: pinned .NET SDK feature band and test runner.

Mapperly generates model/DTO mapping code at compile time and is not a runtime
package dependency. DeepCloner, NLog, and RandomGen are restored as runtime
dependencies of the core library. Test-only packages remain private to the test
project.

## Design notes and limitations

- Random behavior is isolated behind a factory so tests can supply known values.
- Coordinates range from 0 to 7 for both quadrants and sectors.
- Reaching the exact mission deadline is allowed; exceeding it fails the
  mission.
- A starship is destroyed only when shields fall below zero. Zero shields leave
  it vulnerable to the next successful hit.
- Device repairs round damage to one decimal place after each maintenance step;
  very small fractional repair increments can therefore be lost. This is an
  accepted gameplay rule.
- Detailed obstacle movement occurs within the current quadrant. Inter-quadrant
  travel generates the destination without simulating every intermediate
  quadrant.
- The project favors focused, testable game logic over production-service
  architecture. Game balance remains a learning-project limitation.

## Authorship, inspiration, and license

This C# game engine is my own implementation. It was inspired by classic
text-based space games, including the historical Star Trek game associated
with Mike Mayfield; code was not copied from that project. This acknowledgment
describes inspiration, not affiliation or endorsement.

My code is distributed under the [MIT license](LICENSE), copyright 2026 Rob.
NuGet dependencies remain subject to their respective licenses.
