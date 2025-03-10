using BuildingBlocks.Abstractions.Events;
using FoodDelivery.Services.Customers.Products.Features.CreatingProduct.v1.Events.Integration.External;
using FoodDelivery.Services.Customers.Products.Features.ReplenishingProductStock.v1.Events.Integration.External;
using FoodDelivery.Services.Shared.Catalogs.Products.Events.V1.Integration;
using Humanizer;
using MassTransit;
using RabbitMQ.Client;

namespace FoodDelivery.Services.Customers.Products;

internal static class MassTransitExtensions
{
    internal static void ConfigureProductMessagesTopology(
        this IRabbitMqBusFactoryConfigurator cfg,
        IBusRegistrationContext context
    )
    {
        // we configured some shared settings for all receive endpoint message in masstransit consuming topologies

        // // This `queueName` creates an `intermediary exchange` (default is Fan-out, but we can change it with re.ExchangeType) with the same queue named which bound to this exchange
        // cfg.ReceiveEndpoint(
        //     nameof(ProductStockReplenishedV1).Underscore(),
        //     re =>
        //     {
        //         re.Durable = true;
        //
        //         // set intermediate exchange type
        //         // intermediate exchange name will be the same as queue name
        //         re.ExchangeType = ExchangeType.Fan-out;
        //
        //         // a replicated queue to provide high availability and data safety. available in RMQ 3.8+
        //         re.SetQuorumQueue();
        //
        //         // with setting `ConfigureConsumeTopology` to `false`, we should create `primary exchange` and its bounded exchange manually with using `re.Bind` otherwise with `ConfigureConsumeTopology=true` it get publish topology for message type `T` with `_publishTopology.GetMessageTopology<T>()` and use its ExchangeType and ExchangeName based ofo default EntityFormatter
        //         // indicate whether the topic or exchange for the message type should be created and subscribed to the queue when consumed on a reception endpoint.
        //          re.ConfigureConsumeTopology = false;
        //
        //         // masstransit uses `wire-tapping` pattern for defining exchanges. Primary exchange will send the message to intermediary fanout exchange
        //         // setup primary exchange and its type from message type and receive-endpoint formatter
        //         re.Bind<IEventEnvelope<ProductStockReplenishedV1>>(e =>
        //         {
        //             e.RoutingKey = nameof(ProductStockReplenishedV1).Underscore();
        //             e.ExchangeType = ExchangeType.Direct;
        //         });
        //
        //         re.ConfigureConsumer<ProductStockReplenishedConsumer>(context);
        //
        //         re.RethrowFaultedMessages();
        //     }
        // );
        //
        // // This `queueName` creates an `intermediary exchange` (default is Fan-out, but we can change it with re.ExchangeType) with the same queue named which bound to this exchange
        // cfg.ReceiveEndpoint(
        //     nameof(ProductCreatedV1).Underscore(),
        //     re =>
        //     {
        //         re.Durable = true;
        //
        //         // set intermediate exchange type
        //         // intermediate exchange name will be the same as queue name
        //         re.ExchangeType = ExchangeType.Fanout;
        //
        //         // a replicated queue to provide high availability and data safety. available in RMQ 3.8+
        //         re.SetQuorumQueue();
        //
        //         // with setting `ConfigureConsumeTopology` to `false`, we should create `primary exchange` and its bounded exchange manually with using `re.Bind` otherwise with `ConfigureConsumeTopology=true` it get publish topology for message type `T` with `_publishTopology.GetMessageTopology<T>()` and use its ExchangeType and ExchangeName based ofo default EntityFormatter
        //         re.ConfigureConsumeTopology = false;
        //
        //         // masstransit uses `wire-tapping` pattern for defining exchanges. Primary exchange will send the message to intermediary fanout exchange
        //         // setup primary exchange and its type
        //         re.Bind<IEventEnvelope<ProductCreatedV1>>(e =>
        //         {
        //             e.RoutingKey = nameof(ProductCreatedV1).Underscore();
        //             e.ExchangeType = ExchangeType.Direct;
        //         });
        //
        //         re.ConfigureConsumer<ProductCreatedConsumer>(context);
        //
        //         re.RethrowFaultedMessages();
        //     }
        // );
    }
}
