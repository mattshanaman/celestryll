# Clean BadlyDefined App from Emulator and Rebuild Fresh

Write-Host "🧹 Cleaning BadlyDefined from Emulator..." -ForegroundColor Cyan

# Step 1: Uninstall app from emulator
Write-Host "`n📱 Step 1: Uninstalling old app from emulator..." -ForegroundColor Yellow
adb uninstall com.ambient.badlydefined
if ($LASTEXITCODE -eq 0) {
    Write-Host "✅ App uninstalled successfully" -ForegroundColor Green
} else {
    Write-Host "⚠️ App not found or already uninstalled" -ForegroundColor Gray
}

# Step 2: Clean local build artifacts
Write-Host "`n🗑️ Step 2: Cleaning local build artifacts..." -ForegroundColor Yellow
Set-Location -Path "BadlyDefined"
dotnet clean BadlyDefined.csproj
Remove-Item -Path "bin" -Recurse -Force -ErrorAction SilentlyContinue
Remove-Item -Path "obj" -Recurse -Force -ErrorAction SilentlyContinue
Write-Host "✅ Build artifacts cleaned" -ForegroundColor Green

# Step 3: Clear emulator cache (optional but recommended)
Write-Host "`n💾 Step 3: Clearing emulator app cache..." -ForegroundColor Yellow
adb shell pm clear com.ambient.badlydefined 2>$null
Write-Host "✅ Cache cleared (if app was present)" -ForegroundColor Green

# Step 4: Rebuild
Write-Host "`n🔨 Step 4: Building fresh..." -ForegroundColor Yellow
dotnet build BadlyDefined.csproj -f net10.0-android -c Debug
if ($LASTEXITCODE -eq 0) {
    Write-Host "✅ Build successful" -ForegroundColor Green
} else {
    Write-Host "❌ Build failed" -ForegroundColor Red
    Set-Location -Path ".."
    exit 1
}

# Step 5: Deploy and run
Write-Host "`n🚀 Step 5: Deploying fresh build to emulator..." -ForegroundColor Yellow
dotnet build BadlyDefined.csproj -f net10.0-android -t:Run
if ($LASTEXITCODE -eq 0) {
    Write-Host "✅ Deployment successful!" -ForegroundColor Green
} else {
    Write-Host "❌ Deployment failed" -ForegroundColor Red
}

Set-Location -Path ".."

Write-Host "`n🎉 Done! App should start fresh." -ForegroundColor Cyan
Write-Host "If it still crashes, the emulator itself may need to be restarted." -ForegroundColor Yellow
