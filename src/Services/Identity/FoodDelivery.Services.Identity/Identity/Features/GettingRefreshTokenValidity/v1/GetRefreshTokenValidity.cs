using BuildingBlocks.Abstractions.Queries;
using BuildingBlocks.Core.Extensions;
using FoodDelivery.Services.Identity.Shared.Data;
using Microsoft.EntityFrameworkCore;

namespace FoodDelivery.Services.Identity.Identity.Features.GettingRefreshTokenValidity.v1;

internal record GetRefreshTokenValidity(Guid UserId, string RefreshToken) : IQuery<bool>
{
    public static GetRefreshTokenValidity Of(Guid userId, string? refreshToken) =>
        new(userId.NotBeInvalid(), refreshToken.NotBeNull());
}

internal class GetRefreshTokenValidityQueryHandler(IdentityContext context)
    : IQueryHandler<GetRefreshTokenValidity, bool>
{
    public async Task<bool> Handle(
        GetRefreshTokenValidity request, 
        CancellationToken cancellationToken)
    {
        var refreshToken = await context
            .Set<Shared.Models.RefreshToken>()
            .FirstOrDefaultAsync(
                rt => rt.UserId == request.UserId && rt.Token == request.RefreshToken,
                cancellationToken
            );

        return refreshToken != null && refreshToken.IsRefreshTokenValid();
    }
}
