namespace BuildingBlocks.Security.ApiKey;

/// <summary>
/// Defines a method for executing a query to retrieve an API key based on the provided key string.
/// </summary>
public interface IGetApiKeyQuery
{
    /// <summary>
    /// Executes the query to retrieve the API key.
    /// </summary>
    /// <param name="providedApiKey">The provided API key string.</param>
    /// <returns>The corresponding <see cref="ApiKey"/> if found; otherwise, null.</returns>
    Task<ApiKey?> ExecuteAsync(string providedApiKey);
}