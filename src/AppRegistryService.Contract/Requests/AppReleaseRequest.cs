using AppRegistryService.Contract.Models;

namespace AppRegistryService.Contract.Requests;

/// <summary>
/// Defines a new application release.
/// </summary>
/// <param name="Version">Release version.</param>
/// <param name="MinimumOSVersion">Mimumim supported operating system version.</param>
/// <param name="Notes">Release notes.</param>
/// <param name="Level">Release level.</param>
public sealed record AppReleaseRequest(Version Version, Version MinimumOSVersion, string Notes, ReleaseLevel Level);
