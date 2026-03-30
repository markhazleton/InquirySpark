using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InquirySpark.Repository.Database.Entities.Charting;

[Table("MetricScoreSnapshot")]
public class MetricScoreSnapshotEntity
{
    [Key]
    public int MetricScoreSnapshotId { get; set; }
    public int MetricGroupId { get; set; }
    public DateTime SnapshotDt { get; set; }
    [Required]
    [StringLength(255)]
    public string FilterHash { get; set; }
    [Column(TypeName = "decimal(18, 4)")]
    public decimal ScoreValue { get; set; }
    [Column(TypeName = "decimal(18, 4)")]
    public decimal? TargetValue { get; set; }
    public int SampleSize { get; set; }
    [Column(TypeName = "decimal(18, 4)")]
    public decimal? TrendDelta { get; set; }
    public int DataVersionId { get; set; }

    [ForeignKey("MetricGroupId")]
    public MetricGroupEntity MetricGroup { get; set; }
}
