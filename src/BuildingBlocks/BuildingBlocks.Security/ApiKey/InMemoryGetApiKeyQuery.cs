using BuildingBlocks.Security.ApiKey.Authorization;

namespace BuildingBlocks.Security.ApiKey;

/// <summary>
/// Provides an in-memory storage for API keys and implements the <see cref="IGetApiKeyQuery"/> interface.
/// </summary>
public class InMemoryGetApiKeyQuery : IGetApiKeyQuery
{
    private readonly Dictionary<string, ApiKey> _apiKeys;

    /// <summary>
    /// Initializes a new instance of the <see cref="InMemoryGetApiKeyQuery"/> class with predefined API keys.
    /// </summary>
    public InMemoryGetApiKeyQuery()
    {
        var existingApiKeys = new List<ApiKey>
        {
            new(
                1,
                "Customer1",
                "C5BFF7F0-B4DF-475E-A331-F737424F013C",
                new DateTime(2021, 01, 01),
                new List<string> { Roles.Customer }
            ),
            new(
                2,
                "Admin1",
                "5908D47C-85D3-4024-8C2B-6EC9464398AD",
                new DateTime(2021, 01, 01),
                new List<string> { Roles.Admin, Roles.Customer, Roles.ThirdParty }
            ),
            new(
                3,
                "Third Party1",
                "06795D9D-A770-44B9-9B27-03C6ABDB1BAE",
                new DateTime(2021, 01, 01),
                new List<string> { Roles.ThirdParty }
            )
        };

        _apiKeys = existingApiKeys.ToDictionary(x => x.Key, x => x);
    }

    /// <summary>
    /// Retrieves the API key based on the provided API key string.
    /// </summary>
    /// <param name="providedApiKey">The provided API key string.</param>
    /// <returns>The corresponding <see cref="ApiKey"/> if found; otherwise, null.</returns>
    public Task<ApiKey?> ExecuteAsync(string providedApiKey)
    {
        _apiKeys.TryGetValue(providedApiKey, out var key);
        return Task.FromResult(key);
    }
}