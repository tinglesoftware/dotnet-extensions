using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Primitives;
using Moq;
using System.Globalization;
using Tingle.AspNetCore.Tokens.Binders;
using Tingle.AspNetCore.Tokens.Protection;
using Xunit;

namespace Tingle.AspNetCore.Tokens.Tests;

public class ContinuationTokenModelBinderTests
{
    private const string QueryKey = "token";

    private readonly IServiceProvider serviceProvider;
    private readonly IModelBinderProvider binderProvider;
    private readonly IModelBinder binder;

    public ContinuationTokenModelBinderTests()
    {
        // prepare services and the service provider
        var services = new ServiceCollection();
        services.AddDataProtection();
        services.AddLogging();
        services.AddScoped(typeof(ITokenProtector<>), typeof(TokenProtector<>));
        serviceProvider = services.BuildServiceProvider(true).CreateScope().ServiceProvider;

        // create the model binder provider
        binderProvider = new ContinuationTokenModelBinderProvider();

        // prepare the model binder provider context
        var modelMetadata = new EmptyModelMetadataProvider().GetMetadataForType(typeof(ContinuationToken<TestDataClass>));
        var binderProviderContextMock = new Mock<ModelBinderProviderContext>();
        binderProviderContextMock.SetupGet(c => c.Metadata).Returns(modelMetadata);
        binderProviderContextMock.SetupGet(c => c.Services).Returns(serviceProvider);

        // get the binder
        binder = binderProvider.GetBinder(binderProviderContextMock.Object)!;
    }

    [Fact]
    public async Task BindModelAsync_Succeeds_NoValue()
    {
        // prepare HTTP context
        var httpContext = new DefaultHttpContext() { RequestServices = serviceProvider };

        // prepare binding context
        var bindingContext = GetBindingContext<ContinuationToken<string>>(httpContext);

        // test and assert
        Assert.False(bindingContext.Result.IsModelSet);
        await binder.BindModelAsync(bindingContext);
        Assert.True(bindingContext.Result.IsModelSet);
    }

    [Fact]
    public async Task BindModelAsync_Succeeds_WrongName()
    {
        // prepare HTTP context
        var httpContext = new DefaultHttpContext() { RequestServices = serviceProvider };
        httpContext.Request.Query = new QueryCollection(new Dictionary<string, StringValues> { ["ct"] = "baba" });

        // prepare binding context
        var bindingContext = GetBindingContext<ContinuationToken<string>>(httpContext);

        // test and assert
        Assert.False(bindingContext.Result.IsModelSet);
        await binder.BindModelAsync(bindingContext);
        Assert.True(bindingContext.Result.IsModelSet);
    }

    [Fact]
    public async Task BindModelAsync_Succeeds_EmptyValue()
    {
        // prepare HTTP context
        var httpContext = new DefaultHttpContext() { RequestServices = serviceProvider };
        httpContext.Request.Query = new QueryCollection(new Dictionary<string, StringValues> { [QueryKey] = "" });

        // prepare binding context
        var bindingContext = GetBindingContext<ContinuationToken<string>>(httpContext);

        // test and assert
        Assert.False(bindingContext.Result.IsModelSet);
        await binder.BindModelAsync(bindingContext);
        Assert.True(bindingContext.Result.IsModelSet);
    }

    [Fact]
    public async Task BindModelAsync_InvalidToken_Causes_ValidationFailure()
    {
        // prepare HTTP context
        var httpContext = new DefaultHttpContext() { RequestServices = serviceProvider };
        httpContext.Request.Query = new QueryCollection(new Dictionary<string, StringValues> { [QueryKey] = "baba" });

        // prepare binding context
        var bindingContext = GetBindingContext<ContinuationToken<string>>(httpContext);

        // assert
        Assert.Equal(0, bindingContext.ModelState.ErrorCount);
        Assert.Equal(ModelValidationState.Valid, bindingContext.ModelState.ValidationState);
        await binder.BindModelAsync(bindingContext);
        Assert.False(bindingContext.Result.IsModelSet);
        Assert.Equal(1, bindingContext.ModelState.ErrorCount);
        Assert.Equal(ModelValidationState.Invalid, bindingContext.ModelState.ValidationState);

        // prepare binding context
        bindingContext = GetBindingContext<TimedContinuationToken<TestDataClass>>(httpContext);

        // assert
        Assert.Equal(0, bindingContext.ModelState.ErrorCount);
        Assert.Equal(ModelValidationState.Valid, bindingContext.ModelState.ValidationState);
        await binder.BindModelAsync(bindingContext);
        Assert.False(bindingContext.Result.IsModelSet);
        Assert.Equal(1, bindingContext.ModelState.ErrorCount);
        Assert.Equal(ModelValidationState.Invalid, bindingContext.ModelState.ValidationState);
    }

    [Fact]
    public async Task BindModelAsync_Works()
    {
        var original = TestDataClass.CreateRandom();
        var ctProtector = serviceProvider.GetRequiredService<ITokenProtector<TestDataClass>>();
        var token = ctProtector.Protect(original);

        // prepare HTTP context
        var httpContext = new DefaultHttpContext() { RequestServices = serviceProvider };
        httpContext.Request.Query = new QueryCollection(new Dictionary<string, StringValues> { [QueryKey] = token });

        // prepare binding context
        var bindingContext = GetBindingContext<ContinuationToken<TestDataClass>>(httpContext);

        // execute
        await binder.BindModelAsync(bindingContext);

        // assert
        Assert.True(bindingContext.Result.IsModelSet);
        var ct = Assert.IsAssignableFrom<ContinuationToken<TestDataClass>>(bindingContext.Result.Model);
        Assert.Equal(token, ct.GetProtected());
        Assert.Equal(original, ct.GetValue());
    }

