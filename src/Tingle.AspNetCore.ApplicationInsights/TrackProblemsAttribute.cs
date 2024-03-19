using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.AspNetCore.Mvc.Filters;
using SC = Tingle.AspNetCore.ApplicationInsights.InsightsJsonSerializerContext;

namespace Microsoft.AspNetCore.Mvc;

/// <summary>
/// Track the problem details in application insights when the response is a <see cref="BadRequestObjectResult"/>
/// with value of type <see cref="ProblemDetails"/>.
/// The properties are seen as custom properties of an application insights telemetry record
/// </summary>
/// <param name="includeErrors">Whether to include errors from <see cref="ValidationProblemDetails.Errors"/>.</param>
public sealed class TrackProblemsAttribute(bool includeErrors = true) : ActionFilterAttribute
{
    /// <summary>
    /// Sets the custom properties in <see cref="RequestTelemetry"/>
    /// </summary>
    /// <param name="context"></param>
    public override void OnResultExecuting(ResultExecutingContext context)
    {
        if (context.Result is ObjectResult or)
        {
            if (or.Value is ProblemDetails pd)
            {
                var telemetry = context.HttpContext.Features.Get<RequestTelemetry>();
                if (telemetry != null)
                {
                    telemetry.Properties["problem.type"] = pd.Type;
                    telemetry.Properties["problem.title"] = pd.Title;
                    telemetry.Properties["problem.status"] = pd.Status.ToString();
                    telemetry.Properties["problem.detail"] = pd.Detail;
                    telemetry.Properties["problem.instance"] = pd.Instance;

                    // collect validation errors for validation problems type if allowed and available
                    if (includeErrors && pd is ValidationProblemDetails vpd)
                    {
                        telemetry.Properties["problem.errors"] = System.Text.Json.JsonSerializer.Serialize(vpd.Errors, SC.Default.IDictionaryStringStringArray);
                    }
                }
            }
        }
    }
}
