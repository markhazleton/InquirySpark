using ControlSpark.WebMvc.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using static InquirySpark.Admin.Areas.Identity.Models.WebsiteModel;

namespace InquirySpark.Admin.Areas.Identity.Data;

public class ControlSparkUserContext(DbContextOptions<Data.ControlSparkUserContext> options) : IdentityDbContext<ControlSparkUser>(options)
{
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        // Customize the ASP.NET Identity model and override the defaults if needed.
        // For example, you can rename the ASP.NET Identity table names and more.
        // Add your customizations after calling base.OnModelCreating(builder);
    }

    public DbSet<WebsiteEditModel> WebsiteEditModel { get; set; } = default!;
}
