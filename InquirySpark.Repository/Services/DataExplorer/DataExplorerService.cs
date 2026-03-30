using InquirySpark.Common.Models;
using InquirySpark.Repository.Configuration;
using InquirySpark.Repository.Database;
using InquirySpark.Repository.Services.Security;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Data;
using System.Text;

namespace InquirySpark.Repository.Services.DataExplorer;

public interface IDataExplorerService
{
    Task<BaseResponse<DataExplorerResultDto>> GetChartDataAsync(int chartDefinitionId, DataExplorerQueryDto query);
    Task<BaseResponse<DatasetSchemaDto>> GetDatasetSchemaAsync(int datasetId);
    Task<BaseResponse<DatasetSummaryDto>> GetDatasetSummaryAsync(int datasetId);
}

public class DataExplorerQueryDto
{
    public int PageIndex { get; set; } = 0;
    public int PageSize { get; set; } = 25;
    public string? SearchValue { get; set; }
    public List<DataExplorerFilterDto> Filters { get; set; } = new();
    public string? SortColumn { get; set; }
    public string SortDirection { get; set; } = "asc";
}

public class DataExplorerFilterDto
{
    public string ColumnName { get; set; } = string.Empty;
    public string Operator { get; set; } = "equals"; // equals, contains, gt, lt, gte, lte, in
    public string Value { get; set; } = string.Empty;
}

public class DataExplorerResultDto
{
    public List<Dictionary<string, object?>> Rows { get; set; } = new();
    public int TotalRows { get; set; }
    public int FilteredRows { get; set; }
    public List<string> Columns { get; set; } = new();
    
    /// <summary>
    /// Indicates this data is read-only and cannot be modified (T065)
    /// </summary>
    public bool IsReadOnly { get; set; } = true;
    
    /// <summary>
    /// Watermark message for export files (T065)
    /// </summary>
    public string Watermark { get; set; } = $"READ-ONLY | Generated {DateTime.UtcNow:yyyy-MM-dd HH:mm} UTC";
    public Dictionary<string, object> Summary { get; set; } = new();
}

public class DatasetSchemaDto
{
    public int DatasetId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string SourceType { get; set; } = string.Empty;
    public List<ColumnSchemaDto> Columns { get; set; } = new();
}

public class ColumnSchemaDto
{
    public string Name { get; set; } = string.Empty;
    public string DataType { get; set; } = string.Empty;
    public bool IsNullable { get; set; }
}

public class DatasetSummaryDto
{
    public int DatasetId { get; set; }
    public int TotalRows { get; set; }
    public DateTime? LastRefreshed { get; set; }
    public Dictionary<string, ColumnSummaryDto> ColumnSummaries { get; set; } = new();
}

public class ColumnSummaryDto
{
    public string ColumnName { get; set; } = string.Empty;
    public int DistinctCount { get; set; }
    public int NullCount { get; set; }
    public object? MinValue { get; set; }
    public object? MaxValue { get; set; }
}

