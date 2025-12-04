using System;
using System.IO;

namespace InquirySpark.Common.Models;

/// <summary>
/// Describes the immutable persistence provider settings shared across every InquirySpark host.
/// </summary>
public sealed record PersistenceProviderConfig
{
    /// <summary>
    /// Logical provider name. Defaults to Sqlite for this feature baseline.
    /// </summary>
    public string ProviderName { get; init; } = "Sqlite";

    /// <summary>
    /// Provider-specific connection string (e.g., Data Source path with read-only flags).
    /// </summary>
    public required string ConnectionString { get; init; }
        = string.Empty;

    /// <summary>
    /// Indicates whether the data store must remain immutable.
    /// </summary>
    public bool ReadOnly { get; init; } = true;

    /// <summary>
    /// Optional timeout for long-running operations.
    /// </summary>
    public int? CommandTimeoutSeconds { get; init; }
        = null;

    /// <summary>
    /// Optional migration assembly metadata retained for documentation.
    /// </summary>
    public string? MigrationAssembly { get; init; }
        = null;

    /// <summary>
    /// Normalized file path used for health checks and diagnostics.
    /// </summary>
    public string? DataFilePath { get; init; }
        = null;
}

/// <summary>
/// Helper routines that ensure <see cref="PersistenceProviderConfig"/> instances are valid before use.
/// </summary>
public static class PersistenceProviderConfigValidator
{
    private const string SqliteProviderName = "Sqlite";

    /// <summary>
    /// Validates the supplied configuration, throwing <see cref="InvalidOperationException"/> when invalid.
    /// </summary>
    /// <param name="config">The configuration to validate.</param>
    /// <param name="fileExists">Optional delegate used to verify file existence for testing.</param>
    public static void Validate(PersistenceProviderConfig config, Func<string, bool>? fileExists = null)
    {
        ArgumentNullException.ThrowIfNull(config);

        if (string.IsNullOrWhiteSpace(config.ConnectionString))
        {
            throw new InvalidOperationException("Persistence provider connection string cannot be empty.");
        }

        if (!string.Equals(config.ProviderName, SqliteProviderName, StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException($"Unsupported provider '{config.ProviderName}'. Only '{SqliteProviderName}' is allowed in this baseline.");
        }

        if (!config.ReadOnly)
        {
            throw new InvalidOperationException("Persistence provider must run in read-only mode for immutable SQLite assets.");
        }

        if (config.CommandTimeoutSeconds is < 0)
        {
            throw new InvalidOperationException("Command timeout must be positive when specified.");
        }

        if (!string.IsNullOrWhiteSpace(config.DataFilePath))
        {
            var exists = (fileExists ?? File.Exists).Invoke(config.DataFilePath);
            if (!exists)
            {
                throw new InvalidOperationException($"SQLite data file was not found at '{config.DataFilePath}'.");
            }
        }
    }
}
