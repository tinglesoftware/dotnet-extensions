using Microsoft.Extensions.FileProviders;
using System.Security.Cryptography;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Extension methods for <see cref="ApnsNotifierOptions"/>.
/// </summary>
public static class ApnsNotifierOptionsExtensions
{
    /// <summary>
    /// Configures the application to use a specified private to generate a token for the notifier.
    /// </summary>
    /// <param name="options">The Apple push notification options to configure.</param>
    /// <param name="privateKeyFileResolver">
    /// A delegate to a method to return the <see cref="IFileInfo"/> for the private key
    /// which is passed in the value of <see cref="ApnsNotifierOptions.KeyId"/>.
    /// </param>
    /// <returns></returns>
    public static ApnsNotifierOptions UsePrivateKey(this ApnsNotifierOptions options,
                                                    Func<string, IFileInfo> privateKeyFileResolver)
    {
        if (options is null) throw new ArgumentNullException(nameof(options));
        if (privateKeyFileResolver == null) throw new ArgumentNullException(nameof(privateKeyFileResolver));

        options.PrivateKeyBytes = async (keyId) =>
        {
            var fileInfo = privateKeyFileResolver(keyId);

            using var stream = fileInfo.CreateReadStream();
            using var reader = new StreamReader(stream);

            var privateKey = await reader.ReadToEndAsync().ConfigureAwait(false);
            return ParsePrivateKey(privateKey);
        };

        return options;
    }

    /// <summary>
    /// Configures the application to use a specified private to generate a token for the notifier.
    /// </summary>
    /// <param name="options">The Apple push notification options to configure.</param>
    /// <param name="privateKeyResolver">
    /// A delegate to a method to return the private key which is passed in the value of
    /// <see cref="ApnsNotifierOptions.KeyId"/>.
    /// </param>
    /// <returns></returns>
    public static ApnsNotifierOptions UsePrivateKey(this ApnsNotifierOptions options,
                                                    Func<string, string> privateKeyResolver)
    {
        if (options is null) throw new ArgumentNullException(nameof(options));
        if (privateKeyResolver == null) throw new ArgumentNullException(nameof(privateKeyResolver));

        options.PrivateKeyBytes = (keyId) =>
        {
            var privateKey = privateKeyResolver(keyId);
            return Task.FromResult(ParsePrivateKey(privateKey));
        };

        return options;
    }

    internal static byte[] ParsePrivateKey(string privateKey)
    {
        if (privateKey is null) throw new ArgumentNullException(nameof(privateKey));

        // exclude the PEM header if present
        if (PemEncoding.TryFind(privateKey, out var pem))
        {
            privateKey = privateKey[pem.Base64Data];
        }

        return Convert.FromBase64String(privateKey);
    }
}
