using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.Logging;

namespace Tingle.AspNetCore.JsonPatch;

internal class SystemTextJsonPatchInputFormatter : SystemTextJsonInputFormatter, IInputFormatterExceptionPolicy
{
    /// <summary>
    /// Initializes a new instance of <see cref="SystemTextJsonPatchInputFormatter"/>.
    /// </summary>
    /// <param name="options">The <see cref="JsonOptions"/>.</param>
    /// <param name="logger">The <see cref="ILogger"/>.</param>
    public SystemTextJsonPatchInputFormatter(JsonOptions options, ILogger<SystemTextJsonPatchInputFormatter> logger) : base(options, logger)
    {
        // Clear all values and only include json-patch+json value.
        SupportedMediaTypes.Clear();

        SupportedMediaTypes.Add(MediaTypeHeaderValues.ApplicationJsonPatch);
    }

    /// <inheritdoc />
    public virtual InputFormatterExceptionPolicy ExceptionPolicy
    {
        get
        {
            if (GetType() == typeof(SystemTextJsonPatchInputFormatter))
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
            if (result.Model is IJsonPatchDocument jsonPatchDocument && SerializerOptions is not null)
            {
                jsonPatchDocument.SerializerOptions = SerializerOptions;
            }
        }

        return result;
    }

    /// <inheritdoc />
    public override bool CanRead(InputFormatterContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        var modelType = context.ModelType;
        if (!typeof(IJsonPatchDocument).IsAssignableFrom(modelType) ||
            !modelType.IsGenericType)
        {
            return false;
        }

        return base.CanRead(context);
    }
}
