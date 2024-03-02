using System.Runtime.InteropServices;

namespace AppRegistryService.Contract.Responses;

/// <summary>
/// Defines an application run info.
/// </summary>
/// <param name="Date">Run date.</param>
/// <param name="Version">Application version.</param>
/// <param name="OSVersion">Operating system version.</param>
/// <param name="OSArchitecture">Operating system architecture.</param>
/// <param name="Count">Run count.</param>
public sealed record AppRunInfo(DateOnly Date, Version Version, Version OSVersion, Architecture OSArchitecture, int Count);
