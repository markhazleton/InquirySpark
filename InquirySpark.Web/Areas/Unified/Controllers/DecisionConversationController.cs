using InquirySpark.Repository.Services.UnifiedWeb;
using Microsoft.AspNetCore.Authorization;

namespace InquirySpark.Web.Areas.Unified.Controllers;

/// <summary>
/// Unified controller providing DecisionSpark conversation workflow parity in InquirySpark.Web.
/// Delegates file-backed data access to <see cref="IDecisionSparkFileStorageService"/> (CAP-DS-001, CAP-DS-003).
/// </summary>
[Area("Unified")]
[Authorize]
public sealed class DecisionConversationController(
    IDecisionSparkFileStorageService fileStorage,
    ILogger<DecisionConversationController> logger) : Controller
{
    /// <summary>Lists available decision conversations.</summary>
    public async Task<IActionResult> Index(CancellationToken cancellationToken = default)
    {
        try
        {
            var specs = await fileStorage.ListSpecsAsync(cancellationToken: cancellationToken);
            return View(specs);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "[DecisionConversation] Error loading conversation list.");
            return View(Enumerable.Empty<InquirySpark.Common.Models.Spec.DecisionSpec>());
        }
    }

    /// <summary>Starts or resumes a conversation for the given spec ID.</summary>
    [Route("Unified/DecisionConversation/Start/{specId}")]
    public async Task<IActionResult> Start(string specId, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(specId))
            return BadRequest();

        var result = await fileStorage.GetSpecAsync(specId, cancellationToken: cancellationToken);
        if (result is null)
            return NotFound();

        var (doc, _) = result.Value;
        return View(doc);
    }

    /// <summary>Lists persisted conversation sessions for the current user.</summary>
    [Route("Unified/DecisionConversation/Sessions")]
    public IActionResult Sessions()
    {
        // Session listing is managed client-side via session state.
        // Full persistence listing requires IConversationPersistence extension (future work).
        return View();
    }

    /// <summary>Returns a JSON list of available specs for the conversation API parity (CAP-IA-030).</summary>
    [Route("Unified/DecisionConversation/Api/Specs")]
    public async Task<IActionResult> ApiSpecs(CancellationToken cancellationToken = default)
    {
        var specs = await fileStorage.ListSpecsAsync(cancellationToken: cancellationToken);
        return Json(specs);
    }
}
