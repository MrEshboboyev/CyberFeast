using BuildingBlocks.Abstractions.Events;
using BuildingBlocks.Abstractions.Messaging;
using BuildingBlocks.Core.Messaging;
using Humanizer;
using MassTransit;

namespace BuildingBlocks.Integration.MassTransit;

/// <summary>
/// Sets the primary exchange name for each entity type globally.
/// </summary>
public class CustomEntityNameFormatter : IEntityNameFormatter
{
    /// <summary>
    /// Formats the entity name for the specified message type.
    /// </summary>
    public string FormatEntityName<T>()
    {
        if (!typeof(IEventEnvelope).IsAssignableFrom(typeof(T)))
            return $"{typeof(T).Name.Underscore()}{MessagingConstants.PrimaryExchangePostfix}";

        var messageProperty = typeof(T).GetProperty(nameof(IEventEnvelope.Message));

        return typeof(IMessage)
            .IsAssignableFrom(messageProperty!.PropertyType)
            ? $"{messageProperty.PropertyType.Name.Underscore()}{MessagingConstants.PrimaryExchangePostfix}"
            : $"{typeof(T).Name.Underscore()}{MessagingConstants.PrimaryExchangePostfix}";
    }
}

/// <summary>
/// Provides a specific message entity name formatter for message types.
/// </summary>
/// <typeparam name="TMessage">The type of the message.</typeparam>
public class CustomEntityNameFormatter<TMessage> : IMessageEntityNameFormatter<TMessage>
    where TMessage : class
{
    /// <summary>
    /// Formats the entity name for the specified message type.
    /// </summary>
    public string FormatEntityName()
    {
        if (!typeof(IEventEnvelope).IsAssignableFrom(typeof(TMessage)))
            return $"{typeof(TMessage).Name.Underscore()}{MessagingConstants.PrimaryExchangePostfix}";

        var messageProperty = typeof(TMessage).GetProperty(nameof(IEventEnvelope.Message));

        return typeof(IMessage).IsAssignableFrom(messageProperty!.PropertyType)
            ? $"{messageProperty.PropertyType.Name.Underscore()}{MessagingConstants.PrimaryExchangePostfix}"
            : $"{typeof(TMessage).Name.Underscore()}{MessagingConstants.PrimaryExchangePostfix}";
    }
}