using Microsoft.Extensions.Configuration;
using Omegapoint.Azure.Infrastructure.Exceptions;

namespace Omegapoint.Azure.Infrastructure.ServiceBus.Config;

public record FullyQualifiedNamespace
{
    private FullyQualifiedNamespace(string value)
    {
        AssertValid(value);
        Value = value;
    }

    public static FullyQualifiedNamespace CreateFromConfig(IConfiguration configuration)
    {
        return new FullyQualifiedNamespace(configuration
            .GetSection("ServiceBusConnection")
            .GetValue<string>("fullyQualifiedNamespace", null));
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
