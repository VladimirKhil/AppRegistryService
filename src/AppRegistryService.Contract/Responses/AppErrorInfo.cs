namespace AppRegistryService.Contract.Responses;

/// <summary>
/// Defines application error.
/// </summary>
/// <param name="Id">Unique error id.</param>
/// <param name="AppVersion">Application release version.</param>
/// <param name="OSVersion">Client operating system version (for the first appearance).</param>
/// <param name="ErrorTime">Error first appearance time.</param>
/// <param name="ErrorMessage">Error text.</param>
/// <param name="Count"> Error count.</param>
public sealed record AppErrorInfo(int Id, Version AppVersion, Version OSVersion, DateTimeOffset ErrorTime, string ErrorMessage, int Count);
