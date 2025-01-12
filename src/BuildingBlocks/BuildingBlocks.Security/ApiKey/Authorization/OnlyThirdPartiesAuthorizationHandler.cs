using Microsoft.AspNetCore.Authorization;

namespace BuildingBlocks.Security.ApiKey.Authorization;

/// <summary>
/// Handles the <see cref="OnlyThirdPartiesRequirement"/> authorization requirement.
/// </summary>
public class OnlyThirdPartiesAuthorizationHandler : AuthorizationHandler<OnlyThirdPartiesRequirement>
{
    /// <summary>
    /// Handles the authorization requirement by checking if the user is in the "Third Party" role.
    /// </summary>
    /// <param name="context">The authorization handler context.</param>
    /// <param name="requirement">The requirement to handle.</param>
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        OnlyThirdPartiesRequirement requirement
    )
    {
        if (context.User.IsInRole(Roles.ThirdParty))
            context.Succeed(requirement);

        return Task.CompletedTask;
    }
}