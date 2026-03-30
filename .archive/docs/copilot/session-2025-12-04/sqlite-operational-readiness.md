# SQLite Operational Readiness Guide

**Feature:** specs/001-remove-sql-server  
**Purpose:** Deployment verification and health monitoring for SQLite-based InquirySpark application

---

## Pre-Deployment Checklist

### ✅ Build Verification

Before deploying to any environment, ensure all build verification checks pass:

```powershell
# Run full build verification
.\eng\BuildVerification.ps1

# Verify output
# ✅ SQLite databases verified
# ✅ Solution built successfully (Release)
# ✅ Unit tests passed
# ✅ Frontend assets built
```

**Expected Results:**
- Exit code: 0
- Build warnings: ~275 (allowed during baseline)
- Build errors: 0
- Tests: 37/37 passed

**Failure Response:**
- Do not deploy if verification fails
- Review build logs for errors
- Consult [sqlite-build-checklist.md](sqlite-build-checklist.md)

---

### ✅ Database Asset Integrity

Verify immutable SQLite databases are present and uncorrupted:

```powershell
# Check database files exist
Test-Path "data\sqlite\ControlSparkUser.db"  # Should return True
Test-Path "data\sqlite\InquirySpark.db"      # Should return True

# Verify checksums (compare with baseline)
Get-FileHash "data\sqlite\*.db" -Algorithm SHA256
```

**Baseline Checksums** (from [sqlite-data-assets.md](sqlite-data-assets.md)):
- `ControlSparkUser.db`: Expected SHA256 checksum
- `InquirySpark.db`: Expected SHA256 checksum

**Failure Response:**
- Restore databases from source control: `git checkout HEAD -- data/sqlite/*.db`
- Recalculate checksums after restore
- If corruption persists, escalate to infrastructure team

---

### ✅ Configuration Validation

Ensure `appsettings.json` contains correct SQLite connection strings:

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
- Connection strings **must include** `Mode=ReadOnly`
- Paths are relative to project directory (Admin uses `../../data/sqlite/`)
- No SQL Server connection strings present

**Failure Response:**
- Correct connection strings in `appsettings.json`
- Redeploy configuration files
- Verify no SQL Server credentials in config

---

## Deployment Steps

### Step 1: Package Application

```powershell
# Publish Admin application for deployment
dotnet publish InquirySpark.Admin/InquirySpark.Admin.csproj `
    --configuration Release `
    --output .\publish\admin `
    --self-contained false `
    --runtime win-x64

# Verify SQLite databases copied to publish directory
Test-Path ".\publish\admin\data\sqlite\ControlSparkUser.db"
Test-Path ".\publish\admin\data\sqlite\InquirySpark.db"
```

**Publish Verification:**
- [ ] `InquirySpark.Admin.dll` present in output
- [ ] `wwwroot/lib/` contains Bootstrap, DataTables, jQuery assets
- [ ] `data/sqlite/*.db` files copied to publish directory
- [ ] `appsettings.json` has correct connection strings
- [ ] No `Microsoft.EntityFrameworkCore.SqlServer.dll` present

---

### Step 2: Deploy to Target Environment

**IIS Deployment (Windows Server):**

```powershell
# Copy published files to IIS webroot
Copy-Item -Path ".\publish\admin\*" -Destination "C:\inetpub\wwwroot\inquiryspark-admin" -Recurse -Force

# Set read-only permissions on SQLite databases
$dbPath = "C:\inetpub\wwwroot\inquiryspark-admin\data\sqlite"
Get-ChildItem $dbPath -Filter *.db | ForEach-Object {
    $_.IsReadOnly = $true
}

# Configure IIS Application Pool
# - .NET CLR Version: No Managed Code (for .NET 10)
# - Identity: ApplicationPoolIdentity (or custom service account)
# - Enable 32-Bit Applications: False
```

**Kestrel Deployment (Linux/Docker):**

```bash
# Copy published files
scp -r ./publish/admin/* user@server:/var/www/inquiryspark-admin/

# Set permissions
chmod +r /var/www/inquiryspark-admin/data/sqlite/*.db
chown -R www-data:www-data /var/www/inquiryspark-admin

# Start Kestrel service
systemctl start inquiryspark-admin
systemctl enable inquiryspark-admin
```

---

### Step 3: Post-Deployment Verification

#### A. Application Health Check

Navigate to the System Health dashboard:

```
https://your-domain.com/SystemHealth
```

**Expected State:**
- **InquirySpark Database Status:** Healthy ✅
- **Provider Name:** Microsoft.EntityFrameworkCore.Sqlite
- **Connection String:** Contains `Mode=ReadOnly`
- **File Exists:** Yes ✅
- **Can Connect:** Yes ✅
- **Read-Only Connection:** Enforced ✅
- **Integrity Check:** PASS ✅
- **Table Count:** > 0 ✅

**Failure Scenarios:**

