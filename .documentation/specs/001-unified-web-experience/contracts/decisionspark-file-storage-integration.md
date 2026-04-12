# DecisionSpark File-Storage Integration

**Feature**: `001-unified-web-experience`
**Created**: 2026-04-12
**Task**: T001B — Document DecisionSpark file-storage access patterns
**Authority**: This document is the schema authority for T004C (`IDecisionSparkFileStorageService`) and T004D (`DecisionSparkFileStorageService`).

---

## Overview

DecisionSpark uses **exclusively file-based storage** — no database engine of any kind. All persistent state lives in JSON files on the local filesystem. The integration path into InquirySpark.Web must wrap these file operations behind a typed service interface (`IDecisionSparkFileStorageService`) so that controllers can use them without taking direct filesystem dependencies.

All underlying infrastructure already lives in `InquirySpark.Common` and is reused as-is. The new service interface acts as a composition boundary between InquirySpark.Web and the existing Common persistence layer.

---

## Data Domains and File Layout

### Domain 1: DecisionSpec Documents

**Purpose**: JSON documents describing a decision tree — traits (questions), outcomes, selection rules, and metadata.

**Root config key**: `DecisionSpecs:RootPath` (e.g., `C:\websites\DecisionSpark\DecisionSpecs`)

**Directory layout**:

```
{RootPath}/
├── draft/
│   ├── {specId}.{version}.Draft.json          ← spec document
│   └── {specId}.{version}.metadata.json       ← sidecar metadata
├── inreview/
│   ├── {specId}.{version}.InReview.json
│   └── {specId}.{version}.metadata.json
├── published/
│   ├── {specId}.{version}.Published.json
│   └── {specId}.{version}.metadata.json
├── retired/
│   ├── {specId}.{version}.Retired.json
│   └── {specId}.{version}.metadata.json
├── archive/
│   └── {specId}.{version}.{status}.json.{timestamp}   ← soft-deleted
└── drafts/
    └── {draftId}.json                         ← LLM-generated pending drafts
```

**Schema**: `DecisionSpecDocument` (see `InquirySpark.Common/Core/Models/Spec/DecisionSpecDocument.cs`)

Key fields: `SpecId`, `Version`, `Status`, `Metadata` (Name, Description, Tags), `Traits[]` (Key, AnswerType, QuestionText, Options[], Mapping, Bounds, DependsOn), `Outcomes[]` (OutcomeId, SelectionRules, DisplayCards), `Audit[]` (AuditEntry records)

**Naming convention**: `{specId}.{version}.{status}.json` — status is always PascalCase (`Draft`, `InReview`, `Published`, `Retired`)

**Concurrency**: Writes are protected by a `SemaphoreSlim(1,1)` in `DecisionSpecFileStore`. Reads are unlocked. ETags are SHA-256 hashes of the file content used for optimistic concurrency on updates.

**Lifecycle transitions**: Draft → InReview → Published → Retired (via `StatusTransitionRequest`). Files move between status directories on transition; metadata sidecar moves with them.

**Soft delete**: Moves the file to `archive/` with a timestamp suffix. Restorable within `SoftDeleteRetentionDays` (default: 30).

**Existing services (InquirySpark.Common)**:
- `DecisionSpecFileStore` — atomic write, read, soft-delete, restore, list-by-status
- `DecisionSpecMetadataStore` — sidecar metadata save, load, move, delete
- `IDecisionSpecRepository` / `DecisionSpecRepository` — full CRUD with audit history, uses both stores above

---

### Domain 2: Conversation Sessions

**Purpose**: Snapshots of in-progress decision conversations (session state saved to disk for diagnostics/replay).

**Root config key**: `ConversationStorage:Path` (e.g., `C:\websites\DecisionSpark\conversations`)

**Directory layout**:

```
{ConversationStorage:Path}/
└── {sessionId}.json        ← one file per session
```

**Schema**: `DecisionSession` (see `InquirySpark.Common/Core/Models/Runtime/`)

**Write semantics**: Append/overwrite on `SaveConversationAsync`. Persistence is best-effort — failures are logged but do not throw.

**Read semantics**: Not required at runtime (sessions are in-memory; files are for diagnostic/replay use). No read interface is currently exposed.

