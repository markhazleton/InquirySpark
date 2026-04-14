#nullable enable
using InquirySpark.Common.Models;
using InquirySpark.Common.Models.UnifiedWeb;

namespace InquirySpark.Repository.Services.UnifiedWeb;

/// <summary>
/// Service contract for bridging identity and role context during the phased capability completion.
/// Maps legacy application roles to unified Identity roles and provides cross-identity context
/// for controllers that need to resolve user context from both legacy surfaces.
/// The canonical identity authority is ControlSparkUserContextConnection (Identity SQLite) per FR-015.
/// This bridge supports phased transition only and does NOT introduce a parallel long-term sign-in model.
/// </summary>
public interface IIdentityMigrationBridgeService
{
    /// <summary>
    /// Returns the unified role(s) mapped from a legacy application role.
    /// Returns an empty collection if no mapping is defined.
    /// </summary>
    Task<BaseResponseCollection<string>> GetUnifiedRolesForLegacyRoleAsync(
        string sourceApp,
        string sourceRole,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Returns all role mappings for a given source application.
    /// </summary>
    Task<BaseResponseCollection<RoleMappingItem>> GetRoleMappingsForAppAsync(
        string sourceApp,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Verifies that a user in the unified Identity store has the permissions equivalent
    /// to those expected from a given legacy role, using the configured role mapping.
    /// Returns true if parity is confirmed; false if a privilege gap is detected.
    /// </summary>
    Task<BaseResponse<bool>> VerifyRoleParityAsync(
        string userId,
        string sourceApp,
        string sourceRole,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Returns all role mappings that have non-equivalent privilege mappings
    /// (i.e., where IsEquivalent = false) and require manual review.
    /// </summary>
    Task<BaseResponseCollection<RoleMappingItem>> GetPrivilegeEscalationRisksAsync(
        CancellationToken cancellationToken = default);
}
