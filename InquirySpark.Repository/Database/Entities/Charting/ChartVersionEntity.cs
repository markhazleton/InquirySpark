using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InquirySpark.Repository.Database.Entities.Charting;

[Table("ChartVersion")]
public class ChartVersionEntity
{
    [Key]
    public int ChartVersionId { get; set; }
    public int ChartDefinitionId { get; set; }
    public int VersionNumber { get; set; }
    public string? SnapshotPayload { get; set; }
    public bool ApprovedFl { get; set; }
    public int? ApprovedById { get; set; }
    public DateTime? ApprovedDt { get; set; }
    public string? DiffSummary { get; set; }
    public int? RollbackSourceVersionNumber { get; set; }

    [ForeignKey("ChartDefinitionId")]
    public ChartDefinitionEntity ChartDefinition { get; set; }
    public ICollection<ChartBuildTaskEntity> BuildTasks { get; set; } = new List<ChartBuildTaskEntity>();
    public ICollection<ChartAssetEntity> Assets { get; set; } = new List<ChartAssetEntity>();
}
