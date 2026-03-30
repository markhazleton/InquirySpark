using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InquirySpark.Repository.Database.Entities;

[Table("UserPreference")]
public class UserPreferenceEntity
{
    [Key]
    public int UserPreferenceId { get; set; }
    public int UserId { get; set; }
    [Required]
    [StringLength(255)]
    public string PreferenceKey { get; set; }
    [Required]
    public string PreferenceValue { get; set; }
    public DateTime ModifiedDt { get; set; }
    [Timestamp]
    public byte[] RowVersion { get; set; }
}
