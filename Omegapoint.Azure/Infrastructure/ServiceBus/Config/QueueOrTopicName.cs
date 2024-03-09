using Microsoft.Extensions.Configuration;
using Omegapoint.Azure.Infrastructure.Exceptions;

namespace Omegapoint.Azure.Infrastructure.ServiceBus.Config;

public record QueueOrTopicName
{
    private QueueOrTopicName(string value)
    {
        AssertValid(value);
        Value = value;
    }

    public static QueueOrTopicName CreateFromConfig(IConfiguration configuration, string key)
    {
        return new QueueOrTopicName(configuration.GetValue<string>(key, null));
    }

    public string Value { get; }

    private static bool IsValid(string value)
    {
        return !string.IsNullOrWhiteSpace(value);
    }

    private static void AssertValid(string value)
    {
        if (!IsValid(value))
        {
            throw new ConfigurationArgumentException<string>(value);
        }
    }
}
