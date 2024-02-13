namespace AppRegistryService.Client;

/// <summary>
/// Provides options for <see cref="AppRegistryServiceClient" />.
/// </summary>
public sealed class AppRegistryServiceClientOptions
{
    public const string ConfigurationSectionName = "AppRegistryServiceClient";

    public const int DefaultRetryCount = 3;

    /// <summary>
    /// AppRegistryService address.
    /// </summary>
    public Uri? ServiceUri { get; set; }

    /// <summary>
    /// Secret to access restricted API.
    /// </summary>
    public string? ClientSecret { get; set; }

    /// <summary>
    /// Retry count policy.
    /// </summary>
    public int RetryCount { get; set; } = DefaultRetryCount;

    /// <summary>
    /// Client timeout.
    /// </summary>
    public TimeSpan Timeout { get; set; } = TimeSpan.FromSeconds(300);
}
