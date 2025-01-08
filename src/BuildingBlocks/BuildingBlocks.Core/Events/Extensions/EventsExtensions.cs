using System.Reflection;
using BuildingBlocks.Abstractions.Events;
using BuildingBlocks.Abstractions.Events.Internal;
using BuildingBlocks.Abstractions.Messaging;
using BuildingBlocks.Core.Events.Internal;
using BuildingBlocks.Core.Reflection.Extensions;

namespace BuildingBlocks.Core.Events.Extensions;

/// <summary>
/// Provides extension methods for working with event types and wrapping domain and integration events.
/// </summary>
public static class EventsExtensions
{
    /// <summary>
    /// Gets the types of handled integration events from the specified assemblies.
    /// </summary>
    /// <param name="assemblies">The assemblies to scan for integration event handlers.</param>
    /// <returns>A collection of handled integration event types.</returns>
    public static IEnumerable<Type> GetHandledIntegrationEventTypes(this Assembly[] assemblies)
    {
        var messageHandlerTypes = typeof(IIntegrationEventHandler<>)
            .GetAllTypesImplementingOpenGenericInterface(assemblies)
            .ToList();

        var inheritsTypes = messageHandlerTypes
            .SelectMany(x => x.GetInterfaces())
            .Where(x =>
                x.GetInterfaces().Any(i => i.IsGenericType)
                && x.GetGenericTypeDefinition() == typeof(IIntegrationEventHandler<>));

        foreach (var inheritsType in inheritsTypes)
        {
            var messageType = inheritsType.GetGenericArguments().First();
            if (messageType.IsAssignableTo(typeof(IIntegrationEvent)))
            {
                yield return messageType;
            }
        }
    }

    /// <summary>
    /// Gets the types of handled domain notification events from the specified assemblies.
    /// </summary>
    /// <param name="assemblies">The assemblies to scan for domain notification event handlers.</param>
    /// <returns>A collection of handled domain notification event types.</returns>
    public static IEnumerable<Type> GetHandledDomainNotificationEventTypes(this Assembly[] assemblies)
    {
        var messageHandlerTypes = typeof(IDomainNotificationEventHandler<>)
            .GetAllTypesImplementingOpenGenericInterface(assemblies)
            .ToList();

        var inheritsTypes = messageHandlerTypes
            .SelectMany(x => x.GetInterfaces())
            .Where(x =>
                x.GetInterfaces().Any(i => i.IsGenericType)
                && x.GetGenericTypeDefinition() == typeof(IDomainNotificationEventHandler<>));

        foreach (var inheritsType in inheritsTypes)
        {
            var messageType = inheritsType.GetGenericArguments().First();
            if (messageType.IsAssignableTo(typeof(IDomainNotificationEvent)))
            {
                yield return messageType;
            }
        }
    }

    /// <summary>
    /// Gets the types of handled domain events from the specified assemblies.
    /// </summary>
    /// <param name="assemblies">The assemblies to scan for domain event handlers.</param>
    /// <returns>A collection of handled domain event types.</returns>
    public static IEnumerable<Type> GetHandledDomainEventTypes(this Assembly[] assemblies)
    {
        var messageHandlerTypes = typeof(IDomainEventHandler<>)
            .GetAllTypesImplementingOpenGenericInterface(assemblies)
            .ToList();

        var inheritsTypes = messageHandlerTypes
            .SelectMany(x => x.GetInterfaces())
            .Where(x =>
                x.GetInterfaces().Any(i => i.IsGenericType)
                && x.GetGenericTypeDefinition() == typeof(IDomainEventHandler<>));

        foreach (var inheritsType in inheritsTypes)
        {
            var messageType = inheritsType.GetGenericArguments().First();
            if (messageType.IsAssignableTo(typeof(IDomainEvent)))
            {
                yield return messageType;
            }
        }
    }

    /// <summary>
    /// Wraps domain events in domain notification events.
    /// </summary>
    /// <param name="domainEvents">The domain events to wrap.</param>
    /// <returns>A collection of wrapped domain notification events.</returns>
    public static IEnumerable<IDomainNotificationEvent> GetWrappedDomainNotificationEvents(
        this IEnumerable<IDomainEvent> domainEvents)
    {
        foreach (IDomainEvent domainEvent in domainEvents.Where(x =>
                     typeof(IHaveNotificationEvent).IsAssignableFrom(x.GetType())))
        {
            Type genericType = typeof(DomainNotificationEventWrapper<>).MakeGenericType(domainEvent.GetType());

            IDomainNotificationEvent? domainNotificationEvent = (IDomainNotificationEvent?)
                Activator.CreateInstance(genericType, domainEvent);

            if (domainNotificationEvent is not null)
                yield return domainNotificationEvent;
        }
    }

    /// <summary>
    /// Wraps domain events in integration events.
    /// </summary>
    /// <param name="domainEvents">The domain events to wrap.</param>
    /// <returns>A collection of wrapped integration events.</returns>
    public static IEnumerable<IIntegrationEvent> GetWrappedIntegrationEvents(
        this IEnumerable<IDomainEvent> domainEvents)
    {
        foreach (IDomainEvent domainEvent in domainEvents.Where(x =>
                     typeof(IHaveExternalEvent).IsAssignableFrom(x.GetType())))
        {
            Type genericType = typeof(IntegrationEventWrapper<>).MakeGenericType(domainEvent.GetType());

            IIntegrationEvent? domainNotificationEvent = (IIntegrationEvent?)
                Activator.CreateInstance(genericType, domainEvent);

            if (domainNotificationEvent is not null)
                yield return domainNotificationEvent;
        }
    }
}