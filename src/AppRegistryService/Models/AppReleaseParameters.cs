using AppRegistry.Database.Models;

namespace AppRegistryService.Models;

/// <summary>
/// Defines a new application release.
/// </summary>
/// <param name="Version">Release version.</param>
/// <param name="MinimumOSVersion">Mimumim supported operating system version.</param>
/// <param name="Notes">Release notes.</param>
/// <param name="Level">Release level.</param>
/// <param name="LocalizedNotes">Localized release notes.</param>
public sealed record AppReleaseParameters(Version Version, Version MinimumOSVersion, string Notes, ReleaseLevel Level, Dictionary<string, string>? LocalizedNotes = null);
