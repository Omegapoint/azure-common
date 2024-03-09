namespace Omegapoint.Azure.Infrastructure.ServiceBus.Messages;

// ReSharper disable once InconsistentNaming
public interface IServiceBusMessage<out ID>
{
    ID Id { get; }
    byte[] Body { get; }
}
