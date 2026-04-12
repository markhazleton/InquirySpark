using InquirySpark.Repository.Services.UnifiedWeb;
using Microsoft.AspNetCore.Authorization;

namespace InquirySpark.Web.Areas.Unified.Controllers;

/// <summary>
/// Displays operational readiness status across all unified capability domains.
/// Provides a go/no-go dashboard view driven by the capability completion matrix.
/// </summary>
[Area("Unified")]
[Authorize]
public sealed class OperationalReadinessController(
    IUnifiedWebCapabilityService capabilityService,
    IUnifiedAuditService auditService,
    ILogger<OperationalReadinessController> logger) : Controller
{
    private readonly IUnifiedWebCapabilityService _capabilityService = capabilityService;
    private readonly IUnifiedAuditService _auditService = auditService;
    private readonly ILogger<OperationalReadinessController> _logger = logger;

    /// <summary>
    /// Displays the operational readiness dashboard aggregated from all domains.
    /// </summary>
    [Route("Unified/OperationalReadiness")]
    public async Task<IActionResult> Index()
    {
        _logger.LogInformation("[OperationalReadiness] Dashboard accessed by {User}", User.Identity?.Name ?? "anonymous");

        _auditService.EmitInfo(
            "SYS.OperationalReadiness.Viewed",
            User.Identity?.Name ?? "anonymous",
            actionDetails: "Operational readiness dashboard viewed");

        var inventoryResult = await _capabilityService.GetCapabilityInventoryAsync();
        var phasesResult = await _capabilityService.GetCompletionPhasesAsync();

        if (!inventoryResult.IsSuccessful)
        {
            _logger.LogWarning("[OperationalReadiness] Failed to load capability inventory: {Errors}",
                string.Join("; ", inventoryResult.Errors));
            return View("Error");
        }

        var capabilities = inventoryResult.Data ?? [];

        var domainSummaries = capabilities
            .GroupBy(c => c.Domain)
            .OrderBy(g => g.Key)
            .Select(g => new DomainReadinessSummary
            {
                Domain = g.Key,
                Total = g.Count(),
                Phase4Count = g.Count(c => c.Phase >= 4),
                Phase3Count = g.Count(c => c.Phase == 3),
                Phase2Count = g.Count(c => c.Phase == 2),
                BelowPhase2Count = g.Count(c => c.Phase < 2),
                IsReadyForCutover = g.All(c => c.Phase >= 3),
                IsCutOver = g.All(c => c.Phase >= 4),
            })
            .ToList();

        var model = new OperationalReadinessDashboardViewModel
        {
            DomainSummaries = domainSummaries,
            TotalCapabilities = capabilities.Count,
            CutOverCount = capabilities.Count(c => c.Phase >= 4),
            ValidatedCount = capabilities.Count(c => c.Phase == 3),
            ReadyForCutoverDomains = domainSummaries.Count(d => d.IsReadyForCutover && !d.IsCutOver),
            AllDomainsCutOver = domainSummaries.All(d => d.IsCutOver),
            LastBuiltAt = DateTimeOffset.UtcNow,
        };

        return View(model);
    }
}

/// <summary>Dashboard view model for operational readiness overview.</summary>
public sealed class OperationalReadinessDashboardViewModel
{
    public List<DomainReadinessSummary> DomainSummaries { get; set; } = [];
    public int TotalCapabilities { get; set; }
    public int CutOverCount { get; set; }
    public int ValidatedCount { get; set; }
    public int ReadyForCutoverDomains { get; set; }
    public bool AllDomainsCutOver { get; set; }
    public DateTimeOffset LastBuiltAt { get; set; }
    public double OverallCompletionPercent =>
        TotalCapabilities == 0 ? 0 : Math.Round((double)CutOverCount / TotalCapabilities * 100, 1);
}

/// <summary>Per-domain readiness summary for the operational readiness dashboard.</summary>
public sealed class DomainReadinessSummary
{
    public string Domain { get; set; } = string.Empty;
    public int Total { get; set; }
    public int Phase4Count { get; set; }
    public int Phase3Count { get; set; }
    public int Phase2Count { get; set; }
    public int BelowPhase2Count { get; set; }
    public bool IsReadyForCutover { get; set; }
    public bool IsCutOver { get; set; }
    public string StatusBadgeClass => IsCutOver ? "bg-success" : IsReadyForCutover ? "bg-warning text-dark" : "bg-secondary";
    public string StatusLabel => IsCutOver ? "Cut Over" : IsReadyForCutover ? "Ready" : "In Progress";
}
