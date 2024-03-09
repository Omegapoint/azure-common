using System;

namespace Omegapoint.Azure.Infrastructure.Exceptions;

public class ConfigurationArgumentException<T>(T value) : ArgumentException($"{GetMessage(value)}")
{
    private static string GetMessage(T value) => $"The value '{value}' is not valid.";
}
