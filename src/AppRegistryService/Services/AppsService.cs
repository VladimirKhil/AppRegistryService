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

public sealed class AppsService(AppRegistryDbConnection connection, OtelMetrics metrics) : IAppsService
{
    public async Task<string[]> GetAppSchreenshotsAsync(Guid appId, CancellationToken cancellationToken = default) =>
        await connection.Screenshots.Where(s => s.AppId == appId).Select(s => s.ScreentshotUri).ToArrayAsync(cancellationToken);

    public async Task<(AppErrorWithVersion[], int)> GetAppErrorsPageAsync(
        Guid appId,
        int from,
        int count,
        CancellationToken cancellationToken = default)
    {
        var query = from r in connection.Releases
                    where r.AppId == appId
                    from e in connection.Errors
                    where e.ReleaseId == r.Id && e.Status != ErrorStatus.Fixed
                    orderby e.Time descending
                    select new { Error = e, r.Version };

        var errors = await query.Skip(from).Take(count).ToArrayAsync(cancellationToken);
        var total = await query.CountAsync(cancellationToken);

        return (errors.Select(e => new AppErrorWithVersion(e.Error, e.Version)).ToArray(), total);
    }

    public async Task<(AppRelease, AppInstaller)[]> GetAppInstallersAsync(Guid appId, string language, CancellationToken cancellationToken = default)
    {
        var query = from r in connection.Releases
                    where r.AppId == appId
                    from i in connection.Installers
                    where i.ReleaseId == r.Id
                    from l in connection.Languages.Where(l => l.Code == language).DefaultIfEmpty()
                    from rl in connection.ReleasesLocalized.Where(rl => rl.ReleaseId == r.Id && rl.LanguageId == l.Id).DefaultIfEmpty()
                    from il in connection.InstallersLocalized.Where(il => il.InstallerId == i.Id && il.LanguageId == l.Id).DefaultIfEmpty()
                    orderby i.Order ascending
                    select new { Release = Localize(r, rl), Installer = Localize(i, il) };

        var result = await query.ToArrayAsync(cancellationToken);

        return result.Select(r => (r.Release, r.Installer)).ToArray();
    }


    public async Task<(AppRelease, AppInstaller)> GetAppLatestInstallerAsync(
        Guid appId,
        Version? osVersion,
        string language,
        CancellationToken cancellationToken = default)
    {
        var osVersionInt = osVersion == null ? int.MaxValue : osVersion.ToOSInt();

        var query = from r in connection.Releases
                    where r.AppId == appId
                    from i in connection.Installers
                    where i.ReleaseId == r.Id && r.MinimumOSVersion <= osVersionInt
                    from l in connection.Languages.Where(l => l.Code == language).DefaultIfEmpty()
                    from rl in connection.ReleasesLocalized.Where(rl => rl.ReleaseId == r.Id && rl.LanguageId == l.Id).DefaultIfEmpty()
                    from il in connection.InstallersLocalized.Where(il => il.InstallerId == i.Id && il.LanguageId == l.Id).DefaultIfEmpty()
                    orderby r.PublishDate descending
                    select new { Release = Localize(r, rl), Installer = Localize(i, il) };

        var result = await query.FirstOrDefaultAsync(cancellationToken);

        return result == null
            ? throw new ServiceException(Contract.Models.WellKnownAppRegistryServiceErrorCode.AppReleaseNotFound, HttpStatusCode.NotFound)
            : (result.Release, result.Installer);
    }

    public async Task<(AppRelease[], int)> GetAppReleasesPageAsync(
        Guid appId,
        int from,
        int count,
        string language,
        CancellationToken cancellationToken = default)
    {
        var query = from r in connection.Releases
                    where r.AppId == appId
                    from l in connection.Languages.Where(l => l.Code == language).DefaultIfEmpty()
                    from rl in connection.ReleasesLocalized.Where(rl => rl.ReleaseId == r.Id && rl.LanguageId == l.Id).DefaultIfEmpty()
                    orderby r.PublishDate descending
                    select new { Release = r, Localization = rl };

        var releases = await query
            .Skip(from)
            .Take(count)
            .Select(r => Localize(r.Release, r.Localization))
            .ToArrayAsync(cancellationToken);

        var total = await query.CountAsync(cancellationToken);

        return (releases, total);
    }

    public async Task<(AppRunWithVersion[], int)> GetAppRunsPageAsync(Guid appId, DateOnly to, int count, CancellationToken cancellationToken = default)
    {
        var query = from r in connection.Releases
                    where r.AppId == appId
                    from run in connection.Runs
                    where run.ReleaseId == r.Id && run.Date <= to
                    orderby run.Date descending
                    select new { Run = run, r.Version };

        var runs = await query.Take(count).ToArrayAsync(cancellationToken);
        var total = await query.CountAsync(cancellationToken);

        return (runs.Select(r => new AppRunWithVersion(r.Run, r.Version)).ToArray(), total);
    }

    public async Task<bool> TryPostAppUsageAsync(
        Guid appId,
        Version appVersion,
        Version osVersion,
        Architecture osArchitecture,
        CancellationToken cancellationToken = default)
    {
        var release = await connection.Releases.FirstOrDefaultAsync(r => r.AppId == appId && r.Version == appVersion.ToInt(), cancellationToken);

        if (release == null)
        {
            return false;
        }

        var osVersionInt = osVersion.ToOSInt();
        var today = DateOnly.FromDateTime(DateTimeOffset.UtcNow.Date);

        await connection.Runs.InsertOrUpdateAsync(
            () => new AppRun
            {
                ReleaseId = release.Id,
                OSVersion = osVersionInt,
                OSArchitecture = osArchitecture,
                Date = today,
                Count = 1,
            },
            appRun => new AppRun
            {
                ReleaseId = release.Id,
                OSVersion = osVersionInt,
                OSArchitecture = osArchitecture,
                Date = today,
                Count = appRun.Count + 1,
            },
            () => new AppRun
            {
                ReleaseId = release.Id,
                OSVersion = osVersionInt,
                OSArchitecture = osArchitecture,
                Date = today
            },
            cancellationToken);

        metrics.AddRun(appId);

        return true;
    }

