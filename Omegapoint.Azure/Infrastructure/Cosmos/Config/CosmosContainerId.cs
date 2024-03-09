using Microsoft.Extensions.Configuration;
using Omegapoint.Azure.Infrastructure.Exceptions;

namespace Omegapoint.Azure.Infrastructure.Cosmos.Config;

public record CosmosContainerId
{
    private CosmosContainerId(string value)
    {
        AssertValid(value);
        Value = value;
    }

    public static CosmosContainerId CreateFromConfig(IConfiguration configuration, string key)
    {
        return new CosmosContainerId(configuration.GetValue<string>(key, null));
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
