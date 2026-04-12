#nullable enable
using InquirySpark.Common.Models;
using InquirySpark.Common.Models.UnifiedWeb;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace InquirySpark.Repository.Services.UnifiedWeb;

/// <summary>
/// Implements capability-completion governance for InquirySpark.Web.
/// Retrieves capability tracking state from IOptions/config — no DB queries.
/// Audit events are emitted via ILogger pipeline per constitution (no EF write).
/// </summary>
public sealed class UnifiedWebCapabilityService : IUnifiedWebCapabilityService
{
    private readonly UnifiedWebOptions _options;
    private readonly ILogger<UnifiedWebCapabilityService> _logger;
    private readonly IUnifiedAuditService _audit;

    // In-memory runtime state (not persisted to DB per spec constraint)
    private readonly List<ParityValidationRecordItem> _parityRecords = [];
    private readonly List<CutoverDecisionRecordItem> _cutoverDecisions = [];

    /// <summary>Initializes a new instance of <see cref="UnifiedWebCapabilityService"/>.</summary>
    public UnifiedWebCapabilityService(
        IOptions<UnifiedWebOptions> options,
        ILogger<UnifiedWebCapabilityService> logger,
        IUnifiedAuditService? auditService = null)
    {
        _options = options.Value;
        _logger = logger;
        _audit = auditService ?? NullUnifiedAuditService.Instance;
    }

    // ── Capability inventory ──────────────────────────────────────────────

    /// <inheritdoc/>
    public Task<BaseResponseCollection<CapabilityItem>> GetCapabilityInventoryAsync(
        CancellationToken cancellationToken = default)
    {
        var items = _options.CapabilityCompletion.Capabilities ?? [];
        return Task.FromResult(new BaseResponseCollection<CapabilityItem>(items));
    }

    /// <inheritdoc/>
    public Task<BaseResponseCollection<CapabilityItem>> GetCapabilitiesByDomainAsync(
        string domain,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(domain);
        var items = (_options.CapabilityCompletion.Capabilities ?? [])
            .Where(c => string.Equals(c.Domain, domain, StringComparison.OrdinalIgnoreCase))
            .ToList();
        return Task.FromResult(new BaseResponseCollection<CapabilityItem>(items));
    }

    // ── Parity validation ─────────────────────────────────────────────────

    /// <inheritdoc/>
    public Task<BaseResponse<ParityValidationRecordItem>> SubmitParityValidationAsync(
        ParityValidationRecordItem record,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(record);

        _parityRecords.Add(record);

        _logger.LogInformation(
            "[UnifiedWeb] ParityValidation recorded for {CapabilityId} by {ValidatedBy}. " +
            "Functional={F} Permission={P} UX={U} Perf={Perf} FullyValidated={Valid}",
            record.CapabilityId, record.ValidatedBy,
            record.FunctionalParityPassed, record.PermissionParityPassed,
            record.UxConsistencyPassed, record.PerformancePassed, record.IsFullyValidated);

        _audit.EmitInfo("UC.Parity.ValidationSubmitted", record.ValidatedBy,
            resourceId: record.CapabilityId,
            actionDetails: $"FullyValidated={record.IsFullyValidated}");

        if (record.IsFullyValidated)
        {
            AdvanceCapabilityPhase(record.CapabilityId, 3, "validated");
        }

        return Task.FromResult(new BaseResponse<ParityValidationRecordItem>(record));
    }

    // ── Phase status ──────────────────────────────────────────────────────

    /// <inheritdoc/>
    public Task<BaseResponse<CapabilityItem>> GetCapabilityPhaseStatusAsync(
        string capabilityId,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(capabilityId);

        var item = (_options.CapabilityCompletion.Capabilities ?? [])
            .FirstOrDefault(c => string.Equals(c.CapabilityId, capabilityId, StringComparison.OrdinalIgnoreCase));

        return Task.FromResult(item is null
            ? new BaseResponse<CapabilityItem>(new[] { $"Capability '{capabilityId}' not found in configuration." })
            : new BaseResponse<CapabilityItem>(item));
    }

    /// <inheritdoc/>
    public Task<BaseResponseCollection<CapabilityPhaseItem>> GetCompletionPhasesAsync(
        CancellationToken cancellationToken = default)
    {
        var phases = _options.CapabilityCompletion.Phases ?? [];
        return Task.FromResult(new BaseResponseCollection<CapabilityPhaseItem>(phases));
    }

    // ── Cutover decisions ─────────────────────────────────────────────────

