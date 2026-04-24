Write-Host "🚀 GOOGLE PLAY RELEASE BUILD SCRIPT" -ForegroundColor Cyan
Write-Host "=" * 70 -ForegroundColor Gray
Write-Host ""
Write-Host "✅ Using R8 code shrinker (modern ProGuard replacement)" -ForegroundColor Green
Write-Host ""

# Verify we're in the right location
if (-not (Test-Path "BadlyDefined\BadlyDefined.csproj")) {
    Write-Host "❌ ERROR: Must run from solution root directory" -ForegroundColor Red
    exit 1
}

Write-Host "📋 Pre-Flight Checks..." -ForegroundColor Yellow
Write-Host ""

# Check required files
$requiredFiles = @(
    "BadlyDefined\Resources\AppIcon\appicon.svg",
    "BadlyDefined\Resources\AppIcon\appiconfg.svg",
    "BadlyDefined\Resources\Splash\splash.svg",
    "BadlyDefined\Platforms\Android\AndroidManifest.xml",
    "PRIVACY_POLICY.md"
)

$allFilesExist = $true
foreach ($file in $requiredFiles) {
    if (Test-Path $file) {
        Write-Host "  ✅ $file" -ForegroundColor Green
    } else {
        Write-Host "  ❌ MISSING: $file" -ForegroundColor Red
        $allFilesExist = $false
    }
}

if (-not $allFilesExist) {
    Write-Host "`n❌ Missing required files. Cannot proceed." -ForegroundColor Red
    exit 1
}

Write-Host "`n✅ All required files present" -ForegroundColor Green
Write-Host ""

# Clean previous builds
Write-Host "🧹 Cleaning previous builds..." -ForegroundColor Yellow
Set-Location BadlyDefined
dotnet clean -c Release -v quiet
if (Test-Path "bin\Release") {
    Remove-Item "bin\Release" -Recurse -Force -ErrorAction SilentlyContinue
}
if (Test-Path "obj\Release") {
    Remove-Item "obj\Release" -Recurse -Force -ErrorAction SilentlyContinue
}
Write-Host "✅ Clean complete" -ForegroundColor Green
Write-Host ""

# Restore packages
Write-Host "📦 Restoring packages..." -ForegroundColor Yellow
dotnet restore -v quiet
Write-Host "✅ Packages restored" -ForegroundColor Green
Write-Host ""

# Build release AAB
Write-Host "🔨 Building RELEASE AAB for Google Play..." -ForegroundColor Yellow
Write-Host "   This may take 2-5 minutes..." -ForegroundColor Gray
Write-Host ""

$buildOutput = dotnet publish BadlyDefined.csproj -f net10.0-android -c Release -p:AndroidPackageFormat=aab 2>&1

if ($LASTEXITCODE -eq 0) {
    Write-Host "✅ BUILD SUCCESSFUL!" -ForegroundColor Green
    Write-Host ""
    
    # Find the AAB file
    $aabPath = Get-ChildItem -Path "bin\Release\net10.0-android\publish" -Filter "*.aab" -Recurse | Select-Object -First 1
    
    if ($aabPath) {
        Write-Host "📦 RELEASE AAB CREATED:" -ForegroundColor Cyan
        Write-Host "   Location: $($aabPath.FullName)" -ForegroundColor White
        Write-Host "   Size: $([math]::Round($aabPath.Length / 1MB, 2)) MB" -ForegroundColor White
        Write-Host ""
        
        # Copy to convenient location
        $outputName = "BadlyDefined-v8-release.aab"
        Copy-Item $aabPath.FullName "..\$outputName" -Force
        Write-Host "✅ Copied to: ..\$outputName" -ForegroundColor Green
        Write-Host ""
    } else {
        Write-Host "⚠️ AAB file not found in expected location" -ForegroundColor Yellow
    }
    
    Write-Host "=" * 70 -ForegroundColor Gray
    Write-Host "🎉 RELEASE BUILD COMPLETE!" -ForegroundColor Cyan
    Write-Host ""
    Write-Host "📋 NEXT STEPS:" -ForegroundColor Yellow
    Write-Host "  1. Test the AAB on a device" -ForegroundColor White
    Write-Host "  2. Upload to Google Play Console (Internal Test track)" -ForegroundColor White
    Write-Host "  3. Add privacy policy URL in Play Console" -ForegroundColor White
    Write-Host "  4. Upload screenshots (minimum 2)" -ForegroundColor White
    Write-Host "  5. Complete Data Safety form" -ForegroundColor White
    Write-Host "  6. Complete Content Rating" -ForegroundColor White
    Write-Host "  7. Invite test users" -ForegroundColor White
    Write-Host ""
    Write-Host "🔗 Play Console: https://play.google.com/console" -ForegroundColor Cyan
    
} else {
    Write-Host "❌ BUILD FAILED!" -ForegroundColor Red
    Write-Host ""
    Write-Host "Error output:" -ForegroundColor Yellow
    Write-Host $buildOutput
    Set-Location ..
    exit 1
}

Set-Location ..
Write-Host ""
Write-Host "✅ Script complete!" -ForegroundColor Green
