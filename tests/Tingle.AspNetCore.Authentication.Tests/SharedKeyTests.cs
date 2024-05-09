using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.TestHost;
using System.Globalization;
using System.Net;
using System.Reflection;
using System.Security.Claims;
using System.Text;
using System.Xml.Linq;
using Tingle.AspNetCore.Authentication.SharedKey;
using Tingle.AspNetCore.Authentication.SharedKey.Validation;
using Tingle.AspNetCore.Authentication.SharedKey.Validation.Exceptions;

#nullable disable // aligned with tests for inbuilt authentication

namespace Tingle.AspNetCore.Authentication.Tests;

public class SharedKeyTests : SharedAuthenticationTests<SharedKeyOptions>
{
    protected override string DefaultScheme => SharedKeyDefaults.AuthenticationScheme;
    protected override Type HandlerType => typeof(SharedKeyHandler);
    protected override bool SupportsSignIn { get => false; }
    protected override bool SupportsSignOut { get => false; }

    protected override void RegisterAuth(AuthenticationBuilder services, Action<SharedKeyOptions> configure)
    {
        services.AddSharedKey(o =>
        {
            ConfigureDefaults(o);
            configure.Invoke(o);
        });
    }

    private void ConfigureDefaults(SharedKeyOptions o)
    {
    }

    [Fact]
    public async Task SharedKeyTokenValidation()
    {
        var date = DateTimeOffset.UtcNow.ToString("r");
        (var tokenText, var key) = CreateStandardTokenAndKey(date, "/oauth");

        var server = CreateServer(o =>
        {
            o.ValidationParameters = new SharedKeyTokenValidationParameters
            {
                KnownFixedKeys = [],
                KeysResolver = (ctx) => Task.FromResult((IEnumerable<string>)[key])
            };
        });

        var newSharedKeyToken = "SharedKey " + tokenText;
        var response = await SendAsync(server, "http://example.com/oauth", newSharedKeyToken, date);
        Assert.Equal(HttpStatusCode.OK, response.Response.StatusCode);
    }

    [Fact]
    public async Task SaveSharedKeyToken()
    {
        var date = DateTimeOffset.UtcNow.ToString("r");
        (var tokenText, var key) = CreateStandardTokenAndKey(date, "/token");

        var server = CreateServer(o =>
        {
            o.SaveToken = true;
            o.ValidationParameters = new SharedKeyTokenValidationParameters
            {
                KnownFixedKeys = [],
                KeysResolver = (ctx) => Task.FromResult((IEnumerable<string>)[key])
            };
        });

        var newSharedKeyToken = "SharedKey " + tokenText;
        var response = await SendAsync(server, "http://example.com/token", newSharedKeyToken, date);
        Assert.Equal(HttpStatusCode.OK, response.Response.StatusCode);
        Assert.Equal(tokenText, await response.Response.Content.ReadAsStringAsync());
    }

    [Fact]
    public async Task SignInThrows()
    {
        var server = CreateServer();
        var transaction = await server.SendAsync("https://example.com/signIn");
        Assert.Equal(HttpStatusCode.OK, transaction.Response.StatusCode);
    }

    [Fact]
    public async Task SignOutThrows()
    {
        var server = CreateServer();
        var transaction = await server.SendAsync("https://example.com/signOut");
        Assert.Equal(HttpStatusCode.OK, transaction.Response.StatusCode);
    }

    [Fact]
    public async Task ThrowAtAuthenticationFailedEvent()
    {
        var server = CreateServer(o =>
        {
            o.Events = new SharedKeyEvents
            {
                OnAuthenticationFailed = context =>
                {
                    context.Response.StatusCode = 401;
                    throw new Exception();
                },
                OnMessageReceived = context =>
                {
                    context.Token = "something";
                    return Task.FromResult(0);
                }
            };
        },
        async (context, next) =>
        {
            try
            {
                await next();
                Assert.Fail("Expected exception is not thrown");
            }
            catch (Exception)
            {
                context.Response.StatusCode = 401;
                await context.Response.WriteAsync("i got this");
            }
        });

        var transaction = await server.SendAsync("https://example.com/signIn");

        Assert.Equal(HttpStatusCode.Unauthorized, transaction.Response.StatusCode);
    }

