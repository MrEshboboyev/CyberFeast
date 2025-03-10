using BuildingBlocks.Core.Domain.Exceptions;

namespace FoodDelivery.Services.Customers.RestockSubscriptions.Exceptions.Domain;

public class RestockSubscriptionDomainException(string message) : DomainException(message);
