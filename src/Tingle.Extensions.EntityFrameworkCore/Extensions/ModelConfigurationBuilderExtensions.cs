using System.Text.Json;
using System.Text.Json.Nodes;
using Tingle.Extensions.EntityFrameworkCore.Converters;
using Tingle.Extensions.Primitives;
using Microsoft.EntityFrameworkCore.Metadata.Conventions.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using System.ComponentModel.DataAnnotations;
using System.Net;
using Tingle.Extensions.EntityFrameworkCore.Conventions;

namespace Microsoft.EntityFrameworkCore;

/// <summary>Extensions for <see cref="ModelConfigurationBuilder"/>.</summary>
public static class ModelConfigurationBuilderExtensions
{
    /// <summary>
    /// Add fields of type <see cref="Etag"/> to be converted to a <see cref="T:byte[]"/>.
    /// </summary>
    /// <param name="configurationBuilder">The <see cref="ModelConfigurationBuilder"/> to use.</param>
    public static void AddEtagToBytesConventions(this ModelConfigurationBuilder configurationBuilder)
    {
        ArgumentNullException.ThrowIfNull(configurationBuilder);

        configurationBuilder.Properties<Etag>().HaveConversion<EtagToBytesConverter, EtagComparer>();
    }

    /// <summary>
    /// Add fields of type <see cref="Etag"/> to be converted to a <see cref="uint"/>.
    /// </summary>
    /// <param name="configurationBuilder">The <see cref="ModelConfigurationBuilder"/> to use.</param>
    public static void AddEtagToInt32Conventions(this ModelConfigurationBuilder configurationBuilder)
    {
        ArgumentNullException.ThrowIfNull(configurationBuilder);

        configurationBuilder.Properties<Etag>().HaveConversion<EtagToInt32Converter, EtagComparer>();
    }

    /// <summary>
    /// Add fields of type <see cref="Etag"/> to be converted to a <see cref="int"/>.
    /// </summary>
    /// <param name="configurationBuilder">The <see cref="ModelConfigurationBuilder"/> to use.</param>
    public static void AddEtagToUInt32Conventions(this ModelConfigurationBuilder configurationBuilder)
    {
        ArgumentNullException.ThrowIfNull(configurationBuilder);

        configurationBuilder.Properties<Etag>().HaveConversion<EtagToUInt32Converter, EtagComparer>();
    }

    /// <summary>
    /// Add fields of type <see cref="Etag"/> to be converted to a <see cref="long"/>.
    /// </summary>
    /// <param name="configurationBuilder">The <see cref="ModelConfigurationBuilder"/> to use.</param>
    public static void AddEtagToInt64Conventions(this ModelConfigurationBuilder configurationBuilder)
    {
        ArgumentNullException.ThrowIfNull(configurationBuilder);

        configurationBuilder.Properties<Etag>().HaveConversion<EtagToInt64Converter, EtagComparer>();
    }

    /// <summary>
    /// Add fields of type <see cref="Etag"/> to be converted to a <see cref="ulong"/>.
    /// </summary>
    /// <param name="configurationBuilder">The <see cref="ModelConfigurationBuilder"/> to use.</param>
    public static void AddEtagToUInt64Conventions(this ModelConfigurationBuilder configurationBuilder)
    {
        ArgumentNullException.ThrowIfNull(configurationBuilder);

        configurationBuilder.Properties<Etag>().HaveConversion<EtagToUInt64Converter, EtagComparer>();
    }

    /// <summary>
    /// Add fields of type <see cref="SequenceNumber"/> to be converted using <see cref="SequenceNumberConverter"/>.
    /// </summary>
    /// <param name="configurationBuilder">The <see cref="ModelConfigurationBuilder"/> to use.</param>
    public static void AddSequenceNumberConventions(this ModelConfigurationBuilder configurationBuilder)
    {
        ArgumentNullException.ThrowIfNull(configurationBuilder);

        configurationBuilder.Properties<SequenceNumber>()
                            .HaveConversion<SequenceNumberConverter, SequenceNumberComparer>();
    }

    /// <summary>
    /// Add fields of type <see cref="ByteSize"/> to be converted using <see cref="ByteSizeConverter"/>.
    /// </summary>
    /// <param name="configurationBuilder">The <see cref="ModelConfigurationBuilder"/> to use.</param>
    public static void AddByteSizeConventions(this ModelConfigurationBuilder configurationBuilder)
    {
        ArgumentNullException.ThrowIfNull(configurationBuilder);

        configurationBuilder.Properties<ByteSize>().HaveConversion<ByteSizeConverter, ByteSizeComparer>();
    }

