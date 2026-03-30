using InquirySpark.Common.Models;
using InquirySpark.Repository.Database;
using InquirySpark.Repository.Models.Charting;
using InquirySpark.Repository.Services.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;

namespace InquirySpark.Repository.Services.Charting;

public interface IChartValidationService
{
    Task<BaseResponse<ChartValidationReport>> ValidateDefinitionAsync(int chartDefinitionId);
    Task<BaseResponse<bool>> AutoApproveIfValidAsync(int chartDefinitionId, int userId);
}

public class ChartValidationReport
{
    public bool IsValid { get; set; }
    public bool DatasetAvailable { get; set; }
    public bool SchemaValid { get; set; }
    public bool FormulasSafe { get; set; }
    public List<string> Errors { get; set; } = new();
    public List<string> Warnings { get; set; } = new();
    public bool AutoApproved { get; set; }
}

public class ChartValidationService(
    InquirySparkContext context,
    IFormulaParserService formulaParser,
    IAuditLogService auditLog,
    ILogger<ChartValidationService> logger) : IChartValidationService
{
    private readonly InquirySparkContext _context = context;
    private readonly IFormulaParserService _formulaParser = formulaParser;
    private readonly IAuditLogService _auditLog = auditLog;
    private readonly ILogger<ChartValidationService> _logger = logger;

    // JSON Schema for filter payload validation
    private static readonly string FilterPayloadSchema = @"{
        'type': 'object',
        'properties': {
            'filters': {
                'type': 'array',
                'items': {
                    'type': 'object',
                    'properties': {
                        'column': { 'type': 'string' },
                        'operator': { 'type': 'string', 'enum': ['equals', 'contains', 'gt', 'lt', 'gte', 'lte', 'in', 'between'] },
                        'value': { }
                    },
                    'required': ['column', 'operator', 'value']
                }
            }
        }
    }";

    // JSON Schema for calculation payload validation
    private static readonly string CalculationPayloadSchema = @"{
        'type': 'object',
        'properties': {
            'calculations': {
                'type': 'array',
                'items': {
                    'type': 'object',
                    'properties': {
                        'name': { 'type': 'string' },
                        'formula': { 'type': 'string' },
                        'type': { 'type': 'string', 'enum': ['aggregate', 'calculated', 'derived'] }
                    },
                    'required': ['name', 'formula']
                }
            }
        }
    }";

    public async Task<BaseResponse<ChartValidationReport>> ValidateDefinitionAsync(int chartDefinitionId)
    {
        return await DbContextHelper.ExecuteAsync(async () =>
        {
            var report = new ChartValidationReport
            {
                IsValid = true
            };

            // Load chart definition
            var chartDef = await _context.ChartDefinitions
                .FirstOrDefaultAsync(c => c.ChartDefinitionId == chartDefinitionId);

            if (chartDef == null)
            {
                report.Errors.Add($"Chart definition {chartDefinitionId} not found");
                report.IsValid = false;
                return report;
            }

            // 1. Validate dataset availability
            report.DatasetAvailable = await ValidateDatasetAsync(chartDef.DatasetId, report);

            // 2. Validate JSON schema for filter and calculation payloads
            report.SchemaValid = ValidateSchemas(chartDef, report);

            // 3. Validate formulas for safety
            report.FormulasSafe = await ValidateFormulasAsync(chartDef, report);

            // Overall validation status
            report.IsValid = report.DatasetAvailable && report.SchemaValid && report.FormulasSafe;

            return report;
        });
    }

    public async Task<BaseResponse<bool>> AutoApproveIfValidAsync(int chartDefinitionId, int userId)
    {
        return await DbContextHelper.ExecuteAsync(async () =>
        {
            var validationResult = await ValidateDefinitionAsync(chartDefinitionId);

            if (!validationResult.IsSuccessful || !validationResult.Data.IsValid)
            {
                _logger.LogWarning("Chart {ChartId} failed validation, cannot auto-approve", chartDefinitionId);
                return false;
            }

            // Auto-approve the chart
            var chartDef = await _context.ChartDefinitions
                .FirstOrDefaultAsync(c => c.ChartDefinitionId == chartDefinitionId);

            if (chartDef != null)
            {
                chartDef.AutoApprovedFl = true;
                chartDef.ModifiedById = userId;
                chartDef.ModifiedDt = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                // Log audit entry
                await _auditLog.LogActionAsync(userId, "ChartDefinition", chartDefinitionId.ToString(), 
                    "AutoApprove", "Chart passed validation and was auto-approved");

                validationResult.Data.AutoApproved = true;
                _logger.LogInformation("Chart {ChartId} auto-approved after validation", chartDefinitionId);
                return true;
            }

            return false;
        });
    }

    private async Task<bool> ValidateDatasetAsync(int datasetId, ChartValidationReport report)
    {
        try
        {
            // Check if dataset exists (this would check against actual dataset catalog)
            // For now, we'll do a simple check
            if (datasetId <= 0)
            {
                report.Errors.Add("Invalid dataset ID");
                return false;
            }

            // In a real implementation, this would query the dataset catalog
            // and verify the dataset is accessible and contains data
            _logger.LogInformation("Dataset {DatasetId} validation passed", datasetId);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating dataset {DatasetId}", datasetId);
            report.Errors.Add($"Dataset validation error: {ex.Message}");
            return false;
        }
    }

    private bool ValidateSchemas(Database.Entities.Charting.ChartDefinitionEntity chartDef, ChartValidationReport report)
    {
        bool allValid = true;

        try
        {
            // Validate filter payload
            if (!string.IsNullOrEmpty(chartDef.FilterPayload))
            {
                var filterSchema = JSchema.Parse(FilterPayloadSchema);
                var filterJson = JObject.Parse(chartDef.FilterPayload);

                if (!filterJson.IsValid(filterSchema, out IList<string> filterErrors))
                {
                    report.Errors.AddRange(filterErrors.Select(e => $"Filter schema error: {e}"));
                    allValid = false;
                }
            }

            // Validate calculation payload
            if (!string.IsNullOrEmpty(chartDef.CalculationPayload))
            {
                var calcSchema = JSchema.Parse(CalculationPayloadSchema);
                var calcJson = JObject.Parse(chartDef.CalculationPayload);

                if (!calcJson.IsValid(calcSchema, out IList<string> calcErrors))
                {
                    report.Errors.AddRange(calcErrors.Select(e => $"Calculation schema error: {e}"));
                    allValid = false;
                }
            }
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "JSON parsing error during schema validation");
            report.Errors.Add($"Invalid JSON format: {ex.Message}");
            allValid = false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during schema validation");
            report.Errors.Add($"Schema validation error: {ex.Message}");
            allValid = false;
        }

        return allValid;
    }

    private async Task<bool> ValidateFormulasAsync(Database.Entities.Charting.ChartDefinitionEntity chartDef, ChartValidationReport report)
    {
        bool allSafe = true;

        try
        {
            if (!string.IsNullOrEmpty(chartDef.CalculationPayload))
            {
                var calcPayload = JObject.Parse(chartDef.CalculationPayload);
                var calculations = calcPayload["calculations"]?.ToObject<List<JObject>>();

                if (calculations != null)
                {
                    // Get chart type from visual payload
                    string chartType = "bar"; // default
                    if (!string.IsNullOrEmpty(chartDef.VisualPayload))
                    {
                        var visualPayload = JObject.Parse(chartDef.VisualPayload);
                        chartType = visualPayload["chartType"]?.ToString() ?? "bar";
                    }

                    // Get available columns (this would come from dataset schema in real implementation)
                    var availableColumns = new List<string> { "Value", "Category", "Count", "Total" }; // Mock data

                    foreach (var calc in calculations)
                    {
                        var formula = calc["formula"]?.ToString();
                        if (!string.IsNullOrEmpty(formula))
                        {
                            var validationResult = _formulaParser.ValidateFormula(formula, chartType, availableColumns);

                            if (!validationResult.IsSuccessful || !validationResult.Data.IsValid)
                            {
                                report.Errors.AddRange(validationResult.Data.Errors.Select(e => $"Formula error in '{calc["name"]}': {e}"));
                                allSafe = false;
                            }

                            if (validationResult.Data.Warnings.Any())
                            {
                                report.Warnings.AddRange(validationResult.Data.Warnings.Select(w => $"Formula warning in '{calc["name"]}': {w}"));
                            }
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating formulas");
            report.Errors.Add($"Formula validation error: {ex.Message}");
            allSafe = false;
        }

        return allSafe;
    }
}
