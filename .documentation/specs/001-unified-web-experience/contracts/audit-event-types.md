# Audit Event Types

**Spec:** `001-unified-web-experience`  
**Task:** T047C  
**FR Reference:** FR-007  
**Status:** Schema Authority — defines payload fields for `UnifiedAuditEventItem` (T048)

---

## Required Payload Fields (All Events)

Every audit event MUST include:

| Field | Type | Description |
|---|---|---|
| `EventType` | `string` | Canonical event type identifier (see catalog below) |
| `UserId` | `string` | Identity of the acting user |
| `Timestamp` | `DateTimeOffset` | UTC time event was recorded |
| `CorrelationId` | `string` | Request or operation correlation ID |
| `ResourceId` | `string?` | Identifier of the resource acted upon (null for system events) |
| `ActionDetails` | `string?` | Human-readable description of what happened |
| `Domain` | `string?` | Capability domain context (e.g., "Decision Workspace") |
| `Source` | `string` | Source application ("InquirySpark.Web", "DecisionSpark", "Admin") |
| `Severity` | `string` | Informational \| Warning \| Critical |

---

## Audit Event Catalog

### Decision Workspace Events (from DecisionSpark)

| EventType | Trigger | Severity | ResourceId Meaning |
|---|---|---|---|
| `DS.Conversation.Created` | New conversation started | Informational | Conversation ID |
| `DS.Conversation.Completed` | Conversation status → Complete | Informational | Conversation ID |
| `DS.Spec.Created` | New decision spec record | Informational | Spec ID |
| `DS.Spec.Updated` | Decision spec saved | Informational | Spec ID |
| `DS.Spec.Deleted` | Decision spec permanently removed | Warning | Spec ID |
| `DS.Spec.Exported` | Spec exported to file | Informational | Spec ID |

### Inquiry Administration Events (from InquirySpark.Admin)

| EventType | Trigger | Severity | ResourceId Meaning |
|---|---|---|---|
| `IA.Application.Created` | Application record created | Informational | Application ID |
| `IA.Application.Updated` | Application record saved | Informational | Application ID |
| `IA.Application.Deleted` | Application record removed | Warning | Application ID |
| `IA.Company.Created` | Company record created | Informational | Company ID |
| `IA.Company.Updated` | Company record saved | Informational | Company ID |

### Inquiry Authoring Events

| EventType | Trigger | Severity | ResourceId Meaning |
|---|---|---|---|
| `IO.Survey.Created` | Survey created | Informational | Survey ID |
| `IO.Survey.Updated` | Survey saved | Informational | Survey ID |
| `IO.Survey.Published` | Survey published for responses | Informational | Survey ID |
| `IO.Question.Created` | Question added to survey | Informational | Question ID |
| `IO.Question.Updated` | Question saved | Informational | Question ID |
| `IO.Question.Deleted` | Question removed | Warning | Question ID |

### Capability Completion Events (US3 lifecycle)

| EventType | Trigger | Severity | ResourceId Meaning |
|---|---|---|---|
| `UC.Parity.ValidationSubmitted` | Parity validation record submitted | Informational | Capability ID |
| `UC.Cutover.DecisionRecorded` | Go/No-Go cutover decision recorded | Warning | Domain name |
| `UC.Cutover.Reverted` | Domain cutover rolled back | Critical | Domain name |
| `UC.Phase.Advanced` | Capability phase increased | Informational | Capability ID |

### System Events

| EventType | Trigger | Severity | ResourceId Meaning |
|---|---|---|---|
| `SYS.Auth.LoginSucceeded` | User authenticated | Informational | User ID |
| `SYS.Auth.LoginFailed` | Authentication failed | Warning | Username attempted |
| `SYS.Auth.AccessDenied` | Authorization challenge triggered | Warning | Route/resource |
| `SYS.Build.Started` | Build operation started (health/ops) | Informational | null |

---

## Implementation Notes

- Events are emitted via `IUnifiedAuditService` (T049/T050)  
- `UnifiedAuditEventItem` in `InquirySpark.Common` is the canonical model (T048)  
- `UnifiedAuditService` implementation uses **ILogger pipeline only** — no EF Core, no database writes
- Structured logging: all fields are logged as named properties, not string-concatenated