| **Symptom** | **Diagnosis** | **Resolution** |
|-------------|---------------|----------------|
| **File Exists: No** | Database files not deployed | Re-run publish with `CopyToOutputDirectory=PreserveNewest` |
| **Can Connect: No** | Connection string incorrect | Verify paths in `appsettings.json`, restart application |
| **Integrity Check: FAIL** | Database corrupted | Restore from git: `git checkout HEAD -- data/sqlite/*.db` |
| **Read-Only Connection: Not Enforced** | Missing `Mode=ReadOnly` | Update connection string, redeploy config |
| **Table Count: 0** | Empty or wrong database | Verify correct `.db` files deployed (check SHA256 checksums) |

#### B. Smoke Test - Application Functionality

Verify core application features work with SQLite:

```powershell
# Test authentication
# Navigate to: https://your-domain.com/Identity/Account/Login
# Attempt login with demo credentials

# Test data access
# Navigate to: https://your-domain.com/Inquiry/Applications
# Verify application list displays (confirms EF Core Sqlite queries work)

# Test navigation
# Click through menu items to ensure no SQL Server dependencies remain
```

**Success Criteria:**
- Login succeeds (uses ControlSparkUser.db Identity tables)
- Application list loads (queries InquirySpark.db)
- No database write errors (read-only enforcement working)
- No "SQL Server" error messages in logs

---

## Health Monitoring

### Automated Health Checks

**Endpoint:** `/SystemHealth`  
**Frequency:** Every 5 minutes (configure in monitoring tool)  
**Expected Response:** HTTP 200 with "Healthy" status

**Monitoring Integration Examples:**

**Azure Monitor / Application Insights:**
```csharp
// Add health check endpoint
builder.Services.AddHealthChecks()
    .AddCheck("sqlite-database", () => {
        var canConnect = context.Database.CanConnect();
        return canConnect ? HealthCheckResult.Healthy() : HealthCheckResult.Unhealthy();
    });

app.MapHealthChecks("/health");
```

**Prometheus Metrics:**
```yaml
- job_name: 'inquiryspark-admin'
  metrics_path: '/SystemHealth'
  scrape_interval: 5m
  static_configs:
    - targets: ['yourdomain.com']
```

**Custom PowerShell Monitor:**
```powershell
# health-monitor.ps1
$healthUrl = "https://your-domain.com/SystemHealth"
$response = Invoke-WebRequest -Uri $healthUrl -UseBasicParsing

if ($response.StatusCode -eq 200 -and $response.Content -match "Healthy") {
    Write-Host "✅ Health check passed"
    exit 0
} else {
    Write-Host "❌ Health check failed"
    Send-AlertEmail -Subject "InquirySpark Health Check Failed"
    exit 1
}
```

---

### Log Monitoring

**Key Log Patterns to Alert On:**

| **Log Pattern** | **Severity** | **Action** |
|-----------------|--------------|------------|
| `SqliteException: readonly database` | ERROR | Expected behavior - confirms Mode=ReadOnly working |
| `SqliteException: database is locked` | WARNING | Multiple concurrent reads (normal under load) |
| `SqliteException: database disk image is malformed` | CRITICAL | Database corruption - restore from backup immediately |
| `Microsoft.EntityFrameworkCore.SqlServer` | CRITICAL | SQL Server dependency still present - rollback deployment |
| `attempt to write a readonly database` | INFO | Application tried to write (expected, read-only guard working) |

**Log Aggregation Query (Azure Log Analytics / Splunk):**
```kusto
AppTraces
| where TimeGenerated > ago(1h)
| where Message contains "SqliteException" or Message contains "SqlServer"
| summarize count() by Message, severityLevel
| order by count_ desc
```

---

## Rollback Procedures

### Scenario 1: Health Check Fails Post-Deployment

**Rollback Steps:**

1. **Stop application:**
   ```powershell
   # IIS
   Stop-WebSite -Name "InquirySpark-Admin"

   # Kestrel
   systemctl stop inquiryspark-admin
   ```

2. **Restore previous deployment:**
   ```powershell
   # Restore from backup directory
   Remove-Item "C:\inetpub\wwwroot\inquiryspark-admin\*" -Recurse -Force
   Copy-Item "C:\backups\inquiryspark-admin\previous\*" -Destination "C:\inetpub\wwwroot\inquiryspark-admin" -Recurse
   ```

3. **Restart application:**
   ```powershell
   # IIS
   Start-WebSite -Name "InquirySpark-Admin"

   # Kestrel
   systemctl start inquiryspark-admin
   ```

4. **Verify rollback success:**
   - Navigate to `/SystemHealth` endpoint
   - Confirm "Healthy" status returned

---

### Scenario 2: Database Corruption Detected

**Recovery Steps:**

1. **Isolate corrupted database:**
   ```powershell
   Move-Item "data\sqlite\InquirySpark.db" "data\sqlite\InquirySpark.db.corrupt.bak"
   ```

2. **Restore from source control:**
   ```powershell
   git checkout HEAD -- data/sqlite/InquirySpark.db
   ```

