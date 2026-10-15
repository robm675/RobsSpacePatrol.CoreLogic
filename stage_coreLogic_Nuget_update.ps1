[CmdletBinding()]
param(
    [ValidatePattern('^\d+\.\d+\.\d+$')]
    [string]$LibraryVersion,

    [switch]$IncludeDto,

    [ValidatePattern('^\d+\.\d+\.\d+$')]
    [string]$DtoVersion,

    [switch]$NoPause
)

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'

$coreRoot = $PSScriptRoot
$workspace = Split-Path -Parent $coreRoot
$csharpRoot = Join-Path $workspace 'RobsSpacePatrol.CSharp'
$apiRoot = Join-Path $workspace 'RobsSpacePatrol.API'
$libraryProject = Join-Path $coreRoot 'RobsSpacePatrol.Library\RobsSpacePatrol.Library.csproj'
$dtoProject = Join-Path $coreRoot 'RobsSpacePatrol.DTO\RobsSpacePatrol.DTO.csproj'
$stagingScript = Join-Path $coreRoot 'Scripts\Stage-CoreLogicPackages.ps1'
$csharpPackages = Join-Path $csharpRoot 'local-packages'
$apiPackages = Join-Path $apiRoot 'local-packages'

$consumerProjects = @(
    (Join-Path $csharpRoot 'RobsSpacePatrol.CLI\RobsSpacePatrol.CLI.csproj'),
    (Join-Path $csharpRoot 'RobsSpacePatrol.CLI.Library\RobsSpacePatrol.CLI.Library.csproj'),
    (Join-Path $csharpRoot 'RobsSpacePatrol.CLI.Library.Tests\RobsSpacePatrol.CLI.Library.Tests.csproj'),
    (Join-Path $apiRoot 'RobsSpacePatrol.API\RobsSpacePatrol.API.csproj'),
    (Join-Path $apiRoot 'RobsSpacePatrol.API.Tests\RobsSpacePatrol.API.Tests.csproj'),
    (Join-Path $apiRoot 'RobsSpacePatrol.CLI.API.Library\RobsSpacePatrol.CLI.API.Library.csproj'),
    (Join-Path $apiRoot 'RobsSpacePatrol.CLI.API\RobsSpacePatrol.CLI.API.csproj')
)

