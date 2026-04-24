Write-Host "📱 Retrieving crash log from Android device..." -ForegroundColor Cyan

# Find adb.exe
$adbPaths = @(
    "C:\Program Files (x86)\Android\android-sdk\platform-tools\adb.exe",
    "$env:LOCALAPPDATA\Android\Sdk\platform-tools\adb.exe",
    "$env:ANDROID_HOME\platform-tools\adb.exe"
)

$adb = $null
foreach ($path in $adbPaths) {
    if (Test-Path $path) {
        $adb = $path
        break
    }
}

if (-not $adb) {
    Write-Host "❌ Could not find adb.exe" -ForegroundColor Red
    Write-Host "Instead, use F5 in Visual Studio to see crash details!" -ForegroundColor Yellow
    exit 1
}

Write-Host "✅ Found adb at: $adb" -ForegroundColor Green

# Get the log file
Write-Host "`nPulling boot log from device..." -ForegroundColor Yellow
& $adb shell "run-as com.ambient.badlydefined cat /data/user/0/com.ambient.badlydefined/files/.local/share/badlydefined_boot.log" 2>$null

if ($LASTEXITCODE -eq 0) {
    Write-Host "`n✅ Boot log retrieved above!" -ForegroundColor Green
} else {
    Write-Host "`n❌ Could not retrieve boot log" -ForegroundColor Red
    Write-Host "The app might have crashed before writing the log" -ForegroundColor Yellow
    Write-Host "`nTry: Press F5 in Visual Studio to debug!" -ForegroundColor Cyan
}
