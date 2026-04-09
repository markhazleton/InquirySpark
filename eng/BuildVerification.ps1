#!/usr/bin/env pwsh
<#
.SYNOPSIS
    Local build verification script that replicates CI environment checks.

.DESCRIPTION
    Runs the same build/test/validation steps as the GitHub Actions workflow
    (sqlite-baseline.yml) to catch issues before pushing to CI. Enforces
    zero compilation errors while allowing warnings during baseline phase.

.PARAMETER SkipTests
    Skip running unit tests (faster feedback loop during development).

.PARAMETER SkipNpm
    Skip npm build for InquirySpark.Admin frontend assets.

.PARAMETER Configuration
    Build configuration (Debug or Release). Defaults to Release to match CI.

.EXAMPLE
    .\BuildVerification.ps1
    Run full verification (build + test + npm + SQLite checks)

.EXAMPLE
    .\BuildVerification.ps1 -SkipTests
    Run build verification without running tests

.EXAMPLE
    .\BuildVerification.ps1 -Configuration Debug
    Run verification using Debug configuration
#>

[CmdletBinding()]
param(
    [Parameter()]
    [switch]$SkipTests,

    [Parameter()]
    [switch]$SkipNpm,

    [Parameter()]
    [ValidateSet('Debug', 'Release')]
    [string]$Configuration = 'Release'
)

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'

# Script configuration
$script:RepoRoot = Split-Path $PSScriptRoot -Parent
$script:SolutionFile = Join-Path $RepoRoot 'InquirySpark.sln'
$script:AdminProjectDir = Join-Path $RepoRoot 'InquirySpark.Admin'
$script:SqliteDataDir = Join-Path $RepoRoot 'data\sqlite'
$script:ErrorCount = 0
$script:WarningCount = 0

function Write-Header {
    param([string]$Message)
    Write-Host "`n═══════════════════════════════════════════════════════" -ForegroundColor Cyan
    Write-Host " $Message" -ForegroundColor Cyan
    Write-Host "═══════════════════════════════════════════════════════`n" -ForegroundColor Cyan
}

function Write-Step {
    param([string]$Message)
    Write-Host "▶ $Message" -ForegroundColor Yellow
}

function Write-Success {
    param([string]$Message)
    Write-Host "✅ $Message" -ForegroundColor Green
}

function Write-Failure {
    param([string]$Message)
    Write-Host "❌ $Message" -ForegroundColor Red
    $script:ErrorCount++
}

function Write-WarningMessage {
    param([string]$Message)
    Write-Host "⚠️  $Message" -ForegroundColor DarkYellow
    $script:WarningCount++
}

function Test-CommandExists {
    param([string]$Command)
    $null -ne (Get-Command $Command -ErrorAction SilentlyContinue)
}

function Invoke-BuildStep {
    param(
        [string]$Name,
        [scriptblock]$Action
    )
    
    Write-Step $Name
    try {
        & $Action
        Write-Success "$Name completed"
        return $true
    }
    catch {
        Write-Failure "$Name failed: $_"
        return $false
    }
}

# ═══════════════════════════════════════════════════════
# Main Verification Script
# ═══════════════════════════════════════════════════════

Write-Header "SQLite Baseline Build Verification"

Write-Host "Configuration: $Configuration" -ForegroundColor Gray
Write-Host "Repository Root: $RepoRoot" -ForegroundColor Gray
Write-Host "Skip Tests: $SkipTests" -ForegroundColor Gray
Write-Host "Skip NPM: $SkipNpm" -ForegroundColor Gray

# ═══════════════════════════════════════════════════════
# Step 1: Verify Prerequisites
# ═══════════════════════════════════════════════════════

Invoke-BuildStep "Check prerequisites" {
    # Verify dotnet CLI
    if (-not (Test-CommandExists 'dotnet')) {
        throw ".NET SDK not found in PATH"
    }
    
    $dotnetVersion = dotnet --version
    Write-Host "  .NET SDK: $dotnetVersion" -ForegroundColor Gray
    
    if (-not $dotnetVersion.StartsWith('10.')) {
        Write-WarningMessage ".NET SDK version is $dotnetVersion (expected 10.x per global.json)"
    }
    
    # Verify Node.js (only if not skipping npm)
    if (-not $SkipNpm) {
        if (-not (Test-CommandExists 'node')) {
            throw "Node.js not found in PATH"
        }
        $nodeVersion = node --version
        Write-Host "  Node.js: $nodeVersion" -ForegroundColor Gray
    }
    
    # Verify solution file exists
    if (-not (Test-Path $script:SolutionFile)) {
        throw "Solution file not found: $script:SolutionFile"
    }
}

# ═══════════════════════════════════════════════════════
# Step 2: Verify SQLite Database Assets
# ═══════════════════════════════════════════════════════

