using Bogus;
using FoodDelivery.Services.Catalogs.Brands.Contracts;
using FoodDelivery.Services.Catalogs.Brands.ValueObjects;
using FoodDelivery.Services.Catalogs.Categories;
using FoodDelivery.Services.Catalogs.Products.Models;
using FoodDelivery.Services.Catalogs.Products.ValueObjects;
using FoodDelivery.Services.Catalogs.Suppliers;
using FoodDelivery.Services.Catalogs.Suppliers.Contracts;
using NSubstitute;

namespace FoodDelivery.Services.Catalogs.Products.Data;

public sealed class ProductFaker : Faker<Product>
{
    public ProductFaker()
    {
        long id = 1;

        var supplierChecker = Substitute.For<ISupplierChecker>();
        supplierChecker.SupplierExists(Arg.Any<SupplierId>()).Returns(true);

        var brandChecker = Substitute.For<IBrandChecker>();
        brandChecker.BrandExists(Arg.Any<BrandId>()).Returns(true);

        // we should not instantiate customer aggregate manually because it is possible we break aggregate invariant in creating a product, and it is better we
        // create a product with its factory method
        CustomInstantiator(faker =>
            Product.Create(
                ProductId.Of(id++),
                Name.Of(faker.Commerce.ProductName()),
                ProductInformation.Of(faker.Commerce.ProductName(), faker.Commerce.ProductDescription()),
                Stock.Of(faker.Random.Int(10, 20), 5, 20),
                faker.PickRandom<ProductStatus>(),
                faker.PickRandom<ProductType>(),
                Dimensions.Of(faker.Random.Int(10, 50), faker.Random.Int(10, 50), faker.Random.Int(10, 50)),
                Size.Of(faker.PickRandom<string>("M", "S", "L")),
                faker.Random.Enum<ProductColor>(),
                faker.Commerce.ProductDescription(),
                Price.Of(faker.PickRandom<decimal>(100, 200, 500)),
                CategoryId.Of(faker.Random.Long(1, 3)),
                SupplierId.Of(faker.Random.Long(1, 5)),
                BrandId.Of(faker.Random.Long(1, 5)),
                categoryId => Task.FromResult(true),
                supplierChecker,
                brandChecker
            )
        );
    }
}
