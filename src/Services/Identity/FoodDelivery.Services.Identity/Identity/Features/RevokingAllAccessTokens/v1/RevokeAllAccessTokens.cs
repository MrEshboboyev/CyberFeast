using BuildingBlocks.Abstractions.Commands;
using BuildingBlocks.Core.Extensions;
using FoodDelivery.Services.Identity.Identity.Features.RevokingAccessToken.v1;
using FoodDelivery.Services.Identity.Shared.Exceptions;
using FoodDelivery.Services.Identity.Shared.Models;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace FoodDelivery.Services.Identity.Identity.Features.RevokingAllAccessTokens.v1;

internal record RevokeAllAccessTokens(string UserName) : ICommand
{
    /// <summary>
    /// RevokeAllAccessTokens with in-line validation.
    /// </summary>
    /// <param name="userName"></param>
    /// <returns></returns>
    public static RevokeAllAccessTokens Of(string? userName) => new(userName.NotBeEmptyOrNull());
}

internal class RevokeAllAccessTokenHandler(
    IdentityDbContext identityDbContext,
    IMediator mediator,
    UserManager<ApplicationUser> userManager)
    : ICommandHandler<RevokeAllAccessTokens>
{
    public async Task Handle(RevokeAllAccessTokens request, CancellationToken cancellationToken)
    {
        var appUser = await userManager.FindByNameAsync(request.UserName);
        appUser.NotBeNull(new IdentityUserNotFoundException(request.UserName));

        var tokens = identityDbContext
            .Set<AccessToken>()
            .Where(x => x.UserId == appUser.Id && x.ExpiredAt > DateTime.Now);

        foreach (var accessToken in tokens)
        {
            await mediator.Send(new RevokeAccessToken(accessToken.Token, appUser.UserName!), cancellationToken);
        }
    }
}
