namespace InquirySpark.Admin.Contracts.Responses;

/// <summary>
/// Overall application health response matching the system-health.openapi.yaml contract.
/// </summary>
public class SystemHealthResponse
{
    /// <summary>Aggregate health status: Healthy, Degraded, or Unhealthy.</summary>
    public string Status { get; set; } = "Unhealthy";

    /// <summary>Active persistence provider details.</summary>
    public ProviderInfo Provider { get; set; } = new();

    /// <summary>Semantic version string of the running host assembly.</summary>
    public string? BuildVersion { get; set; }

    /// <summary>Optional diagnostic messages produced during the health check.</summary>
    public List<string> Diagnostics { get; set; } = [];
}

/// <summary>Provider info sub-object embedded in <see cref="SystemHealthResponse"/>.</summary>
public class ProviderInfo
{
    /// <summary>Provider name (always "Sqlite" for this baseline).</summary>
    public string Name { get; set; } = "Sqlite";

    /// <summary>Sanitized data-source path (credentials redacted).</summary>
    public string ConnectionString { get; set; } = string.Empty;

    /// <summary>Indicates the provider is enforcing read-only mode.</summary>
    public bool ReadOnly { get; set; } = true;
}

/// <summary>
/// SQLite database file state response matching the system-health.openapi.yaml contract.
/// </summary>
public class DatabaseStateResponse
{
    /// <summary>Absolute path to the mounted <c>.db</c> asset.</summary>
    public string FilePath { get; set; } = string.Empty;

    /// <summary>UTC timestamp of the file's last write (must not change between calls).</summary>
    public DateTime LastWriteUtc { get; set; }

    /// <summary>SHA-256 hex digest used to detect tampering.</summary>
    public string Checksum { get; set; } = string.Empty;

    /// <summary>MUST be <see langword="false"/> for the immutable SQLite baseline.</summary>
    public bool Writable { get; set; }
}
