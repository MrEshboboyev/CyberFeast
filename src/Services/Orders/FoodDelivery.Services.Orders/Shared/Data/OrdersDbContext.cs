using BuildingBlocks.Core.Persistence.EfCore;
using BuildingBlocks.Core.Persistence.EFCore;
using FoodDelivery.Services.Orders.Orders.Models;
using FoodDelivery.Services.Orders.Shared.Contracts;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace FoodDelivery.Services.Orders.Shared.Data;

public class OrdersDbContext(DbContextOptions<OrdersDbContext> options) : 
    EfDbContextBase(options), 
    IOrdersDbContext
{
    public const string DefaultSchema = "order";

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.HasPostgresExtension(EfConstants.UuidGenerator);
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }

    public DbSet<Order> Orders => Set<Order>();
}