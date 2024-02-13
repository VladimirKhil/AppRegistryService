using AppRegistry.Database.Models;

namespace AppRegistryService.Contracts;

public interface IFamiliesService
{
    Task<AppFamily[]> GetFamiliesAsync(CancellationToken cancellationToken = default);

    Task<App[]> GetFamilyAppsAsync(Guid appFamilyId, CancellationToken cancellationToken = default);
}
