namespace AppRegistryService.Contract.Responses;

/// <summary>
/// Defines an application installer info response.
/// </summary>
public sealed class AppInstallerReleaseInfoResponse
{
    /// <summary>
    /// Release info.
    /// </summary>
    public AppReleaseInfo? Release { get; set; }

    /// <summary>
    /// Installer info.
    /// </summary>
    public AppInstallerInfo? Installer { get; set; }
}
