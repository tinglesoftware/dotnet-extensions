using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using MongoDB.Driver;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Tingle.Extensions.MongoDB;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>Extension methods for <see cref="IServiceCollection"/>.</summary>
public static class IServiceCollectionExtensions
{
    /// <summary>
    /// Add an <see cref="IHostedService"/> to perform database setup depending on the configuration.
    /// </summary>
    /// <typeparam name="TContext">The type of context to be used in setup.</typeparam>
    /// <param name="services">The <see cref="IServiceCollection"/> to add to.</param>
    /// <returns></returns>
    /// <remarks>
    /// Database creation is done when configuration value <c>MONGO_CREATE_DATABASE</c> is set to <see langword="true"/>.
    /// </remarks>
    public static IServiceCollection AddMongoDatabaseSetup<TContext>(this IServiceCollection services)
        where TContext : MongoDbContext
    {
        return services.AddHostedService<DatabaseSetup<TContext>>();
    }

    public static IServiceCollection AddMongoDbContext<[DynamicallyAccessedMembers(MongoDbContext.DynamicallyAccessedMemberTypes)] TContext>(
        this IServiceCollection services,
        IConfiguration configuration,
        Action<MongoDbContextOptionsBuilder>? optionsAction = null,
        ServiceLifetime contextLifetime = ServiceLifetime.Singleton,
        ServiceLifetime optionsLifetime = ServiceLifetime.Singleton)
        where TContext : MongoDbContext
    {
        var connectionString = configuration.GetConnectionString("Mongo");
        return connectionString is not null
            ? AddMongoDbContext<TContext>(services, connectionString, optionsAction, contextLifetime, optionsLifetime)
            : AddMongoDbContext<TContext>(services, optionsAction, contextLifetime, optionsLifetime);
    }

    public static IServiceCollection AddMongoDbContext<[DynamicallyAccessedMembers(MongoDbContext.DynamicallyAccessedMemberTypes)] TContext>(
        this IServiceCollection services,
        string connectionString,
        Action<MongoDbContextOptionsBuilder>? optionsAction = null,
        ServiceLifetime contextLifetime = ServiceLifetime.Singleton,
        ServiceLifetime optionsLifetime = ServiceLifetime.Singleton)
        where TContext : MongoDbContext
    {
        return AddMongoDbContext<TContext>(services, options =>
        {
            options.UseMongoConnectionString(connectionString);
            optionsAction?.Invoke(options);
        }, contextLifetime, optionsLifetime);
    }

    public static IServiceCollection AddMongoDbContext<[DynamicallyAccessedMembers(MongoDbContext.DynamicallyAccessedMemberTypes)] TContext>(
        this IServiceCollection services,
        Action<MongoDbContextOptionsBuilder>? optionsAction = null,
        ServiceLifetime contextLifetime = ServiceLifetime.Singleton,
        ServiceLifetime optionsLifetime = ServiceLifetime.Singleton)
        where TContext : MongoDbContext
        => AddMongoDbContext<TContext, TContext>(services, optionsAction, contextLifetime, optionsLifetime);

    public static IServiceCollection AddMongoDbContext<TContextService, [DynamicallyAccessedMembers(MongoDbContext.DynamicallyAccessedMemberTypes)] TContextImplementation>(
        this IServiceCollection services,
        Action<MongoDbContextOptionsBuilder>? optionsAction = null,
        ServiceLifetime contextLifetime = ServiceLifetime.Singleton,
        ServiceLifetime optionsLifetime = ServiceLifetime.Singleton)
        where TContextImplementation : MongoDbContext, TContextService
        => AddMongoDbContext<TContextService, TContextImplementation>(
            services,
            optionsAction == null
                ? null
                : (p, b) => optionsAction(b), contextLifetime, optionsLifetime);

    public static IServiceCollection AddMongoDbContext<[DynamicallyAccessedMembers(MongoDbContext.DynamicallyAccessedMemberTypes)] TContext>(
        this IServiceCollection services,
        ServiceLifetime contextLifetime,
        ServiceLifetime optionsLifetime = ServiceLifetime.Singleton)
        where TContext : MongoDbContext
        => AddMongoDbContext<TContext, TContext>(services, contextLifetime, optionsLifetime);

