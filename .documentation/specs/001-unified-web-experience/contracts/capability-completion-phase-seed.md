# Capability Completion Phase Seed

**Feature**: `001-unified-web-experience`
**Created**: 2026-04-12
**Task**: T003
**Purpose**: Defines the phased completion stages for InquirySpark.Web capability delivery and the per-phase eligibility and readiness criteria.

---

## Phase Model

Capability completion proceeds through four phases. Each phase has eligibility criteria (entry) and readiness criteria (exit gate). Phases are per capability domain — not global. Multiple domains can be in different phases simultaneously.

---

### Phase 0: Not Started

**Meaning**: The capability exists in a legacy app and no implementation work has begun in InquirySpark.Web.

**Entry criteria**: All capabilities start here.

**Exit criteria**: Implementation task assigned and work has begun.

**Config representation** (in `appsettings.json`):
```json
{ "CapabilityId": "CAP-DS-001", "Phase": 0, "Status": "not-started" }
```

---

### Phase 1: In Progress

**Meaning**: The capability is actively being built in InquirySpark.Web. The legacy app remains the operational source.

**Entry criteria**:
- Capability inventoried (in `capability-inventory-seed.md`)
- Parity trace row created (in `capability-parity-traceability.md`)
- Implementing developer assigned

**Exit criteria**:
- Controller/view scaffold exists for the capability
- Basic action routing verified by developer
- No compile errors in InquirySpark.Web for this capability's files

---

### Phase 2: Implemented (Validation Pending)

**Meaning**: Code is complete. Functional parity review has not yet been recorded.

**Entry criteria**:
- All CRUD actions (or equivalent) implemented in the unified controller
- Zero-warning build passes (`dotnet build InquirySpark.sln -warnaserror`)
- Relevant unit/integration test stubs exist in `InquirySpark.Common.Tests`

**Exit criteria**:
- Functional parity validation checklist completed (per `pre-cutover-gate-criteria.md`)
- Permission parity verified (correct roles can access, incorrect roles cannot)
- UX consistency review passed (Bootstrap 5 + DataTables conventions applied)
- Performance: 95% of key actions in this domain complete in ≤2 seconds

---

### Phase 3: Validated

**Meaning**: Parity evidence accepted. The capability is considered equivalent or superior to the legacy version.

**Entry criteria**: All Phase 2 exit criteria passed.

**Recorded in**: `capability-parity-traceability.md` (status → `validated`), `contracts/us1-parity-evidence.md`

**Exit criteria for cutover**: All capabilities from the same legacy application source are in Phase 3. Pre-cutover gate criteria document (`pre-cutover-gate-criteria.md`) fully passed for the domain. Stakeholder communication issued.

---

### Phase 4: Cut Over

**Meaning**: The legacy app entry point for this capability has been removed from active deployment.

**Entry criteria**:
- Phase 3 for all capabilities from the same source app
- Cutover runbook executed (`contracts/cutover-runbook.md`)
- Rollback integrity checklist verified (`contracts/rollback-integrity-checklist.md`)
- Post-cutover validation evidence recorded (`contracts/post-cutover-parity-evidence.md`)

**Recorded in**: Capability matrix status → `cut-over`; decommission verification evidence (`contracts/decommission-verification-evidence.md`)

---

## Initial Phase Configuration Seed

This seed represents the starting state for `appsettings.json`. All capabilities begin at Phase 0.

