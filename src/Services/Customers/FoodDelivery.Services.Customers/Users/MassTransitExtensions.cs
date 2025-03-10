using BuildingBlocks.Abstractions.Events;
using FoodDelivery.Services.Customers.Users.Features.RegisteringUser.v1.Events.Integration.External;
using FoodDelivery.Services.Shared.Identity.Users.Events.V1.Integration;
using Humanizer;
using MassTransit;
using RabbitMQ.Client;

namespace FoodDelivery.Services.Customers.Users;

internal static class MassTransitExtensions
{
    internal static void ConfigureUsersMessagesTopology(
        this IRabbitMqBusFactoryConfigurator cfg,
        IBusRegistrationContext context
    )
    {
        // we configured some shared settings for all publish message in masstransit publish topologies

        // // This `queueName` creates an intermediary exchange (default is Fanout, but we can change it with re.ExchangeType) with the same queue named which bound to this exchange
        // cfg.ReceiveEndpoint(
        //     queueName: nameof(UserRegisteredV1).Underscore(),
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
        //         // indicate whether the topic or exchange for the message type should be created and subscribed to the queue when consumed on a reception endpoint.
        //         re.ConfigureConsumeTopology = true;
        //
        //         // // masstransit uses `wire-tapping` pattern for defining exchanges. Primary exchange will send the message to intermediary fanout exchange
        //         // // setup primary exchange and its type from message type and receive-endpoint formatter
        //         // re.Bind<IEventEnvelope<UserRegisteredV1>>(e =>
        //         // {
        //         //     e.RoutingKey = nameof(UserRegisteredV1).Underscore();
        //         //     e.ExchangeType = ExchangeType.Direct;
        //         // });
        //
        //         re.ConfigureConsumer<UserRegisteredConsumer>(context);
        //
        //         re.RethrowFaultedMessages();
        //     }
        // );
    }
}
