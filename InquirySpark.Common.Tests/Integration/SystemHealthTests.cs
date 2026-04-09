#nullable enable
using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace InquirySpark.Common.Tests.Integration;

/// <summary>
/// Integration tests for the system health API endpoints in InquirySpark.Admin.
/// Uses <see cref="WebApplicationFactory{TEntryPoint}"/> to host the Admin application
/// in-process and verifies that the SQLite provider is correctly reported.
/// </summary>
[TestClass]
public class SystemHealthTests
{
    private static WebApplicationFactory<Program>? _factory;
    private static HttpClient? _client;

    [ClassInitialize]
    public static void ClassInit(TestContext _)
    {
        // Locate the immutable SQLite assets relative to the test output directory.
        var repoRoot = FindRepoRoot(AppContext.BaseDirectory);
        var inquirySpark = Path.Combine(repoRoot, "data", "sqlite", "InquirySpark.db");
        var userDb = Path.Combine(repoRoot, "data", "sqlite", "ControlSparkUser.db");

        // Environment variables override appsettings.json in ASP.NET Core's default configuration.
        // This is the most reliable way to inject connection strings into a WebApplicationFactory host.
        Environment.SetEnvironmentVariable("ConnectionStrings__InquirySparkConnection",
            $"Data Source={inquirySpark};Mode=ReadOnly");
        // Identity DB needs write access so SeedRoles can create roles on first run.
        Environment.SetEnvironmentVariable("ConnectionStrings__ControlSparkUserContextConnection",
            $"Data Source={userDb};Mode=ReadWriteCreate");

        _factory = new WebApplicationFactory<Program>();

        _client = _factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false
        });
    }

    [ClassCleanup]
    public static void ClassCleanup()
    {
        _client?.Dispose();
        _factory?.Dispose();
        Environment.SetEnvironmentVariable("ConnectionStrings__InquirySparkConnection", null);
        Environment.SetEnvironmentVariable("ConnectionStrings__ControlSparkUserContextConnection", null);
    }

    // ── GET /api/system/health ────────────────────────────────────────────────

    [TestMethod]
    public async Task GetHealth_Returns200()
    {
        var response = await _client!.GetAsync("/api/system/health");
        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode,
            "Health endpoint must return 200 when SQLite provider is healthy.");
    }

    [TestMethod]
    public async Task GetHealth_StatusIsHealthy()
    {
        var response = await _client!.GetAsync("/api/system/health");
        response.EnsureSuccessStatusCode();

        var json = await ParseJsonAsync(response);
        var status = json.RootElement.GetProperty("status").GetString();
        Assert.AreEqual("Healthy", status, "Status must be 'Healthy' when SQLite file is reachable.");
    }

    [TestMethod]
    public async Task GetHealth_ProviderNameIsSqlite()
    {
        var response = await _client!.GetAsync("/api/system/health");
        response.EnsureSuccessStatusCode();

        var json = await ParseJsonAsync(response);
        var providerName = json.RootElement
            .GetProperty("provider")
            .GetProperty("name")
            .GetString();

        Assert.AreEqual("Sqlite", providerName, "Provider name must be 'Sqlite' for the SQLite baseline.");
    }

    [TestMethod]
    public async Task GetHealth_ProviderIsReadOnly()
    {
        var response = await _client!.GetAsync("/api/system/health");
        response.EnsureSuccessStatusCode();

        var json = await ParseJsonAsync(response);
        var readOnly = json.RootElement
            .GetProperty("provider")
            .GetProperty("readOnly")
            .GetBoolean();

        Assert.IsTrue(readOnly, "Provider.readOnly must be true — immutable SQLite assets must not allow writes.");
    }

    // ── GET /api/system/database/state ────────────────────────────────────────

    [TestMethod]
    public async Task GetDatabaseState_Returns200()
    {
        var response = await _client!.GetAsync("/api/system/database/state");
        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode,
            "Database state endpoint must return 200 when the SQLite file exists in read-only mode.");
    }

    [TestMethod]
    public async Task GetDatabaseState_WritableIsFalse()
    {
        var response = await _client!.GetAsync("/api/system/database/state");
        response.EnsureSuccessStatusCode();

        var json = await ParseJsonAsync(response);
        var writable = json.RootElement.GetProperty("writable").GetBoolean();

        Assert.IsFalse(writable, "DatabaseStateResponse.writable MUST be false for the immutable SQLite baseline.");
    }

    [TestMethod]
    public async Task GetDatabaseState_ChecksumIsNonEmpty()
    {
        var response = await _client!.GetAsync("/api/system/database/state");
        response.EnsureSuccessStatusCode();

        var json = await ParseJsonAsync(response);
        var checksum = json.RootElement.GetProperty("checksum").GetString();

        Assert.IsFalse(string.IsNullOrWhiteSpace(checksum),
            "Checksum must be a non-empty SHA-256 hex digest.");
    }

    [TestMethod]
    public async Task GetDatabaseState_ChecksumIsDeterministic()
    {
        var r1 = await _client!.GetAsync("/api/system/database/state");
        var r2 = await _client.GetAsync("/api/system/database/state");

        r1.EnsureSuccessStatusCode();
        r2.EnsureSuccessStatusCode();

        var j1 = await ParseJsonAsync(r1);
        var j2 = await ParseJsonAsync(r2);

        var c1 = j1.RootElement.GetProperty("checksum").GetString();
        var c2 = j2.RootElement.GetProperty("checksum").GetString();

        Assert.AreEqual(c1, c2, "Checksum of an immutable file must be identical across successive calls.");
    }

    // ── helpers ───────────────────────────────────────────────────────────────

    private static async Task<JsonDocument> ParseJsonAsync(HttpResponseMessage response)
    {
        var body = await response.Content.ReadAsStringAsync();
        return JsonDocument.Parse(body);
    }

    /// <summary>Walks up from <paramref name="start"/> until a directory containing InquirySpark.sln is found.</summary>
    private static string FindRepoRoot(string start)
    {
        var dir = new DirectoryInfo(start);
        while (dir != null)
        {
            if (File.Exists(Path.Combine(dir.FullName, "InquirySpark.sln")))
                return dir.FullName;
            dir = dir.Parent;
        }
        throw new InvalidOperationException($"Could not locate repository root from '{start}'.");
    }
}
