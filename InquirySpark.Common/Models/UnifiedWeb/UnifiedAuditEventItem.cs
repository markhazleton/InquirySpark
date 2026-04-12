namespace InquirySpark.Common.Models.UnifiedWeb;

/// <summary>
/// Represents a single audit event emitted by the unified web capability pipeline.
/// Schema authority: see audit-event-types.md (T047C).
/// All events are logged via ILogger structured logging — no EF Core writes.
/// </summary>
public sealed class UnifiedAuditEventItem
{
    /// <summary>Canonical event type identifier from the audit event catalog.</summary>
    public string EventType { get; set; } = string.Empty;

    /// <summary>Identity of the acting user.</summary>
    public string UserId { get; set; } = string.Empty;

    /// <summary>UTC time the event was recorded.</summary>
    public DateTimeOffset Timestamp { get; set; } = DateTimeOffset.UtcNow;

    /// <summary>Request or operation correlation identifier.</summary>
    public string CorrelationId { get; set; } = Guid.NewGuid().ToString();

    /// <summary>Identifier of the resource acted upon (null for system-level events).</summary>
    public string ResourceId { get; set; }

    /// <summary>Human-readable description of what happened.</summary>
    public string ActionDetails { get; set; }

    /// <summary>Capability domain context (e.g., "Decision Workspace").</summary>
    public string Domain { get; set; }

    /// <summary>Source application that emitted the event.</summary>
    public string Source { get; set; } = "InquirySpark.Web";

    /// <summary>Severity level: Informational, Warning, or Critical.</summary>
    public string Severity { get; set; } = "Informational";
}