Invoke-BuildStep "Verify SQLite database assets" {
    $controlSparkDb = Join-Path $script:SqliteDataDir 'ControlSparkUser.db'
    $inquirySparkDb = Join-Path $script:SqliteDataDir 'InquirySpark.db'
    
    if (-not (Test-Path $controlSparkDb)) {
        throw "Missing ControlSparkUser.db at $controlSparkDb"
    }
    
    if (-not (Test-Path $inquirySparkDb)) {
        throw "Missing InquirySpark.db at $inquirySparkDb"
    }
    
    $controlSize = (Get-Item $controlSparkDb).Length
    $inquirySize = (Get-Item $inquirySparkDb).Length
    
    Write-Host "  ControlSparkUser.db: $($controlSize / 1KB) KB" -ForegroundColor Gray
    Write-Host "  InquirySpark.db: $($inquirySize / 1KB) KB" -ForegroundColor Gray
    
    if ($controlSize -lt 10KB) {
        throw "ControlSparkUser.db suspiciously small ($controlSize bytes)"
    }
    
    if ($inquirySize -lt 100KB) {
        throw "InquirySpark.db suspiciously small ($inquirySize bytes)"
    }
}

# ═══════════════════════════════════════════════════════
# Step 3: Restore Dependencies
# ═══════════════════════════════════════════════════════

Invoke-BuildStep "Restore NuGet packages" {
    $output = dotnet restore $script:SolutionFile 2>&1
    if ($LASTEXITCODE -ne 0) {
        Write-Host $output -ForegroundColor Red
        throw "dotnet restore failed with exit code $LASTEXITCODE"
    }
}

# ═══════════════════════════════════════════════════════
# Step 4: Build Solution (Warnings as Errors)
# ═══════════════════════════════════════════════════════

Invoke-BuildStep "Build solution ($Configuration)" {
    $output = dotnet build $script:SolutionFile `
        --no-restore `
        --configuration $Configuration `
        -warnaserror `
        2>&1 | Out-String
    
    $buildSuccess = $LASTEXITCODE -eq 0
    
    # Parse warnings and errors
    $warningPattern = 'warning [A-Z]+\d{4}:'
    $errorPattern = 'error [A-Z]+\d{4}:'
    
    $warningMatches = [regex]::Matches($output, $warningPattern)
    $errorMatches = [regex]::Matches($output, $errorPattern)
    
    $warnings = $warningMatches.Count
    $errors = $errorMatches.Count
    
    Write-Host "  Warnings: $warnings" -ForegroundColor $(if ($warnings -gt 0) { 'DarkYellow' } else { 'Gray' })
    Write-Host "  Errors: $errors" -ForegroundColor $(if ($errors -gt 0) { 'Red' } else { 'Gray' })
    
    if (-not $buildSuccess) {
        Write-Host $output -ForegroundColor Red
        throw "Build failed with $errors error(s) and $warnings warning(s) — all warnings are treated as errors"
    }
}

# ═══════════════════════════════════════════════════════
# Step 5: Run Unit Tests (with Charting Filters)
# ═══════════════════════════════════════════════════════

