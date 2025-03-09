using BuildingBlocks.Core.Exception.Types;
using Microsoft.AspNetCore.Http;

namespace FoodDelivery.Services.Identity.Identity.Exceptions;

public class EmailNotConfirmedException(string email) : AppException(
    $"Email not confirmed for email address `{email}`",
    StatusCodes.Status422UnprocessableEntity)
{
    public string Email { get; } = email;
}
