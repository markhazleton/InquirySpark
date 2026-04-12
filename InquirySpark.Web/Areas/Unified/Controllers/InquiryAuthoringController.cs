using InquirySpark.Repository.Database;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace InquirySpark.Web.Areas.Unified.Controllers;

/// <summary>
/// Unified controller providing InquirySpark.Admin authoring capability-family parity.
/// Covers: surveys, questions, question groups, group members, answers, and email templates
/// (CAP-IA-006 through CAP-IA-011).
/// Data source: InquirySparkContext (SQLite read-only).
/// </summary>
[Area("Unified")]
[Authorize]
public sealed class InquiryAuthoringController(
    InquirySparkContext context) : Controller
{
    // ── Surveys (CAP-IA-006) ──────────────────────────────────────────────

    [Route("Unified/InquiryAuthoring/Surveys")]
    public async Task<IActionResult> Surveys(CancellationToken cancellationToken = default)
    {
        var items = await context.Surveys
            .Include(s => s.SurveyType)
            .ToListAsync(cancellationToken);
        return View(nameof(Surveys), items);
    }

    [Route("Unified/InquiryAuthoring/Surveys/{id:int}")]
    public async Task<IActionResult> SurveyDetails(int id, CancellationToken cancellationToken = default)
    {
        var item = await context.Surveys
            .Include(s => s.SurveyType)
            .Include(s => s.QuestionGroups)
            .FirstOrDefaultAsync(s => s.SurveyId == id, cancellationToken);
        return item is null ? NotFound() : View("SurveyDetails", item);
    }

    // ── Survey Email Templates (CAP-IA-007) ───────────────────────────────

    [Route("Unified/InquiryAuthoring/SurveyEmailTemplates")]
    public async Task<IActionResult> SurveyEmailTemplates(CancellationToken cancellationToken = default)
    {
        var items = await context.SurveyEmailTemplates
            .Include(t => t.Survey)
            .ToListAsync(cancellationToken);
        return View(nameof(SurveyEmailTemplates), items);
    }

    // ── Questions (CAP-IA-008) ────────────────────────────────────────────

    [Route("Unified/InquiryAuthoring/Questions")]
    public async Task<IActionResult> Questions(CancellationToken cancellationToken = default)
    {
        var items = await context.Questions
            .Include(q => q.QuestionType)
            .ToListAsync(cancellationToken);
        return View(nameof(Questions), items);
    }

    [Route("Unified/InquiryAuthoring/Questions/{id:int}")]
    public async Task<IActionResult> QuestionDetails(int id, CancellationToken cancellationToken = default)
    {
        var item = await context.Questions
            .Include(q => q.QuestionType)
            .Include(q => q.QuestionAnswers)
            .FirstOrDefaultAsync(q => q.QuestionId == id, cancellationToken);
        return item is null ? NotFound() : View("QuestionDetails", item);
    }

    // ── Question Groups (CAP-IA-009) ──────────────────────────────────────

    [Route("Unified/InquiryAuthoring/QuestionGroups")]
    public async Task<IActionResult> QuestionGroups(CancellationToken cancellationToken = default)
    {
        var items = await context.QuestionGroups
            .Include(g => g.Survey)
            .ToListAsync(cancellationToken);
        return View(nameof(QuestionGroups), items);
    }

    // ── Question Group Members (CAP-IA-010) ───────────────────────────────

    [Route("Unified/InquiryAuthoring/QuestionGroupMembers")]
    public async Task<IActionResult> QuestionGroupMembers(CancellationToken cancellationToken = default)
    {
        var items = await context.QuestionGroupMembers
            .Include(m => m.QuestionGroup)
            .Include(m => m.Question)
            .ToListAsync(cancellationToken);
        return View(nameof(QuestionGroupMembers), items);
    }

    // ── Question Answers (CAP-IA-011) ─────────────────────────────────────

    [Route("Unified/InquiryAuthoring/QuestionAnswers")]
    public async Task<IActionResult> QuestionAnswers(CancellationToken cancellationToken = default)
    {
        var items = await context.QuestionAnswers
            .Include(a => a.Question)
            .ToListAsync(cancellationToken);
        return View(nameof(QuestionAnswers), items);
    }
}
