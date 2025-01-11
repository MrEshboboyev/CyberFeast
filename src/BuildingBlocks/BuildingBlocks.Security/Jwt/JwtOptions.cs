namespace BuildingBlocks.Security.Jwt;

/// <summary>
/// Defines configuration options for JWT tokens.
/// </summary>
public class JwtOptions
{
    /// <summary>
    /// Gets or sets the algorithm used for JWT tokens.
    /// </summary>
    public string? Algorithm { get; set; }

    /// <summary>
    /// Gets or sets the issuer of the JWT tokens.
    /// </summary>
    public string? Issuer { get; set; }

    /// <summary>
    /// Gets or sets the secret key used for signing JWT tokens.
    /// </summary>
    public string? SecretKey { get; set; } = null;

    /// <summary>
    /// Gets or sets the audience of the JWT tokens.
    /// </summary>
    public string? Audience { get; set; }

    /// <summary>
    /// Gets or sets the token lifetime in seconds.
    /// </summary>
    public double TokenLifeTimeSecond { get; set; } = 300;

    /// <summary>
    /// Gets or sets a value indicating whether to check for revoked access tokens.
    /// </summary>
    public bool CheckRevokedAccessTokens { get; set; }

    /// <summary>
    /// Gets or sets the configuration options for Google external login.
    /// </summary>
    public GoogleExternalLogin? GoogleLoginConfigs { get; set; }

    /// <summary>
    /// Defines configuration options for Google external login.
    /// </summary>
    public sealed class GoogleExternalLogin
    {
        /// <summary>
        /// Gets or sets the client ID for Google external login.
        /// </summary>
        public string? ClientId { get; set; } = null;

        /// <summary>
        /// Gets or sets the client secret for Google external login.
        /// </summary>
        public string? ClientSecret { get; set; } = null;
    }
}