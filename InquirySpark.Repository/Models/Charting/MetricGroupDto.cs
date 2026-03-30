namespace InquirySpark.Repository.Models.Charting;

public class MetricGroupDto
{
    public int MetricGroupId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int? ParentMetricGroupId { get; set; }
    public string CalculationType { get; set; } = string.Empty;
    public decimal? Weight { get; set; }
    public string? QuestionSetRef { get; set; }
    public decimal? BenchmarkTarget { get; set; }
    public int DisplayOrder { get; set; }
}
