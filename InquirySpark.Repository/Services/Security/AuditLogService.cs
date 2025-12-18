using InquirySpark.Repository.Database;
using InquirySpark.Repository.Database.Entities.Security;
using Microsoft.Extensions.Logging;

namespace InquirySpark.Repository.Services.Security;

public interface IAuditLogService
{
    Task LogActionAsync(int actorId, string entityType, string entityId, string action, string changes = null);
}

public class AuditLogService(InquirySparkContext context, ILogger<AuditLogService> logger) : IAuditLogService
{
    private readonly InquirySparkContext _context = context;
    private readonly ILogger<AuditLogService> _logger = logger;

    public async Task LogActionAsync(int actorId, string entityType, string entityId, string action, string changes = null)
    {
        try
        {
            var auditLog = new AuditLogEntity
            {
                ActorId = actorId,
                EntityType = entityType,
                EntityId = entityId,
                Action = action,
                Changes = changes,
                CreatedDt = DateTime.UtcNow
            };

            _context.AuditLogs.Add(auditLog);
            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to log audit action for {EntityType} {EntityId}", entityType, entityId);
        }
    }
}
