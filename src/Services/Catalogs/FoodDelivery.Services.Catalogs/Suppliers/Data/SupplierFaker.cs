using Bogus;

namespace FoodDelivery.Services.Catalogs.Suppliers.Data;

public sealed class SupplierFaker : Faker<Supplier>
{
    public SupplierFaker()
    {
        long id = 1;

        // Call for objects that have complex initialization
        // faker doesn't work with normal syntax because it has no default constructor
        CustomInstantiator(faker =>
        {
            var supplier = new Supplier(SupplierId.Of(id++), faker.Person.FullName);
            return supplier;
        });
    }
}