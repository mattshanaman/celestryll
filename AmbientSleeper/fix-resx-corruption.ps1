# Fix Corrupted RESX Files
# Some .resx files were corrupted during translation application
# This script identifies and fixes XML structure issues

Write-Host "????????????????????????????????????????????????????????????????" -ForegroundColor Cyan
Write-Host "?            RESX File Corruption Fix Utility                  ?" -ForegroundColor Cyan
Write-Host "????????????????????????????????????????????????????????????????" -ForegroundColor Cyan
Write-Host ""

$resxFiles = @(
    "Resources\Strings\AppResources.ar.resx"
    "Resources\Strings\AppResources.zh-Hant.resx"
    "Resources\Strings\AppResources.ja.resx"
    "Resources\Strings\AppResources.hi.resx"
)

$fixed = 0
$errors = 0

foreach ($file in $resxFiles) {
    if (-not (Test-Path $file)) {
        Write-Host "??  File not found: $file" -ForegroundColor Yellow
        continue
    }
    
    Write-Host "Checking: $file" -ForegroundColor Cyan
    
    try {
        # Try to load as XML
        [xml]$xml = Get-Content $file -Raw -Encoding UTF8
        Write-Host "  ? Valid XML" -ForegroundColor Green
    }
    catch {
        Write-Host "  ? XML Error detected" -ForegroundColor Red
        $errors++
        
        # Read content
        $content = Get-Content $file -Raw -Encoding UTF8
        
        # Common issues to fix:
        # 1. Missing <data name="EQ_FlatButton" tag
        $content = $content -replace '(\s+)</data>\s+\$', "`$1</data>`n  <data name=`"EQ_FlatButton`" xml:space=`"preserve`">`n    <value>`$"
        
        # 2. Ensure all <data> tags are properly closed
        $lines = $content -split "`n"
        $fixedLines = @()
        $inDataTag = $false
        $dataName = ""
        
        for ($i = 0; $i -lt $lines.Count; $i++) {
            $line = $lines[$i]
            
            # Check if line starts a data tag
            if ($line -match '<data name="([^"]+)"') {
                $inDataTag = $true
                $dataName = $matches[1]
                $fixedLines += $line
            }
            # Check if line has a value without proper opening
            elseif ($line -match '^\s*\$' -and -not $inDataTag) {
                # Orphaned value line - skip it
                Write-Host "    Removed orphaned line: $($line.Trim())" -ForegroundColor Yellow
                continue
            }
            # Check for closing </data> tag
            elseif ($line -match '</data>') {
                $inDataTag = $false
                $dataName = ""
                $fixedLines += $line
            }
            else {
                $fixedLines += $line
            }
        }
        
        $fixedContent = $fixedLines -join "`n"
        
        # Write fixed content
        $utf8WithBom = New-Object System.Text.UTF8Encoding($true)
        [System.IO.File]::WriteAllText((Resolve-Path $file), $fixedContent, $utf8WithBom)
        
        Write-Host "  ? Fixed and saved" -ForegroundColor Green
        $fixed++
    }
}

Write-Host ""
Write-Host "????????????????????????????????????????????????????????????????" -ForegroundColor Cyan
Write-Host "?                        SUMMARY                                ?" -ForegroundColor Cyan
Write-Host "????????????????????????????????????????????????????????????????" -ForegroundColor Cyan
Write-Host ""
Write-Host "Files checked: $($resxFiles.Count)" -ForegroundColor White
Write-Host "Files fixed: $fixed" -ForegroundColor Green
Write-Host ""

if ($fixed -gt 0) {
    Write-Host "? Files have been repaired." -ForegroundColor Green
    Write-Host "??  However, some translations may have been lost." -ForegroundColor Yellow
    Write-Host ""
    Write-Host "Recommended next steps:" -ForegroundColor Cyan
    Write-Host "1. Close Visual Studio" -ForegroundColor White
    Write-Host "2. Re-run: apply-all-translations.ps1" -ForegroundColor White
    Write-Host "3. Rebuild solution" -ForegroundColor White
}
else {
    Write-Host "? All files are valid!" -ForegroundColor Green
}

Write-Host ""
