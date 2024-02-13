using AppRegistryService.Contract;
using AppRegistryService.Contract.Responses;

namespace AppRegistryService.Client.NoOp;

internal sealed class NoOpFamiliesApi : IFamiliesApi
{
    public Task<AppFamilyInfo[]?> GetFamiliesAsync(CancellationToken cancellationToken = default) =>
        Task.FromResult((AppFamilyInfo[]?)Array.Empty<AppFamilyInfo>());

    public Task<AppInfo[]?> GetFamilyAppsAsync(Guid appFamilyId, CancellationToken cancellationToken = default) =>
        Task.FromResult((AppInfo[]?)Array.Empty<AppInfo>());
}
