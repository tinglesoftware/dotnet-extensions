using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using Tingle.AspNetCore.Tokens.Binders;

namespace Tingle.AspNetCore.Tokens.Tests;

public class TokensModelBinderProviderTests
{
    [Fact]
    public void GetBinder_Returns_Null()
    {
        var provider = new ContinuationTokenModelBinderProvider();

        // it must be generic
        var context = GetBinderProviderContext<string>();
        var binder = provider.GetBinder(context);
        Assert.Null(binder);

        // the generic type must be ContinuationToken<>
        context = GetBinderProviderContext(typeof(List<>));
        binder = provider.GetBinder(context);
        Assert.Null(binder);

        // there must be an underlying type ContinuationToken<>
        context = GetBinderProviderContext(typeof(ContinuationToken<>));
        binder = provider.GetBinder(context);
        Assert.Null(binder);

        // there must be an underlying type TimedContinuationToken<>
        context = GetBinderProviderContext(typeof(TimedContinuationToken<>));
        binder = provider.GetBinder(context);
        Assert.Null(binder);
    }

    [Fact]
    public void GetBinder_Returns_NonNull()
    {
        // test for ContinuationToken<string>
        var provider = new ContinuationTokenModelBinderProvider();
        var context = GetBinderProviderContext<ContinuationToken<string>>();
        var binder = provider.GetBinder(context);
        _ = Assert.IsAssignableFrom<BinderTypeModelBinder>(binder);

        // test for TimedContinuationToken<string>
        context = GetBinderProviderContext<TimedContinuationToken<string>>();
        binder = provider.GetBinder(context);
        _ = Assert.IsAssignableFrom<BinderTypeModelBinder>(binder);

        // test for TimedContinuationToken<TestDataClass>
        context = GetBinderProviderContext<TimedContinuationToken<TestDataClass>>();
        binder = provider.GetBinder(context);
        _ = Assert.IsAssignableFrom<BinderTypeModelBinder>(binder);
    }

    private static ModelBinderProviderContext GetBinderProviderContext<TModel>() => GetBinderProviderContext(typeof(TModel));
    private static ModelBinderProviderContext GetBinderProviderContext(Type modelType)
    {
        var metadata = new EmptyModelMetadataProvider().GetMetadataForType(modelType);
        return new DummyModelBinderProviderContext(metadata);
    }

    private class DummyModelBinderProviderContext : ModelBinderProviderContext
    {
        public DummyModelBinderProviderContext(ModelMetadata metadata)
        {
            Metadata = metadata;
        }

        public override BindingInfo BindingInfo => throw new NotImplementedException();

        public override ModelMetadata Metadata { get; }

        public override IModelMetadataProvider MetadataProvider => throw new NotImplementedException();

        public override IModelBinder CreateBinder(ModelMetadata metadata) => throw new NotImplementedException();
    }
}
