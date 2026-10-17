foreach ($repo in @(
  'RobsSpacePatrol.CSharp',
  'RobsSpacePatrol.API'
)) {
  Write-Host "================================"
  Write-Host "Release preview: $repo"

  Write-Host "Source changes versus main:"
  git -C $repo diff --stat origin/main HEAD

  Write-Host "Staged working-tree changes:"
  git -C $repo status --short

  Write-Host "NuGet reference changes:"
  git -C $repo diff -- '*.csproj'
}

Write-Host "Release preparation preview completed."
Write-Host "No branches were pushed."
Write-Host "No pull requests were created."
