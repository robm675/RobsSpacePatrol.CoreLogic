[CmdletBinding()]
param(
    [Parameter(Mandatory)][string]$Workspace,
    [Parameter(Mandatory)][string]$PackagesPath
)
Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'

$csharpRoot = Join-Path $Workspace 'RobsSpacePatrol.CSharp'
$apiRoot    = Join-Path $Workspace 'RobsSpacePatrol.API'

function Assert-File([string]$Path) {
    if (-not (Test-Path -LiteralPath $Path -PathType Leaf)) { throw "Missing required file: $Path" }
}
function Set-Reference([string]$Project, [string]$PackageId, [string]$Version) {
    Assert-File $Project
    $content = [IO.File]::ReadAllText($Project)
    $pattern = '(<PackageReference\s+Include="' + [regex]::Escape($PackageId) + '"\s+Version=")[^"]+("\s*/>)'
    $matches = [regex]::Matches($content, $pattern)
    if ($matches.Count -ne 1) { throw "Expected exactly one inline version reference for $PackageId in $Project; found $($matches.Count)" }
    $updated = [regex]::Replace($content, $pattern, ('${1}' + $Version + '${2}'))
    [IO.File]::WriteAllText($Project, $updated, [Text.UTF8Encoding]::new($false))
    Write-Host "Updated $PackageId to $Version in $(Split-Path $Project -Leaf)"
}
function Get-Package([string]$Id) {
    $escaped = [regex]::Escape($Id)
    $files = @(Get-ChildItem -LiteralPath $PackagesPath -Filter '*.nupkg' -File | Where-Object { $_.Name -match ('^' + $escaped + '\.(\d+\.\d+\.\d+(?:-[A-Za-z0-9.-]+)?)\.nupkg$') })
    if ($files.Count -ne 1) { throw "Expected exactly one $Id package in $PackagesPath, found $($files.Count)" }
    $version = [regex]::Match($files[0].Name, '^' + $escaped + '\.(.+)\.nupkg$').Groups[1].Value
    return @{ File = $files[0]; Version = $version }
}

$lib = Get-Package 'RobsSpacePatrol.Library'
$dto = Get-Package 'RobsSpacePatrol.DTO'
Write-Host "Staging Library $($lib.Version) and DTO $($dto.Version)"

foreach ($root in @($csharpRoot, $apiRoot)) {
    $folder = Join-Path $root 'local-packages'
    if (-not (Test-Path -LiteralPath $folder -PathType Container)) { throw "Missing local-packages folder: $folder" }
    foreach ($pkg in @($lib, $dto)) {
        $destination = Join-Path $folder $pkg.File.Name
        # An unchanged version must have identical contents; silently replacing the same version is unsafe.
        if (Test-Path -LiteralPath $destination) {
            $oldHash = (Get-FileHash -LiteralPath $destination -Algorithm SHA256).Hash
            $newHash = (Get-FileHash -LiteralPath $pkg.File.FullName -Algorithm SHA256).Hash
            if ($oldHash -ne $newHash) { throw "Version collision: $destination already exists with DIFFERENT bytes. Increase the package version before releasing." }
        } else {
            Copy-Item -LiteralPath $pkg.File.FullName -Destination $destination
        }
    }
}

$csharpLibraryConsumers = @(
    'RobsSpacePatrol.CLI/RobsSpacePatrol.CLI.csproj',
    'RobsSpacePatrol.CLI.Library/RobsSpacePatrol.CLI.Library.csproj',
    'RobsSpacePatrol.CLI.Library.Tests/RobsSpacePatrol.CLI.Library.Tests.csproj'
)
$apiLibraryConsumers = @(
    'RobsSpacePatrol.API/RobsSpacePatrol.API.csproj',
    'RobsSpacePatrol.API.Tests/RobsSpacePatrol.API.Tests.csproj'
)
$apiDtoConsumers = @(
    'RobsSpacePatrol.API/RobsSpacePatrol.API.csproj',
    'RobsSpacePatrol.API.Tests/RobsSpacePatrol.API.Tests.csproj',
    'RobsSpacePatrol.CLI.API.Library/RobsSpacePatrol.CLI.API.Library.csproj',
    'RobsSpacePatrol.CLI.API/RobsSpacePatrol.CLI.API.csproj'
)
foreach ($p in $csharpLibraryConsumers) { Set-Reference (Join-Path $csharpRoot $p) 'RobsSpacePatrol.Library' $lib.Version }
foreach ($p in $apiLibraryConsumers) { Set-Reference (Join-Path $apiRoot $p) 'RobsSpacePatrol.Library' $lib.Version }
foreach ($p in $apiDtoConsumers) { Set-Reference (Join-Path $apiRoot $p) 'RobsSpacePatrol.DTO' $dto.Version }

Write-Host 'Package files and references staged in temporary checkouts.'
