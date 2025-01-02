namespace AppRegistryService.Contract.Responses;

/// <summary>
/// Defines applications screenshots response.
/// </summary>
/// <param name="ScreenshotUris">Screenshots uris.</param>
public sealed record ScreenshotsResponse(string[] ScreenshotUris);
