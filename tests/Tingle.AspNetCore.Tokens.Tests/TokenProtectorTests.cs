using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using System.Security.Cryptography;
using Tingle.AspNetCore.Tokens.Protection;

namespace Tingle.AspNetCore.Tokens.Tests;

public class TokenProtectorTests
{
    private readonly IDataProtectionProvider protectionProvider = new EphemeralDataProtectionProvider(NullLoggerFactory.Instance);

    [Fact]
    public void Protection_Works()
    {
        var rnd = new Random();

        AssertValueEncryptDecrypt(Guid.NewGuid());                                      // Guid
        AssertValueEncryptDecrypt(rnd.Next(int.MinValue, int.MaxValue));                // int
        AssertValueEncryptDecrypt(Convert.ToInt64(rnd.NextDouble() * int.MinValue));    // long
        AssertValueEncryptDecrypt(rnd.NextDouble() * int.MinValue);                     // double
        AssertValueEncryptDecrypt(DateTimeOffset.UtcNow);                               // DateTimeOffset
        AssertValueEncryptDecrypt(DateTime.UtcNow);                                     // DateTime
        AssertValueEncryptDecrypt(Guid.NewGuid().ToString());                           // string
    }

    [Fact]
    public void TimeLimited_Protection_Works()
    {
        var rnd = new Random();

        // test with absolute expiration
        var expiration = DateTimeOffset.UtcNow.AddSeconds(1);
        AssertTimeLimitedValueEncryptDecrypt(Guid.NewGuid(), expiration);                                   // Guid
        AssertTimeLimitedValueEncryptDecrypt(rnd.Next(int.MinValue, int.MaxValue), expiration);             // int
        AssertTimeLimitedValueEncryptDecrypt(Convert.ToInt64(rnd.NextDouble() * int.MinValue), expiration); // long
        AssertTimeLimitedValueEncryptDecrypt(rnd.NextDouble() * int.MinValue, expiration);                  // double
        AssertTimeLimitedValueEncryptDecrypt(DateTimeOffset.UtcNow, expiration);                            // DateTimeOffset
        AssertTimeLimitedValueEncryptDecrypt(DateTime.UtcNow, expiration);                                  // DateTime
        AssertTimeLimitedValueEncryptDecrypt(Guid.NewGuid().ToString(), expiration);                        // string

        // not test with lifespan
        var lifespan = TimeSpan.FromSeconds(60);
        AssertTimeLimitedValueEncryptDecrypt(Guid.NewGuid(), lifespan);                                     // Guid
        AssertTimeLimitedValueEncryptDecrypt(rnd.Next(int.MinValue, int.MaxValue), lifespan);               // int
        AssertTimeLimitedValueEncryptDecrypt(Convert.ToInt64(rnd.NextDouble() * int.MinValue), lifespan);   // long
        AssertTimeLimitedValueEncryptDecrypt(rnd.NextDouble() * int.MinValue, lifespan);                    // double
        AssertTimeLimitedValueEncryptDecrypt(DateTimeOffset.UtcNow, lifespan);                              // DateTimeOffset
        AssertTimeLimitedValueEncryptDecrypt(DateTime.UtcNow, lifespan);                                    // DateTime
        AssertTimeLimitedValueEncryptDecrypt(Guid.NewGuid().ToString(), lifespan);                          // string
    }

    [Fact]
    public void TimeLimited_Protection_Works_On_DataClass()
    {
        var d = TestDataClass.CreateRandom();

        var expiration = DateTimeOffset.UtcNow.AddSeconds(1);
        AssertTimeLimitedValueEncryptDecrypt(d, expiration);

        var lifespan = TimeSpan.FromSeconds(60);
        AssertTimeLimitedValueEncryptDecrypt( d, lifespan);
    }

    [Fact]
    public async Task TimeLimited_Protection_Fails_Expired()
    {
        var d = TestDataClass.CreateRandom();
        var expiration = DateTimeOffset.UtcNow.AddSeconds(1);

        var options = new OptionsSnapshot<JsonOptions>(new());
        var prot = new TokenProtector<TestDataClass>(protectionProvider, options);
        var enc = prot.Protect(d, expiration);

        // delay the usage
        await Task.Delay(TimeSpan.FromSeconds(2));

        var ex = Assert.ThrowsAny<CryptographicException>(() => prot.UnProtect(enc, out DateTimeOffset actualExpiration));
        Assert.StartsWith("The payload expired", ex.Message);

        var lifespan = TimeSpan.FromSeconds(1);

        prot = new TokenProtector<TestDataClass>(protectionProvider, options);
        enc = prot.Protect(d, lifespan);

        // delay the usage
        await Task.Delay(TimeSpan.FromSeconds(2));

        ex = Assert.ThrowsAny<CryptographicException>(() => prot.UnProtect(enc, out DateTimeOffset actualExpiration));
        Assert.StartsWith("The payload expired", ex.Message);
    }

    private void AssertValueEncryptDecrypt<T>(T datum, bool unwrapDateTime = false)
    {
        var options = new OptionsSnapshot<JsonOptions>(new());
        ITokenProtector<T> prot = new TokenProtector<T>(protectionProvider, options);
        var actual = prot.UnProtect(prot.Protect(datum));
        if (unwrapDateTime && typeof(T) == typeof(DateTime))
        {
            Assert.Equal((DateTime)(object)datum!, (DateTime)(object)actual!, TimeSpan.FromSeconds(1));
        }
        else if (unwrapDateTime && typeof(T) == typeof(DateTimeOffset))
        {
            Assert.Equal(((DateTimeOffset)(object)datum!).DateTime, ((DateTimeOffset)(object)actual!).DateTime, TimeSpan.FromSeconds(1));
        }
        else Assert.Equal(datum, actual);
    }

    private void AssertTimeLimitedValueEncryptDecrypt<T>(T datum, DateTimeOffset expectedExpiration)
    {
        var options = new OptionsSnapshot<JsonOptions>(new());
        ITokenProtector<T> prot = new TokenProtector<T>(protectionProvider, options);
        var actual = prot.UnProtect(prot.Protect(datum, expectedExpiration), out DateTimeOffset actualExpiration);
        Assert.Equal(datum, actual);
        Assert.Equal(expectedExpiration, actualExpiration);
    }

    private void AssertTimeLimitedValueEncryptDecrypt<T>(T datum, TimeSpan lifespan)
    {
        var options = new OptionsSnapshot<JsonOptions>(new());
        ITokenProtector<T> prot = new TokenProtector<T>(protectionProvider, options);
        var actual = prot.UnProtect(prot.Protect(datum, lifespan), out _);
        Assert.Equal(datum, actual);
    }

    private class OptionsSnapshot<TOptions>(TOptions value) : IOptionsSnapshot<TOptions> where TOptions : class
    {
        public TOptions Value { get; } = value;
        public TOptions Get(string? name) => Value;
    }
}