    [Fact]
    public async Task CustomHeaderReceived()
    {
        var server = CreateServer(o =>
        {
            o.Events = new SharedKeyEvents()
            {
                OnMessageReceived = context =>
                {
                    var claims = new[]
                    {
                            new Claim(ClaimTypes.NameIdentifier, "Bob le Magnifique"),
                            new Claim(ClaimTypes.Email, "bob@contoso.com"),
                            new Claim(ClaimsIdentity.DefaultNameClaimType, "bob")
                    };

                    context.Principal = new ClaimsPrincipal(new ClaimsIdentity(claims, context.Scheme.Name));
                    context.Success();

                    return Task.FromResult<object>(null);
                }
            };
        });

        var response = await SendAsync(server, "http://example.com/oauth", "someHeader someblob");
        Assert.Equal(HttpStatusCode.OK, response.Response.StatusCode);
        Assert.Equal("Bob le Magnifique", response.ResponseText);
    }

    [Fact]
    public async Task NoHeaderReceived()
    {
        var server = CreateServer();
        var response = await SendAsync(server, "http://example.com/oauth");
        Assert.Equal(HttpStatusCode.Unauthorized, response.Response.StatusCode);
    }

    [Fact]
    public async Task HeaderWithoutSharedKeyReceived()
    {
        var server = CreateServer();
        var response = await SendAsync(server, "http://example.com/oauth", "Token");
        Assert.Equal(HttpStatusCode.Unauthorized, response.Response.StatusCode);
    }

    [Fact]
    public async Task UnrecognizedTokenReceived()
    {
        var server = CreateServer();
        var response = await SendAsync(server, "http://example.com/oauth", "SharedKey someblob");
        Assert.Equal(HttpStatusCode.Unauthorized, response.Response.StatusCode);
        Assert.Equal("", response.ResponseText);
    }

    [Fact]
    public async Task InvalidTokenReceived()
    {
        var server = CreateServer(options =>
        {
            options.TokenValidators.Clear();
            options.TokenValidators.Add(new InvalidTokenValidator());
        });

        var response = await SendAsync(server, "http://example.com/oauth", "SharedKey someblob");
        Assert.Equal(HttpStatusCode.Unauthorized, response.Response.StatusCode);
        Assert.Equal("SharedKey error=\"invalid_token\"", response.Response.Headers.WwwAuthenticate.First().ToString());
        Assert.Equal("", response.ResponseText);
    }

    [Fact]
    public async Task InvalidTokenReceived_NullKeys()
    {
        var date = DateTimeOffset.UtcNow.ToString("r");
        (var tokenText, var key) = CreateStandardTokenAndKey(date, "/oauth");

        var server = CreateServer(o =>
        {
            o.ValidationParameters = new SharedKeyTokenValidationParameters
            {
                KnownFixedKeys = [],
                KeysResolver = (ctx) => Task.FromResult((IEnumerable<string>)null)
            };
        });

        var newSharedKeyToken = "SharedKey " + tokenText;
        var response = await SendAsync(server, "http://example.com/oauth", newSharedKeyToken, date);
        Assert.Equal(HttpStatusCode.Unauthorized, response.Response.StatusCode);
        Assert.Equal("SharedKey error=\"invalid_token\", error_description=\"Unable to resolve signing keys\"", response.Response.Headers.WwwAuthenticate.First().ToString());
        Assert.Equal("", response.ResponseText);
    }

    [Fact]
    public async Task InvalidTokenReceived_NoKey()
    {
        var date = DateTimeOffset.UtcNow.ToString("r");
        (var tokenText, var key) = CreateStandardTokenAndKey(date, "/oauth");

        var server = CreateServer(o =>
        {
            o.ValidationParameters = new SharedKeyTokenValidationParameters
            {
                KnownFixedKeys = [],
                KeysResolver = (ctx) => Task.FromResult<IEnumerable<string>>([])
            };
        });

        var newSharedKeyToken = "SharedKey " + tokenText;
        var response = await SendAsync(server, "http://example.com/oauth", newSharedKeyToken, date);
        Assert.Equal(HttpStatusCode.Unauthorized, response.Response.StatusCode);
        Assert.Equal("SharedKey error=\"invalid_token\", error_description=\"Unable to resolve signing keys\"", response.Response.Headers.WwwAuthenticate.First().ToString());
        Assert.Equal("", response.ResponseText);
    }

