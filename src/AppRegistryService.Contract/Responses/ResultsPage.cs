namespace AppRegistryService.Contract.Responses;

/// <summary>
/// Represents a result collection page.
/// </summary>
/// <typeparam name="T">Result type.</typeparam>
public sealed class ResultsPage<T>
{
    /// <summary>
    /// Results collection.
    /// </summary>
    public T[]? Results { get; set; }

    /// <summary>
    /// Total number of results in repository.
    /// </summary>
    public int Total { get; set; }
}
