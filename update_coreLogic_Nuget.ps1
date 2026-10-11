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
$apiRoot = Join-Path $repositoriesRoot 'RobsSpacePatrol.API'
$coreSolution = Join-Path $coreRoot 'RobsSpacePatrol.CoreLogic.sln'
$csharpSolution = Join-Path $csharpRoot 'RobsSpacePatrol.CSharp.sln'
$apiSolution = Join-Path $apiRoot 'RobsSpacePatrol.API.sln'
$libraryProject = Join-Path $coreRoot 'RobsSpacePatrol.Library\RobsSpacePatrol.Library.csproj'
$dtoProject = Join-Path $coreRoot 'RobsSpacePatrol.DTO\RobsSpacePatrol.DTO.csproj'
$csharpLocalPackages = Join-Path $csharpRoot 'local-packages'
$apiLocalPackages = Join-Path $apiRoot 'local-packages'

$csharpLibraryConsumers = @(
    (Join-Path $csharpRoot 'RobsSpacePatrol.CLI\RobsSpacePatrol.CLI.csproj'),
    (Join-Path $csharpRoot 'RobsSpacePatrol.CLI.Library\RobsSpacePatrol.CLI.Library.csproj'),
    (Join-Path $csharpRoot 'RobsSpacePatrol.CLI.Library.Tests\RobsSpacePatrol.CLI.Library.Tests.csproj')
)

$apiLibraryConsumers = @(
    (Join-Path $apiRoot 'RobsSpacePatrol.API\RobsSpacePatrol.API.csproj'),
    (Join-Path $apiRoot 'RobsSpacePatrol.API.Tests\RobsSpacePatrol.API.Tests.csproj')
)

$apiDtoConsumers = @(
    (Join-Path $apiRoot 'RobsSpacePatrol.API\RobsSpacePatrol.API.csproj'),
    (Join-Path $apiRoot 'RobsSpacePatrol.API.Tests\RobsSpacePatrol.API.Tests.csproj'),
    (Join-Path $apiRoot 'RobsSpacePatrol.CLI.API.Library\RobsSpacePatrol.CLI.API.Library.csproj'),
    (Join-Path $apiRoot 'RobsSpacePatrol.CLI.API\RobsSpacePatrol.CLI.API.csproj')
)

$apiTestProject = Join-Path $apiRoot 'RobsSpacePatrol.API.Tests\RobsSpacePatrol.API.Tests.csproj'

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
Assert-PathExists $apiSolution
Assert-PathExists $libraryProject
Assert-PathExists $dtoProject
Assert-PathExists $csharpLocalPackages
Assert-PathExists $apiLocalPackages
Assert-PathExists $apiTestProject
$csharpLibraryConsumers | ForEach-Object { Assert-PathExists $_ }
$apiLibraryConsumers | ForEach-Object { Assert-PathExists $_ }
$apiDtoConsumers | ForEach-Object { Assert-PathExists $_ }

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

$csharpLibraryPackage = Join-Path $csharpLocalPackages "RobsSpacePatrol.Library.$LibraryVersion.nupkg"
$apiLibraryPackage = Join-Path $apiLocalPackages "RobsSpacePatrol.Library.$LibraryVersion.nupkg"
$csharpDtoPackage = if ($IncludeDto) {
    Join-Path $csharpLocalPackages "RobsSpacePatrol.DTO.$DtoVersion.nupkg"
}
$apiDtoPackage = if ($IncludeDto) {
    Join-Path $apiLocalPackages "RobsSpacePatrol.DTO.$DtoVersion.nupkg"
}

foreach ($package in @($csharpLibraryPackage, $apiLibraryPackage)) {
    if (Test-Path -LiteralPath $package) {
        throw "The target Library package already exists: $package`nChoose another version with -LibraryVersion."
    }
}

if ($IncludeDto) {
    foreach ($package in @($csharpDtoPackage, $apiDtoPackage)) {
        if (Test-Path -LiteralPath $package) {
            throw "The target DTO package already exists: $package`nChoose another version with -DtoVersion."
        }
    }
}

Write-Host "Updating RobsSpacePatrol.Library $currentLibraryVersion -> $LibraryVersion" -ForegroundColor Green
if ($IncludeDto) {
    Write-Host "Updating RobsSpacePatrol.DTO $currentDtoVersion -> $DtoVersion" -ForegroundColor Green
}

# Verify CoreLogic before changing any package or consumer project files.
Invoke-DotNet @('test', $coreSolution)

$filesToRestore = @($libraryProject) + $csharpLibraryConsumers + $apiLibraryConsumers
if ($IncludeDto) {
    $filesToRestore += @($dtoProject) + $apiDtoConsumers
}
$filesToRestore = $filesToRestore | Select-Object -Unique

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
            '--output', $csharpLocalPackages
        )
        $createdPackages.Add($csharpDtoPackage)
        Copy-Item -LiteralPath $csharpDtoPackage -Destination $apiDtoPackage
        $createdPackages.Add($apiDtoPackage)
    }

    Set-ProjectVersion -Path $libraryProject -Version $LibraryVersion
    Invoke-DotNet @(
        'pack', $libraryProject,
        '--configuration', 'Release',
        '--output', $csharpLocalPackages
    )
    $createdPackages.Add($csharpLibraryPackage)
    Copy-Item -LiteralPath $csharpLibraryPackage -Destination $apiLibraryPackage
    $createdPackages.Add($apiLibraryPackage)

    foreach ($project in $csharpLibraryConsumers + $apiLibraryConsumers) {
        Set-PackageReferenceVersion `
            -Path $project `
            -PackageId 'RobsSpacePatrol.Library' `
            -Version $LibraryVersion
    }

    if ($IncludeDto) {
        foreach ($project in $apiDtoConsumers) {
            Set-PackageReferenceVersion `
                -Path $project `
                -PackageId 'RobsSpacePatrol.DTO' `
                -Version $DtoVersion
        }
    }

    Invoke-DotNet @('restore', $csharpSolution, '--force-evaluate')
    Invoke-DotNet @('test', $csharpSolution, '--no-restore')

    Invoke-DotNet @('restore', $apiSolution, '--force-evaluate')
    Invoke-DotNet @('build', $apiSolution, '--configuration', 'Release', '--no-restore')
    Invoke-DotNet @(
        'test', $apiTestProject,
        '--configuration', 'Release',
        '--no-build',
        '--no-restore'
    )

    Write-Host "`nCoreLogic package update completed successfully." -ForegroundColor Green
    Write-Host "CSharp now uses RobsSpacePatrol.Library $LibraryVersion." -ForegroundColor Green
    Write-Host "API now uses RobsSpacePatrol.Library $LibraryVersion." -ForegroundColor Green
    if ($IncludeDto) {
        Write-Host "The Library package and API projects use RobsSpacePatrol.DTO $DtoVersion." -ForegroundColor Green
    }
    Write-Host 'The server-dependent CLI API integration tests were not run.' -ForegroundColor Yellow
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
