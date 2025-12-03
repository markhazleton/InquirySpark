using InquirySpark.Common.SDK.Services;
using InquirySpark.Repository.Database;
using InquirySpark.Repository.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi;

var builder = WebApplication.CreateBuilder(args);


// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddLogging();
builder.Services.AddDbContext<InquirySparkContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("InquirySparkContext")));

builder.Services.AddTransient<ISurveyService, SurveyService>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "InquirySpark API",
        Version = "v1"
    });

    var xmlPath = Path.Combine(AppContext.BaseDirectory, "InquirySpark.Common.xml");
    c.IncludeXmlComments(xmlPath);
    xmlPath = Path.Combine(AppContext.BaseDirectory, "InquirySpark.Repository.xml");
    c.IncludeXmlComments(xmlPath);
    xmlPath = Path.Combine(AppContext.BaseDirectory, "InquirySpark.WebApi.xml");
    c.IncludeXmlComments(xmlPath);
    c.EnableAnnotations();
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "InquirySpark API");
    });
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
