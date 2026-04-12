# Contracts: Unified InquirySpark Web Experience

## Purpose
Define planning-level interface contracts for migration governance, parity tracking, and unified experience readiness. These contracts are technology-agnostic and used as acceptance interfaces for implementation and testing.

## Contract 1: Capability Inventory Query

### Request
- Inputs:
  - Domain filter (optional)
  - Parity status filter (optional)
  - Risk level filter (optional)

### Response
- Collection of CapabilityItem records containing:
  - Capability identity
  - Legacy and unified entry points
  - Current parity status
  - Domain ownership
  - Latest validation summary

### Behavioral Guarantees
- Must return deterministic ordering by domain priority then capability name.
- Must include status values from canonical parity state set.

## Contract 2: Parity Validation Submission

### Request
- Inputs:
  - Capability identity
  - Functional/access/UX/performance pass flags
  - Validator identity
  - Validation notes and evidence reference

### Response
- Recorded ParityValidationRecord with timestamp and resulting capability parity status.

### Behavioral Guarantees
- Validation record is immutable once accepted.
- Capability status transition is enforced by state rules.

## Contract 3: Migration Phase Status

### Request
- Inputs:
  - Domain identity
  - Phase identity

### Response
- Phase details including:
  - Current phase state
  - In-scope capability counts by parity state
  - Cutover gate checklist status
  - Rollback readiness indicator

### Behavioral Guarantees
- Phase status response must include complete gate evidence references for auditability.

## Contract 4: Cutover Decision Recording

### Request
- Inputs:
  - Phase identity
  - Decision (Go, NoGo, Rollback)
  - Approver
  - Rationale
  - Evidence bundle reference

### Response
- Persisted CutoverDecisionRecord and resulting phase status.

### Behavioral Guarantees
- Go decision must be rejected if required readiness gates are incomplete.
- Rollback decision must produce a traceable operational event.

## Contract 5: Route Compatibility Lookup

### Request
- Inputs:
  - Legacy route or workflow identifier

### Response
- RouteCompatibilityMapping result:
  - Compatibility tier
  - Unified destination
  - Messaging guidance if no mapping

### Behavioral Guarantees
- Critical-tier mappings must resolve deterministically.
- Missing mappings must return explicit migration guidance.
