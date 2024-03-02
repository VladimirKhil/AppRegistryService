using AppRegistry.Database.Models;
using AppRegistryService.Contract.Requests;
using AppRegistryService.Exceptions;

namespace AppRegistry.ComponentTests.Services;

internal sealed class AppsServiceTests : TestsBase
{
    [Test]
    public async Task GetSchreenshots_Ok()
    {
        var screenshots = await AppsService.GetAppSchreenshotsAsync(AppId);

        Assert.That(screenshots, Is.Not.Null);
        Assert.That(screenshots, Has.Length.GreaterThanOrEqualTo(1));
        Assert.That(screenshots[0], Is.EqualTo("http://screenshot"));
    }

    [Test]
    public async Task GetReleases_Ok()
    {
        var (releases, total) = await AppsService.GetAppReleasesPageAsync(AppId, 0, 10);

        Assert.Multiple(() =>
        {
            Assert.That(releases, Has.Length.GreaterThanOrEqualTo(1));
            Assert.That(total, Is.GreaterThanOrEqualTo(1));
        });

        var release = releases.FirstOrDefault(r => r.Id == ReleaseId);

        Assert.That(release, Is.Not.Null);

        Assert.Multiple(() =>
        {
            Assert.That(release.AppId, Is.EqualTo(AppId));
            Assert.That(release.Version, Is.EqualTo(1_000_000));
            Assert.That(release.MinimumOSVersion, Is.EqualTo(10_000_000));
            Assert.That(release.Notes, Is.EqualTo("notes"));
            Assert.That(release.PublishDate, Is.EqualTo(DateOnly.FromDateTime(DateTimeOffset.UtcNow.Date)));
            Assert.That(release.Level, Is.EqualTo(ReleaseLevel.Major));
        });
    }

    [Test]
    public async Task GetReleases_Localized_Ok()
    {
        var (releases, total) = await AppsService.GetAppReleasesPageAsync(AppId, 0, 10, "ru-RU");

        Assert.Multiple(() =>
        {
            Assert.That(releases, Has.Length.GreaterThanOrEqualTo(1));
            Assert.That(total, Is.GreaterThanOrEqualTo(1));
        });

        var release = releases.FirstOrDefault(r => r.Id == ReleaseId);

        Assert.That(release, Is.Not.Null);

        Assert.Multiple(() =>
        {
            Assert.That(release.AppId, Is.EqualTo(AppId));
            Assert.That(release.Version, Is.EqualTo(1_000_000));
            Assert.That(release.MinimumOSVersion, Is.EqualTo(10_000_000));
            Assert.That(release.Notes, Is.EqualTo("заметки"));
            Assert.That(release.PublishDate, Is.EqualTo(DateOnly.FromDateTime(DateTimeOffset.UtcNow.Date)));
            Assert.That(release.Level, Is.EqualTo(ReleaseLevel.Major));
        });
    }

    [Test]
    public async Task PublishRelease_Ok()
    {
        var releaseId = await AppsService.PublishAppReleaseAsync(AppId, new AppRegistryService.Models.AppReleaseParameters(
            new Version(2, 0),
            new Version(6, 0),
            "test notes",
            ReleaseLevel.Major,
            new Dictionary<string, string>
            {
                ["ru-RU"] = "тестовые заметки"
            }));

        var (releases, total) = await AppsService.GetAppReleasesPageAsync(AppId, 0, 10);

        Assert.Multiple(() =>
        {
            Assert.That(releases, Has.Length.GreaterThanOrEqualTo(1));
            Assert.That(total, Is.GreaterThanOrEqualTo(1));
        });

        var release = releases.FirstOrDefault(r => r.Id == releaseId);

        Assert.That(release, Is.Not.Null);

        Assert.Multiple(() =>
        {
            Assert.That(release.AppId, Is.EqualTo(AppId));
            Assert.That(release.Version, Is.EqualTo(2_000_000));
            Assert.That(release.MinimumOSVersion, Is.EqualTo(6_000_000));
            Assert.That(release.Notes, Is.EqualTo("test notes"));
            Assert.That(release.PublishDate, Is.EqualTo(DateOnly.FromDateTime(DateTimeOffset.UtcNow.Date)));
            Assert.That(release.Level, Is.EqualTo(ReleaseLevel.Major));
        });

        var (releasesRu, totalRu) = await AppsService.GetAppReleasesPageAsync(AppId, 0, 10, "ru-RU");

        Assert.Multiple(() =>
        {
            Assert.That(releasesRu, Has.Length.GreaterThanOrEqualTo(1));
            Assert.That(totalRu, Is.GreaterThanOrEqualTo(1));
        });

        var releaseRu = releasesRu.FirstOrDefault(r => r.Id == releaseId);

        Assert.That(releaseRu, Is.Not.Null);

        Assert.Multiple(() =>
        {
            Assert.That(releaseRu.AppId, Is.EqualTo(AppId));
            Assert.That(releaseRu.Version, Is.EqualTo(2_000_000));
            Assert.That(releaseRu.MinimumOSVersion, Is.EqualTo(6_000_000));
            Assert.That(releaseRu.Notes, Is.EqualTo("тестовые заметки"));
            Assert.That(releaseRu.PublishDate, Is.EqualTo(DateOnly.FromDateTime(DateTimeOffset.UtcNow.Date)));
            Assert.That(releaseRu.Level, Is.EqualTo(ReleaseLevel.Major));
        });
    }

