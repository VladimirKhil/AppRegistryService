using AppRegistryService.Contract;

namespace AppRegistryService.Client.NoOp;

/// <summary>
/// Provides a no-op implementation for <see cref="IAppRegistryServiceClient" />.
/// It is used when AppService Uri has not been specified.
/// </summary>
/// <inheritdoc />
internal sealed class NoOpAppRegistryServiceClient : IAppRegistryServiceClient
{
    public IAppsApi Apps { get; } = new NoOpAppsApi();

    public IAdminApi Admin { get; } = new NoOpAdminApi();

    public IFamiliesApi Families { get; }
}
