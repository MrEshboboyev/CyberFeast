using BuildingBlocks.Core.Exception.Types;
using Microsoft.AspNetCore.Http;

namespace FoodDelivery.Services.Customers.Products.Exceptions;

public class ProductNotFoundException(long id)
    : AppException($"Product with id {id} not found", StatusCodes.Status404NotFound);
