# Quickstart — SQL Server Dependency Removal Baseline

## 1. Prerequisites
- Windows 11 or WSL2 environment with .NET SDK 10.0.100 installed
- Node.js 20.x for InquirySpark.Admin asset pipeline (`npm run build` triggered automatically)
- No SQL Server services or LocalDB required; ensure Microsoft.Data.Sqlite native dependencies are available (installed with .NET runtime)
- Pre-migrated SQLite databases checked into `InquirySpark.Admin/wwwroot/_content/data/` (exact paths documented in future tasks)

## 2. Clone & Restore
```powershell
git clone https://github.com/MarkHazleton/InquirySpark.git
cd InquirySpark
dotnet restore InquirySpark.sln
```

## 3. Build with Warnings as Errors
```powershell
dotnet build InquirySpark.sln -warnaserror
```
- Directory.Build.props (added in this feature) enables nullable + analyzer enforcement.
- Any SQL Server package reference should fail the build; remove before proceeding.

## 4. Configure SQLite Provider
1. Ensure `appsettings.Development.json` for Web/WebApi/Admin points to the immutable `.db` files (e.g., `"Data Source=./data/inquiryspark.db;Mode=ReadOnly"`).
2. Do **not** enable EF Core migrations or schema creation.
3. Verify health endpoints:
   - WebApi: `https://localhost:5001/api/system/health`
   - Admin/Web: leverage equivalent diagnostics views (to be wired during implementation).

## 5. Run Tests
```powershell
dotnet test InquirySpark.sln
```
- MSTest covers shared libraries.
- Add smoke/integration tests to ensure SQLite providers initialize for every host.

## 6. Documentation Update Checklist
- README “Getting Started” section → remove SQL Server prerequisites, highlight SQLite steps.
- Deployment/run-book docs → specify how to package the `.db` files and verify read-only mounting.
- Troubleshooting guide → include common SQLite pitfalls (file locking, path issues, missing native dependencies).

## 7. Verification Before Merge
- `git status` clean, no `.db` modifications present.
- Health endpoints report `provider.name = "Sqlite"` and `readOnly = true`.
- CI build succeeds on first attempt with `dotnet build -warnaserror` and `dotnet test`.
