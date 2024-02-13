using AppRegistry.Database;
using AppRegistryService.Configuration;
using AppRegistryService.Contracts;
using AppRegistryService.MapperProfiles;
using AppRegistryService.Metrics;
using AppRegistryService.Middlewares;
using AppRegistryService.Services;
using AspNetCoreRateLimit;
using AutoMapper;
using FluentMigrator.Runner;
using FluentMigrator.Runner.Conventions;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using Serilog;
using System.Data.Common;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((ctx, lc) => lc
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

    services.AddControllers();

    ConfigureAutoMapper(services);

    services.AddAppRegistryDatabase(configuration);
    ConfigureMigrationRunner(services, configuration);

    services.AddTransient<IFamiliesService, FamiliesService>();
    services.AddTransient<IAppsService, AppsService>();

    AddRateLimits(services, configuration);
    AddMetrics(services);
}

static void ConfigureAutoMapper(IServiceCollection services)
{
    var mapperConfiguration = new MapperConfiguration(cfg => cfg.AddProfile<AppRegistryProfile>());
    var mapper = mapperConfiguration.CreateMapper();

    services.AddSingleton(mapper);
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
    var meters = new OtelMetrics();

    services.AddOpenTelemetry().WithMetrics(builder =>
        builder
            .ConfigureResource(rb => rb.AddService("AppRegistry"))
            .AddMeter(meters.MeterName)
            .AddAspNetCoreInstrumentation()
            .AddRuntimeInstrumentation()
            .AddProcessInstrumentation()
            .AddPrometheusExporter());

    services.AddSingleton(meters);
}

static void Configure(WebApplication app)
{
    app.UseMiddleware<ErrorHandlingMiddleware>();

    app.UseRouting();
    app.MapControllers();

    app.UseIpRateLimiting();

    CreateDatabase(app);
    ApplyMigrations(app);

    app.UseOpenTelemetryPrometheusScrapingEndpoint();
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
