# Quick Start - Build BadlyDefined Project
# Run this script to quickly clean and build BadlyDefined

Write-Host ""
Write-Host "=====================================" -ForegroundColor Cyan
Write-Host "  BadlyDefined - Quick Build" -ForegroundColor Cyan
Write-Host "=====================================" -ForegroundColor Cyan
Write-Host ""

# Stop any locked processes
Write-Host "Stopping Visual Studio processes..." -ForegroundColor Yellow
Get-Process | Where-Object {$_.ProcessName -like "*devenv*" -or $_.ProcessName -like "*MSBuild*"} | Stop-Process -Force -ErrorAction SilentlyContinue
Start-Sleep -Seconds 2

# Clean GamesCore
Write-Host ""
Write-Host "Cleaning GamesCore..." -ForegroundColor Yellow
if (Test-Path "GamesCore\bin") { Remove-Item "GamesCore\bin" -Recurse -Force -ErrorAction SilentlyContinue }
if (Test-Path "GamesCore\obj") { Remove-Item "GamesCore\obj" -Recurse -Force -ErrorAction SilentlyContinue }
dotnet clean "GamesCore\GamesCore.csproj" --nologo -v quiet
Write-Host "✓ GamesCore cleaned" -ForegroundColor Green

# Clean BadlyDefined
Write-Host ""
Write-Host "Cleaning BadlyDefined..." -ForegroundColor Yellow
if (Test-Path "BadlyDefined\bin") { Remove-Item "BadlyDefined\bin" -Recurse -Force -ErrorAction SilentlyContinue }
if (Test-Path "BadlyDefined\obj") { Remove-Item "BadlyDefined\obj" -Recurse -Force -ErrorAction SilentlyContinue }
dotnet clean "BadlyDefined\BadlyDefined.csproj" --nologo -v quiet
Write-Host "✓ BadlyDefined cleaned" -ForegroundColor Green

# Build solution
Write-Host ""
Write-Host "Building BadlyDefined.sln..." -ForegroundColor Yellow
Write-Host ""
dotnet build "BadlyDefined.sln" --nologo

if ($LASTEXITCODE -eq 0) {
    Write-Host ""
    Write-Host "=====================================" -ForegroundColor Green
    Write-Host "  ✓ BUILD SUCCESSFUL!" -ForegroundColor Green
    Write-Host "=====================================" -ForegroundColor Green
    Write-Host ""
    Write-Host "Next steps:" -ForegroundColor Cyan
    Write-Host "1. Open BadlyDefined.sln in Visual Studio" -ForegroundColor White
    Write-Host "2. Set BadlyDefined as startup project" -ForegroundColor White
    Write-Host "3. Select your target platform (Android/iOS/etc.)" -ForegroundColor White
    Write-Host "4. Press F5 to run!" -ForegroundColor White
} else {
    Write-Host ""
    Write-Host "=====================================" -ForegroundColor Red
    Write-Host "  ✗ BUILD FAILED" -ForegroundColor Red
    Write-Host "=====================================" -ForegroundColor Red
    Write-Host ""
    Write-Host "Check the errors above and try:" -ForegroundColor Yellow
    Write-Host "1. Close Visual Studio completely" -ForegroundColor White
    Write-Host "2. Run this script again" -ForegroundColor White
    Write-Host "3. Check PROJECT_STRUCTURE_ISOLATED.md for troubleshooting" -ForegroundColor White
}

Write-Host ""
