using AppRegistryService.Contract;
using AppRegistryService.Contract.Models;
using AppRegistryService.Contract.Requests;
using AppRegistryService.Contract.Responses;

namespace AppRegistryService.Client.NoOp;

internal sealed class NoOpAdminApi : IAdminApi
{
    public Task<ResultsPage<AppErrorInfo>?> GetAppErrorsPageAsync(Guid appId, int from, int count, CancellationToken cancellationToken = default) =>
        throw new NotImplementedException();

    public Task<ResultsPage<AppRunInfo>?> GetAppRunsPageAsync(Guid appId, DateOnly to, int count, CancellationToken cancellationToken = default) =>
        throw new NotImplementedException();

    public Task<PublishAppReleaseResponse?> PublishAppReleaseAsync(
        Guid appId,
        AppReleaseRequest request,
        CancellationToken cancellationToken = default) => throw new NotImplementedException();

    public Task<AppInstallerReleaseInfoResponse?> PostAppUsageAsync(
        Guid appId, AppUsageInfo appUsageInfo, CancellationToken cancellationToken = default) =>
        Task.FromResult((AppInstallerReleaseInfoResponse?)new AppInstallerReleaseInfoResponse());

    public Task<ErrorStatus?> SendAppErrorReportAsync(Guid appId, AppErrorRequest appErrorRequest, CancellationToken cancellationToken = default) =>
        Task.FromResult<ErrorStatus?>(ErrorStatus.NotFixed);

    public Task ResolveErrorsAsync(ResolveErrorsRequest request, CancellationToken cancellationToken = default) => throw new NotImplementedException();

    public Task UpdateInstallerAsync(Guid installerId, UpdateInstallerRequest updateInstallerRequest, CancellationToken cancellationToken = default) =>
        throw new NotImplementedException();

    public Task UpdateReleaseAsync(Guid releaseId, UpdateReleaseRequest updateReleaseRequest, CancellationToken cancellationToken = default) =>
        throw new NotImplementedException();
}
