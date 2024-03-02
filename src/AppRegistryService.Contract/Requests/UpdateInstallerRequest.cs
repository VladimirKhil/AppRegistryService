namespace AppRegistryService.Contract.Requests;

/// <summary>
/// Defines a request to update installer.
/// </summary>
/// <param name="ReleaseId">New release identifier.</param>
/// <param name="Uri">New download uri.</param>
public sealed record UpdateInstallerRequest(Guid ReleaseId, string Uri);
