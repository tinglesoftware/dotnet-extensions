using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.DependencyInjection;
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
        var options = new OptionsSnapshot<TokenProtectorOptions>(new());

        AssertValueEncryptDecrypt(options, Guid.NewGuid());                                  // Guid
        AssertValueEncryptDecrypt(options, rnd.Next(int.MinValue, int.MaxValue));            // int
        AssertValueEncryptDecrypt(options, Convert.ToInt64(rnd.NextDouble() * int.MinValue));// long
        AssertValueEncryptDecrypt(options, rnd.NextDouble() * int.MinValue);                 // double
        AssertValueEncryptDecrypt(options, DateTimeOffset.UtcNow);                           // DateTimeOffset
        AssertValueEncryptDecrypt(options, DateTime.UtcNow);                                 // DateTime
        AssertValueEncryptDecrypt(options, Guid.NewGuid().ToString());                       // string
    }

    [Fact]
    public void Protection_With_UseConversionInsteadOfJson_Works()
    {
        var rnd = new Random();
        var options = new OptionsSnapshot<TokenProtectorOptions>(new() { UseConversionInsteadOfJson = true, });

        AssertValueEncryptDecrypt(options, Guid.NewGuid());                                        // Guid
        AssertValueEncryptDecrypt(options, rnd.Next(int.MinValue, int.MaxValue));                  // int
        AssertValueEncryptDecrypt(options, Convert.ToInt64(rnd.NextDouble() * int.MinValue));      // long
        AssertValueEncryptDecrypt(options, rnd.NextDouble() * int.MinValue);                       // double
        AssertValueEncryptDecrypt(options, DateTimeOffset.UtcNow, true);                           // DateTimeOffset
        AssertValueEncryptDecrypt(options, DateTime.UtcNow, true);                                 // DateTime
        AssertValueEncryptDecrypt(options, Guid.NewGuid().ToString());                             // string
    }

    [Fact]
    public void TimeLimited_Protection_Works()
    {
        var rnd = new Random();
        var options = new OptionsSnapshot<TokenProtectorOptions>(new());

        // test with absolute expiration
        var expiration = DateTimeOffset.UtcNow.AddSeconds(1);
        AssertTimeLimitedValueEncryptDecrypt(options, Guid.NewGuid(), expiration);                                  // Guid
        AssertTimeLimitedValueEncryptDecrypt(options, rnd.Next(int.MinValue, int.MaxValue), expiration);            // int
        AssertTimeLimitedValueEncryptDecrypt(options, Convert.ToInt64(rnd.NextDouble() * int.MinValue), expiration);// long
        AssertTimeLimitedValueEncryptDecrypt(options, rnd.NextDouble() * int.MinValue, expiration);                 // double
        AssertTimeLimitedValueEncryptDecrypt(options, DateTimeOffset.UtcNow, expiration);                           // DateTimeOffset
        AssertTimeLimitedValueEncryptDecrypt(options, DateTime.UtcNow, expiration);                                 // DateTime
        AssertTimeLimitedValueEncryptDecrypt(options, Guid.NewGuid().ToString(), expiration);                       // string

        // not test with lifespan
        var lifespan = TimeSpan.FromSeconds(60);
        AssertTimeLimitedValueEncryptDecrypt(options, Guid.NewGuid(), lifespan);                                  // Guid
        AssertTimeLimitedValueEncryptDecrypt(options, rnd.Next(int.MinValue, int.MaxValue), lifespan);            // int
        AssertTimeLimitedValueEncryptDecrypt(options, Convert.ToInt64(rnd.NextDouble() * int.MinValue), lifespan);// long
        AssertTimeLimitedValueEncryptDecrypt(options, rnd.NextDouble() * int.MinValue, lifespan);                 // double
        AssertTimeLimitedValueEncryptDecrypt(options, DateTimeOffset.UtcNow, lifespan);                           // DateTimeOffset
        AssertTimeLimitedValueEncryptDecrypt(options, DateTime.UtcNow, lifespan);                                 // DateTime
        AssertTimeLimitedValueEncryptDecrypt(options, Guid.NewGuid().ToString(), lifespan);                       // string
    }

    [Fact]
    public void TimeLimited_Protection_Works_On_DataClass()
    {
        var d = TestDataClass.CreateRandom();
        var options = new OptionsSnapshot<TokenProtectorOptions>(new());

        var expiration = DateTimeOffset.UtcNow.AddSeconds(1);
        AssertTimeLimitedValueEncryptDecrypt(options, d, expiration);

        var lifespan = TimeSpan.FromSeconds(60);
        AssertTimeLimitedValueEncryptDecrypt(options, d, lifespan);
    }

    [Fact]
    public async Task TimeLimited_Protection_Fails_Expired()
    {
        var d = TestDataClass.CreateRandom();
        var expiration = DateTimeOffset.UtcNow.AddSeconds(1);

        var options = new OptionsSnapshot<TokenProtectorOptions>(new());
        var ctc = new TokenProtector<TestDataClass>(protectionProvider, options);
        var enc = ctc.Protect(d, expiration);

        // delay the usage
        await Task.Delay(TimeSpan.FromSeconds(2));

        var ex = Assert.ThrowsAny<CryptographicException>(() => ctc.UnProtect(enc, out DateTimeOffset actualExpiration));
        Assert.StartsWith("The payload expired", ex.Message);

        var lifespan = TimeSpan.FromSeconds(1);

        ctc = new TokenProtector<TestDataClass>(protectionProvider, options);
        enc = ctc.Protect(d, lifespan);

        // delay the usage
        await Task.Delay(TimeSpan.FromSeconds(2));

        ex = Assert.ThrowsAny<CryptographicException>(() => ctc.UnProtect(enc, out DateTimeOffset actualExpiration));
        Assert.StartsWith("The payload expired", ex.Message);
    }

    private void AssertValueEncryptDecrypt<T>(IOptionsSnapshot<TokenProtectorOptions> options, T datum, bool unwrapDateTime = false)
    {
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

    private void AssertTimeLimitedValueEncryptDecrypt<T>(IOptionsSnapshot<TokenProtectorOptions> options, T datum, DateTimeOffset expectedExpiration)
    {
        ITokenProtector<T> prot = new TokenProtector<T>(protectionProvider, options);
        var actual = prot.UnProtect(prot.Protect(datum, expectedExpiration), out DateTimeOffset actualExpiration);
        Assert.Equal(datum, actual);
        Assert.Equal(expectedExpiration, actualExpiration);
    }

    private void AssertTimeLimitedValueEncryptDecrypt<T>(IOptionsSnapshot<TokenProtectorOptions> options, T datum, TimeSpan lifespan)
    {
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
