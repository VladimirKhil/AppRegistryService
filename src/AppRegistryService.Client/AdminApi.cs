﻿using AppRegistryService.Contract;
using AppRegistryService.Contract.Models;
using AppRegistryService.Contract.Requests;
using AppRegistryService.Contract.Responses;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace AppRegistryService.Client;

internal sealed class AdminApi : IAdminApi
{
    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        Converters =
        {
            new JsonStringEnumConverter(JsonNamingPolicy.CamelCase)
        }
    };

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
            throw await GetErrorAsync(response, cancellationToken);
        }

        var releaseResponse = await response.Content.ReadFromJsonAsync<PublishAppReleaseResponse>(cancellationToken: cancellationToken);
        return releaseResponse;
    }

    public async Task<AppInstallerReleaseInfoResponse?> PostAppUsageAsync(
        Guid appId,
        AppUsageInfo appUsageInfo,
        CancellationToken cancellationToken = default)
    {
        using var response = await _client.PostAsJsonAsync($"admin/apps/{appId}/usage", appUsageInfo, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            throw await GetErrorAsync(response, cancellationToken);
        }

        var appResponse = await response.Content.ReadFromJsonAsync<AppInstallerReleaseInfoResponse>(cancellationToken: cancellationToken);
        return appResponse;
    }

    public async Task<ErrorStatus?> SendAppErrorReportAsync(Guid appId, AppErrorRequest appErrorInfo, CancellationToken cancellationToken = default)
    {
        using var response = await _client.PostAsJsonAsync($"admin/apps/{appId}/errors", appErrorInfo, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            throw await GetErrorAsync(response, cancellationToken);
        }

        var errorResponse = await response.Content.ReadFromJsonAsync<SendAppErrorResponse>(cancellationToken: cancellationToken);

        return errorResponse?.ErrorStatus;
    }

    public async Task ResolveErrorsAsync(ResolveErrorsRequest request, CancellationToken cancellationToken = default)
    {
        using var response = await _client.PostAsJsonAsync("admin/errors/resolve", request, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            throw await GetErrorAsync(response, cancellationToken);
        }
    }

    public async Task UpdateInstallerAsync(Guid installerId, UpdateInstallerRequest updateInstallerRequest, CancellationToken cancellationToken = default)
    {
        var json = JsonSerializer.Serialize(updateInstallerRequest);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        using var response = await _client.PatchAsync($"admin/installers/{installerId}", content, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            throw await GetErrorAsync(response, cancellationToken);
        }
    }

    public async Task UpdateReleaseAsync(Guid releaseId, UpdateReleaseRequest updateReleaseRequest, CancellationToken cancellationToken = default)
    {
        var json = JsonSerializer.Serialize(updateReleaseRequest);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        using var response = await _client.PatchAsync($"admin/releases/{releaseId}", content, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            throw await GetErrorAsync(response, cancellationToken);
        }
    }

    private static async Task<AppRegistryClientException> GetErrorAsync(HttpResponseMessage response, CancellationToken cancellationToken)
    {
        var serverError = await response.Content.ReadAsStringAsync(cancellationToken);

        try
        {
            var error = JsonSerializer.Deserialize<AppRegistryServiceError>(serverError, SerializerOptions);

            if (error != null)
            {
                return new AppRegistryClientException { ErrorCode = error.ErrorCode, StatusCode = response.StatusCode };
            }
        }
        catch // Invalid JSON or wrong type
        {

        }

        return new AppRegistryClientException(serverError) { StatusCode = response.StatusCode };
    }
}
