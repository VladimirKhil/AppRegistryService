using AppRegistryService.Contract.Models;
using AppRegistryService.Contract.Requests;
using AppRegistryService.Contract.Responses;

namespace AppRegistryService.Contract;

/// <summary>
/// Provides common application API.
/// </summary>
public interface IAppsApi
{
    /// <summary>
    /// Gets application screenshots.
    /// </summary>
    /// <param name="appId">Application identifier.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task<ScreenshotsResponse?> GetAppScreenshotsAsync(Guid appId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets application releases collection in reversed chronological order (latest first) with paging support.
    /// </summary>
    /// <param name="appId">Application identifier.</param>
    /// <param name="from">The reversed chronological index of the first release to return.</param>
    /// <param name="count">Number of releases to return.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Releases page.</returns>
    Task<ResultsPage<AppReleaseInfo>?> GetAppReleasesPageAsync(
        Guid appId,
        int from,
        int count,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets application installers info.
    /// </summary>
    /// <param name="appId">Application identifier.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Installers and corresponding releases info.</returns>
    Task<AppInstallerReleaseInfoResponse[]?> GetAppInstallersAsync(
        Guid appId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets application latest installer info for the provided operating system version.
    /// </summary>
    /// <param name="appId">Application identifier.</param>
    /// <param name="osVersion">Client operating system version. Current OS version by default.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Installer and corresponding release info.</returns>
    Task<AppInstallerReleaseInfoResponse?> GetAppLatestInstallerAsync(
        Guid appId,
        Version? osVersion = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Posts application usage info and receives latest installer info for this application.
    /// </summary>
    /// <param name="appId">Application identifier.</param>
    /// <param name="appUsageInfo">Application usage info.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Installer and corresponding release info.</returns>
    Task<AppInstallerReleaseInfoResponse?> PostAppUsageAsync(
        Guid appId,
        AppUsageInfo appUsageInfo,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Sends application error information.
    /// </summary>
    /// <param name="appId">Application identifier.</param>
    /// <param name="appErrorRequest">Application error information.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Object containing the current status of this error.</returns>
    Task<ErrorStatus?> SendAppErrorReportAsync(
        Guid appId,
        AppErrorRequest appErrorRequest,
        CancellationToken cancellationToken = default);
}
