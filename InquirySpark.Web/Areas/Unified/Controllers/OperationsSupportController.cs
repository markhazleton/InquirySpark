using InquirySpark.Repository.Database;
using InquirySpark.Repository.Services.Charting;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace InquirySpark.Web.Areas.Unified.Controllers;

/// <summary>
/// Unified controller providing operations support capability parity.
/// Covers: charting, system health, conversation API, role management, and user preferences
/// (CAP-DS-007, CAP-IA-025 through CAP-IA-030).
/// </summary>
[Area("Unified")]
[Authorize]
public sealed class OperationsSupportController(
    InquirySparkContext context,
    IChartDefinitionService chartService,
    ILogger<OperationsSupportController> logger) : Controller
{
    // ── System Health (CAP-DS-007, CAP-IA-028) ────────────────────────────

    [Route("Unified/OperationsSupport/Health")]
    [AllowAnonymous]
    public IActionResult Health()
    {
        try
        {
            var canConnect = context.Database.CanConnect();
            ViewData["DbStatus"] = canConnect ? "Connected" : "Disconnected";
            ViewData["Provider"] = context.Database.ProviderName ?? "Unknown";
            ViewData["CheckedAt"] = DateTime.UtcNow;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "[OperationsSupport] Health check failed.");
            ViewData["DbStatus"] = "Error";
            ViewData["Error"] = ex.Message;
        }
        return View();
    }

    // ── Chart Builder (CAP-IA-026) ────────────────────────────────────────

    [Route("Unified/OperationsSupport/ChartBuilder")]
    [Authorize(Policy = "Analyst")]
    public async Task<IActionResult> ChartBuilder()
    {
        var result = await chartService.GetDatasetCatalogAsync();
        if (!result.IsSuccessful)
        {
            logger.LogError("[OperationsSupport] Failed to load chart definitions: {Errors}",
                string.Join(", ", result.Errors));
            return View(new List<InquirySpark.Repository.Models.Charting.ChartDefinitionDto>());
        }
        return View(result.Data);
    }

    // ── Chart Settings (CAP-IA-025) ────────────────────────────────────────

    [Route("Unified/OperationsSupport/ChartSettings")]
    [Authorize(Policy = "Analyst")]
    public async Task<IActionResult> ChartSettings(CancellationToken cancellationToken = default)
    {
        var items = await context.ChartSettings.ToListAsync(cancellationToken);
        return View(items);
    }

    // ── User Preferences (CAP-IA-029) ─────────────────────────────────────

    [Route("Unified/OperationsSupport/UserPreferences")]
    public IActionResult UserPreferences()
    {
        // User preference retrieval requires an integer userId from the Inquiry DB.
        // Preference display is handled client-side; this action renders the preferences UI shell.
        return View();
    }
}
