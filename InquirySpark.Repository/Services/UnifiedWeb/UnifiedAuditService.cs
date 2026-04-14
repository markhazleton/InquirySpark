using InquirySpark.Common.Models.UnifiedWeb;
using Microsoft.Extensions.Logging;

namespace InquirySpark.Repository.Services.UnifiedWeb;

/// <summary>
/// ILogger-pipeline-only implementation of <see cref="IUnifiedAuditService"/>.
/// All audit events are emitted as structured log entries — no EF Core, no database writes.
/// </summary>
public sealed class UnifiedAuditService(ILogger<UnifiedAuditService> logger) : IUnifiedAuditService
{
    private readonly ILogger<UnifiedAuditService> _logger = logger;

    /// <inheritdoc/>
    public void Emit(UnifiedAuditEventItem auditEvent)
    {
        var level = auditEvent.Severity switch
        {
            "Critical" => LogLevel.Critical,
            "Warning" => LogLevel.Warning,
            _ => LogLevel.Information,
        };

        _logger.Log(
            level,
            "[Audit] {EventType} | User={UserId} | Resource={ResourceId} | Domain={Domain} | Correlation={CorrelationId} | Source={Source} | Details={ActionDetails}",
            auditEvent.EventType,
            auditEvent.UserId,
            auditEvent.ResourceId ?? "(none)",
            auditEvent.Domain ?? "(none)",
            auditEvent.CorrelationId,
            auditEvent.Source,
            auditEvent.ActionDetails ?? string.Empty
        );
    }

    /// <inheritdoc/>
    public void EmitInfo(string eventType, string userId, string? resourceId = null, string? actionDetails = null, string? domain = null)
        => Emit(new UnifiedAuditEventItem
        {
            EventType = eventType,
            UserId = userId,
            ResourceId = resourceId,
            ActionDetails = actionDetails,
            Domain = domain,
            Severity = "Informational",
        });

    /// <inheritdoc/>
    public void EmitWarning(string eventType, string userId, string? resourceId = null, string? actionDetails = null, string? domain = null)
        => Emit(new UnifiedAuditEventItem
        {
            EventType = eventType,
            UserId = userId,
            ResourceId = resourceId,
            ActionDetails = actionDetails,
            Domain = domain,
            Severity = "Warning",
        });

    /// <inheritdoc/>
    public void EmitCritical(string eventType, string userId, string? resourceId = null, string? actionDetails = null, string? domain = null)
        => Emit(new UnifiedAuditEventItem
        {
            EventType = eventType,
            UserId = userId,
            ResourceId = resourceId,
            ActionDetails = actionDetails,
            Domain = domain,
            Severity = "Critical",
        });
}
