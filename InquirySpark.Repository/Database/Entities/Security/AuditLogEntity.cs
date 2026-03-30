using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InquirySpark.Repository.Database.Entities.Security;

[Table("AuditLog")]
public class AuditLogEntity
{
    [Key]
    public int AuditLogId { get; set; }
    public int ActorId { get; set; }
    [Required]
    [StringLength(255)]
    public string EntityType { get; set; } = string.Empty;
    [Required]
    [StringLength(255)]
    public string EntityId { get; set; } = string.Empty;
    [Required]
    [StringLength(50)]
    public string Action { get; set; } = string.Empty;
    public string? Changes { get; set; }
    public DateTime CreatedDt { get; set; }
}
