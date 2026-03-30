namespace InquirySpark.Repository.Models.Charting;

public class ChartAssetDto
{
    public int ChartAssetId { get; set; }
    public int ChartDefinitionId { get; set; }
    public int ChartVersionId { get; set; }
    public string DisplayName { get; set; }
    public string? Description { get; set; }
    public string? Tags { get; set; }
    public DateTime GenerationDt { get; set; }
    public DateTime DataSnapshotDt { get; set; }
    public string ApprovalStatus { get; set; }
    public int UsageCount { get; set; }
    public DateTime? LastAccessedDt { get; set; }
    public string? CdnBaseUrl { get; set; }
    public string? CommentsJson { get; set; }
}
