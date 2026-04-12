# .documentation Guide

> Living orientation document for the `.documentation/` directory.
> Stock DevSpark framework assets live under `.devspark/`; `.documentation/` is the team-owned override and work-product layer.
> Last updated: 2026-04-12

## Directory Map

| Folder | Purpose |
|--------|---------|
| `memory/` | Authoritative constitution and long-lived governance files |
| `copilot/` | Harvest reports and session-scoped output from AI coding assistant runs |
| `specs/` | Active feature specification folders (spec, plan, tasks, data-model, checklists) |
| `templates/` | Reusable templates for specs, plans, tasks, checklists, and agent files |
| `scripts/powershell/` | Optional team-level PowerShell overrides; current repo state uses stock `.devspark/scripts/powershell/` scripts |
| `commands/` | Optional team-level overrides for DevSpark command prompts; current repo state uses stock `.devspark/defaults/commands/` prompts |
| `decisions/` | Architecture Decision Records (ADRs) — empty until first ADR is added |

## Key Files

| File | Description |
|------|-------------|
| `memory/constitution.md` | Project constitution — canonical engineering principles and conventions. **Never archive.** |
| `AGENTS.md` | Shared repository context for DevSpark agent-context hydration |
| `copilot/harvest-2026-04-12.md` | Most recent harvest report (2026-04-12) |

## Active Specs

No active spec folders are currently present under `.documentation/specs/`.
Completed and historical spec artifacts have been moved to `.archive/`.

## Templates

| Template | Use For |
|----------|---------|
| `templates/spec-template.md` | Starting a new feature specification |
| `templates/plan-template.md` | Creating an implementation plan |
| `templates/tasks-template.md` | Generating a task breakdown |
| `templates/checklist-template.md` | Creating feature or review checklists |
| `templates/agent-file-template.md` | Authoring custom agent override files |

Stock-only templates continue to live under `.devspark/templates/` unless the team chooses to override them. In DevSpark v1.5 this includes `quick-spec-template.md`.

## DevSpark Overrides

This repository currently tracks no active team PowerShell overrides and no active team command overrides. DevSpark resolves to stock assets under `.devspark/` by default.

- PowerShell scripts: `.devspark/scripts/powershell/`
- Stock commands: `.devspark/defaults/commands/`
- Team override placeholders: `.documentation/scripts/powershell/README.md` and `.documentation/commands/README.md`

This repository does not need `.documentation/scripts/bash/` unless the team intentionally decides to override Bash stock scripts as well.

## Constitution Location

`/.documentation/memory/constitution.md`

## What Is In `.archive/`

Completed and historical docs. Do not read from here during normal operations.
