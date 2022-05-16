using System.Security.Cryptography;
using System.Text;

namespace Tingle.Extensions.Http.Authentication;

/// <summary>
/// Implementation of <see cref="AuthenticationHandler"/> for SharedKey authentication scheme.
/// This is mainly used for Tingle APIs/Services but can be modified to be used with Microsoft APIs.
/// The implementation generates a token based on the HTTP method, path, time, content length and
/// content type, then hashing using the pre-shared key (PSK). The hashing algorithm is HMACSHA256 (<see cref="HMACSHA256"/>).
/// </summary>
public class SharedKeyAuthenticationHandler : AuthenticationHeaderHandler
{
    private readonly byte[] key;

    /// <summary>
    /// Creates an instance of the <see cref="SharedKeyAuthenticationHandler"/> class.
    /// </summary>
    /// <param name="key">The bytes representing the pre-shared key (PSK)</param>
    public SharedKeyAuthenticationHandler(byte[] key)
    {
        this.key = key ?? throw new ArgumentNullException(nameof(key));
    }

    /// <summary>
    /// Creates an instance of the <see cref="SharedKeyAuthenticationHandler"/> class.
    /// </summary>
    /// <param name="key">The base64 encoded pre-shared key (PSK).</param>
    public SharedKeyAuthenticationHandler(string key)
    {
        if (string.IsNullOrWhiteSpace(key))
        {
            throw new ArgumentException($"'{nameof(key)}' cannot be null or whitespace.", nameof(key));
        }

        this.key = Convert.FromBase64String(key);
    }

    /// <summary>
    /// Creates a new instance of the <see cref="SharedKeyAuthenticationHandler"/> class with a specific inner handler.
    /// </summary>
    /// <param name="innerHandler">The inner handler which is responsible for processing the HTTP response messages.</param>
    /// <param name="key">The bytes representing the pre-shared key (PSK)</param>
    public SharedKeyAuthenticationHandler(HttpMessageHandler innerHandler, byte[] key) : base(innerHandler)
    {
        this.key = key ?? throw new ArgumentNullException(nameof(key));
    }

    /// <summary>
    /// Creates an instance of the <see cref="SharedKeyAuthenticationHandler"/> class.
    /// </summary>
    /// <param name="innerHandler">The inner handler which is responsible for processing the HTTP response messages.</param>
    /// <param name="key">The base64 encoded pre-shared key (PSK).</param>
    public SharedKeyAuthenticationHandler(HttpMessageHandler innerHandler, string key) : base(innerHandler)
    {
        if (string.IsNullOrWhiteSpace(key))
        {
            throw new ArgumentException($"'{nameof(key)}' cannot be null or whitespace.", nameof(key));
        }

        this.key = Convert.FromBase64String(key);
    }

    /// <summary>
    /// The scheme to be used in the Authorization header.
    /// This is similar to <see cref="System.Net.Http.Headers.AuthenticationHeaderValue.Scheme"/>
    /// </summary>
    public override string Scheme { get; set; } = "SharedKey";

    /// <summary>
    /// The header name for the date header name
    /// </summary>
    public virtual string DateHeaderName { get; set; } = "x-ts-date";

    /// <inheritdoc/>
    protected override Task<string?> GetParameterAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        string? rfcDate = null;
        if (request.Headers.TryGetValues(DateHeaderName, out var dateValues))
        {
            rfcDate = dateValues.FirstOrDefault();
        }

        if (string.IsNullOrWhiteSpace(rfcDate))
        {
            rfcDate = DateTimeOffset.UtcNow.ToString("r");
            request.Headers.TryAddWithoutValidation(DateHeaderName, rfcDate);
        }

        string? contentType = null;
        int contentLength = 0;
        if (request.Content != null)
        {
            contentLength = (int)(request.Content.Headers.ContentLength ?? 0);
            contentType = request.Content.Headers.ContentType?.ToString();
        }

        var path = request.RequestUri!.AbsolutePath;
        var signature = Sign(request.Method.Method, contentLength, contentType, rfcDate, path);
        return Task.FromResult<string?>(signature);
    }

    /// <summary>
    /// Generates the SHA256 signature hash
    /// </summary>
    /// <returns></returns>
    protected virtual string Sign(string method, int contentLength, string? contentType, string date, string resource)
    {
        if (key == null)
        {
            throw new ArgumentNullException(nameof(key), "The PSK bytes must not be null. Signing/Hashing cannot proceed");
        }

        var stringToHash = string.Join("\n", method, contentLength, contentType, $"{DateHeaderName}:{date}", resource);
        var bytesToHash = Encoding.ASCII.GetBytes(stringToHash);

        using var sha256 = new HMACSHA256(key);
        var calculatedHash = sha256.ComputeHash(bytesToHash);
        return Convert.ToBase64String(calculatedHash);
    }
}
