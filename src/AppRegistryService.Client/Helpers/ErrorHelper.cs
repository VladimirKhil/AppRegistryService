using AppRegistryService.Contract.Models;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace AppRegistryService.Client.Helpers;

internal static class ErrorHelper
{
    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        Converters =
        {
            new JsonStringEnumConverter(JsonNamingPolicy.CamelCase)
        }
    };

    internal static async Task<AppRegistryClientException> GetErrorAsync(this HttpResponseMessage response, CancellationToken cancellationToken)
    {
        var serverError = await response.Content.ReadAsStringAsync(cancellationToken);

        try
        {
            var error = JsonSerializer.Deserialize<AppRegistryServiceError>(serverError, SerializerOptions);

            if (error != null)
            {
                return new AppRegistryClientException { ErrorCode = error.ErrorCode, StatusCode = response.StatusCode };
            }
        }
        catch // Invalid JSON or wrong type
        {

        }

        return new AppRegistryClientException(serverError) { StatusCode = response.StatusCode };
    }
}
