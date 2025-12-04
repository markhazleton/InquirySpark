using System;
using System.IO;
using System.Linq;
using InquirySpark.Common.Models;
using InquirySpark.Repository.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Sqlite.Infrastructure.Internal;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace InquirySpark.Common.Tests.Providers;

[TestClass]
public class SqliteProviderTests
{
    [TestMethod]
    public void Configure_RegistersSqliteExtension()
    {
        using var tempDb = TempDbFile.Create();
        var builder = new DbContextOptionsBuilder<InquirySparkContext>();

        InquirySparkContext.Configure(builder, BuildConfig(tempDb.Path));

        var extension = builder.Options.Extensions.OfType<SqliteOptionsExtension>().SingleOrDefault();
        Assert.IsNotNull(extension, "SqliteOptionsExtension should be registered on the options builder.");
    }

    [TestMethod]
    public void Configure_SetsReadOnlyModeOnConnectionString()
    {
        using var tempDb = TempDbFile.Create();
        var builder = new DbContextOptionsBuilder<InquirySparkContext>();

        InquirySparkContext.Configure(builder, BuildConfig(tempDb.Path));

        var extension = builder.Options.Extensions.OfType<SqliteOptionsExtension>().Single();
        StringAssert.Contains(extension.ConnectionString, "Mode=ReadOnly", "Configurator must enforce Mode=ReadOnly to protect immutable assets.");
    }

    private static PersistenceProviderConfig BuildConfig(string path)
    {
        return new PersistenceProviderConfig
        {
            ConnectionString = $"Data Source={path}",
            ReadOnly = true,
            CommandTimeoutSeconds = 15
        };
    }

    private sealed class TempDbFile : IDisposable
    {
        private TempDbFile(string path)
        {
            Path = path;
        }

        public string Path { get; }

        public static TempDbFile Create()
        {
            var filePath = System.IO.Path.GetTempFileName();
            // touch the file to satisfy validation (no schema required for configuration tests)
            File.WriteAllBytes(filePath, []);
            return new TempDbFile(filePath);
        }

        public void Dispose()
        {
            try
            {
                if (File.Exists(Path))
                {
                    File.Delete(Path);
                }
            }
            catch
            {
                // ignore cleanup errors; temp files will be purged by OS
            }
        }
    }
}
