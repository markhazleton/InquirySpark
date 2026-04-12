# Repository Story: InquirySpark

> Generated 2026-04-12 | Window: 12 months | Scope: full

## Executive Summary
InquirySpark is a .NET-based survey and inquiry platform centered on an admin web application, shared domain/repository libraries, and a test project. The current codebase and commit history show a product moving through major platform transitions while still shipping feature work, most recently the HATEOAS conversation API merged on 2026-04-12.

The repository shows meaningful delivery scale in the active window: 63 commits in the past 12 months (73 total in repository history), 3 contributors, and sustained activity from 2025-12 through 2026-04. The project age is substantial (first commit 2023-10-24), and latest activity is current (latest commit 2026-04-12), indicating an actively maintained codebase.

Velocity is concentrated in three recent periods: 24 commits in 2025-12, 9 in 2026-03, and 30 in 2026-04. This pattern reflects delivery waves: major platform and governance updates, then feature implementation and merge hardening.

Governance posture appears process-aware but still maturing in release hygiene. The history includes 3 merged PR commits in the 12-month window, a present constitution file, and explicit documentation/governance command usage. At the same time, there are no git tags, which limits release traceability by version.

Major delivery evidence includes large integration merges such as SQL Server-to-SQLite migration (88,132 total changed lines in one merge commit) and the recent conversation API integration (30,376 total changed lines in one merge commit).

## Technical Analysis

### Development Velocity
The 12-month window contains 63 commits, with monthly distribution of 24 (2025-12), 9 (2026-03), and 30 (2026-04). This indicates cyclical delivery with a strong recent acceleration into April.

Repository volume signals show broad engineering activity: 446 touched C# files and 382 touched Markdown files in the period, alongside 88 cshtml files and 85 PowerShell files. Large merges dominate structural change, including 193 files changed for the SQLite migration baseline and 133 files changed for the conversation API merge.

Churn ratio is not directly provided in the generated metrics, so exact add/remove balance across the entire window cannot be computed from the context file alone. The largest-merge records still clearly indicate both additive and transformational work.

Baseline comparison for 2025-04 was requested in audit parameters, but no monthly commit data exists for that month in the reported timeline, so direct baseline deltas are unavailable.

### Contributor Dynamics
The contributor census shows 3 roles: Lead Architect (42 commits), Developer A (20 commits), and Developer B (1 commit). Commit ownership is concentrated: the top contributor accounts for 42 of 63 in-window commits (66.7%), indicating a moderate-to-high bus-factor risk.

Monthly role distribution suggests role shifts by period: 2025-12 was architect-heavy, 2026-04 had substantial Developer A contribution (20) plus Lead Architect activity (10), and 2026-03 was primarily Lead Architect with one Developer B commit.

This points to a core-driver model with episodic collaboration, rather than a broad, evenly distributed team cadence.

### Quality Signals
Conventional commit usage is present but partial: 28 conventional commits out of 63 in-window commits (44.4%). Prefix discipline exists, but adoption is not yet dominant.

Test signals are visible but mixed in the available metrics: 236 test files are detected and 13 test-related commits are recorded in-window. A test-to-source ratio is not directly computable from this context output because source file count is not included in `test_metrics`.

Commit message quality appears serviceable based on structured prefixes and descriptive merge subjects, though formal average-length metrics are not included in the generated output.

### Governance & Process Maturity
Governance artifacts are present: constitution exists at `.documentation/memory/constitution.md`, active spec count is 0 after archival cleanup, and archived spec directory count is 10. This indicates an explicit documentation lifecycle and archival discipline.

PR-based workflow evidence exists but is not dominant in the window: 3 merged PR commits are reported. Using in-window commits as denominator, this is about 4.8% and suggests a mix of direct commits and merged branches.

Tag discipline is currently weak from a release-management standpoint: 0 tags were found. That prevents quick mapping from code history to named releases.

### Architecture & Technology
Language and tooling signals indicate a polyglot operational surface around a .NET core: JavaScript, TypeScript, PowerShell, Shell, Python, and Markdown are all present, with GitHub Actions enabled.

Hotspot and file-type activity reinforce the architecture: project files (`.csproj`) and `Program.cs` files changed frequently, indicating active dependency/configuration management. The stack appears to rely on solution-level orchestration with strong documentation and script automation.

Repository-level package configuration signals are mixed in this context (`has_package_json: false`), likely because package manifests are project-scoped rather than root-scoped.

