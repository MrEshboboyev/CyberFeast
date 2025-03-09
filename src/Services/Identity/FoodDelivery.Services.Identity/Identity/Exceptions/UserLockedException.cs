using BuildingBlocks.Core.Exception.Types;
using Microsoft.AspNetCore.Http;

namespace FoodDelivery.Services.Identity.Identity.Exceptions;

public class UserLockedException(string userId)
    : AppException($"userId '{userId}' has been locked.", StatusCodes.Status403Forbidden);
