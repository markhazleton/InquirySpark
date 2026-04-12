using InquirySpark.Repository.Services.UnifiedWeb;
using InquirySpark.Web.Areas.Unified.ViewModels;
using Microsoft.AspNetCore.Authorization;

namespace InquirySpark.Web.Areas.Unified.Controllers;

/// <summary>
/// Unified operations home controller.
/// Serves the landing page for the single unified workspace experience (US1).
/// </summary>
[Area("Unified")]
[Authorize]
public sealed class OperationsController(
    IUnifiedWebCapabilityService capabilityService,
    ILogger<OperationsController> logger) : Controller
{
    /// <summary>Renders the unified operations home dashboard.</summary>
    public async Task<IActionResult> Index()
    {
        var inventoryResponse = await capabilityService.GetCapabilityInventoryAsync();

        var capabilities = inventoryResponse.IsSuccessful && inventoryResponse.Data is not null
            ? inventoryResponse.Data
            : [];

        var domains = capabilities
            .Select(c => c.Domain)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .Count();

        var model = new OperationsHomeViewModel
        {
            UserDisplayName = User.Identity?.Name ?? "Operator",
            TotalCapabilities = capabilities.Count,
            CapabilitiesNotStarted = capabilities.Count(c => string.Equals(c.Status, "not-started", StringComparison.OrdinalIgnoreCase)),
            CapabilitiesInProgress = capabilities.Count(c => string.Equals(c.Status, "in-progress", StringComparison.OrdinalIgnoreCase)),
            CapabilitiesValidated = capabilities.Count(c =>
                string.Equals(c.Status, "validated", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(c.Status, "cut-over", StringComparison.OrdinalIgnoreCase)),
            DomainCount = domains,
        };

        logger.LogInformation("[Operations] Dashboard loaded. Capabilities={Count} Domains={Domains}",
            model.TotalCapabilities, model.DomainCount);

        return View(model);
    }

    /// <summary>Displays the application error page (used as the global exception handler endpoint).</summary>
    [AllowAnonymous]
    [Route("Unified/Operations/Error")]
    public IActionResult Error() => View("~/Views/Shared/Error.cshtml");
}
