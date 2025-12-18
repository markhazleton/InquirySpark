using InquirySpark.Admin.Contracts.Requests;
using InquirySpark.Repository.Models.Charting;
using InquirySpark.Repository.Services.Charting;
using InquirySpark.Repository.Services.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InquirySpark.Admin.Controllers.Api;

[ApiController]
[Route("api/[controller]")]
[Authorize(Policy = "Analyst")]
public class ChartDefinitionsController(
    IChartDefinitionService service, 
    IChartValidationService validationService,
    IFormulaParserService formulaParser,
    IAuditLogService auditService, 
    ILogger<ChartDefinitionsController> logger) : ControllerBase
{
    private readonly IChartDefinitionService _service = service;
    private readonly IChartValidationService _validationService = validationService;
    private readonly IFormulaParserService _formulaParser = formulaParser;
    private readonly IAuditLogService _auditService = auditService;
    private readonly ILogger<ChartDefinitionsController> _logger = logger;

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await _service.GetDatasetCatalogAsync();
        
        if (!result.IsSuccessful)
            return BadRequest(result.Errors);
        
        return Ok(result.Data);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(int id)
    {
        var result = await _service.GetDefinitionAsync(id);
        
        if (!result.IsSuccessful)
            return BadRequest(result.Errors);
        
        if (result.Data == null)
            return NotFound();
        
        return Ok(result.Data);
    }

    [HttpGet("{id}/versions")]
    public async Task<IActionResult> GetVersionHistory(int id)
    {
        var result = await _service.GetVersionHistoryAsync(id);
        
        if (!result.IsSuccessful)
            return BadRequest(result.Errors);
        
        return Ok(result.Data);
    }

    [HttpPost("{id}/versions/compare")]
    public async Task<IActionResult> CompareVersions(int id, [FromBody] ChartVersionComparisonRequest request)
    {
        if (id != request.ChartDefinitionId)
            return BadRequest("Chart definition ID mismatch");
        
        if (!ModelState.IsValid)
            return BadRequest(ModelState);
        
        var result = await _service.CompareVersionsAsync(id, request.FromVersion, request.ToVersion);
        
        if (!result.IsSuccessful)
            return BadRequest(result.Errors);
        
        return Ok(result.Data);
    }

    [HttpPost("{id}/rollback")]
    public async Task<IActionResult> Rollback(int id, [FromBody] ChartRollbackRequest request)
    {
        if (id != request.ChartDefinitionId)
            return BadRequest("Chart definition ID mismatch");
        
        if (!ModelState.IsValid)
            return BadRequest(ModelState);
        
        var userId = GetUserId();
        
        var result = await _service.RollbackToVersionAsync(id, request.VersionNumber, userId);
        
        if (!result.IsSuccessful)
            return BadRequest(result.Errors);
        
        await _auditService.LogActionAsync(userId, "ChartDefinition", id.ToString(), "Rollback", $"Rolled back to version {request.VersionNumber}");
        
        return Ok(result.Data);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] ChartDefinitionRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);
        
        var userId = GetUserId();
        
        var dto = new ChartDefinitionDto
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
        
        var result = await _service.SaveDefinitionAsync(dto);
        
        if (!result.IsSuccessful)
            return BadRequest(result.Errors);
        
        await _auditService.LogActionAsync(userId, "ChartDefinition", result.Data.ChartDefinitionId.ToString(), "Create", $"Created chart '{request.Name}'");
        
        return CreatedAtAction(nameof(Get), new { id = result.Data.ChartDefinitionId }, result.Data);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] ChartDefinitionRequest request)
    {
        if (id != request.ChartDefinitionId)
            return BadRequest("ID mismatch");
        
        if (!ModelState.IsValid)
            return BadRequest(ModelState);
        
        var userId = GetUserId();
        
        var dto = new ChartDefinitionDto
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
        
        var result = await _service.SaveDefinitionAsync(dto);
        
        if (!result.IsSuccessful)
            return BadRequest(result.Errors);
        
        await _auditService.LogActionAsync(userId, "ChartDefinition", id.ToString(), "Update", $"Updated chart '{request.Name}'");
        
        return Ok(result.Data);
    }

    [HttpDelete("{id}")]
    [Authorize(Policy = "Operator")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _service.DeleteAsync(id);
        
        if (!result.IsSuccessful)
            return BadRequest(result.Errors);
        
        if (!result.Data)
            return NotFound();
        
        var userId = GetUserId();
        await _auditService.LogActionAsync(userId, "ChartDefinition", id.ToString(), "Delete", "Deleted chart definition");
        
        return NoContent();
    }
    
    /// <summary>
    /// Validate a chart definition
    /// </summary>
    [HttpPost("{id}/validate")]
    public async Task<IActionResult> Validate(int id)
    {
        var result = await _validationService.ValidateDefinitionAsync(id);
        
        if (!result.IsSuccessful)
            return BadRequest(result.Errors);
        
        return Ok(result.Data);
    }
    
    /// <summary>
    /// Auto-approve a chart definition if it passes validation
    /// </summary>
    [HttpPost("{id}/auto-approve")]
    [Authorize(Policy = "Operator")]
    public async Task<IActionResult> AutoApprove(int id)
    {
        var userId = GetUserId();
        var result = await _validationService.AutoApproveIfValidAsync(id, userId);
        
        if (!result.IsSuccessful)
            return BadRequest(result.Errors);
        
        if (!result.Data)
            return BadRequest(new { message = "Chart failed validation and cannot be auto-approved" });
        
        return Ok(new { message = "Chart auto-approved successfully", approved = true });
    }
    
    /// <summary>
    /// Validate a formula without saving
    /// </summary>
    [HttpPost("validate-formula")]
    public IActionResult ValidateFormula([FromBody] ValidateFormulaRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);
        
        var result = _formulaParser.ValidateFormula(
            request.Formula, 
            request.ChartType, 
            request.AvailableColumns ?? new List<string>());
        
        if (!result.IsSuccessful)
            return BadRequest(result.Errors);
        
        return Ok(result.Data);
    }
    
    /// <summary>
    /// Get supported functions for formula editor
    /// </summary>
    [HttpGet("formula-functions")]
    public IActionResult GetFormulaFunctions([FromQuery] string chartType = null)
    {
        var functions = string.IsNullOrEmpty(chartType)
            ? _formulaParser.GetSupportedFunctions()
            : _formulaParser.GetFunctionsForChartType(chartType);
        
        return Ok(new { functions });
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

public class ValidateFormulaRequest
{
    public string Formula { get; set; }
    public string ChartType { get; set; }
    public List<string> AvailableColumns { get; set; }
}
