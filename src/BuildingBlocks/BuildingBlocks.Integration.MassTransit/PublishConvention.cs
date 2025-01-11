using MassTransit;
using MassTransit.Configuration;
using MassTransit.RabbitMqTransport.Configuration;
using RabbitMQ.Client;

namespace BuildingBlocks.Integration.MassTransit;

/// <summary>
/// Provides a custom publish topology convention for message types.
/// </summary>
public class CustomPublishTopologyConvention : IPublishTopologyConvention
{
    /// <summary>
    /// Tries to get a custom message publish topology convention for the specified message type.
    /// </summary>
    public bool TryGetMessagePublishTopologyConvention<T>(out IMessagePublishTopologyConvention<T> convention)
        where T : class
    {
        convention = new CustomMessagePublishTopologyConvention<T>();
        return true;
    }
}

/// <summary>
/// Provides a custom message publish topology convention for a specific message type.
/// </summary>
/// <typeparam name="T">The type of the message.</typeparam>
public class CustomMessagePublishTopologyConvention<T> : IMessagePublishTopologyConvention<T>
    where T : class
{
    /// <summary>
    /// Tries to get a custom message publish topology for the specified message type.
    /// </summary>
    public bool TryGetMessagePublishTopology(out IMessagePublishTopology<T> messagePublishTopology)
    {
        messagePublishTopology = new CustomMessagePublishTopology<T>();
        return true;
    }

    /// <summary>
    /// Tries to get a custom message publish topology convention for the specified message type.
    /// </summary>
    public bool TryGetMessagePublishTopologyConvention<T1>(out IMessagePublishTopologyConvention<T1> convention)
        where T1 : class
    {
        if (typeof(T1) == typeof(T))
        {
            convention = (IMessagePublishTopologyConvention<T1>)this;
            return true;
        }

        convention = null;
        return false;
    }
}

/// <summary>
/// Represents the custom publish topology for a specific message type.
/// </summary>
/// <typeparam name="T">The type of the message.</typeparam>
public class CustomMessagePublishTopology<T>(bool exclude = false) : IMessagePublishTopology<T>
    where T : class
{
    /// <summary>
    /// Gets a value indicating whether this topology should be excluded.
    /// </summary>
    public bool Exclude { get; private set; } = exclude;

    /// <summary>
    /// Applies the custom publish topology using the provided builder.
    /// </summary>
    public void Apply(ITopologyPipeBuilder<PublishContext<T>> builder)
    {
        builder.AddFilter(new CustomPublishTopologyFilter<T>());
    }

    /// <summary>
    /// Tries to get the publishing address by adding the exchange name to the base address.
    /// </summary>
    public bool TryGetPublishAddress(Uri baseAddress, out Uri? publishAddress)
    {
        var exchangeName = typeof(T).Name.ToLowerInvariant();

        if (baseAddress != null)
        {
            var builder = new UriBuilder(baseAddress) { Path = $"exchange/{exchangeName}" };
            publishAddress = builder.Uri;
            return true;
        }

        publishAddress = null;
        return false;
    }
}

/// <summary>
/// Provides a custom filter for configuring RabbitMQ exchanges when publishing messages.
/// </summary>
/// <typeparam name="T">The type of the message.</typeparam>
public class CustomPublishTopologyFilter<T> : IFilter<PublishContext<T>>
    where T : class
{
    public async Task Send(PublishContext<T> context, IPipe<PublishContext<T>> next)
    {
        var exchange = context.GetPayload<RabbitMqExchangeConfigurator>();

        exchange.Durable = true;
        exchange.ExchangeType = ExchangeType.Direct;

        await next.Send(context);
    }

    public void Probe(ProbeContext context)
    {
        context.CreateFilterScope("CustomPublishTopologyFilter");
    }
}