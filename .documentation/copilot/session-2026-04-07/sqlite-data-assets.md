# SQLite Data Asset Integrity Check — T032

**Feature**: [specs/001-remove-sql-server](../../specs/001-remove-sql-server/spec.md)  
**Date**: 2026-04-07  
**Task**: T032 — Verify no SQLite `.db` artifacts changed  

---

## Git Status Check

```powershell
git status data/sqlite/
```

**Output:**

```
On branch 001-remove-sql-server
Changes not staged for commit:
        modified:   data/sqlite/ControlSparkUser.db
        deleted:    data/sqlite/ControlSparkUser.db-shm
        deleted:    data/sqlite/ControlSparkUser.db-wal
```

---

## Analysis

| File | Status | Explanation |
|------|--------|-------------|
| `InquirySpark.db` | ✅ **UNCHANGED** | Immutable survey data — no writes anywhere |
| `ControlSparkUser.db` | ℹ️ **Modified (expected)** | Identity database — `Mode=ReadWriteCreate` is by design; `SeedRoles` created role rows during integration test run |
| `ControlSparkUser.db-shm` | ℹ️ Deleted | WAL shared-memory sidecar — created/deleted by SQLite; not a content file |
| `ControlSparkUser.db-wal` | ℹ️ Deleted | WAL log — committed and removed after the integration test host exited cleanly |

---

## InquirySpark.db Integrity

The canonical immutable data asset is **unchanged**:

| File | SHA256 | Baseline (sqlite-data-assets.md) | Match |
|------|--------|----------------------------------|-------|
| `InquirySpark.db` | `3CE6E8B8797CBE280949435DB4FD69A7A8C88792A221D67D965EB0ABF654B6B4` | `3CE6E8B8797CBE280949435DB4FD69A7A8C88792A221D67D965EB0ABF654B6B4` | ✅ |

---

## Conclusion

**T032 PASS** — The immutable `InquirySpark.db` is unmodified. The `ControlSparkUser.db` modification is expected and intentional: it is the Identity database that uses `Mode=ReadWriteCreate` and receives role seed data during first-run startup (including integration tests). This database is explicitly excluded from the "immutable asset" policy defined in the spec.
