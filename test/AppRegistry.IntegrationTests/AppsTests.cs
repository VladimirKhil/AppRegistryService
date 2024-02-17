using AppRegistryService.Contract.Models;
using AppRegistryService.Contract.Requests;
using System.Runtime.InteropServices;

namespace AppRegistry.IntegrationTests;

internal sealed class AppsTests : TestsBase
{
    private Guid _appId;

    [SetUp]
    public async Task SetUp()
    {
        var families = await FamiliesApi.GetFamiliesAsync();

        for (var i = 0; i < families!.Length; i++)
        {
            var apps = await FamiliesApi.GetFamilyAppsAsync(families![i].Id);

            if (apps != null && apps.Length > 0)
            {
                _appId = apps![0].Id;
                break;
            }
        }
    }

    [Test]
    public async Task GetScreenshots_Ok()
    {
        var screenshots = await Apps.GetAppScreenshotsAsync(_appId);

        Assert.That(screenshots, Is.Not.Null);
        Assert.That(screenshots.ScreenshotUris, Has.Length.GreaterThanOrEqualTo(1));
    }

    [Test]
    public async Task GetReleases_Ok()
    {
        var releases = await Apps.GetAppReleasesPageAsync(_appId, 0, 10);

        Assert.That(releases, Is.Not.Null);
        Assert.That(releases.Results, Is.Not.Null);

        Assert.Multiple(() =>
        {
            Assert.That(releases.Results, Has.Length.GreaterThanOrEqualTo(1));
            Assert.That(releases.Total, Is.GreaterThanOrEqualTo(1));
        });

        var release = releases.Results[0];

        Assert.That(release.AppId, Is.EqualTo(_appId));
    }

    [Test]
    public async Task GetInstallers_Ok()
    {
        var pairs = await Apps.GetAppInstallersAsync(_appId);

        Assert.That(pairs, Is.Not.Null);
        Assert.That(pairs, Has.Length.GreaterThanOrEqualTo(1));

        var release = pairs[0].Release;
        var installer = pairs[0].Installer;

        Assert.Multiple(() =>
        {
            Assert.That(release, Is.Not.Null);
            Assert.That(installer, Is.Not.Null);
        });

        Assert.Multiple(() =>
        {
            Assert.That(release!.AppId, Is.EqualTo(_appId));
            Assert.That(installer!.ReleaseId, Is.EqualTo(release.Id));
        });
    }

    [Test]
    public async Task GetLatestInstaller_Ok()
    {
        var response = await Apps.GetAppLatestInstallerAsync(_appId, new Version(20, 0));

        Assert.That(response, Is.Not.Null);

        var release = response.Release;
        var installer = response.Installer;

        Assert.Multiple(() =>
        {
            Assert.That(release, Is.Not.Null);
            Assert.That(installer, Is.Not.Null);
        });

        Assert.Multiple(() =>
        {
            Assert.That(release!.AppId, Is.EqualTo(_appId));
            Assert.That(installer!.ReleaseId, Is.EqualTo(release.Id));
        });
    }

    [Test]
    public async Task GetRuns_Ok()
    {
        await Apps.PostAppUsageAsync(
            _appId,
            new AppUsageInfo(new Version(1, 0), new Version(12, 0), Architecture.X64));

        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var runs = await Admin.GetAppRunsPageAsync(_appId, today, 10);

        Assert.That(runs, Is.Not.Null);
        Assert.That(runs.Results, Is.Not.Null);

        Assert.Multiple(() =>
        {
            Assert.That(runs.Results, Has.Length.GreaterThanOrEqualTo(1));
            Assert.That(runs.Total, Is.GreaterThanOrEqualTo(1));
        });

        var run = runs.Results[0];

        Assert.Multiple(() =>
        {
            Assert.That(run.Count, Is.GreaterThanOrEqualTo(1));
            Assert.That(run.Date, Is.EqualTo(today));
        });
    }

    [Test]
    public async Task Errors_Report_Get_Resolve_Ok()
    {
        var now = DateTimeOffset.UtcNow;
        var errorMessage = "test error " + Guid.NewGuid();

        var status = await Apps.SendAppErrorReportAsync(
            _appId,
            new AppErrorRequest(new Version(1, 0), new Version(12, 0), Architecture.X64)
            {
                ErrorMessage = errorMessage,
                ErrorTime = now
            });

        Assert.That(status, Is.EqualTo(ErrorStatus.NotFixed));

        var results = await Admin.GetAppErrorsPageAsync(_appId, 0, 10);

        Assert.That(results, Is.Not.Null);
        Assert.That(results.Results, Is.Not.Null);

        Assert.Multiple(() =>
        {
            Assert.That(results.Results, Has.Length.GreaterThanOrEqualTo(1));
            Assert.That(results.Total, Is.GreaterThanOrEqualTo(1));
        });

        var error = results.Results[0];

        Assert.Multiple(() =>
        {
            Assert.That(error.Count, Is.GreaterThanOrEqualTo(1));
        });

        await Admin.ResolveErrorsAsync(new ResolveErrorsRequest(new [] { error.Id }));

        var results2 = await Admin.GetAppErrorsPageAsync(_appId, 0, 10);

        Assert.That(results2, Is.Not.Null);
        Assert.That(results2.Total, Is.EqualTo(0));

        var status2 = await Apps.SendAppErrorReportAsync(
            _appId,
            new AppErrorRequest(new Version(1, 0), new Version(12, 0), Architecture.X64)
            {
                ErrorMessage = errorMessage,
                ErrorTime = now
            });

        Assert.That(status2, Is.EqualTo(ErrorStatus.Fixed));
    }
}