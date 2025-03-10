namespace FoodDelivery.Services.Customers.RestockSubscriptions.Dtos.v1;

public record RestockSubscriptionReadDto
{
    public string CustomerName { get; init; } = null!;
    public string CustomerId { get; init; } = null!;
    public string ProductId { get; init; } = null!;
    public string ProductName { get; init; } = null!;
    public bool Processed { get; init; }
    public DateTime? ProcessedTime { get; init; }
}
