# ACCURATE Translation Audit - Finds ONLY English Strings
# Properly distinguishes between English and translated text

Write-Host "????????????????????????????????????????????????????????????????" -ForegroundColor Cyan
Write-Host "?      ACCURATE Translation Audit - English Detection         ?" -ForegroundColor Cyan
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

# Load English strings as baseline
$englishFile = "Resources\Strings\AppResources.resx"
if (-not (Test-Path $englishFile)) {
    Write-Host "ERROR: English file not found: $englishFile" -ForegroundColor Red
    exit 1
}

Write-Host "Loading English baseline..." -ForegroundColor Yellow
[xml]$englishXml = Get-Content $englishFile -Encoding UTF8

$englishStrings = @{}
foreach ($data in $englishXml.root.data) {
    if ($data.name -and $data.value) {
        $englishStrings[$data.name] = $data.value
    }
}

Write-Host "Loaded $($englishStrings.Count) English strings" -ForegroundColor Green
Write-Host ""

$allUntranslated = @{}
$grandTotal = 0

foreach ($lang in $languages) {
    $file = "Resources\Strings\AppResources.$($lang.Code).resx"
    
    if (-not (Test-Path $file)) {
        Write-Host "$($lang.Flag) $($lang.Name): FILE NOT FOUND" -ForegroundColor Red
        continue
    }
    
    Write-Host "$($lang.Flag) $($lang.Name) ($($lang.Code))..." -ForegroundColor Yellow
    
    [xml]$xml = Get-Content $file -Encoding UTF8
    
    $untranslated = @()
    
    foreach ($key in $englishStrings.Keys) {
        $englishValue = $englishStrings[$key]
        
        # Find matching element in translated file
        $translatedData = $xml.root.data | Where-Object { $_.name -eq $key } | Select-Object -First 1
        
        if ($translatedData) {
            $translatedValue = $translatedData.value
            
            # Check if translation is EXACTLY the same as English
            if ($translatedValue -eq $englishValue) {
                $untranslated += [PSCustomObject]@{
                    Key = $key
                    EnglishValue = $englishValue
                }
            }
        }
        else {
            # Key doesn't exist in translated file
            $untranslated += [PSCustomObject]@{
                Key = $key
                EnglishValue = $englishValue
            }
        }
    }
    
    if ($untranslated.Count -gt 0) {
        Write-Host "  ? Found $($untranslated.Count) ACTUALLY untranslated strings" -ForegroundColor Red
        $allUntranslated[$lang.Code] = $untranslated
        $grandTotal += $untranslated.Count
        
        # Show first 10
        $untranslated | Select-Object -First 10 | ForEach-Object {
            $displayValue = if ($_.EnglishValue.Length -gt 60) {
                $_.EnglishValue.Substring(0, 60) + "..."
            } else {
                $_.EnglishValue
            }
            Write-Host "     • $($_.Key): $displayValue" -ForegroundColor Gray
        }
        
        if ($untranslated.Count > 10) {
            Write-Host "     ... and $($untranslated.Count - 10) more" -ForegroundColor Gray
        }
    }
    else {
        Write-Host "  ? All strings are translated!" -ForegroundColor Green
    }
    
    Write-Host ""
}

Write-Host "????????????????????????????????????????????????????????????????" -ForegroundColor Cyan
Write-Host "?                    ACCURATE AUDIT SUMMARY                    ?" -ForegroundColor Cyan
Write-Host "????????????????????????????????????????????????????????????????" -ForegroundColor Cyan
Write-Host ""

