$ErrorActionPreference = 'Stop'

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

foreach ($entry in $repositories) {
  $repo = $entry.Name
  $validatedSha = $entry.ValidatedSha

  if ([string]::IsNullOrWhiteSpace($validatedSha)) {
      throw "Missing validated commit SHA for $repo."
  }

  $sourceBranch = $env:SOURCE_BRANCH

  Write-Host "================================"
  Write-Host "Verifying $repo"

  git -C $repo fetch origin --prune
  if ($LASTEXITCODE -ne 0) {
    throw "Unable to fetch $repo"
  }

  $matchingBranch = "refs/remotes/origin/$sourceBranch"

  git -C $repo show-ref --verify --quiet $matchingBranch
  $branchExists = ($LASTEXITCODE -eq 0)

  if ($branchExists) {
    $currentSha = (git -C $repo rev-parse $matchingBranch).Trim()
    if ($LASTEXITCODE -ne 0) {
      throw "Unable to resolve $matchingBranch"
    }

    Write-Host "Source: $sourceBranch"
  }
  else {
    $currentSha = (git -C $repo rev-parse refs/remotes/origin/main).Trim()
    if ($LASTEXITCODE -ne 0) {
      throw "Unable to resolve main for $repo"
    }

    Write-Host "Source: main"
  }

  Write-Host "Validated SHA: $validatedSha"
  Write-Host "Current SHA:   $currentSha"

  if ($currentSha -ne $validatedSha) {
      throw "$repo changed since validation. Start a new workflow run."
  }

  git -C $repo merge-base --is-ancestor origin/main $validatedSha
  if ($LASTEXITCODE -ne 0) {
    throw "$repo is no longer based on current main."
  }

  Write-Host "$repo is still at its validated commit."
}

$global:LASTEXITCODE = 0
Write-Host "All release branches passed pre-push verification."