    [Test]
    public async Task GetInstallers_Ok()
    {
        var pairs = await AppsService.GetAppInstallersAsync(AppId);

        Assert.That(pairs, Is.Not.Null);
        Assert.That(pairs, Has.Length.GreaterThanOrEqualTo(1));

        var (release, installer) = pairs[0];

        Assert.Multiple(() =>
        {
            Assert.That(release.Id, Is.EqualTo(ReleaseId));
            Assert.That(release.AppId, Is.EqualTo(AppId));

            Assert.That(installer.ReleaseId, Is.EqualTo(ReleaseId));
            Assert.That(installer.Size, Is.EqualTo(100));
            Assert.That(installer.AdditionalSize, Is.EqualTo(200));
            Assert.That(installer.Title, Is.EqualTo("Title"));
            Assert.That(installer.Description, Is.EqualTo("Description"));
            Assert.That(installer.Uri, Is.EqualTo("http://uri"));
        });
    }

    [Test]
    public async Task GetInstallers_Localized_Ok()
    {
        var pairs = await AppsService.GetAppInstallersAsync(AppId, "ru-RU");

        Assert.That(pairs, Is.Not.Null);
        Assert.That(pairs, Has.Length.GreaterThanOrEqualTo(1));

        var (release, installer) = pairs[0];

        Assert.Multiple(() =>
        {
            Assert.That(release.Id, Is.EqualTo(ReleaseId));
            Assert.That(release.AppId, Is.EqualTo(AppId));
            Assert.That(release.Notes, Is.EqualTo("заметки"));

            Assert.That(installer.ReleaseId, Is.EqualTo(ReleaseId));
            Assert.That(installer.Size, Is.EqualTo(100));
            Assert.That(installer.AdditionalSize, Is.EqualTo(200));
            Assert.That(installer.Title, Is.EqualTo("Заголовок"));
            Assert.That(installer.Description, Is.EqualTo("Описание"));
            Assert.That(installer.Uri, Is.EqualTo("http://uri"));
        });
    }

    [Test]
    public async Task GetLatestInstaller_Ok()
    {
        var (release, installer) = await AppsService.GetAppLatestInstallerAsync(AppId, new Version(20, 0));

        Assert.Multiple(() =>
        {
            Assert.That(release, Is.Not.Null);
            Assert.That(installer, Is.Not.Null);
        });

        Assert.Multiple(() =>
        {
            Assert.That(release.Id, Is.EqualTo(ReleaseId));
            Assert.That(release.AppId, Is.EqualTo(AppId));

            Assert.That(installer.ReleaseId, Is.EqualTo(ReleaseId));
            Assert.That(installer.Size, Is.EqualTo(100));
            Assert.That(installer.AdditionalSize, Is.EqualTo(200));
            Assert.That(installer.Title, Is.EqualTo("Title"));
            Assert.That(installer.Description, Is.EqualTo("Description"));
            Assert.That(installer.Uri, Is.EqualTo("http://uri"));
        });
    }

    [Test]
    public async Task GetLatestInstaller_Localized_Ok()
    {
        var (release, installer) = await AppsService.GetAppLatestInstallerAsync(AppId, new Version(20, 0), "ru-RU");

        Assert.Multiple(() =>
        {
            Assert.That(release, Is.Not.Null);
            Assert.That(installer, Is.Not.Null);
        });

        Assert.Multiple(() =>
        {
            Assert.That(release.Id, Is.EqualTo(ReleaseId));
            Assert.That(release.AppId, Is.EqualTo(AppId));

            Assert.That(installer.ReleaseId, Is.EqualTo(ReleaseId));
            Assert.That(installer.Size, Is.EqualTo(100));
            Assert.That(installer.AdditionalSize, Is.EqualTo(200));
            Assert.That(installer.Title, Is.EqualTo("Заголовок"));
            Assert.That(installer.Description, Is.EqualTo("Описание"));
            Assert.That(installer.Uri, Is.EqualTo("http://uri"));
        });
    }

    [Test]
    public void GetLatestInstaller_UpsupportedOSVersion_Fail()
    {
        var exc = Assert.ThrowsAsync<ServiceException>(() => AppsService.GetAppLatestInstallerAsync(AppId, new Version(2, 0)));

        Assert.That(exc.ErrorCode, Is.EqualTo(AppRegistryService.Contract.Models.WellKnownAppRegistryServiceErrorCode.AppReleaseNotFound));
    }

