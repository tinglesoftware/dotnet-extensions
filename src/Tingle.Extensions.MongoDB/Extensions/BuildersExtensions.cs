using MongoDB.Bson.Serialization;
using System.Linq.Expressions;

namespace MongoDB.Driver;

/// <summary>
/// Extensions on <see cref="Builders{TDocument}"/> for indexing properties in an array.
/// Copied from https://jira.mongodb.org/browse/CSHARP-1309 before modification.
/// </summary>
public static class BuildersExtensions
{
    /// <summary>
    /// Creates an ascending index key definition.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="TChild"></typeparam>
    /// <param name="builder"></param>
    /// <param name="collectionExpression">The collection.</param>
    /// <param name="memberExpression">The member.</param>
    /// <returns>An ascending index key definition.</returns>
    public static IndexKeysDefinition<T> Ascending<T, TChild>(this IndexKeysDefinitionBuilder<T> builder,
                                                              Expression<Func<T, IEnumerable<TChild>>> collectionExpression,
                                                              Expression<Func<TChild, object?>> memberExpression)
    {
        var definition = CreateFieldDefinition(collectionExpression, memberExpression);
        return builder.Ascending(definition);
    }

    /// <summary>
    /// Creates an ascending index key definition.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="TChild"></typeparam>
    /// <param name="builder"></param>
    /// <param name="collectionExpression">The collection.</param>
    /// <param name="memberExpression">The member.</param>
    /// <returns>An ascending index key definition.</returns>
    public static IndexKeysDefinition<T> Ascending<T, TChild>(this IndexKeysDefinition<T> builder,
                                                              Expression<Func<T, IEnumerable<TChild>>> collectionExpression,
                                                              Expression<Func<TChild, object?>> memberExpression)
    {
        var definition = CreateFieldDefinition(collectionExpression, memberExpression);
        return builder.Ascending(definition);
    }

    /// <summary>
    /// Creates a descending index key definition.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="TChild"></typeparam>
    /// <param name="builder"></param>
    /// <param name="collectionExpression">The collection.</param>
    /// <param name="memberExpression">The member.</param>
    /// <returns>A descending index key definition.</returns>
    public static IndexKeysDefinition<T> Descending<T, TChild>(this IndexKeysDefinitionBuilder<T> builder,
                                                               Expression<Func<T, IEnumerable<TChild>>> collectionExpression,
                                                               Expression<Func<TChild, object?>> memberExpression)
    {
        var definition = CreateFieldDefinition(collectionExpression, memberExpression);
        return builder.Descending(definition);
    }

    /// <summary>
    /// Creates a descending index key definition.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="TChild"></typeparam>
    /// <param name="builder"></param>
    /// <param name="collectionExpression">The collection.</param>
    /// <param name="memberExpression">The member.</param>
    /// <returns>A descending index key definition.</returns>
    public static IndexKeysDefinition<T> Descending<T, TChild>(this IndexKeysDefinition<T> builder,
                                                               Expression<Func<T, IEnumerable<TChild>>> collectionExpression,
                                                               Expression<Func<TChild, object?>> memberExpression)
    {
        var definition = CreateFieldDefinition(collectionExpression, memberExpression);
        return builder.Descending(definition);
    }

    private static StringFieldDefinition<T> CreateFieldDefinition<T, TChild>(Expression<Func<T, IEnumerable<TChild>>> collectionExpression,
                                                                             Expression<Func<TChild, object?>> memberExpression)
    {
        var collection = new ExpressionFieldDefinition<T>(collectionExpression);
        var child = new ExpressionFieldDefinition<TChild>(memberExpression);
        var collectionDefinition = collection.Render(new RenderArgs<T>
        {
            DocumentSerializer = BsonSerializer.LookupSerializer<T>(),
            SerializerRegistry = BsonSerializer.SerializerRegistry,
        });
        var childDefinition = child.Render(new RenderArgs<TChild>
        {
            DocumentSerializer = BsonSerializer.LookupSerializer<TChild>(),
            SerializerRegistry = BsonSerializer.SerializerRegistry,
        });

        var fieldDefinition = new StringFieldDefinition<T>(collectionDefinition.FieldName + "." + childDefinition.FieldName);
        return fieldDefinition;
    }
}
