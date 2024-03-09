using System;
using System.Threading;
using System.Threading.Tasks;
using Omegapoint.Azure.Infrastructure.ServiceBus.Messages;

namespace Omegapoint.Azure.Infrastructure.ServiceBus;

// ReSharper disable once InconsistentNaming
public interface IServiceBusMessageHandler<in TMessage, ID> : IDisposable where TMessage : IServiceBusMessage<ID>
{
    Task<long> ScheduleMessage(TMessage message, DateTimeOffset scheduledEnqueueTime,
        CancellationToken cancellationToken = default);

    Task SendMessage(TMessage message, CancellationToken cancellationToken = default);
}
