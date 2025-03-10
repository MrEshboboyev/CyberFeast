using FoodDelivery.Services.Customers.Shared.Clients.Identity.Dtos;
using FoodDelivery.Services.Customers.Users.Model;

namespace FoodDelivery.Services.Customers.Shared.Clients.Identity;

/// <summary>
/// IdentityApiClient acts as an anti-corruption-layer for our system.
/// An Anti-Corruption Layer (ACL) is a set of patterns placed between the domain model and other bounded contexts or third party dependencies. The intent of this layer is to prevent the intrusion of foreign concepts and models into the domain model.
/// </summary>
public interface IIdentityApiClient
{
    Task<UserIdentity?> GetUserByEmailAsync(string email, CancellationToken cancellationToken = default);

    Task<UserIdentity?> CreateUserIdentityAsync(
        CreateUserClientDto createUserClientDto,
        CancellationToken cancellationToken = default
    );
}
