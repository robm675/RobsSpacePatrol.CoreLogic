[CmdletBinding(SupportsShouldProcess)]
param(
    [ValidateNotNullOrEmpty()]
    [string]$CommitMessage = "Initial public release"
)

Set-StrictMode -Version Latest
$ErrorActionPreference = "Stop"

$repositoryRoot = $PSScriptRoot
$solution = Join-Path $repositoryRoot "RobsSpacePatrol.CoreLogic.sln"
$expectedRemote = "https://github.com/robm675/RobsSpacePatrol.CoreLogic.git"

function Invoke-DotNet {
    param([Parameter(Mandatory)][string[]]$Arguments)

    Write-Host "`n> dotnet $($Arguments -join ' ')" -ForegroundColor Cyan
    & dotnet @Arguments
    if ($LASTEXITCODE -ne 0) {
        throw "dotnet failed with exit code $LASTEXITCODE."
    }
}

function Invoke-Git {
    param([Parameter(Mandatory)][string[]]$Arguments)

    Write-Host "`n> git $($Arguments -join ' ')" -ForegroundColor Cyan
    & git @Arguments
    if ($LASTEXITCODE -ne 0) {
        throw "git failed with exit code $LASTEXITCODE."
    }
}

if (-not (Get-Command dotnet -ErrorAction SilentlyContinue)) {
    throw "dotnet was not found on PATH."
}
if (-not (Get-Command git -ErrorAction SilentlyContinue)) {
    throw "git was not found on PATH."
}
if (-not (Test-Path -LiteralPath $solution -PathType Leaf)) {
    throw "Solution not found: $solution"
}

Push-Location $repositoryRoot
try {
    & git rev-parse --is-inside-work-tree *> $null
    if ($LASTEXITCODE -ne 0) {
        throw "This directory is not an initialized Git repository."
    }

    $branch = (& git branch --show-current).Trim()
    if ($LASTEXITCODE -ne 0 -or $branch -ne "main") {
        throw "The current branch must be main. Current branch: $branch"
    }

    $origin = (& git remote get-url origin).Trim()
    if ($LASTEXITCODE -ne 0 -or $origin -ne $expectedRemote) {
        throw "The origin remote must be $expectedRemote. Current value: $origin"
    }

    $gitName = (& git config user.name).Trim()
    $gitEmail = (& git config user.email).Trim()
    if (-not $gitName -or -not $gitEmail) {
        throw "Configure git user.name and user.email before publishing."
    }

    $remoteMain = & git ls-remote --heads origin refs/heads/main
    if ($LASTEXITCODE -ne 0) {
        throw "GitHub could not be reached or authenticated."
    }

    & git rev-parse --verify --quiet HEAD 2> $null
    $hasLocalCommit = $LASTEXITCODE -eq 0

    if ($remoteMain) {
        if (-not $hasLocalCommit) {
            throw "The remote main branch already has history, but this local repository does not. Resolve that history before publishing."
        }

        Invoke-Git @("fetch", "origin", "main")
        & git merge-base --is-ancestor origin/main HEAD
        if ($LASTEXITCODE -ne 0) {
            throw "The local main branch does not contain origin/main. Pull and resolve the histories before publishing."
        }
    }

    Invoke-DotNet @("restore", $solution, "--force-evaluate")
    Invoke-DotNet @("test", $solution, "--configuration", "Release", "--no-restore")

    Write-Host "`nFiles currently eligible for the commit:" -ForegroundColor Yellow
    & git status --short --untracked-files=all
    if ($LASTEXITCODE -ne 0) {
        throw "git status failed."
    }

    if (-not $PSCmdlet.ShouldProcess($expectedRemote, "commit the current repository contents and push main")) {
        return
    }

    $confirmation = Read-Host "Type PUBLISH to commit and push these files"
    if ($confirmation -cne "PUBLISH") {
        throw "Publishing cancelled; no files were staged, committed, or pushed."
    }

    Invoke-Git @("add", "--all")
    Invoke-Git @("diff", "--cached", "--check")

    & git diff --cached --quiet
    $hasStagedChanges = $LASTEXITCODE -ne 0
    if ($hasStagedChanges) {
        Invoke-Git @("commit", "-m", $CommitMessage)
    }
    elseif (-not $hasLocalCommit) {
        throw "There are no changes to commit and no existing local commit to push."
    }
    else {
        Write-Host "No new changes to commit; the existing local commits will be pushed." -ForegroundColor Yellow
    }

    Invoke-Git @("push", "--set-upstream", "origin", "main")
    Write-Host "`nPublished successfully to $expectedRemote" -ForegroundColor Green
}
finally {
    Pop-Location
}
