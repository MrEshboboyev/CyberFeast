namespace FoodDelivery.Services.Catalogs.Products.Models;

public class ProductView
{
    public long ProductId { get; set; }
    public string ProductName { get; set; } = null!;
    public long CategoryId { get; set; }
    public string CategoryName { get; set; } = null!;
    public long SupplierId { get; set; }
    public string SupplierName { get; set; } = null!;
    public long BrandId { get; set; }
    public string BrandName { get; set; } = null!;
}