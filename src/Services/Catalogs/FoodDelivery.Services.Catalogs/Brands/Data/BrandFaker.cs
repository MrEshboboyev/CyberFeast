using Bogus;
using FoodDelivery.Services.Catalogs.Brands.ValueObjects;

namespace FoodDelivery.Services.Catalogs.Brands.Data;

public sealed class BrandFaker : Faker<Brand>
{
    public BrandFaker()
    {
        long id = 1;

        CustomInstantiator(f => 
            Brand.Of(BrandId.Of(id++),
            BrandName.Of(f.Company.CompanyName())));
    }
}