using MongoDB.Driver;
using MongoDB.Driver.Linq;
using System.Linq.Expressions;

namespace System.Linq;

/// <summary>
/// Extension methods on <see cref="IMongoQueryable{T}"/>
/// </summary>
public static class IMongoQueryableExtensions
{
    /// <summary>
    /// Returns the number of elements in a sequence.
    /// This method removes the need to convert instances of <see cref="IQueryable{T}"/> to <see cref="IMongoQueryable{T}"/>.
    /// </summary>
    /// <typeparam name="T">The type of the elements of source.</typeparam>
    /// <param name="source">The <see cref="IMongoQueryable{T}"/> that contains the elements to be counted.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The number of elements in the input sequence.</returns>
    public static Task<long> LongCountInMongoAsync<T>(this IQueryable<T> source, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(source);

        // cast to IMongoQueryable<T>, if not possible, a type cast exception is thrown
        var mq = (IMongoQueryable<T>)source;

        // execute the async operation
        return mq.LongCountAsync(cancellationToken);
    }

    /// <summary>
    /// Returns the number of elements in the specified sequence that satisfies a condition.
    /// This method removes the need to convert instances of <see cref="IQueryable{T}"/> to <see cref="IMongoQueryable{T}"/>.
    /// </summary>
    /// <typeparam name="T">The type of the elements of source.</typeparam>
    /// <param name="source">The <see cref="IMongoQueryable{T}"/> that contains the elements to be counted.</param>
    /// <param name="predicate">A function to test an element for a condition.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The number of elements in the sequence that satisfies the condition in the predicate function.</returns>
    public static Task<long> LongCountInMongoAsync<T>(this IQueryable<T> source, Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(source);

        // cast to IMongoQueryable<T>, if not possible, a type cast exception is thrown
        var mq = (IMongoQueryable<T>)source;

        // execute the async operation
        return mq.LongCountAsync(predicate, cancellationToken);
    }

