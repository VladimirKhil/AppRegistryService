using AppRegistryService.Contract;
using AppRegistryService.Contract.Models;
using AppRegistryService.Contract.Requests;
using AppRegistryService.Contract.Responses;
using System.Net.Http.Json;

namespace AppRegistryService.Client;

internal sealed class AppsApi : IAppsApi
{
    private readonly HttpClient _client;

    public AppsApi(HttpClient client) => _client = client;

    public Task<ScreenshotsResponse?> GetAppScreenshotsAsync(Guid appId, CancellationToken cancellationToken = default) =>
        _client.GetFromJsonAsync<ScreenshotsResponse>($"apps/{appId}/screenshots", cancellationToken);

    public Task<ResultsPage<AppReleaseInfo>?> GetAppReleasesPageAsync(
        Guid appId,
        int from,
        int count,
        CancellationToken cancellationToken = default) =>
        _client.GetFromJsonAsync<ResultsPage<AppReleaseInfo>>(
            $"apps/{appId}/releases?from={from}&count={count}",
            cancellationToken);

    public Task<AppInstallerReleaseInfoResponse[]?> GetAppInstallersAsync(Guid appId, CancellationToken cancellationToken = default) =>
        _client.GetFromJsonAsync<AppInstallerReleaseInfoResponse[]>(
            $"apps/{appId}/installers",
            cancellationToken);

    public Task<AppInstallerReleaseInfoResponse?> GetAppLatestInstallerAsync(
        Guid appId,
        Version? osVersion,
        CancellationToken cancellationToken = default) =>
        _client.GetFromJsonAsync<AppInstallerReleaseInfoResponse>(
            $"apps/{appId}/installers/latest?osVersion={osVersion ?? Environment.OSVersion.Version}",
            cancellationToken);

    public async Task<AppInstallerReleaseInfoResponse?> PostAppUsageAsync(
        Guid appId,
        AppUsageInfo appUsageInfo,
        CancellationToken cancellationToken = default)
    {
        using var response = await _client.PostAsJsonAsync($"apps/{appId}/usage", appUsageInfo, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            throw new HttpRequestException(await response.Content.ReadAsStringAsync(cancellationToken), null, response.StatusCode);
        }

        var appResponse = await response.Content.ReadFromJsonAsync<AppInstallerReleaseInfoResponse>(cancellationToken: cancellationToken);
        return appResponse;
    }

    public async Task<ErrorStatus?> SendAppErrorReportAsync(Guid appId, AppErrorRequest appErrorInfo, CancellationToken cancellationToken = default)
    {
        using var response = await _client.PostAsJsonAsync($"apps/{appId}/errors", appErrorInfo, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            throw new HttpRequestException(await response.Content.ReadAsStringAsync(cancellationToken), null, response.StatusCode);
        }

        var errorResponse = await response.Content.ReadFromJsonAsync<SendAppErrorResponse>(cancellationToken: cancellationToken);

        return errorResponse?.ErrorStatus;
    }
}
