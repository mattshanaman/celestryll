Write-Host "🔄 CONVERTING ALL FRAME ELEMENTS TO BORDER (.NET 10)" -ForegroundColor Cyan
Write-Host "=" * 70 -ForegroundColor Gray
Write-Host ""

$files = @(
    "BadlyDefined\Pages\GamePage.xaml",
    "BadlyDefined\Pages\ProfilePage.xaml",
    "BadlyDefined\Pages\TestModePage.xaml"
)

$totalFrames = 0
$totalConverted = 0

foreach ($file in $files) {
    if (Test-Path $file) {
        Write-Host "📄 Processing: $file" -ForegroundColor Yellow
        
        $content = Get-Content $file -Raw
        $frameCount = ([regex]::Matches($content, "<Frame\s")).Count
        $totalFrames += $frameCount
        
        Write-Host "   Found: $frameCount Frame elements" -ForegroundColor White
        
        # Replace Frame tags
        $content = $content -replace '<Frame\s', '<Border '
        $content = $content -replace '</Frame>', '</Border>'
        
        # Replace Frame-specific properties with Border equivalents
        # BorderColor → Stroke
        $content = $content -replace '\sBorderColor=', ' Stroke='
        
        # BorderWidth → StrokeThickness
        $content = $content -replace '\sBorderWidth=', ' StrokeThickness='
        
        # CornerRadius="X" → StrokeShape="RoundRectangle X"
        $content = $content -replace '\sCornerRadius="(\d+)"', ' StrokeShape="RoundRectangle $1"'
        
        # Remove HasShadow (not supported in Border)
        $content = $content -replace '\s+HasShadow="[^"]*"', ''
        
        # Clean up any double spaces
        $content = $content -replace '\s{2,}', ' '
        
        # Clean up spaces before closing >
        $content = $content -replace '\s+>', '>'
        
        # Save the file
        Set-Content $file $content -NoNewline
        
        $convertedCount = $frameCount
        $totalConverted += $convertedCount
        
        Write-Host "   ✅ Converted: $convertedCount Frame → Border" -ForegroundColor Green
        Write-Host ""
    }
    else {
        Write-Host "   ⚠️ File not found: $file" -ForegroundColor Red
        Write-Host ""
    }
}

Write-Host "=" * 70 -ForegroundColor Gray
Write-Host "📊 CONVERSION SUMMARY" -ForegroundColor Cyan
Write-Host "   Total Frame elements found: $totalFrames" -ForegroundColor White
Write-Host "   Total converted to Border: $totalConverted" -ForegroundColor Green
Write-Host ""
Write-Host "✅ Conversion Complete!" -ForegroundColor Green
Write-Host ""
Write-Host "🔧 Next Steps:" -ForegroundColor Yellow
Write-Host "   1. Review the changes in each file" -ForegroundColor White
Write-Host "   2. Run: dotnet build" -ForegroundColor White
Write-Host "   3. Test each page visually" -ForegroundColor White
Write-Host "   4. Deploy to emulator" -ForegroundColor White
Write-Host ""

# Build to verify
Write-Host "🔨 Building to verify..." -ForegroundColor Yellow
Set-Location BadlyDefined
$buildResult = dotnet build BadlyDefined.csproj -f net10.0-android -v quiet 2>&1
if ($LASTEXITCODE -eq 0) {
    Write-Host "✅ Build SUCCESSFUL - No XAML errors!" -ForegroundColor Green
}
else {
    Write-Host "❌ Build FAILED - Check errors above" -ForegroundColor Red
    Write-Host $buildResult
}
Set-Location ..
