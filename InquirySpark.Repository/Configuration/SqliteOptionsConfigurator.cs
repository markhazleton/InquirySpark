using System;
using System.IO;
using InquirySpark.Common.Models;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Logging;

namespace InquirySpark.Repository.Configuration;

/// <summary>
/// Centralizes EF Core option configuration for the immutable SQLite provider.
/// </summary>
public static class SqliteOptionsConfigurator
{
    private const int DefaultCommandTimeout = 30;

    /// <summary>
    /// Applies the shared SQLite configuration to the supplied options builder.
    /// </summary>
    /// <param name="optionsBuilder">Builder to configure.</param>
    /// <param name="providerConfig">Provider configuration coming from appsettings/environment variables.</param>
    /// <param name="loggerFactory">Optional logger factory for EF diagnostics.</param>
    /// <returns>The original builder for chaining.</returns>
    public static DbContextOptionsBuilder Configure(
        DbContextOptionsBuilder optionsBuilder,
        PersistenceProviderConfig providerConfig,
        ILoggerFactory? loggerFactory = null)
    {
        ArgumentNullException.ThrowIfNull(optionsBuilder);
        ArgumentNullException.ThrowIfNull(providerConfig);

        var normalizedConfig = Normalize(providerConfig);
        PersistenceProviderConfigValidator.Validate(normalizedConfig, File.Exists);

        optionsBuilder.UseSqlite(normalizedConfig.ConnectionString, sqliteOptions =>
        {
            sqliteOptions.CommandTimeout(normalizedConfig.CommandTimeoutSeconds ?? DefaultCommandTimeout);
            sqliteOptions.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
        });

        if (!normalizedConfig.ReadOnly)
        {
            // The validator prevents this path, but keep a guard in case future callers bypass validation.
            throw new InvalidOperationException("SQLite provider must run in read-only mode.");
        }

        optionsBuilder.EnableDetailedErrors();
        optionsBuilder.EnableSensitiveDataLogging(false);
        optionsBuilder.ConfigureWarnings(warnings =>
        {
            warnings.Throw(RelationalEventId.PendingModelChangesWarning);
        });

        if (loggerFactory is not null)
        {
            optionsBuilder.UseLoggerFactory(loggerFactory);
        }

        return optionsBuilder;
    }

    private static PersistenceProviderConfig Normalize(PersistenceProviderConfig providerConfig)
    {
        var builder = new SqliteConnectionStringBuilder(providerConfig.ConnectionString)
        {
            Cache = SqliteCacheMode.Shared,
            Mode = SqliteOpenMode.ReadOnly
        };

        var resolvedPath = ResolveAbsolutePath(builder.DataSource);

        return providerConfig with
        {
            ConnectionString = builder.ConnectionString,
            DataFilePath = providerConfig.DataFilePath ?? resolvedPath
        };
    }

    private static string ResolveAbsolutePath(string dataSource)
    {
        if (string.IsNullOrWhiteSpace(dataSource))
        {
            throw new InvalidOperationException("SQLite connection string is missing a Data Source path.");
        }

        return Path.IsPathRooted(dataSource)
            ? dataSource
            : Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, dataSource));
    }
}
