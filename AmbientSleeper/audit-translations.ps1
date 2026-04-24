# Scan Translation Files for English Values
# Identifies strings that are still in English and need translation

Write-Host "????????????????????????????????????????????????????????????????" -ForegroundColor Cyan
Write-Host "?      Translation Audit - Find Untranslated English Strings  ?" -ForegroundColor Cyan
Write-Host "????????????????????????????????????????????????????????????????" -ForegroundColor Cyan
Write-Host ""

$languages = @(
    @{Code="es"; Name="Spanish"; Flag="????"}
    @{Code="de"; Name="German"; Flag="????"}
    @{Code="fr"; Name="French"; Flag="????"}
    @{Code="ja"; Name="Japanese"; Flag="????"}
    @{Code="hi"; Name="Hindi"; Flag="????"}
    @{Code="zh-Hant"; Name="Chinese"; Flag="????"}
    @{Code="ar"; Name="Arabic"; Flag="????"}
)

$allUntranslated = @{}

foreach ($lang in $languages) {
    $file = "Resources\Strings\AppResources.$($lang.Code).resx"
    
    if (-not (Test-Path $file)) {
        Write-Host "$($lang.Flag) $($lang.Name): FILE NOT FOUND" -ForegroundColor Red
        continue
    }
    
    Write-Host "$($lang.Flag) $($lang.Name) ($($lang.Code))..." -ForegroundColor Yellow
    
    # Read XML content
    [xml]$xml = Get-Content $file -Encoding UTF8
    
    $untranslated = @()
    
    # Check each data element
    foreach ($data in $xml.root.data) {
        $key = $data.name
        $value = $data.value
        
        # Skip if null or empty
        if ([string]::IsNullOrWhiteSpace($value)) {
            continue
        }
        
        # Check if value looks like English
        # English characteristics: starts with capital letter, contains common English words
        $englishPatterns = @(
            '^(The|A|An|This|That|These|Those|My|Your|Our) '
            '\b(and|or|the|of|to|for|with|from|by|at|in|on)\b'
            '^(Welcome|Getting|Create|Add|Remove|Save|Load|Delete|Import|Export|Play|Stop|Start)'
            '^(Library|Playback|Timer|Settings|Help|Legal|Mix|Playlist|Sound|Audio|Bundle|Alarm)'
            '^(Free|Standard|Premium|Pro\+?|Subscription|Tier|Feature|Mode|Tab)'
            '^(Duration|Stop At|Troubleshooting|Advanced|Integration)'
        )
        
        $isEnglish = $false
        foreach ($pattern in $englishPatterns) {
            if ($value -match $pattern) {
                $isEnglish = $true
                break
            }
        }
        
        if ($isEnglish) {
            $untranslated += [PSCustomObject]@{
                Key = $key
                EnglishValue = $value
            }
        }
    }
    
    if ($untranslated.Count -gt 0) {
        Write-Host "  ? Found $($untranslated.Count) untranslated strings" -ForegroundColor Red
        $allUntranslated[$lang.Code] = $untranslated
        
        # Show first 10
        $untranslated | Select-Object -First 10 | ForEach-Object {
            Write-Host "     • $($_.Key): $($_.EnglishValue)" -ForegroundColor Gray
        }
        
        if ($untranslated.Count > 10) {
            Write-Host "     ... and $($untranslated.Count - 10) more" -ForegroundColor Gray
        }
    }
    else {
        Write-Host "  ? All strings appear to be translated" -ForegroundColor Green
    }
    
    Write-Host ""
}

Write-Host "????????????????????????????????????????????????????????????????" -ForegroundColor Cyan
Write-Host "?                    AUDIT SUMMARY                             ?" -ForegroundColor Cyan
Write-Host "????????????????????????????????????????????????????????????????" -ForegroundColor Cyan
Write-Host ""

$totalUntranslated = ($allUntranslated.Values | ForEach-Object { $_.Count } | Measure-Object -Sum).Sum

if ($totalUntranslated -gt 0) {
    Write-Host "? TRANSLATIONS INCOMPLETE" -ForegroundColor Red
    Write-Host ""
    Write-Host "Total untranslated strings: $totalUntranslated" -ForegroundColor Yellow
    Write-Host ""
    
    foreach ($lang in $languages) {
        if ($allUntranslated.ContainsKey($lang.Code)) {
            $count = $allUntranslated[$lang.Code].Count
            Write-Host "  $($lang.Flag) $($lang.Name): $count strings need translation" -ForegroundColor Yellow
        }
        else {
            Write-Host "  $($lang.Flag) $($lang.Name): ? Complete" -ForegroundColor Green
        }
    }
    
    Write-Host ""
    Write-Host "?? Generating detailed report..." -ForegroundColor Cyan
    
    # Generate detailed report
    $report = @"
# Translation Audit Report - Untranslated Strings Found

## Summary

Total untranslated strings: $totalUntranslated

## Details by Language

"@
    
    foreach ($lang in $languages) {
        if ($allUntranslated.ContainsKey($lang.Code)) {
            $strings = $allUntranslated[$lang.Code]
            $report += @"

### $($lang.Flag) $($lang.Name) ($($lang.Code))

**Untranslated: $($strings.Count) strings**

| Key | English Value |
|-----|---------------|
"@
            foreach ($str in $strings) {
                $report += "`n| ``$($str.Key)`` | $($str.EnglishValue) |"
            }
            
            $report += "`n"
        }
    }
    
    $report += @"

## Next Steps

1. Review the untranslated strings above
2. Update translation scripts with proper translations
3. Run: ``powershell -ExecutionPolicy Bypass -File .\apply-all-translations.ps1``
4. Re-run this audit to verify

## Files to Update

"@
    
    foreach ($lang in $languages) {
        if ($allUntranslated.ContainsKey($lang.Code)) {
            $report += "- ``apply-$($lang.Name.ToLower())-translations.ps1```n"
        }
    }
    
    Set-Content -Path "TRANSLATION_AUDIT_REPORT.md" -Value $report -Encoding UTF8
    Write-Host "? Report saved to: TRANSLATION_AUDIT_REPORT.md" -ForegroundColor Green
}
else {
    Write-Host "? ALL TRANSLATIONS COMPLETE!" -ForegroundColor Green
    Write-Host ""
    Write-Host "All 7 languages have been fully translated." -ForegroundColor Green
}

Write-Host ""
Write-Host "Press any key to continue..."
$null = $Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")
