using ControlSpark.WebMvc.Areas.Identity.Data;
using InquirySpark.Repository.Database;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddUserSecrets<Program>();

var connectionString = builder.Configuration.GetConnectionString("ControlSparkUserContextConnection")
    ?? throw new InvalidOperationException("Connection string 'ControlSparkUserContextConnection' not found.");

builder.Services.AddDbContext<ControlSparkUserContext>(options => options.UseSqlite(connectionString));
builder.Services.AddQuickGridEntityFrameworkAdapter();

builder.Services.AddDefaultIdentity<ControlSparkUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ControlSparkUserContext>();


var inquirySparkContextConnectionString = builder.Configuration.GetConnectionString("InquirySparkConnection")
    ?? throw new InvalidOperationException("Connection string 'InquirySparkConnection' not found.");

builder.Services.AddDbContext<InquirySparkContext>(options =>
{
    options.UseSqlite(inquirySparkContextConnectionString);
});

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromSeconds(360);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddMvc();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}
app.UseHttpsRedirection();
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
