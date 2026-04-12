using ControlSpark.WebMvc.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace InquirySpark.Admin.Areas.Identity.Data;

/// <summary>
/// EF Core identity DbContext for InquirySpark.Web.
/// Uses the canonical ControlSparkUserContextConnection (same SQLite DB as InquirySpark.Admin)
/// to ensure single-sign-on session continuity per FR-015.
/// </summary>
public class ControlSparkUserContext(DbContextOptions<ControlSparkUserContext> options)
    : IdentityDbContext<ControlSparkUser>(options)
{
    /// <inheritdoc/>
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
    }
}
