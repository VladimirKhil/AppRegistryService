using AppRegistryService.Contract.Models;

namespace AppRegistryService.Contract.Responses;

/// <summary>
/// Defines an application release information.
/// </summary>
public sealed class AppReleaseInfo
{
    /// <summary>
    /// Unique release identifier.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Application identifier link.
    /// </summary>
    public Guid AppId { get; set; }

    /// <summary>
    /// Release version.
    /// </summary>
    public Version? Version { get; set; }

    /// <summary>
    /// Mimumim supported operating system version.
    /// </summary>
    public Version? MinimumOSVersion { get; set; }

    /// <summary>
    /// Release publish date.
    /// </summary>
    public DateOnly? PublishDate { get; set; }

    /// <summary>
    /// Release notes.
    /// </summary>
    public string? Notes { get; set; }

    /// <summary>
    /// Release level.
    /// </summary>
    public ReleaseLevel Level { get; set; }
}
