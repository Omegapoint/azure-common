using Microsoft.Azure.Cosmos;

namespace Omegapoint.Azure.Infrastructure.Cosmos.Entities;

// ReSharper disable once InconsistentNaming
public interface ICosmosEntity<out ID> where ID : class
{
    /// <summary>
    /// Default document entity identifier
    /// </summary>
    public ID Id { get; }

    public PartitionKey PartitionKey { get; }

    /// <summary>
    /// e-tag
    /// </summary>
    public string ETag { get; }
}