    [Fact]
    public async Task InvalidTokenReceived_NotBase64Key()
    {
        var date = DateTimeOffset.UtcNow.ToString("r");
        (var tokenText, var key) = CreateStandardTokenAndKey(date, "/oauth");

        var server = CreateServer(o =>
        {
            o.ValidationParameters = new SharedKeyTokenValidationParameters
            {
                KnownFixedKeys = [],
                KeysResolver = (ctx) => Task.FromResult((IEnumerable<string>)["not-base64", "BEahPY/aD0KqvZdLuNQJBw=="])
            };
        });

        var newSharedKeyToken = "SharedKey " + tokenText;
        var response = await SendAsync(server, "http://example.com/oauth", newSharedKeyToken, date);
        Assert.Equal(HttpStatusCode.Unauthorized, response.Response.StatusCode);
        Assert.Equal("SharedKey error=\"invalid_token\", error_description=\"Invalid signing keys\"", response.Response.Headers.WwwAuthenticate.First().ToString());
        Assert.Equal("", response.ResponseText);
    }

    [Theory]
    [InlineData(typeof(SharedKeyInvalidDateException), "The supplied date is invalid; Supplied: '(null)'")]
    [InlineData(typeof(SharedKeyTimeWindowExpiredException), "The supplied date is invalid; NotOlderThan: '01/01/0001 00:00:00 +00:00', Supplied: '01/01/0001 00:00:00 +00:00'")]
    [InlineData(typeof(SharedKeyInvalidSignatureException), "The signature is invalid")]
    [InlineData(typeof(SharedKeyNoDateException), "Date header must be supplied in any of these headers ()")]
    [InlineData(typeof(SharedKeyNoKeysException), "Unable to resolve signing keys")]
    [InlineData(typeof(SharedKeyInvalidSigningKeysException), "Invalid signing keys")]
    public async Task ExceptionReportedInHeaderForAuthenticationFailures(Type errorType, string message)
    {
        var server = CreateServer(options =>
        {
            options.TokenValidators.Clear();
            options.TokenValidators.Add(new InvalidTokenValidator(errorType));
        });

        var response = await SendAsync(server, "http://example.com/oauth", "SharedKey someblob");
        Assert.Equal(HttpStatusCode.Unauthorized, response.Response.StatusCode);
        Assert.Equal($"SharedKey error=\"invalid_token\", error_description=\"{message}\"", response.Response.Headers.WwwAuthenticate.First().ToString());
        Assert.Equal("", response.ResponseText);
    }

    [Theory]
    [InlineData(typeof(SharedKeyInvalidDateException), "The supplied date is invalid; Supplied: '01/15/2045 00:00:00 +00:00'")]
    [InlineData(typeof(SharedKeyTimeWindowExpiredException), "The supplied date is invalid; NotOlderThan: '01/15/2001 00:00:00 +00:00', Supplied: '02/20/2000 00:00:00 +00:00'")]
    [InlineData(typeof(SharedKeyNoDateException), "Date header must be supplied in any of these headers (x-ts-date, x-ms-date)")]
    public async Task ExceptionReportedInHeaderWithDetailsForAuthenticationFailures(Type errorType, string message)
    {
        var server = CreateServer(options =>
        {
            options.TokenValidators.Clear();
            options.TokenValidators.Add(new DetailedInvalidTokenValidator(errorType));
        });

        var response = await SendAsync(server, "http://example.com/oauth", "SharedKey someblob");
        Assert.Equal(HttpStatusCode.Unauthorized, response.Response.StatusCode);
        Assert.Equal($"SharedKey error=\"invalid_token\", error_description=\"{message}\"", response.Response.Headers.WwwAuthenticate.First().ToString());
        Assert.Equal("", response.ResponseText);
    }

    [Theory]
    [InlineData(typeof(ArgumentException))]
    public async Task ExceptionNotReportedInHeaderForOtherFailures(Type errorType)
    {
        var server = CreateServer(options =>
        {
            options.TokenValidators.Clear();
            options.TokenValidators.Add(new InvalidTokenValidator(errorType));
        });

        var response = await SendAsync(server, "http://example.com/oauth", "SharedKey someblob");
        Assert.Equal(HttpStatusCode.Unauthorized, response.Response.StatusCode);
        Assert.Equal("SharedKey error=\"invalid_token\"", response.Response.Headers.WwwAuthenticate.First().ToString());
        Assert.Equal("", response.ResponseText);
    }

    [Fact]
    public async Task ExceptionsReportedInHeaderForMultipleAuthenticationFailures()
    {
        var server = CreateServer(options =>
        {
            options.TokenValidators.Clear();
            options.TokenValidators.Add(new InvalidTokenValidator(typeof(SharedKeyNoKeysException)));
            options.TokenValidators.Add(new InvalidTokenValidator(typeof(SharedKeyInvalidSignatureException)));
        });

        var response = await SendAsync(server, "http://example.com/oauth", "SharedKey someblob");
        Assert.Equal(HttpStatusCode.Unauthorized, response.Response.StatusCode);
        Assert.Equal("SharedKey error=\"invalid_token\", error_description=\"Unable to resolve signing keys; The signature is invalid\"",
            response.Response.Headers.WwwAuthenticate.First().ToString());
        Assert.Equal("", response.ResponseText);
    }

