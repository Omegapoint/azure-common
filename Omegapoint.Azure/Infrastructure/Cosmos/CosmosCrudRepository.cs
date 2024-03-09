using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;
using Microsoft.Extensions.Logging;
using Omegapoint.Azure.Infrastructure.Cosmos.Config;
using Omegapoint.Azure.Infrastructure.Cosmos.Entities;
using Omegapoint.Azure.Infrastructure.Cosmos.Exceptions;
using Omegapoint.Azure.Infrastructure.Cosmos.Extensions;

namespace Omegapoint.Azure.Infrastructure.Cosmos;

// ReSharper disable once InconsistentNaming
public abstract record CosmosCrudRepository<TEntity, ID> : ICosmosRepository<TEntity, ID>
    where TEntity : ICosmosEntity<ID> where ID : class
{
    private Container _container;
    protected abstract ILogger Logger { get; init; }

    protected CosmosCrudRepository(CosmosConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(configuration);
        CosmosClient cosmosClient = configuration.CosmosClient;
        _container = cosmosClient.GetContainer(configuration.DatabaseId.Value, configuration.ContainerId.Value);
    }

    public virtual async Task<TEntity> Save(TEntity entity, CancellationToken cancellationToken = default)
    {
        var response = await _container.UpsertItemAsync(
            item: entity,
            partitionKey: entity.PartitionKey,
            new ItemRequestOptions { IfMatchEtag = entity.ETag },
            cancellationToken: cancellationToken);

        return response.Resource;
    }

    public virtual async IAsyncEnumerable<TEntity> SaveAll(IEnumerable<TEntity> entities,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        foreach (TEntity entity in entities)
        {
            yield return await Save(entity, cancellationToken);
        }
    }

    public virtual async Task<TEntity> FindById(ID id, CancellationToken cancellationToken = default)
    {
        TEntity entity;
        try
        {
            using var linqFeed = FilterById(id);
            entity = await linqFeed.FirstOrDefault(cancellationToken);
        }
        catch (CosmosException e) when (e.StatusCode == HttpStatusCode.NotFound)
        {
            throw new NotFoundException<ID>(id, e);
        }

        if (entity == null)
        {
            throw new NotFoundException<ID>(id);
        }

        return entity;
    }

    public virtual async Task<bool> ExistsById(ID id, CancellationToken cancellationToken = default)
    {
        try
        {
            using FeedIterator<TEntity> linqFeed = FilterById(id);
            return await linqFeed.Any(cancellationToken);
        }
        catch (CosmosException e) when (e.StatusCode == HttpStatusCode.NotFound)
        {
            Logger.LogInformation("Entity with id '{EntityId}' does not exists", id);
            return false;
        }
    }

    public virtual async IAsyncEnumerable<TEntity> FindAll(
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        await foreach (TEntity entity in _container
                           .GetItemLinqQueryable<TEntity>()
                           .ToFeedIterator()
                           .ToAsyncEnumerable(cancellationToken))
        {
            yield return entity;
        }
    }

    public virtual async IAsyncEnumerable<TEntity> FindAllById(
        IEnumerable<ID> ids,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        foreach (ID id in ids)
        {
            using FeedIterator<TEntity> linqFeed = FilterById(id);
            await foreach (TEntity entity in linqFeed.ToAsyncEnumerable(cancellationToken))
            {
                yield return entity;
            }
        }
    }

    public virtual async Task<long> Count(CancellationToken cancellationToken = default)
    {
        IEnumerable<TEntity> all = await _container
            .GetItemLinqQueryable<TEntity>()
            .ToFeedIterator()
            .All(cancellationToken);

        return all.Count();
    }

    public virtual async Task DeleteById(ID id, CancellationToken cancellationToken = default)
    {
        TEntity entity = await FindById(id, cancellationToken);
        await Delete(entity, cancellationToken);
    }

    public virtual async Task Delete(TEntity entity, CancellationToken cancellationToken = default)
    {
        try
        {
            await _container.DeleteItemAsync<TEntity>(
                id: entity.Id.ToString(),
                partitionKey: entity.PartitionKey,
                cancellationToken: cancellationToken
            );
        }
        catch (CosmosException e) when (e.StatusCode == HttpStatusCode.NotFound)
        {
            Logger.LogInformation("Entity with id '{EntityId}' does not exists or has already been removed",
                entity.Id.ToString());
        }
    }

    public async Task DeleteAllById(IEnumerable<ID> ids, CancellationToken cancellationToken = default)
    {
        await foreach (TEntity entity in FindAllById(ids, cancellationToken))
        {
            await Delete(entity, cancellationToken);
        }
    }

    public async Task DeleteAll(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
    {
        foreach (TEntity entity in entities)
        {
            await Delete(entity, cancellationToken);
        }
    }

    public async Task DeleteAll(CancellationToken cancellationToken = default)
    {
        await foreach (TEntity entity in _container
                           .GetItemLinqQueryable<TEntity>()
                           .ToFeedIterator()
                           .ToAsyncEnumerable(cancellationToken))
        {
            await Delete(entity, cancellationToken);
        }
    }

    private FeedIterator<TEntity> FilterById(ID id)
    {
        const string query = "SELECT * FROM c WHERE c.id = @id";
        QueryDefinition queryDefinition = new QueryDefinition(query).WithParameter("@id", id);
        FeedIterator<TEntity> linqFeed = _container.GetItemQueryIterator<TEntity>(queryDefinition);
        return linqFeed;
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            _container = null;
        }
    }
}
