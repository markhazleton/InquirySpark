using InquirySpark.Admin.Models;
using InquirySpark.Repository.Database;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace InquirySpark.Admin.Controllers;

/// <summary>
/// System health diagnostics controller for SQLite provider validation
/// </summary>
public class SystemHealthController(
    InquirySparkContext inquirySparkContext,
    ILogger<SystemHealthController> logger) : BaseController(logger)
{
    private readonly InquirySparkContext _inquirySparkContext = inquirySparkContext;

    /// <summary>
    /// Display system health dashboard
    /// </summary>
    public IActionResult Index()
    {
        var model = new SystemHealthViewModel
        {
            InquirySparkStatus = GetDatabaseStatus(_inquirySparkContext),
            CheckedAt = DateTime.UtcNow
        };

        return View(model);
    }

    /// <summary>
    /// Get database connection status details
    /// </summary>
    private DatabaseStatus GetDatabaseStatus(InquirySparkContext context)
    {
        var status = new DatabaseStatus
        {
            ProviderName = "Microsoft.EntityFrameworkCore.Sqlite"
        };

        try
        {
            // Get connection string from DbContext
            var connection = context.Database.GetDbConnection();
            var connectionString = connection.ConnectionString ?? string.Empty;
            status.ConnectionString = RedactConnectionString(connectionString);

            // Check if database file exists
            var dataSource = ExtractDataSource(connectionString);
            if (!string.IsNullOrEmpty(dataSource))
            {
                status.DatabasePath = dataSource;
                status.FileExists = System.IO.File.Exists(dataSource);

                if (status.FileExists)
                {
                    var fileInfo = new FileInfo(dataSource);
                    status.FileSizeKB = fileInfo.Length / 1024.0;
                    status.LastModified = fileInfo.LastWriteTimeUtc;
                    status.IsReadOnly = fileInfo.IsReadOnly;
                }
            }

            // Test connection
            var canConnect = context.Database.CanConnect();
            status.CanConnect = canConnect;

            if (canConnect)
            {
                // Verify read-only mode
                status.IsReadOnlyConnection = connectionString.Contains("Mode=ReadOnly", StringComparison.OrdinalIgnoreCase);

                // Get database metadata
                using var sqliteConnection = new SqliteConnection(connectionString);
                sqliteConnection.Open();

                // Check pragma integrity
                using var integrityCmd = sqliteConnection.CreateCommand();
                integrityCmd.CommandText = "PRAGMA integrity_check;";
                var integrityResult = integrityCmd.ExecuteScalar()?.ToString();
                status.IntegrityCheck = integrityResult == "ok" ? "PASS" : integrityResult ?? "UNKNOWN";

                // Get table count
                using var tableCountCmd = sqliteConnection.CreateCommand();
                tableCountCmd.CommandText = "SELECT COUNT(*) FROM sqlite_master WHERE type='table';";
                status.TableCount = Convert.ToInt32(tableCountCmd.ExecuteScalar());

                status.Status = "Healthy";
            }
            else
            {
                status.Status = "Cannot Connect";
            }
        }
        catch (Exception ex)
        {
            status.Status = "Error";
            status.ErrorMessage = ex.Message;
            _logger.LogError(ex, "System health check failed for {Context}", context.GetType().Name);
        }

        return status;
    }

    /// <summary>
    /// Extract Data Source from SQLite connection string
    /// </summary>
    private static string ExtractDataSource(string connectionString)
    {
        var builder = new SqliteConnectionStringBuilder(connectionString);
        return builder.DataSource;
    }

    /// <summary>
    /// Redact sensitive connection string details
    /// </summary>
    private static string RedactConnectionString(string connectionString)
    {
        if (string.IsNullOrEmpty(connectionString))
            return string.Empty;

        var builder = new SqliteConnectionStringBuilder(connectionString);
        var dataSource = builder.DataSource;

        // Show only filename, not full path
        if (!string.IsNullOrEmpty(dataSource))
        {
            var fileName = Path.GetFileName(dataSource);
            return $"Data Source=.../{fileName};Mode={builder.Mode}";
        }

        return "Data Source=(redacted)";
    }
}
