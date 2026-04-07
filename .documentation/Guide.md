# .documentation Guide

> Living orientation document for the `.documentation/` directory.
> Last updated: 2026-04-07

## Directory Map

| Folder | Purpose |
|--------|---------|
| `memory/` | Authoritative constitution and long-lived governance files |
| `copilot/` | Harvest reports and session-scoped output from AI coding assistant runs |
| `specs/` | Active feature specification folders (spec, plan, tasks, data-model, checklists) |
| `templates/` | Reusable templates for specs, plans, tasks, checklists, and agent files |
| `scripts/powershell/` | Team-level PowerShell context scripts used by DevSpark commands |
| `commands/` | Team-level overrides for DevSpark command prompts (empty = use stock defaults) |
| `decisions/` | Architecture Decision Records (ADRs) — empty until first ADR is added |

## Key Files

| File | Description |
|------|-------------|
| `memory/constitution.md` | Project constitution — canonical engineering principles and conventions. **Never archive.** |
| `copilot/harvest-2026-04-07.md` | Most recent harvest report (2026-04-07) |

## Active Specs

| Spec | Status | Notes |
|------|--------|-------|
| `specs/001-remove-sql-server/` | In-progress | US2/US3 (build baseline + quality validation) still open |

## Templates

| Template | Use For |
|----------|---------|
| `templates/spec-template.md` | Starting a new feature specification |
| `templates/plan-template.md` | Creating an implementation plan |
| `templates/tasks-template.md` | Generating a task breakdown |
| `templates/checklist-template.md` | Creating feature or review checklists |
| `templates/agent-file-template.md` | Authoring custom agent override files |

## DevSpark Scripts

Scripts under `scripts/powershell/` are team-level overrides for DevSpark context-gathering commands. When a script is not present here the stock version under `.devspark/scripts/powershell/` is used automatically.

| Script | Purpose |
|--------|---------|
| `archive-context.ps1` | Gathers archive candidates for `devspark.archive` |
| `check-prerequisites.ps1` | Validates dev environment prerequisites |
| `common.ps1` | Shared utilities used by other scripts |
| `create-new-feature.ps1` | Scaffolds a new spec folder |
| `evolution-context.ps1` | Prepares context for `devspark.evolve-constitution` |
| `get-pr-context.ps1` | Collects context for `devspark.pr-review` |
| `migrate-to-documentation.ps1` | Migrates legacy `docs/` content to `.documentation/` |
| `quickfix-context.ps1` | Prepares context for `devspark.quickfix` |
| `release-context.ps1` | Collects context for `devspark.release` |
| `repo-story-context.ps1` | Collects context for `devspark.repo-story` |
| `setup-plan.ps1` | Sets up plan scaffolding in a spec folder |
| `site-audit.ps1` | Collects context for `devspark.site-audit` |
| `sync-upstream.ps1` | Synchronizes DevSpark framework updates from upstream |
| `update-agent-context.ps1` | Refreshes agent context across the repo |

## Constitution Location

`/.documentation/memory/constitution.md`

## What Is In `.archive/`

Completed and historical docs. Do not read from here during normal operations.
