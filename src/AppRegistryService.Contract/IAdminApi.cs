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
    /// Marks errors as resolved by latest app release.
    /// </summary>
    /// <param name="request">Resolved errors request.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task ResolveErrorsAsync(ResolveErrorsRequest request, CancellationToken cancellationToken = default);
}
