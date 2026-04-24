Write-Host "🧹 CLEAN DEPLOYMENT FOR BADLYDEFINED" -ForegroundColor Cyan
Write-Host "=" * 60 -ForegroundColor Gray

# Step 1: Uninstall from emulator
Write-Host "`n📱 Uninstalling old app..." -ForegroundColor Yellow
adb uninstall com.ambient.badlydefined 2>&1 | Out-Null
Write-Host "✅ Old app removed" -ForegroundColor Green

# Step 2: Clean build
Write-Host "`n🗑️ Cleaning build artifacts..." -ForegroundColor Yellow
Set-Location BadlyDefined
dotnet clean -v quiet
if (Test-Path "bin") { Remove-Item "bin" -Recurse -Force }
if (Test-Path "obj") { Remove-Item "obj" -Recurse -Force }
Write-Host "✅ Build artifacts cleaned" -ForegroundColor Green

# Step 3: Rebuild
Write-Host "`n🔨 Rebuilding..." -ForegroundColor Yellow
dotnet build BadlyDefined.csproj -f net10.0-android -v quiet
if ($LASTEXITCODE -eq 0) {
    Write-Host "✅ Build successful" -ForegroundColor Green
} else {
    Write-Host "❌ Build failed" -ForegroundColor Red
    Set-Location ..
    exit 1
}

# Step 4: Deploy
Write-Host "`n🚀 Deploying to emulator..." -ForegroundColor Yellow
Write-Host "   Watch for debug messages in Output window!" -ForegroundColor Gray
dotnet build BadlyDefined.csproj -f net10.0-android -t:Run

Set-Location ..
Write-Host "`n✅ Done! Check Output window for debug messages." -ForegroundColor Cyan
