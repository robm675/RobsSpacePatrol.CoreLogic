[CmdletBinding()]
param(
    [ValidatePattern('^\d+\.\d+\.\d+$')]
    [string]$LibraryVersion,

    [switch]$IncludeDto,

    [ValidatePattern('^\d+\.\d+\.\d+$')]
    [string]$DtoVersion
)

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'

$coreRoot = $PSScriptRoot
$repositoriesRoot = Split-Path -Parent $coreRoot
$csharpRoot = Join-Path $repositoriesRoot 'RobsSpacePatrol.CSharp'
$coreSolution = Join-Path $coreRoot 'RobsSpacePatrol.CoreLogic.sln'
$csharpSolution = Join-Path $csharpRoot 'RobsSpacePatrol.CSharp.sln'
$libraryProject = Join-Path $coreRoot 'RobsSpacePatrol.Library\RobsSpacePatrol.Library.csproj'
$dtoProject = Join-Path $coreRoot 'RobsSpacePatrol.DTO\RobsSpacePatrol.DTO.csproj'
$localPackages = Join-Path $csharpRoot 'local-packages'

$consumerProjects = @(
    (Join-Path $csharpRoot 'RobsSpacePatrol.CLI\RobsSpacePatrol.CLI.csproj'),
    (Join-Path $csharpRoot 'RobsSpacePatrol.CLI.Library\RobsSpacePatrol.CLI.Library.csproj'),
    (Join-Path $csharpRoot 'RobsSpacePatrol.CLI.Library.Tests\RobsSpacePatrol.CLI.Library.Tests.csproj')
)

function Assert-PathExists {
    param([Parameter(Mandatory)][string]$Path)

    if (-not (Test-Path -LiteralPath $Path)) {
        throw "Required path was not found: $Path"
    }
}

function Invoke-DotNet {
    param([Parameter(Mandatory)][string[]]$Arguments)

    Write-Host "`n> dotnet $($Arguments -join ' ')" -ForegroundColor Cyan
    & dotnet @Arguments

    if ($LASTEXITCODE -ne 0) {
        throw "dotnet command failed with exit code ${LASTEXITCODE}: dotnet $($Arguments -join ' ')"
    }
}

function Get-ProjectVersion {
    param([Parameter(Mandatory)][string]$Path)

    $content = [System.IO.File]::ReadAllText($Path)
    $match = [regex]::Match($content, '<Version>(\d+)\.(\d+)\.(\d+)</Version>')

    if (-not $match.Success) {
        throw "A three-part <Version> value was not found in $Path"
    }

    return $match.Groups[0].Value.Replace('<Version>', '').Replace('</Version>', '')
}

function Get-NextPatchVersion {
    param([Parameter(Mandatory)][string]$Version)

    $parts = $Version.Split('.')
    return '{0}.{1}.{2}' -f $parts[0], $parts[1], ([int]$parts[2] + 1)
}

function Set-ProjectVersion {
    param(
        [Parameter(Mandatory)][string]$Path,
        [Parameter(Mandatory)][string]$Version
    )

    $content = [System.IO.File]::ReadAllText($Path)
    $updated = [regex]::Replace(
        $content,
        '<Version>\d+\.\d+\.\d+</Version>',
        "<Version>$Version</Version>",
        1
    )

    if ($updated -eq $content) {
        throw "The package version could not be updated in $Path"
    }

    [System.IO.File]::WriteAllText($Path, $updated, [System.Text.UTF8Encoding]::new($false))
}

function Set-PackageReferenceVersion {
    param(
        [Parameter(Mandatory)][string]$Path,
        [Parameter(Mandatory)][string]$PackageId,
        [Parameter(Mandatory)][string]$Version
    )

    $content = [System.IO.File]::ReadAllText($Path)
    $pattern = '(<PackageReference\s+Include="' + [regex]::Escape($PackageId) + '"\s+Version=")[^"]+("\s*/>)'
    $matches = [regex]::Matches($content, $pattern)

    if ($matches.Count -ne 1) {
        throw "Expected exactly one $PackageId PackageReference in $Path, but found $($matches.Count)."
    }

    $updated = [regex]::Replace($content, $pattern, "`${1}$Version`${2}")
    [System.IO.File]::WriteAllText($Path, $updated, [System.Text.UTF8Encoding]::new($false))
}

