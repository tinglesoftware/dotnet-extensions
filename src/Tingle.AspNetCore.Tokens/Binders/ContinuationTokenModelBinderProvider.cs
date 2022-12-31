using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;

namespace Tingle.AspNetCore.Tokens.Binders;

/// <summary>
/// A <see cref="IModelBinderProvider"/> that produces binders for instances of:
/// <list type="number">
/// <item><see cref="ContinuationToken{T}"/></item>
/// <item><see cref="TimedContinuationToken{T}"/></item>
/// </list>
/// if applicable.
/// </summary>
internal class ContinuationTokenModelBinderProvider : IModelBinderProvider
{
    /// <inheritdoc/>
    public IModelBinder? GetBinder(ModelBinderProviderContext context)
    {
        if (context == null) throw new ArgumentNullException(nameof(context));

        // ensure the model type is generic
        var modelType = context.Metadata.ModelType;
        if (modelType.IsGenericType)
        {
            // get the definition of the generic type (plain format, no arguments)
            var genericTypeDefinition = modelType.GetGenericTypeDefinition();

            // try make a type for the model binder
            Type? modelBinderType = null;

            // ensure the generic type matches what we expect
            var underlyingType = modelType.GenericTypeArguments.FirstOrDefault();
            if ((genericTypeDefinition == typeof(ContinuationToken<>) || genericTypeDefinition == typeof(TimedContinuationToken<>))
                && underlyingType != null)
            {
                // get the underlying type and make the binder type
                modelBinderType = typeof(ContinuationTokenModelBinder<>).MakeGenericType(underlyingType);
            }

            // if we got a model binder type, then return it
            if (modelBinderType != null)
                return new BinderTypeModelBinder(modelBinderType);
        }

        // we did not find anything
        return null;
    }
}
