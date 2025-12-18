using InquirySpark.Common.Models;
using Microsoft.Extensions.Logging;
using NCalc;
using System.Text.RegularExpressions;

namespace InquirySpark.Repository.Services.Charting;

public interface IFormulaParserService
{
    BaseResponse<FormulaValidationResult> ValidateFormula(string formula, string chartType, List<string> availableColumns);
    BaseResponse<object> EvaluateFormula(string formula, Dictionary<string, object> parameters);
    List<string> GetSupportedFunctions();
    List<string> GetFunctionsForChartType(string chartType);
}

public class FormulaValidationResult
{
    public bool IsValid { get; set; }
    public List<string> Errors { get; set; } = new();
    public List<string> Warnings { get; set; } = new();
    public List<string> DetectedColumns { get; set; } = new();
    public List<string> DetectedFunctions { get; set; } = new();
    public string NormalizedFormula { get; set; }
}

public class FormulaParserService(ILogger<FormulaParserService> logger) : IFormulaParserService
{
    private readonly ILogger<FormulaParserService> _logger = logger;

    // Safe functions allowed in formulas
    private static readonly HashSet<string> AllowedFunctions = new(StringComparer.OrdinalIgnoreCase)
    {
        // Math functions
        "Abs", "Acos", "Asin", "Atan", "Ceiling", "Cos", "Exp", "Floor", "Log", "Log10",
        "Max", "Min", "Pow", "Round", "Sign", "Sin", "Sqrt", "Tan", "Truncate",
        
        // Aggregate functions
        "Sum", "Avg", "Count", "StdDev", "Variance",
        
        // String functions
        "Len", "Lower", "Upper", "Trim", "Substring", "Concat",
        
        // Conditional functions
        "If", "IsNull", "Coalesce"
    };

    // Chart type specific function compatibility
    private static readonly Dictionary<string, List<string>> ChartTypeFunctions = new()
    {
        ["bar"] = new() { "Sum", "Avg", "Count", "Max", "Min" },
        ["line"] = new() { "Sum", "Avg", "Count", "Max", "Min", "StdDev" },
        ["pie"] = new() { "Sum", "Count" },
        ["scatter"] = new() { "Avg", "Sqrt", "Pow" },
        ["gauge"] = new() { "Avg", "Sum", "Count", "Max", "Min" },
        ["heatmap"] = new() { "Sum", "Avg", "Count" }
    };

    public BaseResponse<FormulaValidationResult> ValidateFormula(string formula, string chartType, List<string> availableColumns)
    {
        try
        {
            var result = new FormulaValidationResult
            {
                IsValid = true
            };

            if (string.IsNullOrWhiteSpace(formula))
            {
                result.Errors.Add("Formula cannot be empty");
                result.IsValid = false;
                return new BaseResponse<FormulaValidationResult>(result);
            }

            // Check for dangerous keywords
            var dangerousPatterns = new[] { "System", "Reflection", "IO", "Process", "File", "Directory" };
            foreach (var pattern in dangerousPatterns)
            {
                if (formula.Contains(pattern, StringComparison.OrdinalIgnoreCase))
                {
                    result.Errors.Add($"Formula contains forbidden keyword: {pattern}");
                    result.IsValid = false;
                }
            }

            // Extract column references (e.g., [ColumnName])
            var columnPattern = @"\[([^\]]+)\]";
            var columnMatches = Regex.Matches(formula, columnPattern);
            foreach (Match match in columnMatches)
            {
                var columnName = match.Groups[1].Value;
                result.DetectedColumns.Add(columnName);

                if (!availableColumns.Contains(columnName, StringComparer.OrdinalIgnoreCase))
                {
                    result.Warnings.Add($"Column '{columnName}' not found in dataset");
                }
            }

            // Extract function calls
            var functionPattern = @"(\w+)\s*\(";
            var functionMatches = Regex.Matches(formula, functionPattern);
            foreach (Match match in functionMatches)
            {
                var functionName = match.Groups[1].Value;
                result.DetectedFunctions.Add(functionName);

                if (!AllowedFunctions.Contains(functionName))
                {
                    result.Errors.Add($"Function '{functionName}' is not allowed");
                    result.IsValid = false;
                }
            }

            // Check chart type compatibility
            if (!string.IsNullOrEmpty(chartType) && ChartTypeFunctions.ContainsKey(chartType.ToLower()))
            {
                var allowedForChart = ChartTypeFunctions[chartType.ToLower()];
                foreach (var detectedFunc in result.DetectedFunctions)
                {
                    if (!allowedForChart.Contains(detectedFunc, StringComparer.OrdinalIgnoreCase))
                    {
                        result.Warnings.Add($"Function '{detectedFunc}' may not be compatible with chart type '{chartType}'");
                    }
                }
            }

            // Try to parse with NCalc
            try
            {
                // Replace column references with dummy values for syntax checking
                var testFormula = Regex.Replace(formula, columnPattern, "1");
                var expression = new Expression(testFormula, EvaluateOptions.IgnoreCase);
                
                // Check for syntax errors
                if (expression.HasErrors())
                {
                    result.Errors.Add($"Syntax error: {expression.Error}");
                    result.IsValid = false;
                }

                result.NormalizedFormula = formula;
            }
            catch (Exception ex)
            {
                result.Errors.Add($"Formula parsing error: {ex.Message}");
                result.IsValid = false;
                _logger.LogError(ex, "Error parsing formula: {Formula}", formula);
            }

            return new BaseResponse<FormulaValidationResult>(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating formula: {Formula}", formula);
            return new BaseResponse<FormulaValidationResult>(new[] { $"Validation error: {ex.Message}" });
        }
    }

    public BaseResponse<object> EvaluateFormula(string formula, Dictionary<string, object> parameters)
    {
        try
        {
            var expression = new Expression(formula, EvaluateOptions.IgnoreCase);

            // Add parameters
            if (parameters != null)
            {
                foreach (var param in parameters)
                {
                    expression.Parameters[param.Key] = param.Value;
                }
            }

            // Evaluate
            var result = expression.Evaluate();
            return new BaseResponse<object>(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error evaluating formula: {Formula}", formula);
            return new BaseResponse<object>(new[] { $"Formula evaluation failed: {ex.Message}" });
        }
    }

    public List<string> GetSupportedFunctions()
    {
        return AllowedFunctions.OrderBy(f => f).ToList();
    }

    public List<string> GetFunctionsForChartType(string chartType)
    {
        if (string.IsNullOrEmpty(chartType))
            return GetSupportedFunctions();

        if (ChartTypeFunctions.TryGetValue(chartType.ToLower(), out var functions))
        {
            return functions.OrderBy(f => f).ToList();
        }

        return GetSupportedFunctions();
    }
}
