using System.Reflection;
using BuildingBlocks.Abstractions.Domain;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace BuildingBlocks.Core.Persistence.EFCore.Converters;

/// <summary>
/// Provides a value converter for <see cref="EntityId{TId}"/> types in Entity Framework Core.
/// </summary>
/// <typeparam name="TEntityId">The type of the entity ID.</typeparam>
/// <typeparam name="TId">The type of the underlying ID.</typeparam>
public class EntityIdValueConverter<TEntityId, TId>(
    ConverterMappingHints mappingHints = null!) : ValueConverter<TEntityId, TId>(
    id => id.Value,
    value => Create(value), mappingHints) where TEntityId : EntityId<TId>
{
    /// <summary>
    /// Creates an instance of the <typeparamref name="TEntityId"/> type using a private or protected constructor.
    /// </summary>
    /// <param name="id">The ID value.</param>
    /// <returns>An instance of the <typeparamref name="TEntityId"/> type.</returns>
    private static TEntityId Create(TId id) =>
    (
        Activator.CreateInstance(
            typeof(TEntityId),
            BindingFlags.Instance | BindingFlags.NonPublic,
            null,
            [id],
            null,
            null
        ) as TEntityId
    )!;
}