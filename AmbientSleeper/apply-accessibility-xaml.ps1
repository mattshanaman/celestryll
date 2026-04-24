# ACCESSIBILITY XAML UPDATE - Automated Script
# This script adds accessibility properties to all interactive elements in XAML files
# Preserves all existing functionality, localization, and performance enhancements

Write-Host "????????????????????????????????????????????????????????????????" -ForegroundColor Cyan
Write-Host "?         Accessibility XAML Update - Automated Tool           ?" -ForegroundColor Cyan
Write-Host "????????????????????????????????????????????????????????????????" -ForegroundColor Cyan
Write-Host ""
Write-Host "??  IMPORTANT: This script makes extensive XAML changes." -ForegroundColor Yellow
Write-Host "   It's recommended to:" -ForegroundColor Yellow
Write-Host "   1. Commit current changes to Git" -ForegroundColor White
Write-Host "   2. Close Visual Studio" -ForegroundColor White
Write-Host "   3. Run this script" -ForegroundColor White
Write-Host "   4. Review changes with Git diff" -ForegroundColor White
Write-Host ""

$response = Read-Host "Continue? (yes/no)"
if ($response -ne "yes") {
    Write-Host "Cancelled by user." -ForegroundColor Yellow
    exit 0
}

Write-Host ""
Write-Host "Starting accessibility updates..." -ForegroundColor Green
Write-Host ""

# Define XAML files to update
$xamlFiles = @(
    "Views\PlaybackPage.xaml"
    "Views\LibraryPage.xaml"
    "Views\TimerPage.xaml"
    "Views\EqPage.xaml"
    "Views\SettingsPage.xaml"
    "Views\HelpPage.xaml"
    "Views\LegalPage.xaml"
    "Views\UpgradePage.xaml"
    "Views\PlaybackSettingsPage.xaml"
)

$updated = 0
$failed = 0

foreach ($file in $xamlFiles) {
    if (-not (Test-Path $file)) {
        Write-Host "??  File not found: $file" -ForegroundColor Yellow
        continue
    }
    
    Write-Host "Processing: $file" -ForegroundColor Cyan
    
    $content = Get-Content $file -Raw -Encoding UTF8
    $originalContent = $content
    $changes = 0
    
    # 1. Add accessibility to Play buttons
    $pattern = '(<Button[^>]*Text="\{x:Static resx:AppResources\.Common_PlayButton\}"[^>]*)(/>|>)'
    if ($content -match $pattern) {
        $content = $content -replace $pattern, ('$1' + @"
        SemanticProperties.Description="{x:Static resx:AppResources.A11y_PlayButton}"
        SemanticProperties.Hint="{x:Static resx:AppResources.A11y_PlayButtonHint}"
        MinimumHeightRequest="44"
        MinimumWidthRequest="44"$2
"@)
        $changes++
    }
    
    # 2. Add accessibility to Stop buttons
    $pattern = '(<Button[^>]*Text="\{x:Static resx:AppResources\.Common_StopButton\}"[^>]*)(/>|>)'
    if ($content -match $pattern) {
        $content = $content -replace $pattern, ('$1' + @"
        SemanticProperties.Description="{x:Static resx:AppResources.A11y_StopButton}"
        SemanticProperties.Hint="{x:Static resx:AppResources.A11y_StopButtonHint}"
        MinimumHeightRequest="44"
        MinimumWidthRequest="44"$2
"@)
        $changes++
    }
    
    # 3. Add accessibility to Save buttons
    $pattern = '(<Button[^>]*Text="\{x:Static resx:AppResources\.Common_SaveButton\}"[^>]*)(/>|>)'
    if ($content -match $pattern) {
        $content = $content -replace $pattern, ('$1' + @"
        SemanticProperties.Description="{x:Static resx:AppResources.A11y_SaveMixButton}"
        SemanticProperties.Hint="{x:Static resx:AppResources.A11y_SaveMixButtonHint}"
        MinimumHeightRequest="44"
        MinimumWidthRequest="44"$2
"@)
        $changes++
    }
    
    # 4. Add accessibility to Sliders
    $pattern = '(<Slider[^>]*)(/>|>)'
    if ($content -match $pattern) {
        $content = $content -replace $pattern, ('$1' + @"
        SemanticProperties.Description="{x:Static resx:AppResources.A11y_VolumeSlider}"
        SemanticProperties.Hint="{x:Static resx:AppResources.A11y_VolumeSliderHint}"$2
"@)
        $changes++
    }
    
    # Write if changes were made
    if ($changes -gt 0) {
        $utf8WithBom = New-Object System.Text.UTF8Encoding($true)
        [System.IO.File]::WriteAllText((Resolve-Path $file), $content, $utf8WithBom)
        Write-Host "  ? Applied $changes accessibility updates" -ForegroundColor Green
        $updated++
    }
    else {
        Write-Host "  - No updates needed" -ForegroundColor Gray
    }
}

Write-Host ""
Write-Host "????????????????????????????????????????????????????????????????" -ForegroundColor Cyan
Write-Host "?                        SUMMARY                                ?" -ForegroundColor Cyan
Write-Host "????????????????????????????????????????????????????????????????" -ForegroundColor Cyan
Write-Host ""
Write-Host "Files processed: $($xamlFiles.Count)" -ForegroundColor White
Write-Host "Files updated: $updated" -ForegroundColor Green
Write-Host "Files failed: $failed" -ForegroundColor Red
Write-Host ""

if ($updated -gt 0) {
    Write-Host "? Accessibility properties added!" -ForegroundColor Green
    Write-Host ""
    Write-Host "Next steps:" -ForegroundColor Cyan
    Write-Host "1. Review changes with: git diff" -ForegroundColor White
    Write-Host "2. Rebuild solution in Visual Studio" -ForegroundColor White
    Write-Host "3. Test with screen readers" -ForegroundColor White
    Write-Host ""
}

Write-Host "Note: This script applies basic patterns. Manual review recommended." -ForegroundColor Yellow
Write-Host ""
