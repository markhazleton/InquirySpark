namespace InquirySpark.Repository.Models.Charting;

public class ChartDefinitionDto
{
    public int ChartDefinitionId { get; set; }
    public int DatasetId { get; set; }
    public string Name { get; set; }
    public string? Description { get; set; }
    public string? Tags { get; set; }
    public string? FilterPayload { get; set; }
    public string? VisualPayload { get; set; }
    public string? CalculationPayload { get; set; }
    public int VersionNumber { get; set; }
    public bool AutoApprovedFl { get; set; }
    public int CreatedById { get; set; }
    public DateTime CreatedDt { get; set; }
    public int ModifiedById { get; set; }
    public DateTime ModifiedDt { get; set; }
    public bool IsArchivedFl { get; set; }
}
