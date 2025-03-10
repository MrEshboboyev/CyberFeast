using BuildingBlocks.Core.Exception.Types;

namespace FoodDelivery.Services.Customers.Customers.Exceptions.Domain;

internal class CustomerAlreadyVerifiedException(long customerId)
    : AppException($"Customer with InternalCommandId: '{customerId}' already verified.")
{
    public long CustomerId { get; } = customerId;
}
