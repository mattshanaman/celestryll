# Emergency Rollback - Simplified GamePage.xaml

## If app is still crashing, this creates a simplified version

Write-Host "Creating simplified GamePage backup..." -ForegroundColor Yellow

# Backup current file
Copy-Item "BadlyDefined\Pages\GamePage.xaml" "BadlyDefined\Pages\GamePage.xaml.wonky"

Write-Host "Backup created: GamePage.xaml.wonky" -ForegroundColor Green
Write-Host "`nTo restore wonky version:" -ForegroundColor Cyan
Write-Host "Copy-Item 'BadlyDefined\Pages\GamePage.xaml.wonky' 'BadlyDefined\Pages\GamePage.xaml'" -ForegroundColor White
