using System.Reflection;
using BuildingBlocks.Abstractions.Events;
using BuildingBlocks.Abstractions.Messaging;
using BuildingBlocks.Abstractions.Persistence;
using BuildingBlocks.Core.Events;
using BuildingBlocks.Core.Exception.Types;
using BuildingBlocks.Core.Extensions;
using BuildingBlocks.Core.Extensions.ServiceCollection;
using BuildingBlocks.Core.Messaging;
using BuildingBlocks.Core.Reflection;
using BuildingBlocks.Core.Reflection.Extensions;
using BuildingBlocks.Core.Web.Extensions;
using Humanizer;
using MassTransit;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
using IExternalEventBus = BuildingBlocks.Abstractions.Messaging.IExternalEventBus;

namespace BuildingBlocks.Integration.MassTransit;

public static class DependencyInjectionExtensions
{
    public static WebApplicationBuilder AddCustomMassTransit(
        this WebApplicationBuilder builder,
        Action<IBusRegistrationContext, IRabbitMqBusFactoryConfigurator>? configureMessagesTopologies = null,
        Action<IBusRegistrationConfigurator>? configureBusRegistration = null,
        Action<MessagingOptions>? configureMessagingOptions = null,
        params Assembly[] scanAssemblies
    )
    {
        builder.Services.AddValidationOptions(configureMessagingOptions);
        var messagingOptions = builder.Configuration.BindOptions(configureMessagingOptions);

        var assemblies =
            scanAssemblies.Length != 0
                ? scanAssemblies
                : ReflectionUtilities.GetReferencedAssemblies(
                    Assembly.GetCallingAssembly()).ToArray();

        if (!builder.Environment.IsTest())
        {
            builder.Services.AddMassTransit(ConfiguratorAction);
        }
        else
        {
            builder.Services.AddMassTransitTestHarness(ConfiguratorAction);
        }

        void ConfiguratorAction(IBusRegistrationConfigurator busRegistrationConfigurator)
        {
            busRegistrationConfigurator.ConfigureHealthCheckOptions(options =>
            {
                options.Name = "masstransit";
                options.MinimalFailureStatus = HealthStatus.Unhealthy;
                options.Tags.Add("health");
            });

            configureBusRegistration?.Invoke(busRegistrationConfigurator);

            busRegistrationConfigurator.AddConsumers(assemblies);

            busRegistrationConfigurator.SetEndpointNameFormatter(new SnakeCaseEndpointNameFormatter(false));

            busRegistrationConfigurator.UsingRabbitMq(
                (context, cfg) =>
                {
                    cfg.UseConsumeFilter(typeof(CorrelationConsumeFilter<>), context);

                    cfg.Publish<IIntegrationEvent>(
                        p => p.Exclude = true);
                    cfg.Publish<IntegrationEvent>(
                        p => p.Exclude = true);
                    cfg.Publish<IMessage>(
                        p => p.Exclude = true);
                    cfg.Publish<Message>(
                        p => p.Exclude = true);
                    cfg.Publish<ITransactionRequest>(
                        p => p.Exclude = true);
                    cfg.Publish<IEventEnvelope>(
                        p => p.Exclude = true);

                    if (messagingOptions.AutoConfigEndpoints)
                    {
                        cfg.ConfigureEndpoints(context);
                    }

                    var rabbitMqOptions = builder.Configuration.BindOptions<RabbitMqOptions>();

                    cfg.Host(
                        rabbitMqOptions.Host,
                        rabbitMqOptions.Port,
                        "/",
                        hostConfigurator =>
                        {
                            hostConfigurator.PublisherConfirmation = true;
                            hostConfigurator.Username(rabbitMqOptions.UserName);
                            hostConfigurator.Password(rabbitMqOptions.Password);
                        }
                    );

                    cfg.MessageTopology.SetEntityNameFormatter(new CustomEntityNameFormatter());

                    ApplyMessagesPublishTopology(cfg.PublishTopology, assemblies);
                    ApplyMessagesConsumeTopology(cfg, context, assemblies);
                    ApplyMessagesSendTopology(cfg.SendTopology, assemblies);

                    configureMessagesTopologies?.Invoke(context, cfg);

                    cfg.UseMessageRetry(r => AddRetryConfiguration(r));
                }
            );
        }

        builder
            .Services.AddOptions<MassTransitHostOptions>()
            .Configure(options =>
            {
                options.WaitUntilStarted = true;
                options.StartTimeout = TimeSpan.FromSeconds(30);
                options.StopTimeout = TimeSpan.FromSeconds(60);
            });

        builder
            .Services.AddOptions<HostOptions>()
            .Configure(options =>
            {
                options.StartupTimeout = TimeSpan.FromSeconds(60);
                options.ShutdownTimeout = TimeSpan.FromSeconds(60);
            });

        builder.Services.AddTransient<IExternalEventBus, MassTransitBus>();
        builder.Services.AddTransient<IBusDirectPublisher, MasstransitDirectPublisher>();

        return builder;
    }

