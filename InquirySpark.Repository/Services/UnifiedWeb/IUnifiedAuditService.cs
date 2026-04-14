using InquirySpark.Common.Models.UnifiedWeb;

namespace InquirySpark.Repository.Services.UnifiedWeb;

/// <summary>
/// Emits structured audit events to the application log pipeline.
/// Implementation is ILogger-only — no database writes (per FR-007 spec).
/// </summary>
public interface IUnifiedAuditService
{
    /// <summary>
    /// Emits a single audit event to the structured log pipeline.
    /// </summary>
    /// <param name="auditEvent">The populated audit event to log.</param>
    void Emit(UnifiedAuditEventItem auditEvent);

    /// <summary>
    /// Convenience factory: creates and emits an informational audit event.
    /// </summary>
    void EmitInfo(string eventType, string userId, string? resourceId = null, string? actionDetails = null, string? domain = null);

    /// <summary>
    /// Convenience factory: creates and emits a warning-level audit event.
    /// </summary>
    void EmitWarning(string eventType, string userId, string? resourceId = null, string? actionDetails = null, string? domain = null);

    /// <summary>
    /// Convenience factory: creates and emits a critical-level audit event.
    /// </summary>
    void EmitCritical(string eventType, string userId, string? resourceId = null, string? actionDetails = null, string? domain = null);
}
