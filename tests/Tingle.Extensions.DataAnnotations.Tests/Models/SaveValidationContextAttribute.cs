using System.ComponentModel.DataAnnotations;

namespace Tingle.Extensions.DataAnnotations.Tests.Models;

public sealed class SaveValidationContextAttribute : ValidationAttribute
{
    public static readonly IList<ValidationContext> SavedContexts = [];

    /// <inheritdoc/>
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        SavedContexts.Add(validationContext);
        return ValidationResult.Success;
    }
}