    [Fact]
    public async Task BindModelAsync_Timed_Works()
    {
        var original = TestDataClass.CreateRandom();
        var ctProtector = serviceProvider.GetRequiredService<ITokenProtector<TestDataClass>>();
        var expiration = DateTimeOffset.UtcNow.AddSeconds(1);
        var token = ctProtector.Protect(original, expiration);

        // prepare HTTP context
        var httpContext = new DefaultHttpContext() { RequestServices = serviceProvider };
        httpContext.Request.Query = new QueryCollection(new Dictionary<string, StringValues> { [QueryKey] = token });

        // prepare binding context
        var bindingContext = GetBindingContext<TimedContinuationToken<TestDataClass>>(httpContext);

        // execute
        await binder.BindModelAsync(bindingContext);

        // assert
        Assert.True(bindingContext.Result.IsModelSet);
        var ct = Assert.IsAssignableFrom<TimedContinuationToken<TestDataClass>>(bindingContext.Result.Model);
        Assert.Equal(token, ct.GetProtected());
        Assert.Equal(original, ct.GetValue());
    }

    [Fact]
    public async Task BindModelAsync_Fails_ExpiredToken()
    {
        var original = TestDataClass.CreateRandom();
        var ctProtector = serviceProvider.GetRequiredService<ITokenProtector<TestDataClass>>();
        var expiration = DateTimeOffset.UtcNow.AddSeconds(1);
        var token = ctProtector.Protect(original, expiration);

        // delay the usage
        await Task.Delay(TimeSpan.FromSeconds(2));

        // prepare HTTP context
        var httpContext = new DefaultHttpContext() { RequestServices = serviceProvider };
        httpContext.Request.Query = new QueryCollection(new Dictionary<string, StringValues> { [QueryKey] = token });

        // prepare binding context
        var bindingContext = GetBindingContext<TimedContinuationToken<TestDataClass>>(httpContext);

        // assert
        Assert.Equal(0, bindingContext.ModelState.ErrorCount);
        Assert.Equal(ModelValidationState.Valid, bindingContext.ModelState.ValidationState);
        await binder.BindModelAsync(bindingContext);
        Assert.False(bindingContext.Result.IsModelSet);
        Assert.Equal(1, bindingContext.ModelState.ErrorCount);
        Assert.Equal(ModelValidationState.Invalid, bindingContext.ModelState.ValidationState);
    }

    [Fact]
    public async Task BindModelAsync_Fails_Timed_Resolved_Without_Time()
    {
        var original = TestDataClass.CreateRandom();
        var ctProtector = serviceProvider.GetRequiredService<ITokenProtector<TestDataClass>>();
        var token = ctProtector.Protect(original);

        // delay the usage
        await Task.Delay(TimeSpan.FromSeconds(2));

        // prepare HTTP context
        var httpContext = new DefaultHttpContext() { RequestServices = serviceProvider };
        httpContext.Request.Query = new QueryCollection(new Dictionary<string, StringValues> { [QueryKey] = token });

        // prepare binding context
        var bindingContext = GetBindingContext<TimedContinuationToken<TestDataClass>>(httpContext);

        // assert
        Assert.Equal(0, bindingContext.ModelState.ErrorCount);
        Assert.Equal(ModelValidationState.Valid, bindingContext.ModelState.ValidationState);
        await binder.BindModelAsync(bindingContext);
        Assert.False(bindingContext.Result.IsModelSet);
        Assert.Equal(1, bindingContext.ModelState.ErrorCount);
        Assert.Equal(ModelValidationState.Invalid, bindingContext.ModelState.ValidationState);
    }

    [Fact]
    public async Task BindModelAsync_Fails_UnTimed_Resolved_With_Time()
    {
        var original = TestDataClass.CreateRandom();
        var ctProtector = serviceProvider.GetRequiredService<ITokenProtector<TestDataClass>>();
        var expiration = DateTimeOffset.UtcNow.AddSeconds(1);
        var token = ctProtector.Protect(original, expiration);

        // delay the usage
        await Task.Delay(TimeSpan.FromSeconds(2));

        // prepare HTTP context
        var httpContext = new DefaultHttpContext() { RequestServices = serviceProvider };
        httpContext.Request.Query = new QueryCollection(new Dictionary<string, StringValues> { [QueryKey] = token });

        // prepare binding context
        var bindingContext = GetBindingContext<ContinuationToken<TestDataClass>>(httpContext);

        // assert
        Assert.Equal(0, bindingContext.ModelState.ErrorCount);
        Assert.Equal(ModelValidationState.Valid, bindingContext.ModelState.ValidationState);
        await binder.BindModelAsync(bindingContext);
        Assert.False(bindingContext.Result.IsModelSet);
        Assert.Equal(1, bindingContext.ModelState.ErrorCount);
        Assert.Equal(ModelValidationState.Invalid, bindingContext.ModelState.ValidationState);
    }


    private static ModelBindingContext GetBindingContext<TModel>(HttpContext httpContext)
        => GetBindingContext(typeof(TModel), httpContext);
    private static ModelBindingContext GetBindingContext(Type modelType, HttpContext httpContext)
    {
        var bindingContext = new DefaultModelBindingContext
        {
            ModelMetadata = new EmptyModelMetadataProvider().GetMetadataForType(modelType),
            ModelName = "model",
            ModelState = new ModelStateDictionary(),
            ValidationState = new ValidationStateDictionary(),
            ActionContext = new ActionContext { HttpContext = httpContext },
            ValueProvider = new QueryStringValueProvider(BindingSource.Query,
                                                         httpContext.Request.Query,
                                                         CultureInfo.InvariantCulture),
            BindingSource = BindingSource.Query,
            FieldName = QueryKey,

        };
        return bindingContext;
    }
}
