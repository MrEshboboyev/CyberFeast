using BuildingBlocks.Persistence.EfCore.Postgres;

namespace FoodDelivery.Services.Identity.Shared.Data;

/// <summary>
/// Provides a factory for creating the IdentityContext at design-time.
/// Ensures the connection string is correctly configured for migrations.
/// </summary>
public class DbContextDesignFactory() : DbContextDesignFactoryBase<IdentityContext>(
    "PostgresOptions:ConnectionString");
