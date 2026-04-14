using InquirySpark.Common.Models.Spec;
using InquirySpark.Repository.Services.UnifiedWeb;
using Microsoft.AspNetCore.Authorization;

namespace InquirySpark.Web.Areas.Unified.Controllers;

/// <summary>
/// Unified controller providing DecisionSpark decision-specification management parity (CAP-DS-002 through CAP-DS-006).
/// All file-backed data access delegates to <see cref="IDecisionSparkFileStorageService"/>.
/// </summary>
[Area("Unified")]
[Authorize]
public sealed class DecisionSpecificationController(
    IDecisionSparkFileStorageService fileStorage,
    ILogger<DecisionSpecificationController> logger) : Controller
{
    /// <summary>Lists all decision specifications with optional filtering.</summary>
    public async Task<IActionResult> Index(
        [FromQuery] string? search = null,
        [FromQuery] string? status = null,
        CancellationToken cancellationToken = default)
    {
        var summaries = await fileStorage.ListSpecsAsync(status, search, cancellationToken);
        return View(summaries);
    }

    /// <summary>Shows details for a specific decision specification.</summary>
    [Route("Unified/DecisionSpecification/{specId}")]
    public async Task<IActionResult> Details(string specId, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(specId))
            return BadRequest();

        var result = await fileStorage.GetSpecAsync(specId, cancellationToken: cancellationToken);
        if (result is null)
            return NotFound();

        var (doc, etag) = result.Value;
        ViewData["ETag"] = etag;
        return View(doc);
    }

    /// <summary>Shows the create form for a new decision specification.</summary>
    [HttpGet]
    [Route("Unified/DecisionSpecification/Create")]
    public IActionResult Create() => View(new DecisionSpecDocument());

    /// <summary>Creates a new decision specification.</summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Route("Unified/DecisionSpecification/Create")]
    public async Task<IActionResult> Create(DecisionSpecDocument spec, CancellationToken cancellationToken = default)
    {
        if (!ModelState.IsValid)
            return View(spec);

        var (created, _) = await fileStorage.CreateSpecAsync(spec, cancellationToken);
        logger.LogInformation("[DecisionSpecification] Created spec {SpecId}", created.SpecId);
        return RedirectToAction(nameof(Details), new { specId = created.SpecId });
    }

    /// <summary>Shows the edit form for a decision specification.</summary>
    [HttpGet]
    [Route("Unified/DecisionSpecification/Edit/{specId}")]
    public async Task<IActionResult> Edit(string specId, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(specId))
            return BadRequest();

        var result = await fileStorage.GetSpecAsync(specId, cancellationToken: cancellationToken);
        if (result is null)
            return NotFound();

        var (doc, etag) = result.Value;
        ViewData["ETag"] = etag;
        return View(doc);
    }

    /// <summary>Updates an existing decision specification.</summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Route("Unified/DecisionSpecification/Edit/{specId}")]
    public async Task<IActionResult> Edit(
        string specId,
        DecisionSpecDocument spec,
        [FromForm] string eTag,
        CancellationToken cancellationToken = default)
    {
        if (!ModelState.IsValid)
            return View(spec);

        var result = await fileStorage.UpdateSpecAsync(specId, spec, eTag, cancellationToken);
        if (result is null)
        {
            ModelState.AddModelError(string.Empty, "Concurrent update conflict — please reload and reapply your changes.");
            return View(spec);
        }

        logger.LogInformation("[DecisionSpecification] Updated spec {SpecId}", specId);
        return RedirectToAction(nameof(Details), new { specId });
    }

    /// <summary>Transitions a specification lifecycle status (CAP-DS-005).</summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Route("Unified/DecisionSpecification/{specId}/Status")]
    public async Task<IActionResult> TransitionStatus(
        string specId,
        [FromForm] string version,
        [FromForm] string newStatus,
        [FromForm] string comment,
        CancellationToken cancellationToken = default)
    {
        await fileStorage.TransitionSpecStatusAsync(specId, version, newStatus, comment,
            User.Identity?.Name ?? "system", cancellationToken);
        logger.LogInformation("[DecisionSpecification] Status transitioned {SpecId} -> {NewStatus}", specId, newStatus);
        return RedirectToAction(nameof(Details), new { specId });
    }

    /// <summary>Shows the audit history for a specification (CAP-DS-006).</summary>
    [Route("Unified/DecisionSpecification/{specId}/Audit")]
    public async Task<IActionResult> Audit(string specId, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(specId))
            return BadRequest();

        var history = await fileStorage.GetSpecAuditHistoryAsync(specId, cancellationToken);
        ViewData["SpecId"] = specId;
        return View(history);
    }

    /// <summary>Soft-deletes (archives) a specification.</summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Route("Unified/DecisionSpecification/Delete/{specId}")]
    public async Task<IActionResult> Delete(
        string specId,
        [FromForm] string version,
        [FromForm] string eTag,
        CancellationToken cancellationToken = default)
    {
        var deleted = await fileStorage.DeleteSpecAsync(specId, version, eTag, cancellationToken);
        if (!deleted)
        {
            ModelState.AddModelError(string.Empty, "Spec not found or was already deleted.");
            return RedirectToAction(nameof(Index));
        }

        logger.LogInformation("[DecisionSpecification] Soft-deleted spec {SpecId}", specId);
        return RedirectToAction(nameof(Index));
    }
}
