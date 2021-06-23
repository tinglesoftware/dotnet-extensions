using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Tingle.Extensions.DataAnnotations.Tests.Models
{
    public sealed class SaveValidationContextAttribute : ValidationAttribute
    {
        public static IList<ValidationContext> SavedContexts = new List<ValidationContext>();

        /// <inheritdoc/>
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            SavedContexts.Add(validationContext);
            return ValidationResult.Success;
        }
    }
}
