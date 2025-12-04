# Quickstart — SQL Server Dependency Removal Baseline

## 1. Prerequisites

**Required:**
- Windows 11 or WSL2/Linux environment
- **.NET SDK 10.0.100** installed ([Download](https://dotnet.microsoft.com/download/dotnet/10.0))
  - Verify: `dotnet --version` → should output `10.0.100`
- **Node.js 20.x or later** for InquirySpark.Admin frontend assets ([Download](https://nodejs.org/))
  - Verify: `node --version` → should output `v20.x.x` or higher
- **Git** for source control

**Not Required (Removed):**
- ❌ SQL Server services or LocalDB
- ❌ Microsoft.EntityFrameworkCore.SqlServer package references
- ❌ SQL Server connection strings or credentials

**What's Included:**
- ✅ Pre-migrated SQLite databases in `data/sqlite/` directory
- ✅ Microsoft.Data.Sqlite native dependencies (installed with .NET runtime)
- ✅ Read-only connection string enforcement (`Mode=ReadOnly`)

---

## 2. Clone & Restore

```powershell
# Clone repository
git clone https://github.com/MarkHazleton/InquirySpark.git
cd InquirySpark

# Restore NuGet packages
dotnet restore InquirySpark.sln
```

**Expected Packages:**
- `Microsoft.EntityFrameworkCore.Sqlite 10.0.0`
- `Microsoft.Data.Sqlite 10.0.0`
- `Microsoft.AspNetCore.Identity.EntityFrameworkCore 10.0.0`

**Verify No SQL Server Packages:**
```powershell
# Should return no results
Get-ChildItem -Recurse -Filter *.csproj | Select-String "Microsoft.EntityFrameworkCore.SqlServer"
```

---

## 3. Build Solution

```powershell
# Build with Release configuration (warnings allowed during baseline)
dotnet build InquirySpark.sln --configuration Release
```

**Expected Results:**
- **Exit code:** 0 (success)
- **Errors:** 0
- **Warnings:** ~275 (allowed - see note below)
  - CS8618 (Non-nullable properties): 172 warnings
  - IDE0055 (Formatting): 44 warnings
  - CS8600-CS8604 (Nullable references): 54 warnings
  - CS1591 (XML docs): Suppressed via Directory.Build.props

**Build Configuration:**
- `Directory.Build.props` enforces:
  - `Nullable=enable` (C# 13 nullable reference types)
  - `AnalysisLevel=latest` (all .NET 10 analyzers)
  - `EnforceCodeStyleInBuild=true`
  - `TreatWarningsAsErrors=false` (warnings allowed during baseline phase)

**Build Failure Resolution:**
- If build errors occur, run: `.\eng\BuildVerification.ps1`
- See [sqlite-build-checklist.md](../../docs/copilot/session-2025-12-04/sqlite-build-checklist.md) for troubleshooting

---

## 4. Verify SQLite Configuration

### A. Check Connection Strings

**InquirySpark.Admin/appsettings.json:**
```json
{
  "ConnectionStrings": {
    "InquirySparkConnection": "Data Source=../../data/sqlite/InquirySpark.db;Mode=ReadOnly",
    "ControlSparkUserContextConnection": "Data Source=../../data/sqlite/ControlSparkUser.db;Mode=ReadOnly"
  }
}
```

**Critical Requirements:**
- ✅ Connection strings **must include** `Mode=ReadOnly`
- ✅ Paths are relative to project directory: `../../data/sqlite/*.db`
- ✅ No SQL Server connection strings present

### B. Verify Database Files

```powershell
# Check database files exist
Test-Path "data\sqlite\ControlSparkUser.db"  # Should return True
Test-Path "data\sqlite\InquirySpark.db"      # Should return True

# Check file sizes
Get-ChildItem "data\sqlite\*.db" | Select Name, @{Name="SizeKB";Expression={$_.Length/1KB}}
```

**Expected File Sizes:**
- `ControlSparkUser.db`: ~24 KB (Identity data)
- `InquirySpark.db`: ~356 KB (domain data)

**Verify Integrity:**
```powershell
# Install sqlite3 CLI (if not present)
# Windows: choco install sqlite
# Linux: sudo apt-get install sqlite3

# Check database integrity
sqlite3 data/sqlite/ControlSparkUser.db "PRAGMA integrity_check;"  # Expected: ok
sqlite3 data/sqlite/InquirySpark.db "PRAGMA integrity_check;"      # Expected: ok
```

---

## 5. Run Tests

```powershell
# Run all unit tests
dotnet test InquirySpark.sln --configuration Release --verbosity normal
```

**Expected Results:**
- **Tests:** 37/37 passed ✅
- **Projects:** InquirySpark.Common.Tests
- **Duration:** ~2-5 seconds

**Test Coverage Includes:**
- `SqliteProviderTests.cs`:
  - `Configure_RegistersSqliteProvider` - Confirms EF Core uses Sqlite
  - `Configure_EnforcesReadOnlyMode` - Validates `Mode=ReadOnly` enforcement
- Extension method tests (StringExtensions, EnumExtensions)
- BaseResponse/BaseResponseCollection validation

**Test Failure Resolution:**
- Review test output for specific failures
- Check database files are present and readable
- See [sqlite-build-checklist.md](../../docs/copilot/session-2025-12-04/sqlite-build-checklist.md)

---

## 6. Run Application & Health Check

### A. Start Admin Application

```powershell
# Navigate to Admin project
cd InquirySpark.Admin

# Run application (Kestrel)
dotnet run
```

**Expected Output:**
```
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: https://localhost:7001
info: Microsoft.Hosting.Lifetime[0]
      Application started. Press Ctrl+C to shut down.
```

**Access Application:**
- **URL:** https://localhost:7001
- **Default Page:** Home page with navigation menu
- **Health Check:** https://localhost:7001/SystemHealth

### B. System Health Verification

Navigate to the System Health dashboard:

**URL:** https://localhost:7001/SystemHealth

**Expected State:**

| **Metric** | **Expected Value** | **Status** |
|------------|-------------------|------------|
| Provider Name | `Microsoft.EntityFrameworkCore.Sqlite` | ✅ |
| Connection String | Contains `Mode=ReadOnly` | ✅ |
| File Exists | Yes | ✅ |
| Can Connect | Yes | ✅ |
| Read-Only Connection | Enforced | ✅ |
| Integrity Check | PASS | ✅ |
| Table Count | > 0 | ✅ |
| Overall Status | **Healthy** | ✅ |

**Screenshot Reference:**

The System Health page displays:
- Database connection status (Healthy badge)
- File path and size information
- Read-only enforcement confirmation
- PRAGMA integrity_check result
- Table count verification

### C. Smoke Test Application Features

1. **Test Authentication:**
   - Navigate to: https://localhost:7001/Identity/Account/Login
   - Attempt login with demo credentials (uses `ControlSparkUser.db`)

2. **Test Data Access:**
   - Navigate to: https://localhost:7001/Inquiry/Applications
   - Verify application list displays (confirms EF Core Sqlite queries work)

3. **Test Navigation:**
   - Click through menu items
   - Verify no "SQL Server" error messages appear

**Success Criteria:**
- ✅ Login functionality works
- ✅ Application list loads with data
- ✅ No database write errors (read-only enforcement working)
- ✅ No exceptions in console logs

---

## 7. Local Build Verification Script

For pre-commit validation, use the automated verification script:

```powershell
# Run full verification
.\eng\BuildVerification.ps1

# Fast iteration (skip tests during development)
.\eng\BuildVerification.ps1 -SkipTests

# Minimal check (build only)
.\eng\BuildVerification.ps1 -SkipTests -SkipNpm
```

**Script Validates:**
1. ✅ Prerequisites (.NET SDK 10.0.100, Node.js 20+)
2. ✅ SQLite database assets (file existence, size sanity checks)
3. ✅ NuGet package restore
4. ✅ Solution build (0 errors, warnings allowed)
5. ✅ Unit tests (37/37 passed)
6. ✅ NPM build (frontend assets copied to wwwroot/lib)

**Expected Output:**
```
═══════════════════════════════════════════════════════
 Verification Complete
═══════════════════════════════════════════════════════

✅ All checks passed!

✅ SQLite databases verified
✅ Solution built successfully (Release)
✅ Unit tests passed
✅ Frontend assets built

🚀 Safe to commit and push to CI
```

---

## 8. Documentation Resources

### Core Documentation

- **[README.md](../../README.md)** - Repository overview and getting started guide
- **[sqlite-build-checklist.md](../../docs/copilot/session-2025-12-04/sqlite-build-checklist.md)** - Comprehensive build verification checklist
- **[sqlite-operational-readiness.md](../../docs/copilot/session-2025-12-04/sqlite-operational-readiness.md)** - Deployment and monitoring guide
- **[sqlite-data-assets.md](../../docs/copilot/session-2025-12-04/sqlite-data-assets.md)** - Database file inventory with checksums

### Key Topics

**Before Committing:**
- [ ] Run `.\eng\BuildVerification.ps1` → exits with code 0
- [ ] Review build output for new warnings (compare with baseline)
- [ ] Verify unit tests pass (37/37 for Common.Tests)
- [ ] Confirm SQLite databases unmodified (`git status` shows no changes in `data/sqlite/`)

**Before Merging Pull Requests:**
- [ ] GitHub Actions workflow passes on all platforms (Windows + Linux)
- [ ] Test results uploaded as artifacts (no skipped tests)
- [ ] SQLite integrity checks pass
- [ ] No new CS errors introduced (warnings allowed)

**Troubleshooting:**
- **Database locked errors:** Normal with SQLite, typically self-resolving
- **Connection string errors:** Verify paths in `appsettings.json`
- **Missing database files:** Run `git checkout HEAD -- data/sqlite/*.db`
- **Build failures:** See [sqlite-build-checklist.md](../../docs/copilot/session-2025-12-04/sqlite-build-checklist.md)

---

## 9. CI/CD Integration

### GitHub Actions Workflow

**File:** `.github/workflows/sqlite-baseline.yml`

**Triggers:**
- Push to `main` or `develop` branches
- Pull requests targeting `main` or `develop`
- Manual workflow dispatch

**Jobs:**
1. **build-and-test** (Windows + Linux matrix)
   - Restores packages with NuGet cache
   - Builds solution in Release configuration
   - Runs unit tests with trx output
   - Uploads test results as artifacts

2. **admin-npm-build** (Windows + Linux matrix)
   - Installs npm dependencies with cache
   - Runs `npm run build`
   - Verifies `wwwroot/lib/` directory populated

3. **sqlite-data-integrity** (Linux only)
   - Checks database files exist
   - Validates file sizes (>10KB for ControlSpark, >100KB for InquirySpark)
   - Runs `sqlite3 PRAGMA integrity_check;`

**Monitoring:**
- GitHub Actions UI shows per-job status
- Test results available as downloadable artifacts
- Workflow fails if any job returns non-zero exit code

---

## 10. Verification Before Merge

### Final Checklist

- [ ] **Git Status Clean:**
  ```powershell
  git status
  # Should show no modifications to data/sqlite/*.db files
  ```

- [ ] **Health Endpoint Reports SQLite:**
  ```powershell
  # Navigate to https://localhost:7001/SystemHealth
  # Verify: Provider = "Microsoft.EntityFrameworkCore.Sqlite"
  # Verify: Read-Only Connection = Enforced
  # Verify: Status = "Healthy"
  ```

- [ ] **Build Succeeds:**
  ```powershell
  .\eng\BuildVerification.ps1
  # Expected: Exit code 0, all checks passed
  ```

- [ ] **Tests Pass:**
  ```powershell
  dotnet test InquirySpark.sln
  # Expected: 37/37 tests passed
  ```

- [ ] **CI Passes:**
  - GitHub Actions workflow completes successfully
  - All jobs (build-and-test, npm-build, sqlite-integrity) green
  - Test artifacts uploaded

- [ ] **Documentation Updated:**
  - README reflects SQLite-only requirements
  - Connection string examples use `Mode=ReadOnly`
  - No references to SQL Server or LocalDB

---

## 11. Next Steps

After completing this quickstart:

1. **Explore System Health Dashboard:**
   - Review database connection metrics
   - Understand read-only enforcement
   - Familiarize with health check expectations

2. **Review Operational Readiness:**
   - Read [sqlite-operational-readiness.md](../../docs/copilot/session-2025-12-04/sqlite-operational-readiness.md)
   - Understand deployment procedures
   - Learn rollback and troubleshooting steps

3. **Contribute to Project:**
   - Follow build verification checklist before commits
   - Use `.\eng\BuildVerification.ps1` for pre-commit validation
   - Monitor GitHub Actions workflow results

---

**Last Updated:** 2024-12-04  
**Feature Status:** Baseline complete, SQLite-only migration finalized  
**Support:** See [sqlite-build-checklist.md](../../docs/copilot/session-2025-12-04/sqlite-build-checklist.md) for troubleshooting