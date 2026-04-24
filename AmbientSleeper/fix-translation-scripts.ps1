# Fix File Writing Issues in All Translation Scripts
# This script patches all translation scripts to handle file locking properly

Write-Host "????????????????????????????????????????????????????????????????" -ForegroundColor Cyan
Write-Host "?     Translation Script Patcher - Fix File Access Issues     ?" -ForegroundColor Cyan
Write-Host "????????????????????????????????????????????????????????????????" -ForegroundColor Cyan
Write-Host ""

$scripts = @(
    "apply-spanish-translations.ps1"
    "apply-german-translations.ps1"
    "apply-french-translations.ps1"
    "apply-japanese-translations.ps1"
    "apply-hindi-translations.ps1"
    "apply-chinese-translations.ps1"
    "apply-arabic-translations.ps1"
)

$fixedCount = 0

foreach ($scriptName in $scripts) {
    if (Test-Path $scriptName) {
        Write-Host "Fixing $scriptName..." -ForegroundColor Yellow
        
        $content = Get-Content $scriptName -Raw
        
        # Fix 1: Replace Resolve-Path with absolute path conversion
        $oldPattern = '\[System\.IO\.File\]::WriteAllText\(\(Resolve-Path \$resxFile\), \$content, \$utf8WithBom\)'
        $newPattern = '[System.IO.File]::WriteAllText((Get-Item $resxFile).FullName, $content, $utf8WithBom)'
        
        if ($content -match [regex]::Escape($oldPattern)) {
            $content = $content -replace [regex]::Escape($oldPattern), $newPattern
            Write-Host "  ? Fixed WriteAllText path resolution" -ForegroundColor Green
        }
        
        # Fix 2: Add file unlock section before writing
        $writeSection = 'Write-Host "Writing changes to file..." -ForegroundColor Yellow'
        if ($content -match [regex]::Escape($writeSection)) {
            $newWriteSection = @'
Write-Host "Writing changes to file..." -ForegroundColor Yellow

# Ensure file is not read-only
$fileItem = Get-Item $resxFile -Force
if ($fileItem.IsReadOnly) {
    $fileItem.IsReadOnly = $false
    Write-Host "  Removed read-only attribute" -ForegroundColor Cyan
}

# Write with retry logic
$maxRetries = 3
$retryCount = 0
$written = $false

while (-not $written -and $retryCount -lt $maxRetries) {
    try {
        $utf8WithBom = New-Object System.Text.UTF8Encoding($true)
        [System.IO.File]::WriteAllText((Get-Item $resxFile).FullName, $content, $utf8WithBom)
        $written = $true
    }
    catch {
        $retryCount++
        if ($retryCount -lt $maxRetries) {
            Write-Host "  Retry $retryCount of $maxRetries..." -ForegroundColor Yellow
            Start-Sleep -Milliseconds 500
        }
        else {
            Write-Host "  ERROR: Failed after $maxRetries attempts" -ForegroundColor Red
            Write-Host "  $($_.Exception.Message)" -ForegroundColor Red
            Write-Host ""
            Write-Host "  SOLUTION: Close the .resx file in Visual Studio and try again." -ForegroundColor Yellow
            throw
        }
    }
}
'@
            
            $content = $content -replace [regex]::Escape($writeSection), $newWriteSection
            
            # Remove duplicate WriteAllText and UTF8 encoding lines
            $content = $content -replace '\$utf8WithBom = New-Object System\.Text\.UTF8Encoding\(\$true\)\s+\[System\.IO\.File\]::WriteAllText\(\(Get-Item \$resxFile\)\.FullName, \$content, \$utf8WithBom\)', ''
            
            Write-Host "  ? Added retry logic and file unlock" -ForegroundColor Green
        }
        
        # Write the fixed script
        Set-Content -Path $scriptName -Value $content -Encoding UTF8
        $fixedCount++
        Write-Host "  ? $scriptName patched successfully" -ForegroundColor Green
        Write-Host ""
    }
    else {
        Write-Host "  ? $scriptName not found" -ForegroundColor Red
        Write-Host ""
    }
}

Write-Host "????????????????????????????????????????????????????????????????" -ForegroundColor Green
Write-Host "?                  PATCHING COMPLETE                           ?" -ForegroundColor Green
Write-Host "????????????????????????????????????????????????????????????????" -ForegroundColor Green
Write-Host ""
Write-Host "Fixed $fixedCount of $($scripts.Count) scripts" -ForegroundColor White
Write-Host ""
Write-Host "?? Changes Applied:" -ForegroundColor Cyan
Write-Host "  • Fixed path resolution (Resolve-Path ? Get-Item)" -ForegroundColor Gray
Write-Host "  • Added read-only attribute removal" -ForegroundColor Gray
Write-Host "  • Added 3-attempt retry logic with 500ms delays" -ForegroundColor Gray
Write-Host "  • Better error messages with solutions" -ForegroundColor Gray
Write-Host ""
Write-Host "?? Before Running Translation Scripts:" -ForegroundColor Yellow
Write-Host "  1. Close ALL .resx files in Visual Studio" -ForegroundColor Gray
Write-Host "  2. Close Solution Explorer if it shows .resx expanded" -ForegroundColor Gray
Write-Host "  3. Run: powershell -ExecutionPolicy Bypass -File .\apply-all-translations.ps1" -ForegroundColor Gray
Write-Host ""
Write-Host "? Scripts are now more resilient to file locking!" -ForegroundColor Green
Write-Host ""
