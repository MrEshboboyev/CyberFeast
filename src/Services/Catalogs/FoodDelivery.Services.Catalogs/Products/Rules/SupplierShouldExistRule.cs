using System.Diagnostics.CodeAnalysis;
using BuildingBlocks.Abstractions.Domain;
using BuildingBlocks.Core.Extensions;
using FoodDelivery.Services.Catalogs.Suppliers;
using FoodDelivery.Services.Catalogs.Suppliers.Contracts;
using Microsoft.AspNetCore.Http;

namespace FoodDelivery.Services.Catalogs.Products.Rules;

public class SupplierShouldExistRule(
    [NotNull] ISupplierChecker? supplierChecker, 
    [NotNull] SupplierId? id) : IBusinessRule
{
    public bool IsBroken()
    {
        supplierChecker.NotBeNull();
        id.NotBeNull();
        var exists = supplierChecker.SupplierExists(id);

        return !exists;
    }

    public string Message => $"Supplier with id {id} not exist.";

    public int Status => StatusCodes.Status404NotFound;
}
