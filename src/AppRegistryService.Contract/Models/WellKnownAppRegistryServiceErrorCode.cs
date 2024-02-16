namespace AppRegistryService.Contract.Models;

/// <summary>
/// Defines well-known AppRegistry service error codes.
/// </summary>
public enum WellKnownAppRegistryServiceErrorCode
{
    /// <summary>
    /// Application family not found.
    /// </summary>
    AppFamilyNotFound,

    /// <summary>
    /// Application release not found.
    /// </summary>
    AppReleaseNotFound,

    /// <summary>
    /// Language not found.
    /// </summary>
    LanguageNotFound,
}
