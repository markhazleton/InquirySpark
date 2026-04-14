# Data Model: Unified InquirySpark Web Experience

## Overview
This feature introduces planning and governance entities for migration orchestration and parity validation. These entities describe transition state and operational controls; physical persistence choices are deferred to implementation tasks.

## Entities

### CapabilityDomain
- Description: Logical grouping of related functionality to be migrated together.
- Key fields:
  - DomainId
  - DomainName
  - LegacySource (DecisionSpark, InquirySpark.Admin, Mixed)
  - OwnerRole
  - PriorityRank
  - ActiveFlag
- Relationships:
  - One CapabilityDomain has many CapabilityItems.
  - One CapabilityDomain has many MigrationPhases.

### CapabilityItem
- Description: Atomic user-visible capability tracked for parity.
- Key fields:
  - CapabilityId
  - DomainId
  - CapabilityName
  - LegacyEntryPoint
  - UnifiedEntryPoint
  - ParityStatus (NotStarted, InProgress, Validated, CutOver)
  - RiskLevel (Low, Medium, High)
  - ValidationEvidenceRef
- Relationships:
  - Belongs to one CapabilityDomain.
  - Has many ParityValidationRecords.

### MigrationPhase
- Description: Time-boxed rollout stage for a domain or domain subset.
- Key fields:
  - PhaseId
  - DomainId
  - PhaseName
  - PlannedStart
  - PlannedEnd
  - PhaseStatus (Planned, Active, Completed, RolledBack)
  - RollbackPolicyRef
- Relationships:
  - Belongs to one CapabilityDomain.
  - Has one or more CutoverDecisionRecords.

### IdentityMappingRecord
- Description: Transition mapping between legacy identity representation and canonical authority.
- Key fields:
  - MappingId
  - LegacyUserKey
  - CanonicalUserKey
  - RoleMappingStatus
  - EffectiveFrom
  - EffectiveTo
  - MigrationState
- Relationships:
  - Referenced by ParityValidationRecords for access-equivalence checks.

### ParityValidationRecord
- Description: Evidence that a capability item meets parity expectations.
- Key fields:
  - ValidationId
  - CapabilityId
  - FunctionalParityPass
  - AccessParityPass
  - UXConsistencyPass
  - PerformancePass
  - ValidatedBy
  - ValidatedAt
  - Notes
- Relationships:
  - Belongs to one CapabilityItem.
  - May reference one IdentityMappingRecord.

### CutoverDecisionRecord
- Description: Governance decision for go/no-go and rollback outcomes.
- Key fields:
  - DecisionId
  - PhaseId
  - DecisionType (Go, NoGo, Rollback)
  - DecisionTimestamp
  - Approver
  - EvidenceBundleRef
  - DecisionRationale
- Relationships:
  - Belongs to one MigrationPhase.

### RouteCompatibilityMapping
- Description: Transitional mapping from legacy route patterns to unified entry points.
- Key fields:
  - MappingId
  - LegacyRoutePattern
  - UnifiedRoutePattern
  - CompatibilityTier (Critical, Standard, None)
  - ExpirationPolicy
  - Status
- Relationships:
  - Associated with one or more CapabilityItems.

## Validation Rules
- CapabilityItem.ParityStatus cannot advance to CutOver unless at least one ParityValidationRecord has all pass flags true.
- MigrationPhase cannot move to Completed unless all in-scope CapabilityItems are Validated or explicitly deferred with approved rationale.
- CutoverDecisionRecord DecisionType=Go requires evidence bundle reference.
- IdentityMappingRecord must be unique per legacy/canonical key pair at any given effective time window.

## State Transitions

### CapabilityItem ParityStatus
- NotStarted -> InProgress -> Validated -> CutOver
- InProgress -> NotStarted (if phase rollback occurs before validation)
- Validated -> InProgress (if regression detected)

### MigrationPhase PhaseStatus
- Planned -> Active -> Completed
- Active -> RolledBack
- RolledBack -> Active (only with approved restart decision)
