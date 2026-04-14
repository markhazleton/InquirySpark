using InquirySpark.Repository.Configuration.Unified;
using InquirySpark.Repository.Services.UnifiedWeb;
using InquirySpark.Web.Areas.Unified.ViewModels.Completion;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InquirySpark.Web.Areas.Unified.Controllers;

/// <summary>
/// Renders the capability completion matrix — an operator-visible dashboard
/// showing the parity, deployment, and cutover status for all 30+ capabilities
/// across DecisionSpark and InquirySpark.Admin.
/// Capability: all capabilities, no discrete CAP-ID (cross-domain view).
/// </summary>
[Authorize]
[Area("Unified")]
public class CapabilityCompletionMatrixController(
    IUnifiedWebCapabilityService capabilityService,
    ILogger<CapabilityCompletionMatrixController> logger) : Controller
{
    private static readonly IReadOnlyDictionary<string, (string Icon, string Color)> _domainMeta =
        new Dictionary<string, (string, string)>(StringComparer.OrdinalIgnoreCase)
        {
            ["Decision Workspace"] = ("bi-diagram-3", "info"),
            ["Inquiry Administration"] = ("bi-gear", "primary"),
            ["Inquiry Authoring"] = ("bi-pencil-square", "secondary"),
            ["Inquiry Operations"] = ("bi-activity", "success"),
            ["Operations Support"] = ("bi-tools", "warning"),
        };

    /// <summary>Renders the full capability completion matrix.</summary>
    [Route("Unified/CapabilityCompletionMatrix")]
    public async Task<IActionResult> Index(CancellationToken cancellationToken = default)
    {
        var inventoryResult = await capabilityService.GetCapabilityInventoryAsync(cancellationToken);
        var phasesResult = await capabilityService.GetCompletionPhasesAsync(cancellationToken);

        if (!inventoryResult.IsSuccessful)
        {
            logger.LogWarning("[CapabilityMatrix] Failed to load capability inventory: {Errors}",
                string.Join(", ", inventoryResult.Errors));
        }

        var capabilities = inventoryResult.Data ?? [];
        var phases = phasesResult.Data ?? [];

        // Group by domain
        var domainGroups = capabilities
            .GroupBy(c => c.Domain)
            .Select(g =>
            {
                var meta = _domainMeta.TryGetValue(g.Key, out var m) ? m : ("bi-grid", "secondary");
                return new CapabilityDomainGroup
                {
                    Domain = g.Key,
                    Icon = meta.Item1,
                    ColorVariant = meta.Item2,
                    Capabilities = g.Select(c => new CapabilityMatrixRow
                    {
                        CapabilityId = c.CapabilityId,
                        Name = c.Name,
                        Phase = c.Phase,
                        Status = c.Status,
                        UnifiedRoute = !string.IsNullOrEmpty(c.CapabilityId)
                            ? CapabilityRoutingMap.Resolve(c.CapabilityId)
                            : null,
                        Notes = c.Notes,
                    }).OrderBy(r => r.CapabilityId).ToList(),
                };
            })
            .OrderBy(g => g.Domain)
            .ToList();

        var summary = new CapabilityMatrixSummary
        {
            Total = capabilities.Count,
            NotStarted = capabilities.Count(c => c.Phase == 0),
            Deployed = capabilities.Count(c => c.Phase == 2),
            Validated = capabilities.Count(c => c.Phase == 3),
            CutOver = capabilities.Count(c => c.Phase == 4),
        };

        var phaseInfos = phases.Select(p => new PhaseInfo
        {
            PhaseNumber = p.PhaseNumber,
            Name = p.PhaseName,
            Description = p.Description,
        }).OrderBy(p => p.PhaseNumber).ToList();

        var vm = new CapabilityCompletionMatrixViewModel
        {
            DomainGroups = domainGroups,
            Summary = summary,
            Phases = phaseInfos,
        };

        logger.LogInformation("[CapabilityMatrix] Rendering matrix: {Total} capabilities, {Pct}% complete",
            summary.Total, summary.CompletionPercent);

        return View(vm);
    }
}
