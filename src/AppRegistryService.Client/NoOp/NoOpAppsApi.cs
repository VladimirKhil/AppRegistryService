using AppRegistryService.Contract;
using AppRegistryService.Contract.Models;
using AppRegistryService.Contract.Requests;
using AppRegistryService.Contract.Responses;

namespace AppRegistryService.Client.NoOp;

internal sealed class NoOpAppsApi : IAppsApi
{
    public Task<ScreenshotsResponse?> GetAppScreenshotsAsync(Guid appId, CancellationToken cancellationToken = default) =>
        Task.FromResult<ScreenshotsResponse?>(new ScreenshotsResponse(Array.Empty<string>()));

    public Task<ResultsPage<AppReleaseInfo>?> GetAppReleasesPageAsync(
        Guid appId,
        int from,
        int count,
        CancellationToken cancellationToken = default) =>
        Task.FromResult((ResultsPage<AppReleaseInfo>?)new ResultsPage<AppReleaseInfo>());

    public Task<AppInstallerReleaseInfoResponse[]?> GetAppInstallersAsync(Guid appId, CancellationToken cancellationToken = default) =>
        Task.FromResult((AppInstallerReleaseInfoResponse[]?)Array.Empty<AppInstallerReleaseInfoResponse>());

    public Task<AppInstallerReleaseInfoResponse?> GetAppLatestInstallerAsync(
        Guid appId,
        Version? osVersion = null,
        CancellationToken cancellationToken = default) =>
        Task.FromResult((AppInstallerReleaseInfoResponse?)new AppInstallerReleaseInfoResponse());

    public Task<AppInstallerReleaseInfoResponse?> PostAppUsageAsync(
        Guid appId, AppUsageInfo appUsageInfo, CancellationToken cancellationToken = default) =>
        Task.FromResult((AppInstallerReleaseInfoResponse?)new AppInstallerReleaseInfoResponse());

    public Task<ErrorStatus?> SendAppErrorReportAsync(Guid appId, AppErrorRequest appErrorRequest, CancellationToken cancellationToken = default) =>
        Task.FromResult<ErrorStatus?>(ErrorStatus.NotFixed);
}
