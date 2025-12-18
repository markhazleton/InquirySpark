using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InquirySpark.Repository.Database.Entities.Charting;

[Table("ChartAssetFile")]
public class ChartAssetFileEntity
{
    [Key]
    public int ChartAssetFileId { get; set; }
    public int ChartAssetId { get; set; }
    [Required]
    [StringLength(50)]
    public string Format { get; set; }
    [StringLength(50)]
    public string? ResolutionHint { get; set; }
    [Required]
    public string BlobPath { get; set; }
    public long FileSizeBytes { get; set; }
    [StringLength(255)]
    public string? Checksum { get; set; }
    public DateTime? ExpiresDt { get; set; }

    [ForeignKey("ChartAssetId")]
    public ChartAssetEntity ChartAsset { get; set; }
}
