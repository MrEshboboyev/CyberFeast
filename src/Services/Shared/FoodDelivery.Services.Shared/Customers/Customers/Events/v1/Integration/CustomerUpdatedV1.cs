using BuildingBlocks.Core.Extensions;
using BuildingBlocks.Core.Messaging;

namespace FoodDelivery.Services.Shared.Customers.Customers.Events.V1.Integration;

/// <summary>
/// Represents an integration event for when a customer is updated.
/// </summary>
public record CustomerUpdatedV1(
    long Id,
    string FirstName,
    string LastName,
    string Email,
    string PhoneNumber,
    Guid IdentityId,
    DateTime CreatedAt,
    DateTime? BirthDate = null,
    string? Nationality = null,
    string? Address = null
) : IntegrationEvent
{
    /// <summary>
    /// Creates a new instance of <see cref="CustomerUpdatedV1"/> with in-line validation.
    /// </summary>
    /// <param name="id">The ID of the customer.</param>
    /// <param name="firstName">The first name of the customer.</param>
    /// <param name="lastName">The last name of the customer.</param>
    /// <param name="email">The email of the customer.</param>
    /// <param name="phoneNumber">The phone number of the customer.</param>
    /// <param name="identityId">The identity ID of the customer.</param>
    /// <param name="createdAt">The creation date of the customer.</param>
    /// <param name="birthDate">The birthdate of the customer.</param>
    /// <param name="nationality">The nationality of the customer.</param>
    /// <param name="address">The address of the customer.</param>
    /// <returns>A validated <see cref="CustomerUpdatedV1"/> instance.</returns>
    /// <exception cref="ArgumentException">Thrown when any required parameter is invalid.</exception>
    public static CustomerUpdatedV1 Of(
        long id,
        string firstName,
        string lastName,
        string email,
        string phoneNumber,
        Guid identityId,
        DateTime createdAt,
        DateTime? birthDate = null,
        string? nationality = null,
        string? address = null)
    {
        id.NotBeNegativeOrZero();
        firstName.NotBeNullOrWhiteSpace();
        lastName.NotBeNullOrWhiteSpace();
        email.NotBeNullOrWhiteSpace().NotBeInvalidEmail();
        phoneNumber.NotBeNullOrWhiteSpace();
        identityId.NotBeInvalid();

        return new CustomerUpdatedV1(
            id, firstName, lastName, email, phoneNumber, identityId, createdAt, birthDate, nationality, address);
    }
}