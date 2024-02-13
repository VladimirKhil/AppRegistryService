using AppRegistryService.Contract.Requests;
using AppRegistryService.Contract.Responses;
using AppRegistryService.Contracts;
using AppRegistryService.Helpers;
using AppRegistryService.Models;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using System.Runtime.InteropServices;

namespace AppRegistryService.Controllers;

[Route("api/v1/admin")]
[ApiController]
public sealed class AdminController : ControllerBase
{
    private readonly IAppsService _appsService;
    private readonly IMapper _mapper;

    public AdminController(IAppsService appsService, IMapper mapper)
    {
        _appsService = appsService;
        _mapper = mapper;
    }

    [HttpGet("apps/{appId}/errors")]
    public async Task<ResultsPage<AppErrorInfo>?> GetAppErrorsPageAsync(Guid appId, int from, int count, CancellationToken cancellationToken = default)
    {
        var (errors, total) = await _appsService.GetAppErrorsPageAsync(appId, from, count, cancellationToken);
        return new ResultsPage<AppErrorInfo> { Results = errors.Select(ConvertToErrorInfo).ToArray(), Total = total };
    }

    [HttpGet("apps/{appId}/runs")]
    public async Task<ResultsPage<AppRunInfo>?> GetAppRunsPageAsync(Guid appId, DateOnly to, int count, CancellationToken cancellationToken = default)
    {
        var (runs, total) = await _appsService.GetAppRunsPageAsync(appId, to, count, cancellationToken);
        return new ResultsPage<AppRunInfo> { Results = runs.Select(ConvertToRunInfo).ToArray(), Total = total };
    }

    [HttpPost("apps/{appId}/releases")]
    public async Task<PublishAppReleaseResponse?> PublishAppReleaseAsync(
        Guid appId,
        AppReleaseRequest request,
        CancellationToken cancellationToken = default)
    {
        var parameters = new AppReleaseParameters(
            request.Version,
            request.MinimumOSVersion,
            request.Notes,
            _mapper.Map<AppRegistry.Database.Models.ReleaseLevel>(request.Level));

        return new PublishAppReleaseResponse(await _appsService.PublishAppReleaseAsync(appId, parameters, cancellationToken));
    }

    [HttpPost("errors/resolve")]
    public Task ResolveErrorsAsync(ResolveErrorsRequest request, CancellationToken cancellationToken = default) =>
        _appsService.ResolveErrorsAsync(request.ErrorIds, cancellationToken);

    private static AppRunInfo ConvertToRunInfo(AppRunWithVersion source) => new(
        source.Run.Date,
        VersionHelper.CreateVersion(source.Version),
        VersionHelper.CreateVersion(source.Run.OSVersion),
        source.Run.OSArhitecture ?? Architecture.X64,
        source.Run.Count
    );

    private static AppErrorInfo ConvertToErrorInfo(AppErrorWithVersion source) => new(
        source.Error.Id,
        VersionHelper.CreateVersion(source.Version),
        VersionHelper.CreateVersion(source.Error.OSVersion),
        source.Error.Time,
        source.Error.Message ?? "",
        source.Error.Count
    );
}
