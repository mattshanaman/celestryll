# .NET 10 Migration Plan

## Table of Contents
- [Executive Summary](#executive-summary)
- [Migration Strategy](#migration-strategy)
- [Detailed Dependency Analysis](#detailed-dependency-analysis)
- [Project-by-Project Plans](#project-by-project-plans)
- [Package Update Reference](#package-update-reference)
- [Breaking Changes Catalog](#breaking-changes-catalog)
- [Testing & Validation Strategy](#testing--validation-strategy)
- [Risk Management](#risk-management)
- [Complexity & Effort Assessment](#complexity--effort-assessment)
- [Source Control Strategy](#source-control-strategy)
- [Success Criteria](#success-criteria)

## Executive Summary

### Selected Strategy
**All-At-Once Strategy** - All projects upgraded simultaneously in a single atomic operation.

**Rationale**: 
- 1 project total (small solution size)
- Currently on .NET 9
- Clear dependency structure (no project references)
- All 7 current NuGet packages are assessed as 100% compatible with .NET 10

### Scope
- **Current State**: .NET 9 MAUI (`net9.0-android;net9.0-ios;net9.0-maccatalyst`)
- **Target State**: .NET 10 MAUI (`net10.0-android;net10.0-ios;net10.0-maccatalyst`)
- **Projects Affected**: 1 (`AmbientSleeper.csproj`)
- **Critical Issues**: 
  - 39 Source incompatible API issues related to `TimeSpan` formatting/methods
  - No security vulnerabilities
  - No package conflicts

## Implementation Timeline

### Phase 1: Atomic Upgrade
**Operations** (performed as a single coordinated batch):
- Update project file to .NET 10 target frameworks
- Restore dependencies
- Fix compilation errors (TimeSpan breaking changes)
- Build and verify

**Deliverables**: Solution builds with 0 errors and 0 warnings.

### Phase 2: Test Validation
**Operations**:
- Execute any unit test projects (if existing)
- Execute manual/smoke validation for MAUI platforms
- Address any runtime failures

**Deliverables**: All tests and platforms launch successfully.

## Migration Strategy
We will use the **All-At-Once Strategy**. With only one project in the solution, there are no complex dependency chains or project tiers requiring phased execution. The entire update (framework bumps and code fixes) will happen as an atomic update. 

## Detailed Dependency Analysis
The solution consists of a single application component (`AmbientSleeper.csproj`). There are no localized dependencies or circular dependencies to untangle. 

## Project-by-Project Plans

### Project: AmbientSleeper.csproj
**Current State**: `net9.0-android;net9.0-ios;net9.0-maccatalyst`, 7 NuGet dependencies, ~23k LOC
**Target State**: `net10.0-android;net10.0-ios;net10.0-maccatalyst`

**Migration Steps**:
1. Update `TargetFrameworks` property in the `.csproj` file to target .NET 10 platforms.
2. Run `dotnet restore` to resolve packages targeting the new framework.
3. Address source incompatible changes related to `System.TimeSpan` overloads:
   - `TimeSpan.FromSeconds(Int64)` (14 occurences)
   - `TimeSpan.FromMinutes(Int64)` (10 occurences)
   - `TimeSpan.FromMilliseconds(Int64, Int64)` (9 occurences)
   - `TimeSpan.FromMilliseconds(Double)` (3 occurences)
   - `TimeSpan.FromHours(Int32)` (2 occurences)
   - `TimeSpan.FromMinutes(Double)` (1 occurence)
4. Validate build success for all target platforms payload.

## Package Update Reference
Based on the assessment data, all current package versions are entirely compatible with the target framework. A direct version bump is not strictly mandated but restoring for the new TFM is necessary.

| Package | Current Version | Target Version | Projects Affected | Update Reason |
|---------|-----------------|----------------|-------------------|---------------|
| CommunityToolkit.Maui | 12.2.0 | 12.2.0 | AmbientSleeper | Compatible |
| CommunityToolkit.Mvvm | 8.4.0 | 8.4.0 | AmbientSleeper | Compatible |
| Microsoft.Extensions.Logging.Debug | 9.0.9 | 9.0.9 | AmbientSleeper | Compatible |
| Microsoft.Maui.Controls | 9.0.110 | 9.0.110 | AmbientSleeper | Compatible |
| Plugin.LocalNotification | 12.0.2 | 12.0.2 | AmbientSleeper | Compatible |
| Plugin.Maui.AppRating | 1.2.2 | 1.2.2 | AmbientSleeper | Compatible |
| Plugin.Maui.Audio | 4.0.0 | 4.0.0 | AmbientSleeper | Compatible |

## Breaking Changes Catalog
### AmbientSleeper Source Incompatibility
- **System.TimeSpan Updates**: New math overloads in newer .NET versioning creates ambiguous invocation errors during re-compilation if the numeric types are not strictly cast (e.g., `FromSeconds(long)` vs `FromSeconds(double)`). Mitigation: Explicitly cast parameters or use standard overloads strictly matching the type signature during step 3. Include `System.TimeSpan.FromSeconds`, `FromMinutes`, `FromMilliseconds`, `FromHours`.

## Testing & Validation Strategy
**Validation Checklist**:
- [ ] Builds without errors for Android (`net10.0-android`)
- [ ] Builds without errors for iOS (`net10.0-ios`)
- [ ] Builds without errors for MacCatalyst (`net10.0-maccatalyst`)
- [ ] Application deploys cleanly and the audio/playback interactions (implied by `Plugin.Maui.Audio` and manifest) function normally.
- [ ] Timer features correctly manipulate the `TimeSpan` changes effectively without crashing.

## Risk Management

| Risk | Level | Description | Mitigation Strategy |
|------|-------|-------------|---------------------|
| Ambiguous Overload Calls | Low | Code might not compile smoothly due to TimeSpan signatures. | Explicitly cast literals and variables to `double` or `int`/`long` respective to the intended invocation signature. |
| Platform SDK incompatibility | Medium | MAUI Workloads might require an explicit SDK update locally. | Ensure `dotnet workload install maui` is run targeting .NET 10 payloads before compiling. |

## Complexity & Effort Assessment
| Project | Complexity Rating | Dependencies | Risk Level |
|---------|-------------------|--------------|------------|
| AmbientSleeper | Low | 0 | Low |

The overall effort is expected to be Low due to limited breaking changes surface area, small project numbers, and direct compatibility of existing Nuget integrations.

## Source Control Strategy
- **Single Commit Upgrades**: Given the atomic All-At-Once nature of this single-project upgrade, all framework updates and compilation fixes will be bundled into a single unified commit (`Upgrade to .NET 10`).

## Success Criteria
- [ ] `AmbientSleeper.csproj` accurately reflects .NET 10 TFMs.
- [ ] All 39 `TimeSpan` code issues are appropriately patched.
- [ ] The solution successfully builds with 0 compilation errors across all .NET 10 MAUI targets.
- [ ] The upgrade process followed atomic All-At-Once constraints.