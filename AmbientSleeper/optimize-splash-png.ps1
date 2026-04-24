# PNG Optimization Script for AmbientSleeper
# This script optimizes the splash screen PNG file

$sourceFile = "Resources\Splash\ambient_sleeper_background_and_small_icon2.png"
$backupFile = "Resources\Splash\ambient_sleeper_background_and_small_icon2_BACKUP.png"
$tempFile = "Resources\Splash\ambient_sleeper_temp.png"

Write-Host "==================================================" -ForegroundColor Cyan
Write-Host "  PNG Splash Screen Optimizer for AmbientSleeper" -ForegroundColor Cyan
Write-Host "==================================================" -ForegroundColor Cyan
Write-Host ""

# Check if source file exists
if (-not (Test-Path $sourceFile)) {
    Write-Host "ERROR: Source file not found!" -ForegroundColor Red
    Write-Host "Expected: $sourceFile" -ForegroundColor Yellow
    Write-Host ""
    Write-Host "Please make sure you're running this script from the project root directory." -ForegroundColor Yellow
    pause
    exit 1
}

# Get original file size
$originalSize = (Get-Item $sourceFile).Length
$originalSizeMB = [math]::Round($originalSize / 1MB, 2)

Write-Host "Original file: $sourceFile" -ForegroundColor White
Write-Host "Original size: $originalSizeMB MB ($originalSize bytes)" -ForegroundColor Yellow
Write-Host ""

# Create backup
Write-Host "Creating backup..." -ForegroundColor Cyan
Copy-Item $sourceFile $backupFile -Force
Write-Host "Backup created: $backupFile" -ForegroundColor Green
Write-Host ""

# Method 1: Try using .NET Image classes for optimization
Write-Host "Optimizing PNG with .NET System.Drawing..." -ForegroundColor Cyan

try {
    Add-Type -AssemblyName System.Drawing
    
    # Load the image
    $image = [System.Drawing.Image]::FromFile((Resolve-Path $sourceFile))
    
    # Create encoder parameters for quality
    $encoder = [System.Drawing.Imaging.ImageCodecInfo]::GetImageEncoders() | 
     Where-Object { $_.MimeType -eq 'image/png' }
    
    $encoderParams = New-Object System.Drawing.Imaging.EncoderParameters(1)
    $encoderParams.Param[0] = New-Object System.Drawing.Imaging.EncoderParameter(
        [System.Drawing.Imaging.Encoder]::Quality, 85L
    )
    
    # Save with compression
    $image.Save((Resolve-Path $tempFile), $encoder, $encoderParams)
    $image.Dispose()
    
  # Get new size
 $newSize = (Get-Item $tempFile).Length
    $newSizeMB = [math]::Round($newSize / 1MB, 2)
    $reduction = [math]::Round((($originalSize - $newSize) / $originalSize) * 100, 1)
    
    Write-Host ""
    Write-Host "Optimization complete!" -ForegroundColor Green
    Write-Host "New size: $newSizeMB MB ($newSize bytes)" -ForegroundColor Green
    Write-Host "Reduction: $reduction%" -ForegroundColor Green
    Write-Host ""
    
 # Ask if user wants to keep the optimized version
    $response = Read-Host "Replace original file with optimized version? (Y/N)"
    
    if ($response -eq 'Y' -or $response -eq 'y') {
        Remove-Item $sourceFile -Force
        Move-Item $tempFile $sourceFile -Force
        Write-Host ""
        Write-Host "SUCCESS! Original file replaced with optimized version." -ForegroundColor Green
        Write-Host "Backup saved as: $backupFile" -ForegroundColor Cyan
    } else {
        Remove-Item $tempFile -Force
  Write-Host ""
     Write-Host "Optimization cancelled. Original file unchanged." -ForegroundColor Yellow
    }
}
catch {
 Write-Host ""
    Write-Host "ERROR: Failed to optimize with .NET System.Drawing" -ForegroundColor Red
    Write-Host "Error: $($_.Exception.Message)" -ForegroundColor Yellow
    Write-Host ""
    Write-Host "ALTERNATIVE: Use TinyPNG.com" -ForegroundColor Cyan
    Write-Host "1. Go to: https://tinypng.com" -ForegroundColor White
    Write-Host "2. Upload: $sourceFile" -ForegroundColor White
    Write-Host "3. Download the optimized file" -ForegroundColor White
  Write-Host "4. Replace the original file" -ForegroundColor White
}

Write-Host ""
Write-Host "==================================================" -ForegroundColor Cyan
Write-Host "Next Steps:" -ForegroundColor Cyan
Write-Host "1. Rebuild the project (dotnet build)" -ForegroundColor White
Write-Host "2. Test the splash screen on a device" -ForegroundColor White
Write-Host "3. If satisfied, delete the backup file" -ForegroundColor White
Write-Host "==================================================" -ForegroundColor Cyan
Write-Host ""

pause