    [Theory]
    [InlineData("custom_error", "custom_description", "custom_uri")]
    [InlineData("custom_error", "custom_description", null)]
    [InlineData("custom_error", null, null)]
    [InlineData(null, "custom_description", "custom_uri")]
    [InlineData(null, "custom_description", null)]
    [InlineData(null, null, "custom_uri")]
    public async Task ExceptionsReportedInHeaderExposesUserDefinedError(string error, string description, string uri)
    {
        var server = CreateServer(options =>
        {
            options.Events = new SharedKeyEvents
            {
                OnChallenge = context =>
                {
                    context.Error = error;
                    context.ErrorDescription = description;
                    context.ErrorUri = uri;

                    return Task.FromResult(0);
                }
            };
        });

        var response = await SendAsync(server, "http://example.com/oauth", "SharedKey someblob");
        Assert.Equal(HttpStatusCode.Unauthorized, response.Response.StatusCode);
        Assert.Equal("", response.ResponseText);

        var builder = new StringBuilder(SharedKeyDefaults.AuthenticationScheme);

        if (!string.IsNullOrEmpty(error))
        {
            builder.Append(" error=\"");
            builder.Append(error);
            builder.Append('"');
        }
        if (!string.IsNullOrEmpty(description))
        {
            if (!string.IsNullOrEmpty(error))
            {
                builder.Append(',');
            }

            builder.Append(" error_description=\"");
            builder.Append(description);
            builder.Append('\"');
        }
        if (!string.IsNullOrEmpty(uri))
        {
            if (!string.IsNullOrEmpty(error) ||
                !string.IsNullOrEmpty(description))
            {
                builder.Append(',');
            }

            builder.Append(" error_uri=\"");
            builder.Append(uri);
            builder.Append('\"');
        }

        Assert.Equal(builder.ToString(), response.Response.Headers.WwwAuthenticate.First().ToString());
    }

    [Fact]
    public async Task ExceptionNotReportedInHeaderWhenIncludeErrorDetailsIsFalse()
    {
        var server = CreateServer(o =>
        {
            o.IncludeErrorDetails = false;
        });

        var response = await SendAsync(server, "http://example.com/oauth", "SharedKey someblob");
        Assert.Equal(HttpStatusCode.Unauthorized, response.Response.StatusCode);
        Assert.Equal("SharedKey", response.Response.Headers.WwwAuthenticate.First().ToString());
        Assert.Equal("", response.ResponseText);
    }

    [Fact]
    public async Task ExceptionNotReportedInHeaderWhenTokenWasMissing()
    {
        var server = CreateServer();

        var response = await SendAsync(server, "http://example.com/oauth");
        Assert.Equal(HttpStatusCode.Unauthorized, response.Response.StatusCode);
        Assert.Equal("SharedKey", response.Response.Headers.WwwAuthenticate.First().ToString());
        Assert.Equal("", response.ResponseText);
    }

