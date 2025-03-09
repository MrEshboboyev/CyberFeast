using BuildingBlocks.Core.Exception.Types;
using Microsoft.AspNetCore.Http;

namespace FoodDelivery.Services.Identity.Identity.Exceptions;

public class PasswordIsInvalidException(string message) : AppException(message, StatusCodes.Status403Forbidden);
