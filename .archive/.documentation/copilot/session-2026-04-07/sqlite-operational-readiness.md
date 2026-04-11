# SQLite Operational Readiness — Verification Steps

**Feature**: [specs/001-remove-sql-server](../../specs/001-remove-sql-server/spec.md)  
**Date**: 2026-04-07  
**Status**: ✅ COMPLETE  

---

## Overview

This runbook replaces all SQL Server health-verification steps with equivalent SQLite procedures. Operators and QA engineers should follow these steps when deploying or verifying the InquirySpark.Admin host.

---

## 1. Pre-Deployment Checklist

| Check | Expected | Command / Action |
|-------|----------|-----------------|
| SQLite `.db` files present | Files exist in `data/sqlite/` | `Get-ChildItem data/sqlite/*.db` |
| No SQL Server connection strings | Zero results | `Select-String -Recurse -Path *.json -Pattern "sqlserver\|Data Source=.*;Initial Catalog" -CaseSensitive:$false` |
| Build succeeds without warnings | Exit code 0 | `eng/BuildVerification.ps1` |

---

## 2. Health-Endpoint Smoke Tests

After starting the Admin host, verify each endpoint:

### GET `/api/system/health`

```http
GET https://<host>/api/system/health
```

**Expected 200 response:**

```json
{
  "status": "Healthy",
  "provider": {
    "name": "Sqlite",
    "connectionString": "Data Source=.../InquirySpark.db;Mode=ReadOnly",
    "readOnly": true
  },
  "buildVersion": "10.xxxx.xxxx.xxxx",
  "diagnostics": ["Database connection succeeded."]
}
```

**Failure signals:**
- `status: "Unhealthy"` → SQLite file is missing or unreadable. Re-deploy `data/sqlite/InquirySpark.db`.
- `readOnly: false` → Connection string misconfiguration. Confirm `Mode=ReadOnly` in `appsettings.json`.
- HTTP 503 → Application failed to start. Check host logs.

---

### GET `/api/system/database/state`

```http
GET https://<host>/api/system/database/state
```

**Expected 200 response:**

```json
{
  "filePath": "/app/data/sqlite/InquirySpark.db",
  "lastWriteUtc": "2025-12-04T00:00:00Z",
  "checksum": "<sha256-hex>",
  "writable": false
}
```

**Failure signals:**
- HTTP 409 → Write access detected (`Mode=ReadOnly` missing). Check `appsettings.json`.
- HTTP 503 → `.db` file not found at the expected path.
- `writable: true` → Configuration error; must not happen on any production host.

---

## 3. Integrity Verification (PowerShell)

Run this block to verify the `.db` checksum has not changed since baseline was captured:

```powershell
$dbPath = "data/sqlite/InquirySpark.db"
$baseline = "<sha256-from-sqlite-data-assets.md>"

$actual = (Get-FileHash -Path $dbPath -Algorithm SHA256).Hash.ToLower()

if ($actual -eq $baseline) {
    Write-Host "PASS: SQLite asset is unmodified." -ForegroundColor Green
} else {
    Write-Error "FAIL: Checksum mismatch — database may have been written to."
}
```

---

## 4. Admin UI Verification

1. Navigate to `/SystemHealth` in the Admin application.
2. The **InquirySpark Database** card should show:
   - Provider: `Microsoft.EntityFrameworkCore.Sqlite`
   - Status: `Healthy`
   - IntegrityCheck: `PASS`
   - IsReadOnlyConnection: `✓ Yes`
3. The **Provider Status** partial (on the dashboard) should display `Healthy` with `Read-Only: Yes`.

---

## 5. Troubleshooting Quick Reference

| Symptom | Likely Cause | Resolution |
|---------|-------------|-----------|
| 503 on `/api/system/health` | SQLite file missing | Re-copy `data/sqlite/InquirySpark.db` to deployment path |
| 409 on `/api/system/database/state` | Missing `Mode=ReadOnly` in connection string | Update `appsettings.json` |
| `CanConnect = false` | Corrupt or locked file | Verify file integrity; re-deploy from source |
| Build warning `CS0436` in tests | Admin `Program` symbol conflict | Ensure only one `public partial class Program` |
| `EF1001` warning in tests | EF internal API usage | Suppressed via `<NoWarn>` in test csproj |

---

## 6. Connection String Reference

**Admin host** (`appsettings.json`):

```json
"ConnectionStrings": {
  "InquirySparkConnection": "Data Source=../data/sqlite/InquirySpark.db;Mode=ReadOnly",
  "ControlSparkUserContextConnection": "Data Source=../data/sqlite/ControlSparkUser.db;Mode=ReadWriteCreate"
}
```

> `ControlSparkUser.db` intentionally uses `ReadWriteCreate` for Identity operations (user accounts, roles). Only `InquirySpark.db` is immutable read-only.
