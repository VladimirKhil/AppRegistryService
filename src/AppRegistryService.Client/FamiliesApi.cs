using AppRegistryService.Contract;
using AppRegistryService.Contract.Responses;
using System.Net.Http.Json;

namespace AppRegistryService.Client;

internal sealed class FamiliesApi : IFamiliesApi
{
    private readonly HttpClient _client;

    public FamiliesApi(HttpClient client) => _client = client;

    public Task<AppFamilyInfo[]?> GetFamiliesAsync(CancellationToken cancellationToken = default) =>
        _client.GetFromJsonAsync<AppFamilyInfo[]>(
            "families",
            cancellationToken);

    public Task<AppInfo[]?> GetFamilyAppsAsync(Guid appFamilyId, CancellationToken cancellationToken = default) =>
        _client.GetFromJsonAsync<AppInfo[]>(
            $"families/{appFamilyId}/apps",
            cancellationToken);
}
