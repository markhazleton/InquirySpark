using System.Reflection;
using DecisionSpark.Core.Models.Configuration;
using DecisionSpark.Core.Persistence.FileStorage;
using DecisionSpark.Core.Persistence.Repositories;
using DecisionSpark.Core.Services;
using DecisionSpark.Core.Services.Validation;
using DecisionSpark.Health;
using DecisionSpark.Middleware;
using DecisionSpark.Swagger;
using FluentValidation;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.OpenApi;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog - using bootstrap logger pattern for better startup logging
var logDirectory = builder.Configuration["Serilog:LogDirectory"] ?? "logs";
var logPath = Path.Combine(logDirectory, "decisionspark-.txt");

// Ensure log directory exists
Directory.CreateDirectory(logDirectory);

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .WriteTo.Console(restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Error)
    .WriteTo.File(logPath, rollingInterval: RollingInterval.Day)
    .CreateLogger();

// Clear default providers to prevent duplication
builder.Logging.ClearProviders();
builder.Logging.AddSerilog(Log.Logger);

try
{
    Log.Information("Starting web application");

    builder.Host.UseSerilog();

    // Configure DecisionSpecs options
    builder.Services.AddOptions<DecisionSpecsOptions>()
        .BindConfiguration(DecisionSpecsOptions.SectionName)
        .ValidateOnStart();

    // Configure Application Insights telemetry (SC-003: LLM draft telemetry)
    var aiConnectionString = builder.Configuration["ApplicationInsights:ConnectionString"];
    if (!string.IsNullOrWhiteSpace(aiConnectionString))
    {
        builder.Services.AddApplicationInsightsTelemetry(options =>
        {
            options.ConnectionString = aiConnectionString;
        });
        Log.Information("Application Insights telemetry configured");
    }
    else
    {
        // Register a no-op TelemetryClient so consumers get a safe instance regardless
        builder.Services.AddApplicationInsightsTelemetry();
        Log.Information("Application Insights connection string not configured - telemetry disabled");
    }

    // Add services to the container - using single AddControllersWithViews call
    builder.Services.AddControllersWithViews()
        .AddJsonOptions(options =>
        {
            options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
        });

    // Add health checks for DecisionSpecs directory
    builder.Services.AddHealthChecks()
        .AddCheck<DecisionSpecsHealthCheck>("DecisionSpecs");

    // Add Swagger/OpenAPI with custom configuration
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(options =>
    {
        options.SwaggerDoc("v1", new OpenApiInfo
        {
            Title = "DecisionSpark API",
            Version = "v1",
            Description = "Dynamic Decision Routing Engine - Guides users through minimal questions to recommend optimal outcomes",
            Contact = new OpenApiContact
            {
                Name = "DecisionSpark",
                Url = new Uri("https://github.com/markhazleton/DecisionSpark")
            }
        });

        // Add API Key authentication to Swagger UI
        var apiKeyScheme = new OpenApiSecurityScheme
        {
            Type = SecuritySchemeType.ApiKey,
            Name = "X-API-KEY",
            In = ParameterLocation.Header,
            Description = "API Key required for authentication. Use: dev-api-key-change-in-production"
        };

        options.AddSecurityDefinition("ApiKey", apiKeyScheme);

        // Use fully qualified type names to avoid schema ID conflicts
        options.CustomSchemaIds(type => type.FullName?.Replace("+", "."));

        // Configure to handle Dictionary<string, object> and polymorphic types
        options.UseAllOfToExtendReferenceSchemas();
        options.UseOneOfForPolymorphism();
        options.UseAllOfForInheritance();

        // Handle Dictionary<string, object> as additionalProperties
        options.MapType<Dictionary<string, object>>(() => new OpenApiSchema
        {
            Type = JsonSchemaType.Object,
            AdditionalPropertiesAllowed = true,
            AdditionalProperties = new OpenApiSchema
            {
                Type = JsonSchemaType.Object
            }
        });

        // Add schema filter to handle List<object> and other complex types
        options.SchemaFilter<ObjectTypeSchemaFilter>();

        // Include XML comments
        var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
        var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFilename);
        if (File.Exists(xmlPath))
        {
            options.IncludeXmlComments(xmlPath);
        }
    });

    // Register Decision Engine services - using keyed services for better organization
    builder.Services.AddSingleton<ISessionStore, InMemorySessionStore>();
    builder.Services.AddSingleton<IDecisionSpecLoader, FileSystemDecisionSpecLoader>();
    builder.Services.AddSingleton<IOpenAIService, OpenAIService>();
    builder.Services.AddSingleton<IRoutingEvaluator, RoutingEvaluator>();
    builder.Services.AddSingleton<IResponseMapper, ResponseMapper>();
    builder.Services.AddSingleton<ITraitParser, TraitParser>();

    // Register new services for question type feature
    builder.Services.AddSingleton<IOptionIdGenerator, OptionIdGenerator>();
    builder.Services.AddSingleton<IQuestionPresentationDecider, QuestionPresentationDecider>();
    builder.Services.AddScoped<IUserSelectionService, UserSelectionService>();
    builder.Services.AddSingleton<IConversationPersistence, FileConversationPersistence>();

    // Register question generator - use OpenAI version if available, fallback to stub
    var useOpenAI = builder.Configuration.GetValue<bool>("OpenAI:EnableFallback", true);
    if (useOpenAI)
    {
        builder.Services.AddSingleton<IQuestionGenerator, OpenAIQuestionGenerator>();
        Log.Information("Configured OpenAI-powered question generator");
    }
    else
    {
        builder.Services.AddSingleton<IQuestionGenerator, StubQuestionGenerator>();
        Log.Information("Configured stub question generator");
    }

    // Register DecisionSpec file storage and repository services
    builder.Services.AddSingleton<DecisionSpecFileStore>();
    builder.Services.AddSingleton<FileSearchIndexer>();
    builder.Services.AddSingleton<IDecisionSpecRepository, DecisionSpecRepository>();
    builder.Services.AddScoped<TraitPatchService>();

    // Register DecisionSpec Draft Service (LLM-assisted spec generation)
    builder.Services.AddSingleton<DecisionSpecDraftService>(sp =>
    {
        var openAIService = sp.GetRequiredService<IOpenAIService>();
        var repository = sp.GetRequiredService<IDecisionSpecRepository>();
        var validator = sp.GetRequiredService<IValidator<DecisionSpark.Core.Models.Spec.DecisionSpecDocument>>();
        var logger = sp.GetRequiredService<ILogger<DecisionSpecDraftService>>();
        var options = builder.Configuration.GetSection(DecisionSpecsOptions.SectionName).Get<DecisionSpecsOptions>()
            ?? throw new InvalidOperationException("DecisionSpecs options are not configured.");

        var draftsPath = Path.Combine(options.RootPath, "drafts");

        return new DecisionSpecDraftService(openAIService, repository, validator, logger, draftsPath);
    });

    // Register FluentValidation validators
    builder.Services.AddValidatorsFromAssemblyContaining<DecisionSpecValidator>();
    builder.Services.AddValidatorsFromAssemblyContaining<DecisionSpark.Areas.Admin.ViewModels.DecisionSpecs.DecisionSpecEditViewModelValidator>();

    // Register index refresh background service
    builder.Services.AddHostedService<IndexRefreshHostedService>();

    var app = builder.Build();

    // Log OpenAI configuration status
    var openAIService = app.Services.GetRequiredService<IOpenAIService>();
    if (openAIService.IsAvailable())
    {
        Log.Information("OpenAI service is configured and available");
    }
    else
    {
        Log.Warning("OpenAI service is not configured - using fallback mode");
    }

    // Configure the HTTP request pipeline
    if (app.Environment.IsDevelopment())
    {
        app.UseDeveloperExceptionPage();
        app.UseSwagger();
        app.UseSwaggerUI(options =>
        {
            options.SwaggerEndpoint("/swagger/v1/swagger.json", "DecisionSpark API v1");
            options.RoutePrefix = "swagger";
            options.DocumentTitle = "DecisionSpark API";
            options.DisplayRequestDuration();
        });
    }
    else
    {
        app.UseExceptionHandler("/Home/Error");
        app.UseHsts();
    }

    app.UseHttpsRedirection();
    app.UseStaticFiles();

    // Validation error handling middleware
    app.UseValidationProblemDetails();

    app.UseRouting();

    // API Key Authentication - must be after UseRouting and before UseAuthorization
    app.UseApiKeyAuthentication();

    app.UseAuthorization();

    app.MapControllers();

    // Map Admin area route
    app.MapControllerRoute(
        name: "admin",
        pattern: "Admin/{controller=DecisionSpecs}/{action=Index}/{id?}",
        defaults: new { area = "Admin" });

    app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}");

    // Map health check endpoints
    app.MapHealthChecks("/health");

    Log.Information("DecisionSpark API starting on {Environment}", app.Environment.EnvironmentName);
    Log.Information("Swagger UI available at: /swagger");
    Log.Information("Health check available at: /health");

    await app.RunAsync();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
    throw;
}
finally
{
    await Log.CloseAndFlushAsync();
}
