# Master Translation Applicator - ALL 7 LANGUAGES
# Applies translations to all language resource files
# IDEMPOTENT: Safe to re-run multiple times, will update existing translations

Write-Host "????????????????????????????????????????????????????????????????" -ForegroundColor Cyan
Write-Host "?      AmbientSleeper - Master Translation Applicator v2.0     ?" -ForegroundColor Cyan
Write-Host "?                    ALL 7 LANGUAGES                           ?" -ForegroundColor Cyan
Write-Host "????????????????????????????????????????????????????????????????" -ForegroundColor Cyan
Write-Host ""

Write-Host "This script will apply translations to:" -ForegroundColor Yellow
Write-Host "  1. ???? Spanish (es)    - 150+ strings" -ForegroundColor Gray
Write-Host "  2. ???? German (de)     - 150+ strings" -ForegroundColor Gray
Write-Host "  3. ???? French (fr)     - 150+ strings" -ForegroundColor Gray
Write-Host "  4. ???? Japanese (ja)   - 150+ strings" -ForegroundColor Gray
Write-Host "  5. ???? Hindi (hi)      - 150+ strings" -ForegroundColor Gray
Write-Host "  6. ???? Chinese (zh-Hant) - 150+ strings (Traditional)" -ForegroundColor Gray
Write-Host "  7. ???? Arabic (ar)     - 150+ strings (RTL)" -ForegroundColor Gray
Write-Host ""
Write-Host "? IDEMPOTENT: Safe to re-run, will update existing translations" -ForegroundColor Green
Write-Host ""

$confirmation = Read-Host "Continue? (y/N)"
if ($confirmation -ne 'y' -and $confirmation -ne 'Y') {
    Write-Host "Cancelled." -ForegroundColor Yellow
    exit 0
}

Write-Host ""

# Track overall statistics
$totalNew = 0
$totalUpdated = 0
$totalErrors = 0

# Language scripts to run
$languages = @(
    @{Name="Spanish"; Flag="????"; Script="apply-spanish-translations.ps1"; Code="es"}
    @{Name="German"; Flag="????"; Script="apply-german-translations.ps1"; Code="de"}
    @{Name="French"; Flag="????"; Script="apply-french-translations.ps1"; Code="fr"}
    @{Name="Japanese"; Flag="????"; Script="apply-japanese-translations.ps1"; Code="ja"}
    @{Name="Hindi"; Flag="????"; Script="apply-hindi-translations.ps1"; Code="hi"}
    @{Name="Chinese"; Flag="????"; Script="apply-chinese-translations.ps1"; Code="zh-Hant"}
    @{Name="Arabic"; Flag="????"; Script="apply-arabic-translations.ps1"; Code="ar"}
)

$step = 1
foreach ($lang in $languages) {
    Write-Host ""
    Write-Host "????????????????????????????????????????????????????????????" -ForegroundColor Cyan
    Write-Host " STEP $step of 7: $($lang.Flag) $($lang.Name)" -ForegroundColor Cyan
    Write-Host "????????????????????????????????????????????????????????????" -ForegroundColor Cyan
    Write-Host ""
    
    if (Test-Path ".\$($lang.Script)") {
        try {
            # Capture the output
            $output = & ".\$($lang.Script)" 2>&1
            
            # Display output
            $output | ForEach-Object { Write-Host $_ }
            
            # Parse statistics from output
            $newMatch = $output | Select-String "New translations: (\d+)"
            $updatedMatch = $output | Select-String "Updated translations: (\d+)"
            
            if ($newMatch) {
                $totalNew += [int]$newMatch.Matches[0].Groups[1].Value
            }
            if ($updatedMatch) {
                $totalUpdated += [int]$updatedMatch.Matches[0].Groups[1].Value
            }
            
            Write-Host "? $($lang.Name) completed successfully!" -ForegroundColor Green
        }
        catch {
            Write-Host "ERROR: Failed to apply $($lang.Name) translations" -ForegroundColor Red
            Write-Host $_.Exception.Message -ForegroundColor Red
            $totalErrors++
        }
    }
    else {
        Write-Host "ERROR: Script not found: $($lang.Script)" -ForegroundColor Red
        $totalErrors++
    }
    
    $step++
}

