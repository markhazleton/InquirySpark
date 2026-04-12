#nullable enable
using InquirySpark.Common.Models;
using InquirySpark.Common.Models.UnifiedWeb;

namespace InquirySpark.Repository.Services.UnifiedWeb;

/// <summary>
/// Service contract for capability-completion governance in InquirySpark.Web.
/// Manages the capability inventory, parity validation records, cutover decisions,
/// and rollback controls for the phased capability-completion process (FR-005, FR-006, FR-010).
/// State is sourced from IOptions/config — no database queries.
/// </summary>
public interface IUnifiedWebCapabilityService
{
    // ── Capability inventory ──────────────────────────────────────────────

    /// <summary>Returns all inventoried capabilities from the configuration.</summary>
    Task<BaseResponseCollection<CapabilityItem>> GetCapabilityInventoryAsync(CancellationToken cancellationToken = default);

    /// <summary>Returns capabilities filtered by domain.</summary>
    Task<BaseResponseCollection<CapabilityItem>> GetCapabilitiesByDomainAsync(string domain, CancellationToken cancellationToken = default);

    // ── Parity validation ─────────────────────────────────────────────────

    /// <summary>
    /// Records a parity validation result for a capability and advances its phase if fully validated.
    /// Emits an audit log entry on success.
    /// </summary>
    Task<BaseResponse<ParityValidationRecordItem>> SubmitParityValidationAsync(
        ParityValidationRecordItem record,
        CancellationToken cancellationToken = default);

    // ── Phase status ──────────────────────────────────────────────────────

    /// <summary>Returns the current capability completion phase status for a given capability ID.</summary>
    Task<BaseResponse<CapabilityItem>> GetCapabilityPhaseStatusAsync(string capabilityId, CancellationToken cancellationToken = default);

    /// <summary>Returns all defined capability completion phases.</summary>
    Task<BaseResponseCollection<CapabilityPhaseItem>> GetCompletionPhasesAsync(CancellationToken cancellationToken = default);

    // ── Cutover decisions ─────────────────────────────────────────────────

    /// <summary>
    /// Records a go/no-go cutover decision for a domain and emits an audit log entry.
    /// </summary>
    Task<BaseResponse<CutoverDecisionRecordItem>> RecordCutoverDecisionAsync(
        CutoverDecisionRecordItem decision,
        CancellationToken cancellationToken = default);

    // ── Rollback ──────────────────────────────────────────────────────────

    /// <summary>
    /// Reverts the cutover status for a domain and re-enables legacy access flags.
    /// Scope: cutover-status reversal and legacy-access re-enablement only.
    /// Data-integrity verification is performed separately per T047A1.
    /// </summary>
    Task<BaseResponse<bool>> RevertDomainCutoverAsync(string domain, string revertedBy, string reason, CancellationToken cancellationToken = default);
}
