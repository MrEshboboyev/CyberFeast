namespace BuildingBlocks.Security.ApiKey;

/// <summary>
/// Represents an API key with properties such as ID, owner, key, creation date, and associated roles.
/// </summary>
public class ApiKey
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ApiKey"/> class.
    /// </summary>
    /// <param name="id">The ID of the API key.</param>
    /// <param name="owner">The owner of the API key.</param>
    /// <param name="key">The actual API key string.</param>
    /// <param name="created">The date the API key was created.</param>
    /// <param name="roles">The roles associated with the API key.</param>
    public ApiKey(int id, string owner, string key, DateTime created, IReadOnlyCollection<string> roles)
    {
        Id = id;
        Owner = owner ?? throw new ArgumentNullException(nameof(owner));
        Key = key ?? throw new ArgumentNullException(nameof(key));
        Created = created;
        Roles = roles ?? throw new ArgumentNullException(nameof(roles));
    }

    /// <summary>
    /// Gets the ID of the API key.
    /// </summary>
    public int Id { get; }

    /// <summary>
    /// Gets the owner of the API key.
    /// </summary>
    public string Owner { get; }

    /// <summary>
    /// Gets the actual API key string.
    /// </summary>
    public string Key { get; }

    /// <summary>
    /// Gets the date the API key was created.
    /// </summary>
    public DateTime Created { get; }

    /// <summary>
    /// Gets the roles associated with the API key.
    /// </summary>
    public IReadOnlyCollection<string> Roles { get; }
}