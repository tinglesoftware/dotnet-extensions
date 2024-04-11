using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.Logging;

namespace Tingle.AspNetCore.JsonPatch;

internal class SystemTextJsonMergePatchInputFormatter : SystemTextJsonInputFormatter, IInputFormatterExceptionPolicy
{
    /// <summary>
    /// Initializes a new instance of <see cref="SystemTextJsonMergePatchInputFormatter"/>.
    /// </summary>
    /// <param name="options">The <see cref="JsonOptions"/>.</param>
    /// <param name="logger">The <see cref="ILogger"/>.</param>
    public SystemTextJsonMergePatchInputFormatter(JsonOptions options, ILogger<SystemTextJsonMergePatchInputFormatter> logger) : base(options, logger)
    {
        // Clear all values and only include merge-patch+json value.
        SupportedMediaTypes.Clear();

        SupportedMediaTypes.Add(MediaTypeHeaderValues.ApplicationJsonMergePatch);
    }

    /// <inheritdoc />
    public virtual InputFormatterExceptionPolicy ExceptionPolicy
    {
        get
        {
            if (GetType() == typeof(SystemTextJsonMergePatchInputFormatter))
            {
                return InputFormatterExceptionPolicy.MalformedInputExceptions;
            }
            return InputFormatterExceptionPolicy.AllExceptions;
        }
    }

    /// <inheritdoc />
    public override async Task<InputFormatterResult> ReadRequestBodyAsync(InputFormatterContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        var result = await base.ReadRequestBodyAsync(context).ConfigureAwait(false);
        if (!result.HasError)
        {
            if (result.Model is IJsonMergePatchDocument jsonMergePatchDocument && SerializerOptions is not null)
            {
                jsonMergePatchDocument.SerializerOptions = SerializerOptions;
            }
        }

        return result;
    }

    /// <inheritdoc />
    public override bool CanRead(InputFormatterContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        var modelType = context.ModelType;
        if (!typeof(IJsonMergePatchDocument).IsAssignableFrom(modelType) ||
            !modelType.IsGenericType)
        {
            return false;
        }

        return base.CanRead(context);
    }
}
