using BuildingBlocks.Core.Messaging;

namespace BuildingBlocks.Core.Web.HeaderPropagation.Extensions;

/// <summary>
/// Provides extension methods for <see cref="HeaderPropagationStore"/>.
/// </summary>
public static class HeaderPropagationStoreExtensions
{
    /// <summary>
    /// Adds a correlation ID to the header propagation store.
    /// </summary>
    /// <param name="headerPropagationStore">The header propagation store.</param>
    /// <param name="correlationId">The correlation ID.</param>
    public static void AddCorrelationId(
        this HeaderPropagationStore headerPropagationStore,
        Guid correlationId)
    {
        if (!headerPropagationStore.Headers.ContainsKey(MessageHeaders.CorrelationId))
        {
            headerPropagationStore.Headers.Add(MessageHeaders.CorrelationId, correlationId.ToString());
        }
    }

    /// <summary>
    /// Gets the correlation ID from the header propagation store.
    /// </summary>
    /// <param name="headerPropagationStore">The header propagation store.</param>
    /// <returns>The correlation ID, or null if not found.</returns>
    public static Guid? GetCorrelationId(this HeaderPropagationStore headerPropagationStore)
    {
        headerPropagationStore.Headers.TryGetValue(MessageHeaders.CorrelationId, out var cid);
        return string.IsNullOrEmpty(cid) ? null : Guid.Parse(cid!);
    }

    /// <summary>
    /// Adds a message name to the header propagation store.
    /// </summary>
    /// <param name="headerPropagationStore">The header propagation store.</param>
    /// <param name="messageName">The message name.</param>
    public static void AddMessageName(
        this HeaderPropagationStore headerPropagationStore,
        string messageName)
    {
        if (!headerPropagationStore.Headers.ContainsKey(MessageHeaders.Name))
        {
            headerPropagationStore.Headers.Add(MessageHeaders.Name, messageName);
        }
    }

    /// <summary>
    /// Gets the message name from the header propagation store.
    /// </summary>
    /// <param name="headerPropagationStore">The header propagation store.</param>
    /// <returns>The message name, or null if not found.</returns>
    public static string? GetMessageName(this HeaderPropagationStore headerPropagationStore)
    {
        headerPropagationStore.Headers.TryGetValue(MessageHeaders.Name, out var name);
        return name;
    }

    /// <summary>
    /// Adds a message type to the header propagation store.
    /// </summary>
    /// <param name="headerPropagationStore">The header propagation store.</param>
    /// <param name="messageType">The message type.</param>
    public static void AddMessageType(
        this HeaderPropagationStore headerPropagationStore,
        string messageType)
    {
        if (!headerPropagationStore.Headers.ContainsKey(MessageHeaders.Type))
        {
            headerPropagationStore.Headers.Add(MessageHeaders.Type, messageType);
        }
    }

    /// <summary>
    /// Gets the message type from the header propagation store.
    /// </summary>
    /// <param name="headerPropagationStore">The header propagation store.</param>
    /// <returns>The message type, or null if not found.</returns>
    public static string? GetMessageType(this HeaderPropagationStore headerPropagationStore)
    {
        headerPropagationStore.Headers.TryGetValue(MessageHeaders.Type, out var type);
        return type;
    }

    /// <summary>
    /// Adds a message ID to the header propagation store.
    /// </summary>
    /// <param name="headerPropagationStore">The header propagation store.</param>
    /// <param name="messageId">The message ID.</param>
    public static void AddMessageId(
        this HeaderPropagationStore headerPropagationStore, 
        Guid messageId)
    {
        if (!headerPropagationStore.Headers.ContainsKey(MessageHeaders.MessageId))
        {
            headerPropagationStore.Headers.Add(MessageHeaders.MessageId, messageId.ToString());
        }
    }

    /// <summary>
    /// Gets the message ID from the header propagation store.
    /// </summary>
    /// <param name="headerPropagationStore">The header propagation store.</param>
    /// <returns>The message ID, or null if not found.</returns>
    public static Guid? GetMessageId(this HeaderPropagationStore headerPropagationStore)
    {
        headerPropagationStore.Headers.TryGetValue(MessageHeaders.MessageId, out var messageId);
        return string.IsNullOrEmpty(messageId) ? null : Guid.Parse(messageId!);
    }

    /// <summary>
    /// Adds a causation ID to the header propagation store.
    /// </summary>
    /// <param name="headerPropagationStore">The header propagation store.</param>
    /// <param name="causationId">The causation ID.</param>
    public static void AddCausationId(
        this HeaderPropagationStore headerPropagationStore,
        Guid causationId)
    {
        if (!headerPropagationStore.Headers.ContainsKey(MessageHeaders.CausationId))
        {
            headerPropagationStore.Headers.Add(MessageHeaders.CausationId, causationId.ToString());
        }
    }

    /// <summary>
    /// Gets the causation ID from the header propagation store.
    /// </summary>
    /// <param name="headerPropagationStore">The header propagation store.</param>
    /// <returns>The causation ID, or null if not found.</returns>
    public static Guid? GetCausationId(this HeaderPropagationStore headerPropagationStore)
    {
        headerPropagationStore.Headers.TryGetValue(MessageHeaders.CausationId, out var causationId);
        return string.IsNullOrEmpty(causationId) ? null : Guid.Parse(causationId!);
    }

    /// <summary>
    /// Adds the created time (in Unix timestamp format) to the header propagation store.
    /// </summary>
    /// <param name="headerPropagationStore">The header propagation store.</param>
    /// <param name="created">The created time.</param>
    public static void AddCreatedUnixTime(
        this HeaderPropagationStore headerPropagationStore,
        long created)
    {
        if (!headerPropagationStore.Headers.ContainsKey(MessageHeaders.Created))
        {
            headerPropagationStore.Headers.Add(MessageHeaders.Created, created.ToString());
        }
    }

    /// <summary>
    /// Gets the created time (in Unix timestamp format) from the header propagation store.
    /// </summary>
    /// <param name="headerPropagationStore">The header propagation store.</param>
    /// <returns>The created time, or null if not found.</returns>
    public static long? GetCreatedUnixTime(this HeaderPropagationStore headerPropagationStore)
    {
        headerPropagationStore.Headers.TryGetValue(MessageHeaders.Created, out var created);
        return string.IsNullOrEmpty(created) ? null : long.Parse(created);
    }
}