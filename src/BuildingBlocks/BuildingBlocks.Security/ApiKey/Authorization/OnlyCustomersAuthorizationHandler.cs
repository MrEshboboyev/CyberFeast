using Microsoft.AspNetCore.Authorization;

namespace BuildingBlocks.Security.ApiKey.Authorization;

/// <summary>
/// Handles the <see cref="OnlyCustomersRequirement"/> authorization requirement.
/// </summary>
public class OnlyCustomersAuthorizationHandler : AuthorizationHandler<OnlyCustomersRequirement>
{
    /// <summary>
    /// Handles the authorization requirement by checking if the user is in the "Customer" role.
    /// </summary>
    /// <param name="context">The authorization handler context.</param>
    /// <param name="requirement">The requirement to handle.</param>
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        OnlyCustomersRequirement requirement
    )
    {
        if (context.User.IsInRole(Roles.Customer))
            context.Succeed(requirement);

        return Task.CompletedTask;
    }
}