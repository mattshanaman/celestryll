# Generate Multi-Language Resource File Templates
# This script creates template .resx files for all target languages

Write-Host "????????????????????????????????????????????????????????????????" -ForegroundColor Cyan
Write-Host "?   AmbientSleeper - Multi-Language Resource File Generator   ?" -ForegroundColor Cyan
Write-Host "????????????????????????????????????????????????????????????????" -ForegroundColor Cyan
Write-Host ""

# Define target languages
$languages = @(
    @{Code="es"; Name="Spanish"; File="AppResources.es.resx"},
    @{Code="fr"; Name="French"; File="AppResources.fr.resx"},
    @{Code="ja"; Name="Japanese"; File="AppResources.ja.resx"},
    @{Code="hi"; Name="Hindi"; File="AppResources.hi.resx"},
    @{Code="de"; Name="German"; File="AppResources.de.resx"},
    @{Code="zh-Hant"; Name="Traditional Chinese"; File="AppResources.zh-Hant.resx"},
    @{Code="ar"; Name="Arabic (MENA)"; File="AppResources.ar.resx"}
)

$sourceFile = "Resources\Strings\AppResources.resx"
$targetDir = "Resources\Strings"

# Check if source file exists
if (-not (Test-Path $sourceFile)) {
    Write-Host "? Error: Source file not found: $sourceFile" -ForegroundColor Red
    Write-Host "   Please run this script from the project root directory." -ForegroundColor Yellow
    Write-Host "   Current directory: $(Get-Location)" -ForegroundColor Yellow
    exit 1
}

# Get absolute paths
$sourceFileAbsolute = Resolve-Path $sourceFile
$targetDirAbsolute = Resolve-Path $targetDir

Write-Host "? Found source file: $sourceFile" -ForegroundColor Green
Write-Host ""

# Read source file
Write-Host "?? Reading source file..." -ForegroundColor Cyan
$sourceContent = Get-Content $sourceFileAbsolute -Raw -Encoding UTF8

# Count resource strings
$stringCount = ([regex]::Matches($sourceContent, '<data name="')).Count
Write-Host "   Found $stringCount resource strings to translate" -ForegroundColor Gray
Write-Host ""

# Create each language file
foreach ($lang in $languages) {
    $targetFile = Join-Path $targetDirAbsolute $lang.File
    
    Write-Host "?? Creating $($lang.Name) ($($lang.Code))..." -ForegroundColor Cyan
    
    if (Test-Path $targetFile) {
        Write-Host "   ??  File already exists" -ForegroundColor Yellow
        $overwrite = Read-Host "   Overwrite? (y/N)"
        if ($overwrite -ne 'y' -and $overwrite -ne 'Y') {
            Write-Host "   ??  Skipped" -ForegroundColor Gray
            Write-Host ""
            continue
        }
    }
    
    # Create language-specific template
    $langContent = $sourceContent
    
    # Add comment at the top
    $commentBlock = @"
  <!--
    $($lang.Name) ($($lang.Code)) Translation
    
    Status: ?? TRANSLATION REQUIRED
    
    Instructions:
    1. Translate all <value> tags below
    2. Keep format placeholders: {0}, {1}, {0:P0}, etc.
    3. Keep emojis and icons unchanged
    4. Preserve XML structure
    5. Use UTF-8 encoding
    
    String Count: $stringCount
    Priority: Start with critical navigation, action, and confirmation strings
    
    For translation guidelines, see: MULTI_LANGUAGE_LOCALIZATION_GUIDE.md
  -->

"@
    
    # Insert comment after the opening <root> tag
    $langContent = $langContent -replace '(<root>)', "`$1`n$commentBlock"
    
    # Write file using FileStream for complete control
    try {
        # Create UTF-8 encoding with BOM
        $utf8WithBom = New-Object System.Text.UTF8Encoding($true)
        
        # Create file stream (Create mode will create new file or overwrite existing)
        $fileStream = [System.IO.File]::Create($targetFile)
        $streamWriter = New-Object System.IO.StreamWriter($fileStream, $utf8WithBom)
        
        # Write content
        $streamWriter.Write($langContent)
        
        # Close streams
        $streamWriter.Close()
        $fileStream.Close()
        
        Write-Host "   ? Created successfully!" -ForegroundColor Green
        Write-Host "      Strings to translate: $stringCount" -ForegroundColor Gray
    }
    catch {
        Write-Host "   ? Error: $($_.Exception.Message)" -ForegroundColor Red
        Write-Host "      Type: $($_.Exception.GetType().FullName)" -ForegroundColor DarkGray
        if ($fileStream) { $fileStream.Dispose() }
        if ($streamWriter) { $streamWriter.Dispose() }
    }
    Write-Host ""
}

Write-Host "????????????????????????????????????????????????????????????????" -ForegroundColor Green
Write-Host "?                    ? GENERATION COMPLETE                    ?" -ForegroundColor Green
Write-Host "????????????????????????????????????????????????????????????????" -ForegroundColor Green
Write-Host ""

Write-Host "?? Created Files:" -ForegroundColor Cyan
$filesCreated = 0
foreach ($lang in $languages) {
    $targetFile = Join-Path $targetDirAbsolute $lang.File
    if (Test-Path $targetFile) {
        $size = (Get-Item $targetFile).Length / 1KB
        Write-Host "   ? $($lang.File) ($([math]::Round($size, 1)) KB)" -ForegroundColor Green
        $filesCreated++
    }
    else {
        Write-Host "   ??  $($lang.File) - NOT CREATED" -ForegroundColor Yellow
    }
}
Write-Host ""
Write-Host "   Total: $filesCreated of $($languages.Count) files created" -ForegroundColor Cyan
Write-Host ""

if ($filesCreated -gt 0) {
    Write-Host "?? Next Steps:" -ForegroundColor Cyan
    Write-Host "   1. Open each .resx file in Visual Studio or text editor" -ForegroundColor Gray
    Write-Host "   2. Translate all <value> tags (preserve {0}, {1}, etc.)" -ForegroundColor Gray
    Write-Host "   3. Keep emojis and format strings unchanged" -ForegroundColor Gray
    Write-Host "   4. Save with UTF-8 encoding" -ForegroundColor Gray
    Write-Host "   5. Build solution to verify" -ForegroundColor Gray
    Write-Host "   6. Test in each language" -ForegroundColor Gray
    Write-Host ""

    Write-Host "?? Translation Options:" -ForegroundColor Cyan
    Write-Host "   • Professional translation service (recommended)" -ForegroundColor Gray
    Write-Host "   • Azure Translator API + native review" -ForegroundColor Gray
    Write-Host "   • Community translation platforms" -ForegroundColor Gray
    Write-Host ""

    Write-Host "?? Total Translation Work:" -ForegroundColor Cyan
    Write-Host "   Strings: $stringCount" -ForegroundColor White
    Write-Host "   Languages: $($languages.Count)" -ForegroundColor White
    Write-Host "   Total: $($stringCount * $languages.Count) translations" -ForegroundColor White
    Write-Host ""

    Write-Host "?? Pro Tip: Start with the top 100 critical strings (navigation, actions, confirmations)" -ForegroundColor Yellow
    Write-Host "   See MULTI_LANGUAGE_LOCALIZATION_GUIDE.md for priority list" -ForegroundColor Yellow
}
else {
    Write-Host "??  Warning: No files were created!" -ForegroundColor Yellow
    Write-Host "   Please review the errors above." -ForegroundColor Yellow
}
Write-Host ""

Write-Host "Press any key to continue..."
$null = $Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")
