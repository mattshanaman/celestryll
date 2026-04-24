# Regenerate AppResources.Designer.cs
# This script helps regenerate the Designer.cs file for the AppResources.resx

Write-Host "AppResources.Designer.cs Regeneration Helper" -ForegroundColor Cyan
Write-Host "=============================================" -ForegroundColor Cyan
Write-Host ""

$resxPath = "Resources\Strings\AppResources.resx"
$designerPath = "Resources\Strings\AppResources.Designer.cs"

if (Test-Path $resxPath) {
    Write-Host "? Found AppResources.resx" -ForegroundColor Green
} else {
    Write-Host "? AppResources.resx not found!" -ForegroundColor Red
    Write-Host "  Make sure you're running this from the project root directory." -ForegroundColor Yellow
    exit 1
}

Write-Host ""
Write-Host "To regenerate AppResources.Designer.cs, please do ONE of the following:" -ForegroundColor Yellow
Write-Host ""

Write-Host "OPTION 1: Visual Studio (Recommended)" -ForegroundColor Green
Write-Host "  1. Open the solution in Visual Studio"
Write-Host "  2. In Solution Explorer, find Resources\Strings\AppResources.resx"
Write-Host "  3. Double-click to open it (or right-click ? Open)"
Write-Host "  4. Make a tiny change (add a space, remove it)"
Write-Host "  5. Press Ctrl+S to save"
Write-Host "  6. The Designer.cs file will auto-regenerate"
Write-Host "  7. Build the solution (Ctrl+Shift+B)"
Write-Host ""

Write-Host "OPTION 2: Run Custom Tool in Visual Studio" -ForegroundColor Green
Write-Host "  1. Open the solution in Visual Studio"
Write-Host "  2. In Solution Explorer, right-click AppResources.resx"
Write-Host "  3. Select 'Run Custom Tool'"
Write-Host "  4. The Designer.cs file will regenerate"
Write-Host "  5. Build the solution (Ctrl+Shift+B)"
Write-Host ""

Write-Host "OPTION 3: Clean and Rebuild" -ForegroundColor Green
Write-Host "  1. Close all running instances of the app"
Write-Host "  2. Run this command: dotnet clean"
Write-Host "  3. If that fails, manually delete obj\ and bin\ folders"
Write-Host "  4. Run this command: dotnet build"
Write-Host ""

Write-Host "After regeneration, verify the build succeeds:" -ForegroundColor Cyan
Write-Host "  dotnet build" -ForegroundColor White
Write-Host ""

Write-Host "Press any key to continue..."
$null = $Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")
