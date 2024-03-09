using System;

namespace Omegapoint.Azure.Infrastructure.Cosmos.Exceptions;

// ReSharper disable once InconsistentNaming
public class NotFoundException<ID> : Exception
{
    public NotFoundException(ID id) : base($"{GetMessage(id)}")
    {
    }

    public NotFoundException(ID id, Exception innerException) : base(GetMessage(id), innerException)
    {
    }

    private static string GetMessage(ID id) => $"Entity with id '{id}' was not found.";
}
