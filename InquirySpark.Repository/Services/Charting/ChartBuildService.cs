using InquirySpark.Common.Models;
using InquirySpark.Repository.Configuration;
using InquirySpark.Repository.Database;
using InquirySpark.Repository.Database.Entities.Charting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace InquirySpark.Repository.Services.Charting;

/// <summary>
/// Service for managing chart build jobs and tasks
/// </summary>
public class ChartBuildService(InquirySparkContext context, ILogger<ChartBuildService> logger) : IChartBuildService
{
    private readonly InquirySparkContext _context = context;
    private readonly ILogger<ChartBuildService> _logger = logger;

    /// <inheritdoc/>
    public async Task<BaseResponse<ChartBuildJobDto>> CreateBuildJobAsync(string triggerType, int requestedById)
    {
        return await DbContextHelper.ExecuteAsync<ChartBuildJobDto>(async () =>
        {
            // Get all active, approved chart definitions
            var activeDefinitions = await _context.ChartDefinitions
                .Where(d => d.IsArchivedFl == false && d.AutoApprovedFl == true)
                .Include(d => d.Versions.OrderByDescending(v => v.VersionNumber).Take(1))
                .ToListAsync();

            if (activeDefinitions.Count == 0)
            {
                throw new InvalidOperationException("No active chart definitions found to build");
            }

            // Create parent job
            var job = new ChartBuildJobEntity
            {
                TriggerType = triggerType,
                RequestedById = requestedById,
                RequestedDt = DateTime.UtcNow,
                Status = "Pending",
                SuccessCount = 0,
                FailureCount = 0
            };

            _context.ChartBuildJobs.Add(job);
            await _context.SaveChangesAsync();

            // Create child tasks
            var tasks = new List<ChartBuildTaskEntity>();
            foreach (var definition in activeDefinitions)
            {
                var latestVersion = definition.Versions.FirstOrDefault();
                if (latestVersion != null)
                {
                    tasks.Add(new ChartBuildTaskEntity
                    {
                        ChartBuildJobId = job.ChartBuildJobId,
                        ChartDefinitionId = definition.ChartDefinitionId,
                        ChartVersionId = latestVersion.ChartVersionId,
                        Priority = 0, // Default priority
                        Status = "Pending",
                        StartedDt = null,
                        CompletedDt = null
                    });
                }
            }

            _context.ChartBuildTasks.AddRange(tasks);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Created build job {JobId} with {TaskCount} tasks (Trigger: {TriggerType}, User: {UserId})",
                job.ChartBuildJobId, tasks.Count, triggerType, requestedById);

            // Return DTO
            return new ChartBuildJobDto
            {
                ChartBuildJobId = job.ChartBuildJobId,
                TriggerType = job.TriggerType,
                RequestedById = job.RequestedById,
                RequestedDt = job.RequestedDt,
                Status = job.Status,
                SuccessCount = 0,
                FailureCount = 0,
                BuildTasks = tasks.Select(t => new ChartBuildTaskDto
                {
                    ChartBuildTaskId = t.ChartBuildTaskId,
                    ChartBuildJobId = t.ChartBuildJobId,
                    ChartDefinitionId = t.ChartDefinitionId,
                    ChartVersionId = t.ChartVersionId,
                    Priority = t.Priority,
                    Status = t.Status
                }).ToList()
            };
        });
    }

    /// <inheritdoc/>
    public async Task<BaseResponse<ChartBuildJobDto>> CreateSelectiveBuildJobAsync(string triggerType, int requestedById, List<int> chartDefinitionIds)
    {
        return await DbContextHelper.ExecuteAsync<ChartBuildJobDto>(async () =>
        {
            // Get specific chart definitions
            var definitions = await _context.ChartDefinitions
                .Where(d => chartDefinitionIds.Contains(d.ChartDefinitionId))
                .Where(d => d.IsArchivedFl == false)
                .Include(d => d.Versions.OrderByDescending(v => v.VersionNumber).Take(1))
                .ToListAsync();

            if (definitions.Count == 0)
            {
                throw new InvalidOperationException("No valid chart definitions found");
            }

            // Create parent job
            var job = new ChartBuildJobEntity
            {
                TriggerType = triggerType,
                RequestedById = requestedById,
                RequestedDt = DateTime.UtcNow,
                Status = "Pending",
                SuccessCount = 0,
                FailureCount = 0,
                SummaryLog = $"Selective build for {definitions.Count} definitions"
            };

            _context.ChartBuildJobs.Add(job);
            await _context.SaveChangesAsync();

            // Create child tasks
            var tasks = new List<ChartBuildTaskEntity>();
            foreach (var definition in definitions)
            {
                var latestVersion = definition.Versions.FirstOrDefault();
                if (latestVersion != null)
                {
                    tasks.Add(new ChartBuildTaskEntity
                    {
                        ChartBuildJobId = job.ChartBuildJobId,
                        ChartDefinitionId = definition.ChartDefinitionId,
                        ChartVersionId = latestVersion.ChartVersionId,
                        Priority = 0,
                        Status = "Pending"
                    });
                }
            }

            _context.ChartBuildTasks.AddRange(tasks);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Created selective build job {JobId} with {TaskCount} tasks",
                job.ChartBuildJobId, tasks.Count);

            return new ChartBuildJobDto
            {
                ChartBuildJobId = job.ChartBuildJobId,
                TriggerType = job.TriggerType,
                RequestedById = job.RequestedById,
                RequestedDt = job.RequestedDt,
                Status = job.Status,
                SuccessCount = 0,
                FailureCount = 0,
                SummaryLog = job.SummaryLog,
                BuildTasks = tasks.Select(t => new ChartBuildTaskDto
                {
                    ChartBuildTaskId = t.ChartBuildTaskId,
                    ChartBuildJobId = t.ChartBuildJobId,
                    ChartDefinitionId = t.ChartDefinitionId,
                    ChartVersionId = t.ChartVersionId,
                    Priority = t.Priority,
                    Status = t.Status
                }).ToList()
            };
        });
    }

    /// <inheritdoc/>
    public async Task<BaseResponse<ChartBuildJobDto>> GetBuildJobAsync(int jobId)
    {
        return await DbContextHelper.ExecuteAsync<ChartBuildJobDto>(async () =>
        {
            var job = await _context.ChartBuildJobs
                .Include(j => j.BuildTasks)
                    .ThenInclude(t => t.ChartDefinition)
                .FirstOrDefaultAsync(j => j.ChartBuildJobId == jobId);

            if (job == null)
            {
                throw new InvalidOperationException($"Build job {jobId} not found");
            }

            return new ChartBuildJobDto
            {
                ChartBuildJobId = job.ChartBuildJobId,
                TriggerType = job.TriggerType,
                RequestedById = job.RequestedById,
                RequestedDt = job.RequestedDt,
                Status = job.Status,
                StartedDt = job.StartedDt,
                CompletedDt = job.CompletedDt,
                SuccessCount = job.SuccessCount,
                FailureCount = job.FailureCount,
                SummaryLog = job.SummaryLog,
                BuildTasks = job.BuildTasks.Select(t => new ChartBuildTaskDto
                {
                    ChartBuildTaskId = t.ChartBuildTaskId,
                    ChartBuildJobId = t.ChartBuildJobId,
                    ChartDefinitionId = t.ChartDefinitionId,
                    ChartVersionId = t.ChartVersionId,
                    Priority = t.Priority,
                    Status = t.Status,
                    StartedDt = t.StartedDt,
                    CompletedDt = t.CompletedDt,
                    ErrorPayload = t.ErrorPayload,
                    ChartName = t.ChartDefinition?.Name ?? "Unknown"
                }).ToList()
            };
        });
    }

    /// <inheritdoc/>
    public async Task<BaseResponseCollection<ChartBuildJobDto>> GetRecentBuildJobsAsync(int limit = 20)
    {
        return await DbContextHelper.ExecuteCollectionAsync<ChartBuildJobDto>(async () =>
        {
            var jobs = await _context.ChartBuildJobs
                .OrderByDescending(j => j.RequestedDt)
                .Take(limit)
                .Select(j => new ChartBuildJobDto
                {
                    ChartBuildJobId = j.ChartBuildJobId,
                    TriggerType = j.TriggerType,
                    RequestedById = j.RequestedById,
                    RequestedDt = j.RequestedDt,
                    Status = j.Status,
                    StartedDt = j.StartedDt,
                    CompletedDt = j.CompletedDt,
                    SuccessCount = j.SuccessCount,
                    FailureCount = j.FailureCount,
                    SummaryLog = j.SummaryLog
                })
                .ToListAsync();

            return jobs;
        });
    }

    /// <inheritdoc/>
    public async Task<BaseResponse<bool>> UpdateTaskStatusAsync(int taskId, string status, string? errorPayload = null)
    {
        return await DbContextHelper.ExecuteAsync<bool>(async () =>
        {
            var task = await _context.ChartBuildTasks
                .Include(t => t.ChartBuildJob)
                .FirstOrDefaultAsync(t => t.ChartBuildTaskId == taskId);

            if (task == null)
            {
                throw new InvalidOperationException($"Build task {taskId} not found");
            }

            var oldStatus = task.Status;
            task.Status = status;
            task.ErrorPayload = errorPayload;

            if (status == "Running" && task.StartedDt == null)
            {
                task.StartedDt = DateTime.UtcNow;
                
                // Update job status to Running if first task
                if (task.ChartBuildJob.Status == "Pending")
                {
                    task.ChartBuildJob.Status = "Running";
                    task.ChartBuildJob.StartedDt = DateTime.UtcNow;
                }
            }
            else if ((status == "Completed" || status == "Failed") && task.CompletedDt == null)
            {
                task.CompletedDt = DateTime.UtcNow;

                // Update job counters
                if (status == "Completed")
                {
                    task.ChartBuildJob.SuccessCount++;
                }
                else
                {
                    task.ChartBuildJob.FailureCount++;
                }

                // Check if job is complete
                var allTasks = await _context.ChartBuildTasks
                    .Where(t => t.ChartBuildJobId == task.ChartBuildJobId)
                    .ToListAsync();

                var pendingCount = allTasks.Count(t => t.Status == "Pending" || t.Status == "Running");
                if (pendingCount == 0)
                {
                    task.ChartBuildJob.Status = task.ChartBuildJob.FailureCount > 0 ? "Completed" : "Completed";
                    task.ChartBuildJob.CompletedDt = DateTime.UtcNow;
                    task.ChartBuildJob.SummaryLog = $"Completed: {task.ChartBuildJob.SuccessCount} succeeded, {task.ChartBuildJob.FailureCount} failed";
                }
            }

            await _context.SaveChangesAsync();

            _logger.LogInformation("Task {TaskId} status updated: {OldStatus} → {NewStatus}", 
                taskId, oldStatus, status);

            return true;
        });
    }

    /// <inheritdoc/>
    public async Task<BaseResponse<int>> RetryFailedTasksAsync(int jobId)
    {
        return await DbContextHelper.ExecuteAsync<int>(async () =>
        {
            var failedTasks = await _context.ChartBuildTasks
                .Where(t => t.ChartBuildJobId == jobId && t.Status == "Failed")
                .ToListAsync();

            if (failedTasks.Count == 0)
            {
                return 0;
            }

            foreach (var task in failedTasks)
            {
                task.Status = "Pending";
                task.StartedDt = null;
                task.CompletedDt = null;
                task.ErrorPayload = null;
            }

            var job = await _context.ChartBuildJobs.FindAsync(jobId);
            if (job != null)
            {
                job.FailureCount -= failedTasks.Count;
                job.Status = "Running";
            }

            await _context.SaveChangesAsync();

            _logger.LogInformation("Retrying {Count} failed tasks for job {JobId}", failedTasks.Count, jobId);

            return failedTasks.Count;
        });
    }

    /// <inheritdoc/>
    public async Task<BaseResponse<bool>> CancelBuildJobAsync(int jobId)
    {
        return await DbContextHelper.ExecuteAsync<bool>(async () =>
        {
            var job = await _context.ChartBuildJobs.FindAsync(jobId);
            if (job == null)
            {
                throw new InvalidOperationException($"Build job {jobId} not found");
            }

            if (job.Status != "Pending" && job.Status != "Running")
            {
                throw new InvalidOperationException($"Cannot cancel job in {job.Status} status");
            }

            job.Status = "Cancelled";
            job.CompletedDt = DateTime.UtcNow;
            job.SummaryLog = $"Cancelled by user at {DateTime.UtcNow}";

            // Cancel pending/running tasks
            var activeTasks = await _context.ChartBuildTasks
                .Where(t => t.ChartBuildJobId == jobId && (t.Status == "Pending" || t.Status == "Running"))
                .ToListAsync();

            foreach (var task in activeTasks)
            {
                task.Status = "Cancelled";
                task.CompletedDt = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();

            _logger.LogWarning("Build job {JobId} cancelled ({TaskCount} tasks affected)", 
                jobId, activeTasks.Count);

            return true;
        });
    }

    /// <inheritdoc/>
    public async Task<BaseResponse<BuildThroughputStatsDto>> GetThroughputStatsAsync()
    {
        return await DbContextHelper.ExecuteAsync<BuildThroughputStatsDto>(async () =>
        {
            var cutoff = DateTime.UtcNow.AddHours(-24);
            
            var jobs = await _context.ChartBuildJobs
                .Where(j => j.RequestedDt >= cutoff)
                .ToListAsync();

            var tasks = await _context.ChartBuildTasks
                .Where(t => t.CompletedDt >= cutoff)
                .ToListAsync();

            var completedTasks = tasks.Where(t => t.Status == "Completed" || t.Status == "Failed").ToList();
            var successfulTasks = completedTasks.Where(t => t.Status == "Completed").ToList();

            var avgDuration = completedTasks.Any()
                ? completedTasks
                    .Where(t => t.StartedDt.HasValue && t.CompletedDt.HasValue)
                    .Select(t => (t.CompletedDt!.Value - t.StartedDt!.Value).TotalMinutes)
                    .Average()
                : 0;

            var chartsPerMinute = completedTasks.Any()
                ? completedTasks.Count / 1440.0 // 24 hours = 1440 minutes
                : 0;

            var successRate = completedTasks.Any()
                ? (double)successfulTasks.Count / completedTasks.Count * 100
                : 0;

            return new BuildThroughputStatsDto
            {
                ChartsPerMinute = Math.Round(chartsPerMinute, 2),
                AverageDurationMinutes = Math.Round(avgDuration, 2),
                SuccessRate = Math.Round(successRate, 2),
                TotalJobsLast24Hours = jobs.Count,
                TotalTasksLast24Hours = tasks.Count,
                FailedTasksLast24Hours = tasks.Count(t => t.Status == "Failed")
            };
        });
    }
}
