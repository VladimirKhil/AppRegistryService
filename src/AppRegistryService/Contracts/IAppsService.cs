using AppRegistry.Database.Models;
using AppRegistryService.Contract.Requests;
using AppRegistryService.Models;
using System.Runtime.InteropServices;

namespace AppRegistryService.Contracts;

public interface IAppsService
{
    Task<string[]> GetAppSchreenshotsAsync(Guid appId, CancellationToken cancellationToken = default);

    Task<Guid> PublishAppReleaseAsync(Guid appId, AppReleaseParameters parameters, CancellationToken cancellationToken = default);

    Task UpdateReleaseAsync(Guid releaseId, bool isMandatory, CancellationToken cancellationToken);

    Task<(AppRelease[] Releases, int Total)> GetAppReleasesPageAsync(
        Guid appId,
        int from,
        int count,
        string language = Constants.DefaultLanguageCode,
        CancellationToken cancellationToken = default);

    Task<(AppRelease, AppInstaller)[]> GetAppInstallersAsync(
        Guid appId,
        string language = Constants.DefaultLanguageCode,
        CancellationToken cancellationToken = default);

    Task<(AppRelease, AppInstaller)> GetAppLatestInstallerAsync(
        Guid appId,
        Version? osVersion,
        string language = Constants.DefaultLanguageCode,
        CancellationToken cancellationToken = default);

    Task PostAppUsageAsync(
        Guid appId,
        Version appVersion,
        Version osVersion,
        Architecture osArchiteture,
        CancellationToken cancellationToken = default);

    Task<ErrorStatus> SendAppErrorReportAsync(
        Guid appId,
        AppErrorRequest appErrorInfo,
        CancellationToken cancellationToken = default);

    Task<(AppErrorWithVersion[], int)> GetAppErrorsPageAsync(Guid appId, int from, int count, CancellationToken cancellationToken = default);

    Task<(AppRunWithVersion[], int)> GetAppRunsPageAsync(Guid appId, DateOnly to, int count, CancellationToken cancellationToken = default);

    Task UpdateInstallerAsync(Guid installerId, Guid newReleaseId, Uri newUri, CancellationToken cancellationToken = default);

    Task ResolveErrorsAsync(int[] errorIds, CancellationToken cancellationToken = default);
}
