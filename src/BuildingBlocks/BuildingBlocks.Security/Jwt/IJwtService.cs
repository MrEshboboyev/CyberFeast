using System.Security.Claims;

namespace BuildingBlocks.Security.Jwt;

/// <summary>
/// Provides methods for generating JWT tokens and retrieving claims principal from a token.
/// </summary>
public interface IJwtService
{
    /// <summary>
    /// Generates a JWT token with the specified claims.
    /// </summary>
    /// <param name="userName">The username of the user.</param>
    /// <param name="email">The email address of the user.</param>
    /// <param name="userId">The user ID.</param>
    /// <param name="isVerified">Whether the user is verified.</param>
    /// <param name="fullName">The full name of the user.</param>
    /// <param name="refreshToken">The refresh token.</param>
    /// <param name="usersClaims">Additional claims for the user.</param>
    /// <param name="rolesClaims">Claims for the user's roles.</param>
    /// <param name="permissionsClaims">Claims for the user's permissions.</param>
    /// <returns>The result of the token generation.</returns>
    GenerateTokenResult GenerateJwtToken(
        string userName,
        string email,
        string userId,
        bool? isVerified = null,
        string? fullName = null,
        string? refreshToken = null,
        IReadOnlyList<Claim>? usersClaims = null,
        IReadOnlyList<string>? rolesClaims = null,
        IReadOnlyList<string>? permissionsClaims = null
    );

    /// <summary>
    /// Retrieves the claims principal from the provided token.
    /// </summary>
    /// <param name="token">The JWT token.</param>
    /// <returns>The claims principal, if valid; otherwise, null.</returns>
    ClaimsPrincipal? GetPrincipalFromToken(string token);
}