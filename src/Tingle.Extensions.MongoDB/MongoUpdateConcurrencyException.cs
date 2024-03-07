namespace MongoDB.Driver;

/// <summary>
/// Exception thrown by Etag versions of Replace and Update
/// when it was expected that Replace/Update for a document would result in a database update
/// but in fact no documents in the database were affected.
/// This usually indicates that the database has been concurrently updated such that a concurrency token that was expected to match
/// did not actually match.
/// </summary>
[Serializable]
public class MongoUpdateConcurrencyException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MongoUpdateConcurrencyException" /> class.
    /// </summary>
    public MongoUpdateConcurrencyException() { }

    /// <summary>
    /// Initializes a new instance of the <see cref="MongoUpdateConcurrencyException" /> class.
    /// </summary>
    /// <param name="message">The message.</param>
    public MongoUpdateConcurrencyException(string message) : base(message) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="MongoUpdateConcurrencyException" /> class.
    /// </summary>
    /// <param name="message">The message.</param>
    /// <param name="inner">The inner exception.</param>
    public MongoUpdateConcurrencyException(string message, Exception inner) : base(message, inner) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="MongoUpdateConcurrencyException" /> class.
    /// </summary>
    /// <param name="acknowledged">Indicates if the database operation was acknowledged by the server.</param>
    public MongoUpdateConcurrencyException(bool acknowledged) : this(acknowledged, null, null, null) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="MongoUpdateConcurrencyException" /> class.
    /// </summary>
    /// <param name="acknowledged">Indicates if the database operation was acknowledged by the server.</param>
    /// <param name="deleted">Indicates the number of documents that were deleted.</param>
    public MongoUpdateConcurrencyException(bool acknowledged, long? deleted = null)
        : this(acknowledged: acknowledged, matched: null, modified: null, deleted: deleted) { }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="acknowledged">Indicates if the database operation was acknowledged by the server.</param>
    /// <param name="matched">Indicates the number of documents that matched.</param>
    /// <param name="modified">Indicated the number of documents that were modified.</param>
    /// <param name="deleted">Indicates the number of documents that were deleted.</param>
    public MongoUpdateConcurrencyException(bool acknowledged, long? matched = null, long? modified = null, long? deleted = null)
        : this(MakeMessage(acknowledged, matched, modified, deleted))
    {
        Acknowledged = acknowledged;
        Matched = matched;
        Modified = modified;
        Deleted = deleted;
    }

    /// <summary>
    /// Indicates if the database operation was acknowledged by the server.
    /// </summary>
    /// <seealso cref="ReplaceOneResult.IsAcknowledged"/>
    /// <seealso cref="UpdateResult.IsAcknowledged"/>
    /// <seealso cref="DeleteResult.IsAcknowledged"/>
    public bool? Acknowledged { get; private set; }

    /// <summary>
    /// Indicates the number of documents that matched.
    /// </summary>
    /// <seealso cref="ReplaceOneResult.MatchedCount"/>
    /// <seealso cref="UpdateResult.MatchedCount"/>
    public long? Matched { get; private set; }

    /// <summary>
    /// Indicated the number of documents that were modified.
    /// </summary>
    /// <seealso cref="ReplaceOneResult.ModifiedCount"/>
    /// <seealso cref="UpdateResult.ModifiedCount"/>
    public long? Modified { get; private set; }

    /// <summary>
    /// Indicates the number of documents that were deleted.
    /// </summary>
    /// <seealso cref="DeleteResult.DeletedCount"/>
    public long? Deleted { get; private set; }

    private static string MakeMessage(bool acknowledged, long? matched, long? modified, long? deleted)
    {
        if (!acknowledged) return " Database update/delete operation was not acknowledged.";

        return deleted != null
            ? "Database delete operation was acknowledged but no document was deleted."
            : $"Database replace/update operation as acknowledged but no document was replaced/updated. Matched: {matched}, Modified: {modified}";
    }
}
