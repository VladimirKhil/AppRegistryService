using System.Runtime.InteropServices;

namespace AppRegistryService.Contract.Requests;

/// <summary>
/// Contains information about application error.
/// </summary>
/// <param name="Version">Application version.</param>
/// <param name="OSVersion">Operating system version. Current OS version by default.</param>
/// <param name="OSArchitecture">Operating system architecture.</param>
public sealed record AppErrorRequest(Version Version, Version OSVersion, Architecture OSArchitecture)
{
    /// <summary>
    /// Error time.
    /// </summary>
    public DateTimeOffset ErrorTime { get; set; }

    /// <summary>
    /// Error text.
    /// </summary>
    public string ErrorMessage { get; set; } = "";

    /// <summary>
    /// User notes.
    /// </summary>
    public string? UserNotes { get; set; }
}
