using AppRegistryService.Contract.Requests;
using AppRegistryService.Contract.Responses;
using AppRegistryService.Contracts;
using AppRegistryService.Helpers;
using AppRegistryService.Models;
using Microsoft.AspNetCore.Mvc;

namespace AppRegistryService.EndpointDefinitions;

internal static class AppEndpointDefinitions
{
    public static void DefineAppEndpoints(this WebApplication app)
    {
        app.MapGet(
            "/api/v1/apps/{appId}/screenshots",
            async (IAppsService appsService,
                Guid appId,
                CancellationToken cancellationToken = default) =>
        {
            var screenshots = await appsService.GetAppSchreenshotsAsync(appId, cancellationToken);
            return new ScreenshotsResponse(screenshots);
        });

        app.MapGet(
            "/api/v1/apps/{appId}/releases",
            async (IAppsService appsService,
                Guid appId,
                int from,
                int count,
                [FromHeader(Name = "Accept-Language")] string acceptLanguage = Constants.DefaultLanguageCode,
                CancellationToken cancellationToken = default) =>
        {
            var (releases, total) = await appsService.GetAppReleasesPageAsync(
                appId,
                from,
                count,
                CultureHelper.GetLanguageFromAcceptLanguageHeader(acceptLanguage),
                cancellationToken);
            
            return new ResultsPage<AppReleaseInfo> { Results = releases.Select(r => r.ToAppReleaseInfo()!).ToArray(), Total = total };
        });

        app.MapGet(
            "/api/v1/apps/{appId}/installers",
            async (IAppsService appsService,
                Guid appId,
                [FromHeader(Name = "Accept-Language")] string acceptLanguage = Constants.DefaultLanguageCode,
                CancellationToken cancellationToken = default) =>
        {
            var result = await appsService.GetAppInstallersAsync(appId, CultureHelper.GetLanguageFromAcceptLanguageHeader(acceptLanguage), cancellationToken);

            return result
                .Select(r => new AppInstallerReleaseInfoResponse
                {
                    Release = r.Item1.ToAppReleaseInfo(),
                    Installer = r.Item2.ToAppInstallerInfo()
                })
                .ToArray();
        });

        app.MapGet(
            "/api/v1/apps/{appId}/installers/latest",
            async (IAppsService appsService,
                Guid appId,
                Version? osVersion,
                [FromHeader(Name = "Accept-Language")] string acceptLanguage = Constants.DefaultLanguageCode,
                CancellationToken cancellationToken = default) =>
        {
            var (release, installer) = await appsService.GetAppLatestInstallerAsync(appId, osVersion, CultureHelper.GetLanguageFromAcceptLanguageHeader(acceptLanguage), cancellationToken);

            return new AppInstallerReleaseInfoResponse
            {
                Release = release.ToAppReleaseInfo(),
                Installer = installer.ToAppInstallerInfo()
            };
        });

        app.MapPost(
            "/api/v1/apps/{appId}/usage",
            async (IAppsService appsService,
                Guid appId,
                AppUsageInfo appUsageInfo,
                [FromHeader(Name = "Accept-Language")] string acceptLanguage = Constants.DefaultLanguageCode,
                CancellationToken cancellationToken = default) =>
        {
            await appsService.TryPostAppUsageAsync(appId, appUsageInfo.AppVersion, appUsageInfo.OSVersion, appUsageInfo.OSArchitecture, cancellationToken);
            var (release, installer) = await appsService.GetAppLatestInstallerAsync(appId, appUsageInfo.OSVersion, CultureHelper.GetLanguageFromAcceptLanguageHeader(acceptLanguage), cancellationToken);

            return new AppInstallerReleaseInfoResponse
            {
                Release = release.ToAppReleaseInfo(),
                Installer = installer.ToAppInstallerInfo()
            };
        });

        app.MapPost(
            "/api/v1/apps/{appId}/errors",
            async (IAppsService appsService,
                Guid appId,
                AppErrorRequest appErrorInfo,
                CancellationToken cancellationToken = default) =>
        {
            var errorStatus = await appsService.SendAppErrorReportAsync(appId, appErrorInfo, cancellationToken);
            return new SendAppErrorResponse(errorStatus.ToContractErrorStatus());
        });
    }
}
