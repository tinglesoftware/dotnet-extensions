using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Reflection;
using System.Runtime.Serialization;
using System.Xml.XPath;

namespace Tingle.AspNetCore.Swagger.Filters.Schemas;

/// <summary>
/// An implementation of <see cref="ISchemaFilter"/> that adds descriptions for enums.
/// </summary>
/// <param name="xmlDoc"></param>
public class EnumDescriptionsSchemaFilter(XPathDocument xmlDoc) : ISchemaFilter
{
    internal const string ExtensionName = "x-enumDescriptions";

    private readonly XPathNavigator _xmlNavigator = xmlDoc.CreateNavigator();

    /// <inheritdoc/>
    public void Apply(OpenApiSchema schema, SchemaFilterContext context)
    {
        if (context.Type is null || !context.Type.IsEnum) return;
        if (schema.Enum is null || schema.Enum.Count == 0) return;

        var extension = schema.Extensions.TryGetValue(ExtensionName, out var ext) ? (OpenApiObject)ext : [];

        var enumType = context.Type;
        var enumValues = Enum.GetValues(enumType);
        foreach (var enumValue in enumValues)
        {
            var memberInfo = enumType.GetMembers().Single(m => m.Name.Equals(enumValue.ToString()));
            var description = TryGetXmlComments(memberInfo, _xmlNavigator);
            if (description is not null)
            {
                // find the enum from this in the schema
                var possibleValues = MakePossibleValues(memberInfo).ToList();
                var found = schema.Enum.OfType<OpenApiString>().SingleOrDefault(v => possibleValues.Contains(v.Value, StringComparer.OrdinalIgnoreCase))?.Value;

                // add description to the extension if the enum is exposed
                if (found is not null)
                {
                    extension[found] = new OpenApiString(description);
                }
            }
        }

        // Add the extension if there descriptions
        if (extension.Count > 0)
        {
            schema.Extensions[ExtensionName] = extension;
        }
    }

    private static string? TryGetXmlComments(MemberInfo memberInfo, XPathNavigator _xmlNavigator)
    {
        var enumMemberName = XmlCommentsNodeNameHelper.GetMemberNameForFieldOrProperty(memberInfo);
        var enumNode = _xmlNavigator.SelectSingleNode($"/doc/members/member[@name='{enumMemberName}']");

        if (enumNode is null) return null;

        var summaryNode = enumNode.SelectSingleNode("summary");
        if (summaryNode != null)
        {
            return XmlCommentsTextHelper.Humanize(summaryNode.InnerXml);
        }

        return null;
    }

    private static IEnumerable<string> MakePossibleValues(MemberInfo memberInfo)
    {
        ArgumentNullException.ThrowIfNull(memberInfo);

        var attr = memberInfo.GetCustomAttribute<EnumMemberAttribute>(inherit: false);

        yield return memberInfo.Name;
        if (attr?.Value is not null) yield return attr.Value;
    }
}
