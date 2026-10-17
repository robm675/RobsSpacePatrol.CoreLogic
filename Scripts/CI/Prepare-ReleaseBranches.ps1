$ErrorActionPreference = 'Stop'

$releaseBranch = "release-preview-$($env:GITHUB_RUN_ID)-$($env:GITHUB_RUN_ATTEMPT)"

foreach ($repo in @(
  'RobsSpacePatrol.CSharp',
  'RobsSpacePatrol.API'
)) {
  Write-Host "================================"
  Write-Host "Preparing local release branch for $repo"

  Push-Location $repo
  try {
    git config user.name "CoreLogic CI"
    git config user.email "corelogic-ci@users.noreply.github.com"

    git switch -c $releaseBranch
    if ($LASTEXITCODE -ne 0) {
      throw "Unable to create release branch for $repo"
    }

    git add -- '*.csproj' local-packages
    if ($LASTEXITCODE -ne 0) {
      throw "Unable to stage release files for $repo"
    }

    $changed = @(git diff --cached --name-only)
    if ($LASTEXITCODE -ne 0) {
      throw "Unable to inspect staged files for $repo"
    }

    if ($changed.Count -gt 0) {
      git commit -m "Prepare validated CoreLogic package release"

      if ($LASTEXITCODE -ne 0) {
        throw "Release commit failed for $repo"
      }
    }
    else {
      Write-Host "No package changes needed for $repo"
    }

    $releaseSha = (git rev-parse HEAD).Trim()
    if ($LASTEXITCODE -ne 0) {
      throw "Unable to identify release commit for $repo"
    }

    Write-Host "Local release branch: $releaseBranch"
    Write-Host "Release commit: $releaseSha"

    Write-Host "Proposed changes compared with main:"
    git diff --stat origin/main HEAD
    if ($LASTEXITCODE -ne 0) {
      throw "Unable to inspect release differences for $repo"
    }
  }
  finally {
    Pop-Location
  }
}

Write-Host "Local release branches prepared successfully."
Write-Host "Nothing was pushed to GitHub."
