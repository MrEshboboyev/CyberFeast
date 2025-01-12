using BuildingBlocks.Core.Extensions;
using BuildingBlocks.Core.Messaging;

namespace FoodDelivery.Services.Shared.Identity.Users.Events.V1.Integration;

/// <summary>
/// Represents the event when a user is registered.
/// </summary>
public record UserRegisteredV1(
    Guid IdentityId,
    string Email,
    string PhoneNumber,
    string UserName,
    string FirstName,
    string LastName,
    IEnumerable<string>? Roles
) : IntegrationEvent
{
    /// <summary>
    /// Creates a new instance of the <see cref="UserRegisteredV1"/> event with in-line validation.
    /// </summary>
    /// <param name="identityId">The identity ID of the user.</param>
    /// <param name="email">The email of the user.</param>
    /// <param name="phoneNumber">The phone number of the user.</param>
    /// <param name="userName">The username of the user.</param>
    /// <param name="firstName">The first name of the user.</param>
    /// <param name="lastName">The last name of the user.</param>
    /// <param name="roles">The roles of the user.</param>
    /// <returns>A new instance of the <see cref="UserRegisteredV1"/> event.</returns>
    public static UserRegisteredV1 Of(
        Guid identityId,
        string? email,
        string? phoneNumber,
        string? userName,
        string? firstName,
        string? lastName,
        IEnumerable<string>? roles)
    {
        return new UserRegisteredV1(
            identityId.NotBeInvalid(),
            email.NotBeEmptyOrNull().NotBeInvalidEmail(),
            phoneNumber.NotBeEmptyOrNull(),
            userName.NotBeEmptyOrNull(),
            firstName.NotBeEmptyOrNull(),
            lastName.NotBeEmptyOrNull(),
            roles
        );
    }
}