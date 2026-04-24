Write-Host "💥 NUCLEAR CLEAN - FORCE COMPLETE REBUILD" -ForegroundColor Red
Write-Host "This will delete EVERYTHING and rebuild from scratch" -ForegroundColor Yellow
Write-Host ""

$projectPath = "BadlyDefined"

# Clean dotnet
Write-Host "🗑️ Running dotnet clean..." -ForegroundColor Yellow
Set-Location $projectPath
dotnet clean -v quiet
Write-Host "✅ Dotnet clean complete" -ForegroundColor Green

# Force delete bin and obj everywhere
Write-Host "`n🗑️ Force deleting bin/obj folders..." -ForegroundColor Yellow
Get-ChildItem -Path . -Include bin,obj -Recurse -Directory | Remove-Item -Recurse -Force -ErrorAction SilentlyContinue
Write-Host "✅ All bin/obj deleted" -ForegroundColor Green

# Delete intermediate output
Write-Host "`n🗑️ Deleting intermediate files..." -ForegroundColor Yellow
Remove-Item "*.g.cs" -Recurse -Force -ErrorAction SilentlyContinue
Remove-Item "*.g.i.cs" -Recurse -Force -ErrorAction SilentlyContinue
Write-Host "✅ Intermediate files deleted" -ForegroundColor Green

# Restore packages
Write-Host "`n📦 Restoring packages..." -ForegroundColor Yellow
dotnet restore -v quiet
Write-Host "✅ Packages restored" -ForegroundColor Green

# Rebuild
Write-Host "`n🔨 Rebuilding (this takes a moment)..." -ForegroundColor Yellow
dotnet build BadlyDefined.csproj -f net10.0-android -c Debug -v minimal
if ($LASTEXITCODE -ne 0) {
    Write-Host "❌ Build FAILED!" -ForegroundColor Red
    Set-Location ..
    exit 1
}
Write-Host "✅ Build successful" -ForegroundColor Green

# Deploy
Write-Host "`n🚀 Deploying VERSION 5..." -ForegroundColor Yellow
Write-Host "   This is a FRESH build with NO BorderWidth on Buttons" -ForegroundColor Gray
dotnet build BadlyDefined.csproj -f net10.0-android -t:Run

Set-Location ..
Write-Host ""
Write-Host "✅ DONE! If this still fails, the GamePage.xaml file itself has been reverted." -ForegroundColor Cyan
Write-Host "   Check that GamePage.xaml Buttons have NO BorderWidth property." -ForegroundColor Yellow
