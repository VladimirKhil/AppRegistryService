namespace AppRegistryService.Contract;

/// <summary>
/// Provides API for working with published applications, releases and installers.
/// </summary> 
public interface IAppRegistryServiceClient
{
    /// <summary>
    /// Families API.
    /// </summary>
    IFamiliesApi Families { get; }

    /// <summary>
    /// Applications API.
    /// </summary>
    IAppsApi Apps { get; }

    /// <summary>
    /// Admin API.
    /// </summary>
    IAdminApi Admin { get; }
}
