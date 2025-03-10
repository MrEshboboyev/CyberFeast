using BuildingBlocks.Core.Persistence.EfCore;
using FoodDelivery.Services.Catalogs.Brands;
using FoodDelivery.Services.Catalogs.Categories;
using FoodDelivery.Services.Catalogs.Products.Models;
using FoodDelivery.Services.Catalogs.Shared.Contracts;
using FoodDelivery.Services.Catalogs.Suppliers;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace FoodDelivery.Services.Catalogs.Shared.Data;

public class CatalogDbContext(
    DbContextOptions<CatalogDbContext> options) : EfDbContextBase(options), ICatalogDbContext
{
    public const string DefaultSchema = "catalog";

    public DbSet<Product> Products { get; set; } = null!;
    public DbSet<ProductView> ProductsView { get; set; } = null!;
    public DbSet<Category> Categories { get; set; } = null!;
    public DbSet<Supplier> Suppliers { get; set; } = null!;
    public DbSet<Brand> Brands { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        base.OnModelCreating(modelBuilder);
    }
}