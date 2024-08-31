using AppRegistryService.Contracts;
using AppRegistryService.Helpers;
using AppRegistryService.Models;
using Microsoft.AspNetCore.Mvc;

namespace AppRegistryService.EndpointDefinitions;

internal static class FamiliesEndpointDefinitions
{
    public static void DefineFamiliesEndpoints(this WebApplication app)
    {
        app.MapGet(
            "/api/v1/families",
            async (IFamiliesService familiesService,
                [FromHeader(Name = "Accept-Language")] string acceptLanguage = Constants.DefaultLanguageCode,
                CancellationToken cancellationToken = default) =>
        {
            var families = await familiesService.GetFamiliesAsync(CultureHelper.GetLanguageFromAcceptLanguageHeader(acceptLanguage), cancellationToken);
            return families.Select(f => f.ToAppFamilyInfo()).ToArray();
        });

        app.MapGet(
            "/api/v1/families/{appFamilyId}/apps",
            async (IFamiliesService familiesService,
                Guid appFamilyId,
                [FromHeader(Name = "Accept-Language")] string acceptLanguage = Constants.DefaultLanguageCode,
                CancellationToken cancellationToken = default) =>
        {
            var apps = await familiesService.GetFamilyAppsAsync(appFamilyId, CultureHelper.GetLanguageFromAcceptLanguageHeader(acceptLanguage), cancellationToken);
            var mappedApps = apps.Select(a => a.ToAppInfo()).ToArray();

            return Results.Ok(mappedApps);
        });
    }
}
