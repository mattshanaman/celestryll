Write-Host "🔍 COMPREHENSIVE CRASH DIAGNOSIS" -ForegroundColor Cyan
Write-Host "=" * 70 -ForegroundColor Gray

Write-Host "`n📋 This will:" -ForegroundColor Yellow
Write-Host "   1. Clean everything" -ForegroundColor White
Write-Host "   2. Rebuild" -ForegroundColor White
Write-Host "   3. Deploy to emulator" -ForegroundColor White
Write-Host "   4. Show you where to check for crash logs" -ForegroundColor White
Write-Host ""

# Clean
Write-Host "🗑️ Step 1/4: Cleaning..." -ForegroundColor Yellow
Set-Location BadlyDefined
dotnet clean -v quiet 2>&1 | Out-Null
Write-Host "✅ Cleaned" -ForegroundColor Green

# Build
Write-Host "`n🔨 Step 2/4: Building..." -ForegroundColor Yellow
$buildOutput = dotnet build BadlyDefined.csproj -f net10.0-android 2>&1
if ($LASTEXITCODE -eq 0) {
    Write-Host "✅ Build successful" -ForegroundColor Green
} else {
    Write-Host "❌ Build failed!" -ForegroundColor Red
    $buildOutput | Select-String "error"
    Set-Location ..
    exit 1
}

# Instructions for monitoring
Write-Host "`n📊 Step 3/4: Preparing to deploy and monitor..." -ForegroundColor Yellow
Write-Host ""
Write-Host "⚠️  IMPORTANT: Before I deploy, do this in Visual Studio:" -ForegroundColor Red
Write-Host "   1. Click: View → Output" -ForegroundColor White
Write-Host "   2. In the dropdown, select: Debug" -ForegroundColor White  
Write-Host "   3. Keep that window visible" -ForegroundColor White
Write-Host ""
Write-Host "Press ENTER when ready..." -ForegroundColor Cyan
$null = Read-Host

# Deploy
Write-Host "`n🚀 Step 4/4: Deploying to emulator..." -ForegroundColor Yellow
Write-Host "👀 Watch the Output window (Debug) for messages!" -ForegroundColor Cyan
Write-Host ""

dotnet build BadlyDefined.csproj -f net10.0-android -t:Run

Write-Host ""
Write-Host "=" * 70 -ForegroundColor Gray
Write-Host "🔍 CHECK THE OUTPUT WINDOW NOW!" -ForegroundColor Red
Write-Host ""
Write-Host "Look for messages that start with:" -ForegroundColor Yellow
Write-Host "   🔧 = Starting something" -ForegroundColor White
Write-Host "   ✅ = Success" -ForegroundColor Green
Write-Host "   💥 = CRASH/ERROR (THIS IS WHAT WE NEED!)" -ForegroundColor Red
Write-Host "   ❌ = Failure" -ForegroundColor Red
Write-Host ""
Write-Host "Copy any 💥 or ❌ messages and share them!" -ForegroundColor Cyan

Set-Location ..