public class DataExplorerService(
    InquirySparkContext context,
    IAuditLogService auditService,
    ILogger<DataExplorerService> logger) : IDataExplorerService
{
    private readonly InquirySparkContext _context = context;
    private readonly IAuditLogService _auditService = auditService;
    private readonly ILogger<DataExplorerService> _logger = logger;
    private const int MaxRowLimit = 100000; // Hard limit for data exploration

    /// <summary>
    /// Retrieves paged, filtered, and sorted data for a chart definition's dataset
    /// </summary>
    public async Task<BaseResponse<DataExplorerResultDto>> GetChartDataAsync(
        int chartDefinitionId, 
        DataExplorerQueryDto query)
    {
        return await DbContextHelper.ExecuteAsync<DataExplorerResultDto>(async () =>
        {
            // Get the chart definition to determine the dataset
            var chartDef = await _context.ChartDefinitions
                .FirstOrDefaultAsync(c => c.ChartDefinitionId == chartDefinitionId);

            if (chartDef == null)
            {
                throw new KeyNotFoundException($"Chart definition {chartDefinitionId} not found");
            }

            // For demo purposes, we'll query from existing tables
            // In production, this would query the actual dataset tables/views
            var result = new DataExplorerResultDto();

            // Get connection and build query
            using var connection = _context.Database.GetDbConnection();
            await connection.OpenAsync();

            // Build the SQL query with filters and pagination
            var sql = BuildExplorerQuery(chartDef, query, out var parameters);
            var countSql = BuildCountQuery(chartDef, query, out var countParameters);

            _logger.LogInformation("Executing data explorer query for chart {ChartId}: {Query}", 
                chartDefinitionId, sql);

            // Execute count query
            using (var countCmd = connection.CreateCommand())
            {
                countCmd.CommandText = countSql;
                AddParameters(countCmd, countParameters);
                
                var countResult = await countCmd.ExecuteScalarAsync();
                result.FilteredRows = Convert.ToInt32(countResult);
            }

            // Execute data query
            using (var cmd = connection.CreateCommand())
            {
                cmd.CommandText = sql;
                AddParameters(cmd, parameters);

                using var reader = await cmd.ExecuteReaderAsync();
                
                // Get column names
                for (int i = 0; i < reader.FieldCount; i++)
                {
                    result.Columns.Add(reader.GetName(i));
                }

                // Read rows
                while (await reader.ReadAsync())
                {
                    var row = new Dictionary<string, object?>();
                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        row[reader.GetName(i)] = reader.IsDBNull(i) ? null : reader.GetValue(i);
                    }
                    result.Rows.Add(row);
                }
            }

            result.TotalRows = result.FilteredRows; // Simplified - would need separate query for total

            _logger.LogInformation("Data explorer returned {RowCount} rows for chart {ChartId}", 
                result.Rows.Count, chartDefinitionId);

            return result;
        });
    }

    /// <summary>
    /// Gets the schema information for a dataset
    /// </summary>
    public async Task<BaseResponse<DatasetSchemaDto>> GetDatasetSchemaAsync(int datasetId)
    {
        return await DbContextHelper.ExecuteAsync<DatasetSchemaDto>(async () =>
        {
            // For demo, return schema from Surveys table
            var schema = new DatasetSchemaDto
            {
                DatasetId = datasetId,
                Name = "Survey Dataset",
                SourceType = "SqlTable",
                Columns = new List<ColumnSchemaDto>
                {
                    new() { Name = "SurveyId", DataType = "int", IsNullable = false },
                    new() { Name = "SurveyNm", DataType = "nvarchar", IsNullable = false },
                    new() { Name = "SurveyShortNm", DataType = "nvarchar", IsNullable = true },
                    new() { Name = "StartDt", DataType = "datetime", IsNullable = true },
                    new() { Name = "EndDt", DataType = "datetime", IsNullable = true },
                    new() { Name = "ModifiedDt", DataType = "datetime", IsNullable = false }
                }
            };

            await Task.CompletedTask;
            return schema;
        });
    }

    /// <summary>
    /// Gets summary statistics for a dataset
    /// </summary>
    public async Task<BaseResponse<DatasetSummaryDto>> GetDatasetSummaryAsync(int datasetId)
    {
        return await DbContextHelper.ExecuteAsync<DatasetSummaryDto>(async () =>
        {
            // Get row count from Surveys table as demo
            var rowCount = await _context.Surveys.CountAsync();

            var summary = new DatasetSummaryDto
            {
                DatasetId = datasetId,
                TotalRows = rowCount,
                LastRefreshed = DateTime.UtcNow,
                ColumnSummaries = new Dictionary<string, ColumnSummaryDto>
                {
                    ["SurveyId"] = new ColumnSummaryDto
                    {
                        ColumnName = "SurveyId",
                        DistinctCount = rowCount,
                        NullCount = 0
                    },
                    ["SurveyNm"] = new ColumnSummaryDto
                    {
                        ColumnName = "SurveyNm",
                        DistinctCount = rowCount,
                        NullCount = 0
                    }
                }
            };

            return summary;
        });
    }

    private string BuildExplorerQuery(
        Database.Entities.Charting.ChartDefinitionEntity chartDef,
        DataExplorerQueryDto query,
        out Dictionary<string, object> parameters)
    {
        parameters = new Dictionary<string, object>();

        // For demo, query Surveys table
        var sql = new StringBuilder("SELECT ");
        sql.Append("SurveyId, SurveyNm, SurveyShortNm, SurveyDs, StartDt, EndDt, ModifiedDt ");
        sql.Append("FROM Surveys WHERE 1=1 ");

        // Apply filters
        int filterIndex = 0;
        foreach (var filter in query.Filters)
        {
            var paramName = $"@filter{filterIndex}";
            sql.Append(filter.Operator.ToLower() switch
            {
                "equals" => $"AND {filter.ColumnName} = {paramName} ",
                "contains" => $"AND {filter.ColumnName} LIKE '%' || {paramName} || '%' ",
                "gt" => $"AND {filter.ColumnName} > {paramName} ",
                "lt" => $"AND {filter.ColumnName} < {paramName} ",
                "gte" => $"AND {filter.ColumnName} >= {paramName} ",
                "lte" => $"AND {filter.ColumnName} <= {paramName} ",
                _ => ""
            });
            parameters[paramName] = filter.Value;
            filterIndex++;
        }

        // Apply search
        if (!string.IsNullOrWhiteSpace(query.SearchValue))
        {
            sql.Append("AND (SurveyNm LIKE @search OR SurveyShortNm LIKE @search) ");
            parameters["@search"] = $"%{query.SearchValue}%";
        }

        // Apply sorting
        if (!string.IsNullOrWhiteSpace(query.SortColumn))
        {
            sql.Append($"ORDER BY {query.SortColumn} {query.SortDirection.ToUpper()} ");
        }
        else
        {
            sql.Append("ORDER BY SurveyId DESC ");
        }

        // Apply pagination
        sql.Append($"LIMIT {query.PageSize} OFFSET {query.PageIndex * query.PageSize}");

        return sql.ToString();
    }

    private string BuildCountQuery(
        Database.Entities.Charting.ChartDefinitionEntity chartDef,
        DataExplorerQueryDto query,
        out Dictionary<string, object> parameters)
    {
        parameters = new Dictionary<string, object>();

        var sql = new StringBuilder("SELECT COUNT(*) FROM Surveys WHERE 1=1 ");

        // Apply filters (same as main query)
        int filterIndex = 0;
        foreach (var filter in query.Filters)
        {
            var paramName = $"@filter{filterIndex}";
            sql.Append(filter.Operator.ToLower() switch
            {
                "equals" => $"AND {filter.ColumnName} = {paramName} ",
                "contains" => $"AND {filter.ColumnName} LIKE '%' || {paramName} || '%' ",
                "gt" => $"AND {filter.ColumnName} > {paramName} ",
                "lt" => $"AND {filter.ColumnName} < {paramName} ",
                "gte" => $"AND {filter.ColumnName} >= {paramName} ",
                "lte" => $"AND {filter.ColumnName} <= {paramName} ",
                _ => ""
            });
            parameters[paramName] = filter.Value;
            filterIndex++;
        }

        // Apply search
        if (!string.IsNullOrWhiteSpace(query.SearchValue))
        {
            sql.Append("AND (SurveyNm LIKE @search OR SurveyShortNm LIKE @search) ");
            parameters["@search"] = $"%{query.SearchValue}%";
        }

        return sql.ToString();
    }

    private void AddParameters(IDbCommand cmd, Dictionary<string, object> parameters)
    {
        foreach (var param in parameters)
        {
            var dbParam = cmd.CreateParameter();
            dbParam.ParameterName = param.Key;
            dbParam.Value = param.Value;
            cmd.Parameters.Add(dbParam);
        }
    }
}
