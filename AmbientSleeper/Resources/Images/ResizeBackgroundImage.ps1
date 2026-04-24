# ResizeBackgroundImage.ps1
# Resizes ambient_sleeper_background_and_small_icon.png to common phone/tablet background sizes.
# Produces center-cropped copies for portrait and landscape orientations.
#
# HOW TO RUN:
#   Right-click this file in Explorer → "Run with PowerShell"
#   — OR —
#   Open your own PowerShell window and run:
#       pwsh -File "C:\Projects\AmbientSleeper\Resources\Images\ResizeBackgroundImage.ps1"
#
# OUTPUT FILES (saved alongside this script):
#   ambient_sleeper_bg_phone_portrait_1080x1920.png   — Full HD phone portrait
#   ambient_sleeper_bg_phone_portrait_720x1280.png    — HD phone portrait
#   ambient_sleeper_bg_tablet_portrait_1200x1600.png  — Android tablet portrait
#   ambient_sleeper_bg_tablet_portrait_1536x2048.png  — iPad/large tablet portrait

Add-Type -AssemblyName System.Drawing

$scriptDir  = Split-Path -Parent $MyInvocation.MyCommand.Path
$sourceFile = Join-Path $scriptDir "ambient_sleeper_background_and_small_icon.png"

if (-not (Test-Path $sourceFile)) {
    Write-Error "Source file not found: $sourceFile"
    exit 1
}

# Target sizes: [label, width, height]
$targets = @(
    @{ Label = "phone_portrait_1080x1920";  W = 1080; H = 1920 },
    @{ Label = "phone_portrait_720x1280";   W =  720; H = 1280 },
    @{ Label = "tablet_portrait_1200x1600"; W = 1200; H = 1600 },
    @{ Label = "tablet_portrait_1536x2048"; W = 1536; H = 2048 }
)

function Resize-CenterCrop {
    param(
        [System.Drawing.Bitmap]$Source,
        [int]$TargetW,
        [int]$TargetH
    )

    $srcW = $Source.Width
    $srcH = $Source.Height

    # Scale so the image fully covers the target (cover, not contain)
    $scale = [Math]::Max($TargetW / $srcW, $TargetH / $srcH)

    # Center-crop offset in source pixels
    $cropX = ($srcW - $TargetW / $scale) / 2
    $cropY = ($srcH - $TargetH / $scale) / 2

    $bmp = New-Object System.Drawing.Bitmap($TargetW, $TargetH)
    $g   = [System.Drawing.Graphics]::FromImage($bmp)
    $g.InterpolationMode  = [System.Drawing.Drawing2D.InterpolationMode]::HighQualityBicubic
    $g.CompositingQuality = [System.Drawing.Drawing2D.CompositingQuality]::HighQuality
    $g.SmoothingMode      = [System.Drawing.Drawing2D.SmoothingMode]::HighQuality
    $g.PixelOffsetMode    = [System.Drawing.Drawing2D.PixelOffsetMode]::HighQuality

    $g.DrawImage(
        $Source,
        [System.Drawing.Rectangle]::new(0, 0, $TargetW, $TargetH),
        [float]$cropX, [float]$cropY,
        [float]($TargetW / $scale), [float]($TargetH / $scale),
        [System.Drawing.GraphicsUnit]::Pixel
    )
    $g.Dispose()

    # Render to MemoryStream (avoids GDI+ "generic error" on direct file save)
    $saveMs = New-Object System.IO.MemoryStream
    $bmp.Save($saveMs, [System.Drawing.Imaging.ImageFormat]::Png)
    $bmp.Dispose()
    return $saveMs.ToArray()
}

# ---------------------------------------------------------------------------
# Try writing to $Path first; if Controlled Folder Access (Windows Defender
# ransomware protection) blocks it, fall back to $FallbackDir.
# ---------------------------------------------------------------------------
function Write-ImageBytes {
    param([byte[]]$Bytes, [string]$Path, [string]$FallbackDir)
    try {
        [System.IO.File]::WriteAllBytes($Path, $Bytes)
        return $Path
    } catch {
        $fallback = Join-Path $FallbackDir (Split-Path -Leaf $Path)
        [System.IO.File]::WriteAllBytes($fallback, $Bytes)
        return $fallback
    }
}

# Load source into MemoryStream — releases the file handle immediately so GDI+
# does not hold a lock on the source file while writing output files.
$sourceBytes = [System.IO.File]::ReadAllBytes($sourceFile)
$sourceMs    = New-Object System.IO.MemoryStream(,$sourceBytes)
$srcImage    = [System.Drawing.Bitmap]::new($sourceMs)

$desktop      = [System.Environment]::GetFolderPath('Desktop')
$usedFallback = $false

Write-Host "Source: $(Split-Path -Leaf $sourceFile)  ($($srcImage.Width) x $($srcImage.Height))"
Write-Host ""

foreach ($t in $targets) {
    $outFile = Join-Path $scriptDir "ambient_sleeper_bg_$($t.Label).png"
    try {
        $bytes   = Resize-CenterCrop -Source $srcImage -TargetW $t.W -TargetH $t.H
        $written = Write-ImageBytes -Bytes $bytes -Path $outFile -FallbackDir $desktop
        if ($written -eq $outFile) {
            Write-Host "  Created: $written  ($($t.W) x $($t.H))"
        } else {
            $usedFallback = $true
            Write-Host "  Saved to Desktop: $(Split-Path -Leaf $written)  ($($t.W) x $($t.H))"
        }
    } catch {
        Write-Warning "  FAILED: ambient_sleeper_bg_$($t.Label).png — $_"
    }
}

$srcImage.Dispose()
$sourceMs.Dispose()

Write-Host ""
if ($usedFallback) {
    Write-Host "NOTE: Windows Defender Controlled Folder Access blocked writing to:" -ForegroundColor Yellow
    Write-Host "  $scriptDir" -ForegroundColor Yellow
    Write-Host ""
    Write-Host "Files were saved to your Desktop instead. Copy them with:" -ForegroundColor Cyan
    Write-Host "  Copy-Item \"$desktop\\ambient_sleeper_bg_*.png\" \"$scriptDir\"" -ForegroundColor Cyan
    Write-Host ""
    Write-Host "Or allow pwsh.exe in: Windows Security → Virus & threat protection →"
    Write-Host "  Ransomware protection → Controlled folder access → Allow an app" -ForegroundColor Gray
} else {
    Write-Host "Done. Files saved to: $scriptDir"
}
