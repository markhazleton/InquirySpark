namespace InquirySpark.Repository.Services.Charting;

/// <summary>
/// Statistics for chart build throughput
/// </summary>
public class ChartBuildThroughputDto
{
    /// <summary>
    /// Total number of builds in the period
    /// </summary>
    public int TotalBuilds { get; set; }

    /// <summary>
    /// Number of successful builds
    /// </summary>
    public int SuccessfulBuilds { get; set; }

    /// <summary>
    /// Number of failed builds
    /// </summary>
    public int FailedBuilds { get; set; }

    /// <summary>
    /// Average build time in seconds
    /// </summary>
    public double AverageBuildTimeSeconds { get; set; }

    /// <summary>
    /// Start date of the reporting period
    /// </summary>
    public DateTime? StartDate { get; set; }

    /// <summary>
    /// End date of the reporting period
    /// </summary>
    public DateTime? EndDate { get; set; }
}
