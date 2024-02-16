using AppRegistryService.Client.NoOp;
using AppRegistryService.Contract;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Polly;
using Polly.Extensions.Http;
using System.Net;
using System.Net.Http.Headers;
using System.Text;

namespace AppRegistryService.Client;

/// <summary>
/// Provides an extension method for adding <see cref="IAppRegistryServiceClient" /> implementation to service collection.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds <see cref="IAppRegistryServiceClient" /> implementation to service collection.
    /// </summary>
    /// <remarks>
    /// When no gameresult service Uri has been provided, ads no-op implementation.
    /// </remarks>
    /// <param name="services">Service collection.</param>
    /// <param name="configuration">App configuration.</param>
    public static IServiceCollection AddAppRegistryServiceClient(this IServiceCollection services, IConfiguration configuration)
    {
        var optionsSection = configuration.GetSection(AppRegistryServiceClientOptions.ConfigurationSectionName);
        services.Configure<AppRegistryServiceClientOptions>(optionsSection);

        var options = optionsSection.Get<AppRegistryServiceClientOptions>();

        if (options?.ServiceUri != null)
        {
            services.AddHttpClient<IAppRegistryServiceClient, AppRegistryServiceClient>(
                client =>
                {
                    var serviceUri = options.ServiceUri;
                    client.BaseAddress = serviceUri != null ? new Uri(serviceUri, "api/v1/") : null;
                    client.Timeout = options.Timeout;
                    client.DefaultRequestVersion = HttpVersion.Version20;

                    if (options.Culture != null)
                    {
                        client.DefaultRequestHeaders.AcceptLanguage.Add(new StringWithQualityHeaderValue(options.Culture));
                    }

                    SetAuthSecret(options, client);
                })
            .AddPolicyHandler(HttpPolicyExtensions
                .HandleTransientHttpError()
                .WaitAndRetryAsync(
                    options.RetryCount,
                    retryAttempt => TimeSpan.FromSeconds(Math.Pow(1.5, retryAttempt)))); ;
        }
        else
        {
            services.AddSingleton<IAppRegistryServiceClient, NoOpAppRegistryServiceClient>();
        }

        return services;
    }

    private static void SetAuthSecret(AppRegistryServiceClientOptions options, HttpClient client)
    {
        if (options.ClientSecret == null)
        {
            return;
        }

        var authHeader = Convert.ToBase64String(Encoding.ASCII.GetBytes($"admin:{options.ClientSecret}"));
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authHeader);
    }
}
