using InquirySpark.Repository.Database;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace InquirySpark.Web.Areas.Unified.Controllers;

/// <summary>
/// Unified controller providing InquirySpark.Admin operations capability-family parity.
/// Covers: companies, import history, survey status, review status, site roles, and site menus
/// (CAP-IA-012 through CAP-IA-017).
/// Data source: InquirySparkContext (SQLite read-only).
/// </summary>
[Area("Unified")]
[Authorize]
public sealed class InquiryOperationsController(
    InquirySparkContext context) : Controller
{
    // ── Companies (CAP-IA-012) ────────────────────────────────────────────

    /// <summary>Lists all companies from the read-only inquiry data store.</summary>
    [Route("Unified/InquiryOperations/Companies")]
    public async Task<IActionResult> Companies(CancellationToken cancellationToken = default)
    {
        var items = await context.Companies.ToListAsync(cancellationToken);
        return View(nameof(Companies), items);
    }

    /// <summary>Shows details for the company with the specified identifier.</summary>
    [Route("Unified/InquiryOperations/Companies/{id:int}")]
    public async Task<IActionResult> CompanyDetails(int id, CancellationToken cancellationToken = default)
    {
        var item = await context.Companies.FindAsync(id);
        return item is null ? NotFound() : View("CompanyDetails", item);
    }

    // ── Import Histories (CAP-IA-013) ─────────────────────────────────────

    /// <summary>Lists all data import history records.</summary>
    [Route("Unified/InquiryOperations/ImportHistories")]
    public async Task<IActionResult> ImportHistories(CancellationToken cancellationToken = default)
    {
        var items = await context.ImportHistories.ToListAsync(cancellationToken);
        return View(nameof(ImportHistories), items);
    }

    // ── Survey Status (CAP-IA-014) ────────────────────────────────────────
    /// <summary>Displays survey status overview across all surveys.</summary>    [Route("Unified/InquiryOperations/SurveyStatus")]
    public async Task<IActionResult> SurveyStatus(CancellationToken cancellationToken = default)
    {
        var items = await context.Surveys
            .Include(s => s.SurveyType)
            .ToListAsync(cancellationToken);
        return View(nameof(SurveyStatus), items);
    }

    // ── Survey Review Status (CAP-IA-015) ─────────────────────────────────

    /// <summary>Displays survey review status for all surveys pending review.</summary>
    [Route("Unified/InquiryOperations/SurveyReviewStatus")]
    public async Task<IActionResult> SurveyReviewStatus(CancellationToken cancellationToken = default)
    {
        var items = await context.SurveyReviewStatuses
            .ToListAsync(cancellationToken);
        return View(nameof(SurveyReviewStatus), items);
    }

    // ── Site Roles (CAP-IA-016) ───────────────────────────────────────────

    /// <summary>Lists all site role definitions.</summary>
    [Route("Unified/InquiryOperations/SiteRoles")]
    public async Task<IActionResult> SiteRoles(CancellationToken cancellationToken = default)
    {
        var items = await context.SiteRoles.ToListAsync(cancellationToken);
        return View(nameof(SiteRoles), items);
    }

    // ── Site App Menus (CAP-IA-017) ───────────────────────────────────────

    /// <summary>Lists all site application menu entries.</summary>
    [Route("Unified/InquiryOperations/SiteAppMenus")]
    public async Task<IActionResult> SiteAppMenus(CancellationToken cancellationToken = default)
    {
        var items = await context.SiteAppMenus.ToListAsync(cancellationToken);
        return View(nameof(SiteAppMenus), items);
    }
}
