# Diagnose BadlyDefined Runtime Issue

Write-Host "🔍 BadlyDefined Runtime Diagnostic Script" -ForegroundColor Cyan
Write-Host "=" * 50 -ForegroundColor Gray

# Step 1: Clean build
Write-Host "`n📦 Step 1: Cleaning previous builds..." -ForegroundColor Yellow
Set-Location -Path "BadlyDefined"
dotnet clean BadlyDefined.csproj -v quiet
Write-Host "✅ Clean complete" -ForegroundColor Green

# Step 2: Restore packages
Write-Host "`n📥 Step 2: Restoring packages..." -ForegroundColor Yellow
dotnet restore BadlyDefined.csproj
Write-Host "✅ Restore complete" -ForegroundColor Green

# Step 3: Build for Android
Write-Host "`n🔨 Step 3: Building for Android..." -ForegroundColor Yellow
$buildOutput = dotnet build BadlyDefined.csproj -f net10.0-android -v normal 2>&1
$buildSuccess = $buildOutput | Select-String "Build succeeded"
$buildErrors = $buildOutput | Select-String "error"

if ($buildSuccess) {
    Write-Host "✅ Build succeeded" -ForegroundColor Green
} else {
    Write-Host "❌ Build failed" -ForegroundColor Red
    Write-Host "`nErrors found:" -ForegroundColor Red
    $buildErrors | ForEach-Object { Write-Host $_ -ForegroundColor Red }
    exit 1
}

# Step 4: Check for XAML compilation errors
Write-Host "`n🔍 Step 4: Checking for XAML errors..." -ForegroundColor Yellow
$xamlErrors = $buildOutput | Select-String "XamlC|XAML"
if ($xamlErrors) {
    Write-Host "⚠️ XAML warnings/errors found:" -ForegroundColor Yellow
    $xamlErrors | ForEach-Object { Write-Host $_ -ForegroundColor Yellow }
} else {
    Write-Host "✅ No XAML compilation issues" -ForegroundColor Green
}

# Step 5: List generated files
Write-Host "`n📂 Step 5: Checking generated files..." -ForegroundColor Yellow
$apkFiles = Get-ChildItem -Path "bin\Debug\net10.0-android\*.apk" -ErrorAction SilentlyContinue
if ($apkFiles) {
    Write-Host "✅ APK generated: $($apkFiles.Name)" -ForegroundColor Green
    Write-Host "   Size: $([math]::Round($apkFiles.Length/1MB, 2)) MB" -ForegroundColor Gray
} else {
    Write-Host "⚠️ No APK found" -ForegroundColor Yellow
}

# Step 6: Check for common issues
Write-Host "`n🔍 Step 6: Checking for common issues..." -ForegroundColor Yellow

# Check if converters are registered
$appXaml = Get-Content "App.xaml" -Raw
if ($appXaml -match "StringIsNotNullOrEmptyConverter") {
    Write-Host "✅ StringIsNotNullOrEmptyConverter registered" -ForegroundColor Green
} else {
    Write-Host "❌ StringIsNotNullOrEmptyConverter NOT registered" -ForegroundColor Red
}

if ($appXaml -match "InvertedBoolConverter") {
    Write-Host "✅ InvertedBoolConverter registered" -ForegroundColor Green
} else {
    Write-Host "❌ InvertedBoolConverter NOT registered" -ForegroundColor Red
}

# Step 7: Recommendations
Write-Host "`n💡 Recommendations:" -ForegroundColor Cyan
Write-Host "1. Deploy to emulator: dotnet build -f net10.0-android -t:Run" -ForegroundColor White
Write-Host "2. Check Visual Studio Output window (View → Output → Build)" -ForegroundColor White
Write-Host "3. Monitor Android logcat: adb logcat | Select-String 'BadlyDefined'" -ForegroundColor White
Write-Host "4. Try on physical device if emulator issues persist" -ForegroundColor White

Write-Host "`n✅ Diagnostic complete!" -ForegroundColor Green
Set-Location -Path ".."
