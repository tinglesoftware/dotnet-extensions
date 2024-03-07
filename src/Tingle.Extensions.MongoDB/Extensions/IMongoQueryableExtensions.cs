using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace System.Linq;

/// <summary>
/// Extension methods on <see cref="IMongoQueryable{T}"/>
/// </summary>
public static class IMongoQueryableExtensions
{
    /// <summary>
    /// Returns a list containing all the documents returned from Mongo.
    /// This method removes the need to convert instances of <see cref="IQueryable{T}"/> to <see cref="IMongoQueryable{T}"/>.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="query"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public static Task<List<T>> ToListInMongoAsync<T>(this IQueryable<T> query, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(query);

        // cast to IMongoQueryable<T>, if not possible, a type cast exception is thrown
        var mq = (IMongoQueryable<T>)query;

        // execute the async operation
        return mq.ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Returns the only element of a sequence from Mongo, and throws an exception if there is not exactly one element in the sequence.
    /// This method removes the need to convert instances of <see cref="IQueryable{T}"/> to <see cref="IMongoQueryable{T}"/>.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="query"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public static Task<T> SingleInMongoAsync<T>(this IQueryable<T> query, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(query);

        // cast to IMongoQueryable<T>, if not possible, a type cast exception is thrown
        var mq = (IMongoQueryable<T>)query;

        // execute the async operation
        return mq.SingleAsync(cancellationToken);
    }

    /// <summary>
    /// Returns the only element of a sequence from Mongo, or a default value if the sequence is empty.
    /// This method throws an exception if there is more than one element in the sequence.
    /// This method removes the need to convert instances of <see cref="IQueryable{T}"/> to <see cref="IMongoQueryable{T}"/>.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="query"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public static Task<T> SingleOrDefaultInMongoAsync<T>(this IQueryable<T> query, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(query);

        // cast to IMongoQueryable<T>, if not possible, a type cast exception is thrown
        var mq = (IMongoQueryable<T>)query;

        // execute the async operation
        return mq.SingleOrDefaultAsync(cancellationToken);
    }
}
