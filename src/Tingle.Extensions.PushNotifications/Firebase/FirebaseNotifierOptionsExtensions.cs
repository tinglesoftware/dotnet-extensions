using Microsoft.Extensions.FileProviders;
using System.Text.Json;
using System.Text.Json.Serialization;
using Tingle.Extensions.PushNotifications;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Extension methods for <see cref="FirebaseNotifierOptions"/>.
/// </summary>
public static class FirebaseNotifierOptionsExtensions
{
    /// <summary>
    /// Configures the application to use a specified private key to generate a token for the notifier.
    /// </summary>
    /// <param name="options">The Firebase notification options to configure.</param>
    /// <param name="path">The path for the Service Account JSON file.</param>
    /// <returns></returns>
    public static FirebaseNotifierOptions UseConfigurationFromFile(this FirebaseNotifierOptions options, string path)
        => options.UseConfigurationFromFile(new FileInfo(path));

    /// <summary>
    /// Configures the application to use a specified private key to generate a token for the notifier.
    /// </summary>
    /// <param name="options">The Firebase notification options to configure.</param>
    /// <param name="fileInfo">The <see cref="FileInfo"/> pointing to the Service Account JSON file.</param>
    /// <returns></returns>
    public static FirebaseNotifierOptions UseConfigurationFromFile(this FirebaseNotifierOptions options, FileInfo fileInfo)
    {
        if (options is null) throw new ArgumentNullException(nameof(options));
        if (fileInfo == null) throw new ArgumentNullException(nameof(fileInfo));

        using var stream = fileInfo.OpenRead();
        return options.UseConfigurationFromStream(stream);
    }

    /// <summary>
    /// Configures the application to use a specified private key to generate a token for the notifier.
    /// </summary>
    /// <param name="options">The Firebase notification options to configure.</param>
    /// <param name="fileInfo">The <see cref="IFileInfo"/> pointing to the Service Account JSON file.</param>
    /// <returns></returns>
    public static FirebaseNotifierOptions UseConfigurationFromFile(this FirebaseNotifierOptions options, IFileInfo fileInfo)
    {
        if (options is null) throw new ArgumentNullException(nameof(options));
        if (fileInfo == null) throw new ArgumentNullException(nameof(fileInfo));

        using var stream = fileInfo.CreateReadStream();
        return options.UseConfigurationFromStream(stream);
    }

    /// <summary>
    /// Configures the application to use a specified private key to generate a token for the notifier.
    /// </summary>
    /// <param name="options">The Firebase notification options to configure.</param>
    /// <param name="stream">The <see cref="Stream"/> containing the Service Account JSON configuration.</param>
    /// <returns></returns>
    public static FirebaseNotifierOptions UseConfigurationFromStream(this FirebaseNotifierOptions options, Stream stream)
    {
        // parse the stream into settings using JSON
        var settings = JsonSerializer.Deserialize(stream, PushNotificationsJsonSerializerContext.Default.FirebaseSettings)
            ?? throw new InvalidOperationException("The provided stream does not contain a valid JSON object");

        // Ensure the configuration file is for a Service Account
        if (settings.Type != "service_account")
        {
            throw new InvalidOperationException("Only Service Accounts are supported.");
        }

        // set values in the options
        options.ProjectId = settings.ProjectId;
        options.ClientEmail = settings.ClientEmail;
        options.TokenUri = settings.TokenUri;
        options.PrivateKey = settings.PrivateKey;

        return options;
    }
}

internal sealed record FirebaseSettings
{
    [JsonPropertyName("type")]
    public string? Type { get; set; }

    [JsonPropertyName("project_id")]
    public string? ProjectId { get; set; }

    [JsonPropertyName("client_email")]
    public string? ClientEmail { get; set; }

    [JsonPropertyName("token_uri")]
    public string? TokenUri { get; set; }

    [JsonPropertyName("private_key")]
    public string? PrivateKey { get; set; }
}