3. **Verify integrity:**
   ```powershell
   sqlite3 data/sqlite/InquirySpark.db "PRAGMA integrity_check;"
   # Expected output: ok
   ```

4. **Recalculate checksum:**
   ```powershell
   Get-FileHash data\sqlite\InquirySpark.db -Algorithm SHA256
   # Compare with baseline in sqlite-data-assets.md
   ```

5. **Restart application and verify health check**

---

## Maintenance Windows

### Database Refresh (Quarterly)

If SQLite databases require updates (e.g., new seed data, schema changes):

**Procedure:**

1. **Schedule maintenance window** (off-peak hours, 2-hour window)

2. **Backup current databases:**
   ```powershell
   Copy-Item "data\sqlite\*.db" "C:\backups\sqlite\$(Get-Date -Format yyyyMMdd)"
   ```

3. **Stop application:**
   ```powershell
   Stop-WebSite -Name "InquirySpark-Admin"
   ```

4. **Replace databases:**
   ```powershell
   Copy-Item "\\fileserver\inquiryspark-data\InquirySpark.db" "data\sqlite\" -Force
   Copy-Item "\\fileserver\inquiryspark-data\ControlSparkUser.db" "data\sqlite\" -Force
   ```

5. **Set read-only permissions:**
   ```powershell
   Get-ChildItem "data\sqlite\*.db" | ForEach-Object { $_.IsReadOnly = $true }
   ```

6. **Start application and verify:**
   - Check `/SystemHealth` endpoint
   - Run smoke tests
   - Review application logs

7. **Announce maintenance complete**

---

## Troubleshooting Guide

### Issue: "Database is locked" errors under load

**Symptoms:**
- `SqliteException: database is locked`
- Increased response times during concurrent requests

**Root Cause:**
- SQLite uses file-level locking, limits concurrent writes
- Read-only databases handle concurrent reads well

**Resolution:**
- **No action needed** if frequency is low (<1% of requests)
- If persistent:
  - Verify `Mode=ReadOnly` in connection string (reduces lock contention)
  - Consider connection pooling tuning (default is sufficient for most workloads)
  - For high-concurrency scenarios, consider read replicas or caching layer

---

### Issue: Application fails to start after deployment

**Symptoms:**
- HTTP 500 errors on all requests
- Application logs show "Could not load file or assembly"

**Diagnosis:**
```powershell
# Check event logs
Get-EventLog -LogName Application -Source "ASP.NET Core" -Newest 10

# Review application logs
Get-Content "C:\inetpub\wwwroot\inquiryspark-admin\logs\*.log" -Tail 50
```

**Common Causes & Resolutions:**

| **Error Message** | **Resolution** |
|-------------------|----------------|
| `Could not load Microsoft.Data.Sqlite.dll` | Re-publish with `--self-contained false`, verify .NET 10 runtime installed |
| `Unable to open the database file` | Check connection string paths, verify database files deployed |
| `Access to the path is denied` | Grant IIS Application Pool identity read permissions to `data/sqlite/` folder |

---

## Performance Baselines

### Expected Performance Characteristics (SQLite vs SQL Server)

| **Metric** | **SQLite (Read-Only)** | **SQL Server** | **Notes** |
|------------|------------------------|----------------|-----------|
| **Concurrent Reads** | 100+ req/sec | 500+ req/sec | SQLite sufficient for admin workloads |
| **Query Latency (p50)** | 5-15ms | 3-10ms | Comparable for simple queries |
| **File Size** | 356 KB | 10+ MB | SQLite significantly smaller |
| **Memory Footprint** | ~50 MB | ~200 MB | SQLite reduces server memory requirements |
| **Cold Start Time** | <1 sec | 3-5 sec | SQLite faster (no network handshake) |

**Monitoring Recommendations:**
- Set alerting threshold for query latency > 100ms (p95)
- Monitor file system I/O for database reads
- Track connection pool exhaustion (should not occur with read-only)

---

## Contact & Escalation

### Support Tiers

**Tier 1 - Application Monitoring:**
- Check `/SystemHealth` endpoint for database status
- Review application logs for SQLite-specific errors
- Restart application if health check fails

**Tier 2 - Database Issues:**
- Restore corrupted databases from source control
- Verify database file integrity with `sqlite3 PRAGMA integrity_check`
- Escalate if checksums don't match baseline

**Tier 3 - Infrastructure/Architecture:**
- SQLite performance issues under high load
- Consideration of alternative database providers
- Schema changes or database migrations

### Emergency Contacts

- **On-Call Engineer:** [Your Team's On-Call Rotation]
- **Database Admin (Tier 2):** [DBA Contact]
- **Infrastructure Lead (Tier 3):** [Infrastructure Contact]

---

**Last Updated:** 2024-12-04  
**Document Owner:** Platform Engineering Team  
**Related Docs:**
- [sqlite-build-checklist.md](sqlite-build-checklist.md)
- [sqlite-data-assets.md](sqlite-data-assets.md)
- [quickstart.md](../quickstart.md)
