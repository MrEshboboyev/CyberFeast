using BuildingBlocks.Core.Exception.Types;
using FoodDelivery.Services.Identity.Shared.Models;
using Microsoft.AspNetCore.Http;

namespace FoodDelivery.Services.Identity.Identity.Exceptions;

public class RefreshTokenNotFoundException : AppException
{
    public RefreshTokenNotFoundException(RefreshToken? refreshToken)
        : base("Refresh token not found.", StatusCodes.Status404NotFound) { }
}
