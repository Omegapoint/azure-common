using System;
using System.Threading;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Logging;
using Omegapoint.Azure.Infrastructure.ServiceBus.Config;
using Omegapoint.Azure.Infrastructure.ServiceBus.Messages;

namespace Omegapoint.Azure.Infrastructure.ServiceBus;

// ReSharper disable once InconsistentNaming
public abstract record ServiceBusMessageHandler<TMessage, ID> : IServiceBusMessageHandler<TMessage, ID>
    where TMessage : IServiceBusMessage<ID>
{
    private const string ContentTypeApplicationJson = "application/json";
    protected abstract ILogger Logger { get; init; }
    private readonly ServiceBusSender _sender;
    private readonly ServiceBusReceiver _receiver;

    protected ServiceBusMessageHandler(ServiceBusConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(configuration);
        _sender = configuration.Client.CreateSender(configuration.QueueOrTopicName.Value);
        _receiver = configuration.Client.CreateReceiver(configuration.QueueOrTopicName.Value);
    }

    public virtual async Task<long> ScheduleMessage(TMessage message, DateTimeOffset scheduledEnqueueTime,
        CancellationToken cancellationToken = default)
    {
        var serviceBusMessage = new ServiceBusMessage(message.Body)
        {
            ContentType = ContentTypeApplicationJson,
            MessageId = message.Id.ToString()
        };
        long sequenceNumber = await _sender.ScheduleMessageAsync(
            serviceBusMessage,
            scheduledEnqueueTime,
            cancellationToken);
        Logger.LogInformation("Message with id {MessageId} " +
                              "was scheduled to be enqueued at {ScheduledEnqueueTime}. " +
                              "Message sequence number: {SequenceNumber}",
            message.Id, scheduledEnqueueTime, sequenceNumber);
        return sequenceNumber;
    }

    public virtual async Task SendMessage(TMessage message, CancellationToken cancellationToken = default)
    {
        var serviceBusMessage = new ServiceBusMessage(message.Body)
        {
            ContentType = ContentTypeApplicationJson,
            MessageId = message.Id.ToString()
        };
        await _sender.SendMessageAsync(
            serviceBusMessage,
            cancellationToken);
        Logger.LogInformation("Message with id {MessageId} was sent", serviceBusMessage.MessageId);
    }

    public async void Dispose()
    {
        await Dispose(true);
        GC.SuppressFinalize(this);
    }

    private async Task Dispose(bool disposing)
    {
        if (disposing)
        {
            // dispose objects
            await _sender.DisposeAsync();
            await _receiver.DisposeAsync();
        }
    }

    ~ServiceBusMessageHandler()
    {
        Dispose(false).GetAwaiter().GetResult();
    }
}
