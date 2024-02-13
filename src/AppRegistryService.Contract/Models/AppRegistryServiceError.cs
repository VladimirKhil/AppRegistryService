namespace AppRegistryService.Contract.Models;

/// <summary>
/// Defines an AppRegistry service error.
/// </summary>
public sealed class AppRegistryServiceError
{
    /// <summary>
    /// Error code.
    /// </summary>
    public WellKnownAppRegistryServiceErrorCode ErrorCode { get; set; }
}
