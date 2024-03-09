using System;
using Microsoft.Azure.Cosmos;

namespace Omegapoint.Azure.Infrastructure.Cosmos.Config;

public sealed record CosmosConfiguration
{
    public CosmosConfiguration(CosmosClient cosmosClient, CosmosDatabaseId databaseId, CosmosContainerId containerId)
    {
        ArgumentNullException.ThrowIfNull(cosmosClient);
        ArgumentNullException.ThrowIfNull(databaseId);
        ArgumentNullException.ThrowIfNull(containerId);
        CosmosClient = cosmosClient;
        DatabaseId = databaseId;
        ContainerId = containerId;
    }

    public CosmosClient CosmosClient { get; }

    public CosmosDatabaseId DatabaseId { get; }

    public CosmosContainerId ContainerId { get; }
}