    public static IServiceCollection AddMongoDbContext<TContextService, [DynamicallyAccessedMembers(MongoDbContext.DynamicallyAccessedMemberTypes)] TContextImplementation>(
        this IServiceCollection services,
        ServiceLifetime contextLifetime,
        ServiceLifetime optionsLifetime = ServiceLifetime.Singleton)
        where TContextImplementation : MongoDbContext, TContextService
        where TContextService : class
        => AddMongoDbContext<TContextService, TContextImplementation>(
            services,
            (Action<IServiceProvider, MongoDbContextOptionsBuilder>?)null,
            contextLifetime,
            optionsLifetime);

    public static IServiceCollection AddMongoDbContext<[DynamicallyAccessedMembers(MongoDbContext.DynamicallyAccessedMemberTypes)] TContext>(
        this IServiceCollection services,
        Action<IServiceProvider, MongoDbContextOptionsBuilder>? optionsAction,
        ServiceLifetime contextLifetime = ServiceLifetime.Singleton,
        ServiceLifetime optionsLifetime = ServiceLifetime.Singleton)
        where TContext : MongoDbContext
        => AddMongoDbContext<TContext, TContext>(services, optionsAction, contextLifetime, optionsLifetime);

    public static IServiceCollection AddMongoDbContext<TContextService, [DynamicallyAccessedMembers(MongoDbContext.DynamicallyAccessedMemberTypes)] TContextImplementation>(
        this IServiceCollection services,
        Action<IServiceProvider, MongoDbContextOptionsBuilder>? optionsAction,
        ServiceLifetime contextLifetime = ServiceLifetime.Singleton,
        ServiceLifetime optionsLifetime = ServiceLifetime.Singleton)
        where TContextImplementation : MongoDbContext, TContextService
    {
        if (contextLifetime == ServiceLifetime.Singleton)
        {
            optionsLifetime = ServiceLifetime.Singleton;
        }

        if (optionsAction != null)
        {
            CheckContextConstructors<TContextImplementation>();
        }

        AddCoreServices<TContextImplementation>(services, optionsAction, optionsLifetime);

        services.TryAdd(new ServiceDescriptor(typeof(TContextService), typeof(TContextImplementation), contextLifetime));

        if (typeof(TContextService) != typeof(TContextImplementation))
        {
            services.TryAdd(
                new ServiceDescriptor(
                    typeof(TContextImplementation),
                    p => (TContextImplementation)p.GetService<TContextService>()!,
                    contextLifetime));
        }

        return services;
    }

    private static void AddCoreServices<TContextImplementation>(
        IServiceCollection services,
        Action<IServiceProvider, MongoDbContextOptionsBuilder>? optionsAction,
        ServiceLifetime optionsLifetime)
        where TContextImplementation : MongoDbContext
    {
        services.TryAdd(
            new ServiceDescriptor(
                typeof(MongoDbContextOptions<TContextImplementation>),
                p => CreateDbContextOptions<TContextImplementation>(p, optionsAction),
                optionsLifetime));

        services.Add(
            new ServiceDescriptor(
                typeof(MongoDbContextOptions),
                p => p.GetRequiredService<MongoDbContextOptions<TContextImplementation>>(),
                optionsLifetime));
    }

    private static MongoDbContextOptions<TContext> CreateDbContextOptions<TContext>(
        IServiceProvider applicationServiceProvider,
        Action<IServiceProvider, MongoDbContextOptionsBuilder>? optionsAction)
        where TContext : MongoDbContext
    {
        var builder = new MongoDbContextOptionsBuilder<TContext>(
            new MongoDbContextOptions<TContext>());

        builder.UseApplicationServiceProvider(applicationServiceProvider);

        optionsAction?.Invoke(applicationServiceProvider, builder);

        return builder.Options;
    }

    private static void CheckContextConstructors<[DynamicallyAccessedMembers(MongoDbContext.DynamicallyAccessedMemberTypes)] TContext>()
        where TContext : MongoDbContext
    {
        var declaredConstructors = typeof(TContext).GetTypeInfo().DeclaredConstructors.ToList();
        if (declaredConstructors.Count == 1
            && declaredConstructors[0].GetParameters().Length == 0)
        {
            throw new ArgumentException($"{typeof(TContext).Name} does not have the necessary constructor");
        }
    }
}
