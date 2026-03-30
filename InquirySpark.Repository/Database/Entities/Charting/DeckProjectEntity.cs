using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InquirySpark.Repository.Database.Entities.Charting;

[Table("DeckProject")]
public class DeckProjectEntity
{
    [Key]
    public int DeckProjectId { get; set; }
    [Required]
    [StringLength(255)]
    public string Name { get; set; }
    public string? Description { get; set; }
    public int OwnerId { get; set; }
    [Required]
    [StringLength(50)]
    public string Status { get; set; }
    [StringLength(255)]
    public string? Theme { get; set; }
    public DateTime CreatedDt { get; set; }
    public DateTime ModifiedDt { get; set; }

    public ICollection<DeckSlideEntity> Slides { get; set; } = new List<DeckSlideEntity>();
}
