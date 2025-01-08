using System.Collections.Concurrent;
using BuildingBlocks.Abstractions.Domain;
using BuildingBlocks.Core.Persistence.EFCore.Converters;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace BuildingBlocks.Core.Persistence.EFCore;

/// <summary>
/// Provides custom value converters for strongly typed IDs in Entity Framework Core.
/// </summary>
/// <typeparam name="TId">The type of the strongly typed ID.</typeparam>
public class StronglyTypedIdValueConverterSelector<TId>(
    ValueConverterSelectorDependencies dependencies) : ValueConverterSelector(dependencies)
{
    private readonly ConcurrentDictionary<(Type ModelClrType, Type ProviderClrType),
        ValueConverterInfo> _converters = new();

    /// <summary>
    /// Selects the appropriate value converter for the given model and provider types.
    /// </summary>
    /// <param name="modelClrType">The model CLR type.</param>
    /// <param name="providerClrType">The provider CLR type.</param>
    /// <returns>A collection of value converter information.</returns>
    public override IEnumerable<ValueConverterInfo> Select(
        Type? modelClrType,
        Type? providerClrType = null)
    {
        var baseConverters = base.Select(modelClrType!, providerClrType);
        foreach (var converter in baseConverters)
        {
            yield return converter;
        }

        var underlyingModelType = UnwrapNullableType(modelClrType);
        var underlyingProviderType = UnwrapNullableType(providerClrType);

        if (underlyingProviderType is not null &&
            underlyingProviderType != typeof(TId)) yield break;

        var isAggregateTypedIdValue = typeof(AggregateId<TId>)
            .IsAssignableFrom(underlyingModelType);
        var isEntityTypedIdValue = typeof(EntityId<TId>)
            .IsAssignableFrom(underlyingModelType);

        if (isAggregateTypedIdValue)
        {
            var converterType = typeof(AggregateIdValueConverter<,>).MakeGenericType(
                underlyingModelType!,
                typeof(TId)
            );

            yield return _converters.GetOrAdd(
                (underlyingModelType, typeof(TId))!,
                _ =>
                {
                    return new ValueConverterInfo(
                        modelClrType: modelClrType!,
                        providerClrType: typeof(TId),
                        factory: valueConverterInfo =>
                            (ValueConverter)Activator.CreateInstance(
                                converterType,
                                valueConverterInfo.MappingHints)!
                    );
                }
            );
        }
        else if (isEntityTypedIdValue)
        {
            var converterType = typeof(EntityIdValueConverter<,>)
                .MakeGenericType(underlyingModelType!, typeof(TId));

            yield return _converters.GetOrAdd(
                (underlyingModelType, typeof(TId))!,
                _ =>
                {
                    return new ValueConverterInfo(
                        modelClrType: modelClrType!,
                        providerClrType: typeof(TId),
                        factory: valueConverterInfo =>
                            (ValueConverter)Activator.CreateInstance(
                                converterType, valueConverterInfo.MappingHints)!
                    );
                }
            );
        }
    }

    /// <summary>
    /// Unwraps the underlying type if the given type is nullable.
    /// </summary>
    /// <param name="type">The type to unwrap.</param>
    /// <returns>The underlying type if nullable; otherwise, the original type.</returns>
    private static Type? UnwrapNullableType(Type? type)
    {
        if (type is null)
        {
            return null;
        }

        return Nullable.GetUnderlyingType(type) ?? type;
    }
}