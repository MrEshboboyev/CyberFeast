using BuildingBlocks.Abstractions.Persistence.EFCore;
using FoodDelivery.Services.Orders.Orders.Models;
using Microsoft.EntityFrameworkCore;

namespace FoodDelivery.Services.Orders.Shared.Contracts;

public interface IOrdersDbContext : IDbContext
{
    public DbSet<Order> Orders { get; }
}
