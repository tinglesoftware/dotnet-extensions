#if NET8_0_OR_GREATER
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Microsoft.EntityFrameworkCore.Metadata.Conventions.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;

namespace Tingle.Extensions.EntityFrameworkCore.Conventions;

/// <summary>
/// A convention that configures the maximum length based on the <see cref="LengthAttribute" /> applied on the property.
/// </summary>
/// <remarks>
/// See <see href="https://aka.ms/efcore-docs-conventions">Model building conventions</see> for more information and examples.
/// </remarks>
/// <param name="dependencies">Parameter object containing dependencies for this convention.</param>
public class LengthAttributeConvention(ProviderConventionSetBuilderDependencies dependencies) : PropertyAttributeConventionBase<LengthAttribute>(dependencies), IComplexPropertyAddedConvention
{
    /// <inheritdoc />
    protected override void ProcessPropertyAdded(
        IConventionPropertyBuilder propertyBuilder,
        LengthAttribute attribute,
        MemberInfo clrMember,
        IConventionContext context)
    {
        if (attribute.MaximumLength > 0)
        {
            propertyBuilder.HasMaxLength(attribute.MaximumLength, fromDataAnnotation: true);
        }
    }

    /// <inheritdoc />
    protected override void ProcessPropertyAdded(
        IConventionComplexPropertyBuilder propertyBuilder,
        LengthAttribute attribute,
        MemberInfo clrMember,
        IConventionContext context)
    {
        var property = propertyBuilder.Metadata;
#pragma warning disable EF1001
        var member = property.GetIdentifyingMemberInfo();
#pragma warning restore EF1001
        if (member != null
            && Attribute.IsDefined(member, typeof(ForeignKeyAttribute), inherit: true))
        {
            throw new InvalidOperationException(
                CoreStrings.AttributeNotOnEntityTypeProperty(
                    "MaxLength", property.DeclaringType.DisplayName(), property.Name));
        }
    }
}
#endif
