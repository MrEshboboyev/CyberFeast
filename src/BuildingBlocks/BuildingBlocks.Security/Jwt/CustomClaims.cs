namespace BuildingBlocks.Security.Jwt;

/// <summary>
/// Defines custom claim types used in the application.
/// </summary>
public static class CustomClaimTypes
{
    /// <summary>
    /// Claim type for permissions.
    /// </summary>
    public const string Permission = "permission";

    /// <summary>
    /// Claim type for IP addresses.
    /// </summary>
    public const string IpAddress = "ipAddress";

    /// <summary>
    /// Claim type for refresh tokens.
    /// </summary>
    public const string RefreshToken = "refreshToken";
}