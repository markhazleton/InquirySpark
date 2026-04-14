#nullable enable
using InquirySpark.Common.Models.Runtime;
using InquirySpark.Common.Models.Spec;
using InquirySpark.Common.Persistence.Repositories;

namespace InquirySpark.Repository.Services.UnifiedWeb;

/// <summary>
/// Typed facade over DecisionSpark's file-based data mechanisms for use in InquirySpark.Web.
/// Wraps IDecisionSpecRepository (spec CRUD + audit) and IConversationPersistence (session save).
/// All path validation and authorization enforcement belong here; the underlying stores trust validated input.
/// Schema authority: contracts/decisionspark-file-storage-integration.md (T001B).
/// </summary>
public interface IDecisionSparkFileStorageService
{
    // ── DecisionSpec domain ────────────────────────────────────────────────

    /// <summary>Lists DecisionSpec summaries, optionally filtered by status and search term.</summary>
    Task<IEnumerable<DecisionSpecSummary>> ListSpecsAsync(
        string? status = null,
        string? searchTerm = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a DecisionSpec document and its ETag by spec ID and optional version.
    /// Returns null if not found.
    /// </summary>
    Task<(DecisionSpecDocument Document, string ETag)?> GetSpecAsync(
        string specId,
        string? version = null,
        CancellationToken cancellationToken = default);

    /// <summary>Creates a new DecisionSpec and returns the created document with ETag.</summary>
    Task<(DecisionSpecDocument Document, string ETag)> CreateSpecAsync(
        DecisionSpecDocument spec,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing DecisionSpec with optimistic concurrency check via ETag.
    /// Returns null if not found or ETag mismatch.
    /// </summary>
    Task<(DecisionSpecDocument Document, string ETag)?> UpdateSpecAsync(
        string specId,
        DecisionSpecDocument spec,
        string ifMatchETag,
        CancellationToken cancellationToken = default);

    /// <summary>Soft-deletes a DecisionSpec (moves to archive). Returns false if not found.</summary>
    Task<bool> DeleteSpecAsync(
        string specId,
        string version,
        string ifMatchETag,
        CancellationToken cancellationToken = default);

    /// <summary>Restores a soft-deleted DecisionSpec from archive. Returns null if not found.</summary>
    Task<(DecisionSpecDocument Document, string ETag)?> RestoreSpecAsync(
        string specId,
        string version,
        CancellationToken cancellationToken = default);

    /// <summary>Transitions a DecisionSpec to a new lifecycle status (Draft → InReview → Published → Retired).</summary>
    Task TransitionSpecStatusAsync(
        string specId,
        string version,
        string newStatus,
        string comment,
        string actor,
        CancellationToken cancellationToken = default);

    /// <summary>Returns the full audit history for a DecisionSpec.</summary>
    Task<IEnumerable<AuditEntry>> GetSpecAuditHistoryAsync(
        string specId,
        CancellationToken cancellationToken = default);

    // ── Conversation domain ───────────────────────────────────────────────

    /// <summary>
    /// Persists a conversation session snapshot to disk. Best-effort: does not throw on failure.
    /// </summary>
    Task SaveConversationAsync(
        DecisionSession session,
        CancellationToken cancellationToken = default);
}
