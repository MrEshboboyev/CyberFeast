using BuildingBlocks.Core.Domain.Exceptions;

namespace FoodDelivery.Services.Catalogs.Products.Exceptions.Domain;

public class ProductDomainException(string message) : DomainException(message);
