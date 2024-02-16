using AppRegistry.Database.Models;
using AppRegistryService.Models;

namespace AppRegistryService.Contracts;

/// <summary>
/// Provides API for working with application families.
/// </summary>
public interface IFamiliesService
{
    Task<AppFamily[]> GetFamiliesAsync(string language = Constants.DefaultLanguageCode, CancellationToken cancellationToken = default);

    Task<App[]> GetFamilyAppsAsync(Guid appFamilyId, string language = Constants.DefaultLanguageCode, CancellationToken cancellationToken = default);
}
