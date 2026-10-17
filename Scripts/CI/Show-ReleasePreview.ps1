foreach ($repo in @(
  'RobsSpacePatrol.CSharp',
  'RobsSpacePatrol.API'
)) {
  Write-Host "================================"
  Write-Host "Release preview: $repo"

  Write-Host "Source changes versus main:"
  git -C $repo diff --stat origin/main HEAD
  if ($LASTEXITCODE -ne 0) { throw "Unable to compare $repo with main" }

  Write-Host "Staged working-tree changes:"
  git -C $repo status --short
  if ($LASTEXITCODE -ne 0) { throw "Unable to get working-tree status for $repo" }

  Write-Host "NuGet reference changes:"
  git -C $repo diff -- '*.csproj'
  if ($LASTEXITCODE -ne 0) { throw "Unable to inspect project references for $repo" }
}

Write-Host "Release preparation preview completed."
Write-Host "No branches were pushed."
Write-Host "No pull requests were created."
