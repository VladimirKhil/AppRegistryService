namespace AppRegistryService.Contract.Responses;

/// <summary>
/// Defines an application release installer.
/// </summary>
public sealed class AppInstallerInfo
{
    /// <summary>
    /// Unique installer identifier.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Release identifier link.
    /// </summary>
    public Guid ReleaseId { get; set; }

    /// <summary>
    /// Installer file uri.
    /// </summary>
    public Uri? Uri { get; set; }

    /// <summary>
    /// Installer title.
    /// </summary>
    public string? Title { get; set; }

    /// <summary>
    /// Installer description.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Installer file size in bytes.
    /// </summary>
    public long? Size { get; set; }

    /// <summary>
    /// Installer additional (optional) files size in bytes.
    /// </summary>
    public long? AdditionalSize { get; set; }
}
