# Quickstart: Unified InquirySpark Web Experience Planning Validation

## Purpose
Validate planning assumptions and readiness signals for consolidating DecisionSpark and InquirySpark.Admin into InquirySpark.Web.

## Preconditions
- Feature branch checked out: `001-unified-web-experience`
- Spec, plan, research, and design artifacts exist under `.documentation/specs/001-unified-web-experience/`
- Solution builds successfully on current baseline

## 1. Baseline Verification
1. Build solution:
   - `dotnet build InquirySpark.sln`
2. Confirm both legacy app surfaces are present and runnable in baseline environment.
3. Record current capability inventory snapshot for both legacy applications.

## 2. Capability-Domain Migration Matrix Initialization
1. Create initial domain grouping list (e.g., conversation, specs admin, survey admin, reporting/admin tooling).
2. Enumerate capabilities per domain with legacy entry points.
3. Set initial parity statuses to NotStarted.

## 3. Identity Strategy Validation
1. Confirm canonical identity authority choice and migration bridge assumptions.
2. Identify role mapping rules from both legacy app contexts.
3. Define acceptance checks for access-equivalence in parity validation.

## 4. Route Compatibility Strategy Validation
1. Enumerate high-value legacy routes/workflows.
2. Assign compatibility tiers (Critical, Standard, None).
3. Validate that each Critical mapping has a unified destination and migration messaging.

## 5. Performance and Operational Gate Definition
1. Define test list for key user actions.
2. Validate measurement method for 95% <=2s target.
3. Define cutover gate checklist template per capability domain:
   - Functional parity
   - Access parity
   - UX consistency
   - Performance target pass
   - Rollback readiness

## 6. Cutover Readiness Dry Run
1. Simulate one domain-level go/no-go review using the gate checklist.
2. Confirm required evidence references and decision recording format.
3. Verify rollback decision path is documented and testable.

## Exit Criteria
- Migration matrix initialized with parity statuses.
- Identity and route strategy assumptions explicitly validated.
- Performance and cutover gates measurable and documented.
- One dry-run cutover decision completed with traceable evidence.
