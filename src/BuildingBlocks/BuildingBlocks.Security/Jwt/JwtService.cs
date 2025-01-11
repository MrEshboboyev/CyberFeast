using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BuildingBlocks.Core.Extensions;
using BuildingBlocks.Core.Utils;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using JwtRegisteredClaimNames = Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames;

namespace BuildingBlocks.Security.Jwt;

/// <summary>
/// Provides methods for generating and validating JWT tokens.
/// </summary>
public class JwtService : IJwtService
{
    private readonly JwtOptions _jwtOptions;

    /// <summary>
    /// Initializes a new instance of the <see cref="JwtService"/> class.
    /// </summary>
    /// <param name="jwtOptions">The JWT options for configuration.</param>
    public JwtService(IOptions<JwtOptions> jwtOptions)
    {
        _jwtOptions = jwtOptions.Value;
    }

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
    public GenerateTokenResult GenerateJwtToken(
        string userName,
        string email,
        string userId,
        bool? isVerified = null,
        string? fullName = null,
        string? refreshToken = null,
        IReadOnlyList<Claim>? usersClaims = null,
        IReadOnlyList<string>? rolesClaims = null,
        IReadOnlyList<string>? permissionsClaims = null
    )
    {
        if (string.IsNullOrWhiteSpace(userName))
            throw new ArgumentException("User ID claim (subject) cannot be empty.", nameof(userName));

        var now = DateTime.Now;
        var ipAddress = IpUtilities.GetIpAddress();

        var jwtClaims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.NameId, userId),
            new(JwtRegisteredClaimNames.Name, fullName ?? ""),
            new(JwtRegisteredClaimNames.Sub, userId),
            new(JwtRegisteredClaimNames.Sid, userId),
            new(JwtRegisteredClaimNames.UniqueName, userName),
            new(JwtRegisteredClaimNames.Email, email),
            new(JwtRegisteredClaimNames.GivenName, fullName ?? ""),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(
                JwtRegisteredClaimNames.Iat,
                now.ToString("yyyy-MM-ddTHH:mm:ssZ", CultureInfo.InvariantCulture)
            ),
            new(CustomClaimTypes.RefreshToken, refreshToken ?? ""),
            new(CustomClaimTypes.IpAddress, ipAddress)
        };

        if (rolesClaims?.Any() == true)
        {
            jwtClaims.AddRange(rolesClaims.Select(
                role => new Claim(ClaimTypes.Role, role.ToLower(CultureInfo.InvariantCulture))));
        }

        if (!string.IsNullOrWhiteSpace(_jwtOptions.Audience))
            jwtClaims.Add(new Claim(JwtRegisteredClaimNames.Aud, _jwtOptions.Audience));

        if (permissionsClaims?.Any() == true)
        {
            jwtClaims.AddRange(permissionsClaims.Select(
                permissionsClaim => new Claim(CustomClaimTypes.Permission,
                    permissionsClaim.ToLower(CultureInfo.InvariantCulture))));
        }

        if (usersClaims?.Any() == true)
            jwtClaims = jwtClaims.Union(usersClaims).ToList();

        _jwtOptions.SecretKey.NotBeNullOrWhiteSpace();

        var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.SecretKey));
        var signingCredentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);

        var expireTime = now.AddSeconds(_jwtOptions.TokenLifeTimeSecond == 0
            ? 300
            : _jwtOptions.TokenLifeTimeSecond);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(jwtClaims),
            Expires = expireTime,
            SigningCredentials = signingCredentials,
            Claims = jwtClaims.ConvertClaimsToDictionary(),
            Issuer = _jwtOptions.Issuer,
            Audience = _jwtOptions.Audience,
            NotBefore = now
        };

        var jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
        jwtSecurityTokenHandler.OutboundClaimTypeMap.Clear();
        var securityToken = jwtSecurityTokenHandler.CreateToken(tokenDescriptor);
        var token = jwtSecurityTokenHandler.WriteToken(securityToken);

        return new GenerateTokenResult(token, expireTime);
    }

    /// <summary>
    /// Retrieves the claims principal from the provided token.
    /// </summary>
    /// <param name="token">The JWT token.</param>
    /// <returns>The claims principal, if valid; otherwise, null.</returns>
    public ClaimsPrincipal? GetPrincipalFromToken(string token)
    {
        token.NotBeNullOrWhiteSpace();
        _jwtOptions.SecretKey.NotBeNullOrWhiteSpace();

        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateAudience = true,
            ValidateIssuer = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.SecretKey)),
            ValidateLifetime = false,
            ClockSkew = TimeSpan.Zero,
        };

        var tokenHandler = new JwtSecurityTokenHandler();

        var principal = tokenHandler.ValidateToken(
            token,
            tokenValidationParameters,
            out var securityToken
        );

        if (securityToken is not JwtSecurityToken jwtSecurityToken
            || jwtSecurityToken.Header.Alg != SecurityAlgorithms.HmacSha256)
        {
            throw new SecurityTokenException("Invalid access token.");
        }

        return principal;
    }
}

/// <summary>
/// Provides helper methods for working with JWT tokens.
/// </summary>
public static class JwtHelper
{
    /// <summary>
    /// Converts a list of claims to a dictionary.
    /// </summary>
    /// <param name="claims">The list of claims.</param>
    /// <returns>A dictionary representation of the claims.</returns>
    public static IDictionary<string, object> ConvertClaimsToDictionary(this IList<Claim> claims)
    {
        return claims.ToDictionary(claim => claim.Type, claim => (object)claim.Value);
    }
}