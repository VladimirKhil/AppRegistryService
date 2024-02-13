namespace AppRegistryService.Contract.Responses;

/// <summary>
/// Defines an application info.
/// </summary>
public sealed class AppInfo
{
    /// <summary>
    /// Application unique identifier.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Application family identifier link.
    /// </summary>
    public Guid AppFamilyId { get; set; }

    /// <summary>
    /// Application known issues info.
    /// </summary>
    public string? KnownIssues { get; set; }
}
