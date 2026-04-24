# Pre-Flight Check for Translation Scripts
# Checks if .resx files are locked and provides solutions

Write-Host "????????????????????????????????????????????????????????????????" -ForegroundColor Cyan
Write-Host "?        Translation Pre-Flight Check                          ?" -ForegroundColor Cyan
Write-Host "????????????????????????????????????????????????????????????????" -ForegroundColor Cyan
Write-Host ""

$resxFiles = @(
    "Resources\Strings\AppResources.es.resx"
    "Resources\Strings\AppResources.de.resx"
    "Resources\Strings\AppResources.fr.resx"
    "Resources\Strings\AppResources.ja.resx"
    "Resources\Strings\AppResources.hi.resx"
    "Resources\Strings\AppResources.zh-Hant.resx"
    "Resources\Strings\AppResources.ar.resx"
)

Write-Host "Checking .resx files..." -ForegroundColor Yellow
Write-Host ""

$allClear = $true
$lockedFiles = @()

foreach ($file in $resxFiles) {
    if (Test-Path $file) {
        Write-Host "  Checking $file..." -ForegroundColor Gray
        
        # Check if read-only
        $fileItem = Get-Item $file -Force
        if ($fileItem.IsReadOnly) {
            Write-Host "    ??  Read-only attribute detected" -ForegroundColor Yellow
            $fileItem.IsReadOnly = $false
            Write-Host "    ? Read-only attribute removed" -ForegroundColor Green
        }
        
        # Test if we can write
        try {
            $testContent = Get-Content $file -Raw
            [System.IO.File]::WriteAllText($fileItem.FullName, $testContent, [System.Text.Encoding]::UTF8)
            Write-Host "    ? File is writable" -ForegroundColor Green
        }
        catch {
            Write-Host "    ? FILE IS LOCKED!" -ForegroundColor Red
            Write-Host "       $($_.Exception.Message)" -ForegroundColor Red
            $lockedFiles += $file
            $allClear = $false
        }
    }
    else {
        Write-Host "  ? $file not found!" -ForegroundColor Red
        $allClear = $false
    }
}

Write-Host ""
Write-Host "????????????????????????????????????????????????????????????????" -ForegroundColor Cyan

if ($allClear) {
    Write-Host "?                    ? ALL CLEAR!                            ?" -ForegroundColor Green
    Write-Host "????????????????????????????????????????????????????????????????" -ForegroundColor Cyan
    Write-Host ""
    Write-Host "? All .resx files are writable!" -ForegroundColor Green
    Write-Host ""
    Write-Host "You can now run:" -ForegroundColor Cyan
    Write-Host "  powershell -ExecutionPolicy Bypass -File .\apply-all-translations.ps1" -ForegroundColor White
    Write-Host ""
}
else {
    Write-Host "?                  ??  ISSUES FOUND                           ?" -ForegroundColor Yellow
    Write-Host "????????????????????????????????????????????????????????????????" -ForegroundColor Cyan
    Write-Host ""
    Write-Host "? $($lockedFiles.Count) file(s) are locked:" -ForegroundColor Red
    foreach ($file in $lockedFiles) {
        Write-Host "   • $file" -ForegroundColor Yellow
    }
    Write-Host ""
    Write-Host "?? SOLUTIONS:" -ForegroundColor Cyan
    Write-Host ""
    Write-Host "Option 1: Close Visual Studio Files" -ForegroundColor Yellow
    Write-Host "  1. In Visual Studio, close ALL .resx files" -ForegroundColor Gray
    Write-Host "  2. Close Solution Explorer or collapse Resources/Strings" -ForegroundColor Gray
    Write-Host "  3. Run this check again" -ForegroundColor Gray
    Write-Host ""
    Write-Host "Option 2: Close Visual Studio Completely" -ForegroundColor Yellow
    Write-Host "  1. Save all files" -ForegroundColor Gray
    Write-Host "  2. Close Visual Studio" -ForegroundColor Gray
    Write-Host "  3. Run the translation scripts" -ForegroundColor Gray
    Write-Host "  4. Reopen Visual Studio" -ForegroundColor Gray
    Write-Host ""
    Write-Host "Option 3: Use Process Explorer" -ForegroundColor Yellow
    Write-Host "  1. Download Process Explorer from Microsoft" -ForegroundColor Gray
    Write-Host "  2. Find which process has the file open" -ForegroundColor Gray
    Write-Host "  3. Close that process" -ForegroundColor Gray
    Write-Host ""
}

Write-Host "Press any key to continue..."
$null = $Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")
