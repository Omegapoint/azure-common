using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Omegapoint.Azure.Infrastructure.Cosmos.Entities;

namespace Omegapoint.Azure.Infrastructure.Cosmos;

// ReSharper disable once InconsistentNaming
public interface ICosmosRepository<TEntity, in ID> : IDisposable where TEntity : ICosmosEntity<ID> where ID : class
{
    Task<TEntity> Save(TEntity entity, CancellationToken cancellationToken = default);
    IAsyncEnumerable<TEntity> SaveAll(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default);
    Task<TEntity> FindById(ID id, CancellationToken cancellationToken = default);
    Task<bool> ExistsById(ID id, CancellationToken cancellationToken = default);
    IAsyncEnumerable<TEntity> FindAll(CancellationToken cancellationToken = default);
    IAsyncEnumerable<TEntity> FindAllById(IEnumerable<ID> ids, CancellationToken cancellationToken = default);
    Task<long> Count(CancellationToken cancellationToken = default);
    Task DeleteById(ID id, CancellationToken cancellationToken = default);
    Task Delete(TEntity entity, CancellationToken cancellationToken = default);
    Task DeleteAllById(IEnumerable<ID> ids, CancellationToken cancellationToken = default);
    Task DeleteAll(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default);
    Task DeleteAll(CancellationToken cancellationToken = default);
}
