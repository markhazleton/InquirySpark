using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InquirySpark.Repository.Database.Entities.Charting;

[Table("DataExportRequest")]
public class DataExportRequestEntity
{
    [Key]
    public int DataExportRequestId { get; set; }
    public int ChartDefinitionId { get; set; }
    public int RequestedById { get; set; }
    public DateTime RequestedDt { get; set; }
    public string? FilterPayload { get; set; }
    public string? ColumnSettingsJson { get; set; }
    [Required]
    [StringLength(50)]
    public string Format { get; set; }
    public int RowCount { get; set; }
    [Required]
    [StringLength(50)]
    public string Status { get; set; }
    public DateTime? CompletionDt { get; set; }
    public string? BlobPath { get; set; }

    [ForeignKey("ChartDefinitionId")]
    public ChartDefinitionEntity ChartDefinition { get; set; }
}
