using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.Logging;

namespace Tingle.AspNetCore.JsonPatch;

internal class SystemTextJsonPatchMergeInputFormatter : SystemTextJsonInputFormatter, IInputFormatterExceptionPolicy
{
    /// <summary>
    /// Initializes a new instance of <see cref="SystemTextJsonPatchMergeInputFormatter"/>.
    /// </summary>
    /// <param name="options">The <see cref="JsonOptions"/>.</param>
    /// <param name="logger">The <see cref="ILogger"/>.</param>
    public SystemTextJsonPatchMergeInputFormatter(JsonOptions options, ILogger<SystemTextJsonPatchMergeInputFormatter> logger) : base(options, logger)
    {
        // Clear all values and only include merge-patch+json value.
        SupportedMediaTypes.Clear();

        SupportedMediaTypes.Add(MediaTypeHeaderValues.ApplicationJsonPatchMerge);
    }

    /// <inheritdoc />
    public virtual InputFormatterExceptionPolicy ExceptionPolicy
    {
        get
        {
            if (GetType() == typeof(SystemTextJsonPatchMergeInputFormatter))
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
            if (result.Model is IJsonPatchMergeDocument jsonPatchMergeDocument && SerializerOptions is not null)
            {
                jsonPatchMergeDocument.SerializerOptions = SerializerOptions;
            }
        }

        return result;
    }

    /// <inheritdoc />
    public override bool CanRead(InputFormatterContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        var modelType = context.ModelType;
        if (!typeof(IJsonPatchMergeDocument).IsAssignableFrom(modelType) ||
            !modelType.IsGenericType)
        {
            return false;
        }

        return base.CanRead(context);
    }
}
