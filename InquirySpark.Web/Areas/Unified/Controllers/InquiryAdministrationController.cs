using InquirySpark.Repository.Database;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace InquirySpark.Web.Areas.Unified.Controllers;

/// <summary>
/// Unified controller providing InquirySpark.Admin administration capability-family parity.
/// Covers: applications, users, roles, site roles, and site menus (CAP-IA-001 through CAP-IA-005, CAP-IA-024).
/// Data source: InquirySparkContext (SQLite read-only).
/// </summary>
[Area("Unified")]
[Authorize]
public sealed class InquiryAdministrationController(
    InquirySparkContext context) : Controller
{
    // ── Applications (CAP-IA-001) ─────────────────────────────────────────

    [Route("Unified/InquiryAdministration/Applications")]
    public async Task<IActionResult> Applications(CancellationToken cancellationToken = default)
    {
        var items = await context.Applications
            .Include(a => a.ApplicationType)
            .Include(a => a.Company)
            .ToListAsync(cancellationToken);
        return View(nameof(Applications), items);
    }

    [Route("Unified/InquiryAdministration/Applications/{id:int}")]
    public async Task<IActionResult> ApplicationDetails(int id, CancellationToken cancellationToken = default)
    {
        var item = await context.Applications
            .Include(a => a.ApplicationType)
            .Include(a => a.Company)
            .FirstOrDefaultAsync(a => a.ApplicationId == id, cancellationToken);
        return item is null ? NotFound() : View("ApplicationDetails", item);
    }

    // ── Application Users (CAP-IA-002) ────────────────────────────────────

    [Route("Unified/InquiryAdministration/ApplicationUsers")]
    public async Task<IActionResult> ApplicationUsers(CancellationToken cancellationToken = default)
    {
        var items = await context.ApplicationUsers
            .ToListAsync(cancellationToken);
        return View(nameof(ApplicationUsers), items);
    }

    // ── Application User Roles (CAP-IA-003) ──────────────────────────────

    [Route("Unified/InquiryAdministration/ApplicationUserRoles")]
    public async Task<IActionResult> ApplicationUserRoles(CancellationToken cancellationToken = default)
    {
        var items = await context.ApplicationUserRoles
            .Include(r => r.ApplicationUser)
            .ToListAsync(cancellationToken);
        return View(nameof(ApplicationUserRoles), items);
    }

    // ── Application Surveys (CAP-IA-004) ──────────────────────────────────

    [Route("Unified/InquiryAdministration/ApplicationSurveys")]
    public async Task<IActionResult> ApplicationSurveys(CancellationToken cancellationToken = default)
    {
        var items = await context.ApplicationSurveys
            .Include(s => s.Application)
            .Include(s => s.Survey)
            .ToListAsync(cancellationToken);
        return View(nameof(ApplicationSurveys), items);
    }

    // ── App Properties (CAP-IA-005) ───────────────────────────────────────

    [Route("Unified/InquiryAdministration/AppProperties")]
    public async Task<IActionResult> AppProperties(CancellationToken cancellationToken = default)
    {
        var items = await context.AppProperties.ToListAsync(cancellationToken);
        return View(nameof(AppProperties), items);
    }

    // ── Roles (CAP-IA-024) ────────────────────────────────────────────────

    [Route("Unified/InquiryAdministration/Roles")]
    public async Task<IActionResult> Roles(CancellationToken cancellationToken = default)
    {
        var items = await context.Roles.ToListAsync(cancellationToken);
        return View(nameof(Roles), items);
    }

    // ── Lookup: Application Types (CAP-IA-018) ────────────────────────────

    [Route("Unified/InquiryAdministration/LuApplicationTypes")]
    public async Task<IActionResult> LuApplicationTypes(CancellationToken cancellationToken = default)
    {
        var items = await context.LuApplicationTypes.ToListAsync(cancellationToken);
        return View(nameof(LuApplicationTypes), items);
    }

    // ── Lookup: Question Types (CAP-IA-019) ───────────────────────────────

    [Route("Unified/InquiryAdministration/LuQuestionTypes")]
    public async Task<IActionResult> LuQuestionTypes(CancellationToken cancellationToken = default)
    {
        var items = await context.LuQuestionTypes.ToListAsync(cancellationToken);
        return View(nameof(LuQuestionTypes), items);
    }

    // ── Lookup: Review Status (CAP-IA-020) ───────────────────────────────

    [Route("Unified/InquiryAdministration/LuReviewStatus")]
    public async Task<IActionResult> LuReviewStatus(CancellationToken cancellationToken = default)
    {
        var items = await context.LuReviewStatuses.ToListAsync(cancellationToken);
        return View(nameof(LuReviewStatus), items);
    }

    // ── Lookup: Survey Response Status (CAP-IA-021) ───────────────────────

    [Route("Unified/InquiryAdministration/LuSurveyResponseStatus")]
    public async Task<IActionResult> LuSurveyResponseStatus(CancellationToken cancellationToken = default)
    {
        var items = await context.LuSurveyResponseStatuses.ToListAsync(cancellationToken);
        return View(nameof(LuSurveyResponseStatus), items);
    }

    // ── Lookup: Survey Types (CAP-IA-022) ─────────────────────────────────

    [Route("Unified/InquiryAdministration/LuSurveyTypes")]
    public async Task<IActionResult> LuSurveyTypes(CancellationToken cancellationToken = default)
    {
        var items = await context.LuSurveyTypes.ToListAsync(cancellationToken);
        return View(nameof(LuSurveyTypes), items);
    }

    // ── Lookup: Units of Measure (CAP-IA-023) ─────────────────────────────

    [Route("Unified/InquiryAdministration/LuUnitOfMeasures")]
    public async Task<IActionResult> LuUnitOfMeasures(CancellationToken cancellationToken = default)
    {
        var items = await context.LuUnitOfMeasures.ToListAsync(cancellationToken);
        return View(nameof(LuUnitOfMeasures), items);
    }
}
