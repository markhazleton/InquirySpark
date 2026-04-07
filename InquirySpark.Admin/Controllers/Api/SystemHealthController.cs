using System.Reflection;
using System.Security.Cryptography;
using InquirySpark.Admin.Contracts.Responses;
using InquirySpark.Repository.Database;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace InquirySpark.Admin.Controllers.Api;

/// <summary>
/// Read-only system health endpoints confirming SQLite provider state after SQL Server removal.
/// </summary>
[ApiController]
[Route("api/system")]
[AllowAnonymous]
public class SystemHealthController(
    InquirySparkContext inquirySparkContext,
    ILogger<SystemHealthController> logger) : ControllerBase
{
    private readonly InquirySparkContext _context = inquirySparkContext;
    private readonly ILogger<SystemHealthController> _logger = logger;

    /// <summary>
    /// Returns overall application health including persistence provider details.
    /// </summary>
    /// <returns>200 with <see cref="SystemHealthResponse"/>, or 503 when misconfigured.</returns>
    [HttpGet("health")]
    [ProducesResponseType(typeof(SystemHealthResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status503ServiceUnavailable)]
    public IActionResult GetHealth()
    {
        var diagnostics = new List<string>();
        var response = new SystemHealthResponse
        {
            BuildVersion = Assembly.GetEntryAssembly()?.GetName().Version?.ToString()
        };

        try
        {
            var connection = _context.Database.GetDbConnection();
            var rawCs = connection.ConnectionString ?? string.Empty;
            var isReadOnly = rawCs.Contains("Mode=ReadOnly", StringComparison.OrdinalIgnoreCase);

            response.Provider = new ProviderInfo
            {
                Name = "Sqlite",
                ConnectionString = SanitizeConnectionString(rawCs),
                ReadOnly = isReadOnly
            };

            if (!isReadOnly)
            {
                diagnostics.Add("WARNING: Connection string does not contain Mode=ReadOnly.");
            }

            var canConnect = _context.Database.CanConnect();
            if (canConnect)
            {
                response.Status = "Healthy";
                diagnostics.Add("Database connection succeeded.");
            }
            else
            {
                response.Status = "Unhealthy";
                diagnostics.Add("ERROR: Cannot connect to SQLite database.");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Health check failed");
            response.Status = "Unhealthy";
            diagnostics.Add($"Exception: {ex.Message}");
        }

        response.Diagnostics = diagnostics;

        return response.Status == "Unhealthy"
            ? StatusCode(StatusCodes.Status503ServiceUnavailable, response)
            : Ok(response);
    }

    /// <summary>
    /// Inspects the active SQLite database file, returning metadata proving read-only mount.
    /// </summary>
    /// <returns>200 with <see cref="DatabaseStateResponse"/>, or 409 when write activity detected.</returns>
    [HttpGet("database/state")]
    [ProducesResponseType(typeof(DatabaseStateResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status503ServiceUnavailable)]
    public IActionResult GetDatabaseState()
    {
        try
        {
            var connection = _context.Database.GetDbConnection();
            var rawCs = connection.ConnectionString ?? string.Empty;
            var dataSource = ExtractDataSource(rawCs);

            if (string.IsNullOrEmpty(dataSource) || !System.IO.File.Exists(dataSource))
            {
                return StatusCode(StatusCodes.Status503ServiceUnavailable,
                    new { error = "SQLite database file not found.", path = dataSource });
            }

            var fileInfo = new FileInfo(dataSource);
            var isWritable = !rawCs.Contains("Mode=ReadOnly", StringComparison.OrdinalIgnoreCase);

            if (isWritable)
            {
                _logger.LogWarning("Database state check: connection is not read-only. DataSource={DataSource}", dataSource);
                return Conflict(new { error = "Connection is not in Mode=ReadOnly — write access detected.", filePath = dataSource });
            }

            var checksum = ComputeSha256(dataSource);

            var state = new DatabaseStateResponse
            {
                FilePath = fileInfo.FullName,
                LastWriteUtc = fileInfo.LastWriteTimeUtc,
                Checksum = checksum,
                Writable = false
            };

            return Ok(state);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Database state check failed");
            return StatusCode(StatusCodes.Status503ServiceUnavailable, new { error = ex.Message });
        }
    }

    // ── helpers ──────────────────────────────────────────────────────────────

    private static string SanitizeConnectionString(string rawCs)
    {
        try
        {
            var builder = new SqliteConnectionStringBuilder(rawCs);
            var fileName = Path.GetFileName(builder.DataSource);
            return $"Data Source=.../{fileName};Mode={builder.Mode}";
        }
        catch
        {
            return "Data Source=(redacted)";
        }
    }

    private static string ExtractDataSource(string connectionString)
    {
        try
        {
            return new SqliteConnectionStringBuilder(connectionString).DataSource;
        }
        catch
        {
            return string.Empty;
        }
    }

    private static string ComputeSha256(string filePath)
    {
        using var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
        var hash = SHA256.HashData(stream);
        return Convert.ToHexString(hash).ToLowerInvariant();
    }
}
