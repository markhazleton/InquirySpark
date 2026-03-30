using InquirySpark.Common.Models;
using InquirySpark.Repository.Configuration;
using Microsoft.Extensions.Logging;

namespace InquirySpark.Repository.Services.Notifications;

public interface IBuildNotificationService
{
    Task<BaseResponse<bool>> SendJobStartedNotificationAsync(int jobId, int requestedById, int taskCount);
    Task<BaseResponse<bool>> SendJobCompletedNotificationAsync(int jobId, int successCount, int failureCount, TimeSpan duration);
    Task<BaseResponse<bool>> SendJobFailedNotificationAsync(int jobId, string errorMessage);
    Task<BaseResponse<bool>> SendTaskFailureAlertAsync(int taskId, int chartDefinitionId, string errorMessage);
}

public class BuildNotificationService(ILogger<BuildNotificationService> logger) : IBuildNotificationService
{
    private readonly ILogger<BuildNotificationService> _logger = logger;

    /// <summary>
    /// Sends notification when a build job starts
    /// </summary>
    public async Task<BaseResponse<bool>> SendJobStartedNotificationAsync(int jobId, int requestedById, int taskCount)
    {
        return await DbContextHelper.ExecuteAsync<bool>(async () =>
        {
            _logger.LogInformation(
                "Build job {JobId} started by user {UserId} with {TaskCount} tasks",
                jobId, requestedById, taskCount);

            // TODO: Implement actual notification delivery
            // Options:
            // 1. Email via SMTP or SendGrid
            // 2. In-app notification (database record)
            // 3. Teams/Slack webhook
            // 4. SignalR real-time push

            await Task.CompletedTask;
            return true;
        });
    }

    /// <summary>
    /// Sends notification when a build job completes successfully or with partial failures
    /// </summary>
    public async Task<BaseResponse<bool>> SendJobCompletedNotificationAsync(
        int jobId, 
        int successCount, 
        int failureCount, 
        TimeSpan duration)
    {
        return await DbContextHelper.ExecuteAsync<bool>(async () =>
        {
            var status = failureCount == 0 ? "SUCCESS" : "COMPLETED WITH FAILURES";
            var chartsPerMinute = duration.TotalMinutes > 0 
                ? Math.Round((successCount + failureCount) / duration.TotalMinutes, 2) 
                : 0;

            _logger.LogInformation(
                "Build job {JobId} completed: {Status} - {SuccessCount} succeeded, {FailureCount} failed, Duration: {Duration:hh\\:mm\\:ss}, Throughput: {ChartsPerMin} charts/min",
                jobId, status, successCount, failureCount, duration, chartsPerMinute);

            // TODO: Send notification with job summary
            // Include:
            // - Job ID and completion status
            // - Success/failure counts
            // - Duration and throughput metrics
            // - Link to view results in Admin UI

            await Task.CompletedTask;
            return true;
        });
    }

    /// <summary>
    /// Sends notification when a build job fails catastrophically
    /// </summary>
    public async Task<BaseResponse<bool>> SendJobFailedNotificationAsync(int jobId, string errorMessage)
    {
        return await DbContextHelper.ExecuteAsync<bool>(async () =>
        {
            _logger.LogError(
                "Build job {JobId} FAILED: {ErrorMessage}",
                jobId, errorMessage);

            // TODO: Send urgent failure notification
            // This should alert operators immediately
            // Consider using multiple channels (email + in-app + Teams)

            await Task.CompletedTask;
            return true;
        });
    }

    /// <summary>
    /// Sends alert when an individual task fails
    /// Used for monitoring and troubleshooting
    /// </summary>
    public async Task<BaseResponse<bool>> SendTaskFailureAlertAsync(
        int taskId, 
        int chartDefinitionId, 
        string errorMessage)
    {
        return await DbContextHelper.ExecuteAsync<bool>(async () =>
        {
            _logger.LogWarning(
                "Task {TaskId} for chart definition {ChartDefinitionId} failed: {ErrorMessage}",
                taskId, chartDefinitionId, errorMessage);

            // TODO: Send task failure alert
            // Could be batched to avoid notification spam
            // Or use severity thresholds (e.g., only alert after X failures)

            await Task.CompletedTask;
            return true;
        });
    }
}

/// <summary>
/// Extension methods for integrating notifications with build services
/// </summary>
public static class BuildNotificationExtensions
{
    /// <summary>
    /// Sends appropriate notification based on job status
    /// </summary>
    public static async Task NotifyJobStatusAsync(
        this IBuildNotificationService notificationService,
        int jobId,
        string status,
        int requestedById,
        int successCount,
        int failureCount,
        DateTime? startedDt,
        DateTime? completedDt)
    {
        if (status == "Running" && startedDt.HasValue)
        {
            await notificationService.SendJobStartedNotificationAsync(
                jobId, 
                requestedById, 
                successCount + failureCount);
        }
        else if (status == "Completed" && startedDt.HasValue && completedDt.HasValue)
        {
            var duration = completedDt.Value - startedDt.Value;
            await notificationService.SendJobCompletedNotificationAsync(
                jobId, 
                successCount, 
                failureCount, 
                duration);
        }
        else if (status == "Failed")
        {
            await notificationService.SendJobFailedNotificationAsync(
                jobId, 
                "Job failed - check logs for details");
        }
    }
}
