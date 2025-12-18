using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InquirySpark.Repository.Database.Entities.Charting;

[Table("ChartBuildJob")]
public class ChartBuildJobEntity
{
    [Key]
    public int ChartBuildJobId { get; set; }
    [Required]
    [StringLength(50)]
    public string TriggerType { get; set; }
    public int RequestedById { get; set; }
    public DateTime RequestedDt { get; set; }
    [Required]
    [StringLength(50)]
    public string Status { get; set; }
    public DateTime? StartedDt { get; set; }
    public DateTime? CompletedDt { get; set; }
    public int SuccessCount { get; set; }
    public int FailureCount { get; set; }
    public string? SummaryLog { get; set; }

    public ICollection<ChartBuildTaskEntity> BuildTasks { get; set; } = new List<ChartBuildTaskEntity>();
}
