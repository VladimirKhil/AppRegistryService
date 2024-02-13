using AppRegistryService.Contract;

namespace AppRegistryService.Client;

/// <inheritdoc cref="IAppRegistryServiceClient" />
internal sealed class AppRegistryServiceClient : IAppRegistryServiceClient
{
    public IFamiliesApi Families { get; }

    public IAppsApi Apps { get; }

    public IAdminApi Admin { get; }

    public AppRegistryServiceClient(HttpClient client)
    {
        Families = new FamiliesApi(client);
        Apps = new AppsApi(client);
        Admin = new AdminApi(client);
    }
}
