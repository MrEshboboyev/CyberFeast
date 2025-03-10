using BuildingBlocks.Core.Exception.Types;
using Microsoft.AspNetCore.Http;

namespace FoodDelivery.Services.Customers.RestockSubscriptions.Features.CreatingRestockSubscription.v1.Exceptions;

public class ProductAlreadySubscribedException(
    long productId, 
    string productName) : AppException(
    $"Product with InternalCommandId '{productId}' and Name '{productName}' is already subscribed",
    StatusCodes.Status409Conflict);
