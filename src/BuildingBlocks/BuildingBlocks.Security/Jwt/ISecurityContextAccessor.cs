namespace BuildingBlocks.Security.Jwt;

/// <summary>
/// Provides access to security-related information.
/// </summary>
public interface ISecurityContextAccessor
{
    /// <summary>
    /// Gets the user ID of the authenticated user.
    /// </summary>
    string? UserId { get; }

    /// <summary>
    /// Gets the role of the authenticated user.
    /// </summary>
    string Role { get; }

    /// <summary>
    /// Gets the JWT token of the authenticated user.
    /// </summary>
    string? JwtToken { get; }

    /// <summary>
    /// Gets a value indicating whether the user is authenticated.
    /// </summary>
    bool IsAuthenticated { get; }
}