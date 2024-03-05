using AppRegistryService.Contract.Models;
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

    [HttpPatch("releases/{releaseId}")]
    public Task UpdateReleaseAsync(Guid releaseId, UpdateReleaseRequest request, CancellationToken cancellationToken = default) =>
        _appsService.UpdateReleaseAsync(releaseId, request.IsMandatory, cancellationToken);

    [HttpPost("apps/{appId}/usage")]
    public async Task<AppInstallerReleaseInfoResponse> PostAppUsageAsync(
        Guid appId,
        AppUsageInfo appUsageInfo,
        [FromHeader(Name = "Accept-Language")] string acceptLanguage = Constants.DefaultLanguageCode,
        CancellationToken cancellationToken = default)
    {
        var (release, installer) = await _appsService.GetAppLatestInstallerAsync(
            appId,
            appUsageInfo.OSVersion,
            CultureHelper.GetLanguageFromAcceptLanguageHeader(acceptLanguage),
            cancellationToken);

        var appVersion = appUsageInfo.AppVersion;

        if (appVersion.Major == 0 && appVersion.Minor == 0)
        {
            if (release != null)
            {
                appVersion = VersionHelper.CreateVersion(release.Version);
            }
        }

        await _appsService.PostAppUsageAsync(appId, appVersion, appUsageInfo.OSVersion, appUsageInfo.OSArchitecture, cancellationToken);

        return new AppInstallerReleaseInfoResponse
        {
            Release = _mapper.Map<AppReleaseInfo>(release),
            Installer = _mapper.Map<AppInstallerInfo>(installer)
        };
    }

    [HttpPost("apps/{appId}/errors")]
    public async Task<SendAppErrorResponse> SendAppErrorReportAsync(
        Guid appId,
        AppErrorRequest appErrorInfo,
        CancellationToken cancellationToken = default)
    {
        var errorStatus = await _appsService.SendAppErrorReportAsync(appId, appErrorInfo, cancellationToken);
        return new SendAppErrorResponse(_mapper.Map<ErrorStatus>(errorStatus));
    }

    [HttpPatch("installers/{installerId}")]
    public Task UpdateInstallerAsync(Guid installerId, UpdateInstallerRequest request, CancellationToken cancellationToken = default) =>
        _appsService.UpdateInstallerAsync(installerId, request.ReleaseId, new Uri(request.Uri), cancellationToken);

    [HttpPost("errors/resolve")]
    public Task ResolveErrorsAsync(ResolveErrorsRequest request, CancellationToken cancellationToken = default) =>
        _appsService.ResolveErrorsAsync(request.ErrorIds, cancellationToken);

    private static AppRunInfo ConvertToRunInfo(AppRunWithVersion source) => new(
        source.Run.Date,
        VersionHelper.CreateVersion(source.Version),
        VersionHelper.CreateVersion(source.Run.OSVersion),
        source.Run.OSArchitecture ?? Architecture.X64,
        source.Run.Count
    );

    private static AppErrorInfo ConvertToErrorInfo(AppErrorWithVersion source) => new(
        source.Error.Id,
        VersionHelper.CreateVersion(source.Version),
        VersionHelper.CreateVersion(source.Error.OSVersion),
        source.Error.OSArchitecture,
        source.Error.Time,
        source.Error.Message ?? "",
        source.Error.UserNotes,
        source.Error.Count
    );
}
