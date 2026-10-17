# Inspect the exact CSharp and API commits validated by the preceding CI job.
# This script runs from the GitHub Actions workspace containing both repositories.
$ErrorActionPreference = 'Stop'

if ([string]::IsNullOrWhiteSpace($env:SOURCE_BRANCH)) {
    throw 'SOURCE_BRANCH is missing.'
}

$repositories = @(
    @{ Name = 'RobsSpacePatrol.CSharp'; ExpectedSha = $env:CSHARP_VALIDATED_SHA },
    @{ Name = 'RobsSpacePatrol.API';    ExpectedSha = $env:API_VALIDATED_SHA }
)

foreach ($entry in $repositories) {
    $repo = $entry.Name
    $expectedSha = $entry.ExpectedSha
    Write-Host '================================'
    Write-Host "Repository: $repo"

    if ([string]::IsNullOrWhiteSpace($expectedSha)) {
        throw "Missing validated SHA for $repo."
    }

    $sha = (git -C $repo rev-parse HEAD).Trim()
    if ($LASTEXITCODE -ne 0) {
        throw "Cannot determine commit for $repo."
    }
    if ($sha -ne $expectedSha) {
        throw "$repo checkout ($sha) differs from validated commit ($expectedSha)."
    }
    Write-Host "Validated commit: $sha"

    git -C $repo log -1 --format='%s'
    if ($LASTEXITCODE -ne 0) {
        throw "Cannot inspect latest commit for $repo."
    }

    git -C $repo diff --name-status origin/main HEAD
    if ($LASTEXITCODE -ne 0) {
        throw "Cannot compare $repo against main."
    }

    git -C $repo merge-base --is-ancestor origin/main HEAD
    if ($LASTEXITCODE -ne 0) {
        throw "$repo is not based on current main. Revalidate before preparing a release."
    }

    # Check this for BOTH consumer repositories (previously outside the loop).
    $remoteBranch = "refs/remotes/origin/$($env:SOURCE_BRANCH)"
    git -C $repo show-ref --verify --quiet $remoteBranch
    $branchExists = ($LASTEXITCODE -eq 0)

    if ($branchExists) {
        $latestSha = (git -C $repo rev-parse $remoteBranch).Trim()
        if ($LASTEXITCODE -ne 0) {
            throw "Cannot resolve $remoteBranch for $repo."
        }
        if ($sha -ne $latestSha) {
            throw "$repo feature branch changed since validation."
        }
        Write-Host "$repo source branch matches validated commit."
    }
    else {
        # When there is no matching feature branch, the consumer checkout is main.
        $mainSha = (git -C $repo rev-parse origin/main).Trim()
        if ($LASTEXITCODE -ne 0 -or $sha -ne $mainSha) {
            throw "$repo has no matching feature branch and does not match main."
        }
        Write-Host "$repo uses main; no matching feature branch."
    }
}

$global:LASTEXITCODE = 0
Write-Host 'Both validated consumer commits passed inspection.'
