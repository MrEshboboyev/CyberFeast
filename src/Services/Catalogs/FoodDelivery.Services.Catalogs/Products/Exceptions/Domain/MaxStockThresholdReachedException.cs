using BuildingBlocks.Core.Domain.Exceptions;

namespace FoodDelivery.Services.Catalogs.Products.Exceptions.Domain;

public class MaxStockThresholdReachedException(string message) : DomainException(message);