    [Fact]
    public async Task CustomTokenValidated()
    {
        var server = CreateServer(options =>
        {
            options.Events = new SharedKeyEvents()
            {
                OnTokenValidated = context =>
                {
                    // Retrieve the NameIdentifier claim from the identity
                    // returned by the custom security token validator.
                    var identity = (ClaimsIdentity)context.Principal.Identity;
                    var identifier = identity.FindFirst(ClaimTypes.NameIdentifier);

                    Assert.Equal("Bob le Tout Puissant", identifier.Value);

                    // Remove the existing NameIdentifier claim and replace it
                    // with a new one containing a different value.
                    identity.RemoveClaim(identifier);
                    // Make sure to use a different name identifier
                    // than the one defined by BlobTokenValidator.
                    identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, "Bob le Magnifique"));

                    return Task.FromResult<object>(null);
                }
            };
            options.TokenValidators.Clear();
            options.TokenValidators.Add(new BlobTokenValidator(SharedKeyDefaults.AuthenticationScheme));
        });

        var response = await SendAsync(server, "http://example.com/oauth", "SharedKey someblob");
        Assert.Equal(HttpStatusCode.OK, response.Response.StatusCode);
        Assert.Equal("Bob le Magnifique", response.ResponseText);
    }

    [Fact]
    public async Task RetrievingTokenFromAlternateLocation()
    {
        var server = CreateServer(options =>
        {
            options.Events = new SharedKeyEvents()
            {
                OnMessageReceived = context =>
                {
                    context.Token = "CustomToken";
                    return Task.FromResult<object>(null);
                }
            };
            options.TokenValidators.Clear();
            options.TokenValidators.Add(new BlobTokenValidator("SK", token =>
            {
                Assert.Equal("CustomToken", token);
            }));
        });

        var response = await SendAsync(server, "http://example.com/oauth", "SharedKey Token");
        Assert.Equal(HttpStatusCode.OK, response.Response.StatusCode);
        Assert.Equal("Bob le Tout Puissant", response.ResponseText);
    }

    [Fact]
    public async Task EventOnMessageReceivedSkip_NoMoreEventsExecuted()
    {
        var server = CreateServer(options =>
        {
            options.Events = new SharedKeyEvents()
            {
                OnMessageReceived = context =>
                {
                    context.NoResult();
                    return Task.FromResult(0);
                },
                OnTokenValidated = context =>
                {
                    throw new NotImplementedException();
                },
                OnAuthenticationFailed = context =>
                {
                    throw new NotImplementedException(context.Exception.ToString());
                },
                OnChallenge = context =>
                {
                    throw new NotImplementedException();
                },
            };
        });

        var response = await SendAsync(server, "http://example.com/checkforerrors", "SharedKey Token");
        Assert.Equal(HttpStatusCode.OK, response.Response.StatusCode);
        Assert.Equal(string.Empty, response.ResponseText);
    }

    [Fact]
    public async Task EventOnMessageReceivedReject_NoMoreEventsExecuted()
    {
        var server = CreateServer(options =>
        {
            options.Events = new SharedKeyEvents()
            {
                OnMessageReceived = context =>
                {
                    context.Fail("Authentication was aborted from user code.");
                    context.Response.StatusCode = StatusCodes.Status202Accepted;
                    return Task.FromResult(0);
                },
                OnTokenValidated = context =>
                {
                    throw new NotImplementedException();
                },
                OnAuthenticationFailed = context =>
                {
                    throw new NotImplementedException(context.Exception.ToString());
                },
                OnChallenge = context =>
                {
                    throw new NotImplementedException();
                },
            };
        });

        var exception = await Assert.ThrowsAsync<Exception>(delegate
        {
            return SendAsync(server, "http://example.com/checkforerrors", "SharedKey Token");
        });

        Assert.Equal("Authentication was aborted from user code.", exception.InnerException.Message);
    }

    [Fact]
    public async Task EventOnTokenValidatedSkip_NoMoreEventsExecuted()
    {
        var server = CreateServer(options =>
        {
            options.Events = new SharedKeyEvents()
            {
                OnTokenValidated = context =>
                {
                    context.NoResult();
                    return Task.FromResult(0);
                },
                OnAuthenticationFailed = context =>
                {
                    throw new NotImplementedException(context.Exception.ToString());
                },
                OnChallenge = context =>
                {
                    throw new NotImplementedException();
                },
            };
            options.TokenValidators.Clear();
            options.TokenValidators.Add(new BlobTokenValidator("SK"));
        });

        var response = await SendAsync(server, "http://example.com/checkforerrors", "SharedKey Token");
        Assert.Equal(HttpStatusCode.OK, response.Response.StatusCode);
        Assert.Equal(string.Empty, response.ResponseText);
    }

    [Fact]
    public async Task EventOnTokenValidatedReject_NoMoreEventsExecuted()
    {
        var server = CreateServer(options =>
        {
            options.Events = new SharedKeyEvents()
            {
                OnTokenValidated = context =>
                {
                    context.Fail("Authentication was aborted from user code.");
                    context.Response.StatusCode = StatusCodes.Status202Accepted;
                    return Task.FromResult(0);
                },
                OnAuthenticationFailed = context =>
                {
                    throw new NotImplementedException(context.Exception.ToString());
                },
                OnChallenge = context =>
                {
                    throw new NotImplementedException();
                },
            };
            options.TokenValidators.Clear();
            options.TokenValidators.Add(new BlobTokenValidator("SK"));
        });

        var exception = await Assert.ThrowsAsync<Exception>(delegate
        {
            return SendAsync(server, "http://example.com/checkforerrors", "SharedKey Token");
        });

        Assert.Equal("Authentication was aborted from user code.", exception.InnerException.Message);
    }

    [Fact]
    public async Task EventOnAuthenticationFailedSkip_NoMoreEventsExecuted()
    {
        var server = CreateServer(options =>
        {
            options.Events = new SharedKeyEvents()
            {
                OnTokenValidated = context =>
                {
                    throw new Exception("Test Exception");
                },
                OnAuthenticationFailed = context =>
                {
                    context.NoResult();
                    return Task.FromResult(0);
                },
                OnChallenge = context =>
                {
                    throw new NotImplementedException();
                },
            };
            options.TokenValidators.Clear();
            options.TokenValidators.Add(new BlobTokenValidator("SK"));
        });

        var response = await SendAsync(server, "http://example.com/checkforerrors", "SharedKey Token");
        Assert.Equal(HttpStatusCode.OK, response.Response.StatusCode);
        Assert.Equal(string.Empty, response.ResponseText);
    }

    [Fact]
    public async Task EventOnAuthenticationFailedReject_NoMoreEventsExecuted()
    {
        var server = CreateServer(options =>
        {
            options.Events = new SharedKeyEvents()
            {
                OnTokenValidated = context =>
                {
                    throw new Exception("Test Exception");
                },
                OnAuthenticationFailed = context =>
                {
                    context.Fail("Authentication was aborted from user code.");
                    context.Response.StatusCode = StatusCodes.Status202Accepted;
                    return Task.FromResult(0);
                },
                OnChallenge = context =>
                {
                    throw new NotImplementedException();
                },
            };
            options.TokenValidators.Clear();
            options.TokenValidators.Add(new BlobTokenValidator("SK"));
        });

        var exception = await Assert.ThrowsAsync<Exception>(delegate
        {
            return SendAsync(server, "http://example.com/checkforerrors", "SharedKey Token");
        });

        Assert.Equal("Authentication was aborted from user code.", exception.InnerException.Message);
    }

    [Fact]
    public async Task EventOnChallengeSkip_ResponseNotModified()
    {
        var server = CreateServer(o =>
        {
            o.Events = new SharedKeyEvents()
            {
                OnChallenge = context =>
                {
                    context.HandleResponse();
                    return Task.FromResult(0);
                },
            };
        });

        var response = await SendAsync(server, "http://example.com/unauthorized", "SharedKey Token");
        Assert.Equal(HttpStatusCode.OK, response.Response.StatusCode);
        Assert.Empty(response.Response.Headers.WwwAuthenticate);
        Assert.Equal(string.Empty, response.ResponseText);
    }

    [Fact]
    public async Task EventOnForbidden_ResponseNotModified()
    {
        var date = DateTimeOffset.UtcNow.ToString("r");
        (var tokenText, var key) = CreateStandardTokenAndKey(date, "/forbidden");

        var server = CreateServer(o =>
        {
            o.ValidationParameters = new SharedKeyTokenValidationParameters
            {
                KnownFixedKeys = [],
                KeysResolver = (ctx) => Task.FromResult((IEnumerable<string>)[key])
            };
        });
        var newSharedKeyToken = "SharedKey " + tokenText;
        var response = await SendAsync(server, "http://example.com/forbidden", newSharedKeyToken, date);
        Assert.Equal(HttpStatusCode.Forbidden, response.Response.StatusCode);
    }

    [Fact]
    public async Task EventOnForbiddenSkip_ResponseNotModified()
    {
        var date = DateTimeOffset.UtcNow.ToString("r");
        (var tokenText, var key) = CreateStandardTokenAndKey(date, "/forbidden");
        var server = CreateServer(o =>
        {
            o.ValidationParameters = new SharedKeyTokenValidationParameters
            {
                KnownFixedKeys = [],
                KeysResolver = (ctx) => Task.FromResult((IEnumerable<string>)[key])
            };
            o.Events = new SharedKeyEvents()
            {
                OnForbidden = context =>
                {
                    return Task.FromResult(0);
                }
            };
        });
        var newSharedKeyToken = "SharedKey " + tokenText;
        var response = await SendAsync(server, "http://example.com/forbidden", newSharedKeyToken, date);
        Assert.Equal(HttpStatusCode.Forbidden, response.Response.StatusCode);
    }

    [Fact]
    public async Task EventOnForbidden_ResponseModified()
    {
        var date = DateTimeOffset.UtcNow.ToString("r");
        (var tokenText, var key) = CreateStandardTokenAndKey(date, "/forbidden");
        var server = CreateServer(o =>
        {
            o.ValidationParameters = new SharedKeyTokenValidationParameters
            {
                KnownFixedKeys = [],
                KeysResolver = (ctx) => Task.FromResult((IEnumerable<string>)[key])
            };
            o.Events = new SharedKeyEvents()
            {
                OnForbidden = context =>
                {
                    context.Response.StatusCode = 418;
                    return context.Response.WriteAsync("You Shall Not Pass");
                }
            };
        });
        var newSharedKeyToken = "SharedKey " + tokenText;
        var response = await SendAsync(server, "http://example.com/forbidden", newSharedKeyToken, date);
        Assert.Equal(418, (int)response.Response.StatusCode);
        Assert.Equal("You Shall Not Pass", await response.Response.Content.ReadAsStringAsync());
    }

    class InvalidTokenValidator : ISharedKeyTokenValidator
    {
        public InvalidTokenValidator()
        {
            ExceptionType = typeof(SharedKeyTokenException);
        }

        public InvalidTokenValidator(Type exceptionType)
        {
            ExceptionType = exceptionType;
        }

        public Type ExceptionType { get; set; }

        public bool CanReadToken(string securityToken) => true;

        public Task<(ClaimsPrincipal, SharedKeyValidatedToken)> ValidateTokenAsync(string securityToken,
                                                                                   HttpContext httpContext,
                                                                                   SharedKeyOptions options)
        {
            var constructor = ExceptionType.GetTypeInfo().GetConstructor([typeof(string)]);
            var exception = (Exception)constructor.Invoke([ExceptionType.Name]);
            throw exception;
        }
    }

    class DetailedInvalidTokenValidator : ISharedKeyTokenValidator
    {
        public DetailedInvalidTokenValidator()
        {
            ExceptionType = typeof(SharedKeyTokenException);
        }

        public DetailedInvalidTokenValidator(Type exceptionType)
        {
            ExceptionType = exceptionType;
        }

        public Type ExceptionType { get; set; }

        public bool CanReadToken(string securityToken) => true;

        public Task<(ClaimsPrincipal, SharedKeyValidatedToken)> ValidateTokenAsync(string securityToken,
                                                                                   HttpContext httpContext,
                                                                                   SharedKeyOptions options)
        {
            if (ExceptionType == typeof(SharedKeyNoDateException))
            {
                throw SharedKeyNoDateException.Create(["x-ts-date", "x-ms-date"]);
            }
            if (ExceptionType == typeof(SharedKeyTimeWindowExpiredException))
            {
                throw SharedKeyTimeWindowExpiredException.Create(new DateTimeOffset(2000, 2, 20, 0, 0, 0, TimeSpan.Zero),
                                                                 new DateTimeOffset(2001, 1, 15, 0, 0, 0, TimeSpan.Zero));
            }
            if (ExceptionType == typeof(SharedKeyInvalidDateException))
            {
                var value = new DateTimeOffset(2045, 1, 15, 0, 0, 0, TimeSpan.Zero).ToString(CultureInfo.InvariantCulture);
                throw SharedKeyInvalidDateException.Create(value);
            }
            else
            {
                throw new NotImplementedException(ExceptionType.Name);
            }
        }
    }

    class BlobTokenValidator : ISharedKeyTokenValidator
    {
        private readonly Action<string> _tokenValidator;

        public BlobTokenValidator(string authenticationScheme)
        {
            AuthenticationScheme = authenticationScheme;

        }
        public BlobTokenValidator(string authenticationScheme, Action<string> tokenValidator)
        {
            AuthenticationScheme = authenticationScheme;
            _tokenValidator = tokenValidator;
        }

        public string AuthenticationScheme { get; }

        public bool CanReadToken(string securityToken) => true;

        public Task<(ClaimsPrincipal, SharedKeyValidatedToken)> ValidateTokenAsync(string securityToken,
                                                                                   HttpContext httpContext,
                                                                                   SharedKeyOptions options)
        {
            SharedKeyValidatedToken validatedToken = null;
            _tokenValidator?.Invoke(securityToken);

            var claims = new[]
            {
                // Make sure to use a different name identifier
                // than the one defined by CustomTokenValidated.
                new Claim(ClaimTypes.NameIdentifier, "Bob le Tout Puissant"),
                new Claim(ClaimTypes.Email, "bob@contoso.com"),
                new Claim(ClaimsIdentity.DefaultNameClaimType, "bob"),
            };

            var principal = new ClaimsPrincipal(new ClaimsIdentity(claims, AuthenticationScheme));
            return Task.FromResult((principal, validatedToken));
        }
    }

    private static TestServer CreateServer(Action<SharedKeyOptions> options = null, Func<HttpContext, Func<Task>, Task> handlerBeforeAuth = null)
    {
        var builder = new WebHostBuilder()
            .Configure(app =>
            {
                if (handlerBeforeAuth != null)
                {
                    app.Use(handlerBeforeAuth);
                }

                app.UseAuthentication();
                app.Use(async (context, next) =>
                {
                    if (context.Request.Path == new PathString("/checkforerrors"))
                    {
                        var result = await context.AuthenticateAsync(SharedKeyDefaults.AuthenticationScheme); // this used to be "Automatic"
                        if (result.Failure != null)
                        {
                            throw new Exception("Failed to authenticate", result.Failure);
                        }
                        return;
                    }
                    else if (context.Request.Path == new PathString("/oauth"))
                    {
                        if (context.User == null ||
                            context.User.Identity == null ||
                            !context.User.Identity.IsAuthenticated)
                        {
                            context.Response.StatusCode = 401;
                            // REVIEW: no more automatic challenge
                            await context.ChallengeAsync(SharedKeyDefaults.AuthenticationScheme);
                            return;
                        }

                        var identifier = context.User.FindFirst(ClaimTypes.NameIdentifier);
                        if (identifier == null)
                        {
                            context.Response.StatusCode = 500;
                            return;
                        }

                        await context.Response.WriteAsync(identifier.Value);
                    }
                    else if (context.Request.Path == new PathString("/token"))
                    {
                        var token = await context.GetTokenAsync("access_token");
                        await context.Response.WriteAsync(token);
                    }
                    else if (context.Request.Path == new PathString("/unauthorized"))
                    {
                        // Simulate Authorization failure 
                        var result = await context.AuthenticateAsync(SharedKeyDefaults.AuthenticationScheme);
                        await context.ChallengeAsync(SharedKeyDefaults.AuthenticationScheme);
                    }
                    else if (context.Request.Path == new PathString("/forbidden"))
                    {
                        // Simulate Forbidden
                        await context.ForbidAsync(SharedKeyDefaults.AuthenticationScheme);
                    }
                    else if (context.Request.Path == new PathString("/signIn"))
                    {
                        await Assert.ThrowsAsync<InvalidOperationException>(() => context.SignInAsync(SharedKeyDefaults.AuthenticationScheme, new ClaimsPrincipal()));
                    }
                    else if (context.Request.Path == new PathString("/signOut"))
                    {
                        await Assert.ThrowsAsync<InvalidOperationException>(() => context.SignOutAsync(SharedKeyDefaults.AuthenticationScheme));
                    }
                    else
                    {
                        await next();
                    }
                });
            })
            .ConfigureServices(services => services.AddAuthentication(SharedKeyDefaults.AuthenticationScheme).AddSharedKey(options));

        return new TestServer(builder);
    }

    // TODO: see if we can share the TestExtensions SendAsync method (only diff is auth header)
    private static async Task<Transaction> SendAsync(TestServer server, string uri, string authorizationHeader = null, string dateHeader = null)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, uri);
        if (!string.IsNullOrEmpty(authorizationHeader))
        {
            request.Headers.Add("Authorization", authorizationHeader);
        }
        if (!string.IsNullOrEmpty(dateHeader))
        {
            request.Headers.Add("x-ts-date", dateHeader);
        }

        var transaction = new Transaction
        {
            Request = request,
            Response = await server.CreateClient().SendAsync(request),
        };

        transaction.ResponseText = await transaction.Response.Content.ReadAsStringAsync();

        if (transaction.Response.Content != null &&
            transaction.Response.Content.Headers.ContentType != null &&
            transaction.Response.Content.Headers.ContentType.MediaType == "text/xml")
        {
            transaction.ResponseElement = XElement.Parse(transaction.ResponseText);
        }

        return transaction;
    }

    private static (string tokenText, string key) CreateStandardTokenAndKey(string dateHeaderValue,
                                                                            string resource,
                                                                            string method = "GET",
                                                                            int contentLength = 0,
                                                                            string contentType = null)
    {
        var sharedKeyBytes = Guid.NewGuid().ToByteArray();
        var key = Convert.ToBase64String(sharedKeyBytes);

        // collect items
        dateHeaderValue ??= string.Empty;

        var stringToHash = string.Join("\n", method, contentLength, contentType, $"x-ts-date:{dateHeaderValue}", resource);
        var bytesToHash = Encoding.ASCII.GetBytes(stringToHash);
        using var sha256 = new System.Security.Cryptography.HMACSHA256(sharedKeyBytes);
        var calculatedHash = sha256.ComputeHash(bytesToHash);
        var signature = Convert.ToBase64String(calculatedHash);
        return (tokenText: signature, key);
    }
}
