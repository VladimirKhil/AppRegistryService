using AppRegistryService.Contract;
using AppRegistryService.Contract.Requests;
using AppRegistryService.Contract.Responses;
using System.Net.Http.Json;

namespace AppRegistryService.Client;

internal sealed class AdminApi : IAdminApi
{
    private readonly HttpClient _client;

    public AdminApi(HttpClient client) => _client = client;

    public Task<ResultsPage<AppErrorInfo>?> GetAppErrorsPageAsync(Guid appId, int from, int count, CancellationToken cancellationToken = default) =>
        _client.GetFromJsonAsync<ResultsPage<AppErrorInfo>>(
            $"admin/apps/{appId}/errors?from={from}&count={count}",
            cancellationToken);

    public Task<ResultsPage<AppRunInfo>?> GetAppRunsPageAsync(Guid appId, DateOnly to, int count, CancellationToken cancellationToken = default) =>
        _client.GetFromJsonAsync<ResultsPage<AppRunInfo>>(
            $"admin/apps/{appId}/runs?to={to}&count={count}",
            cancellationToken);

    public async Task<PublishAppReleaseResponse?> PublishAppReleaseAsync(Guid appId, AppReleaseRequest request, CancellationToken cancellationToken = default)
    {
        using var response = await _client.PostAsJsonAsync($"admin/apps/{appId}/releases", request, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            throw new HttpRequestException(await response.Content.ReadAsStringAsync(cancellationToken), null, response.StatusCode);
        }

        var releaseResponse = await response.Content.ReadFromJsonAsync<PublishAppReleaseResponse>(cancellationToken: cancellationToken);
        return releaseResponse;
    }

    public async Task ResolveErrorsAsync(ResolveErrorsRequest request, CancellationToken cancellationToken = default)
    {
        using var response = await _client.PostAsJsonAsync("admin/errors/resolve", request, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            throw new HttpRequestException(await response.Content.ReadAsStringAsync(cancellationToken), null, response.StatusCode);
        }
    }
}
