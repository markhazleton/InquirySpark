using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InquirySpark.Repository.Database.Entities.Charting;

[Table("GaugeTile")]
public class GaugeTileEntity
{
    [Key]
    public int GaugeTileId { get; set; }
    public int DashboardDefinitionId { get; set; }
    public int MetricNodeId { get; set; }
    [Required]
    [StringLength(50)]
    public string TileType { get; set; }
    public string? ThresholdsJson { get; set; }
    public string? DrillTargetUrl { get; set; }
    [Required]
    [StringLength(50)]
    public string Size { get; set; }
    [StringLength(255)]
    public string? ColorPalette { get; set; }
    [StringLength(255)]
    public string? TrendSource { get; set; }
    public DateTime? LastRenderedDt { get; set; }

    [ForeignKey("DashboardDefinitionId")]
    public DashboardDefinitionEntity DashboardDefinition { get; set; }
}
