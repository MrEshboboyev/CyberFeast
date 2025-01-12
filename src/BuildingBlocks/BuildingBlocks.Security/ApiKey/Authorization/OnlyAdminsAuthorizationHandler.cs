using Microsoft.AspNetCore.Authorization;

namespace BuildingBlocks.Security.ApiKey.Authorization;

/// <summary>
/// Handles the <see cref="OnlyAdminsRequirement"/> authorization requirement.
/// </summary>
public class OnlyAdminsAuthorizationHandler : AuthorizationHandler<OnlyAdminsRequirement>
{
    /// <summary>
    /// Handles the authorization requirement by checking if the user is in the "Admin" role.
    /// </summary>
    /// <param name="context">The authorization handler context.</param>
    /// <param name="requirement">The requirement to handle.</param>
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        OnlyAdminsRequirement requirement
    )
    {
        if (context.User.IsInRole(Roles.Admin))
            context.Succeed(requirement);

        return Task.CompletedTask;
    }
}