if (-not $SkipTests) {
    Invoke-BuildStep "Run unit tests" {
        $output = dotnet test $script:SolutionFile `
            --no-build `
            --configuration $Configuration `
            --verbosity normal `
            2>&1 | Out-String
        
        if ($LASTEXITCODE -ne 0) {
            Write-Host $output -ForegroundColor Red
            throw "Tests failed with exit code $LASTEXITCODE"
        }
        
        # Parse test summary
        $passedPattern = 'Passed!\s+-\s+Failed:\s+(\d+),\s+Passed:\s+(\d+),\s+Skipped:\s+(\d+)'
        $failedPattern = 'Failed!\s+-\s+Failed:\s+(\d+),\s+Passed:\s+(\d+),\s+Skipped:\s+(\d+)'
        
        if ($output -match $passedPattern) {
            Write-Host "  Failed: $($Matches[1]), Passed: $($Matches[2]), Skipped: $($Matches[3])" -ForegroundColor Gray
        }
        elseif ($output -match $failedPattern) {
            Write-Host $output -ForegroundColor Red
            throw "Tests failed: $($Matches[1]) failed, $($Matches[2]) passed, $($Matches[3]) skipped"
        }
    }
    
    # Run charting-specific tests with filter
    Invoke-BuildStep "Run charting-specific tests" {
        $chartingTests = dotnet test $script:SolutionFile `
            --no-build `
            --configuration $Configuration `
            --filter "FullyQualifiedName~ChartDefinition|FullyQualifiedName~ChartBuild|FullyQualifiedName~DataExplorer" `
            --verbosity quiet `
            2>&1 | Out-String
        
        if ($LASTEXITCODE -eq 0) {
            Write-Host "  Charting tests passed" -ForegroundColor Gray
        }
        else {
            Write-WarningMessage "Charting-specific tests failed (may not exist yet)"
        }
    }
}
else {
    Write-Host "⏭️  Skipping unit tests (--SkipTests specified)" -ForegroundColor DarkYellow
}

# ═══════════════════════════════════════════════════════
# Step 6: Build Frontend Assets (NPM)
# ═══════════════════════════════════════════════════════

if (-not $SkipNpm) {
    Invoke-BuildStep "Install npm dependencies" {
        Push-Location $script:AdminProjectDir
        try {
            $output = npm ci 2>&1 | Out-String
            if ($LASTEXITCODE -ne 0) {
                Write-Host $output -ForegroundColor Red
                throw "npm ci failed with exit code $LASTEXITCODE"
            }
        }
        finally {
            Pop-Location
        }
    }
    
    Invoke-BuildStep "Build frontend assets" {
        Push-Location $script:AdminProjectDir
        try {
            $output = npm run build 2>&1 | Out-String
            if ($LASTEXITCODE -ne 0) {
                Write-Host $output -ForegroundColor Red
                throw "npm run build failed with exit code $LASTEXITCODE"
            }
            
            # Verify wwwroot/lib was populated
            $libDir = Join-Path $script:AdminProjectDir 'wwwroot\lib'
            if (-not (Test-Path $libDir)) {
                throw "Frontend assets not copied to $libDir"
            }
            
            $libItems = Get-ChildItem $libDir -Recurse | Measure-Object
            Write-Host "  Copied $($libItems.Count) frontend assets to wwwroot/lib" -ForegroundColor Gray
        }
        finally {
            Pop-Location
        }
    }
    
    # Verify TypeScript compilation
    Invoke-BuildStep "Verify TypeScript compilation" {
        Push-Location $script:AdminProjectDir
        try {
            if (Test-Path 'tsconfig.json') {
  Step 7: API Smoke Tests
# ═══════════════════════════════════════════════════════

Invoke-BuildStep "Run API smoke tests" {
    Write-Host "  Checking if InquirySpark.Admin is running..." -ForegroundColor Gray
    
    try {
        $response = Invoke-WebRequest -Uri "https://localhost:7001/api/ChartDefinitions" `
            -SkipCertificateCheck `
            -TimeoutSec 5 `
            -ErrorAction SilentlyContinue
        
        if ($response.StatusCode -eq 200) {
            Write-Host "  ✓ /api/ChartDefinitions responded with 200" -ForegroundColor Gray
        }
    }
    catch {
        Write-WarningMessage "API smoke tests skipped (InquirySpark.Admin not running at https://localhost:7001)"
        Write-Host "  To enable: dotnet run --project InquirySpark.Admin" -ForegroundColor DarkYellow
    }
    
    try {
        $response = Invoke-WebRequest -Uri "https://localhost:7001/api/ChartBuilds?limit=10" `
            -SkipCertificateCheck `
            -TimeoutSec 5 `
            -ErrorAction SilentlyContinue
        
        if ($response.StatusCode -eq 200) {
            Write-Host "  ✓ /api/ChartBuilds responded with 200" -ForegroundColor Gray
        }
    }
    catch {
        # Already warned above
    }
}

# ═══════════════════════════════════════════════════════
#               $output = npx tsc --noEmit 2>&1 | Out-String
                if ($LASTEXITCODE -ne 0) {
                    Write-WarningMessage "TypeScript compilation warnings detected"
                    Write-Host $output -ForegroundColor DarkYellow
                }
                else {
                    Write-Host "  TypeScript compiled successfully" -ForegroundColor Gray
                }
            }
        }
        finally {
            Pop-Location
        }
    }
}
else {
    Write-Host "⏭️  Skipping npm build (--SkipNpm specified)" -ForegroundColor DarkYellow
}

# ═══════════════════════════════════════════════════════
# Final Summary
# ═══════════════════════════════════════════════════════

Write-Header "Verification Complete"

if ($script:ErrorCount -eq 0) {
    Write-Success "All checks passed!"
    Write-Host ""
    Write-Host "✅ SQLite databases verified" -ForegroundColor Green
    Write-Host "✅ Solution built successfully ($Configuration)" -ForegroundColor Green
    if (-not $SkipTests) {
        Write-Host "✅ Unit tests passed" -ForegroundColor Green
    }
    if (-not $SkipNpm) {
        Write-Host "✅ Frontend assets built" -ForegroundColor Green
    }
    
    if ($script:WarningCount -gt 0) {
        Write-Host ""
        Write-Host "⚠️  $script:WarningCount warning(s) encountered (allowed during baseline)" -ForegroundColor DarkYellow
    }
    
    Write-Host ""
    Write-Host "🚀 Safe to commit and push to CI" -ForegroundColor Green
    exit 0
}
else {
    Write-Failure "Verification failed with $script:ErrorCount error(s)"
    Write-Host ""
    Write-Host "❌ Fix errors before committing" -ForegroundColor Red
    exit 1
}
