using AppRegistryService.Models;

namespace AppRegistryService.Helpers;

/// <summary>
/// Provides helper method for working with culture string.
/// </summary>
public static class CultureHelper
{
    /// <summary>
    /// Gets first language from Accept-Language header.
    /// </summary>
    /// <param name="acceptHeaderValue">Accept-Language header value.</param>
    /// <returns>First language from Accept-Language header value or default value.</returns>
    public static string GetLanguageFromAcceptLanguageHeader(string acceptHeaderValue)
    {
        var firstLanguage = acceptHeaderValue.Split(',').FirstOrDefault();
        return string.IsNullOrEmpty(firstLanguage) ? Constants.DefaultLanguageCode : firstLanguage;
    }
}
