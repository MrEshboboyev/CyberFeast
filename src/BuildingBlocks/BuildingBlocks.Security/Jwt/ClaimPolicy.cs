using System.Security.Claims;

namespace BuildingBlocks.Security.Jwt;

/// <summary>
/// Represents a policy with a name and a list of claims.
/// </summary>
public class ClaimPolicy
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ClaimPolicy"/> class.
    /// </summary>
    /// <param name="name">The name of the policy.</param>
    /// <param name="claims">The list of claims associated with the policy.</param>
    public ClaimPolicy(string name, IReadOnlyList<Claim>? claims)
    {
        Name = name;
        Claims = claims ?? new List<Claim>();
    }

    /// <summary>
    /// Gets or sets the name of the policy.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Gets or sets the list of claims associated with the policy.
    /// </summary>
    public IReadOnlyList<Claim> Claims { get; set; }
}