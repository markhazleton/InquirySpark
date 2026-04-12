// ============================================================
// InquirySpark.Web — Unified Entry Point
// ============================================================
// DI Registration ownership (per tasks.md):
//   T016  — Data contexts + IDecisionSparkFileStorageService
//   T016A — Identity/auth builder registrations (AddDefaultIdentity, roles, EF stores)
//   T016B — Authorization policies + Identity UI endpoint mapping
// ============================================================

using ControlSpark.WebMvc.Areas.Identity.Data;
using InquirySpark.Admin.Areas.Identity.Data;
using InquirySpark.Common.Models.Configuration;
using InquirySpark.Common.Persistence.FileStorage;
using InquirySpark.Common.Persistence.Repositories;
using InquirySpark.Common.Services;
using InquirySpark.Repository.Database;
using InquirySpark.Repository.Services.UnifiedWeb;
using InquirySpark.Web.Configuration.Unified;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using FluentValidation;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddUserSecrets<Program>();

// Logging
builder.Logging.ClearProviders();
builder.Logging.AddSimpleConsole(options =>
{
    options.IncludeScopes = true;
});
builder.Logging.AddDebug();

// MVC + Razor Pages (Identity UI requires Razor Pages)
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();
builder.Services.AddHttpContextAccessor();

// Session support (required for Decision Conversation in-memory state)
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// ── T016: Data contexts + file-storage DI registrations ────────────────────

// Canonical Identity store (ControlSparkUserContextConnection — Identity SQLite, ReadWriteCreate in dev)
var identityConnectionString = builder.Configuration.GetConnectionString("ControlSparkUserContextConnection")
    ?? throw new InvalidOperationException("Connection string 'ControlSparkUserContextConnection' not found.");
builder.Services.AddDbContext<ControlSparkUserContext>(options => options.UseSqlite(identityConnectionString));

// InquirySpark data context (read-only SQLite)
var inquiryConnectionString = builder.Configuration.GetConnectionString("InquirySparkConnection")
    ?? throw new InvalidOperationException("Connection string 'InquirySparkConnection' not found.");
builder.Services.AddDbContext<InquirySparkContext>(options => options.UseSqlite(inquiryConnectionString));

// DecisionSpec file-storage pipeline (from InquirySpark.Common)
builder.Services.AddOptions<DecisionSpecsOptions>()
    .BindConfiguration(DecisionSpecsOptions.SectionName)
    .ValidateOnStart();
builder.Services.AddSingleton<DecisionSpecFileStore>();
builder.Services.AddSingleton<DecisionSpecMetadataStore>();
builder.Services.AddSingleton<FileSearchIndexer>();
builder.Services.AddSingleton<IDecisionSpecRepository, DecisionSpecRepository>();
builder.Services.AddHostedService<IndexRefreshHostedService>();

// Conversation persistence (singleton — file-based and config/logger-only dependencies)
builder.Services.AddSingleton<IConversationPersistence, FileConversationPersistence>();

// IDecisionSparkFileStorageService — thin facade over the above (T004C/T004D)
builder.Services.AddSingleton<IDecisionSparkFileStorageService, DecisionSparkFileStorageService>();

// FluentValidation (for DecisionSpec validation used by file storage service)
builder.Services.AddValidatorsFromAssemblyContaining<InquirySpark.Common.Services.Validation.DecisionSpecValidator>();

// ── T016A: Identity/auth builder registrations ──────────────────────────────

// Reuse InquirySpark.Admin authentication/sign-in pattern (per FR-015)
builder.Services.AddDefaultIdentity<ControlSparkUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ControlSparkUserContext>();

// ── UnifiedWeb options (capability completion + identity bridge) ────────────

builder.Services.AddOptions<UnifiedWebOptions>()
    .BindConfiguration("UnifiedWeb")
    .ValidateOnStart();

builder.Services.AddOptions<IdentityBridgeOptions>()
    .BindConfiguration("IdentityBridge")
    .ValidateOnStart();

builder.Services.AddOptions<ClientUiOptions>()
    .BindConfiguration("ClientUi");

// Capability governance and identity bridge services
builder.Services.AddSingleton<IUnifiedAuditService, UnifiedAuditService>();
builder.Services.AddSingleton<IUnifiedWebCapabilityService, UnifiedWebCapabilityService>();
builder.Services.AddSingleton<IIdentityMigrationBridgeService, IdentityMigrationBridgeService>();

// Unified navigation
builder.Services.AddSingleton<InquirySpark.Web.Services.Navigation.UnifiedNavigationBuilder>();

// Operations Support services (Charting, Audit, User Preferences)
builder.Services.AddScoped<InquirySpark.Repository.Services.Charting.IChartDefinitionService, InquirySpark.Repository.Services.Charting.ChartDefinitionService>();
builder.Services.AddScoped<InquirySpark.Repository.Services.Charting.IChartBuildService, InquirySpark.Repository.Services.Charting.ChartBuildService>();
builder.Services.AddScoped<InquirySpark.Repository.Services.Charting.IFormulaParserService, InquirySpark.Repository.Services.Charting.FormulaParserService>();
builder.Services.AddScoped<InquirySpark.Repository.Services.Charting.IChartValidationService, InquirySpark.Repository.Services.Charting.ChartValidationService>();
builder.Services.AddScoped<InquirySpark.Repository.Services.Security.IAuditLogService, InquirySpark.Repository.Services.Security.AuditLogService>();
builder.Services.AddScoped<InquirySpark.Repository.Services.UserPreferences.IUserPreferenceService, InquirySpark.Repository.Services.UserPreferences.UserPreferenceService>();

// ── T016B: Authorization policies + Identity UI endpoint mapping ────────────

builder.Services.AddAuthorization(options =>
{
    // Mirror InquirySpark.Admin authorization policies per FR-004
    options.AddPolicy("Analyst", policy => policy.RequireRole("Analyst", "Administrator"));
    options.AddPolicy("Operator", policy => policy.RequireRole("Operator", "Administrator"));
    options.AddPolicy("Consultant", policy => policy.RequireRole("Consultant", "Administrator"));
    options.AddPolicy("Executive", policy => policy.RequireRole("Executive", "Administrator"));
});

// ─────────────────────────────────────────────────────────────────────────────

var app = builder.Build();

// HTTP pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Unified/Operations/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseSession();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

// Razor Pages (Identity UI endpoints: /Identity/Account/Login, etc.)
app.MapRazorPages();

// Unified area route — all capability surfaces live under /Unified/
app.MapControllerRoute(
    name: "unified",
    pattern: "Unified/{controller=Operations}/{action=Index}/{id?}",
    defaults: new { area = "Unified" });

app.MapGet("/", () => Results.Redirect("/Unified/Operations"));

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

await app.RunAsync();

/// <summary>Exposes Program for integration test projects using WebApplicationFactory.</summary>
public partial class Program { }
