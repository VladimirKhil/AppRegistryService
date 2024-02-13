using AppRegistry.Database;
using AppRegistryService.Configuration;
using AppRegistryService.Contracts;
using AppRegistryService.Metrics;
using AppRegistryService.Services;
using LinqToDB;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace AppRegistry.ComponentTests;

internal abstract class TestsBase
{
    private AppRegistryDbConnection DbConnection { get; }

    protected IFamiliesService FamiliesService { get; }

    protected IAppsService AppsService { get; }

    protected Guid FamilyId { get; } = Guid.NewGuid();

    protected Guid FamilyId2 { get; } = Guid.NewGuid();

    protected Guid AppId { get; } = Guid.NewGuid();

    protected Guid AppId2 { get; } = Guid.NewGuid();

    protected Guid ReleaseId { get; } = Guid.NewGuid();

    public TestsBase()
    {
        var builder = new ConfigurationBuilder().AddJsonFile("appsettings.json");
        var configuration = builder.Build();

        var services = new ServiceCollection();

        services.Configure<AppRegistryOptions>(configuration.GetSection(AppRegistryOptions.ConfigurationSectionName));

        services.AddAppRegistryDatabase(configuration);

        var meters = new OtelMetrics();
        services.AddSingleton(meters);

        services.AddSingleton<ILoggerFactory, NullLoggerFactory>();

        services.AddTransient<IFamiliesService, FamiliesService>();
        services.AddTransient<IAppsService, AppsService>();

        var serviceProvider = services.BuildServiceProvider();

        FamiliesService = serviceProvider.GetRequiredService<IFamiliesService>();
        AppsService = serviceProvider.GetRequiredService<IAppsService>();
        DbConnection = serviceProvider.GetRequiredService<AppRegistryDbConnection>();
    }

    [OneTimeSetUp]
    public async Task SetUp()
    {
        await DbConnection.Families.InsertAsync(() => new Database.Models.AppFamily
        {
            Id = FamilyId,
            Name = "TestAppFamily " + FamilyId,
            Description = "Test description",
            LogoUri = "http://test.logo"
        });

        await DbConnection.Families.InsertAsync(() => new Database.Models.AppFamily
        {
            Id = FamilyId2,
            Name = "TestAppFamily " + FamilyId2,
            Description = "Test description 2",
            LogoUri = "http://test2.logo"
        });

        await DbConnection.Apps.InsertAsync(() => new Database.Models.App
        {
            AppFamilyId = FamilyId,
            Id = AppId,
            Name = "App " + AppId
        });

        await DbConnection.Apps.InsertAsync(() => new Database.Models.App
        {
            AppFamilyId = FamilyId,
            Id = AppId2,
            Name = "App " + AppId2
        });

        await DbConnection.Screenshots.InsertAsync(() => new Database.Models.AppScreenshot
        {
            AppId = AppId,
            ScreentshotUri = "http://screenshot"
        });

        await DbConnection.Releases.InsertAsync(() => new Database.Models.AppRelease
        {
            Id = ReleaseId,
            AppId = AppId,
            Version = 1_000_000,
            MinimumOSVersion = 10_000_000,
            Level = Database.Models.ReleaseLevel.Major,
            PublishDate = DateOnly.FromDateTime(DateTime.UtcNow),
            Notes = "notes"
        });

        await DbConnection.Installers.InsertAsync(() => new Database.Models.AppInstaller
        {
            Id = Guid.NewGuid(),
            ReleaseId = ReleaseId,
            Size = 100,
            AdditionalSize = 200,
            Title = "Title",
            Description = "Description",
            Uri = "http://uri"
        });
    }

    [OneTimeTearDown]
    public async Task TearDown()
    {
        await DbConnection.Errors.DeleteAsync();
        await DbConnection.Runs.DeleteAsync();
        await DbConnection.Installers.DeleteAsync();
        await DbConnection.Releases.DeleteAsync();
        await DbConnection.Screenshots.DeleteAsync();
        await DbConnection.Apps.DeleteAsync();
        await DbConnection.Families.DeleteAsync();
    }
}