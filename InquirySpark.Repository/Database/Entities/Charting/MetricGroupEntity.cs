using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InquirySpark.Repository.Database.Entities.Charting;

[Table("MetricGroup")]
public class MetricGroupEntity
{
    [Key]
    public int MetricGroupId { get; set; }
    [Required]
    [StringLength(255)]
    public string Name { get; set; }
    public string? Description { get; set; }
    public int? ParentMetricGroupId { get; set; }
    [Required]
    [StringLength(50)]
    public string CalculationType { get; set; }
    [Column(TypeName = "decimal(18, 4)")]
    public decimal? Weight { get; set; }
    [StringLength(255)]
    public string? QuestionSetRef { get; set; }
    [Column(TypeName = "decimal(18, 4)")]
    public decimal? BenchmarkTarget { get; set; }
    public int DisplayOrder { get; set; }

    [ForeignKey("ParentMetricGroupId")]
    public MetricGroupEntity ParentMetricGroup { get; set; }
    public ICollection<MetricGroupEntity> ChildMetricGroups { get; set; } = new List<MetricGroupEntity>();
    public ICollection<MetricScoreSnapshotEntity> MetricScoreSnapshots { get; set; } = new List<MetricScoreSnapshotEntity>();
}
