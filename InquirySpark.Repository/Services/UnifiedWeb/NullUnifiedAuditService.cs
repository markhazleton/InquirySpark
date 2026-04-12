using InquirySpark.Common.Models.UnifiedWeb;

namespace InquirySpark.Repository.Services.UnifiedWeb;

/// <summary>
/// No-op implementation of <see cref="IUnifiedAuditService"/> used as a default
/// when no audit service is registered. Enables tests to construct
/// <see cref="UnifiedWebCapabilityService"/> without registering a mock audit service.
/// </summary>
public sealed class NullUnifiedAuditService : IUnifiedAuditService
{
    /// <summary>Singleton no-op instance.</summary>
    public static readonly IUnifiedAuditService Instance = new NullUnifiedAuditService();

    private NullUnifiedAuditService() { }

    /// <inheritdoc/>
    public void Emit(UnifiedAuditEventItem auditEvent) { }

    /// <inheritdoc/>
    public void EmitInfo(string eventType, string userId, string? resourceId = null, string? actionDetails = null, string? domain = null) { }

    /// <inheritdoc/>
    public void EmitWarning(string eventType, string userId, string? resourceId = null, string? actionDetails = null, string? domain = null) { }

    /// <inheritdoc/>
    public void EmitCritical(string eventType, string userId, string? resourceId = null, string? actionDetails = null, string? domain = null) { }
}
