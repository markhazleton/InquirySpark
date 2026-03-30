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
    public string PreferenceKey { get; set; } = string.Empty;
    [Required]
    public string PreferenceValue { get; set; } = string.Empty;
    public DateTime ModifiedDt { get; set; }
    [Timestamp]
    public byte[] RowVersion { get; set; } = Array.Empty<byte>();
}