    /// <summary>
    /// Add fields of type <see cref="Duration"/> to be converted using <see cref="DurationConverter"/>.
    /// </summary>
    /// <param name="configurationBuilder">The <see cref="ModelConfigurationBuilder"/> to use.</param>
    public static void AddDurationConventions(this ModelConfigurationBuilder configurationBuilder)
    {
        ArgumentNullException.ThrowIfNull(configurationBuilder);

        configurationBuilder.Properties<Duration>().HaveConversion<DurationConverter, DurationComparer>();
    }

    /// <summary>
    /// Add fields of type <see cref="IPNetwork"/> to be converted using <see cref="IPNetworkConverter"/>.
    /// </summary>
    /// <param name="configurationBuilder">The <see cref="ModelConfigurationBuilder"/> to use.</param>
    public static void AddIPNetworkConventions(this ModelConfigurationBuilder configurationBuilder)
    {
        ArgumentNullException.ThrowIfNull(configurationBuilder);

        configurationBuilder.Properties<IPNetwork>().HaveConversion<IPNetworkConverter, IPNetworkComparer>();
    }

    /// <summary>
    /// Add fields of type <see cref="JsonElement"/> to be converted using <see cref="JsonElementConverter"/>.
    /// </summary>
    /// <param name="configurationBuilder">The <see cref="ModelConfigurationBuilder"/> to use.</param>
    public static void AddJsonElementConventions(this ModelConfigurationBuilder configurationBuilder)
    {
        ArgumentNullException.ThrowIfNull(configurationBuilder);

        configurationBuilder.Properties<JsonElement>().HaveConversion<JsonElementConverter, JsonElementComparer>();
    }

    /// <summary>
    /// Add fields of type <see cref="JsonObject"/> to be converted using <see cref="JsonObjectConverter"/>.
    /// </summary>
    /// <param name="configurationBuilder">The <see cref="ModelConfigurationBuilder"/> to use.</param>
    public static void AddJsonObjectConventions(this ModelConfigurationBuilder configurationBuilder)
    {
        ArgumentNullException.ThrowIfNull(configurationBuilder);

        configurationBuilder.Properties<JsonObject>().HaveConversion<JsonObjectConverter, JsonObjectComparer>();
    }

    /// <summary>
    /// Add fields of type <see cref="JsonNode"/> to be converted using <see cref="JsonNodeConverter"/>.
    /// </summary>
    /// <param name="configurationBuilder">The <see cref="ModelConfigurationBuilder"/> to use.</param>
    public static void AddJsonNodeConventions(this ModelConfigurationBuilder configurationBuilder)
    {
        ArgumentNullException.ThrowIfNull(configurationBuilder);

        configurationBuilder.Properties<JsonNode>().HaveConversion<JsonNodeConverter, JsonNodeComparer>();
    }

    /// <summary>
    /// Add fields of type<see cref="JsonElement"/>, <see cref="JsonObject"/>, or <see cref="JsonNode"/>
    /// to be converted using <see cref="JsonElementConverter"/>, <see cref="JsonObjectConverter"/>, or
    /// <see cref="JsonNodeConverter"/>.
    /// </summary>
    /// <param name="configurationBuilder">The <see cref="ModelConfigurationBuilder"/> to use.</param>
    public static void AddJsonConventions(this ModelConfigurationBuilder configurationBuilder)
    {
        ArgumentNullException.ThrowIfNull(configurationBuilder);

        configurationBuilder.AddJsonElementConventions();
        configurationBuilder.AddJsonObjectConventions();
        configurationBuilder.AddJsonNodeConventions();
    }

    /// <summary>
    /// Add convention for handling <see cref="LengthAttribute"/>.
    /// </summary>
    /// <param name="configurationBuilder">The <see cref="ModelConfigurationBuilder"/> to use.</param>
    public static void AddLengthAttributeConvention(this ModelConfigurationBuilder configurationBuilder)
    {
        ArgumentNullException.ThrowIfNull(configurationBuilder);

        configurationBuilder.Conventions.Add(provider
            => new LengthAttributeConvention(
                provider.GetRequiredService<ProviderConventionSetBuilderDependencies>()));
    }
}
