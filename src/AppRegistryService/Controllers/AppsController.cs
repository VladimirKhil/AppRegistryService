using AppRegistryService.Contract.Models;
using AppRegistryService.Contract.Requests;
using AppRegistryService.Contract.Responses;
using AppRegistryService.Contracts;
using AppRegistryService.Helpers;
using AppRegistryService.Models;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace AppRegistryService.Controllers;

[Route("api/v1/apps")]
[ApiController]
public sealed class AppsController : ControllerBase
{
    private readonly IAppsService _appsService;
    private readonly IMapper _mapper;

    public AppsController(IAppsService appsService, IMapper mapper)
    {
        _appsService = appsService;
        _mapper = mapper;
    }

    [HttpGet("{appId}/screenshots")]
    public async Task<ScreenshotsResponse> GetAppScreenshotsAsync(
        Guid appId,
        CancellationToken cancellationToken = default)
    {
        var screenshots = await _appsService.GetAppSchreenshotsAsync(appId, cancellationToken);
        return new ScreenshotsResponse(screenshots);
    }

    [HttpGet("{appId}/releases")]
    public async Task<ResultsPage<AppReleaseInfo>> GetAppReleasesPageAsync(
        Guid appId,
        int from,
        int count,
        [FromHeader(Name = "Accept-Language")] string acceptLanguage = Constants.DefaultLanguageCode,
        CancellationToken cancellationToken = default)
    {
        var (releases, total) = await _appsService.GetAppReleasesPageAsync(appId, from, count, CultureHelper.GetLanguageFromAcceptLanguageHeader(acceptLanguage), cancellationToken);
        return new ResultsPage<AppReleaseInfo> { Results = _mapper.Map<AppReleaseInfo[]>(releases), Total = total };
    }

    [HttpGet("{appId}/installers")]
    public async Task<AppInstallerReleaseInfoResponse[]> GetAppInstallersAsync(
        Guid appId,
        [FromHeader(Name = "Accept-Language")] string acceptLanguage = Constants.DefaultLanguageCode,
        CancellationToken cancellationToken = default)
    {
        var result = await _appsService.GetAppInstallersAsync(appId, CultureHelper.GetLanguageFromAcceptLanguageHeader(acceptLanguage), cancellationToken);

        return result
            .Select(r => new AppInstallerReleaseInfoResponse
            {
                Release = _mapper.Map<AppReleaseInfo>(r.Item1),
                Installer = _mapper.Map<AppInstallerInfo>(r.Item2)
            })
            .ToArray();
    }

    [HttpGet("{appId}/installers/latest")]
    public async Task<AppInstallerReleaseInfoResponse> GetAppLatestInstallerAsync(
        Guid appId,
        Version? osVersion,
        [FromHeader(Name = "Accept-Language")] string acceptLanguage = Constants.DefaultLanguageCode,
        CancellationToken cancellationToken = default)
    {
        var (release, installer) = await _appsService.GetAppLatestInstallerAsync(appId, osVersion, CultureHelper.GetLanguageFromAcceptLanguageHeader(acceptLanguage), cancellationToken);

        return new AppInstallerReleaseInfoResponse
        {
            Release = _mapper.Map<AppReleaseInfo>(release),
            Installer = _mapper.Map<AppInstallerInfo>(installer) 
        };
    }

    [HttpPost("{appId}/usage")]
    public async Task<AppInstallerReleaseInfoResponse> PostAppUsageAsync(
        Guid appId,
        AppUsageInfo appUsageInfo,
        [FromHeader(Name = "Accept-Language")] string acceptLanguage = Constants.DefaultLanguageCode,
        CancellationToken cancellationToken = default)
    {
        await _appsService.PostAppUsageAsync(appId, appUsageInfo.AppVersion, appUsageInfo.OSVersion, appUsageInfo.OSArchitecture, cancellationToken);
        var (release, installer) = await _appsService.GetAppLatestInstallerAsync(appId, appUsageInfo.OSVersion, CultureHelper.GetLanguageFromAcceptLanguageHeader(acceptLanguage), cancellationToken);

        return new AppInstallerReleaseInfoResponse
        {
            Release = _mapper.Map<AppReleaseInfo>(release),
            Installer = _mapper.Map<AppInstallerInfo>(installer)
        };
    }

    [HttpPost("{appId}/errors")]
    public async Task<SendAppErrorResponse> SendAppErrorReportAsync(
        Guid appId,
        AppErrorRequest appErrorInfo,
        CancellationToken cancellationToken = default)
    {
        var errorStatus = await _appsService.SendAppErrorReportAsync(appId, appErrorInfo, cancellationToken);
        return new SendAppErrorResponse(_mapper.Map<ErrorStatus>(errorStatus));
    }
}
