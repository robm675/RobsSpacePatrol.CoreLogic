param(
  [Parameter(Mandatory=$true)][string]$OutputPath
)
$ErrorActionPreference = 'Stop'
if ($env:GITHUB_REF -ne 'refs/heads/main' -or $env:GITHUB_EVENT_NAME -ne 'workflow_dispatch') {
  throw 'Release manifests may only be created by a manual dispatch on main.'
}
if ($env:RELEASE_BRANCH -ne "release-preview-$($env:GITHUB_RUN_ID)-$($env:GITHUB_RUN_ATTEMPT)") {
  throw 'Release branch is not tied to the current preparation run.'
}
$entries = [ordered]@{}
foreach ($item in @(
  @{ Name = 'RobsSpacePatrol.CSharp'; Validated = $env:CSHARP_VALIDATED_SHA },
  @{ Name = 'RobsSpacePatrol.API'; Validated = $env:API_VALIDATED_SHA }
)) {
  $repo = $item.Name
  $validated = $item.Validated
  if ($validated -notmatch '^[0-9a-fA-F]{40}$') { throw "Invalid validated SHA for $repo" }
  $head = (git -C $repo rev-parse HEAD).Trim()
  if ($LASTEXITCODE -ne 0 -or $head -notmatch '^[0-9a-fA-F]{40}$') { throw "Cannot determine prepared commit for $repo" }
  $ancestor = (git -C $repo rev-list -1 HEAD).Trim()
  if ($LASTEXITCODE -ne 0 -or $ancestor -ne $head) { throw "Invalid prepared commit for $repo" }
  git -C $repo merge-base --is-ancestor $validated HEAD
  if ($LASTEXITCODE -ne 0) { throw "Prepared commit does not contain validated commit for $repo" }
  $branch = (git -C $repo branch --show-current).Trim()
  if ($branch -ne $env:RELEASE_BRANCH) { throw "Unexpected prepared branch in $repo" }
  $entries[$repo] = [ordered]@{ validated_sha = $validated.ToLowerInvariant(); release_sha = $head.ToLowerInvariant() }
}
$manifest = [ordered]@{
  schema_version = 1
  release_branch = $env:RELEASE_BRANCH
  run_id = [long]$env:GITHUB_RUN_ID
  run_attempt = [int]$env:GITHUB_RUN_ATTEMPT
  source_repo = $env:GITHUB_REPOSITORY
  source_sha = $env:GITHUB_SHA
  consumers = $entries
}
$manifest | ConvertTo-Json -Depth 8 | Set-Content -LiteralPath $OutputPath -Encoding utf8
Write-Host "Created immutable release manifest for $($env:RELEASE_BRANCH)"
