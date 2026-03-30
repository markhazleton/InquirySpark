using InquirySpark.Common.Models;

namespace InquirySpark.Repository.Services.Charting;

/// <summary>
/// Service interface for chart build job management
/// </summary>
public interface IChartBuildService
{
    /// <summary>
    /// Creates a new chart build job with tasks for all active chart definitions
    /// </summary>
    /// <param name="triggerType">Type of trigger (Manual, Schedule, Webhook)</param>
    /// <param name="requestedById">User ID who triggered the build</param>
    /// <returns>Build job ID and task count</returns>
    Task<BaseResponse<ChartBuildJobDto>> CreateBuildJobAsync(string triggerType, int requestedById);

    /// <summary>
    /// Creates a build job for specific chart definitions only
    /// </summary>
    /// <param name="triggerType">Type of trigger</param>
    /// <param name="requestedById">User ID who triggered the build</param>
    /// <param name="chartDefinitionIds">Specific definition IDs to build</param>
    /// <returns>Build job with filtered tasks</returns>
    Task<BaseResponse<ChartBuildJobDto>> CreateSelectiveBuildJobAsync(string triggerType, int requestedById, List<int> chartDefinitionIds);

    /// <summary>
    /// Gets build job status with task details
    /// </summary>
    /// <param name="jobId">Build job identifier</param>
    /// <returns>Build job with nested task collection</returns>
    Task<BaseResponse<ChartBuildJobDto>> GetBuildJobAsync(int jobId);

    /// <summary>
    /// Gets recent build jobs with summary stats
    /// </summary>
    /// <param name="limit">Number of jobs to retrieve (default 20)</param>
    /// <returns>Collection of build jobs</returns>
    Task<BaseResponseCollection<ChartBuildJobDto>> GetRecentBuildJobsAsync(int limit = 20);

    /// <summary>
    /// Updates task status after processing
    /// </summary>
    /// <param name="taskId">Build task identifier</param>
    /// <param name="status">New status (Running, Completed, Failed)</param>
    /// <param name="errorPayload">Optional error details if failed</param>
    /// <returns>Success indicator</returns>
    Task<BaseResponse<bool>> UpdateTaskStatusAsync(int taskId, string status, string? errorPayload = null);

    /// <summary>
    /// Retries failed tasks within a build job
    /// </summary>
    /// <param name="jobId">Build job identifier</param>
    /// <returns>Count of tasks requeued</returns>
    Task<BaseResponse<int>> RetryFailedTasksAsync(int jobId);

    /// <summary>
    /// Cancels a running build job
    /// </summary>
    /// <param name="jobId">Build job identifier</param>
    /// <returns>Success indicator</returns>
    Task<BaseResponse<bool>> CancelBuildJobAsync(int jobId);

    /// <summary>
    /// Gets throughput statistics for build monitoring
    /// </summary>
    /// <returns>Stats including charts/min, avg duration, success rate</returns>
    Task<BaseResponse<BuildThroughputStatsDto>> GetThroughputStatsAsync();
}

/// <summary>
/// DTO for chart build job
/// </summary>
public class ChartBuildJobDto
{
    public int ChartBuildJobId { get; set; }
    public string TriggerType { get; set; } = string.Empty;
    public int RequestedById { get; set; }
    public DateTime RequestedDt { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime? StartedDt { get; set; }
    public DateTime? CompletedDt { get; set; }
    public int SuccessCount { get; set; }
    public int FailureCount { get; set; }
    public string? SummaryLog { get; set; }
    public List<ChartBuildTaskDto> BuildTasks { get; set; } = new();
}

/// <summary>
/// DTO for individual chart build task
/// </summary>
public class ChartBuildTaskDto
{
    public int ChartBuildTaskId { get; set; }
    public int ChartBuildJobId { get; set; }
    public int ChartDefinitionId { get; set; }
    public int ChartVersionId { get; set; }
    public int Priority { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime? StartedDt { get; set; }
    public DateTime? CompletedDt { get; set; }
    public string? ErrorPayload { get; set; }
    public string ChartName { get; set; } = string.Empty;
}

/// <summary>
/// DTO for build throughput statistics
/// </summary>
public class BuildThroughputStatsDto
{
    public double ChartsPerMinute { get; set; }
    public double AverageDurationMinutes { get; set; }
    public double SuccessRate { get; set; }
    public int TotalJobsLast24Hours { get; set; }
    public int TotalTasksLast24Hours { get; set; }
    public int FailedTasksLast24Hours { get; set; }
}
