using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using System.Diagnostics.CodeAnalysis;
using Tingle.Extensions.EntityFrameworkCore;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>Extensions on <see cref="IServiceCollection"/>.</summary>
public static class IServiceCollectionExtensions
{
    /// <summary>
    /// Add an <see cref="IHostedService"/> to perform database setup depending on the configuration.
    /// </summary>
    /// <typeparam name="TContext">The type of context to be used in setup.</typeparam>
    /// <param name="services">The <see cref="IServiceCollection"/> to add to.</param>
    /// <returns></returns>
    /// <remarks>
    /// Migrations are done when the configuration value <c>EFCORE_PERFORM_MIGRATIONS</c> is set to <see langword="true"/>.
    /// Database creation is done when configuration value <c>EFCORE_CREATE_DATABASE</c> is set to <see langword="true"/>.
    /// </remarks>
    [RequiresDynamicCode(MessageStrings.MigrationsRequiresDynamicCodeMessage)]
    public static IServiceCollection AddDatabaseSetup<TContext>(this IServiceCollection services) where TContext : DbContext
        => services.AddHostedService<DatabaseSetup<TContext>>();
}
