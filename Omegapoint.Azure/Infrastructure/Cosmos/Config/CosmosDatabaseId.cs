using Microsoft.Extensions.Configuration;
using Omegapoint.Azure.Infrastructure.Exceptions;

namespace Omegapoint.Azure.Infrastructure.Cosmos.Config;

public record CosmosDatabaseId
{
    private CosmosDatabaseId(string value)
    {
        AssertValid(value);
        Value = value;
    }

    public static CosmosDatabaseId CreateFromConfig(IConfiguration configuration, string key)
    {
        return new CosmosDatabaseId(configuration.GetValue<string>(key, null));
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
