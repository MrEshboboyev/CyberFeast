using System.Reflection;
using BuildingBlocks.Abstractions.Domain;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace BuildingBlocks.Core.Persistence.EFCore.Converters;

/// <summary>
/// Provides a value converter for <see cref="AggregateId{TId}"/> types in Entity Framework Core.
/// </summary>
/// <typeparam name="TAggregateId">The type of the aggregate ID.</typeparam>
/// <typeparam name="TId">The type of the underlying ID.</typeparam>
public class AggregateIdValueConverter<TAggregateId, TId>(
    ConverterMappingHints mappingHints = null!) : ValueConverter<TAggregateId, TId>(
    id => id.Value,
    value => Create(value), mappingHints) where TAggregateId : AggregateId<TId>
{
    /// <summary>
    /// Creates an instance of the <typeparamref name="TAggregateId"/> type using a private or protected constructor.
    /// </summary>
    /// <param name="id">The ID value.</param>
    /// <returns>An instance of the <typeparamref name="TAggregateId"/> type.</returns>
    private static TAggregateId Create(TId id) =>
    (
        Activator.CreateInstance(
            typeof(TAggregateId),
            BindingFlags.Instance | BindingFlags.NonPublic,
            null,
            [id],
            null,
            null
        ) as TAggregateId
    )!;
}