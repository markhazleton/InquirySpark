using DecisionSpark.Core.Models.Configuration;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace DecisionSpark.Health;

public sealed class DecisionSpecsHealthCheck(IConfiguration configuration) : IHealthCheck
{
    private readonly DecisionSpecsOptions _options = configuration.GetSection(DecisionSpecsOptions.SectionName).Get<DecisionSpecsOptions>()
        ?? throw new InvalidOperationException("DecisionSpecs options are not configured.");

    public Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        var rootPath = _options.RootPath;

        if (!Directory.Exists(rootPath))
        {
            return Task.FromResult(HealthCheckResult.Unhealthy($"DecisionSpecs directory does not exist: {rootPath}"));
        }

        try
        {
            var testFile = Path.Combine(rootPath, $".health-check-{Guid.NewGuid()}.tmp");
            File.WriteAllText(testFile, "health check");
            File.Delete(testFile);
            return Task.FromResult(HealthCheckResult.Healthy("DecisionSpecs directory is writable"));
        }
        catch (Exception ex)
        {
            return Task.FromResult(HealthCheckResult.Unhealthy($"DecisionSpecs directory is not writable: {ex.Message}"));
        }
    }
}
