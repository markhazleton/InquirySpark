# SQLite Baseline Build Verification Checklist

**Feature:** specs/001-remove-sql-server  
**User Story:** US2 - Build Quality Baseline  
**Purpose:** Establish reproducible build/test workflow with SQLite-only dependencies

---

## Quick Start

### Local Verification (Pre-Commit)

```powershell
# Full verification (recommended before committing)
.\eng\BuildVerification.ps1

# Fast iteration (skip tests during development)
.\eng\BuildVerification.ps1 -SkipTests

# Minimal check (build only)
.\eng\BuildVerification.ps1 -SkipTests -SkipNpm
```

### CI Verification (GitHub Actions)

Workflow: `.github/workflows/sqlite-baseline.yml`  
Triggers: Push/PR to `main` or `develop`, manual dispatch  
Platforms: Windows + Linux (parallel execution)

---

## Prerequisites Checklist

### ✅ Local Development Environment

- [ ] **.NET SDK 10.0.100** installed and available in PATH
  - Verify: `dotnet --version` → should output `10.0.100`
  - Enforced by: `global.json` in repository root
  - Download: [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)

- [ ] **Node.js 20.x or later** installed (for Admin frontend assets)
  - Verify: `node --version` → should output `v20.x.x` or higher
  - Required for: Bootstrap 5, DataTables, jQuery npm packages
  - Download: [Node.js LTS](https://nodejs.org/)

- [ ] **PowerShell 7.x** (for BuildVerification.ps1 script)
  - Verify: `$PSVersionTable.PSVersion` → should be 7.x
  - Default on Windows 10+, install separately for Linux/macOS

- [ ] **Git** configured for line endings (prevents cross-platform issues)
  - Windows: `git config --global core.autocrlf true`
  - Linux/macOS: `git config --global core.autocrlf input`

### ✅ Repository Setup

- [ ] **SQLite database assets** present in `data/sqlite/`
  - `ControlSparkUser.db` (24 KB - Identity data)
  - `InquirySpark.db` (356 KB - domain data)
  - Both files are **immutable** and **read-only** at runtime

- [ ] **No SQL Server dependencies** remaining
  - All connection strings use SQLite (`Data Source=...;Mode=ReadOnly`)
  - No `Microsoft.EntityFrameworkCore.SqlServer` package references
  - `InquirySpark.Database.sqlproj` unloaded from solution

---

## Build Verification Steps

### Step 1: SQLite Database Integrity

**Validates:** Immutable database files exist and are not corrupted

```powershell
# Manual validation (PowerShell)
Test-Path "data\sqlite\ControlSparkUser.db"  # Should return True
Test-Path "data\sqlite\InquirySpark.db"      # Should return True

# Check file sizes (sanity check)
(Get-Item "data\sqlite\ControlSparkUser.db").Length / 1KB  # ~24 KB
(Get-Item "data\sqlite\InquirySpark.db").Length / 1KB      # ~356 KB
```

**Expected Outcome:**
- Both database files exist
- File sizes within expected ranges (not 0 bytes or suspiciously large)
- CI validates with `sqlite3 PRAGMA integrity_check;` on Linux

**Failure Scenarios:**
- ❌ Missing database files → Re-clone repository or restore from backup
- ❌ Corrupted databases → Checksum mismatch (see `docs/copilot/session-2025-12-04/sqlite-data-assets.md`)

---

### Step 2: NuGet Package Restore

**Validates:** All .NET dependencies download successfully

```powershell
dotnet restore InquirySpark.sln
```

**Expected Outcome:**
- Exit code 0
- No restore errors (authentication, network, or package not found issues)
- Restores ~30 packages including:
  - `Microsoft.EntityFrameworkCore.Sqlite 10.0.0`
  - `Microsoft.Data.Sqlite 10.0.0`
  - `Microsoft.AspNetCore.Identity.EntityFrameworkCore 10.0.0`
  - `MSTest.TestFramework 3.8.1`

**Failure Scenarios:**
- ❌ Network issues → Check firewall/proxy settings
- ❌ NuGet authentication → Configure NuGet credentials if using private feeds
- ❌ Package version conflicts → Check `Directory.Packages.props` or csproj files

---

### Step 3: Solution Build (Allow Warnings)

**Validates:** Code compiles successfully with zero errors

```powershell
dotnet build InquirySpark.sln --no-restore --configuration Release
```

**Expected Outcome:**
- **Exit code 0** (build success)
- **Errors: 0** (no compilation failures)
- **Warnings: ~275** (allowed during SQLite baseline phase)
  - CS8618 (Non-nullable properties): 172 warnings
  - IDE0055 (Formatting): 44 warnings
  - CS8600-CS8604 (Nullable references): 54 warnings
  - CS1591 (XML docs): Suppressed via `<NoWarn>CS1591</NoWarn>` in Directory.Build.props

**Build Configuration:**
- `Directory.Build.props` enforces:
  - `Nullable=enable` (C# 13 nullable reference types)
  - `AnalysisLevel=latest` (all .NET 10 analyzers)
  - `EnforceCodeStyleInBuild=true` (IDE diagnostics in CI)
  - `TreatWarningsAsErrors=false` (warnings allowed during baseline)
- `.editorconfig` provides code style rules (formatting, naming conventions)

**Failure Scenarios:**
- ❌ Compilation errors (CS*) → Fix syntax errors or missing dependencies
- ❌ Exit code ≠ 0 → Check build output for stack traces or fatal errors
- ❌ New errors introduced → Review recent code changes, revert if necessary

---

### Step 4: Unit Test Execution

**Validates:** All tests pass with SQLite provider

```powershell
dotnet test InquirySpark.sln --no-build --configuration Release --verbosity normal
```

**Expected Outcome:**
- **Exit code 0** (all tests passed)
- **37 tests passed** (InquirySpark.Common.Tests)
  - Extension method tests (StringExtensions, EnumExtensions)
  - BaseResponse/BaseResponseCollection validation
  - **SqliteProviderTests** (T010) - validates SqliteOptionsConfigurator

**Test Coverage:**
- `InquirySpark.Common.Tests/Providers/SqliteProviderTests.cs`
  - `Configure_RegistersSqliteProvider` - confirms UseSqlite registration
  - `Configure_EnforcesReadOnlyMode` - validates `Mode=ReadOnly` in connection string

**Failure Scenarios:**
- ❌ Test failures → Review test output, check for breaking changes in EF Core Sqlite
- ❌ Missing test project → Ensure `InquirySpark.Common.Tests.csproj` is in solution
- ❌ MSTest configuration issues → Check `GlobalUsings.cs` for MSTest attributes

---

### Step 5: NPM Build (Frontend Assets)

**Validates:** Bootstrap 5, DataTables, and jQuery assets copied to `wwwroot/lib`

```powershell
cd InquirySpark.Admin
npm ci          # Clean install (respects package-lock.json)
npm run build   # Runs scripts/copy-assets.js
```

**Expected Outcome:**
- **Exit code 0** for both commands
- `wwwroot/lib/` populated with 56+ files:
  - `jquery/` (3.7.1)
  - `bootstrap/js/` (5.3.8)
  - `datatables.net/` (2.3.5) + extensions (Buttons, Responsive, SearchPanes)
  - `bootstrap-icons/` (1.13.1)
  - `jquery-validation/` + `jquery-validation-unobtrusive/`
  - `jszip/`, `pdfmake/` (DataTables export dependencies)

**Build Integration:**
- `InquirySpark.Admin.csproj` includes `<Target Name="NpmBuild" BeforeTargets="Build">`
- Automatically runs `npm install && npm run build` during `dotnet build`
- CI validates assets exist and are not empty

**Failure Scenarios:**
- ❌ npm not found → Install Node.js 20+ LTS
- ❌ Package install failures → Delete `node_modules/` and `package-lock.json`, run `npm install`
- ❌ Copy script errors → Check `scripts/copy-assets.js` syntax, verify `wwwroot/lib/` is writable

---

## CI/CD Integration

### GitHub Actions Workflow

**File:** `.github/workflows/sqlite-baseline.yml`

**Jobs:**
1. **build-and-test** (matrix: windows-latest, ubuntu-latest)
   - Restores packages with NuGet cache
   - Builds solution in Release configuration
   - Runs unit tests with trx output
   - Uploads test results as artifacts

2. **admin-npm-build** (matrix: windows-latest, ubuntu-latest)
   - Installs npm dependencies with npm cache
   - Runs `npm run build`
   - Verifies `wwwroot/lib/` directory exists

3. **sqlite-data-integrity** (ubuntu-latest only)
   - Checks database files exist
   - Validates file sizes (>10KB for ControlSpark, >100KB for InquirySpark)
   - Runs `sqlite3 PRAGMA integrity_check;` for corruption detection

4. **build-summary** (always runs)
   - Aggregates results from all jobs
   - Posts summary with pass/fail status

**Triggering CI:**
- Push to `main` or `develop` branches
- Pull requests targeting `main` or `develop`
- Manual workflow dispatch (Actions tab → Run workflow)

**Monitoring:**
- GitHub Actions UI shows per-job status
- Test results available as downloadable artifacts (`test-results-*.trx`)
- Workflow fails if any job returns non-zero exit code

---

## Known Issues and Workarounds

### Issue: 973 XML Documentation Warnings (CS1591)

**Description:** All public APIs missing XML doc comments generate CS1591 warnings.

**Status:** Deferred to post-baseline cleanup phase (not blocking SQLite migration)

**Workaround:**
- Suppressed via `<NoWarn>$(NoWarn);CS1591</NoWarn>` in Directory.Build.props
- To enforce XML docs in future: Remove CS1591 from NoWarn, add `/// <summary>` to all public members

### Issue: IDE0055 Formatting Warnings (44 occurrences)

**Description:** Whitespace/indentation inconsistencies detected by .editorconfig rules.

**Status:** Informational, does not affect runtime behavior

**Workaround:**
- Run `dotnet format InquirySpark.sln` to auto-fix formatting issues
- Review `.editorconfig` rules if formatting conflicts with team style

### Issue: Nullable Reference Type Warnings (CS8600-CS8618, 226 occurrences)

**Description:** SDK models and MVC views have nullable annotation gaps.

**Status:** Expected with `Nullable=enable` enforcement, non-critical

**Workaround:**
- Phase 5 will add `required` modifiers to SDK models (e.g., `QuestionItem.QuestionNM`)
- Views (`.cshtml`) often have false positives due to Razor syntax

### Issue: EF1001 Warnings (Internal API Usage)

**Description:** `SqliteProviderTests.cs` references EF Core internal types.

**Status:** Acceptable for test code, suppressed in test project only

**Workaround:**
- Add `<NoWarn>$(NoWarn);EF1001</NoWarn>` to `InquirySpark.Common.Tests.csproj` if warnings escalate

### Issue: NU1903 Security Vulnerability (Microsoft.Build 17.11.4)

**Description:** Transitive dependency with known high severity vulnerability.

**Status:** WebSpark.Bootswatch dependency, no direct control

**Workaround:**
- Monitor for WebSpark package updates
- Consider replacing with direct package references if vulnerability affects production

---

## Rollback Plan

If build verification fails after code changes, follow these steps:

### 1. Isolate the Failure

```powershell
# Run verification with verbose output
.\eng\BuildVerification.ps1 -Configuration Debug

# Review build warnings/errors
dotnet build InquirySpark.sln > build.log 2>&1

# Run tests individually
dotnet test InquirySpark.Common.Tests --filter FullyQualifiedName~SqliteProviderTests
```

### 2. Compare with Last Known Good State

```powershell
# Check git diff for recent changes
git diff HEAD~1 HEAD -- InquirySpark.Repository/

# Review build logs from CI (GitHub Actions)
gh run view --log <run_id>
```

### 3. Revert Changes if Necessary

```powershell
# Revert specific commit
git revert <commit_hash>

# Reset to last passing commit
git reset --hard origin/main
```

### 4. Restore SQLite Databases (if corrupted)

```powershell
# Re-checkout database files from git
git checkout HEAD -- data/sqlite/ControlSparkUser.db
git checkout HEAD -- data/sqlite/InquirySpark.db

# Verify checksums match documentation
Get-FileHash data\sqlite\*.db -Algorithm SHA256
```

---

## Maintenance and Future Work

### Phase 5: Code Quality Improvements (Post-Baseline)

Tracked in `specs/001-remove-sql-server/tasks.md` (T024-T030):

- [ ] **T024:** Add `required` modifiers to SDK model properties (fix CS8618 warnings)
- [ ] **T025:** Remove `nullable disable` pragmas from legacy code
- [ ] **T026:** Run `dotnet format` and commit formatting fixes
- [ ] **T027:** Enable `TreatWarningsAsErrors=true` in Directory.Build.props
- [ ] **T028:** Add XML documentation to public APIs (remove CS1591 suppression)
- [ ] **T029:** Configure SonarQube/CodeQL for static analysis
- [ ] **T030:** Establish code coverage baseline (≥80% target)

### Updating Build Configuration

When modifying build behavior:

1. **Update all three files together:**
   - `Directory.Build.props` (MSBuild properties)
   - `.editorconfig` (IDE/analyzer rules)
   - `.github/workflows/sqlite-baseline.yml` (CI enforcement)

2. **Test locally before committing:**
   ```powershell
   .\eng\BuildVerification.ps1  # Must pass with 0 errors
   ```

3. **Document breaking changes:**
   - Update this checklist if new prerequisites added
   - Notify team in PR description or Slack/Teams

### CI Performance Optimization

Current workflow takes ~5-10 minutes. Potential improvements:

- [ ] Enable NuGet package caching (already implemented via `actions/cache`)
- [ ] Enable npm dependency caching (already implemented)
- [ ] Parallelize test projects (use `dotnet test --parallel`)
- [ ] Skip npm build on non-Admin changes (use `paths` filter in workflow)

---

## Support and Troubleshooting

### Common Errors

**Error:** `dotnet: command not found`  
**Solution:** Install .NET SDK 10.0.100, restart terminal

**Error:** `node: command not found`  
**Solution:** Install Node.js 20+ LTS, restart terminal

**Error:** `The type or namespace name 'Sqlite' does not exist`  
**Solution:** Run `dotnet restore` to fetch Microsoft.EntityFrameworkCore.Sqlite package

**Error:** `Access denied writing to wwwroot/lib`  
**Solution:** Check folder permissions, run PowerShell as Administrator (Windows)

**Error:** `Database file is locked`  
**Solution:** Ensure no applications have `*.db` files open, restart VS Code

### Reporting Issues

If build verification fails unexpectedly:

1. **Collect diagnostics:**
   ```powershell
   .\eng\BuildVerification.ps1 > verification.log 2>&1
   dotnet --info > dotnet-info.txt
   node --version > node-version.txt
   ```

2. **Open GitHub issue with:**
   - Operating system (Windows 10/11, Ubuntu 22.04, macOS Ventura)
   - .NET SDK version (`dotnet --version`)
   - Error message excerpt (first 20 lines of failure)
   - Attached log files

3. **Tag with labels:**
   - `build-failure` (compilation errors)
   - `test-failure` (failing unit tests)
   - `ci/cd` (GitHub Actions issues)

---

## Checklist Summary

**Before Every Commit:**
- [ ] Run `.\eng\BuildVerification.ps1` → exits with code 0
- [ ] Review build output for new warnings (compare with baseline)
- [ ] Verify unit tests pass (37/37 for Common.Tests)
- [ ] Confirm SQLite databases unmodified (git status shows no changes in `data/sqlite/`)

**Before Merging Pull Requests:**
- [ ] GitHub Actions workflow passes on all platforms (Windows + Linux)
- [ ] Test results uploaded as artifacts (no skipped tests)
- [ ] SQLite integrity checks pass
- [ ] No new CS errors introduced (warnings allowed)

**After Major Dependency Updates:**
- [ ] Re-run full verification suite
- [ ] Update `global.json` if .NET SDK version changes
- [ ] Update `package.json` and `package-lock.json` if npm packages change
- [ ] Test on both Windows and Linux (use GitHub Actions or local VMs)

---

**Last Updated:** 2024-12-04  
**Feature Status:** Phase 4 complete (T019-T023) ✅  
**Next Phase:** Phase 5 - Code quality improvements (T024-T030)