## Change Patterns
Top hotspots suggest where complexity and coordination pressure concentrate:

1. `InquirySpark.Admin/InquirySpark.Admin.csproj` (10 changes)
2. `InquirySpark.Repository/InquirySpark.Repository.csproj` (9 changes)
3. `InquirySpark.Common.Tests/InquirySpark.Common.Tests.csproj` (9 changes)
4. `InquirySpark.Admin/Program.cs` (9 changes)
5. `InquirySpark.Common/InquirySpark.Common.csproj` (6 changes)

Interpretation:
- Frequent project-file updates point to active dependency, target framework, and build pipeline evolution.
- Repeated `Program.cs` and appsettings changes suggest runtime wiring and environment behavior are key delivery vectors.
- Documentation/spec files appearing in hotspots indicate strong process coupling between implementation and specification workflows.

Potential refactoring/operational candidates:
- Reduce repeated project-file churn by centralizing shared version/build conventions where possible.
- Stabilize frequently touched runtime bootstrapping points (`Program.cs`) with stronger modularization and regression checks.

## Milestone Timeline
No git tags were found in this repository, so a tag-based release timeline cannot be produced.

## Constitution Alignment
Constitution alignment is generally strong in process behavior:
- Documentation lifecycle principle is reflected by active use of `.documentation/` and archival workflows (active spec count 0, archived spec directories 10).
- DI/service and build-discipline intent aligns with repeated changes in project and startup wiring hotspots.
- Governance-first workflows are visible via constitution presence and scripted operations.

Primary gap:
- Release traceability is underpowered without tags, which weakens evidence for milestone discipline despite strong documentation governance.

## Developer FAQ

### What does this project do?
InquirySpark is a survey and inquiry management platform with an admin web interface and shared repository/common libraries. The commit history shows active delivery around infrastructure migration and a conversation API, and the README describes a .NET 10 system with SQLite-backed persistence and an admin-focused interface.

### What tech stack does it use?
History signals show C# as the dominant implementation language by touched files, with JavaScript/TypeScript for frontend assets, PowerShell/Shell/Python for tooling, and GitHub Actions for CI indicators. The README confirms .NET 10, and hotspots in `.csproj` and startup files indicate active framework/build management.

### Where do I start?
Start at the root [README.md](../../README.md), then inspect high-change integration points such as `InquirySpark.Admin/Program.cs` and the three primary project files in Admin, Repository, and Common.Tests. These files are repeatedly modified and are the best orientation points for build/runtime behavior.

### How do I run it locally?
Use the README flow: `dotnet restore InquirySpark.sln`, `dotnet build InquirySpark.sln`, then `dotnet run --project InquirySpark.Admin`. The documentation also states Node.js is required for admin asset pipeline behavior.

### How do I run the tests?
Run `dotnet test InquirySpark.sln` from repository root, as documented in README. The context reports 236 test files and 13 test-related commits in the window, indicating an active test surface.

### What is the branching/PR workflow?
History indicates mixed workflow: branch merges are present (including major feature merges), but only 3 merged PR commits are explicitly reported in-window. This suggests a hybrid model with both direct and PR-mediated integration.

### Who do I ask when I'm stuck?
From an anonymized role standpoint, the primary knowledge center is the Lead Architect role (42 commits, 66.7% of in-window commits). Developer A is the secondary contributor (20 commits), so onboarding/escalation should target those role owners first.

### What areas of the code change most often?
The most frequently changed areas are project configuration (`InquirySpark.Admin.csproj`, `InquirySpark.Repository.csproj`, `InquirySpark.Common.Tests.csproj`), runtime wiring (`InquirySpark.Admin/Program.cs`), and shared project configuration (`InquirySpark.Common.csproj`). These are likely high-impact files for both feature and platform work.

### Are there coding standards I must follow?
Yes. Conventional commits are used in 44.4% of in-window commits, and the repository has an explicit constitution file at `.documentation/memory/constitution.md` that defines engineering standards and documentation rules. Follow constitution-first guidance and existing project patterns.

### What version is currently released?
No version tags are present in git history, so there is no canonical tagged release identifier available from the repository metadata. Use merge commit history and changelog entries as the current proxy until tagging discipline is established.

---

Generated by /devspark.repo-story | DevSpark v1.5.0 - Adaptive System Life Cycle Development
