using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;

namespace Omegapoint.Azure.Infrastructure.Cosmos.Extensions;

public static class FeedIteratorExtensions
{
    /// <summary>
    /// Convert a feed iterator to IAsyncEnumerable
    /// </summary>
    /// <typeparam name="T">The object type to return as an IAsyncEnumerable{T}</typeparam>
    /// <param name="feedIterator">the FeedIterator{T} to be converted</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> representing request cancellation.</param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException">feedIterator is null</exception>
    public static async IAsyncEnumerable<T> ToAsyncEnumerable<T>(
        this FeedIterator<T> feedIterator,
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        foreach (T item in await GetItemsFromFeedIterator(feedIterator, cancellationToken))
        {
            yield return item;
        }
    }

    /// <summary>
    /// Returns the first element of the feed iterator, or a default value if the feed iterator has no results.
    /// </summary>
    /// <param name="feedIterator">the FeedIterator{T} to be converted</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> representing request cancellation.</param>
    /// <typeparam name="T">The object type to return</typeparam>
    /// <returns>A <see cref="T:System.Threading.Tasks.Task`1" /> that represents the asynchronous creation.</returns>
    /// <exception cref="ArgumentNullException">feedIterator is null</exception>
    public static async Task<T> FirstOrDefault<T>(
        this FeedIterator<T> feedIterator,
        CancellationToken cancellationToken)
    {
        IEnumerable<T> enumerable = await GetItemsFromFeedIterator(feedIterator, cancellationToken);
        return enumerable.FirstOrDefault();
    }

    /// <summary>
    /// Determines whether the feed iterator contains any elements.
    /// </summary>
    /// <param name="feedIterator">the FeedIterator{T} to be converted</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> representing request cancellation.</param>
    /// <typeparam name="T">The type of the elements of source.</typeparam>
    /// <returns>true if the source sequence contains any elements; otherwise, false.</returns>
    /// <exception cref="ArgumentNullException">feedIterator is null</exception>
    public static async Task<bool> Any<T>(this FeedIterator<T> feedIterator, CancellationToken cancellationToken)
    {
        IEnumerable<T> enumerable = await GetItemsFromFeedIterator(feedIterator, cancellationToken);
        return enumerable.Any();
    }

    /// <summary>
    /// Returns all elements of the feed iterator, or a empty list if nothing was found.
    /// </summary>
    /// <param name="feedIterator">the FeedIterator{T} to be converted</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> representing request cancellation.</param>
    /// <typeparam name="T">The object type to return</typeparam>
    /// <returns>A <see cref="T:System.Threading.Tasks.Task`1" /> that represents the asynchronous creation.</returns>
    /// <exception cref="ArgumentNullException">feedIterator is null</exception>
    public static async Task<IEnumerable<T>> All<T>(this FeedIterator<T> feedIterator,
        CancellationToken cancellationToken)
    {
        return await GetItemsFromFeedIterator(feedIterator, cancellationToken);
    }

    private static async Task<IEnumerable<T>> GetItemsFromFeedIterator<T>(
        FeedIterator<T> feedIterator,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(feedIterator);
        var entities = new List<T>();
        while (feedIterator.HasMoreResults)
        {
            FeedResponse<T> response = await feedIterator.ReadNextAsync(cancellationToken);
            entities.AddRange(response);
        }

        return entities;
    }
}
