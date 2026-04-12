# Cutover Operation Runbook

**Spec:** `001-unified-web-experience`  
**Task:** T046  
**Status:** Active Governance Document  
**FR Reference:** FR-006, FR-011, FR-014

---

## Purpose

This runbook defines the step-by-step operational procedure for executing a capability domain cutover from a legacy application (DecisionSpark or InquirySpark.Admin) to InquirySpark.Web.

**Precondition for every step:** The domain MUST have a recorded PASS on all criteria in [`pre-cutover-gate-criteria.md`](./pre-cutover-gate-criteria.md) before proceeding. A NO-GO gate means STOP — do not continue the cutover.

---

## Post-Cutover Monitoring Directive

After executing a domain cutover, operations MUST:
- Track **SC-004** (capability parity regressions) and **SC-006** (legacy app access reverted) incident metrics for **30 days** after each domain cutover.
- Review incident counts weekly during the monitoring window.
- Trigger `RevertDomainCutoverAsync` immediately if SC-004 or SC-006 incidents exceed threshold (>2 P1/P2 incidents in 7 days).

---

## Cutover Domain Sequence

Per the cutover policy, domains are cut over in this order:

1. **Operations Support** — lowest risk, no legacy user workflows depend on it
2. **Decision Workspace** — DecisionSpark parity already validated (US1)
3. **Inquiry Administration** — Admin parity validated (US1)
4. **Inquiry Authoring** — depends on Administration cutover being clean
5. **Inquiry Operations** — depends on Administration + Authoring cutover being clean

---

## Step-by-Step Runbook

### Step 1: Pre-Cutover Gate Review

1. Open [`pre-cutover-gate-criteria.md`](./pre-cutover-gate-criteria.md)
2. Complete gate review record for the target domain
3. All hard gates must show PASS
4. Document gate review in `contracts/cutover-records/[domain]-[date]-gate-review.md`
5. **If any hard gate is FAIL: STOP. Do not proceed.**

### Step 2: Enable Dry-Run Mode Verification

```bash
# Confirm DryRunMode=true in appsettings.json UnifiedWeb:CutoverPolicy section
grep -A 5 "CutoverPolicy" InquirySpark.Web/appsettings.json
```

With `DryRunMode=true`, call `RecordCutoverDecisionAsync` with `Decision=Go` and verify the log output without making permanent changes.

### Step 3: Notify Stakeholders

Send pre-cutover notification to stakeholders per [`stakeholder-communication-pack.md`](./stakeholder-communication-pack.md) (T047).

Expected downtime: **Zero** (InquirySpark.Web is additive — legacy apps remain available during cutover window).

### Step 4: Execute Cutover

1. Set `DryRunMode=false` in production configuration
2. Call `RecordCutoverDecisionAsync` for the target domain:

```csharp
await capabilityService.RecordCutoverDecisionAsync(new CutoverDecisionRecordItem
{
    Domain = "Decision Workspace",
    LegacyApp = "DecisionSpark",
    Decision = "Go",
    ApprovedBy = "[approver name]",
    IsCutOver = true,
    Notes = "Gate review passed. Evidence: contracts/cutover-records/...",
    DecisionDate = DateTimeOffset.UtcNow,
});
```

3. Verify log entry: `[UnifiedWeb] CutoverDecision recorded for domain Decision Workspace`
4. Verify capability completion matrix shows Phase 4 for all domain capabilities

### Step 5: Post-Cutover Validation

1. Run smoke tests:
   - Access each unified route in the domain
   - Verify correct data is shown
   - Verify authorization policies enforce correctly
2. Run `dotnet test` — should still pass
3. Record post-cutover parity evidence in `contracts/post-cutover-parity-evidence.md`

### Step 6: Begin 30-Day Monitoring Window

1. Start SC-004/SC-006 incident tracking
2. Schedule weekly review for 30 days
3. Set calendar reminder for end-of-window decommission decision review

### Step 7: Decommission Decision (After 30-Day Window)

If the monitoring window is clean (0 P1/P2 incidents):
1. Remove legacy application from active solution (T063 or T064)
2. Update README.md deployment references (T065)
3. Update CI/CD pipeline configuration (T066)

---

## Rollback Procedure

If a STOP condition is encountered or SC-004/SC-006 thresholds are breached:

```csharp
await capabilityService.RevertDomainCutoverAsync(
    domain: "Decision Workspace",
    revertedBy: "[operator name]",
    reason: "[detailed reason for rollback]"
);
```

After rollback:
1. Verify legacy application is accessible
2. Verify all domain capabilities reverted to Phase 3 (Validated) in the matrix
3. File incident report with root cause and remediation plan
4. Do not re-attempt cutover until root cause is resolved and ALL gates re-verified

---

## References

- [`pre-cutover-gate-criteria.md`](./pre-cutover-gate-criteria.md) — Gate criteria
- [`us1-parity-evidence.md`](./us1-parity-evidence.md) — Phase 3 parity evidence
- [`stakeholder-communication-pack.md`](./stakeholder-communication-pack.md) — Communications
- [`rollback-integrity-checklist.md`](./rollback-integrity-checklist.md) — Rollback data integrity