```json
{
  "UnifiedWeb": {
    "CapabilityCompletion": {
      "Phases": [
        { "CapabilityId": "CAP-DS-001", "Phase": 0, "Status": "not-started", "Domain": "DecisionSpark" },
        { "CapabilityId": "CAP-DS-002", "Phase": 0, "Status": "not-started", "Domain": "DecisionSpark" },
        { "CapabilityId": "CAP-DS-003", "Phase": 0, "Status": "not-started", "Domain": "DecisionSpark" },
        { "CapabilityId": "CAP-DS-004", "Phase": 0, "Status": "not-started", "Domain": "DecisionSpark" },
        { "CapabilityId": "CAP-DS-005", "Phase": 0, "Status": "not-started", "Domain": "DecisionSpark" },
        { "CapabilityId": "CAP-DS-006", "Phase": 0, "Status": "not-started", "Domain": "DecisionSpark" },
        { "CapabilityId": "CAP-DS-007", "Phase": 0, "Status": "not-started", "Domain": "DecisionSpark" },
        { "CapabilityId": "CAP-IA-001", "Phase": 0, "Status": "not-started", "Domain": "InquirySpark.Admin" },
        { "CapabilityId": "CAP-IA-002", "Phase": 0, "Status": "not-started", "Domain": "InquirySpark.Admin" },
        { "CapabilityId": "CAP-IA-003", "Phase": 0, "Status": "not-started", "Domain": "InquirySpark.Admin" },
        { "CapabilityId": "CAP-IA-004", "Phase": 0, "Status": "not-started", "Domain": "InquirySpark.Admin" },
        { "CapabilityId": "CAP-IA-005", "Phase": 0, "Status": "not-started", "Domain": "InquirySpark.Admin" },
        { "CapabilityId": "CAP-IA-006", "Phase": 0, "Status": "not-started", "Domain": "InquirySpark.Admin" },
        { "CapabilityId": "CAP-IA-007", "Phase": 0, "Status": "not-started", "Domain": "InquirySpark.Admin" },
        { "CapabilityId": "CAP-IA-008", "Phase": 0, "Status": "not-started", "Domain": "InquirySpark.Admin" },
        { "CapabilityId": "CAP-IA-009", "Phase": 0, "Status": "not-started", "Domain": "InquirySpark.Admin" },
        { "CapabilityId": "CAP-IA-010", "Phase": 0, "Status": "not-started", "Domain": "InquirySpark.Admin" },
        { "CapabilityId": "CAP-IA-011", "Phase": 0, "Status": "not-started", "Domain": "InquirySpark.Admin" },
        { "CapabilityId": "CAP-IA-012", "Phase": 0, "Status": "not-started", "Domain": "InquirySpark.Admin" },
        { "CapabilityId": "CAP-IA-013", "Phase": 0, "Status": "not-started", "Domain": "InquirySpark.Admin" },
        { "CapabilityId": "CAP-IA-014", "Phase": 0, "Status": "not-started", "Domain": "InquirySpark.Admin" },
        { "CapabilityId": "CAP-IA-015", "Phase": 0, "Status": "not-started", "Domain": "InquirySpark.Admin" },
        { "CapabilityId": "CAP-IA-016", "Phase": 0, "Status": "not-started", "Domain": "InquirySpark.Admin" },
        { "CapabilityId": "CAP-IA-017", "Phase": 0, "Status": "not-started", "Domain": "InquirySpark.Admin" },
        { "CapabilityId": "CAP-IA-018", "Phase": 0, "Status": "not-started", "Domain": "InquirySpark.Admin" },
        { "CapabilityId": "CAP-IA-019", "Phase": 0, "Status": "not-started", "Domain": "InquirySpark.Admin" },
        { "CapabilityId": "CAP-IA-020", "Phase": 0, "Status": "not-started", "Domain": "InquirySpark.Admin" },
        { "CapabilityId": "CAP-IA-021", "Phase": 0, "Status": "not-started", "Domain": "InquirySpark.Admin" },
        { "CapabilityId": "CAP-IA-022", "Phase": 0, "Status": "not-started", "Domain": "InquirySpark.Admin" },
        { "CapabilityId": "CAP-IA-023", "Phase": 0, "Status": "not-started", "Domain": "InquirySpark.Admin" },
        { "CapabilityId": "CAP-IA-024", "Phase": 0, "Status": "not-started", "Domain": "InquirySpark.Admin" },
        { "CapabilityId": "CAP-IA-025", "Phase": 0, "Status": "not-started", "Domain": "InquirySpark.Admin" },
        { "CapabilityId": "CAP-IA-026", "Phase": 0, "Status": "not-started", "Domain": "InquirySpark.Admin" },
        { "CapabilityId": "CAP-IA-027", "Phase": 0, "Status": "not-started", "Domain": "InquirySpark.Admin" },
        { "CapabilityId": "CAP-IA-028", "Phase": 0, "Status": "not-started", "Domain": "InquirySpark.Admin" },
        { "CapabilityId": "CAP-IA-029", "Phase": 0, "Status": "not-started", "Domain": "InquirySpark.Admin" },
        { "CapabilityId": "CAP-IA-030", "Phase": 0, "Status": "not-started", "Domain": "InquirySpark.Admin" }
      ]
    }
  }
}
```

This seed is copied into `InquirySpark.Web/appsettings.json` as part of T017 and bound to the `CapabilityPhaseItem` configuration model (T007) via `IOptions<>`.
