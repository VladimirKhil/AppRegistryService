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

    protected Guid AppId3 { get; } = Guid.NewGuid();

    protected Guid ReleaseId { get; } = Guid.NewGuid();

    protected Guid ReleaseId3 { get; } = Guid.NewGuid();

    protected Guid InstallerId3 { get; } = Guid.NewGuid();

    public TestsBase()
    {
        var builder = new ConfigurationBuilder().AddJsonFile("appsettings.json");
        var configuration = builder.Build();

        var services = new ServiceCollection();

        services.Configure<AppRegistryOptions>(configuration.GetSection(AppRegistryOptions.ConfigurationSectionName));

        services.AddAppRegistryDatabase(configuration);

        services.AddMetrics();
        services.AddSingleton<OtelMetrics>();

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
        await DbConnection.Languages.InsertAsync(() => new Database.Models.Language
        {
            Id = 0,
            Code = "ru-RU"
        });

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

        await DbConnection.FamiliesLocalized.InsertAsync(() => new Database.Models.AppFamilyLocalized
        {
            FamilyId = FamilyId2,
            LanguageId = 0,
            Description = "Тестовое описание 2",
        });

        await DbConnection.Apps.InsertAsync(() => new Database.Models.App
        {
            FamilyId = FamilyId,
            Id = AppId,
            Name = "App " + AppId
        });

        await DbConnection.Apps.InsertAsync(() => new Database.Models.App
        {
            FamilyId = FamilyId,
            Id = AppId2,
            Name = "App " + AppId2
        });

        await DbConnection.Apps.InsertAsync(() => new Database.Models.App
        {
            FamilyId = FamilyId,
            Id = AppId3,
            Name = "App " + AppId3
        });

        await DbConnection.AppsLocalized.InsertAsync(() => new Database.Models.AppLocalized
        {
            AppId = AppId,
            LanguageId = 0,
            KnownIssues = "Решение проблем"
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
            MinimumOSVersion = 1_000_000_000,
            Level = Database.Models.ReleaseLevel.Major,
            PublishDate = DateOnly.FromDateTime(DateTime.UtcNow),
            Notes = "notes"
        });

        await DbConnection.ReleasesLocalized.InsertAsync(() => new Database.Models.AppReleaseLocalized
        {
            ReleaseId = ReleaseId,
            LanguageId = 0,
            Notes = "заметки"
        });

        await DbConnection.Releases.InsertAsync(() => new Database.Models.AppRelease
        {
            Id = ReleaseId3,
            AppId = AppId3,
            Version = 1_000_000,
            MinimumOSVersion = 1_000_000_000,
            Level = Database.Models.ReleaseLevel.Major,
            PublishDate = DateOnly.FromDateTime(DateTime.UtcNow),
            Notes = "notes"
        });

        var installerId = Guid.NewGuid();

        await DbConnection.Installers.InsertAsync(() => new Database.Models.AppInstaller
        {
            Id = installerId,
            ReleaseId = ReleaseId,
            Size = 100,
            AdditionalSize = 200,
            Title = "Title",
            Description = "Description",
            Uri = "http://uri"
        });

        await DbConnection.InstallersLocalized.InsertAsync(() => new Database.Models.AppInstallerLocalized
        {
            InstallerId = installerId,
            LanguageId = 0,
            Title = "Заголовок",
            Description = "Описание",
        });

        await DbConnection.Installers.InsertAsync(() => new Database.Models.AppInstaller
        {
            Id = InstallerId3,
            ReleaseId = ReleaseId3,
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
        await DbConnection.InstallersLocalized.DeleteAsync();
        await DbConnection.Installers.DeleteAsync();
        await DbConnection.ReleasesLocalized.DeleteAsync();
        await DbConnection.Releases.DeleteAsync();
        await DbConnection.Screenshots.DeleteAsync();
        await DbConnection.AppsLocalized.DeleteAsync();
        await DbConnection.Apps.DeleteAsync();
        await DbConnection.FamiliesLocalized.DeleteAsync();
        await DbConnection.Families.DeleteAsync();
        await DbConnection.Languages.DeleteAsync();

        DbConnection.Dispose();
    }
}