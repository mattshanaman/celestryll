# Convert all Frame elements to Border for .NET 10 compatibility
$xamlFile = "BadlyDefined\Pages\GamePage.xaml"
$content = Get-Content $xamlFile -Raw

Write-Host "🔄 Converting Frame to Border for .NET 10..." -ForegroundColor Cyan

# Replace Frame opening tags
$content = $content -replace '<Frame\s+', '<Border '
$content = $content -replace '</Frame>', '</Border>'

# Replace Frame-specific properties with Border equivalents
$content = $content -replace 'BorderColor="', 'Stroke="'
$content = $content -replace 'BorderWidth="([^"]+)"', 'StrokeThickness="$1"'
$content = $content -replace 'CornerRadius="(\d+)"', 'StrokeShape="RoundRectangle $1"'
$content = $content -replace 'HasShadow="[^"]*"', ''  # Remove HasShadow
$content = $content -replace '\s+>', '>'  # Clean up extra spaces

Set-Content $xamlFile $content

Write-Host "✅ Converted all Frame elements to Border" -ForegroundColor Green
Write-Host "   - Changed <Frame> to <Border>" -ForegroundColor White
Write-Host "   - Changed BorderColor to Stroke" -ForegroundColor White
Write-Host "   - Changed BorderWidth to StrokeThickness" -ForegroundColor White
Write-Host "   - Changed CornerRadius to StrokeShape" -ForegroundColor White
Write-Host "   - Removed HasShadow (not supported in Border)" -ForegroundColor White
