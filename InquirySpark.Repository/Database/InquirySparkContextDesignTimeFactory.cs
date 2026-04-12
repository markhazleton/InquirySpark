using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace InquirySpark.Repository.Database;

/// <summary>
/// Design-time factory used by EF Core tools (dotnet ef migrations add) to create
/// InquirySparkContext without a running host. Points to an ephemeral writable DB.
/// </summary>
public class InquirySparkContextDesignTimeFactory : IDesignTimeDbContextFactory<InquirySparkContext>
{
    /// <inheritdoc />
    public InquirySparkContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<InquirySparkContext>();
        // Use a local writable copy for design-time migrations tooling.
        // Never referenced at runtime — only by dotnet-ef CLI.
        var dbPath = Path.GetFullPath(
            Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "..",
                "data", "sqlite", "InquirySpark.db"));
        optionsBuilder.UseSqlite($"Data Source={dbPath};Mode=ReadWriteCreate");
        return new InquirySparkContext(optionsBuilder.Options);
    }
}
