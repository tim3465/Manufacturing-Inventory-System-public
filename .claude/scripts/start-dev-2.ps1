# start-dev-2.ps1
# Launches a secondary dev environment on alternate ports (7137 / 4201).
# Uses $PSScriptRoot so this script works from any folder name (main, worktrees, etc.)

$root = Resolve-Path "$PSScriptRoot\..\.."

$backendPath  = Join-Path $root "backend\CncApp"
$frontendPath = Join-Path $root "frontend\angular"

Write-Host "Starting backend (secondary): $backendPath" -ForegroundColor Cyan
Start-Process powershell -ArgumentList @(
    "-NoExit", "-Command",
    "cd '$backendPath'; dotnet run --project CncApp.Api --launch-profile https --urls `"https://localhost:7137`""
)

Write-Host "Starting frontend (secondary): $frontendPath" -ForegroundColor Cyan
Start-Process powershell -ArgumentList @(
    "-NoExit", "-Command",
    "cd '$frontendPath'; npm install; npx ng serve --port 4201 --proxy-config proxy-2.conf.json"
)

Write-Host ""
Write-Host "Secondary dev environment launched." -ForegroundColor Green
Write-Host "  Backend:  https://localhost:7137" -ForegroundColor Green
Write-Host "  Frontend: http://localhost:4201" -ForegroundColor Green
