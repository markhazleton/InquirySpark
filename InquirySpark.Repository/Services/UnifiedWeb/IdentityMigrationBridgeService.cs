#nullable enable
using InquirySpark.Common.Models;
using InquirySpark.Common.Models.UnifiedWeb;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace InquirySpark.Repository.Services.UnifiedWeb;

/// <summary>
/// Implements identity role-mapping and parity verification for the phased capability completion.
/// Reads role mappings from IOptions/config (no DB queries per spec constraint).
/// Canonical identity authority: ControlSparkUserContextConnection (Identity SQLite) per FR-015.
/// </summary>
public sealed class IdentityMigrationBridgeService : IIdentityMigrationBridgeService
{
    private readonly IdentityBridgeOptions _options;
    private readonly ILogger<IdentityMigrationBridgeService> _logger;

    /// <summary>Initializes a new instance of <see cref="IdentityMigrationBridgeService"/>.</summary>
    public IdentityMigrationBridgeService(
        IOptions<IdentityBridgeOptions> options,
        ILogger<IdentityMigrationBridgeService> logger)
    {
        _options = options.Value;
        _logger = logger;
    }

    /// <inheritdoc/>
    public Task<BaseResponseCollection<string>> GetUnifiedRolesForLegacyRoleAsync(
        string sourceApp,
        string sourceRole,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(sourceApp);
        ArgumentException.ThrowIfNullOrWhiteSpace(sourceRole);

        var unifiedRoles = (_options.RoleMappings ?? [])
            .Where(m => string.Equals(m.SourceApp, sourceApp, StringComparison.OrdinalIgnoreCase)
                     && string.Equals(m.SourceRole, sourceRole, StringComparison.OrdinalIgnoreCase))
            .Select(m => m.UnifiedRole)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        _logger.LogDebug(
            "[IdentityBridge] GetUnifiedRolesForLegacyRole app={App} role={Role} -> {Count} mapped roles",
            sourceApp, sourceRole, unifiedRoles.Count);

        return Task.FromResult(new BaseResponseCollection<string>(unifiedRoles));
    }

    /// <inheritdoc/>
    public Task<BaseResponseCollection<RoleMappingItem>> GetRoleMappingsForAppAsync(
        string sourceApp,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(sourceApp);

        var mappings = (_options.RoleMappings ?? [])
            .Where(m => string.Equals(m.SourceApp, sourceApp, StringComparison.OrdinalIgnoreCase))
            .ToList();

        return Task.FromResult(new BaseResponseCollection<RoleMappingItem>(mappings));
    }

    /// <inheritdoc/>
    public Task<BaseResponse<bool>> VerifyRoleParityAsync(
        string userId,
        string sourceApp,
        string sourceRole,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(userId);
        ArgumentException.ThrowIfNullOrWhiteSpace(sourceApp);
        ArgumentException.ThrowIfNullOrWhiteSpace(sourceRole);

        var mapping = (_options.RoleMappings ?? [])
            .FirstOrDefault(m => string.Equals(m.SourceApp, sourceApp, StringComparison.OrdinalIgnoreCase)
                              && string.Equals(m.SourceRole, sourceRole, StringComparison.OrdinalIgnoreCase));

        if (mapping is null)
        {
            _logger.LogWarning(
                "[IdentityBridge] No role mapping found for app={App} role={Role} userId={UserId}",
                sourceApp, sourceRole, userId);
            return Task.FromResult(new BaseResponse<bool>($"No role mapping defined for {sourceApp}/{sourceRole}."));
        }

        if (!mapping.IsEquivalent)
        {
            _logger.LogWarning(
                "[IdentityBridge] Non-equivalent role mapping for app={App} role={Role} -> {UnifiedRole}. Manual review required.",
                sourceApp, sourceRole, mapping.UnifiedRole);
        }

        return Task.FromResult(new BaseResponse<bool>(mapping.IsEquivalent));
    }

    /// <inheritdoc/>
    public Task<BaseResponseCollection<RoleMappingItem>> GetPrivilegeEscalationRisksAsync(
        CancellationToken cancellationToken = default)
    {
        var risks = (_options.RoleMappings ?? [])
            .Where(m => !m.IsEquivalent)
            .ToList();

        if (risks.Count > 0)
        {
            _logger.LogWarning("[IdentityBridge] {Count} non-equivalent role mappings require review.", risks.Count);
        }

        return Task.FromResult(new BaseResponseCollection<RoleMappingItem>(risks));
    }
}

// ── Options model ─────────────────────────────────────────────────────────────

/// <summary>Options for the identity bridge configuration (appsettings.json → "IdentityBridge").</summary>
public sealed class IdentityBridgeOptions
{
    /// <summary>Gets or sets all configured role mappings between legacy apps and InquirySpark.Web.</summary>
    public List<RoleMappingItem>? RoleMappings { get; set; }
}