if ($grandTotal -gt 0) {
    Write-Host "? TRANSLATIONS INCOMPLETE" -ForegroundColor Red
    Write-Host ""
    Write-Host "Total ACTUALLY untranslated: $grandTotal" -ForegroundColor Yellow
    Write-Host ""
    
    foreach ($lang in $languages) {
        if ($allUntranslated.ContainsKey($lang.Code)) {
            $count = $allUntranslated[$lang.Code].Count
            Write-Host "  $($lang.Flag) $($lang.Name): $count strings" -ForegroundColor Yellow
        }
        else {
            Write-Host "  $($lang.Flag) $($lang.Name): ? Complete" -ForegroundColor Green
        }
    }
    
    Write-Host ""
    Write-Host "?? Breakdown by Category:" -ForegroundColor Cyan
    
    # Analyze by category
    $helpCount = 0
    $legalCount = 0
    $uiCount = 0
    
    if ($allUntranslated.Count -gt 0) {
        $sampleLang = $allUntranslated.Values | Select-Object -First 1
        foreach ($item in $sampleLang) {
            if ($item.Key -like "Help_*") { $helpCount++ }
            elseif ($item.Key -like "Legal_*") { $legalCount++ }
            else { $uiCount++ }
        }
    }
    
    Write-Host "  ?? Help Page: $helpCount strings" -ForegroundColor $(if ($helpCount -gt 0) { "Yellow" } else { "Green" })
    Write-Host "  ??  Legal Page: $legalCount strings" -ForegroundColor $(if ($legalCount -gt 0) { "Yellow" } else { "Green" })
    Write-Host "  ?? UI Strings: $uiCount strings" -ForegroundColor $(if ($uiCount -gt 0) { "Red" } else { "Green" })
    
    Write-Host ""
    Write-Host "?? RECOMMENDATION:" -ForegroundColor Cyan
    
    if ($uiCount -gt 0) {
        Write-Host "  ? UI STRINGS NEED IMMEDIATE TRANSLATION" -ForegroundColor Red
        Write-Host "     These affect core user experience!" -ForegroundColor Red
        Write-Host ""
    }
    
    if ($legalCount -gt 0) {
        Write-Host "  ??  Legal Page: Use certified legal translator" -ForegroundColor Yellow
        Write-Host "     Cost: ~$50-100 per language" -ForegroundColor Gray
        Write-Host "     English is safer until professionally translated" -ForegroundColor Gray
        Write-Host ""
    }
    
    if ($helpCount -gt 0) {
        Write-Host "  ??  Help Page: Professional translation recommended" -ForegroundColor Yellow
        Write-Host "     Cost: ~$25-50 per language" -ForegroundColor Gray
        Write-Host "     English is acceptable for now" -ForegroundColor Gray
        Write-Host ""
    }
    
    # Generate detailed report
    Write-Host "?? Generating detailed report..." -ForegroundColor Cyan
    
    $report = @"
# ACCURATE Translation Audit Report

## Executive Summary

**Total untranslated strings:** $grandTotal  
**Languages affected:** $($allUntranslated.Count)

### Breakdown by Category:
- ?? **Help Page:** $helpCount strings
- ??  **Legal Page:** $legalCount strings  
- ?? **UI Strings:** $uiCount strings

---

## Details by Language

"@
    
    foreach ($lang in $languages) {
        if ($allUntranslated.ContainsKey($lang.Code)) {
            $strings = $allUntranslated[$lang.Code]
            
            # Categorize
            $helpStrings = $strings | Where-Object { $_.Key -like "Help_*" }
            $legalStrings = $strings | Where-Object { $_.Key -like "Legal_*" }
            $uiStrings = $strings | Where-Object { $_.Key -notlike "Help_*" -and $_.Key -notlike "Legal_*" }
            
            $report += @"

### $($lang.Flag) $($lang.Name) ($($lang.Code))

**Total untranslated: $($strings.Count)**
- Help: $($helpStrings.Count)
- Legal: $($legalStrings.Count)
- UI: $($uiStrings.Count)

"@
            
            if ($uiStrings.Count -gt 0) {
                $report += @"

#### ? UI Strings (PRIORITY)

| Key | English Value |
|-----|---------------|
"@
                foreach ($str in $uiStrings) {
                    $report += "`n| ``$($str.Key)`` | $($str.EnglishValue) |"
                }
                $report += "`n"
            }
            
            if ($helpStrings.Count -gt 0) {
                $report += @"

#### ?? Help Page Strings

Count: $($helpStrings.Count) (First 5 shown)

| Key | English Value |
|-----|---------------|
"@
                foreach ($str in ($helpStrings | Select-Object -First 5)) {
                    $truncated = if ($str.EnglishValue.Length -gt 80) {
                        $str.EnglishValue.Substring(0, 80) + "..."
                    } else {
                        $str.EnglishValue
                    }
                    $report += "`n| ``$($str.Key)`` | $truncated |"
                }
                $report += "`n`n*($($helpStrings.Count - 5) more Help strings omitted)*`n"
            }
            
            if ($legalStrings.Count -gt 0) {
                $report += @"

#### ?? Legal Page Strings

Count: $($legalStrings.Count) (First 5 shown)

| Key | English Value |
|-----|---------------|
"@
                foreach ($str in ($legalStrings | Select-Object -First 5)) {
                    $truncated = if ($str.EnglishValue.Length -gt 80) {
                        $str.EnglishValue.Substring(0, 80) + "..."
                    } else {
                        $str.EnglishValue
                    }
                    $report += "`n| ``$($str.Key)`` | $truncated |"
                }
                $report += "`n`n*($($legalStrings.Count - 5) more Legal strings omitted)*`n"
            }
        }
    }
    
    $report += @"

---

## Recommendations

### 1. UI Strings (If Any) - IMMEDIATE ACTION
$(if ($uiCount -gt 0) { "? **$uiCount UI strings need translation NOW**" } else { "? All UI strings are translated" })

### 2. Legal Page - Professional Translation Required
- **Count:** $legalCount strings per language
- **Priority:** Before production release
- **Method:** Certified legal translator ONLY
- **Cost:** ~`$50-100 per language
- **Timeline:** 1-2 weeks per language

### 3. Help Page - Professional Translation Recommended  
- **Count:** $helpCount strings per language
- **Priority:** Can launch with English
- **Method:** Professional translator
- **Cost:** ~`$25-50 per language
- **Timeline:** 1 week per language

---

## Next Steps

1. **If UI strings are untranslated:** Update translation scripts immediately
2. **For Legal page:** Contact certified legal translator
3. **For Help page:** Consider professional service or launch with English

---

**Report Generated:** $(Get-Date -Format "yyyy-MM-dd HH:mm:ss")
"@
    
    Set-Content -Path "ACCURATE_TRANSLATION_AUDIT.md" -Value $report -Encoding UTF8
    Write-Host "? Report saved to: ACCURATE_TRANSLATION_AUDIT.md" -ForegroundColor Green
}
else {
    Write-Host "? PERFECT! ALL TRANSLATIONS COMPLETE!" -ForegroundColor Green
    Write-Host ""
    Write-Host "Every string in all 7 languages is fully translated." -ForegroundColor Green
}

Write-Host ""
Write-Host "Press any key to continue..."
$null = $Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")