    /// <inheritdoc/>
    public Task<BaseResponse<CutoverDecisionRecordItem>> RecordCutoverDecisionAsync(
        CutoverDecisionRecordItem decision,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(decision);

        _cutoverDecisions.Add(decision);

        _logger.LogInformation(
            "[UnifiedWeb] CutoverDecision recorded for domain {Domain} legacyApp={LegacyApp} " +
            "decision={Decision} approvedBy={ApprovedBy}",
            decision.Domain, decision.LegacyApp, decision.Decision, decision.ApprovedBy);

        _audit.EmitWarning("UC.Cutover.DecisionRecorded", decision.ApprovedBy,
            resourceId: decision.Domain,
            actionDetails: $"Decision={decision.Decision} LegacyApp={decision.LegacyApp}",
            domain: decision.Domain);

        if (string.Equals(decision.Decision, "Go", StringComparison.OrdinalIgnoreCase))
        {
            AdvanceDomainCapabilitiesToCutOver(decision.Domain);
        }

        return Task.FromResult(new BaseResponse<CutoverDecisionRecordItem>(decision));
    }

    // ── Rollback ──────────────────────────────────────────────────────────

    /// <inheritdoc/>
    public Task<BaseResponse<bool>> RevertDomainCutoverAsync(
        string domain,
        string revertedBy,
        string reason,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(domain);
        ArgumentException.ThrowIfNullOrWhiteSpace(revertedBy);
        ArgumentException.ThrowIfNullOrWhiteSpace(reason);

        // Revert cutover status for all capabilities in the domain
        var domainCapabilities = (_options.CapabilityCompletion.Capabilities ?? [])
            .Where(c => string.Equals(c.Domain, domain, StringComparison.OrdinalIgnoreCase))
            .ToList();

        foreach (var cap in domainCapabilities)
        {
            if (cap.Phase >= 4)
            {
                cap.Phase = 3;
                cap.Status = "validated";
                cap.Notes = $"Rolled back from CutOver by {revertedBy} at {DateTimeOffset.UtcNow:O}. Reason: {reason}";
            }
        }

        // Mark any Go cutover decisions as reverted
        foreach (var decision in _cutoverDecisions
            .Where(d => string.Equals(d.Domain, domain, StringComparison.OrdinalIgnoreCase) && d.IsCutOver))
        {
            decision.IsCutOver = false;
        }

        _logger.LogWarning(
            "[UnifiedWeb] ROLLBACK: Domain {Domain} cutover reverted by {RevertedBy}. Reason: {Reason}",
            domain, revertedBy, reason);

        _audit.EmitCritical("UC.Cutover.Reverted", revertedBy,
            resourceId: domain,
            actionDetails: reason,
            domain: domain);

        return Task.FromResult(new BaseResponse<bool>(true));
    }

    // ── Private helpers ───────────────────────────────────────────────────

    private void AdvanceCapabilityPhase(string capabilityId, int phase, string status)
    {
        var cap = (_options.CapabilityCompletion.Capabilities ?? [])
            .FirstOrDefault(c => string.Equals(c.CapabilityId, capabilityId, StringComparison.OrdinalIgnoreCase));

        if (cap is not null)
        {
            cap.Phase = phase;
            cap.Status = status;
            _logger.LogInformation("[UnifiedWeb] Capability {CapabilityId} advanced to Phase {Phase} ({Status})",
                capabilityId, phase, status);
        }
    }

    private void AdvanceDomainCapabilitiesToCutOver(string domain)
    {
        foreach (var cap in (_options.CapabilityCompletion.Capabilities ?? [])
            .Where(c => string.Equals(c.Domain, domain, StringComparison.OrdinalIgnoreCase)
                        && c.Phase >= 3))
        {
            cap.Phase = 4;
            cap.Status = "cut-over";
        }
    }
}

// ── Options models ────────────────────────────────────────────────────────────

/// <summary>Root options model for unified web configuration (appsettings.json → "UnifiedWeb").</summary>
public sealed class UnifiedWebOptions
{
    /// <summary>Gets or sets the capability completion tracking configuration.</summary>
    public CapabilityCompletionOptions CapabilityCompletion { get; set; } = new();
}

/// <summary>Options for the capability completion tracking sub-section.</summary>
public sealed class CapabilityCompletionOptions
{
    /// <summary>Gets or sets the list of all inventoried capabilities.</summary>
    public List<CapabilityItem>? Capabilities { get; set; }

    /// <summary>Gets or sets the defined capability completion phases.</summary>
    public List<CapabilityPhaseItem>? Phases { get; set; }
}