    [Test]
    public async Task UpdateInstaller_Ok()
    {
        var (release, installer) = await AppsService.GetAppLatestInstallerAsync(AppId3, new Version(20, 0));

        Assert.Multiple(() =>
        {
            Assert.That(release, Is.Not.Null);
            Assert.That(installer, Is.Not.Null);
        });

        Assert.Multiple(() =>
        {
            Assert.That(release.Id, Is.EqualTo(ReleaseId3));
            Assert.That(release.AppId, Is.EqualTo(AppId3));

            Assert.That(installer.Id, Is.EqualTo(InstallerId3));
            Assert.That(installer.ReleaseId, Is.EqualTo(ReleaseId3));
            Assert.That(installer.Uri, Is.EqualTo("http://uri"));
        });

        var releaseId = await AppsService.PublishAppReleaseAsync(AppId3, new AppRegistryService.Models.AppReleaseParameters(
            new Version(2, 0),
            new Version(6, 0),
            "test notes",
            ReleaseLevel.Major));

        await AppsService.UpdateInstallerAsync(InstallerId3, releaseId, new Uri("http://new/file.f"));

        var (release2, installer2) = await AppsService.GetAppLatestInstallerAsync(AppId3, new Version(20, 0));

        Assert.Multiple(() =>
        {
            Assert.That(release2, Is.Not.Null);
            Assert.That(installer2, Is.Not.Null);
        });

        Assert.Multiple(() =>
        {
            Assert.That(release2.Id, Is.EqualTo(releaseId));
            Assert.That(release2.AppId, Is.EqualTo(AppId3));

            Assert.That(installer2.Id, Is.EqualTo(InstallerId3));
            Assert.That(installer2.ReleaseId, Is.EqualTo(releaseId));
            Assert.That(installer2.Uri, Is.EqualTo("http://new/file.f"));
        });
    }

    [Test]
    public async Task GetRuns_Ok()
    {
        await AppsService.PostAppUsageAsync(AppId, new Version(1, 0), new Version(12, 0), System.Runtime.InteropServices.Architecture.X64);

        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var (runs, total) = await AppsService.GetAppRunsPageAsync(AppId, today, 10);

        Assert.That(runs, Is.Not.Null);
        Assert.That(runs, Has.Length.GreaterThanOrEqualTo(1));

        Assert.That(runs[0].Version, Is.EqualTo(1_000_000));

        var run = runs[0].Run;

        Assert.Multiple(() =>
        {
            Assert.That(total, Is.EqualTo(1));

            Assert.That(run.ReleaseId, Is.EqualTo(ReleaseId));
            Assert.That(run.Count, Is.GreaterThanOrEqualTo(1));
            Assert.That(run.OSVersion, Is.EqualTo(12_000_000));
            Assert.That(run.OSArchitecture, Is.EqualTo(System.Runtime.InteropServices.Architecture.X64));
            Assert.That(run.Date, Is.EqualTo(today));
        });
    }

    [Test]
    public async Task Errors_Report_Get_Resolve_Ok()
    {
        var now = DateTimeOffset.UtcNow;

        var status = await AppsService.SendAppErrorReportAsync(
            AppId,
            new AppErrorRequest(new Version(1, 0), new Version(12, 0), System.Runtime.InteropServices.Architecture.X64)
            {
                ErrorMessage = "test error",
                ErrorTime = now
            });

        Assert.That(status, Is.EqualTo(ErrorStatus.NotFixed));

        var (errors, total) = await AppsService.GetAppErrorsPageAsync(AppId, 0, 10);

        Assert.That(errors, Is.Not.Null);
        Assert.That(errors, Has.Length.GreaterThanOrEqualTo(1));

        var error = errors[0].Error;

        Assert.Multiple(() =>
        {
            Assert.That(total, Is.EqualTo(1));

            Assert.That(error.ReleaseId, Is.EqualTo(ReleaseId));
            Assert.That(error.Count, Is.GreaterThanOrEqualTo(1));
            Assert.That(error.OSVersion, Is.EqualTo(12_000_000));
            Assert.That(error.OSArchitecture, Is.EqualTo(System.Runtime.InteropServices.Architecture.X64));
            Assert.That(error.Message, Is.EqualTo("test error"));
            Assert.That(error.Time.Subtract(now).TotalSeconds, Is.LessThan(1));
            Assert.That(error.Status, Is.EqualTo(ErrorStatus.NotFixed));
        });

        await AppsService.ResolveErrorsAsync(new[] { error.Id });

        var (errors2, total2) = await AppsService.GetAppErrorsPageAsync(AppId, 0, 10);

        Assert.That(errors2, Is.Not.Null);
        Assert.That(errors2, Has.Length.EqualTo(0));
    }
}
