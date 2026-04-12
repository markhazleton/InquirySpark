# Rollback Data Integrity Checklist

**Spec:** `001-unified-web-experience`  
**Task:** T047A1  
**FR Reference:** FR-006  
**Status:** Active — run after any `RevertDomainCutoverAsync` execution

---

## Purpose

This checklist verifies that a domain cutover rollback (`RevertDomainCutoverAsync`) leaves the system in a consistent, recoverable state. It must be completed before the system is declared "rollback complete" and before any re-attempt at cutover.

---

## In-Flight Record State Consistency

After rollback execution, verify:

- [ ] All capabilities in the rolled-back domain show Phase ≤ 3 (not Phase 4 = CutOver) in the capability completion matrix
- [ ] No capability has `Status = "cut-over"` for the rolled-back domain
- [ ] The rollback log entry is present: `[UnifiedWeb] ROLLBACK: Domain {domain} cutover reverted by {user}. Reason: {reason}`
- [ ] Any `CutoverDecisionRecordItem` objects for the domain have `IsCutOver = false`
- [ ] No new capability phase transitions occurred AFTER the rollback timestamp

## DecisionSpark File Storage State Validation

If the rolled-back domain is "Decision Workspace":

- [ ] `IDecisionSparkFileStorageService.ListAllSpecsAsync()` returns consistent results
- [ ] No DecisionSpec objects have timestamps after the rollback that would indicate data corruption
- [ ] File-backed spec data directory is intact (no orphaned temp files)

## Configuration State Coherence

- [ ] `appsettings.json` `UnifiedWeb:CutoverPolicy:DryRunMode` is set appropriately (true for re-test)  
- [ ] `EnableLegacyFallback` is `true` if it was enabled before rollback
- [ ] IOptions-bound capability phase configuration reflects pre-cutover values (restart app if config changed at runtime)

## Authorization and Identity Coherence

- [ ] Users with Analyst/Operator/Consultant/Executive roles can still access their capabilities under `/Unified/`
- [ ] No users are locked out due to role reconfiguration during rollback

## Post-Rollback Validation Run

- [ ] `dotnet test InquirySpark.Common.Tests` passes
- [ ] All US1NavigationTests pass
- [ ] All US1AuthenticationFlowTests pass
- [ ] `/Unified/OperationsSupport/Health` returns HTTP 200
- [ ] Capability completion matrix accessible at `/Unified/CapabilityCompletionMatrix`

---

## Rollback Evidence Record Template

Complete this template and save to `contracts/rollback-records/[domain]-[YYYY-MM-DD]-rollback.md`:

```
Rollback Date: [YYYY-MM-DD HH:MM UTC]
Domain: [domain name]
Reverted By: [operator name]
Reason: [description of rollback trigger - include SC-004/SC-006 incident ref if applicable]

Checklist Results:
- In-flight record consistency: [PASS/FAIL - details]
- File storage state (if applicable): [PASS/FAIL/N/A - details]
- Configuration state: [PASS/FAIL - details]
- Authorization coherence: [PASS/FAIL - details]
- Test suite: [PASS/FAIL - test run output link]

Rollback Declared Complete: [YES/NO]
Re-Cutover Permitted After: [date - typically after root cause fix is validated]
```
