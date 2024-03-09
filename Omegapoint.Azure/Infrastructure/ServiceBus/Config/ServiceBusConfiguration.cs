using System;
using Azure.Messaging.ServiceBus;

namespace Omegapoint.Azure.Infrastructure.ServiceBus.Config;

public sealed record ServiceBusConfiguration
{
    public ServiceBusConfiguration(ServiceBusClient serviceBusClient, QueueOrTopicName queueOrTopicName)
    {
        ArgumentNullException.ThrowIfNull(serviceBusClient);
        ArgumentNullException.ThrowIfNull(queueOrTopicName);

        Client = serviceBusClient;
        QueueOrTopicName = queueOrTopicName;
    }

    public ServiceBusClient Client { get; }
    public QueueOrTopicName QueueOrTopicName { get; }
}
