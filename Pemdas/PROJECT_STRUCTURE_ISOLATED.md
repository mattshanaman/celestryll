# Project Structure - Isolated MAUI Apps

## Overview
This workspace contains **two separate MAUI applications** that share a common library:

```
.
├── BadlyDefined/          # MAUI App #1
│   ├── BadlyDefined.csproj
│   ├── Platforms/
│   ├── Pages/
│   ├── ViewModels/
│   └── Services/
│
├── Pemdas.csproj          # MAUI App #2
├── Platforms/
├── Pages/
├── ViewModels/
└── Services/
│
├── GamesCore/             # Shared Library (NOT a MAUI app)
│   ├── GamesCore.csproj
│   ├── ViewModels/
│   └── Models/
│
├── BadlyDefined.sln       # Solution for BadlyDefined + GamesCore
├── Pemdas.sln             # Solution for Pemdas + GamesCore
└── Build-IsolatedProjects.ps1  # Build script
```

## Key Principles

1. **Complete Isolation**: BadlyDefined and Pemdas never reference each other
2. **Shared Core**: Both apps reference GamesCore for common functionality
3. **Separate Solutions**: Each app has its own `.sln` file to prevent build conflicts
4. **Independent Builds**: Build each app separately to avoid namespace conflicts

## GamesCore - Shared Library

**Purpose**: Contains shared code used by both apps
**Type**: Standard .NET 10 class library (NOT a MAUI project)
**Contains**:
- Base ViewModels
- Shared Models
- Common utilities
- MVVM Toolkit only (no MAUI dependencies)

## Building Projects

### Method 1: Use the Build Script (Recommended)
```powershell
.\Build-IsolatedProjects.ps1
```
Select from menu:
1. Build BadlyDefined only
2. Build Pemdas only
3. Clean all (no build)
4. Build both separately

### Method 2: Manual Build

**Build BadlyDefined:**
```powershell
dotnet clean BadlyDefined\BadlyDefined.csproj
dotnet build BadlyDefined.sln
```

**Build Pemdas:**
```powershell
dotnet clean Pemdas.csproj
dotnet build Pemdas.sln
```

**Build GamesCore only:**
```powershell
dotnet build GamesCore\GamesCore.csproj
```

### Method 3: Visual Studio

**Important**: Always open the correct solution file:
- For BadlyDefined: Open `BadlyDefined.sln`
- For Pemdas: Open `Pemdas.sln`

**Never** try to build both projects in the same Visual Studio instance.

## Common Issues and Solutions

### Issue: 95 Build Errors with Duplicates
**Cause**: Both MAUI apps were being built simultaneously
**Solution**: Use separate solution files (now fixed)

### Issue: "Duplicate 'AppDelegate' definition"
**Cause**: Both apps have platform files with same class names
**Solution**: Build apps separately using their own `.sln` files

### Issue: "Cannot find namespace 'BadlyDefined.Platforms'"
**Cause**: Platform files use conditional compilation
**Solution**: Don't add `using` statements for Platforms namespace (already fixed)

### Issue: Locked files in obj folders
**Cause**: Visual Studio has files locked
**Solution**: 
1. Close Visual Studio
2. Run: `Get-Process | Where-Object {$_.ProcessName -like "*devenv*"} | Stop-Process -Force`
3. Delete bin/obj folders manually or use build script

## Project References

### BadlyDefined.csproj
```xml
<ProjectReference Include="..\GamesCore\GamesCore.csproj" />
```

### Pemdas.csproj
```xml
<ProjectReference Include="GamesCore\GamesCore.csproj" />
```

## Development Workflow

1. **Switch between projects**: 
   - Close current solution
   - Open the other `.sln` file

2. **Make changes to shared code**:
   - Edit files in `GamesCore/`
   - Rebuild both solutions to test changes

3. **Clean build from scratch**:
   ```powershell
   .\Build-IsolatedProjects.ps1
   # Select option 3 (Clean all)
   # Then select option 1 or 2 to build specific app
   ```

## What Was Fixed

1. ✅ Created `BadlyDefined.sln` - Isolated solution for BadlyDefined + GamesCore
2. ✅ Created `Pemdas.sln` - Isolated solution for Pemdas + GamesCore
3. ✅ Removed MAUI dependencies from GamesCore (kept only MVVM)
4. ✅ Fixed invalid `using BadlyDefined.Platforms;` in MauiProgram.cs
5. ✅ Created `Build-IsolatedProjects.ps1` script for easy building
6. ✅ Added this documentation

## Next Steps

1. **Close Visual Studio completely**
2. **Run the build script**:
   ```powershell
   .\Build-IsolatedProjects.ps1
   ```
3. **Select option 1** to build BadlyDefined (your current focus)
4. **Open Visual Studio** with `BadlyDefined.sln`

## Troubleshooting

If you still get errors:
1. Close Visual Studio
2. Run: `.\Build-IsolatedProjects.ps1` → Select option 3 (Clean)
3. Manually verify bin/obj folders are deleted
4. Open the specific `.sln` file you want to work with
5. Build in Visual Studio

---

**Important**: Always use the solution files, never try to build both projects together!
