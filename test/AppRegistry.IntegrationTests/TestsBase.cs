using AppRegistryService.Client;
using AppRegistryService.Contract;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AppRegistry.IntegrationTests;

public abstract class TestsBase
{
    protected IAppRegistryServiceClient RegistryServiceClient { get; }

    protected IFamiliesApi FamiliesApi => RegistryServiceClient.Families;

    protected IAppsApi Apps => RegistryServiceClient.Apps;

    protected IAdminApi Admin => RegistryServiceClient.Admin;

    public TestsBase()
    {
        var builder = new ConfigurationBuilder().AddJsonFile("appsettings.json");
        var configuration = builder.Build();

        var serviceCollection = new ServiceCollection();
        serviceCollection.AddAppRegistryServiceClient(configuration);

        var serviceProvider = serviceCollection.BuildServiceProvider();
        RegistryServiceClient = serviceProvider.GetRequiredService<IAppRegistryServiceClient>();
    }
}
