#nullable enable
using InquirySpark.Common.Models.UnifiedWeb;
using InquirySpark.Repository.Services.UnifiedWeb;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace InquirySpark.Common.Tests.UnifiedWeb;

/// <summary>
/// Unit tests for <see cref="UnifiedAuditService"/> (T055A — US4 audit pipeline).
/// Validates: emit routing, severity-to-log-level mapping, convenience factories.
/// Uses a capturing logger to verify structured log output.
/// </summary>
[TestClass]
public class US4AuditServiceTests
{
    // ── Logger capture helper ─────────────────────────────────────────────

    private sealed class CapturingLogger : ILogger<UnifiedAuditService>
    {
        public List<(LogLevel Level, string Message)> Entries { get; } = [];

        IDisposable ILogger.BeginScope<TState>(TState state) => NullScope.Instance;
        bool ILogger.IsEnabled(LogLevel logLevel) => true;

        void ILogger.Log<TState>(LogLevel logLevel, EventId eventId, TState state,
            Exception? exception, Func<TState, Exception?, string> formatter)
        {
            Entries.Add((logLevel, formatter(state, exception)));
        }

        private sealed class NullScope : IDisposable
        {
            public static readonly NullScope Instance = new();
            private NullScope() { }
            public void Dispose() { }
        }
    }

    private static (UnifiedAuditService Service, CapturingLogger Logger) BuildService()
    {
        var capturingLogger = new CapturingLogger();
        return (new UnifiedAuditService(capturingLogger), capturingLogger);
    }

    // ── Emit routing ──────────────────────────────────────────────────────

    [TestMethod]
    public void Emit_InformationalSeverity_LogsAtInformationLevel()
    {
        var (service, logger) = BuildService();

        service.Emit(new UnifiedAuditEventItem
        {
            EventType = "UC.Parity.ValidationSubmitted",
            UserId = "user1",
            Severity = "Informational",
        });

        Assert.AreEqual(1, logger.Entries.Count);
        Assert.AreEqual(LogLevel.Information, logger.Entries[0].Level);
    }

    [TestMethod]
    public void Emit_WarningSeverity_LogsAtWarningLevel()
    {
        var (service, logger) = BuildService();

        service.Emit(new UnifiedAuditEventItem
        {
            EventType = "UC.Cutover.DecisionRecorded",
            UserId = "approver1",
            Severity = "Warning",
        });

        Assert.AreEqual(1, logger.Entries.Count);
        Assert.AreEqual(LogLevel.Warning, logger.Entries[0].Level);
    }

    [TestMethod]
    public void Emit_CriticalSeverity_LogsAtCriticalLevel()
    {
        var (service, logger) = BuildService();

        service.Emit(new UnifiedAuditEventItem
        {
            EventType = "UC.Cutover.Reverted",
            UserId = "ops-lead",
            Severity = "Critical",
        });

        Assert.AreEqual(1, logger.Entries.Count);
        Assert.AreEqual(LogLevel.Critical, logger.Entries[0].Level);
    }

    [TestMethod]
    public void Emit_UnknownSeverity_DefaultsToInformationLevel()
    {
        var (service, logger) = BuildService();

        service.Emit(new UnifiedAuditEventItem
        {
            EventType = "SYS.Build.Started",
            UserId = "system",
            Severity = "Unknown",
        });

        Assert.AreEqual(LogLevel.Information, logger.Entries[0].Level);
    }

    // ── Convenience factories ─────────────────────────────────────────────

    [TestMethod]
    public void EmitInfo_ProducesInformationLevelEntry()
    {
        var (service, logger) = BuildService();

        service.EmitInfo("UC.Parity.ValidationSubmitted", "user1", resourceId: "CAP-DS-001");

        Assert.AreEqual(1, logger.Entries.Count);
        Assert.AreEqual(LogLevel.Information, logger.Entries[0].Level);
    }

    [TestMethod]
    public void EmitWarning_ProducesWarningLevelEntry()
    {
        var (service, logger) = BuildService();

        service.EmitWarning("UC.Cutover.DecisionRecorded", "approver1", domain: "Decision Workspace");

        Assert.AreEqual(1, logger.Entries.Count);
        Assert.AreEqual(LogLevel.Warning, logger.Entries[0].Level);
    }

    [TestMethod]
    public void EmitCritical_ProducesCriticalLevelEntry()
    {
        var (service, logger) = BuildService();

        service.EmitCritical("UC.Cutover.Reverted", "ops-lead", resourceId: "Decision Workspace",
            actionDetails: "Rollback triggered by SC-004 breach", domain: "Decision Workspace");

        Assert.AreEqual(1, logger.Entries.Count);
        Assert.AreEqual(LogLevel.Critical, logger.Entries[0].Level);
    }

    // ── Multiple emissions ────────────────────────────────────────────────

    [TestMethod]
    public void MultipleEmits_AllEntriesAppear()
    {
        var (service, logger) = BuildService();

        service.EmitInfo("E1", "u1");
        service.EmitWarning("E2", "u2");
        service.EmitCritical("E3", "u3");

        Assert.AreEqual(3, logger.Entries.Count);
        Assert.AreEqual(LogLevel.Information, logger.Entries[0].Level);
        Assert.AreEqual(LogLevel.Warning, logger.Entries[1].Level);
        Assert.AreEqual(LogLevel.Critical, logger.Entries[2].Level);
    }

    // ── Null-object pattern ───────────────────────────────────────────────

    [TestMethod]
    public void NullUnifiedAuditService_DoesNotThrow()
    {
        // NullUnifiedAuditService is used as default when no audit service is registered
        var nullService = NullUnifiedAuditService.Instance;

        // All methods should silently no-op
        nullService.EmitInfo("test-event", "user1");
        nullService.EmitWarning("test-event", "user1");
        nullService.EmitCritical("test-event", "user1");
        nullService.Emit(new UnifiedAuditEventItem { EventType = "test", UserId = "user1" });

        // All methods should silently no-op — no exception thrown means success
    }
}
