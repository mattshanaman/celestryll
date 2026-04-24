# Cleanup script to fix ".NET Android does not support running the previous version" error
# Run this script AFTER closing Visual Studio

Write-Host "Cleaning BadlyDefined project..." -ForegroundColor Yellow

# Remove obj and bin folders
if (Test-Path "BadlyDefined\obj") {
    Write-Host "Removing BadlyDefined\obj..." -ForegroundColor Cyan
    Remove-Item -Recurse -Force "BadlyDefined\obj" -ErrorAction SilentlyContinue
}

if (Test-Path "BadlyDefined\bin") {
    Write-Host "Removing BadlyDefined\bin..." -ForegroundColor Cyan
    Remove-Item -Recurse -Force "BadlyDefined\bin" -ErrorAction SilentlyContinue
}

# Clean GamesCore if it exists
if (Test-Path "GamesCore\obj") {
    Write-Host "Removing GamesCore\obj..." -ForegroundColor Cyan
    Remove-Item -Recurse -Force "GamesCore\obj" -ErrorAction SilentlyContinue
}

if (Test-Path "GamesCore\bin") {
    Write-Host "Removing GamesCore\bin..." -ForegroundColor Cyan
    Remove-Item -Recurse -Force "GamesCore\bin" -ErrorAction SilentlyContinue
}

# Remove .vs folder (Visual Studio cache)
if (Test-Path ".vs") {
    Write-Host "Removing .vs folder..." -ForegroundColor Cyan
    Remove-Item -Recurse -Force ".vs" -ErrorAction SilentlyContinue
}

# Remove Android deployment cache
$androidCache = Join-Path $env:LOCALAPPDATA "Xamarin\Mono for Android\Archives"
if (Test-Path $androidCache) {
    Write-Host "Removing Android deployment cache..." -ForegroundColor Cyan
    Remove-Item -Recurse -Force $androidCache -ErrorAction SilentlyContinue
}

# Clean Android emulator locks (can prevent emulator from starting)
$avdFolder = Join-Path $env:USERPROFILE ".android\avd"
if (Test-Path $avdFolder) {
    Write-Host "Cleaning emulator lock files..." -ForegroundColor Cyan
    Get-ChildItem -Path $avdFolder -Filter "*.lock" -Recurse -ErrorAction SilentlyContinue | Remove-Item -Force -ErrorAction SilentlyContinue
    Get-ChildItem -Path $avdFolder -Filter "hardware-qemu.ini.lock" -Recurse -ErrorAction SilentlyContinue | Remove-Item -Force -ErrorAction SilentlyContinue
}

Write-Host "`nCleanup complete!" -ForegroundColor Green
Write-Host "`nNext steps:" -ForegroundColor Yellow
Write-Host "1. Reopen Visual Studio" -ForegroundColor White
Write-Host "2. Clean Solution (Build -> Clean Solution)" -ForegroundColor White
Write-Host "3. Rebuild Solution (Build -> Rebuild Solution)" -ForegroundColor White
Write-Host "4. Try starting the emulator from Tools -> Device Manager" -ForegroundColor White
Write-Host "5. If emulator starts, UNINSTALL the app from it" -ForegroundColor Red
Write-Host "6. Try deploying again" -ForegroundColor White
Write-Host "`nIf emulator still won't start:" -ForegroundColor Red
Write-Host "- Check Task Manager and kill any 'qemu' processes" -ForegroundColor White
Write-Host "- Restart your computer" -ForegroundColor White
Write-Host "- Recreate the Android emulator in Device Manager" -ForegroundColor White