function Assert-PathExists {
    param(
        [Parameter(Mandatory)][string]$Path,
        [switch]$Container
    )

    $pathType = if ($Container) { 'Container' } else { 'Leaf' }
    if (-not (Test-Path -LiteralPath $Path -PathType $pathType)) {
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

    $content = [IO.File]::ReadAllText($Path)
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

    $content = [IO.File]::ReadAllText($Path)
    $updated = [regex]::Replace(
        $content,
        '<Version>\d+\.\d+\.\d+</Version>',
        "<Version>$Version</Version>",
        1
    )

    if ($updated -eq $content) {
        throw "The package version could not be updated in $Path"
    }

    [IO.File]::WriteAllText($Path, $updated, [Text.UTF8Encoding]::new($false))
}

function Find-ExistingPackage {
    param(
        [Parameter(Mandatory)][string]$PackageId,
        [Parameter(Mandatory)][string]$Version
    )

    $fileName = "$PackageId.$Version.nupkg"
    foreach ($folder in @($csharpPackages, $apiPackages)) {
        $candidate = Join-Path $folder $fileName
        if (Test-Path -LiteralPath $candidate -PathType Leaf) {
            return $candidate
        }
    }

    throw "The existing package $fileName was not found in either consumer's local-packages folder. Use -IncludeDto to create it."
}

function Get-ProjectAssemblyVersion {
    param([Parameter(Mandatory)][string]$Path)

    $content = [IO.File]::ReadAllText($Path)
    $match = [regex]::Match($content, '<AssemblyVersion>([^<]+)</AssemblyVersion>')

    if (-not $match.Success) {
        throw "An <AssemblyVersion> value was not found in $Path"
    }

    return [Version]$match.Groups[1].Value
}

function Get-PackageAssemblyVersion {
    param(
        [Parameter(Mandatory)][string]$PackagePath,
        [Parameter(Mandatory)][string]$AssemblyFileName,
        [Parameter(Mandatory)][string]$ExtractionFolder
    )

    $temporaryAssembly = Join-Path $ExtractionFolder (
        [Guid]::NewGuid().ToString('N') + '-' + $AssemblyFileName
    )
    $archive = [IO.Compression.ZipFile]::OpenRead($PackagePath)

    try {
        $entry = $archive.Entries |
            Where-Object { $_.FullName -match ('/lib/|^lib/') -and $_.Name -eq $AssemblyFileName } |
            Select-Object -First 1

        if (-not $entry) {
            throw "$AssemblyFileName was not found in package $PackagePath"
        }

        $inputStream = $entry.Open()
        $outputStream = [IO.File]::Create($temporaryAssembly)
        try {
            $inputStream.CopyTo($outputStream)
        }
        finally {
            $outputStream.Dispose()
            $inputStream.Dispose()
        }

        return [Reflection.AssemblyName]::GetAssemblyName($temporaryAssembly).Version
    }
    finally {
        $archive.Dispose()
        if (Test-Path -LiteralPath $temporaryAssembly) {
            Remove-Item -LiteralPath $temporaryAssembly -Force
        }
    }
}

function Wait-ForExit {
    if (-not $NoPause) {
        [void](Read-Host "`nPress Enter to finish")
    }
}

if ($DtoVersion -and -not $IncludeDto) {
    throw '-DtoVersion can only be used together with -IncludeDto.'
}

Assert-PathExists $libraryProject
Assert-PathExists $dtoProject
Assert-PathExists $stagingScript
Assert-PathExists $csharpPackages -Container
Assert-PathExists $apiPackages -Container
$consumerProjects | ForEach-Object { Assert-PathExists $_ }

$currentLibraryVersion = Get-ProjectVersion $libraryProject
$currentDtoVersion = Get-ProjectVersion $dtoProject

if (-not $LibraryVersion) {
    $LibraryVersion = Get-NextPatchVersion $currentLibraryVersion
}
if ($IncludeDto -and -not $DtoVersion) {
    $DtoVersion = Get-NextPatchVersion $currentDtoVersion
}

$stagedDtoVersion = if ($IncludeDto) { $DtoVersion } else { $currentDtoVersion }
$packageNames = @(
    "RobsSpacePatrol.Library.$LibraryVersion.nupkg",
    "RobsSpacePatrol.DTO.$stagedDtoVersion.nupkg"
)

foreach ($folder in @($csharpPackages, $apiPackages)) {
    $libraryDestination = Join-Path $folder $packageNames[0]
    if (Test-Path -LiteralPath $libraryDestination) {
        throw "The target Library package already exists: $libraryDestination"
    }

    if ($IncludeDto) {
        $dtoDestination = Join-Path $folder $packageNames[1]
        if (Test-Path -LiteralPath $dtoDestination) {
            throw "The target DTO package already exists: $dtoDestination"
        }
    }
}

$filesToRestore = @($libraryProject) + $consumerProjects
if ($IncludeDto) {
    $filesToRestore += $dtoProject
}
$filesToRestore = $filesToRestore | Select-Object -Unique

$originalContents = @{}
foreach ($file in $filesToRestore) {
    $originalContents[$file] = [IO.File]::ReadAllText($file)
}

$packageExistenceBeforeRun = @{}
foreach ($folder in @($csharpPackages, $apiPackages)) {
    foreach ($packageName in $packageNames) {
        $path = Join-Path $folder $packageName
        $packageExistenceBeforeRun[$path] = Test-Path -LiteralPath $path
    }
}

$tempBase = [IO.Path]::GetFullPath([IO.Path]::GetTempPath())
$tempPackages = Join-Path $tempBase ("RobsSpacePatrol-Packages-" + [Guid]::NewGuid().ToString('N'))
[void](New-Item -ItemType Directory -Path $tempPackages)

Write-Host "Staging RobsSpacePatrol.Library $currentLibraryVersion -> $LibraryVersion" -ForegroundColor Green
if ($IncludeDto) {
    Write-Host "Staging RobsSpacePatrol.DTO $currentDtoVersion -> $DtoVersion" -ForegroundColor Green
}
else {
    Write-Host "Keeping RobsSpacePatrol.DTO at stable version $currentDtoVersion" -ForegroundColor Green
}

try {
    if ($IncludeDto) {
        Set-ProjectVersion -Path $dtoProject -Version $DtoVersion
        Invoke-DotNet @(
            'pack', $dtoProject,
            '--configuration', 'Release',
            '--output', $tempPackages
        )
    }
    else {
        $existingDtoPackage = Find-ExistingPackage `
            -PackageId 'RobsSpacePatrol.DTO' `
            -Version $currentDtoVersion

        $projectDtoAssemblyVersion = Get-ProjectAssemblyVersion -Path $dtoProject
        $packageDtoAssemblyVersion = Get-PackageAssemblyVersion `
            -PackagePath $existingDtoPackage `
            -AssemblyFileName 'RobsSpacePatrol.DTO.dll' `
            -ExtractionFolder $tempPackages

        if ($projectDtoAssemblyVersion -ne $packageDtoAssemblyVersion) {
            throw "The DTO project assembly version ($projectDtoAssemblyVersion) does not match package $currentDtoVersion ($packageDtoAssemblyVersion). Run again with -IncludeDto."
        }

        Copy-Item -LiteralPath $existingDtoPackage -Destination $tempPackages
    }

    Set-ProjectVersion -Path $libraryProject -Version $LibraryVersion
    Invoke-DotNet @(
        'pack', $libraryProject,
        '--configuration', 'Release',
        '--output', $tempPackages
    )

    & $stagingScript -Workspace $workspace -PackagesPath $tempPackages
    if ($LASTEXITCODE -ne 0) {
        throw "Package staging failed with exit code $LASTEXITCODE."
    }

    Write-Host "`nCoreLogic packages and consumer references were staged successfully." -ForegroundColor Green
    Write-Host "No consumer restore, build, or test command was run." -ForegroundColor Yellow
    Write-Host "Commit the coordinated changes on feature branches and let GitHub Actions validate them." -ForegroundColor Yellow
}
catch {
    Write-Warning 'Staging failed. Restoring project files and removing packages created by this run.'

    foreach ($file in $filesToRestore) {
        [IO.File]::WriteAllText(
            $file,
            $originalContents[$file],
            [Text.UTF8Encoding]::new($false)
        )
    }

    foreach ($entry in $packageExistenceBeforeRun.GetEnumerator()) {
        if (-not $entry.Value -and (Test-Path -LiteralPath $entry.Key)) {
            Remove-Item -LiteralPath $entry.Key -Force
        }
    }

    throw
}
finally {
    if (Test-Path -LiteralPath $tempPackages) {
        $resolvedTempPackages = [IO.Path]::GetFullPath($tempPackages)
        $expectedPrefix = $tempBase.TrimEnd(
            [IO.Path]::DirectorySeparatorChar,
            [IO.Path]::AltDirectorySeparatorChar
        ) + [IO.Path]::DirectorySeparatorChar + 'RobsSpacePatrol-Packages-'

        if (-not $resolvedTempPackages.StartsWith(
            $expectedPrefix,
            [StringComparison]::OrdinalIgnoreCase
        )) {
            throw "Refusing to remove unexpected temporary path: $resolvedTempPackages"
        }

        Remove-Item -LiteralPath $resolvedTempPackages -Recurse -Force
    }

    Wait-ForExit
}
