using AppRegistryService.Contract.Models;
using System.Net;

namespace AppRegistryService.Client;

/// <summary>
/// Defines an AppRegistry client exception.
/// </summary>
public sealed class AppRegistryClientException : Exception
{
    /// <summary>
    /// Error code.
    /// </summary>
    public WellKnownAppRegistryServiceErrorCode ErrorCode { get; set; }

    /// <summary>
    /// HTTP error status code.
    /// </summary>
    public HttpStatusCode StatusCode { get; set; }

    public AppRegistryClientException() { }

    public AppRegistryClientException(string message) : base(message) { }
}
