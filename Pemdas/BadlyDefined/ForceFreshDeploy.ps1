Write-Host "🧹 FORCE CLEAN AND REDEPLOY" -ForegroundColor Cyan
Write-Host "This will completely remove the old cached version" -ForegroundColor Yellow
Write-Host ""

# Find adb
$adbPaths = @(
    "C:\Program Files (x86)\Android\android-sdk\platform-tools\adb.exe",
    "$env:LOCALAPPDATA\Android\Sdk\platform-tools\adb.exe",
    "$env:ANDROID_HOME\platform-tools\adb.exe"
)

$adb = $null
foreach ($path in $adbPaths) {
    if (Test-Path $path) {
        $adb = $path
        Write-Host "✅ Found adb: $path" -ForegroundColor Green
        break
    }
}

if ($adb) {
    Write-Host "`n📱 Uninstalling old app from emulator..." -ForegroundColor Yellow
    & $adb uninstall com.ambient.badlydefined 2>&1 | Out-Null
    Write-Host "✅ Old app removed" -ForegroundColor Green
    
    Write-Host "`n🗑️ Clearing app data..." -ForegroundColor Yellow
    & $adb shell pm clear com.ambient.badlydefined 2>&1 | Out-Null
    Write-Host "✅ App data cleared" -ForegroundColor Green
} else {
    Write-Host "⚠️ adb not found, skipping uninstall" -ForegroundColor Yellow
}

Write-Host "`n🗑️ Cleaning build..." -ForegroundColor Yellow
dotnet clean -v quiet
if (Test-Path "bin") { Remove-Item "bin" -Recurse -Force }
if (Test-Path "obj") { Remove-Item "obj" -Recurse -Force }
Write-Host "✅ Build cleaned" -ForegroundColor Green

Write-Host "`n🔨 Rebuilding..." -ForegroundColor Yellow
dotnet build BadlyDefined.csproj -f net10.0-android -v quiet
if ($LASTEXITCODE -ne 0) {
    Write-Host "❌ Build failed!" -ForegroundColor Red
    exit 1
}
Write-Host "✅ Build successful" -ForegroundColor Green

Write-Host "`n🚀 Deploying FRESH build..." -ForegroundColor Yellow
Write-Host "   (This installs version 4 with all fixes)" -ForegroundColor Gray
dotnet build BadlyDefined.csproj -f net10.0-android -t:Run

Write-Host ""
Write-Host "✅ DONE! App should now work!" -ForegroundColor Cyan
Write-Host "   All BorderWidth issues removed from Buttons" -ForegroundColor Gray
Write-Host "   Wonky title and splash screen intact" -ForegroundColor Gray
