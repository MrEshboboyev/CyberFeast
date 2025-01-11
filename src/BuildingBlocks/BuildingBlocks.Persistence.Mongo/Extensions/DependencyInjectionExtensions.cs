using BuildingBlocks.Abstractions.Persistence.Mongo;
using BuildingBlocks.Core.Extensions.ServiceCollection;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Bson.Serialization.Serializers;
using static MongoDB.Bson.Serialization.BsonSerializer;

namespace BuildingBlocks.Persistence.Mongo.Extensions;

/// <summary>
/// Provides extension methods for registering MongoDB context and related services.
/// </summary>
public static class DependencyInjectionExtensions
{
    /// <summary>
    /// Registers the MongoDB context and related services in the dependency injection container.
    /// </summary>
    /// <typeparam name="TContext">The type of MongoDB context.</typeparam>
    /// <param name="services">The service collection.</param>
    /// <returns>The updated service collection.</returns>
    public static IServiceCollection AddMongoDbContext<TContext>(this IServiceCollection services)
        where TContext : MongoDbContext, IMongoDbContext
    {
        services.AddValidatedOptions<MongoOptions>(nameof(MongoOptions)); // Add Mongo options with validation.

        RegisterSerializer(DateTimeSerializer.LocalInstance); // Register the local DateTime serializer.
        RegisterSerializer(
            new GuidSerializer(GuidRepresentation
                .CSharpLegacy)); // Register GUID serializer with C# legacy representation.

        RegisterConventions(); // Register MongoDB serialization conventions.

        services.AddScoped<TContext>(); // Register the MongoDB context.
        services.AddScoped<IMongoDbContext>(sp =>
            sp.GetRequiredService<TContext>()); // Register the context as IMongoDbContext.

        services.AddTransient(typeof(IMongoRepository<,>), typeof(MongoRepository<,>)); // Register generic repository.
        services.AddTransient(typeof(IMongoUnitOfWork<>), typeof(MongoUnitOfWork<>)); // Register unit of work.

        return services; // Return the updated service collection.
    }

    /// <summary>
    /// Registers MongoDB serialization conventions.
    /// </summary>
    private static void RegisterConventions()
    {
        ConventionRegistry.Register(
            "conventions",
            new ConventionPack
            {
                new CamelCaseElementNameConvention(), // Converts element names to camel case.
                new IgnoreExtraElementsConvention(true), // Ignores extra elements in BSON documents.
                new EnumRepresentationConvention(BsonType.String), // Represents enums as strings.
                new IgnoreIfDefaultConvention(false), // Does not ignore default values.
                new ImmutablePocoConvention(), // Handles immutable Plain Old CLR objects (POCOs).
            },
            _ => true
        );
    }
}