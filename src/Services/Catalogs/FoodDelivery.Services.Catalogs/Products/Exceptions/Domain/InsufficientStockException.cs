using BuildingBlocks.Core.Domain.Exceptions;

namespace FoodDelivery.Services.Catalogs.Products.Exceptions.Domain;

public class InsufficientStockException(string message) : DomainException(message);
