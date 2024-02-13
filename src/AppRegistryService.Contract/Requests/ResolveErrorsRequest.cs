namespace AppRegistryService.Contract.Requests;

/// <summary>
/// Defines a resolve errors request.
/// </summary>
/// <param name="ErrorIds">Identifiers of errors to resolve.</param>
public sealed record ResolveErrorsRequest(int[] ErrorIds);
