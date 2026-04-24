@echo off
echo ================================================
echo  PNG Optimization Helper for AmbientSleeper
echo ================================================
echo.

cd /d "%~dp0"

if not exist "Resources\Splash\ambient_sleeper_background_and_small_icon2.png" (
    echo ERROR: PNG file not found!
    echo Please run this from the project root directory.
    pause
    exit /b 1
)

echo Current file location:
echo %CD%\Resources\Splash\ambient_sleeper_background_and_small_icon2.png
echo.

for %%A in ("Resources\Splash\ambient_sleeper_background_and_small_icon2.png") do (
    set size=%%~zA
    set /a sizeMB=%%~zA / 1048576
)

echo Current size: %sizeMB% MB
echo.
echo ================================================
echo  RECOMMENDED: Use TinyPNG.com
echo ================================================
echo.
echo 1. Open: https://tinypng.com
echo 2. Upload: Resources\Splash\ambient_sleeper_background_and_small_icon2.png
echo 3. Download the optimized file
echo 4. Replace the original file
echo.
echo This will reduce the file by 70-90%%
echo From 4.36 MB to approximately 300-800 KB
echo.
echo ================================================
echo.
echo Opening TinyPNG website in your browser...
echo.

start https://tinypng.com

echo.
echo After downloading from TinyPNG:
echo 1. Backup current file (rename to _BACKUP.png)
echo 2. Move downloaded file here
echo 3. Rename to: ambient_sleeper_background_and_small_icon2.png
echo.
pause
