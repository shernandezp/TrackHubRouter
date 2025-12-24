# .NET 10.0 Upgrade Plan

## Execution Steps

Execute steps below sequentially one by one in the order they are listed.

1. Validate that an .NET 10.0 SDK required for this upgrade is installed on the machine and if not, help to get it installed.
2. Ensure that the SDK version specified in global.json files is compatible with the .NET 10.0 upgrade.
3. Upgrade `src\Domain\Domain.csproj`
4. Upgrade `src\Application\Application.csproj`
5. Upgrade `src\Infrastructure\Geotab\Geotab.csproj`
6. Upgrade `src\Infrastructure\CommandTrack\CommandTrack.csproj`
7. Upgrade `src\Infrastructure\Traccar\Traccar.csproj`
8. Upgrade `src\Infrastructure\Common\Common.csproj`
9. Upgrade `src\Infrastructure\ManagerApi\ManagerApi.csproj`
10. Upgrade `src\Infrastructure\GpsGate\GpsGate.csproj`
11. Upgrade `src\SyncWorker\SyncWorker.csproj`
12. Upgrade `tests\Intfrastructure.UnitTests\Infrastructure.UnitTests.csproj`
13. Upgrade `tests\Application.UnitTests\Application.UnitTests.csproj`
14. Upgrade `src\Web\Web.csproj`

## Settings

This section contains settings and data used by execution steps.

### Excluded projects

Table below contains projects that do belong to the dependency graph for selected projects and should not be included in the upgrade.

| Project name                                   | Description                 |
|:-----------------------------------------------|:---------------------------:|

### Aggregate NuGet packages modifications across all projects

NuGet packages used across all selected projects or their dependencies that need version update in projects that reference them.

| Package Name                                  | Current Version | New Version | Description                                   |
|:----------------------------------------------|:---------------:|:-----------:|:----------------------------------------------|
| Microsoft.Extensions.Configuration.Abstractions |   9.0.10        |  10.0.0     | Replace with Microsoft.Extensions.Configuration.Abstractions 10.0.0 (recommended upgrade) |
| Microsoft.Extensions.Http                      |   9.0.10        |  10.0.0     | Replace with Microsoft.Extensions.Http 10.0.0 (recommended upgrade) |
| Microsoft.Extensions.Hosting                   |   9.0.10        |  10.0.0     | Replace with Microsoft.Extensions.Hosting 10.0.0 (recommended upgrade) |

### Project upgrade details

This section contains details about each project upgrade and modifications that need to be done in the project.

#### src\Domain\Domain.csproj modifications

Project properties changes:
  - Target framework should be changed from `net9.0` to `net10.0`

NuGet packages changes:
  - No NuGet package updates were detected specifically for this project in analysis.

Feature upgrades:
  - None identified.

Other changes:
  - None identified.

#### src\Application\Application.csproj modifications

Project properties changes:
  - Target framework should be changed from `net9.0` to `net10.0`

NuGet packages changes:
  - `Microsoft.Extensions.Configuration.Abstractions` should be updated from `9.0.10` to `10.0.0` (*recommended replacement/upgraded package*).

Feature upgrades:
  - None identified.

Other changes:
  - None identified.

#### src\Infrastructure\Geotab\Geotab.csproj modifications

Project properties changes:
  - Target framework should be changed from `net9.0` to `net10.0`

NuGet packages changes:
  - No NuGet package updates were detected specifically for this project in analysis.

Feature upgrades:
  - None identified.

Other changes:
  - None identified.

#### src\Infrastructure\CommandTrack\CommandTrack.csproj modifications

Project properties changes:
  - Target framework should be changed from `net9.0` to `net10.0`

NuGet packages changes:
  - No NuGet package updates were detected specifically for this project in analysis.

Feature upgrades:
  - None identified.

Other changes:
  - None identified.

#### src\Infrastructure\Traccar\Traccar.csproj modifications

Project properties changes:
  - Target framework should be changed from `net9.0` to `net10.0`

NuGet packages changes:
  - No NuGet package updates were detected specifically for this project in analysis.

Feature upgrades:
  - None identified.

Other changes:
  - None identified.

#### src\Infrastructure\Common\Common.csproj modifications

Project properties changes:
  - Target framework should be changed from `net9.0` to `net10.0`

NuGet packages changes:
  - `Microsoft.Extensions.Http` should be updated from `9.0.10` to `10.0.0` (*recommended replacement/upgraded package*).

Feature upgrades:
  - None identified.

Other changes:
  - None identified.

#### src\Infrastructure\ManagerApi\ManagerApi.csproj modifications

Project properties changes:
  - Target framework should be changed from `net9.0` to `net10.0`

NuGet packages changes:
  - No NuGet package updates were detected specifically for this project in analysis.

Feature upgrades:
  - None identified.

Other changes:
  - None identified.

#### src\Infrastructure\GpsGate\GpsGate.csproj modifications

Project properties changes:
  - Target framework should be changed from `net9.0` to `net10.0`

NuGet packages changes:
  - No NuGet package updates were detected specifically for this project in analysis.

Feature upgrades:
  - None identified.

Other changes:
  - None identified.

#### src\SyncWorker\SyncWorker.csproj modifications

Project properties changes:
  - Target framework should be changed from `net9.0` to `net10.0`

NuGet packages changes:
  - `Microsoft.Extensions.Hosting` should be updated from `9.0.10` to `10.0.0` (*recommended replacement/upgraded package*).

Feature upgrades:
  - None identified.

Other changes:
  - None identified.

#### tests\Intfrastructure.UnitTests\Infrastructure.UnitTests.csproj modifications

Project properties changes:
  - Target framework should be changed from `net9.0` to `net10.0`

NuGet packages changes:
  - No NuGet package updates were detected specifically for this project in analysis.

Feature upgrades:
  - None identified.

Other changes:
  - None identified.

#### tests\Application.UnitTests\Application.UnitTests.csproj modifications

Project properties changes:
  - Target framework should be changed from `net9.0` to `net10.0`

NuGet packages changes:
  - No NuGet package updates were detected specifically for this project in analysis.

Feature upgrades:
  - None identified.

Other changes:
  - None identified.

#### src\Web\Web.csproj modifications

Project properties changes:
  - Target framework should be changed from `net9.0` to `net10.0`

NuGet packages changes:
  - No NuGet package updates were detected specifically for this project in analysis.

Feature upgrades:
  - None identified.

Other changes:
  - None identified.
