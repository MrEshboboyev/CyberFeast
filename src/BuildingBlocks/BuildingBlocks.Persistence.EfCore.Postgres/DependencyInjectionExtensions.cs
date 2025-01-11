using System.Reflection;
using BuildingBlocks.Abstractions.Events;
using BuildingBlocks.Abstractions.Persistence;
using BuildingBlocks.Abstractions.Persistence.EFCore;
using BuildingBlocks.Core.Extensions;
using BuildingBlocks.Core.Extensions.ServiceCollection;
using BuildingBlocks.Core.Persistence.EFCore;
using BuildingBlocks.Core.Persistence.EFCore.Interceptors;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace BuildingBlocks.Persistence.EfCore.Postgres;

/// <summary>
/// Provides extension methods for registering and configuring PostgreSQL DbContext instances.
/// </summary>
public static class DependencyInjectionExtensions
{
    /// <summary>
    /// Adds and configures a PostgreSQL DbContext in the service collection.
    /// </summary>
    /// <typeparam name="TDbContext">The type of DbContext.</typeparam>
    /// <param name="services">The service collection.</param>
    /// <param name="configuration">The configuration instance.</param>
    /// <param name="migrationAssembly">The migration assembly. Optional.</param>
    /// <param name="builder">An optional action to further configure the DbContext options.</param>
    /// <param name="configurator">An optional action to configure the Postgres options.</param>
    /// <param name="assembliesToScan">Assemblies to scan for repositories and unit of work patterns.</param>
    /// <returns>The updated service collection.</returns>
    public static IServiceCollection AddPostgresDbContext<TDbContext>(
        this IServiceCollection services,
        IConfiguration configuration,
        Assembly? migrationAssembly = null,
        Action<DbContextOptionsBuilder>? builder = null,
        Action<PostgresOptions>? configurator = null,
        params Assembly[] assembliesToScan
    )
        where TDbContext : DbContext, IDbFacadeResolver, IDomainEventContext
    {
        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

        // Add options to the dependency injection
        services.AddValidationOptions(configurator);
        var options = configuration.BindOptions(configurator);

        var connStr = options.ConnectionString!.NotBeEmptyOrNull();
        services.TryAddScoped<IConnectionFactory>(_ => new NpgsqlConnectionFactory(connStr));

        services.AddDbContext<TDbContext>((sp, dbContextOptionsBuilder) =>
        {
            dbContextOptionsBuilder
                .UseNpgsql(connStr, sqlOptions =>
                {
                    var name = migrationAssembly?.GetName().Name ??
                               options.MigrationAssembly ?? typeof(TDbContext).Assembly.GetName().Name;
                    sqlOptions.MigrationsAssembly(name);
                    sqlOptions.EnableRetryOnFailure(5, TimeSpan.FromSeconds(10), null);
                })
                .UseSnakeCaseNamingConvention()
                .ReplaceService<IValueConverterSelector, StronglyTypedIdValueConverterSelector<long>>();

            dbContextOptionsBuilder.AddInterceptors(
                new AuditInterceptor(),
                new SoftDeleteInterceptor(),
                new ConcurrencyInterceptor()
            );

            builder?.Invoke(dbContextOptionsBuilder);
        });

        services.TryAddScoped<IDbFacadeResolver>(provider => provider.GetService<TDbContext>()!);
        services.TryAddScoped<IDomainEventContext>(provider => provider.GetService<TDbContext>()!);

        services.AddPostgresRepositories(assembliesToScan);
        services.AddPostgresUnitOfWork(assembliesToScan);

        return services;
    }

    private static IServiceCollection AddPostgresRepositories(
        this IServiceCollection services, params Assembly[] assembliesToScan)
    {
        var scanAssemblies = assembliesToScan.Length != 0 ? assembliesToScan : new[] { Assembly.GetCallingAssembly() };

        services.Scan(scan =>
            scan.FromAssemblies(scanAssemblies)
                .AddClasses(classes => classes.AssignableTo(typeof(IRepository<,>)), false)
                .AsImplementedInterfaces()
                .AsSelf()
                .WithTransientLifetime()
        );

        return services;
    }

    private static IServiceCollection AddPostgresUnitOfWork(
        this IServiceCollection services, params Assembly[] assembliesToScan)
    {
        var scanAssemblies = assembliesToScan.Length != 0 ? assembliesToScan : new[] { Assembly.GetCallingAssembly() };

        services.Scan(scan =>
            scan.FromAssemblies(scanAssemblies)
                .AddClasses(classes => classes.AssignableTo(typeof(IEFUnitOfWork<>)), false)
                .AsImplementedInterfaces()
                .AsSelf()
                .WithTransientLifetime()
        );

        return services;
    }
}