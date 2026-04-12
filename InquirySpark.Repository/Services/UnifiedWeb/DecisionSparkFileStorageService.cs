#nullable enable
using InquirySpark.Common.Models.Runtime;
using InquirySpark.Common.Models.Spec;
using InquirySpark.Common.Persistence.Repositories;
using InquirySpark.Common.Services;
using Microsoft.Extensions.Logging;

namespace InquirySpark.Repository.Services.UnifiedWeb;

/// <summary>
/// Facade over DecisionSpark's file-based data mechanisms for use in InquirySpark.Web.
/// Delegates to IDecisionSpecRepository and IConversationPersistence from InquirySpark.Common.
/// Validates inputs at the boundary to prevent path traversal (per security constraints in T001B).
/// </summary>
public sealed class DecisionSparkFileStorageService : IDecisionSparkFileStorageService
{
    private static readonly System.Text.RegularExpressions.Regex SafeIdPattern =
        new(@"^[A-Za-z0-9_\-\.]+$", System.Text.RegularExpressions.RegexOptions.Compiled);

    private readonly IDecisionSpecRepository _specRepository;
    private readonly IConversationPersistence _conversationPersistence;
    private readonly ILogger<DecisionSparkFileStorageService> _logger;

    /// <summary>Initializes a new instance of <see cref="DecisionSparkFileStorageService"/>.</summary>
    public DecisionSparkFileStorageService(
        IDecisionSpecRepository specRepository,
        IConversationPersistence conversationPersistence,
        ILogger<DecisionSparkFileStorageService> logger)
    {
        _specRepository = specRepository;
        _conversationPersistence = conversationPersistence;
        _logger = logger;
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<DecisionSpecSummary>> ListSpecsAsync(
        string? status = null,
        string? searchTerm = null,
        CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Listing specs with status={Status} searchTerm={SearchTerm}", status, searchTerm);
        return await _specRepository.ListAsync(status, null, searchTerm, cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<(DecisionSpecDocument Document, string ETag)?> GetSpecAsync(
        string specId,
        string? version = null,
        CancellationToken cancellationToken = default)
    {
        ValidateId(specId, nameof(specId));
        if (version is not null)
        {
            ValidateId(version, nameof(version));
        }

        return await _specRepository.GetAsync(specId, version, cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<(DecisionSpecDocument Document, string ETag)> CreateSpecAsync(
        DecisionSpecDocument spec,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(spec);
        ValidateId(spec.SpecId, nameof(spec.SpecId));

        var result = await _specRepository.CreateAsync(spec, cancellationToken);
        _logger.LogInformation("Created DecisionSpec {SpecId} v{Version}", spec.SpecId, spec.Version);
        return result;
    }

    /// <inheritdoc/>
    public async Task<(DecisionSpecDocument Document, string ETag)?> UpdateSpecAsync(
        string specId,
        DecisionSpecDocument spec,
        string ifMatchETag,
        CancellationToken cancellationToken = default)
    {
        ValidateId(specId, nameof(specId));
        ArgumentNullException.ThrowIfNull(spec);
        ArgumentException.ThrowIfNullOrWhiteSpace(ifMatchETag);

        return await _specRepository.UpdateAsync(specId, spec, ifMatchETag, cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<bool> DeleteSpecAsync(
        string specId,
        string version,
        string ifMatchETag,
        CancellationToken cancellationToken = default)
    {
        ValidateId(specId, nameof(specId));
        ValidateId(version, nameof(version));
        ArgumentException.ThrowIfNullOrWhiteSpace(ifMatchETag);

        var deleted = await _specRepository.DeleteAsync(specId, version, ifMatchETag, cancellationToken);
        if (deleted)
            _logger.LogInformation("Soft-deleted DecisionSpec {SpecId} v{Version}", specId, version);
        return deleted;
    }

    /// <inheritdoc/>
    public async Task<(DecisionSpecDocument Document, string ETag)?> RestoreSpecAsync(
        string specId,
        string version,
        CancellationToken cancellationToken = default)
    {
        ValidateId(specId, nameof(specId));
        ValidateId(version, nameof(version));

        return await _specRepository.RestoreAsync(specId, version, cancellationToken);
    }

    /// <inheritdoc/>
    public async Task TransitionSpecStatusAsync(
        string specId,
        string version,
        string newStatus,
        string comment,
        string actor,
        CancellationToken cancellationToken = default)
    {
        ValidateId(specId, nameof(specId));
        ValidateId(version, nameof(version));
        ArgumentException.ThrowIfNullOrWhiteSpace(newStatus);
        ArgumentException.ThrowIfNullOrWhiteSpace(actor);

        var auditEntry = new AuditEntry
        {
            SpecId = specId,
            Action = "StatusTransition",
            Summary = $"Status transitioned to {newStatus}. {comment}".Trim(),
            Actor = actor,
            Source = "InquirySpark.Web"
        };

        await _specRepository.AppendAuditEntryAsync(specId, auditEntry, cancellationToken);
        _logger.LogInformation(
            "Transitioned DecisionSpec {SpecId} v{Version} to status {Status} by {Actor}",
            specId, version, newStatus, actor);
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<AuditEntry>> GetSpecAuditHistoryAsync(
        string specId,
        CancellationToken cancellationToken = default)
    {
        ValidateId(specId, nameof(specId));
        return await _specRepository.GetAuditHistoryAsync(specId, cancellationToken);
    }

    /// <inheritdoc/>
    public async Task SaveConversationAsync(
        DecisionSession session,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(session);
        // Best-effort — IConversationPersistence swallows exceptions internally
        await _conversationPersistence.SaveConversationAsync(session);
    }

    // ── Helpers ───────────────────────────────────────────────────────────

    private static void ValidateId(string value, string paramName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(value, paramName);
        if (!SafeIdPattern.IsMatch(value))
            throw new ArgumentException(
                $"Parameter '{paramName}' contains invalid characters. Only alphanumeric, underscore, hyphen, and dot are allowed.",
                paramName);
    }
}
