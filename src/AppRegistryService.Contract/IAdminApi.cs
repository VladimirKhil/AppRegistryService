using AppRegistryService.Contract.Models;
using AppRegistryService.Contract.Requests;
using AppRegistryService.Contract.Responses;

namespace AppRegistryService.Contract;

/// <summary>
/// Provides admin API.
/// </summary>
public interface IAdminApi
{
    /// <summary>
    /// Gets application unresolved errors.
    /// </summary>
    /// <param name="appId">Application identifier.</param>
    /// <param name="from">First error index to return.</param>
    /// <param name="count">Number of errors to return.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task<ResultsPage<AppErrorInfo>?> GetAppErrorsPageAsync(
        Guid appId,
        int from,
        int count,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets application runs statistics in reversed chronological order.
    /// </summary>
    /// <param name="appId">Application identifier.</param>
    /// <param name="to">Last date to return.</param>
    /// <param name="count">Number of dates to return.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task<ResultsPage<AppRunInfo>?> GetAppRunsPageAsync(
        Guid appId,
        DateOnly to,
        int count,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Publishes new application release.
    /// </summary>
    /// <param name="appId">Application identifier.</param>
    /// <param name="request">Release info.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Published release identifier.</returns>
    Task<PublishAppReleaseResponse?> PublishAppReleaseAsync(Guid appId, AppReleaseRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates application release.
    /// </summary>
    /// <param name="releaseId">Release identifier.</param>
    /// <param name="updateReleaseRequest">Update request.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task UpdateReleaseAsync(Guid releaseId, UpdateReleaseRequest updateReleaseRequest, CancellationToken cancellationToken = default);

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
    /// Updates application installer.
    /// </summary>
    /// <param name="installerId">Installer identifier.</param>
    /// <param name="updateInstallerRequest">Update request.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task UpdateInstallerAsync(Guid installerId, UpdateInstallerRequest updateInstallerRequest, CancellationToken cancellationToken = default);

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

    /// <summary>
    /// Marks errors as resolved by latest app release.
    /// </summary>
    /// <param name="request">Resolved errors request.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task ResolveErrorsAsync(ResolveErrorsRequest request, CancellationToken cancellationToken = default);
}
