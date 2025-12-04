namespace InquirySpark.Admin.Models;

/// <summary>
/// System health view model
/// </summary>
public class SystemHealthViewModel
{
    public DatabaseStatus InquirySparkStatus { get; set; } = new();
    public DateTime CheckedAt { get; set; }
}

/// <summary>
/// Database connection status details
/// </summary>
public class DatabaseStatus
{
    public string ProviderName { get; set; } = string.Empty;
    public string ConnectionString { get; set; } = string.Empty;
    public string DatabasePath { get; set; } = string.Empty;
    public bool FileExists { get; set; }
    public double FileSizeKB { get; set; }
    public DateTime? LastModified { get; set; }
    public bool IsReadOnly { get; set; }
    public bool CanConnect { get; set; }
    public bool IsReadOnlyConnection { get; set; }
    public string IntegrityCheck { get; set; } = string.Empty;
    public int TableCount { get; set; }
    public string Status { get; set; } = "Unknown";
    public string? ErrorMessage { get; set; }
}