    private static void ApplyMessagesSendTopology(
        IRabbitMqSendTopologyConfigurator sendTopology,
        Assembly[] assemblies
    ) { }

    private static void ApplyMessagesConsumeTopology(
        IRabbitMqBusFactoryConfigurator rabbitMqBusFactoryConfigurator,
        IBusRegistrationContext context,
        Assembly[] assemblies
    )
    {
        var consumeTopology = rabbitMqBusFactoryConfigurator.ConsumeTopology;

        var messageTypes = ReflectionUtilities
            .GetAllTypesImplementingInterface<IIntegrationEvent>(assemblies)
            .Where(x => !x.IsGenericType);

        foreach (var messageType in messageTypes)
        {
            var eventEnvelopeInterfaceMessageType = typeof(IEventEnvelope<>).MakeGenericType(messageType);
            var eventEnvelopeInterfaceConfigurator = consumeTopology.GetMessageTopology(
                eventEnvelopeInterfaceMessageType
            );

            eventEnvelopeInterfaceConfigurator.ConfigureConsumeTopology = true;

            var messageConfigurator = consumeTopology.GetMessageTopology(messageType);
            messageConfigurator.ConfigureConsumeTopology = true;

            var eventEnvelopeConsumerInterface = typeof(IConsumer<>)
                .MakeGenericType(eventEnvelopeInterfaceMessageType);
            var envelopeConsumerConcretedTypes = eventEnvelopeConsumerInterface
                .GetAllTypesImplementingInterface(assemblies)
                .Where(x => !x.FullName!.Contains(nameof(MassTransit)));

            var consumerType = envelopeConsumerConcretedTypes.SingleOrDefault();

            if (consumerType is null)
            {
                var messageTypeConsumerInterface = typeof(IConsumer<>).MakeGenericType(messageType);
                var messageTypeConsumerConcretedTypes = messageTypeConsumerInterface
                    .GetAllTypesImplementingInterface(assemblies)
                    .Where(x => !x.FullName!.Contains(nameof(MassTransit)));
                var messageTypeConsumerType = messageTypeConsumerConcretedTypes.SingleOrDefault();

                if (messageTypeConsumerType is null)
                {
                    continue;
                }

                consumerType = messageTypeConsumerType;
            }

            ConfigureMessageReceiveEndpoint(rabbitMqBusFactoryConfigurator, context, messageType, consumerType);
        }
    }

    private static void ConfigureMessageReceiveEndpoint(
        IRabbitMqBusFactoryConfigurator rabbitMqBusFactoryConfigurator,
        IBusRegistrationContext context,
        Type messageType,
        Type consumerType
    )
    {
        rabbitMqBusFactoryConfigurator.ReceiveEndpoint(
            queueName: messageType.Name.Underscore(),
            re =>
            {
                re.Durable = true;

                re.ExchangeType = ExchangeType.Fanout;

                re.SetQuorumQueue();

                re.ConfigureConsumeTopology = true;

                re.ConfigureConsumer(context, consumerType);

                re.RethrowFaultedMessages();
            }
        );
    }

    private static void ApplyMessagesPublishTopology(
        IRabbitMqPublishTopologyConfigurator publishTopology,
        Assembly[] assemblies
    )
    {
        // Get all types that implement the IMessage interface
        var messageTypes = ReflectionUtilities
            .GetAllTypesImplementingInterface<IIntegrationEvent>(assemblies)
            .Where(x => !x.IsGenericType);

        foreach (var messageType in messageTypes)
        {
            var eventEnvelopeInterfaceMessageType = typeof(IEventEnvelope<>).MakeGenericType(messageType);
            var eventEnvelopeInterfaceConfigurator = publishTopology.GetMessageTopology(
                eventEnvelopeInterfaceMessageType
            );

            // setup primary exchange
            eventEnvelopeInterfaceConfigurator.Durable = true;
            eventEnvelopeInterfaceConfigurator.ExchangeType = ExchangeType.Direct;

            var eventEnvelopeMessageType = typeof(EventEnvelope<>).MakeGenericType(messageType);
            var eventEnvelopeMessageTypeConfigurator = publishTopology.GetMessageTopology(eventEnvelopeMessageType);
            eventEnvelopeMessageTypeConfigurator.Durable = true;
            eventEnvelopeMessageTypeConfigurator.ExchangeType = ExchangeType.Direct;

            // none event-envelope message types
            var messageConfigurator = publishTopology.GetMessageTopology(messageType);
            messageConfigurator.Durable = true;
            messageConfigurator.ExchangeType = ExchangeType.Direct;
        }
    }

    private static IRetryConfigurator AddRetryConfiguration(IRetryConfigurator retryConfigurator)
    {
        retryConfigurator
            .Exponential(3, TimeSpan.FromMilliseconds(200), TimeSpan.FromMinutes(120), TimeSpan.FromMilliseconds(200))
            .Ignore<ValidationException>(); // don't retry if we have invalid data and message goes to _error queue masstransit

        return retryConfigurator;
    }
}
