using BuildingBlocks.Core.Exception.Types;

namespace FoodDelivery.Services.Customers.RestockSubscriptions.Features.CreatingRestockSubscription.v1.Exceptions;

public class ProductHasStockException(
    long productId, 
    int quantity, 
    string name) : AppException(
    $@"Product with InternalCommandId '{productId}' and name '{name}' already has available stock of '{quantity}' items.");
