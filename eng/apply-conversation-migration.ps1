# apply-conversation-migration.ps1
# Applies the AddConversationApi schema changes to a writable SQLite working copy.
# Run from the repository root: .\eng\apply-conversation-migration.ps1
#
# This script:
#   1. Creates a writable working copy of the seed database
#   2. Applies the two new ALTER TABLE columns via the companion Python script
#   3. Validates the schema changes

[CmdletBinding()]
param(
    [string]$DbPath = "data\sqlite\InquirySpark.db",
    [string]$WorkingCopy = "data\sqlite\conversation-dev.db"
)

$ErrorActionPreference = "Stop"
$repoRoot = Split-Path -Parent $PSScriptRoot

Push-Location $repoRoot
try {
    $sourceDb = Join-Path $repoRoot $DbPath
    $workDb   = Join-Path $repoRoot $WorkingCopy

    if (-not (Test-Path $sourceDb)) {
        Write-Error "Source database not found: $sourceDb"
        exit 1
    }

    Write-Host "Copying $sourceDb -> $workDb ..."
    Copy-Item $sourceDb $workDb -Force

    Write-Host "Applying schema changes to $workDb ..."

    $pyScript = Join-Path $repoRoot "eng\apply_migration.py"
    python $pyScript $workDb

    if ($LASTEXITCODE -ne 0) {
        Write-Error "Migration script failed (exit $LASTEXITCODE)."
        exit 1
    }

    Write-Host ""
    Write-Host "Working copy at: $workDb"
    Write-Host "To use in development, ensure appsettings.Development.json contains:"
    Write-Host "  InquirySparkConnection: Data Source=../../$WorkingCopy;Mode=ReadWriteCreate"
}
finally {
    Pop-Location
}
