using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InquirySpark.Repository.Database.Entities.Charting;

[Table("DeckSlide")]
public class DeckSlideEntity
{
    [Key]
    public int DeckSlideId { get; set; }
    public int DeckProjectId { get; set; }
    public int ChartAssetId { get; set; }
    public int SortOrder { get; set; }
    [StringLength(255)]
    public string? SlideTitle { get; set; }
    public string? SlideNotes { get; set; }
    public string? ExportOptionsJson { get; set; }

    [ForeignKey("DeckProjectId")]
    public DeckProjectEntity DeckProject { get; set; }
    [ForeignKey("ChartAssetId")]
    public ChartAssetEntity ChartAsset { get; set; }
}
