using AppRegistry.Database;
using AppRegistryService.Configuration;
using AppRegistryService.Contract.Models;
using AppRegistryService.Contract.Requests;
using AppRegistryService.Contract.Responses;
using AppRegistryService.Contracts;
using AppRegistryService.EndpointDefinitions;
using AppRegistryService.Metrics;
using AppRegistryService.Middlewares;
using AppRegistryService.Services;
using AspNetCoreRateLimit;
using FluentMigrator.Runner;
using FluentMigrator.Runner.Conventions;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using Serilog;
using System.Data.Common;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((ctx, lc) => lc
    .WriteTo.Console(new Serilog.Formatting.Display.MessageTemplateTextFormatter(
        "[{Timestamp:yyyy/MM/dd HH:mm:ss} {Level}] {Message:lj} {Exception}{NewLine}"))
    .ReadFrom.Configuration(ctx.Configuration)
    .Filter.ByExcluding(logEvent =>
        logEvent.Exception is BadHttpRequestException || logEvent.Exception is OperationCanceledException));

ConfigureServices(builder.Services, builder.Configuration);

var app = builder.Build();

app.UseSerilogRequestLogging();

Configure(app);

app.Run();

static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
{
    services.Configure<AppRegistryOptions>(configuration.GetSection(AppRegistryOptions.ConfigurationSectionName));

    services.AddAppRegistryDatabase(configuration);
    ConfigureMigrationRunner(services, configuration);

    services.AddTransient<IFamiliesService, FamiliesService>();
    services.AddTransient<IAppsService, AppsService>();

    AddRateLimits(services, configuration);
    AddMetrics(services);

    services.ConfigureHttpJsonOptions(options =>
    {
        options.SerializerOptions.TypeInfoResolverChain.Insert(0, AppRegistrySerializerContext.Default);
    });
}

static void ConfigureMigrationRunner(IServiceCollection services, IConfiguration configuration)
{
    services.AddSingleton<IConventionSet>(new DefaultConventionSet(DbConstants.Schema, null));

    var dbConnectionString = configuration.GetConnectionString("AppRegistry");

    services
        .AddFluentMigratorCore()
        .ConfigureRunner(migratorBuilder =>
            migratorBuilder
                .AddPostgres()
                .WithGlobalConnectionString(dbConnectionString)
                .ScanIn(typeof(DbConstants).Assembly).For.Migrations())
        .AddLogging(lb => lb.AddFluentMigratorConsole());
}

static void AddRateLimits(IServiceCollection services, IConfiguration configuration)
{
    services.Configure<IpRateLimitOptions>(configuration.GetSection("IpRateLimit"));

    services.AddMemoryCache();
    services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();
    services.AddInMemoryRateLimiting();
}

static void AddMetrics(IServiceCollection services)
{
    services.AddSingleton<OtelMetrics>();

    services.AddOpenTelemetry().WithMetrics(builder =>
        builder
            .ConfigureResource(rb => rb.AddService("AppRegistry"))
            .AddMeter(OtelMetrics.MeterName)
            .AddAspNetCoreInstrumentation()
            .AddRuntimeInstrumentation()
            .AddProcessInstrumentation()
            .AddOtlpExporter());
}

static void Configure(WebApplication app)
{
    app.UseMiddleware<ErrorHandlingMiddleware>();

    app.UseRouting();

    app.DefineFamiliesEndpoints();
    app.DefineAppEndpoints();
    app.DefineAdminEndpoints();

    app.UseIpRateLimiting();

    CreateDatabase(app);
    ApplyMigrations(app);
}

static void CreateDatabase(WebApplication app)
{
    var dbConnectionString = app.Configuration.GetConnectionString("AppRegistry");

    var connectionStringBuilder = new DbConnectionStringBuilder
    {
        ConnectionString = dbConnectionString
    };

    connectionStringBuilder["Database"] = "postgres";

    DatabaseExtensions.EnsureExists(connectionStringBuilder.ConnectionString!, DbConstants.Schema);
}

static void ApplyMigrations(WebApplication app)
{
    using var scope = app.Services.CreateScope();
    var runner = scope.ServiceProvider.GetRequiredService<IMigrationRunner>();

    if (runner.HasMigrationsToApplyUp())
    {
        runner.MigrateUp();
    }
}

[JsonSerializable(typeof(AppFamilyInfo[]))]
[JsonSerializable(typeof(AppInfo[]))]
[JsonSerializable(typeof(AppUsageInfo))]
[JsonSerializable(typeof(ScreenshotsResponse))]
[JsonSerializable(typeof(ResultsPage<AppReleaseInfo>))]
[JsonSerializable(typeof(ResultsPage<AppErrorInfo>))]
[JsonSerializable(typeof(ResultsPage<AppRunInfo>))]
[JsonSerializable(typeof(AppInstallerReleaseInfoResponse[]))]
[JsonSerializable(typeof(PublishAppReleaseResponse))]
[JsonSerializable(typeof(AppErrorRequest))]
[JsonSerializable(typeof(SendAppErrorResponse))]
[JsonSerializable(typeof(ResolveErrorsRequest))]
[JsonSerializable(typeof(UpdateReleaseRequest))]
[JsonSerializable(typeof(AppReleaseRequest))]
[JsonSerializable(typeof(UpdateInstallerRequest))]
[JsonSerializable(typeof(AppRegistryServiceError))]
internal partial class AppRegistrySerializerContext : JsonSerializerContext { }
