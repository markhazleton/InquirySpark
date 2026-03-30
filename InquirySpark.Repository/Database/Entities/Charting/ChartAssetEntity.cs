using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InquirySpark.Repository.Database.Entities.Charting;

[Table("ChartAsset")]
public class ChartAssetEntity
{
    [Key]
    public int ChartAssetId { get; set; }
    public int ChartDefinitionId { get; set; }
    public int ChartVersionId { get; set; }
    [Required]
    [StringLength(255)]
    public string DisplayName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Tags { get; set; }
    public DateTime GenerationDt { get; set; }
    public DateTime DataSnapshotDt { get; set; }
    [Required]
    [StringLength(50)]
    public string ApprovalStatus { get; set; } = string.Empty;
    public int UsageCount { get; set; }
    public DateTime? LastAccessedDt { get; set; }
    public string? CdnBaseUrl { get; set; }
    public string? CommentsJson { get; set; }

    [ForeignKey("ChartDefinitionId")]
    public ChartDefinitionEntity ChartDefinition { get; set; } = null!;
    [ForeignKey("ChartVersionId")]
    public ChartVersionEntity ChartVersion { get; set; } = null!;
    public ICollection<ChartAssetFileEntity> Files { get; set; } = new List<ChartAssetFileEntity>();
    public ICollection<DeckSlideEntity> DeckSlides { get; set; } = new List<DeckSlideEntity>();
}
