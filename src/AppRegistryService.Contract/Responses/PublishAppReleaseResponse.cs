namespace AppRegistryService.Contract.Responses;

/// <summary>
/// Defines a publish release response.
/// </summary>
/// <param name="AppReleaseId">Published release identifier.</param>
public sealed record PublishAppReleaseResponse(Guid AppReleaseId);