**Existing service**: `IConversationPersistence` / `FileConversationPersistence`

---

### Domain 3: Search Index

**Purpose**: A pre-built search index of all DecisionSpec documents for fast text search without reading every file.

**Config key**: `DecisionSpecs:IndexFileName` (default: `DecisionSpecIndex.json`) stored under `{RootPath}`

**File layout**:

```
{RootPath}/
└── DecisionSpecIndex.json
```

**Refresh strategy**: `IndexRefreshHostedService` refreshes the index on startup and periodically.

**Existing service**: `FileSearchIndexer`

---

## Configuration Structure

```json
{
  "DecisionSpecs": {
    "RootPath": "path/to/DecisionSpecs",
    "LegacyConfigPath": "path/to/legacy/configurations",
    "SoftDeleteRetentionDays": 30,
    "IndexFileName": "DecisionSpecIndex.json"
  },
  "ConversationStorage": {
    "Path": "path/to/conversations"
  }
}
```

These keys must be present in `InquirySpark.Web/appsettings.json` (T017) and `appsettings.Development.json` (T018).

---

## Security Constraints

1. **File path validation**: All `specId` and `version` values used in file path construction must be validated against a safe pattern (`[A-Za-z0-9_\-\.]+`) before use to prevent path traversal. The existing `DecisionSpecFileStore` does NOT currently include explicit traversal guards — the integration service (T004C/T004D) MUST validate inputs.
2. **Authorization**: In InquirySpark.Web, file-storage operations must be gated behind the Identity authorization policies from T016A/T016B. The API-key middleware used by DecisionSpark is NOT carried forward.
3. **Path isolation**: The `RootPath` must be outside the web root (`wwwroot/`). Deployment configuration must ensure the process has read/write access to the configured paths.

---

## Interface Design Requirements for T004C

`IDecisionSparkFileStorageService` must expose typed operations for each domain:

**DecisionSpec domain**:
- `Task<IEnumerable<DecisionSpecSummary>> ListSpecsAsync(string? status, string? searchTerm, CancellationToken ct)`
- `Task<(DecisionSpecDocument Document, string ETag)?> GetSpecAsync(string specId, string? version, CancellationToken ct)`
- `Task<(DecisionSpecDocument Document, string ETag)> CreateSpecAsync(DecisionSpecDocument spec, CancellationToken ct)`
- `Task<(DecisionSpecDocument Document, string ETag)?> UpdateSpecAsync(string specId, DecisionSpecDocument spec, string ifMatchETag, CancellationToken ct)`
- `Task<bool> DeleteSpecAsync(string specId, string version, string ifMatchETag, CancellationToken ct)`
- `Task<(DecisionSpecDocument Document, string ETag)?> RestoreSpecAsync(string specId, string version, CancellationToken ct)`
- `Task TransitionSpecStatusAsync(string specId, string version, string newStatus, string comment, CancellationToken ct)`
- `Task<IEnumerable<AuditEntry>> GetSpecAuditHistoryAsync(string specId, CancellationToken ct)`

**Conversation domain**:
- `Task SaveConversationAsync(DecisionSession session, CancellationToken ct)` (best-effort, does not throw)

The implementation (`DecisionSparkFileStorageService`, T004D) delegates to `IDecisionSpecRepository` and `IConversationPersistence` from `InquirySpark.Common` — it is a thin facade, not a reimplementation.

---

## Implementation Notes for T004D

- Register `DecisionSparkFileStorageService` as `Singleton` (matching the lifetime of the underlying `IDecisionSpecRepository`).
- The implementation must call `builder.Services.AddOptions<DecisionSpecsOptions>().BindConfiguration(DecisionSpecsOptions.SectionName).ValidateOnStart()` before registration (this belongs in T016, the DI registration task).
- Do not re-register `DecisionSpecFileStore`, `DecisionSpecMetadataStore`, or `FileSearchIndexer` directly — they are internal dependencies of `IDecisionSpecRepository` which is already registered. Delegate through `IDecisionSpecRepository`.
- `IConversationPersistence` must be registered as Scoped (it reads from `IConfiguration` in its constructor).
