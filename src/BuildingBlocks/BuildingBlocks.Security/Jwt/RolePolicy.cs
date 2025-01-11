namespace BuildingBlocks.Security.Jwt;

/// <summary>
/// Represents a policy with a name and a list of roles.
/// </summary>
public class RolePolicy
{
    /// <summary>
    /// Initializes a new instance of the <see cref="RolePolicy"/> class.
    /// </summary>
    /// <param name="name">The name of the policy.</param>
    /// <param name="roles">The list of roles associated with the policy.</param>
    public RolePolicy(string name, IReadOnlyList<string>? roles)
    {
        Name = name;
        Roles = roles ?? new List<string>();
    }

    /// <summary>
    /// Gets or sets the name of the policy.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Gets or sets the list of roles associated with the policy.
    /// </summary>
    public IReadOnlyList<string> Roles { get; set; }
}