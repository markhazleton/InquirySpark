# Route Policy and Decommission Readiness Evidence

**Spec:** `001-unified-web-experience`  
**Task:** T057  
**Status:** Evidence Document

---

## Route Registration — All 30 Capabilities Accessible

All unified routes follow the pattern `/Unified/{Controller}/{Action}` under the `Unified` area. The `CanonicalRoutePolicy` constants define the area name and prefix.

| Domain | Route Prefix | Controller |
|---|---|---|
| Decision Workspace | `/Unified/DecisionWorkspace` | `DecisionWorkspaceController` |
| Inquiry Administration | `/Unified/InquiryAdministration` | `InquiryAdministrationController` |
| Inquiry Authoring | `/Unified/InquiryAuthoring` | `InquiryAuthoringController` |
| Inquiry Operations | `/Unified/InquiryOperations` | `InquiryOperationsController` |
| Operations Support | `/Unified/OperationsSupport` | `OperationsSupportController` |
| Capability Matrix | `/Unified/CapabilityCompletionMatrix` | `CapabilityCompletionMatrixController` |
| Operational Readiness | `/Unified/OperationalReadiness` | `OperationalReadinessController` |

## Legacy Route Coexistence

As of the current baseline, all legacy routes from `DecisionSpark` and `InquirySpark.Admin` remain registered. The `CapabilityRoutingMap.Resolve` method maps each CAP-* ID to its canonical unified route.

## Decommission Readiness — Go/No-Go by Domain

| Domain | Legacy App | US1 Parity Evidence | Navigation Parity | Permission Parity | Phase |
|---|---|---|---|---|---|
| Decision Workspace | DecisionSpark | yes (us1-parity-evidence.md) | yes (US1NavigationTests) | yes (US1AuthenticationFlowTests) | 3 — Validated |
| Inquiry Administration | InquirySpark.Admin | yes | yes | yes | 3 — Validated |
| Inquiry Authoring | InquirySpark.Admin | yes | yes | yes | 3 — Validated |
| Inquiry Operations | InquirySpark.Admin | yes | yes | yes | 3 — Validated |
| Operations Support | both | yes | yes | yes | 3 — Validated |

**All domains are at Phase 3 (Validated)** — functional parity, navigation parity, and permission parity are established in code and tests. Domains are NOT yet at Phase 4 (Cut Over) — cutover execution (T063/T064) requires a real go/no-go gate review per `pre-cutover-gate-criteria.md`.

## Decommission Blockers (T063/T064 Preconditions)

These conditions must be satisfied before any legacy project is removed from `InquirySpark.sln`:

1. All domains at Phase 4 (Cut Over) in the `CapabilityCompletionMatrixController`
2. 30-day monitoring window completed with no P1/P2 SC-004/SC-006 incidents
3. Stakeholder sign-off (see `stakeholder-communication-pack.md`)
4. `rollback-integrity-checklist.md` completed with PASS for all items
5. `post-cutover-parity-evidence.md` (T062A) recorded with PASS
