using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InquirySpark.Repository.Database.Entities.Charting;

[Table("ChartBuildTask")]
public class ChartBuildTaskEntity
{
    [Key]
    public int ChartBuildTaskId { get; set; }
    public int ChartBuildJobId { get; set; }
    public int ChartDefinitionId { get; set; }
    public int ChartVersionId { get; set; }
    public int Priority { get; set; }
    [Required]
    [StringLength(50)]
    public string Status { get; set; }
    public DateTime? StartedDt { get; set; }
    public DateTime? CompletedDt { get; set; }
    public string? ErrorPayload { get; set; }

    [ForeignKey("ChartBuildJobId")]
    public ChartBuildJobEntity ChartBuildJob { get; set; }
    [ForeignKey("ChartDefinitionId")]
    public ChartDefinitionEntity ChartDefinition { get; set; }
    [ForeignKey("ChartVersionId")]
    public ChartVersionEntity ChartVersion { get; set; }
}
