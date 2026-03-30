using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InquirySpark.Repository.Database.Entities.Charting;

[Table("ChartDefinition")]
public class ChartDefinitionEntity
{
    [Key]
    public int ChartDefinitionId { get; set; }
    public int DatasetId { get; set; }
    [Required]
    [StringLength(255)]
    public string Name { get; set; }
    public string? Description { get; set; }
    public string? Tags { get; set; }
    public string? FilterPayload { get; set; }
    public string? VisualPayload { get; set; }
    public string? CalculationPayload { get; set; }
    public int VersionNumber { get; set; }
    public bool AutoApprovedFl { get; set; }
    public int CreatedById { get; set; }
    public DateTime CreatedDt { get; set; }
    public int ModifiedById { get; set; }
    public DateTime ModifiedDt { get; set; }
    public bool IsArchivedFl { get; set; }

    public ICollection<ChartVersionEntity> Versions { get; set; } = new List<ChartVersionEntity>();
    public ICollection<ChartBuildTaskEntity> BuildTasks { get; set; } = new List<ChartBuildTaskEntity>();
    public ICollection<ChartAssetEntity> Assets { get; set; } = new List<ChartAssetEntity>();
    public ICollection<DataExportRequestEntity> ExportRequests { get; set; } = new List<DataExportRequestEntity>();
}
