# Greenfield Route Policy: InquirySpark.Web

**Feature**: `001-unified-web-experience`
**Created**: 2026-04-12
**Authority**: FR-008 — "InquirySpark.Web MUST define canonical routes for all unified workflows and MUST NOT depend on legacy route compatibility behavior."

---

## Policy Statement

`InquirySpark.Web` uses **canonical, greenfield routes only**. No legacy URL structures from `DecisionSpark` or `InquirySpark.Admin` are preserved, redirected, or depended upon. All URLs must be designed from first principles to serve the unified experience.

---

## Route Conventions

### Area and Prefix

All unified capability routes live under the `Unified` MVC area:

```
/Unified/{controller}/{action}/{id?}
```

Default home (area root):

```
/Unified/Operations/Index
```

### Controller Route Families

| Capability Family | Controller | Route Prefix |
|---|---|---|
| Operations home / dashboard | `OperationsController` | `/Unified/Operations/` |
| DecisionSpark conversations | `DecisionConversationController` | `/Unified/DecisionConversation/` |
| Decision specifications (admin) | `DecisionSpecificationController` | `/Unified/DecisionSpecification/` |
| Inquiry administration | `InquiryAdministrationController` | `/Unified/InquiryAdministration/` |
| Inquiry authoring | `InquiryAuthoringController` | `/Unified/InquiryAuthoring/` |
| Inquiry operations | `InquiryOperationsController` | `/Unified/InquiryOperations/` |
| Support / charts / health | `OperationsSupportController` | `/Unified/OperationsSupport/` |
| Capability completion matrix | `CapabilityCompletionMatrixController` | `/Unified/CapabilityCompletionMatrix/` |
| Operational readiness | `OperationalReadinessController` | `/Unified/OperationalReadiness/` |

### Identity / Auth Routes

Identity UI endpoints (Login, Register, Manage) are served via Razor Pages and mapped at:

```
/Identity/Account/Login
/Identity/Account/Logout
/Identity/Account/Manage
```

These are reused directly from the `InquirySpark.Admin` pattern and are NOT area-scoped to `Unified`.

---

## Legacy Application Removal Criteria

A legacy application (`DecisionSpark` or `InquirySpark.Admin`) is eligible for removal from active runtime deployment when **all** of the following are true:

1. Every capability inventoried from the legacy app appears in the capability completion matrix with status `Validated`.
2. Pre-cutover gate criteria (`contracts/pre-cutover-gate-criteria.md`, T045A) for all domains sourced from the legacy app are marked **PASS**.
3. Post-cutover validation evidence (`contracts/post-cutover-parity-evidence.md`, T062A) for all migrated domains is recorded.
4. `dotnet build InquirySpark.sln -warnaserror` passes with zero warnings.
5. Rollback data-integrity checklist (`contracts/rollback-integrity-checklist.md`, T047A1) has been executed and passed.
6. Stakeholder communication for decommission timing has been issued (`contracts/stakeholder-communication-pack.md`, T047).

No partial removal is permitted: the legacy app is either fully active or fully removed. There are no legacy-compatibility shims.

---

## Prohibited Patterns

- `Redirect()` calls whose target URLs match legacy app route patterns.
- `[Route]` attributes that replicate legacy segment naming (e.g., `Admin/DecisionSpecs`).
- Any `app.UsePathBase()` configuration that mounts InquirySpark.Web under a legacy prefix.
- Hard-coded string URLs in views or controllers pointing to `DecisionSpark` or `InquirySpark.Admin` origins.

---

## Enforcement

This policy is enforced at PR review. Any route pattern not matching the `Unified` area conventions defined above requires explicit approval and a documented exception in this file.