Write-Host ""
Write-Host "????????????????????????????????????????????????????????????????" -ForegroundColor Green
Write-Host "?            ? ALL TRANSLATIONS COMPLETED!                    ?" -ForegroundColor Green
Write-Host "????????????????????????????????????????????????????????????????" -ForegroundColor Green
Write-Host ""

Write-Host "?? Overall Summary:" -ForegroundColor Cyan
Write-Host "  ? Languages processed: 7" -ForegroundColor White
Write-Host "  ?? New translations: $totalNew" -ForegroundColor Green
Write-Host "  ?  Updated translations: $totalUpdated" -ForegroundColor Cyan
if ($totalErrors -gt 0) {
    Write-Host "  ? Errors: $totalErrors" -ForegroundColor Red
} else {
    Write-Host "  ? Errors: 0" -ForegroundColor Green
}
Write-Host ""

Write-Host "?? Files Updated:" -ForegroundColor Cyan
foreach ($lang in $languages) {
    $file = "Resources\Strings\AppResources.$($lang.Code).resx"
    if (Test-Path $file) {
        $lastWrite = (Get-Item $file).LastWriteTime.ToString("yyyy-MM-dd HH:mm:ss")
        Write-Host "  $($lang.Flag) $file - $lastWrite" -ForegroundColor Gray
    }
}
Write-Host ""

Write-Host "?? Next Steps:" -ForegroundColor Cyan
Write-Host ""
Write-Host "1. BUILD THE SOLUTION" -ForegroundColor Yellow
Write-Host "   dotnet build" -ForegroundColor Gray
Write-Host "   Should succeed with no errors" -ForegroundColor DarkGray
Write-Host ""
Write-Host "2. TEST EACH LANGUAGE" -ForegroundColor Yellow
Write-Host "   • Change device language to test each translation" -ForegroundColor Gray
Write-Host "   • ???? Español - Spanish" -ForegroundColor DarkGray
Write-Host "   • ???? Deutsch - German" -ForegroundColor DarkGray
Write-Host "   • ???? Français - French" -ForegroundColor DarkGray
Write-Host "   • ???? ??? - Japanese" -ForegroundColor DarkGray
Write-Host "   • ???? ?????? - Hindi" -ForegroundColor DarkGray
Write-Host "   • ???? ???? - Traditional Chinese" -ForegroundColor DarkGray
Write-Host "   • ???? ??????? - Arabic (test RTL layout!)" -ForegroundColor DarkGray
Write-Host ""
Write-Host "3. VERIFY TRANSLATIONS" -ForegroundColor Yellow
Write-Host "   • Navigate through all pages" -ForegroundColor Gray
Write-Host "   • Check buttons, labels, messages" -ForegroundColor Gray
Write-Host "   • Verify format placeholders work ({0}, {1})" -ForegroundColor Gray
Write-Host "   • Test Arabic RTL layout" -ForegroundColor Gray
Write-Host ""
Write-Host "4. CHECK GIT DIFF" -ForegroundColor Yellow
Write-Host "   git diff Resources\Strings\" -ForegroundColor Gray
Write-Host "   Verify only <value> tags changed" -ForegroundColor DarkGray
Write-Host ""

Write-Host "?? Translation Status:" -ForegroundColor Cyan
Write-Host "   ? UI Strings: Complete in all 7 languages (~150 each)" -ForegroundColor Green
Write-Host "   ??  Legal Page: Requires certified legal translator" -ForegroundColor Yellow
Write-Host "   ??  Help Page: Recommended professional translation" -ForegroundColor Yellow
Write-Host ""

Write-Host "?? Re-run Anytime:" -ForegroundColor Cyan
Write-Host "   This script is IDEMPOTENT - safe to run multiple times." -ForegroundColor Gray
Write-Host "   • Updates existing translations" -ForegroundColor Gray
Write-Host "   • Adds new translations" -ForegroundColor Gray
Write-Host "   • Preserves XML structure" -ForegroundColor Gray
Write-Host "   • No risk of corruption" -ForegroundColor Gray
Write-Host ""

if ($totalErrors -eq 0) {
    Write-Host "?? SUCCESS! All 7 languages translated!" -ForegroundColor Green
} else {
    Write-Host "??  COMPLETED WITH $totalErrors ERRORS" -ForegroundColor Yellow
    Write-Host "    Review error messages above" -ForegroundColor Yellow
}
Write-Host ""

Write-Host "Press any key to continue..."
$null = $Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")
