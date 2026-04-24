# Clean and Build Script for Isolated MAUI Projects
# This script ensures Pemdas and BadlyDefined are completely isolated

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "ISOLATED PROJECT CLEANUP AND BUILD" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# Function to kill locked processes
function Stop-LockedProcesses {
    Write-Host "Stopping Visual Studio and MSBuild processes..." -ForegroundColor Yellow
    Get-Process | Where-Object {$_.ProcessName -like "*devenv*" -or $_.ProcessName -like "*MSBuild*"} | Stop-Process -Force -ErrorAction SilentlyContinue
    Start-Sleep -Seconds 2
}

# Function to clean project
function Clean-Project {
    param($ProjectPath, $ProjectName)
    
    Write-Host ""
    Write-Host "Cleaning $ProjectName..." -ForegroundColor Yellow
    
    $projectDir = Split-Path -Parent $ProjectPath
    if ($projectDir -eq "") { $projectDir = "." }
    
    # Remove bin and obj folders
    $binPath = Join-Path $projectDir "bin"
    $objPath = Join-Path $projectDir "obj"
    
    if (Test-Path $binPath) {
        Remove-Item $binPath -Recurse -Force -ErrorAction SilentlyContinue
        Write-Host "  Removed bin folder" -ForegroundColor Green
    }
    
    if (Test-Path $objPath) {
        Remove-Item $objPath -Recurse -Force -ErrorAction SilentlyContinue
        Write-Host "  Removed obj folder" -ForegroundColor Green
    }
    
    # Run dotnet clean
    dotnet clean $ProjectPath --nologo -v quiet
    Write-Host "  Ran dotnet clean" -ForegroundColor Green
}

# Function to build project
function Build-Project {
    param($SolutionPath, $ProjectName)
    
    Write-Host ""
    Write-Host "Building $ProjectName..." -ForegroundColor Yellow
    Write-Host "Using solution: $SolutionPath" -ForegroundColor Gray
    Write-Host ""
    
    dotnet build $SolutionPath --nologo
    
    if ($LASTEXITCODE -eq 0) {
        Write-Host ""
        Write-Host "✓ $ProjectName built successfully!" -ForegroundColor Green
        return $true
    } else {
        Write-Host ""
        Write-Host "✗ $ProjectName build failed!" -ForegroundColor Red
        return $false
    }
}

# Menu
Write-Host "Select which project to build:" -ForegroundColor Cyan
Write-Host "1. BadlyDefined (with GamesCore)" -ForegroundColor White
Write-Host "2. Pemdas (with GamesCore)" -ForegroundColor White
Write-Host "3. Clean both (no build)" -ForegroundColor White
Write-Host "4. Build both separately" -ForegroundColor White
Write-Host ""
$choice = Read-Host "Enter choice (1-4)"

Stop-LockedProcesses

switch ($choice) {
    "1" {
        Clean-Project "GamesCore\GamesCore.csproj" "GamesCore"
        Clean-Project "BadlyDefined\BadlyDefined.csproj" "BadlyDefined"
        Build-Project "BadlyDefined.sln" "BadlyDefined"
    }
    "2" {
        Clean-Project "GamesCore\GamesCore.csproj" "GamesCore"
        Clean-Project "Pemdas.csproj" "Pemdas"
        Build-Project "Pemdas.sln" "Pemdas"
    }
    "3" {
        Clean-Project "GamesCore\GamesCore.csproj" "GamesCore"
        Clean-Project "BadlyDefined\BadlyDefined.csproj" "BadlyDefined"
        Clean-Project "Pemdas.csproj" "Pemdas"
        Write-Host ""
        Write-Host "✓ All projects cleaned!" -ForegroundColor Green
    }
    "4" {
        # Build GamesCore first
        Clean-Project "GamesCore\GamesCore.csproj" "GamesCore"
        dotnet build "GamesCore\GamesCore.csproj" --nologo
        
        if ($LASTEXITCODE -eq 0) {
            Write-Host ""
            Write-Host "✓ GamesCore built successfully!" -ForegroundColor Green
            
            # Build BadlyDefined
            Clean-Project "BadlyDefined\BadlyDefined.csproj" "BadlyDefined"
            $badlySuccess = Build-Project "BadlyDefined.sln" "BadlyDefined"
            
            # Build Pemdas
            Clean-Project "Pemdas.csproj" "Pemdas"
            $pemdasSuccess = Build-Project "Pemdas.sln" "Pemdas"
            
            Write-Host ""
            Write-Host "========================================" -ForegroundColor Cyan
            Write-Host "BUILD SUMMARY" -ForegroundColor Cyan
            Write-Host "========================================" -ForegroundColor Cyan
            Write-Host "GamesCore: " -NoNewline; Write-Host "SUCCESS" -ForegroundColor Green
            Write-Host "BadlyDefined: " -NoNewline
            if ($badlySuccess) { Write-Host "SUCCESS" -ForegroundColor Green } else { Write-Host "FAILED" -ForegroundColor Red }
            Write-Host "Pemdas: " -NoNewline
            if ($pemdasSuccess) { Write-Host "SUCCESS" -ForegroundColor Green } else { Write-Host "FAILED" -ForegroundColor Red }
        } else {
            Write-Host ""
            Write-Host "✗ GamesCore build failed! Cannot continue." -ForegroundColor Red
        }
    }
    default {
        Write-Host "Invalid choice!" -ForegroundColor Red
    }
}

Write-Host ""
Write-Host "Done!" -ForegroundColor Cyan
