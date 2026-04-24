# AmbientSleeper .NET 10 Upgrade Tasks

## Overview

This document tracks the execution of the AmbientSleeper MAUI project upgrade from .NET 9 to .NET 10. The single project will be upgraded atomically, followed by testing and validation.

**Progress**: 2/3 tasks complete (67%) ![0%](https://progress-bar.xyz/67)

---

## Tasks

### [✓] TASK-001: Atomic framework and dependency upgrade *(Completed: 2026-04-04 23:38)*
**References**: Plan §Phase 1, Plan §Project-by-Project Plans, Plan §Breaking Changes Catalog

- [✓] (1) Update TargetFrameworks property in AmbientSleeper.csproj to net10.0-android;net10.0-ios;net10.0-maccatalyst per Plan §Phase 1
- [✓] (2) Restore dependencies via dotnet restore
- [✓] (3) Build solution and fix all 39 TimeSpan compilation errors per Plan §Breaking Changes Catalog (explicitly cast numeric literals for FromSeconds, FromMinutes, FromMilliseconds, FromHours overloads)
- [✓] (4) Solution builds with 0 errors for all platforms (**Verify**)

---

### [✓] TASK-002: Run unit tests and fix failures *(Completed: 2026-04-04 23:39)*
**References**: Plan §Phase 2

- [✓] (1) Run unit test projects if present in solution
- [✓] (2) Fix any test failures (reference Plan §Breaking Changes for TimeSpan-related issues)
- [✓] (3) Re-run tests after fixes
- [✓] (4) All tests pass with 0 failures (**Verify**)

---

### [✗] TASK-003: Final commit
**References**: Plan §Source Control Strategy

- [✗] (1) Commit all changes with message: "Upgrade to .NET 10"

---







