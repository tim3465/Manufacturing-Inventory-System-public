# start-dev.ps1
# Launches the full dev environment in two separate terminal windows.
# Uses $PSScriptRoot so this script works from any folder name (main, worktrees, etc.)

$root = Resolve-Path "$PSScriptRoot\..\.."

$backendPath  = Join-Path $root "backend\CncApp"
$frontendPath = Join-Path $root "frontend\angular"

Write-Host "Starting backend: $backendPath" -ForegroundColor Cyan
Start-Process powershell -ArgumentList @(
    "-NoExit", "-Command",
    "cd '$backendPath'; dotnet run --project CncApp.Api --launch-profile https"
)

Write-Host "Starting frontend: $frontendPath" -ForegroundColor Cyan
Start-Process powershell -ArgumentList @(
    "-NoExit", "-Command",
    "cd '$frontendPath'; npm install; npm start"
)

Write-Host ""
Write-Host "Dev environment launched." -ForegroundColor Green
Write-Host "  Backend:  https://localhost:7136" -ForegroundColor Green
Write-Host "  Frontend: http://localhost:4200" -ForegroundColor Green