Assert-PathExists $coreSolution
Assert-PathExists $csharpSolution
Assert-PathExists $libraryProject
Assert-PathExists $dtoProject
Assert-PathExists $localPackages
$consumerProjects | ForEach-Object { Assert-PathExists $_ }

if ($DtoVersion -and -not $IncludeDto) {
    throw '-DtoVersion can only be used together with -IncludeDto.'
}

$currentLibraryVersion = Get-ProjectVersion $libraryProject
if (-not $LibraryVersion) {
    $LibraryVersion = Get-NextPatchVersion $currentLibraryVersion
}

$currentDtoVersion = Get-ProjectVersion $dtoProject
if ($IncludeDto -and -not $DtoVersion) {
    $DtoVersion = Get-NextPatchVersion $currentDtoVersion
}

$libraryPackage = Join-Path $localPackages "RobsSpacePatrol.Library.$LibraryVersion.nupkg"
$dtoPackage = if ($IncludeDto) {
    Join-Path $localPackages "RobsSpacePatrol.DTO.$DtoVersion.nupkg"
}

if (Test-Path -LiteralPath $libraryPackage) {
    throw "The target Library package already exists: $libraryPackage`nChoose another version with -LibraryVersion."
}

if ($IncludeDto -and (Test-Path -LiteralPath $dtoPackage)) {
    throw "The target DTO package already exists: $dtoPackage`nChoose another version with -DtoVersion."
}

Write-Host "Updating RobsSpacePatrol.Library $currentLibraryVersion -> $LibraryVersion" -ForegroundColor Green
if ($IncludeDto) {
    Write-Host "Updating RobsSpacePatrol.DTO $currentDtoVersion -> $DtoVersion" -ForegroundColor Green
}

# Verify CoreLogic before changing any package or consumer project files.
Invoke-DotNet @('test', $coreSolution)

$filesToRestore = @($libraryProject) + $consumerProjects
if ($IncludeDto) {
    $filesToRestore += $dtoProject
}

$originalContents = @{}
foreach ($file in $filesToRestore) {
    $originalContents[$file] = [System.IO.File]::ReadAllText($file)
}

$createdPackages = [System.Collections.Generic.List[string]]::new()

try {
    if ($IncludeDto) {
        Set-ProjectVersion -Path $dtoProject -Version $DtoVersion
        Invoke-DotNet @(
            'pack', $dtoProject,
            '--configuration', 'Release',
            '--output', $localPackages
        )
        $createdPackages.Add($dtoPackage)
    }

    Set-ProjectVersion -Path $libraryProject -Version $LibraryVersion
    Invoke-DotNet @(
        'pack', $libraryProject,
        '--configuration', 'Release',
        '--output', $localPackages
    )
    $createdPackages.Add($libraryPackage)

    foreach ($project in $consumerProjects) {
        Set-PackageReferenceVersion `
            -Path $project `
            -PackageId 'RobsSpacePatrol.Library' `
            -Version $LibraryVersion
    }

    Invoke-DotNet @('restore', $csharpSolution, '--force-evaluate')
    Invoke-DotNet @('test', $csharpSolution, '--no-restore')

    Write-Host "`nCoreLogic package update completed successfully." -ForegroundColor Green
    Write-Host "CSharp now uses RobsSpacePatrol.Library $LibraryVersion." -ForegroundColor Green
    if ($IncludeDto) {
        Write-Host "The Library package uses RobsSpacePatrol.DTO $DtoVersion." -ForegroundColor Green
    }
}
catch {
    Write-Warning 'The update failed. Restoring the original project files and removing packages created by this run.'

    foreach ($file in $filesToRestore) {
        [System.IO.File]::WriteAllText(
            $file,
            $originalContents[$file],
            [System.Text.UTF8Encoding]::new($false)
        )
    }

    foreach ($package in $createdPackages) {
        if (Test-Path -LiteralPath $package) {
            Remove-Item -LiteralPath $package -Force
        }
    }

    throw
}

Read-Host "Press Enter to close"
exit 0