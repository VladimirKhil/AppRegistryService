using System.Runtime.InteropServices;

namespace AppRegistryService.Contract.Requests;

/// <summary>
/// Defines application usage info.
/// </summary>
/// <param name="AppVersion">Application version.</param>
/// <param name="OSVersion">OS version.</param>
/// <param name="OSArchitecture">OS architecture.</param>
public sealed record AppUsageInfo(Version AppVersion, Version OSVersion, Architecture OSArchitecture);
