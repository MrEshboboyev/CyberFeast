using System.Reflection;
using BuildingBlocks.Abstractions.Messaging;
using BuildingBlocks.Core.Reflection.Extensions;

namespace BuildingBlocks.Core.Messaging.Extensions;

/// <summary>
/// Provides extension methods for working with message types in assemblies.
/// </summary>
public static class MessageExtensions
{
    /// <summary>
    /// Gets a collection of all message types handled by the specified assemblies.
    /// </summary>
    /// <param name="assemblies">The assemblies to scan for message handlers.</param>
    /// <returns>An enumerable collection of handled message types.</returns>
    public static IEnumerable<Type> GetHandledMessageTypes(this Assembly[] assemblies)
    {
        var messageHandlerTypes = typeof(IMessageHandler<>)
            .GetAllTypesImplementingOpenGenericInterface(assemblies)
            .ToList();

        var inheritsTypes = messageHandlerTypes
            .SelectMany(x => x.GetInterfaces())
            .Where(x =>
                x.GetInterfaces().Any(i => i.IsGenericType) && x.GetGenericTypeDefinition() == typeof(IMessageHandler<>)
            );

        foreach (var inheritsType in inheritsTypes)
        {
            var messageType = inheritsType.GetGenericArguments().First();
            if (messageType.IsAssignableTo(typeof(IMessage)))
            {
                yield return messageType;
            }
        }
    }
}