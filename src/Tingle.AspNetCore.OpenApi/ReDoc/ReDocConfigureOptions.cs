using Microsoft.Extensions.Options;
using Tingle.AspNetCore.OpenApi.ReDoc;

namespace Microsoft.Extensions.DependencyInjection;

internal class ReDocConfigureOptions : IValidateOptions<ReDocOptions>
{
    public ValidateOptionsResult Validate(string? name, ReDocOptions options)
    {
        if (string.IsNullOrWhiteSpace(options.SpecUrlTemplate))
        {
            return ValidateOptionsResult.Fail($"'{nameof(options.SpecUrlTemplate)}' must be provided");
        }

        if (string.IsNullOrWhiteSpace(options.ScriptUrl))
        {
            return ValidateOptionsResult.Fail($"'{nameof(options.ScriptUrl)}' must be provided");
        }

        if (!options.SpecUrlTemplate.Contains("{documentName}"))
        {
            return ValidateOptionsResult.Fail($"'{nameof(options.SpecUrlTemplate)}'must contain {{documentName}}");
        }

        return ValidateOptionsResult.Success;
    }
}