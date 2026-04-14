# Pre-Cutover Gate Criteria

**Spec:** `001-unified-web-experience`  
**Task:** T045A  
**FR Reference:** FR-011  
**Status:** Active — must pass before `RecordCutoverDecisionAsync` may be called with `Decision=Go` for any domain.

---

## Purpose

These criteria define the testable pass/fail conditions that must be verified for each capability domain before its cutover is approved. Each criterion must have recorded evidence in `us1-parity-evidence.md` or a domain-specific evidence file.

A domain fails gate review if ANY criterion is not marked PASS.

---

## Universal Gate Criteria (All Domains)

The following criteria apply to every domain regardless of capability family:

| # | Criterion | Evidence Required | Gating |
|---|---|---|---|
| G-001 | All domain capabilities are at Phase ≥ 3 (Validated) | Capability matrix screenshot or JSON export | Hard gate |
| G-002 | Zero open build errors (`dotnet build -warnaserror`) | Build output log | Hard gate |
| G-003 | All unit/integration tests pass (`dotnet test`) | Test run output | Hard gate |
| G-004 | Functional parity: all capability use-cases work end-to-end in InquirySpark.Web | Manual test notes in domain evidence file | Hard gate |
| G-005 | Permission parity: each role can access exactly the capabilities they're authorized for | Role-test walkthrough evidence | Hard gate |
| G-006 | No new EF migrations were generated (spec: no new DB objects) | `dotnet ef migrations list` output | Hard gate |
| G-007 | Performance validation: P95 response time ≤ 2000ms for all capability routes | Performance evidence file | Soft (document deviation) |
| G-008 | Post-cutover rollback path tested in dry-run mode | RevertDomainCutoverAsync test evidence | Hard gate |

---

## Domain-Specific Gate Criteria

### Decision Workspace (CAP-DS-001 thru CAP-DS-007)

| # | Criterion | Evidence |
|---|---|---|
| DS-001 | DecisionSpark file storage reads correctly from unified context | Integration test output |
| DS-002 | `IDecisionSparkFileStorageService.ListAllSpecsAsync()` returns same data as legacy DecisionSpark API | Comparison evidence |
| DS-003 | Decision Spec status transitions work: Draft → Review → Approved → Archived | Manual workflow test |
| DS-004 | Audit trail entries are emitted for all state changes | Log capture evidence |

### Inquiry Administration (CAP-IA-001 thru CAP-IA-005, CAP-IA-024)

| # | Criterion | Evidence |
|---|---|---|
| IA-A-001 | Application CRUD matches Admin behavior | Side-by-side test |
| IA-A-002 | User-role assignments survive page reload (read-only context) | UI test evidence |
| IA-A-003 | Lookup table reads (application types, question types) return same values as Admin | Data comparison |

### Inquiry Authoring (CAP-IA-006 thru CAP-IA-011)

| # | Criterion | Evidence |
|---|---|---|
| IA-AU-001 | Survey list shows same surveys as Admin | Data comparison |
| IA-AU-002 | Question group membership displays correctly | UI walkthrough |

### Inquiry Operations (CAP-IA-012 thru CAP-IA-017)

| # | Criterion | Evidence |
|---|---|---|
| IA-OP-001 | Companies list matches Admin | Data comparison |
| IA-OP-002 | Survey status update workflow is accessible | UI walkthrough |

### Operations Support (CAP-IA-025 thru CAP-IA-030)

| # | Criterion | Evidence |
|---|---|---|
| OS-001 | `/Unified/OperationsSupport/Health` returns 200 for anonymous users | HTTP test |
| OS-002 | Chart builder renders catalog of chart definitions | UI test |
| OS-003 | Conversation API proxy returns spec list | HTTP GET test to `/Unified/DecisionConversation/Api` |

---

## Gate Review Record Template

For each domain, record a review entry before submitting a `Decision=Go` cutover:

```
Domain: [domain name]
Reviewed By: [name]
Date: [YYYY-MM-DD]
Universal Gates: G-001=[P/F], G-002=[P/F], G-003=[P/F], G-004=[P/F], G-005=[P/F], G-006=[P/F], G-007=[P/F⚠], G-008=[P/F]
Domain Gates: [list per-domain gate IDs and pass/fail]
Evidence Links: [links to evidence files or artifacts]
Decision: GO / NO-GO
Notes: [any deviations or waivers with justification]
```
