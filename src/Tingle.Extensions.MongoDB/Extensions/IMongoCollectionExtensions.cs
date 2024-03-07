namespace MongoDB.Driver;

/// <summary>
/// Extension methods on <see cref="IMongoCollection{TDocument}"/>
/// </summary>
public static class IMongoCollectionExtensions
{
    /// <summary>
    /// Updates many documents and checks the result.
    /// </summary>
    /// <typeparam name="TDocument"></typeparam>
    /// <param name="collection"></param>
    /// <param name="filter">The filter.</param>
    /// <param name="update">The update.</param>
    /// <param name="options"><see cref="UpdateOptions"/>.</param>
    /// <param name="session">The optional <see cref="IClientSessionHandle"/> to use.</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <exception cref="MongoUpdateConcurrencyException">
    /// The update was not acknowledged.
    /// </exception>
    public static async Task<long> UpdateManyWithResultCheckAsync<TDocument>(this IMongoCollection<TDocument> collection,
                                                                             FilterDefinition<TDocument> filter,
                                                                             UpdateDefinition<TDocument> update,
                                                                             UpdateOptions? options = null,
                                                                             IClientSessionHandle? session = null,
                                                                             CancellationToken cancellationToken = default)
    {
        // Do the actual update
        var ur = session is null
            ? await collection.UpdateManyAsync(filter, update, options, cancellationToken).ConfigureAwait(false)
            : await collection.UpdateManyAsync(session, filter, update, options, cancellationToken).ConfigureAwait(false);

        // Throw exception if the update did not match or did not effect
        // For multiple document updates, we just need to check acknowledgment
        if (!ur.IsAcknowledged)
        {
            throw new MongoUpdateConcurrencyException(acknowledged: ur.IsAcknowledged);
        }

        return ur.ModifiedCount;
    }

    /// <summary>
    /// Deletes multiple documents and checks the result.
    /// </summary>
    /// <typeparam name="TDocument"></typeparam>
    /// <param name="collection"></param>
    /// <param name="filter">The filter without the Etag condition.</param>
    /// <param name="options"><see cref="DeleteOptions"/>.</param>
    /// <param name="session">The optional <see cref="IClientSessionHandle"/> to use.</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <exception cref="MongoUpdateConcurrencyException">
    /// The deletion was not acknowledged.
    /// </exception>
    public static async Task<long> DeleteManyCheckedAsync<TDocument>(this IMongoCollection<TDocument> collection,
                                                                     FilterDefinition<TDocument> filter,
                                                                     DeleteOptions? options = null,
                                                                     IClientSessionHandle? session = null,
                                                                     CancellationToken cancellationToken = default)
    {
        // Do the actual delete
        var dr = session is null
            ? await collection.DeleteManyAsync(filter, options, cancellationToken).ConfigureAwait(false)
            : await collection.DeleteManyAsync(session, filter, options, cancellationToken).ConfigureAwait(false);

        // Throw exception if the delete did not match or did not effect
        // For multiple document updates, we just need to check acknowledgment
        if (!dr.IsAcknowledged)
        {
            throw new MongoUpdateConcurrencyException(acknowledged: dr.IsAcknowledged);
        }

        return dr.DeletedCount;
    }

    /// <summary>
    /// Performs multiple write operations and checks the result.
    /// </summary>
    /// <typeparam name="TDocument"></typeparam>
    /// <param name="collection"></param>
    /// <param name="requests">The requests.</param>
    /// <param name="options"><see cref="BulkWriteOptions"/>.</param>
    /// <param name="session">The optional <see cref="IClientSessionHandle"/> to use.</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public static async Task<BulkWriteResult<TDocument>> BulkWriteCheckedAsync<TDocument>(this IMongoCollection<TDocument> collection,
                                                                                          IEnumerable<WriteModel<TDocument>> requests,
                                                                                          BulkWriteOptions? options = null,
                                                                                          IClientSessionHandle? session = null,
                                                                                          CancellationToken cancellationToken = default)
    {
        // Do the actual bulk write
        var bwr = session is null
            ? await collection.BulkWriteAsync(requests, options, cancellationToken).ConfigureAwait(false)
            : await collection.BulkWriteAsync(session, requests, options, cancellationToken).ConfigureAwait(false);

        if (!bwr.IsAcknowledged)
        {
            throw new MongoUpdateConcurrencyException(acknowledged: bwr.IsAcknowledged);
        }

        return bwr;
    }
}