    public async Task<Guid> PublishAppReleaseAsync(Guid appId, AppReleaseParameters parameters, CancellationToken cancellationToken = default)
    {
        var releaseId = Guid.NewGuid();

        await connection.Releases.InsertAsync(
            () => new AppRelease
            {
                Id = releaseId,
                AppId = appId,
                Version = parameters.Version.ToInt(),
                MinimumOSVersion = parameters.MinimumOSVersion.ToOSInt(),
                Level = parameters.Level,
                Notes = parameters.Notes,
                PublishDate = DateOnly.FromDateTime(DateTimeOffset.UtcNow.Date)
            },
            cancellationToken);

        if (parameters.LocalizedNotes != null)
        {
            foreach (var note in parameters.LocalizedNotes)
            {
                var language = await connection.Languages.FirstOrDefaultAsync(l => l.Code == note.Key, cancellationToken)
                    ?? throw new ServiceException(
                        Contract.Models.WellKnownAppRegistryServiceErrorCode.LanguageNotFound,
                        HttpStatusCode.FailedDependency);

                await connection.ReleasesLocalized.InsertAsync(() => new AppReleaseLocalized
                {
                    ReleaseId = releaseId,
                    LanguageId = language.Id,
                    Notes = note.Value,
                },
                cancellationToken);
            }
        }

        return releaseId;
    }

    public async Task UpdateReleaseAsync(Guid releaseId, bool isMandatory, CancellationToken cancellationToken) =>
        await connection.Releases.Where(r => r.Id == releaseId).Set(r => r.IsMandatory, isMandatory).UpdateAsync(cancellationToken);

    public async Task UpdateInstallerAsync(Guid installerId, Guid newReleaseId, Uri newUri, CancellationToken cancellationToken = default) =>
        await connection.Installers.Where(i => i.Id == installerId).Set(i => i.ReleaseId, newReleaseId).Set(i => i.Uri, newUri.ToString()).UpdateAsync(cancellationToken);

    public async Task ResolveErrorsAsync(int[] errorIds, CancellationToken cancellationToken = default) =>
        await connection.Errors.Where(e => errorIds.Contains(e.Id)).Set(e => e.Status, ErrorStatus.Fixed).UpdateAsync(cancellationToken);

    public async Task<ErrorStatus> SendAppErrorReportAsync(Guid appId, AppErrorRequest appErrorInfo, CancellationToken cancellationToken = default)
    {
        var release = await connection.Releases.FirstOrDefaultAsync(
            r => r.AppId == appId && r.Version == appErrorInfo.Version.ToInt(),
            cancellationToken) ?? throw new ServiceException(
                Contract.Models.WellKnownAppRegistryServiceErrorCode.AppReleaseNotFound,
                HttpStatusCode.NotFound);

        var osVersionInt = appErrorInfo.OSVersion.ToOSInt();

        await connection.Errors.InsertOrUpdateAsync(
            () => new AppError
            {
                ReleaseId = release.Id,
                Time = appErrorInfo.ErrorTime,
                Message = appErrorInfo.ErrorMessage,
                UserNotes = appErrorInfo.UserNotes,
                OSVersion = osVersionInt,
                OSArchitecture = appErrorInfo.OSArchitecture,
                Status = ErrorStatus.NotFixed,
                Count = 1
            },
            error => new AppError
            {
                ReleaseId = release.Id,
                Time = error.Status == ErrorStatus.Fixed ? error.Time : appErrorInfo.ErrorTime,
                Message = appErrorInfo.ErrorMessage,
                UserNotes = error.UserNotes,
                OSVersion = osVersionInt,
                OSArchitecture = appErrorInfo.OSArchitecture,
                Status = error.Status,
                Count = error.Count + 1,
            },
            () => new AppError
            {
                ReleaseId = release.Id,
                OSVersion = osVersionInt,
                OSArchitecture = appErrorInfo.OSArchitecture,
                Message = appErrorInfo.ErrorMessage
            },
            cancellationToken);

        metrics.AddError(appId);

        var error = await connection.Errors.FirstAsync(
            e => e.ReleaseId == release.Id
                && e.OSVersion == osVersionInt
                && e.OSArchitecture == appErrorInfo.OSArchitecture
                && e.Message == appErrorInfo.ErrorMessage,
            cancellationToken);

        return error.Status;
    }

    private static AppRelease Localize(AppRelease release, AppReleaseLocalized? localization) => localization == null
        ? release
        : new AppRelease
        {
            Id = release.Id,
            AppId = release.AppId,
            Level = release.Level,
            MinimumOSVersion = release.MinimumOSVersion,
            PublishDate = release.PublishDate,
            Version = release.Version,
            Notes = localization.Notes
        };

    private static AppInstaller Localize(AppInstaller installer, AppInstallerLocalized? localization) => localization == null
        ? installer
        : new AppInstaller
        {
            Id = installer.Id,
            AdditionalSize = installer.AdditionalSize,
            Description = localization.Description,
            ReleaseId = installer.ReleaseId,
            Size = installer.Size,
            Title = localization.Title,
            Uri = installer.Uri
        };
}
