using Base62;
using System.Text;

namespace Tingle.Extensions.Primitives;

/// <summary>
/// Helper for generating keys.
/// For object identifiers consider <see cref="Ksuid"/> instead.
/// </summary>
public static class Keygen
{
    /// <summary>Output format for generated values.</summary>
    public enum OutputFormat
    {
        /// <summary>Base64</summary>
        Base64,

        /// <summary>Base62</summary>
        Base62,

        /// <summary>Hex</summary>
        Hex,
    }

    /// <summary>Creates a random key byte array.</summary>
    /// <param name="length">The length of the key.</param>
    /// <returns></returns>
    public static byte[] CreateRandomKey(int length)
    {
        var array = new byte[length];
        Random.Shared.NextBytes(array);
        return array;
    }

    /// <summary>Creates a URL safe unique key.</summary>
    /// <param name="length">The length of the key.</param>
    /// <param name="format">The output format.</param>
    /// <returns></returns>
    public static string Create(int length = 32, OutputFormat format = OutputFormat.Base62)
        => Create(CreateRandomKey(length), format);

    /// <summary>Creates a URL safe key.</summary>
    /// <param name="key">The key to be converted to bytes before encoding.</param>
    /// <param name="format">The output format.</param>
    /// <param name="encoding">The encoding to use when converting <paramref name="key"/>.</param>
    /// <returns></returns>
    public static string Create(string key, OutputFormat format = OutputFormat.Base62, Encoding? encoding = null)
    {
        if (string.IsNullOrWhiteSpace(key))
        {
            throw new ArgumentException($"'{nameof(key)}' cannot be null or whitespace.", nameof(key));
        }

        encoding ??= Encoding.UTF8;
        return Create(encoding.GetBytes(key), format);
    }

    /// <summary>Creates a URL safe key.</summary>
    /// <param name="key">The key.</param>
    /// <param name="format">The output format.</param>
    /// <returns></returns>
    public static string Create(byte[] key, OutputFormat format = OutputFormat.Base62)
    {
        return format switch
        {
            OutputFormat.Base64 => Convert.ToBase64String(key),
            OutputFormat.Base62 => key.ToBase62(),
            OutputFormat.Hex => BitConverter.ToString(key).Replace("-", ""),
            _ => throw new ArgumentException("Invalid format", nameof(format)),
        };
    }

    /// <summary>Decode a URL safe key create prior.</summary>
    /// <param name="key">A key created earlier by this <see cref="Keygen"/>.</param>
    /// <param name="format">The output format.</param>
    /// <returns></returns>
    public static byte[] Decode(string key, OutputFormat format = OutputFormat.Base62)
    {
        return format switch
        {
            OutputFormat.Base64 => Convert.FromBase64String(key),
            OutputFormat.Base62 => key.FromBase62(),
            OutputFormat.Hex => key.Chunk(2).Select(b => Convert.ToByte(new string(b), 16)).ToArray(),
            _ => throw new ArgumentException("Invalid format", nameof(format)),
        };
    }
}
