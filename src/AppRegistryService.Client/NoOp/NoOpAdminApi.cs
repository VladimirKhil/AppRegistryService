using AppRegistryService.Contract;
using AppRegistryService.Contract.Requests;
using AppRegistryService.Contract.Responses;

namespace AppRegistryService.Client.NoOp;

internal sealed class NoOpAdminApi : IAdminApi
{
    public Task<ResultsPage<AppErrorInfo>?> GetAppErrorsPageAsync(Guid appId, int from, int count, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<ResultsPage<AppRunInfo>?> GetAppRunsPageAsync(Guid appId, DateOnly to, int count, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<PublishAppReleaseResponse?> PublishAppReleaseAsync(
        Guid appId,
        AppReleaseRequest request,
        CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task ResolveErrorsAsync(ResolveErrorsRequest request, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}
