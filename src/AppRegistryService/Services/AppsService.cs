using AppRegistry.Database;
using AppRegistry.Database.Models;
using AppRegistryService.Contract.Requests;
using AppRegistryService.Contracts;
using AppRegistryService.Exceptions;
using AppRegistryService.Helpers;
using AppRegistryService.Metrics;
using AppRegistryService.Models;
using LinqToDB;
using System.Net;
using System.Runtime.InteropServices;

namespace AppRegistryService.Services;

public sealed class AppsService : IAppsService
{
    private readonly AppRegistryDbConnection _connection;
    private readonly OtelMetrics _metrics;

    public AppsService(AppRegistryDbConnection connection, OtelMetrics metrics)
    {
        _connection = connection;
        _metrics = metrics;
    }

    public async Task<string[]> GetAppSchreenshotsAsync(Guid appId, CancellationToken cancellationToken = default) =>
        await _connection.Screenshots.Where(s => s.AppId == appId).Select(s => s.ScreentshotUri).ToArrayAsync(cancellationToken);

    public async Task<(AppErrorWithVersion[], int)> GetAppErrorsPageAsync(Guid appId, int from, int count, CancellationToken cancellationToken = default)
    {
        var query = from r in _connection.Releases
                    where r.AppId == appId
                    from e in _connection.Errors
                    where e.ReleaseId == r.Id && e.Status != ErrorStatus.Fixed
                    orderby e.Time descending
                    select new { Error = e, r.Version };

        var errors = await query.Skip(from).Take(count).ToArrayAsync(cancellationToken);
        var total = await query.CountAsync(cancellationToken);

        return (errors.Select(e => new AppErrorWithVersion(e.Error, e.Version)).ToArray(), total);
    }

    public async Task<(AppRelease, AppInstaller)[]> GetAppInstallersAsync(Guid appId, CancellationToken cancellationToken = default)
    {
        var query = from r in _connection.Releases
                    where r.AppId == appId
                    from i in _connection.Installers
                    where i.ReleaseId == r.Id
                    orderby r.PublishDate descending
                    select new { Release = r, Installer = i };

        var result = await query.ToArrayAsync(cancellationToken);

        return result.Select(r => (r.Release, r.Installer)).ToArray();
    }

    public async Task<(AppRelease, AppInstaller)> GetAppLatestInstallerAsync(Guid appId, Version? osVersion, CancellationToken cancellationToken = default)
    {
        var versionValue = osVersion == null ? int.MaxValue : osVersion.ToInt();

        var query = from r in _connection.Releases
                    where r.AppId == appId
                    from i in _connection.Installers
                    where i.ReleaseId == r.Id && r.MinimumOSVersion <= versionValue
                    orderby r.PublishDate descending
                    select new { Release = r, Installer = i };

        var result = await query.FirstOrDefaultAsync(cancellationToken);

        return result == null
            ? throw new ServiceException(Contract.Models.WellKnownAppRegistryServiceErrorCode.AppReleaseNotFound, HttpStatusCode.NotFound)
            : (result.Release, result.Installer);
    }

    public async Task<(AppRelease[], int)> GetAppReleasesPageAsync(Guid appId, int from, int count, CancellationToken cancellationToken = default)
    {
        var query = from r in _connection.Releases
                    where r.AppId == appId
                    orderby r.PublishDate descending
                    select r;

        var releases = await query.Skip(from).Take(count).ToArrayAsync(cancellationToken);
        var total = await query.CountAsync(cancellationToken);

        return (releases, total);
    }

    public async Task<(AppRunWithVersion[], int)> GetAppRunsPageAsync(Guid appId, DateOnly to, int count, CancellationToken cancellationToken = default)
    {
        var query = from r in _connection.Releases
                    where r.AppId == appId
                    from run in _connection.Runs
                    where run.ReleaseId == r.Id && run.Date <= to
                    orderby run.Date descending
                    select new { Run = run, r.Version };

        var runs = await query.Take(count).ToArrayAsync(cancellationToken);
        var total = await query.CountAsync(cancellationToken);

        return (runs.Select(r => new AppRunWithVersion(r.Run, r.Version)).ToArray(), total);
    }

