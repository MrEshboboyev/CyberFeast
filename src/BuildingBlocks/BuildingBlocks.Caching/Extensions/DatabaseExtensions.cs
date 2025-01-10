using Humanizer;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using StackExchange.Redis;

namespace BuildingBlocks.Caching.Extensions;

/// <summary>
/// Provides extension methods for publishing and subscribing to messages using Redis.
/// </summary>
public static class DatabaseExtensions
{
    /// <summary>
    /// Publishes a message to the specified Redis channel.
    /// </summary>
    /// <typeparam name="T">The type of the message.</typeparam>
    /// <param name="database">The Redis database.</param>
    /// <param name="channelName">The name of the Redis channel.</param>
    /// <param name="data">The message data.</param>
    public static async Task PublishMessage<T>(
        this IDatabase database,
        string channelName,
        T data)
    {
        var jsonData = JsonConvert.SerializeObject(
            data,
            new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            }
        );

        await database.PublishAsync(channelName, jsonData);
    }

    /// <summary>
    /// Publishes a message to a Redis channel named after the type of the message.
    /// </summary>
    /// <typeparam name="T">The type of the message.</typeparam>
    /// <param name="database">The Redis database.</param>
    /// <param name="data">The message data.</param>
    public static async Task PublishMessage<T>(this IDatabase database, T data)
    {
        var channelName = $"{typeof(T).Name.Underscore()}_channel";
        await database.PublishMessage(channelName, data);
    }

    /// <summary>
    /// Publishes a message to the specified Redis channel within a transaction.
    /// </summary>
    /// <typeparam name="T">The type of the message.</typeparam>
    /// <param name="transaction">The Redis transaction.</param>
    /// <param name="channelName">The name of the Redis channel.</param>
    /// <param name="data">The message data.</param>
    public static async Task PublishMessage<T>(
        this ITransaction transaction,
        string channelName,
        T data)
    {
        var jsonData = JsonConvert.SerializeObject(
            data,
            new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            }
        );

        await transaction.PublishAsync(channelName, jsonData);
    }

    /// <summary>
    /// Publishes a message to a Redis channel named after the type of the message within a transaction.
    /// </summary>
    /// <typeparam name="T">The type of the message.</typeparam>
    /// <param name="transaction">The Redis transaction.</param>
    /// <param name="data">The message data.</param>
    public static async Task PublishMessage<T>(this ITransaction transaction, T data)
    {
        var channelName = $"{typeof(T).Name.Underscore()}_channel";
        await transaction.PublishMessage(channelName, data);
    }

    /// <summary>
    /// Subscribes to a Redis channel and handles incoming messages using the specified handler.
    /// </summary>
    /// <typeparam name="T">The type of the message.</typeparam>
    /// <param name="database">The Redis database.</param>
    /// <param name="channelName">The name of the Redis channel.</param>
    /// <param name="handler">The handler to process incoming messages.</param>
    public static async Task SubscribeMessage<T>(
        this IDatabase database,
        string channelName,
        Func<string, T, Task> handler
    )
    {
        var channelMessageQueue = await database
            .Multiplexer.GetSubscriber().SubscribeAsync(channelName);

        channelMessageQueue.OnMessage(async channelMessage =>
        {
            var message = JsonConvert.DeserializeObject<T>(channelMessage.Message!);
            await handler(channelMessage.Channel!, message!);
        });
    }

    /// <summary>
    /// Subscribes to a Redis channel and handles incoming messages using the specified handler.
    /// </summary>
    /// <typeparam name="T">The type of the message.</typeparam>
    /// <param name="database">The Redis database.</param>
    /// <param name="channelName">The name of the Redis channel.</param>
    /// <param name="handler">The handler to process incoming messages.</param>
    public static async Task SubscribeMessage<T>(
        this IDatabase database,
        string channelName,
        Func<T, Task> handler)
    {
        var channelMessageQueue = await database
            .Multiplexer.GetSubscriber().SubscribeAsync(channelName);

        channelMessageQueue.OnMessage(async channelMessage =>
        {
            var message = JsonConvert.DeserializeObject<T>(channelMessage.Message!);
            await handler(message!);
        });
    }

    /// <summary>
    /// Subscribes to a Redis channel named after the type of the message and handles incoming messages using the specified handler.
    /// </summary>
    /// <typeparam name="T">The type of the message.</typeparam>
    /// <param name="database">The Redis database.</param>
    /// <param name="handler">The handler to process incoming messages.</param>
    public static async Task SubscribeMessage<T>(
        this IDatabase database,
        Func<string, T, Task> handler)
    {
        var channelName = $"{typeof(T).Name.Underscore()}_channel";

        await database.SubscribeMessage(channelName, handler);
    }
}