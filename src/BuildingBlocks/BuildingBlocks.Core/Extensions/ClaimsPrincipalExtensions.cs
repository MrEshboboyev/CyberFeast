using System.Security.Claims;

namespace BuildingBlocks.Core.Extensions;

/// <summary>
/// Provides extension methods for the ClaimsPrincipal class.
/// </summary>
public static class ClaimsPrincipalExtensions
{
    /// <summary>
    /// Retrieves the value of a specified claim from the ClaimsPrincipal.
    /// </summary>
    /// <param name="principal">The ClaimsPrincipal instance.</param>
    /// <param name="type">The type of the claim to retrieve.</param>
    /// <returns>The value of the specified claim.</returns>
    public static string GetClaimValue(this ClaimsPrincipal principal, string type)
    {
        return principal.FindFirst(type)!.Value;
    }
}