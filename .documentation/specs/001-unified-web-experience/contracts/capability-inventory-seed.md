# Capability Inventory Seed

**Feature**: `001-unified-web-experience`
**Created**: 2026-04-12
**Task**: T002 — Authoritative capability inventory from BOTH legacy applications
**Source**: `contracts/capability-discovery-results.md` (T001A)
**Feeds into**: T002A (parity traceability matrix), T002B (delivery plan)

---

## Purpose

This document is the **authoritative list of all user-facing capabilities** that InquirySpark.Web must implement. Every row represents a discrete capability unit that must appear in the capability completion matrix with a `Validated` status before the corresponding legacy application can be decommissioned.

---

## Inventory

| ID | Source App | Capability Name | Category | Data Storage | Priority |
|----|---|---|---|---|---|
| CAP-DS-001 | DecisionSpark | Decision Conversations (guided session) | Core Workflow | File (conversations/) | P1 |
| CAP-DS-002 | DecisionSpark | Decision Specification Admin (CRUD UI) | Administration | File (DecisionSpecs/) | P1 |
| CAP-DS-003 | DecisionSpark | Decision Specification API (REST) | API | File (DecisionSpecs/) | P1 |
| CAP-DS-004 | DecisionSpark | LLM-Assisted Spec Drafting | Administration | File (drafts/) | P2 |
| CAP-DS-005 | DecisionSpark | Spec Lifecycle Management (status transitions) | Administration | File (DecisionSpecs/) | P1 |
| CAP-DS-006 | DecisionSpark | Spec Audit History | Governance | File (audit in spec JSON) | P2 |
| CAP-DS-007 | DecisionSpark | System Health Check (DecisionSpecs directory) | Observability | N/A | P3 |
| CAP-IA-001 | InquirySpark.Admin | Application Management | Administration | SQLite (read-only) | P1 |
| CAP-IA-002 | InquirySpark.Admin | Application User Management | Administration | SQLite + Identity | P1 |
| CAP-IA-003 | InquirySpark.Admin | Application User Role Assignment | Administration | SQLite + Identity | P1 |
| CAP-IA-004 | InquirySpark.Admin | Application Survey Assignment | Administration | SQLite (read-only) | P1 |
| CAP-IA-005 | InquirySpark.Admin | Application Properties | Administration | SQLite (read-only) | P2 |
| CAP-IA-006 | InquirySpark.Admin | Survey Authoring (CRUD) | Authoring | SQLite (read-only) | P1 |
| CAP-IA-007 | InquirySpark.Admin | Survey Email Template Management | Authoring | SQLite (read-only) | P2 |
| CAP-IA-008 | InquirySpark.Admin | Question Authoring (CRUD) | Authoring | SQLite (read-only) | P1 |
| CAP-IA-009 | InquirySpark.Admin | Question Group Management | Authoring | SQLite (read-only) | P1 |
| CAP-IA-010 | InquirySpark.Admin | Question Group Member Management | Authoring | SQLite (read-only) | P1 |
| CAP-IA-011 | InquirySpark.Admin | Question Answer Management | Authoring | SQLite (read-only) | P1 |
| CAP-IA-012 | InquirySpark.Admin | Company Management | Operations | SQLite (read-only) | P2 |
| CAP-IA-013 | InquirySpark.Admin | Import History Tracking | Operations | SQLite (read-only) | P2 |
| CAP-IA-014 | InquirySpark.Admin | Survey Status Management | Operations | SQLite (read-only) | P1 |
| CAP-IA-015 | InquirySpark.Admin | Survey Review Status Tracking | Operations | SQLite (read-only) | P2 |
| CAP-IA-016 | InquirySpark.Admin | Site Role Management | Site Configuration | SQLite (read-only) | P2 |
| CAP-IA-017 | InquirySpark.Admin | Site Application Menu Management | Site Configuration | SQLite (read-only) | P2 |
| CAP-IA-018 | InquirySpark.Admin | Lookup: Application Types | Lookup Management | SQLite (read-only) | P3 |
| CAP-IA-019 | InquirySpark.Admin | Lookup: Question Types | Lookup Management | SQLite (read-only) | P3 |
| CAP-IA-020 | InquirySpark.Admin | Lookup: Review Status | Lookup Management | SQLite (read-only) | P3 |
| CAP-IA-021 | InquirySpark.Admin | Lookup: Survey Response Status | Lookup Management | SQLite (read-only) | P3 |
| CAP-IA-022 | InquirySpark.Admin | Lookup: Survey Types | Lookup Management | SQLite (read-only) | P3 |
| CAP-IA-023 | InquirySpark.Admin | Lookup: Units of Measure | Lookup Management | SQLite (read-only) | P3 |
| CAP-IA-024 | InquirySpark.Admin | Role Management | Access Control | Identity | P1 |
| CAP-IA-025 | InquirySpark.Admin | Chart Settings Configuration | Analytics | SQLite (read-only) | P3 |
| CAP-IA-026 | InquirySpark.Admin | Chart Builder UI | Analytics | SQLite (read-only) | P3 |
| CAP-IA-027 | InquirySpark.Admin | Chart Definitions API | Analytics | SQLite (read-only) | P3 |
| CAP-IA-028 | InquirySpark.Admin | System Health Dashboard | Observability | N/A | P3 |
| CAP-IA-029 | InquirySpark.Admin | User Preferences API | User Settings | SQLite + Identity | P2 |
| CAP-IA-030 | InquirySpark.Admin | Conversation API Integration | API | In-memory | P2 |

---

## Capability Counts

| Source | P1 | P2 | P3 | Total |
|---|---|---|---|---|
| DecisionSpark | 4 | 2 | 1 | 7 |
| InquirySpark.Admin | 11 | 9 | 10 | 30 |
| **Total** | **15** | **11** | **11** | **37** |

---

## Status Legend

Each capability will be tracked in the capability completion matrix (T002A) with these statuses:

| Status | Meaning |
|---|---|
| `not-started` | No implementation work has begun |
| `in-progress` | Actively being built in InquirySpark.Web |
| `implemented` | Code complete, pending validation |
| `validated` | Parity evidence recorded and accepted |
| `cut-over` | Legacy counterpart decommissioned |

---

## Notes

- The API key authentication mechanism from DS-F03 (DecisionSpark REST API) is **not carried forward** to InquirySpark.Web. The unified API surface will use Identity-based authorization.
- The `IConversationPersistence` write path (CAP-DS-001) is best-effort; the session runtime is in-memory.
- All `InquirySpark.Admin` SQLite access is read-only (`InquirySparkContext`). No writes to SQLite occur in the unified app.
- Lookup table capabilities (CAP-IA-018 through CAP-IA-023) are P3 because they are low-frequency administrative tasks.
