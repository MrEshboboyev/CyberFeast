using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace BuildingBlocks.Security.Jwt;

/// <summary>
/// Provides access to security-related information using the IHttpContextAccessor and logging capabilities.
/// </summary>
public class SecurityContextAccessor : ISecurityContextAccessor
{
    private readonly ILogger<SecurityContextAccessor> _logger;
    private readonly IHttpContextAccessor _httpContextAccessor;

    /// <summary>
    /// Initializes a new instance of the <see cref="SecurityContextAccessor"/> class.
    /// </summary>
    /// <param name="httpContextAccessor">The HTTP context accessor to retrieve security information from the current HTTP context.</param>
    /// <param name="logger">The logger to use for logging.</param>
    public SecurityContextAccessor(
        IHttpContextAccessor httpContextAccessor,
        ILogger<SecurityContextAccessor> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
    }

    /// <summary>
    /// Gets the user ID of the authenticated user.
    /// </summary>
    public string? UserId
    {
        get
        {
            var userId = _httpContextAccessor
                .HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            return userId;
        }
    }

    /// <summary>
    /// Gets the JWT token of the authenticated user.
    /// </summary>
    public string? JwtToken => _httpContextAccessor
        .HttpContext?.Request.Headers.Authorization.ToString()?.Replace("Bearer ", "");

    /// <summary>
    /// Gets a value indicating whether the user is authenticated.
    /// </summary>
    public bool IsAuthenticated => _httpContextAccessor
        .HttpContext?.User?.Identity?.IsAuthenticated ?? false;

    /// <summary>
    /// Gets the role of the authenticated user.
    /// </summary>
    public string Role
    {
        get
        {
            var role = _httpContextAccessor
                .HttpContext?.User.FindFirst(ClaimTypes.Role)?.Value;

            return role ?? string.Empty;
        }
    }
}