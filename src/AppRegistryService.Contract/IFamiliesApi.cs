using AppRegistryService.Contract.Responses;

namespace AppRegistryService.Contract;

/// <summary>
/// Provides application families API.
/// </summary>
public interface IFamiliesApi
{
    /// <summary>
    /// Gets all application families.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Application families.</returns>
    Task<AppFamilyInfo[]?> GetFamiliesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets applications for a family.
    /// </summary>
    /// <param name="appFamilyId">Application family identifier.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Applications in the family.</returns>
    Task<AppInfo[]?> GetFamilyAppsAsync(Guid appFamilyId, CancellationToken cancellationToken = default);
}
