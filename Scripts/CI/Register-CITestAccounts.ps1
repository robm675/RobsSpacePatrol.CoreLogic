$ErrorActionPreference = 'Stop'

if ([string]::IsNullOrWhiteSpace($env:CI_TEST_PASSWORD)) {
  throw "CI_TEST_PASSWORD GitHub Secret is missing."
}

$url = "http://127.0.0.1:5067/api/login/register"

$users = @(
  "ci-user@example.com",
  "ci-admin@example.com"
)

foreach ($email in $users) {
  $body = @{
    email    = $email
    password = $env:CI_TEST_PASSWORD
  } | ConvertTo-Json

  Write-Host "Registering $email"

  Invoke-RestMethod `
    -Uri $url `
    -Method Post `
    -ContentType "application/json" `
    -Body $body | Out-Null

  Write-Host "Registered $email successfully."
}
