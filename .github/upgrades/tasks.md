# InquirySpark .NET 10.0 Big Bang Upgrade

## Overview

This scenario upgrades the entire InquirySpark solution (6 projects) from .NET 8.0 to .NET 10.0 in a single atomic operation, simultaneously updating all package dependencies and remediating security vulnerabilities. The upgrade follows the Big Bang strategy: all projects are updated together, with a single coordinated build and test validation phase. The goal is to ensure all projects build and pass automated tests with no manual/visual validation required.

**Progress**: 1/4 tasks complete (25%) ![25%](https://progress-bar.xyz/25)

## Tasks

### [✓] TASK-001: Remove incompatible Azure Container Tools package *(Completed: 2025-12-02 22:40)*
**References**: Plan §Phase 0, Plan §Package Update Reference

- [✓] (1) Remove `Microsoft.VisualStudio.Azure.Containers.Tools.Targets` package from all projects listed in Plan §Package Update Reference (Repository, Web, WebApi)
- [✓] (2) Commit changes with message: "TASK-001: Remove incompatible Azure Container Tools package for .NET 10.0 upgrade"
- [✓] (3) Incompatible package successfully removed from all projects (**Verify**)

### [▶] TASK-002: Atomic framework and package upgrade
**References**: Plan §Phase 1, Plan §Package Update Reference, Plan §Breaking Changes Catalog

- [▶] (1) Update `TargetFramework` to `net10.0` in all projects per Plan §Phase 1
- [ ] (2) Update all package references per Plan §Package Update Reference (including security fix for Azure.Identity)
- [ ] (3) Restore all dependencies for the solution
- [ ] (4) Build the solution and fix all compilation errors per Plan §Breaking Changes Catalog
- [ ] (5) Ensure solution builds with 0 errors (**Verify**)
- [ ] (6) Commit changes with message: "TASK-002: Atomic .NET 10.0 upgrade and package updates"

### [ ] TASK-003: Run automated test suite
**References**: Plan §Phase 2, Plan §3.2, Plan §Testing Strategy

- [ ] (1) Run all unit tests in InquirySpark.Common.Tests project
- [ ] (2) Run all available integration or automated tests for InquirySpark.Admin, InquirySpark.Web, and InquirySpark.WebApi (see Plan §3.2, §Testing Strategy)
- [ ] (3) Fix any test failures related to the upgrade (reference Plan §Breaking Changes Catalog for common issues)
- [ ] (4) Re-run tests after fixes
- [ ] (5) All automated tests pass with 0 failures (**Verify**)
- [ ] (6) Commit changes with message: "TASK-003: Automated test fixes after .NET 10.0 upgrade"

### [ ] TASK-004: Final validation and completion
**References**: Plan §Success Criteria, Plan §10.1, Plan §10.2

- [ ] (1) Ensure all technical success criteria in Plan §Success Criteria are met using automated checks (builds, tests, security scan)
- [ ] (2) Run `dotnet list package --vulnerable` to confirm no security vulnerabilities remain (**Verify**)
- [ ] (3) Run `dotnet build` and `dotnet test` to confirm 0 errors and all tests pass (**Verify**)
- [ ] (4) Commit any final changes with message: "TASK-004: Finalize .NET 10.0 upgrade and validation"
- [ ] (5) All automated validation checks pass and upgrade is complete (**Verify**)