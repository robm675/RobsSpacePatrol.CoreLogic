$ErrorActionPreference = 'Stop'

if ([string]::IsNullOrWhiteSpace($env:GH_TOKEN)) {
  throw "CROSS_REPO_WRITE_TOKEN is missing."
}

gh auth setup-git
if ($LASTEXITCODE -ne 0) {
  throw "Unable to configure Git authentication."
}

$repositories = @(
  @{
    Name = 'RobsSpacePatrol.CSharp'
    ValidatedSha = $env:CSHARP_VALIDATED_SHA
  },
  @{
    Name = 'RobsSpacePatrol.API'
    ValidatedSha = $env:API_VALIDATED_SHA
  }
)

# PASS 1: Verify both repositories before publishing either.

Write-Host "=== RELEASE PREFLIGHT ==="

foreach ($entry in $repositories) {
  $repo = $entry.Name
  $validatedSha = $entry.ValidatedSha

  Write-Host "Checking $repo"

  if ([string]::IsNullOrWhiteSpace($validatedSha)) {
    throw "Missing validated SHA for $repo."
  }

  Push-Location $repo
  try {
    git fetch origin --prune
    if ($LASTEXITCODE -ne 0) {
      throw "Fetch failed for $repo."
    }

    $branch = (git branch --show-current).Trim()
    if ($branch -ne $env:RELEASE_BRANCH) {
      throw "Unexpected release branch in $repo."
    }

    $sourceRef = "refs/remotes/origin/$($env:SOURCE_BRANCH)"

    git show-ref --verify --quiet $sourceRef
    $sourceExists = ($LASTEXITCODE -eq 0)

    if ($sourceExists) {
      $currentSha = (git rev-parse $sourceRef).Trim()
      if ($LASTEXITCODE -ne 0) {
        throw "Unable to resolve source branch for $repo."
      }
    }
    else {
      $currentSha = (git rev-parse origin/main).Trim()
      if ($LASTEXITCODE -ne 0) {
        throw "Unable to resolve main for $repo."
      }
    }

    if ($currentSha -ne $validatedSha) {
      throw "$repo source changed since validation."
    }

    git merge-base --is-ancestor origin/main $validatedSha
    if ($LASTEXITCODE -ne 0) {
      throw "$repo main changed since validation."
    }

    git merge-base --is-ancestor $validatedSha HEAD
    if ($LASTEXITCODE -ne 0) {
      throw "$repo release does not contain validated commit."
    }

    # The release preparation may only add packages
    # and update project references.
    $releaseFiles = @(git diff --name-only $validatedSha HEAD)
    if ($LASTEXITCODE -ne 0) {
      throw "Unable to inspect release files for $repo."
    }

    foreach ($file in $releaseFiles) {
      if ($file -notmatch '(^local-packages/.*\.nupkg$|\.csproj$)') {
        throw "Unexpected release file in $repo`: $file"
      }
    }

    $dirty = @(git status --porcelain)
    if ($LASTEXITCODE -ne 0 -or $dirty.Count -gt 0) {
      throw "Release working tree is not clean in $repo."
    }

    # Avoid overlapping coordinated release PRs.
    $existingJson = gh pr list `
      --repo "robm675/$repo" `
      --state open `
      --base main `
      --limit 100 `
      --json number,headRefName,title

    if ($LASTEXITCODE -ne 0) {
      throw "Unable to inspect existing PRs in $repo."
    }

    $existingPRs = @($existingJson | ConvertFrom-Json)

    $duplicates = @(
      $existingPRs | Where-Object {
        $_.headRefName -like 'release-preview-*' -or
        $_.title -like 'Coordinated CoreLogic release *'
      }
    )

    if ($duplicates.Count -gt 0) {
      throw "Existing coordinated release PR in $repo. Review it first."
    }

    git ls-remote --exit-code --heads origin $env:RELEASE_BRANCH *> $null
    if ($LASTEXITCODE -eq 0) {
      throw "Remote release branch already exists in $repo."
    }
    elseif ($LASTEXITCODE -ne 2) {
      throw "Unable to verify remote release branch in $repo."
    }

    Write-Host "$repo passed release preflight."
  }
  finally {
    Pop-Location
  }
}

Write-Host "Both repositories passed preflight."


# PASS 2: Publish the prepared release branches and PRs.

Write-Host "=== PUBLISH RELEASE ==="

foreach ($entry in $repositories) {
  $repo = $entry.Name
  Push-Location $repo

  try {
    Write-Host "Publishing $repo"

    git push --set-upstream origin $env:RELEASE_BRANCH
    if ($LASTEXITCODE -ne 0) {
      throw "Unable to push release branch for $repo."
    }

    gh pr create `
      --repo "robm675/$repo" `
      --base main `
      --head $env:RELEASE_BRANCH `
      --title "Coordinated CoreLogic release $env:GITHUB_RUN_ID" `
      --body "Prepared using validated CoreLogic NuGet packages and consumer commits. Review all checks before merging."

    if ($LASTEXITCODE -ne 0) {
      throw "Unable to create release PR for $repo."
    }

    Write-Host "Release PR created for $repo."
  }
  finally {
    Pop-Location
  }
}

Write-Host "Both release PRs created successfully."
Write-Host "No pull requests were merged."
