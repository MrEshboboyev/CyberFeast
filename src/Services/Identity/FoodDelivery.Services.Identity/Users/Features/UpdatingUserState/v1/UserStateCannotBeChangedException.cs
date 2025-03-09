using BuildingBlocks.Core.Exception.Types;
using BuildingBlocks.Core.Types.Extensions;
using FoodDelivery.Services.Identity.Shared.Models;
using Microsoft.AspNetCore.Http;

namespace FoodDelivery.Services.Identity.Users.Features.UpdatingUserState.v1;

internal class UserStateCannotBeChangedException(
    UserState state,
    Guid userId) : 
    AppException($"User state cannot be changed to: '{state.ToName()}' for user with ID: '{userId}'.",
    StatusCodes.Status500InternalServerError)
{
    public UserState State { get; } = state;
    public Guid UserId { get; } = userId;
}
