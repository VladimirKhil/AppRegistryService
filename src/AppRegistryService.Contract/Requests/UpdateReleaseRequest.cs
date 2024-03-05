namespace AppRegistryService.Contract.Requests;

/// <summary>
/// Defines a request to update release.
/// </summary>
public sealed class UpdateReleaseRequest
{
    /// <summary>
    /// Is this release mandatory.
    /// </summary>
    public bool IsMandatory { get; set; }
}