    public async Task PostAppUsageAsync(
        Guid appId,
        Version appVersion,
        Version osVersion,
        Architecture osArchitecture,
        CancellationToken cancellationToken = default)
    {
        var release = await _connection.Releases.FirstOrDefaultAsync(r => r.AppId == appId && r.Version == appVersion.ToInt(), cancellationToken)
            ?? throw new ServiceException(Contract.Models.WellKnownAppRegistryServiceErrorCode.AppReleaseNotFound, HttpStatusCode.NotFound);

        var osVersionValue = osVersion.ToInt();
        var today = DateOnly.FromDateTime(DateTimeOffset.UtcNow.Date);

        await _connection.Runs.InsertOrUpdateAsync(
            () => new AppRun
            {
                ReleaseId = release.Id,
                OSVersion = osVersionValue,
                OSArhitecture = osArchitecture,
                Date = today,
                Count = 1,
            },
            appRun => new AppRun
            {
                ReleaseId = release.Id,
                OSVersion = osVersionValue,
                OSArhitecture = osArchitecture,
                Date = today,
                Count = appRun.Count + 1,
            },
            () => new AppRun
            {
                ReleaseId = release.Id,
                OSVersion = osVersionValue,
                OSArhitecture = osArchitecture,
                Date = today
            },
            cancellationToken);

        _metrics.AddRun(appId);
    }

    public async Task<Guid> PublishAppReleaseAsync(Guid appId, AppReleaseParameters parameters, CancellationToken cancellationToken = default)
    {
        var releaseId = Guid.NewGuid();

        await _connection.Releases.InsertAsync(
            () => new AppRelease
            {
                Id = releaseId,
                AppId = appId,
                Version = parameters.Version.ToInt(),
                MinimumOSVersion = parameters.MinimumOSVersion.ToInt(),
                Level = parameters.Level,
                Notes = parameters.Notes,
                PublishDate = DateOnly.FromDateTime(DateTimeOffset.UtcNow.Date)
            },
            cancellationToken);

        return releaseId;
    }

    public async Task ResolveErrorsAsync(int[] errorIds, CancellationToken cancellationToken = default) =>
        await _connection.Errors.Where(e => errorIds.Contains(e.Id)).Set(e => e.Status, ErrorStatus.Fixed).UpdateAsync(cancellationToken);

    public async Task<ErrorStatus> SendAppErrorReportAsync(Guid appId, AppErrorRequest appErrorInfo, CancellationToken cancellationToken = default)
    {
        var release = await _connection.Releases.FirstOrDefaultAsync(
            r => r.AppId == appId && r.Version == appErrorInfo.Version.ToInt(),
            cancellationToken) ?? throw new ServiceException(
                Contract.Models.WellKnownAppRegistryServiceErrorCode.AppReleaseNotFound,
                HttpStatusCode.NotFound);

        var osVersion = appErrorInfo.OSVersion.ToInt();

        await _connection.Errors.InsertOrUpdateAsync(
            () => new AppError
            {
                ReleaseId = release.Id,
                Time = appErrorInfo.ErrorTime,
                Message = appErrorInfo.ErrorMessage,
                OSVersion = osVersion,
                OSArhitecture = appErrorInfo.OSArchitecture,
                Status = ErrorStatus.NotFixed,
                Count = 1
            },
            relation => new AppError
            {
                ReleaseId = release.Id,
                Time = relation.Status == ErrorStatus.Fixed ? relation.Time : appErrorInfo.ErrorTime,
                Message = appErrorInfo.ErrorMessage,
                OSVersion = osVersion,
                OSArhitecture = appErrorInfo.OSArchitecture,
                Status = relation.Status,
                Count = relation.Count + 1,
            },
            () => new AppError
            {
                ReleaseId = release.Id,
                OSVersion = osVersion,
                OSArhitecture = appErrorInfo.OSArchitecture,
                Message = appErrorInfo.ErrorMessage
            },
            cancellationToken);

        _metrics.AddError(appId);

        var error = await _connection.Errors.FirstAsync(
            e => e.ReleaseId == release.Id
                && e.OSVersion == osVersion
                && e.OSArhitecture == appErrorInfo.OSArchitecture
                && e.Message == appErrorInfo.ErrorMessage,
            cancellationToken);

        return error.Status;
    }
}
