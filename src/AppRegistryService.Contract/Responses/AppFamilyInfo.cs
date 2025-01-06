namespace AppRegistryService.Contract.Responses;

/// <summary>
/// Defines an application family info.
/// Applications in a family share the same name and logo but have separate codebases, releases and supported platforms.
/// </summary>
public sealed class AppFamilyInfo
{
    /// <summary>
    /// Unique application family identifier.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Application family name.
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// Application family description.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Application family details.
    /// </summary>
    public string? Details { get; set; }

    /// <summary>
    /// Application family logo uri.
    /// </summary>
    public Uri? LogoUri { get; set; }
}