    /// <summary>
    /// Returns a list containing all the documents returned from Mongo.
    /// This method removes the need to convert instances of <see cref="IQueryable{T}"/> to <see cref="IMongoQueryable{T}"/>.
    /// </summary>
    /// <typeparam name="T">The type of the document.</typeparam>
    /// <param name="source">An <see cref="IMongoQueryable{T}"/>.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A Task whose value is the list of documents.</returns>
    public static Task<List<T>> ToListInMongoAsync<T>(this IQueryable<T> source, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(source);

        // cast to IMongoQueryable<T>, if not possible, a type cast exception is thrown
        var mq = (IMongoQueryable<T>)source;

        // execute the async operation
        return mq.ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Returns the first element of a sequence.
    /// This method removes the need to convert instances of <see cref="IQueryable{T}"/> to <see cref="IMongoQueryable{T}"/>.
    /// </summary>
    /// <typeparam name="T">The type of the elements of source.</typeparam>
    /// <param name="source">An <see cref="IMongoQueryable{T}"/> to return the first element of.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The first element in source.</returns>
    public static Task<T> FirstInMongoAsync<T>(this IQueryable<T> source, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(source);

        // cast to IMongoQueryable<T>, if not possible, a type cast exception is thrown
        var mq = (IMongoQueryable<T>)source;

        // execute the async operation
        return mq.FirstAsync(cancellationToken);
    }

    /// <summary>
    /// Returns the first element of a sequence that satisfies a specified condition.
    /// This method removes the need to convert instances of <see cref="IQueryable{T}"/> to <see cref="IMongoQueryable{T}"/>.
    /// </summary>
    /// <typeparam name="T">The type of the elements of source.</typeparam>
    /// <param name="source">An <see cref="IMongoQueryable{T}"/> to return an element from.</param>
    /// <param name="predicate">A function to test an element for a condition.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The first element in source that passes the test in predicate.</returns>
    public static Task<T> FirstInMongoAsync<T>(this IQueryable<T> source, Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(source);

        // cast to IMongoQueryable<T>, if not possible, a type cast exception is thrown
        var mq = (IMongoQueryable<T>)source;

        // execute the async operation
        return mq.FirstAsync(predicate, cancellationToken);
    }

    /// <summary>
    /// Returns the first element of a sequence, or a default value if the sequence contains no elements.
    /// This method throws an exception if there is more than one element in the sequence.
    /// This method removes the need to convert instances of <see cref="IQueryable{T}"/> to <see cref="IMongoQueryable{T}"/>.
    /// </summary>
    /// <typeparam name="T">The type of the elements of source.</typeparam>
    /// <param name="source">An <see cref="IMongoQueryable{T}"/> to return the first element of.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The first element of the input sequence, or default(T) if the sequence contains no elements.</returns>
    public static Task<T> FirstOrDefaultInMongoAsync<T>(this IQueryable<T> source, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(source);

        // cast to IMongoQueryable<T>, if not possible, a type cast exception is thrown
        var mq = (IMongoQueryable<T>)source;

        // execute the async operation
        return mq.FirstOrDefaultAsync(cancellationToken);
    }

    /// <summary>
    /// Returns the first element of a sequence that satisfies a specified condition or a default value if no such element is found.
    /// This method throws an exception if there is more than one element in the sequence.
    /// This method removes the need to convert instances of <see cref="IQueryable{T}"/> to <see cref="IMongoQueryable{T}"/>.
    /// </summary>
    /// <typeparam name="T">The type of the elements of source.</typeparam>
    /// <param name="source">An <see cref="IMongoQueryable{T}"/> to return a first element from.</param>
    /// <param name="predicate">A function to test an element for a condition.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The first element of the input sequence that satisfies the condition in predicate, or default(T) if no such element is found.</returns>
    public static Task<T> FirstOrDefaultInMongoAsync<T>(this IQueryable<T> source, Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(source);

        // cast to IMongoQueryable<T>, if not possible, a type cast exception is thrown
        var mq = (IMongoQueryable<T>)source;

        // execute the async operation
        return mq.FirstOrDefaultAsync(predicate, cancellationToken);
    }

    /// <summary>
    /// Returns the only element of a sequence from Mongo, and throws an exception if there is not exactly one element in the sequence.
    /// This method removes the need to convert instances of <see cref="IQueryable{T}"/> to <see cref="IMongoQueryable{T}"/>.
    /// </summary>
    /// <typeparam name="T">The type of the elements of source.</typeparam>
    /// <param name="source">An <see cref="IMongoQueryable{T}"/> to return the single element of.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The single element of the input sequence.</returns>
    public static Task<T> SingleInMongoAsync<T>(this IQueryable<T> source, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(source);

        // cast to IMongoQueryable<T>, if not possible, a type cast exception is thrown
        var mq = (IMongoQueryable<T>)source;

        // execute the async operation
        return mq.SingleAsync(cancellationToken);
    }

    /// <summary>
    /// Returns the only element of a sequence from Mongo, and throws an exception if there is not exactly one element in the sequence.
    /// This method removes the need to convert instances of <see cref="IQueryable{T}"/> to <see cref="IMongoQueryable{T}"/>.
    /// </summary>
    /// <typeparam name="T">The type of the elements of source.</typeparam>
    /// <param name="source">An <see cref="IMongoQueryable{T}"/> to return a single element from.</param>
    /// <param name="predicate">A function to test an element for a condition.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The single element of the input sequence that satisfies the condition in predicate.</returns>
    public static Task<T> SingleInMongoAsync<T>(this IQueryable<T> source, Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(source);

        // cast to IMongoQueryable<T>, if not possible, a type cast exception is thrown
        var mq = (IMongoQueryable<T>)source;

        // execute the async operation
        return mq.SingleAsync(predicate, cancellationToken);
    }

    /// <summary>
    /// Returns the only element of a sequence from Mongo, or a default value if the sequence is empty.
    /// This method throws an exception if there is more than one element in the sequence.
    /// This method removes the need to convert instances of <see cref="IQueryable{T}"/> to <see cref="IMongoQueryable{T}"/>.
    /// </summary>
    /// <typeparam name="T">The type of the elements of source.</typeparam>
    /// <param name="source">An <see cref="IMongoQueryable{T}"/> to return the single element of.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The single element of the input sequence, or default(T) if the sequence contains no elements.</returns>
    public static Task<T> SingleOrDefaultInMongoAsync<T>(this IQueryable<T> source, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(source);

        // cast to IMongoQueryable<T>, if not possible, a type cast exception is thrown
        var mq = (IMongoQueryable<T>)source;

        // execute the async operation
        return mq.SingleOrDefaultAsync(cancellationToken);
    }

    /// <summary>
    /// Returns the only element of a sequence from Mongo, or a default value if the sequence is empty.
    /// This method throws an exception if there is more than one element in the sequence.
    /// This method removes the need to convert instances of <see cref="IQueryable{T}"/> to <see cref="IMongoQueryable{T}"/>.
    /// </summary>
    /// <typeparam name="T">The type of the elements of source.</typeparam>
    /// <param name="source">An <see cref="IMongoQueryable{T}"/> to return a single element from.</param>
    /// <param name="predicate">A function to test an element for a condition.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The single element of the input sequence that satisfies the condition in predicate, or default(T) if no such element is found.</returns>
    public static Task<T> SingleOrDefaultInMongoAsync<T>(this IQueryable<T> source, Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(source);

        // cast to IMongoQueryable<T>, if not possible, a type cast exception is thrown
        var mq = (IMongoQueryable<T>)source;

        // execute the async operation
        return mq.SingleOrDefaultAsync(predicate, cancellationToken);
    }
}
