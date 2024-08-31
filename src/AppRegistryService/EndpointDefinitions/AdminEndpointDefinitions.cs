using AppRegistryService.Contract.Requests;
using AppRegistryService.Contract.Responses;
using AppRegistryService.Contracts;
using AppRegistryService.Helpers;
using AppRegistryService.Models;
using Microsoft.AspNetCore.Mvc;
using System.Runtime.InteropServices;

namespace AppRegistryService.EndpointDefinitions;

internal static class AdminEndpointDefinitions
{
    public static void DefineAdminEndpoints(this WebApplication app)
    {
        app.MapGet(
            "/api/v1/admin/apps/{appId}/errors",
            async (IAppsService appsService,
                Guid appId,
                int from,
                int count,
                CancellationToken cancellationToken = default) =>
        {
            var (errors, total) = await appsService.GetAppErrorsPageAsync(appId, from, count, cancellationToken);
            return new ResultsPage<AppErrorInfo> { Results = errors.Select(ConvertToErrorInfo).ToArray(), Total = total };
        });

        app.MapGet(
            "/api/v1/admin/apps/{appId}/runs",
            async (IAppsService appsService,
                Guid appId,
                DateOnly to,
                int count,
                CancellationToken cancellationToken = default) =>
        {
            var (runs, total) = await appsService.GetAppRunsPageAsync(appId, to, count, cancellationToken);
            return new ResultsPage<AppRunInfo> { Results = runs.Select(ConvertToRunInfo).ToArray(), Total = total };
        });

        app.MapPost(
            "/api/v1/admin/apps/{appId}/releases",
            async (IAppsService appsService,
                Guid appId,
                AppReleaseRequest request,
                CancellationToken cancellationToken = default) =>
        {
            var parameters = new AppReleaseParameters(
                request.Version,
                request.MinimumOSVersion,
                request.Notes,
                request.Level.ToReleaseLevel());

            return new PublishAppReleaseResponse(await appsService.PublishAppReleaseAsync(appId, parameters, cancellationToken));
        });

        app.MapPatch(
            "/api/v1/admin/releases/{releaseId}",
            async (IAppsService appsService,
                Guid releaseId,
                UpdateReleaseRequest request,
                CancellationToken cancellationToken = default) =>
        {
            await appsService.UpdateReleaseAsync(releaseId, request.IsMandatory, cancellationToken);
            return Results.NoContent();
        });

        app.MapPost(
            "/api/v1/admin/apps/{appId}/usage",
            async (IAppsService appsService,
                Guid appId,
                AppUsageInfo appUsageInfo,
                [FromHeader(Name = "Accept-Language")] string acceptLanguage = Constants.DefaultLanguageCode,
                CancellationToken cancellationToken = default) =>
        {
            var (release, installer) = await appsService.GetAppLatestInstallerAsync(
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

            await appsService.TryPostAppUsageAsync(appId, appVersion, appUsageInfo.OSVersion, appUsageInfo.OSArchitecture, cancellationToken);

            return new AppInstallerReleaseInfoResponse
            {
                Release = release.ToAppReleaseInfo(),
                Installer = installer.ToAppInstallerInfo()
            };
        });

        app.MapPost(
            "/api/v1/admin/apps/{appId}/errors",
            async (IAppsService appsService,
                Guid appId,
                AppErrorRequest appErrorInfo,
                CancellationToken cancellationToken = default) =>
        {
            var errorStatus = await appsService.SendAppErrorReportAsync(appId, appErrorInfo, cancellationToken);
            return new SendAppErrorResponse(errorStatus.ToContractErrorStatus());
        });

        app.MapPatch(
            "/api/v1/admin/installers/{installerId}",
            async (IAppsService appsService,
                Guid installerId,
                UpdateInstallerRequest request,
                CancellationToken cancellationToken = default) =>
        {
            await appsService.UpdateInstallerAsync(installerId, request.ReleaseId, new Uri(request.Uri), cancellationToken);
            return Results.NoContent();
        });

        app.MapPost(
            "/api/v1/admin/errors/resolve",
            async (IAppsService appsService,
                ResolveErrorsRequest request,
                CancellationToken cancellationToken = default) =>
        {
            await appsService.ResolveErrorsAsync(request.ErrorIds, cancellationToken);
            return Results.NoContent();
        });

        static AppRunInfo ConvertToRunInfo(AppRunWithVersion source) => new(
            source.Run.Date,
            VersionHelper.CreateVersion(source.Version),
            VersionHelper.CreateOSVersion(source.Run.OSVersion),
            source.Run.OSArchitecture ?? Architecture.X64,
            source.Run.Count
        );

        static AppErrorInfo ConvertToErrorInfo(AppErrorWithVersion source) => new(
            source.Error.Id,
            VersionHelper.CreateVersion(source.Version),
            VersionHelper.CreateOSVersion(source.Error.OSVersion),
            source.Error.OSArchitecture,
            source.Error.Time,
            source.Error.Message ?? "",
            source.Error.UserNotes,
            source.Error.Count
        );
    }
}
