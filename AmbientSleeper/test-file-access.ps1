# Emergency Translation Applicator - Works Even With VS Open
# Uses alternate file access method

Write-Host "????????????????????????????????????????????????????????????????" -ForegroundColor Cyan
Write-Host "?   Emergency Translation Applicator - Advanced File Access   ?" -ForegroundColor Cyan
Write-Host "????????????????????????????????????????????????????????????????" -ForegroundColor Cyan
Write-Host ""

Write-Host "This script uses advanced techniques to access locked files." -ForegroundColor Yellow
Write-Host "It may still work even if Visual Studio has files open." -ForegroundColor Yellow
Write-Host ""

# Function to release file handle and write
function Write-FileWithRetry {
    param(
        [string]$Path,
        [string]$Content
    )
    
    $maxAttempts = 5
    $attempt = 0
    
    while ($attempt -lt $maxAttempts) {
        $attempt++
        
        try {
            # Try standard write first
            $utf8 = New-Object System.Text.UTF8Encoding($true)
            $bytes = $utf8.GetBytes($Content)
            
            # Use FileStream with explicit sharing permissions
            $fileStream = New-Object System.IO.FileStream(
                $Path,
                [System.IO.FileMode]::Create,
                [System.IO.FileAccess]::Write,
                [System.IO.FileShare]::ReadWrite  # Allow others to read
            )
            
            $fileStream.Write($bytes, 0, $bytes.Length)
            $fileStream.Flush()
            $fileStream.Close()
            $fileStream.Dispose()
            
            return $true
        }
        catch {
            if ($attempt -lt $maxAttempts) {
                Write-Host "  Attempt $attempt failed, retrying in 1 second..." -ForegroundColor Yellow
                Start-Sleep -Seconds 1
                
                # Try to close any open handles (Windows only)
                try {
                    [System.GC]::Collect()
                    [System.GC]::WaitForPendingFinalizers()
                }
                catch { }
            }
            else {
                Write-Host "  Failed after $maxAttempts attempts" -ForegroundColor Red
                Write-Host "  Error: $($_.Exception.Message)" -ForegroundColor Red
                return $false
            }
        }
    }
    
    return $false
}

# Test with one file first
$testFile = "Resources\Strings\AppResources.es.resx"

Write-Host "Testing file access on $testFile..." -ForegroundColor Yellow

if (Test-Path $testFile) {
    $content = [System.IO.File]::ReadAllText($testFile)
    
    if (Write-FileWithRetry $testFile $content) {
        Write-Host "? Test successful! Files are accessible." -ForegroundColor Green
        Write-Host ""
        Write-Host "You can now run the main script:" -ForegroundColor Cyan
        Write-Host "  powershell -ExecutionPolicy Bypass -File .\apply-all-translations.ps1" -ForegroundColor White
        Write-Host ""
    }
    else {
        Write-Host "? Test failed. Files are locked." -ForegroundColor Red
        Write-Host ""
        Write-Host "SOLUTION:" -ForegroundColor Yellow
        Write-Host "1. Close Visual Studio" -ForegroundColor White
        Write-Host "2. Wait 5 seconds" -ForegroundColor White
        Write-Host "3. Run this script again" -ForegroundColor White
        Write-Host ""
    }
}
else {
    Write-Host "ERROR: Test file not found: $testFile" -ForegroundColor Red
}

Write-Host "Press any key to continue..."
$null = $Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")
