using ControlSpark.WebMvc.Areas.Identity.Data;
using InquirySpark.Repository.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using InquirySpark.Admin.Data;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddUserSecrets<Program>();

// Configure logging with scopes for charting operations
builder.Logging.ClearProviders();
builder.Logging.AddConsole(options =>
{
    options.IncludeScopes = true;
});
builder.Logging.AddDebug();

// TODO: Configure OpenTelemetry when ready
// builder.Services.AddOpenTelemetry()
//     .WithTracing(tracerProviderBuilder =>
//     {
//         tracerProviderBuilder
//             .AddAspNetCoreInstrumentation()
//             .AddHttpClientInstrumentation()
//             .AddEntityFrameworkCoreInstrumentation()
//             .AddSource("InquirySpark.ChartBuilder")
//             .AddSource("InquirySpark.BatchJobs")
//             .AddSource("InquirySpark.DataExplorer")
//             .AddSource("InquirySpark.DeckExports")
//             .AddSource("InquirySpark.GaugeDashboards");
//     })
//     .WithMetrics(meterProviderBuilder =>
//     {
//         meterProviderBuilder
//             .AddAspNetCoreInstrumentation()
//             .AddHttpClientInstrumentation()
//             .AddMeter("InquirySpark.ChartBuilder")
//             .AddMeter("InquirySpark.Performance");
//     });

var connectionString = builder.Configuration.GetConnectionString("ControlSparkUserContextConnection")
    ?? throw new InvalidOperationException("Connection string 'ControlSparkUserContextConnection' not found.");

builder.Services.AddDbContext<InquirySpark.Admin.Areas.Identity.Data.ControlSparkUserContext>(options => options.UseSqlite(connectionString));

builder.Services.AddDefaultIdentity<ControlSparkUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<InquirySpark.Admin.Areas.Identity.Data.ControlSparkUserContext>();

// Configure authorization policies for Benchmark Insights roles
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("Analyst", policy => policy.RequireRole("Analyst", "Administrator"));
    options.AddPolicy("Operator", policy => policy.RequireRole("Operator", "Administrator"));
    options.AddPolicy("Consultant", policy => policy.RequireRole("Consultant", "Administrator"));
    options.AddPolicy("Executive", policy => policy.RequireRole("Executive", "Administrator"));
});

var inquirySparkContextConnectionString = builder.Configuration.GetConnectionString("InquirySparkConnection")
    ?? throw new InvalidOperationException("Connection string 'InquirySparkConnection' not found.");

builder.Services.AddDbContext<InquirySparkContext>(options =>
{
    options.UseSqlite(inquirySparkContextConnectionString);
});

// Register charting services
builder.Services.AddScoped<InquirySpark.Repository.Services.Charting.IChartDefinitionService, InquirySpark.Repository.Services.Charting.ChartDefinitionService>();
builder.Services.AddScoped<InquirySpark.Repository.Services.Charting.IChartBuildService, InquirySpark.Repository.Services.Charting.ChartBuildService>();
builder.Services.AddScoped<InquirySpark.Repository.Services.Charting.IFormulaParserService, InquirySpark.Repository.Services.Charting.FormulaParserService>();
builder.Services.AddScoped<InquirySpark.Repository.Services.Charting.IChartValidationService, InquirySpark.Repository.Services.Charting.ChartValidationService>();
builder.Services.AddScoped<InquirySpark.Repository.Services.Security.IAuditLogService, InquirySpark.Repository.Services.Security.AuditLogService>();
builder.Services.AddScoped<InquirySpark.Repository.Services.UserPreferences.IUserPreferenceService, InquirySpark.Repository.Services.UserPreferences.UserPreferenceService>();

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();
builder.Services.AddHttpContextAccessor();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromSeconds(360);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddMvc();

var app = builder.Build();

// Initialize roles and admin user
using (var scope = app.Services.CreateScope())
{
    await SeedRoles.InitializeAsync(scope.ServiceProvider);
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}
app.UseHttpsRedirection();

// Static files serve local assets from wwwroot
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();

app.UseAuthorization();

app.MapRazorPages();
app.MapControllerRoute(
    name: "MyArea",
    pattern: "{area:exists}/{controller=Main}/{action=Index}/{id?}");
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
