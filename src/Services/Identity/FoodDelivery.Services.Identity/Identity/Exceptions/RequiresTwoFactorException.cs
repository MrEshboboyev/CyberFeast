using BuildingBlocks.Core.Exception.Types;
using Microsoft.AspNetCore.Http;

namespace FoodDelivery.Services.Identity.Identity.Exceptions;

public class RequiresTwoFactorException(string message) : AppException(message, StatusCodes.Status404NotFound);
