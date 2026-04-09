# SQLite Baseline Build Verification Checklist

**Feature:** specs/001-remove-sql-server  
**User Story:** US2 - Build Quality Baseline  
**Phase 4 Completed:** April 7, 2026  
**Purpose:** Establish reproducible build/test workflow with SQLite-only dependencies and zero-warning enforcement

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
- File sizes within expected ranges

**Failure Scenarios:**
- ❌ Missing database files → Re-clone repository or restore from backup
- ❌ Corrupted databases → Checksum mismatch (see `.archive/docs/copilot/session-2025-12-04/sqlite-data-assets.md`)

---

### Step 2: NuGet Package Restore

**Validates:** All .NET dependencies download successfully

```powershell
dotnet restore InquirySpark.sln
```

**Expected Outcome:**
- Exit code 0
- No restore errors

**Failure Scenarios:**
- ❌ Network issues → Check firewall/proxy settings
- ❌ NuGet authentication → Configure NuGet credentials if using private feeds

---

### Step 3: Solution Build (Warnings as Errors)

**Validates:** Code compiles with zero errors AND zero warnings

```powershell
dotnet build InquirySpark.sln --no-restore --configuration Release -warnaserror
```

**Expected Outcome:**
- **Exit code 0** (build success)
- **Errors: 0**
- **Warnings: 0** (all warnings are treated as errors)

**Build Configuration (`Directory.Build.props`):**
- `TreatWarningsAsErrors=true` — any warning fails the build
- `Nullable=enable` — C# 13 nullable reference types enforced
- `AnalysisLevel=latest` — all .NET 10 analyzers active
- `EnforceCodeStyleInBuild=true` — IDE diagnostics run in CI
- `NoWarn=CS1591` — XML doc warnings suppressed (public API docs enforced selectively)

**`.editorconfig` key rules (warning severity):**
- `dotnet_style_null_propagation` → `:warning`
- `dotnet_style_coalesce_expression` → `:warning`
- `csharp_style_throw_expression` → `:warning`
- `CS8600–CS8629` nullable diagnostics → `:warning`
- Naming conventions: interfaces (`I` prefix), types, members → `:warning`

**Failure Scenarios:**
- ❌ Any warning → Fix the flagged code; do not suppress without team agreement
- ❌ Compilation errors → Fix syntax errors or missing dependencies

---

### Step 4: Unit Test Execution

**Validates:** All tests pass with SQLite provider

```powershell
dotnet test InquirySpark.sln --no-build --configuration Release --verbosity normal
```

**Expected Outcome:**
- **Exit code 0** (all tests passed)
- `InquirySpark.Common.Tests` passes including `SqliteProviderTests`

**Failure Scenarios:**
- ❌ Test failures → Review test output, check for breaking changes in EF Core Sqlite
- ❌ Missing test project → Ensure `InquirySpark.Common.Tests.csproj` is in solution

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
- `wwwroot/lib/` populated with required assets

**Failure Scenarios:**
- ❌ npm not found → Install Node.js 20+ LTS
- ❌ Package install failures → Delete `node_modules/` and `package-lock.json`, run `npm install`

---

## CI/CD Integration

### GitHub Actions Workflow

**File:** `.github/workflows/sqlite-baseline.yml`

**Jobs:**
1. **build-and-test** (matrix: windows-latest, ubuntu-latest)
   - Restores packages
   - Builds with `-warnaserror` (zero warnings required)
   - Runs unit tests with trx output
   - Uploads test results as artifacts

2. **admin-npm-build** (matrix: windows-latest, ubuntu-latest)
   - Installs npm dependencies
   - Runs `npm run build`
   - Verifies `wwwroot/lib/` assets

3. **sqlite-data-integrity** (ubuntu-latest only)
   - Checks database files exist and are not corrupted
   - Runs `sqlite3 PRAGMA integrity_check;`

4. **build-summary** (always runs)
   - Aggregates results, posts summary

---

## Phase 4 Completion Criteria

- [x] `Directory.Build.props` has `TreatWarningsAsErrors=true`
- [x] `.editorconfig` elevates nullable and null-safety rules to `:warning`
- [x] `.github/workflows/sqlite-baseline.yml` uses `-warnaserror`
- [x] `eng/BuildVerification.ps1` uses `-warnaserror` and fails on any warning
- [x] Build passes with zero errors and zero warnings
