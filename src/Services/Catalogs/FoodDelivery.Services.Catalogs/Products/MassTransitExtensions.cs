using BuildingBlocks.Abstractions.Events;
using BuildingBlocks.Integration.MassTransit;
using FoodDelivery.Services.Shared.Catalogs.Products.Events.V1.Integration;
using Humanizer;
using MassTransit;

namespace FoodDelivery.Services.Catalogs.Products;

public static class MassTransitExtensions
{
    internal static void ConfigureProductMessagesTopology(this IRabbitMqBusFactoryConfigurator cfg)
    {
        cfg.Message<IEventEnvelope<ProductCreatedV1>>(e =>
        {
            // Name of the `primary exchange` for type based message publishing and sending
            // e.SetEntityName($"{nameof(ProductCreatedV1).Underscore()}{MessagingConstants.PrimaryExchangePostfix}");
            e.SetEntityNameFormatter(new CustomEntityNameFormatter<IEventEnvelope<ProductCreatedV1>>());
        });

        // configuration for MessagePublishTopologyConfiguration and using IPublishEndpoint
        cfg.Publish<IEventEnvelope<ProductCreatedV1>>(e =>
        {
            // we configured some shared settings for all publish message in masstransit publish topologies

            // // setup primary exchange
            // e.Durable = true;
            // e.ExchangeType = ExchangeType.Direct;
        });

        cfg.Send<IEventEnvelope<ProductCreatedV1>>(e =>
        {
            // route by message type to binding fan-out exchange (exchange to exchange binding)
            e.UseRoutingKeyFormatter(context => context.Message.GetType().Name.Underscore());
        });

        cfg.Message<IEventEnvelope<ProductStockDebitedV1>>(e =>
        {
            // Name of the `primary exchange` for type based message publishing and sending
            // e.SetEntityName($"{nameof(ProductStockDebitedV1).Underscore()}{MessagingConstants.PrimaryExchangePostfix}");
            e.SetEntityNameFormatter(new CustomEntityNameFormatter<IEventEnvelope<ProductStockDebitedV1>>());
        });

        // configuration for MessagePublishTopologyConfiguration and using IPublishEndpoint
        cfg.Publish<IEventEnvelope<ProductStockDebitedV1>>(e =>
        {
            // we configured some shared settings for all publish message in masstransit publish topologies

            // // setup primary exchange
            // e.Durable = true;
            // e.ExchangeType = ExchangeType.Direct;
        });

        cfg.Send<IEventEnvelope<ProductStockDebitedV1>>(e =>
        {
            // route by message type to binding fan-out exchange (exchange to exchange binding)
            e.UseRoutingKeyFormatter(context => context.Message.GetType().Name.Underscore());
        });

        cfg.Message<IEventEnvelope<ProductStockReplenishedV1>>(e =>
        {
            // Name of the `primary exchange` for type based message publishing and sending
            // e.SetEntityName($"{nameof(ProductStockDebitedV1).Underscore()}{MessagingConstants.PrimaryExchangePostfix}");
            e.SetEntityNameFormatter(new CustomEntityNameFormatter<IEventEnvelope<ProductStockReplenishedV1>>());
        });

        cfg.Publish<IEventEnvelope<ProductStockReplenishedV1>>(e =>
        {
            // we configured some shared settings for all publish message in masstransit publish topologies

            // // setup primary exchange
            // e.Durable = true;
            // e.ExchangeType = ExchangeType.Direct;
        });

        cfg.Send<IEventEnvelope<ProductStockReplenishedV1>>(e =>
        {
            // route by message type to binding fan-out exchange (exchange to exchange binding)
            e.UseRoutingKeyFormatter(context => context.Message.GetType().Name.Underscore());
        });
    }
}
