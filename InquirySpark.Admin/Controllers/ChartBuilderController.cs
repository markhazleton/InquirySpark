using InquirySpark.Admin.Contracts.Requests;
using InquirySpark.Repository.Services.Charting;
using InquirySpark.Repository.Services.UserPreferences;
using InquirySpark.Repository.Services.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InquirySpark.Admin.Controllers;

[Authorize(Policy = "Analyst")]
public class ChartBuilderController(
    IChartDefinitionService chartDefinitionService,
    IUserPreferenceService userPreferenceService,
    IAuditLogService auditLogService,
    ILogger<ChartBuilderController> logger) : BaseController(logger)
{
    private readonly IChartDefinitionService _chartDefinitionService = chartDefinitionService;
    private readonly IUserPreferenceService _userPreferenceService = userPreferenceService;
    private readonly IAuditLogService _auditLogService = auditLogService;

    /// <summary>
    /// Display library of all chart definitions
    /// </summary>
    public async Task<IActionResult> Index()
    {
        var result = await _chartDefinitionService.GetDatasetCatalogAsync();
        
        if (!result.IsSuccessful)
        {
            _logger.LogError("Failed to load chart definitions: {Errors}", string.Join(", ", result.Errors));
            TempData["ErrorMessage"] = "Failed to load chart definitions.";
            return View(new List<Repository.Models.Charting.ChartDefinitionDto>());
        }

        return View(result.Data);
    }

    /// <summary>
    /// Display form to create new chart definition
    /// </summary>
    public async Task<IActionResult> Create()
    {
        var userId = GetUserId();
        
        // Load user's draft preferences if available
        var draftJson = await _userPreferenceService.GetPreferenceAsync(userId, "chartbuilder.draft");
        if (!string.IsNullOrEmpty(draftJson))
        {
            ViewBag.DraftPreferences = draftJson;
        }
        
        // Load user's layout preferences
        var layoutJson = await _userPreferenceService.GetPreferenceAsync(userId, "chartbuilder.layout");
        if (!string.IsNullOrEmpty(layoutJson))
        {
            ViewBag.LayoutPreferences = layoutJson;
        }
        
        return View(new ChartDefinitionRequest());
    }

    /// <summary>
    /// Process create chart definition form submission
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ChartDefinitionRequest request)
    {
        if (!ModelState.IsValid)
            return View(request);

        var userId = GetUserId();

        var dto = new Repository.Models.Charting.ChartDefinitionDto
        {
            DatasetId = request.DatasetId,
            Name = request.Name,
            Description = request.Description,
            Tags = request.Tags,
            FilterPayload = request.FilterPayload,
            VisualPayload = request.VisualPayload,
            CalculationPayload = request.CalculationPayload,
            AutoApprovedFl = request.AutoApprovedFl,
            CreatedById = userId,
            ModifiedById = userId
        };

        var result = await _chartDefinitionService.SaveDefinitionAsync(dto);

        if (!result.IsSuccessful)
        {
            _logger.LogError("Failed to create chart definition: {Errors}", string.Join(", ", result.Errors));
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error);
            }
            return View(request);
        }

        await _auditLogService.LogActionAsync(userId, "ChartDefinition", result.Data.ChartDefinitionId.ToString(), "Create", $"Created chart '{request.Name}'");
        
        // Clear draft preferences after successful save
        await _userPreferenceService.DeletePreferenceAsync(userId, "chartbuilder.draft");

        TempData["SuccessMessage"] = $"Chart definition '{request.Name}' created successfully.";
        return RedirectToAction(nameof(Details), new { id = result.Data.ChartDefinitionId });
    }

    /// <summary>
    /// Display details of specific chart definition
    /// </summary>
    public async Task<IActionResult> Details(int id)
    {
        var result = await _chartDefinitionService.GetDefinitionAsync(id);

        if (!result.IsSuccessful)
        {
            _logger.LogError("Failed to load chart definition {Id}: {Errors}", id, string.Join(", ", result.Errors));
            return NotFound();
        }

        if (result.Data == null)
            return NotFound();

        return View(result.Data);
    }

    /// <summary>
    /// Display form to edit existing chart definition
    /// </summary>
    public async Task<IActionResult> Edit(int id)
    {
        var result = await _chartDefinitionService.GetDefinitionAsync(id);

        if (!result.IsSuccessful || result.Data == null)
        {
            _logger.LogError("Failed to load chart definition {Id} for editing", id);
            return NotFound();
        }

        var request = new ChartDefinitionRequest
        {
            ChartDefinitionId = result.Data.ChartDefinitionId,
            DatasetId = result.Data.DatasetId,
            Name = result.Data.Name,
            Description = result.Data.Description,
            Tags = result.Data.Tags,
            FilterPayload = result.Data.FilterPayload,
            VisualPayload = result.Data.VisualPayload,
            CalculationPayload = result.Data.CalculationPayload,
            AutoApprovedFl = result.Data.AutoApprovedFl
        };
        
        var userId = GetUserId();
        
        // Load user's layout preferences
        var layoutJson = await _userPreferenceService.GetPreferenceAsync(userId, "chartbuilder.layout");
        if (!string.IsNullOrEmpty(layoutJson))
        {
            ViewBag.LayoutPreferences = layoutJson;
        }

        return View(request);
    }

    /// <summary>
    /// Process edit chart definition form submission
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, ChartDefinitionRequest request)
    {
        if (id != request.ChartDefinitionId)
            return BadRequest();

        if (!ModelState.IsValid)
            return View(request);

        var userId = GetUserId();

        var dto = new Repository.Models.Charting.ChartDefinitionDto
        {
            ChartDefinitionId = id,
            DatasetId = request.DatasetId,
            Name = request.Name,
            Description = request.Description,
            Tags = request.Tags,
            FilterPayload = request.FilterPayload,
            VisualPayload = request.VisualPayload,
            CalculationPayload = request.CalculationPayload,
            AutoApprovedFl = request.AutoApprovedFl,
            ModifiedById = userId
        };

        var result = await _chartDefinitionService.SaveDefinitionAsync(dto);

        if (!result.IsSuccessful)
        {
            _logger.LogError("Failed to update chart definition {Id}: {Errors}", id, string.Join(", ", result.Errors));
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error);
            }
            return View(request);
        }

        await _auditLogService.LogActionAsync(userId, "ChartDefinition", id.ToString(), "Update", $"Updated chart '{request.Name}'");

        TempData["SuccessMessage"] = $"Chart definition '{request.Name}' updated successfully.";
        return RedirectToAction(nameof(Details), new { id });
    }

    /// <summary>
    /// Display version history for chart definition
    /// </summary>
    public async Task<IActionResult> History(int id)
    {
        var definitionResult = await _chartDefinitionService.GetDefinitionAsync(id);
        if (!definitionResult.IsSuccessful || definitionResult.Data == null)
            return NotFound();

        var versionsResult = await _chartDefinitionService.GetVersionHistoryAsync(id);
        if (!versionsResult.IsSuccessful)
        {
            _logger.LogError("Failed to load version history for chart {Id}: {Errors}", id, string.Join(", ", versionsResult.Errors));
            TempData["ErrorMessage"] = "Failed to load version history.";
        }

        ViewBag.ChartDefinition = definitionResult.Data;
        return View(versionsResult.Data ?? new List<Repository.Services.Charting.ChartVersionDto>());
    }

    /// <summary>
    /// Rollback chart definition to specific version
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Rollback(int id, int versionNumber)
    {
        var userId = GetUserId();
        
        var result = await _chartDefinitionService.RollbackToVersionAsync(id, versionNumber, userId);

        if (!result.IsSuccessful)
        {
            _logger.LogError("Failed to rollback chart {Id} to version {Version}: {Errors}", 
                id, versionNumber, string.Join(", ", result.Errors));
            TempData["ErrorMessage"] = $"Failed to rollback to version {versionNumber}.";
        }
        else
        {
            await _auditLogService.LogActionAsync(userId, "ChartDefinition", id.ToString(), "Rollback", $"Rolled back to version {versionNumber}");
            TempData["SuccessMessage"] = $"Successfully rolled back to version {versionNumber}.";
        }

        return RedirectToAction(nameof(History), new { id });
    }

    /// <summary>
    /// Display delete confirmation page
    /// </summary>
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _chartDefinitionService.GetDefinitionAsync(id);

        if (!result.IsSuccessful || result.Data == null)
            return NotFound();

        return View(result.Data);
    }

    /// <summary>
    /// Process delete confirmation (soft delete)
    /// </summary>
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    [Authorize(Policy = "Operator")]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var result = await _chartDefinitionService.DeleteAsync(id);

        if (!result.IsSuccessful || !result.Data)
        {
            _logger.LogError("Failed to delete chart definition {Id}", id);
            TempData["ErrorMessage"] = "Failed to delete chart definition.";
            return RedirectToAction(nameof(Delete), new { id });
        }

        var userId = GetUserId();
        await _auditLogService.LogActionAsync(userId, "ChartDefinition", id.ToString(), "Delete", "Deleted chart definition");

        TempData["SuccessMessage"] = "Chart definition deleted successfully.";
        return RedirectToAction(nameof(Index));
    }
    
    /// <summary>
    /// API endpoint to save user preferences from JavaScript
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> SavePreferences([FromBody] SavePreferenceRequest request)
    {
        var userId = GetUserId();
        await _userPreferenceService.SavePreferenceAsync(userId, request.Key, request.Value);
        return Json(new { success = true });
    }
    
    private int GetUserId()
    {
        // Get user ID from claims, default to 1 for anonymous/demo
        if (User?.Identity?.IsAuthenticated == true)
        {
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (int.TryParse(userIdClaim, out var userId))
                return userId;
        }
        return 1; // Default for demo/anonymous users
    }
}

public class SavePreferenceRequest
{
    public string Key { get; set; }
    public string Value { get; set; }
}
