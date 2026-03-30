using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InquirySpark.Repository.Database.Entities.Charting;

[Table("DashboardDefinition")]
public class DashboardDefinitionEntity
{
    [Key]
    public int DashboardDefinitionId { get; set; }
    [Required]
    [StringLength(255)]
    public string Name { get; set; }
    public string? Description { get; set; }
    [Required]
    [StringLength(255)]
    public string Slug { get; set; }
    public string? DefaultFiltersJson { get; set; }
    public string? LayoutJson { get; set; }
    public int OwnerId { get; set; }
    public DateTime CreatedDt { get; set; }
    public DateTime ModifiedDt { get; set; }
    public bool PublishedFl { get; set; }

    public ICollection<GaugeTileEntity> GaugeTiles { get; set; } = new List<GaugeTileEntity>();
}
