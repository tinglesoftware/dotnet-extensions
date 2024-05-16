using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Reflection;
using System.Xml.XPath;

namespace Tingle.AspNetCore.Swagger.Filters.Schemas;

/// <summary>
/// Adds documentation that is provided by the &lt;inhertidoc /&gt; tag.
/// </summary>
/// <seealso cref="ISchemaFilter" />
public class InheritDocSchemaFilter : ISchemaFilter
{
    private const string SummaryTag = "summary";
    private const string ExampleTag = "example";

    private readonly List<XPathDocument> documents;
    private readonly Dictionary<string, string> inheritedDocs;
    private readonly Type[] excludedTypes;

    /// <summary>
    /// Initializes a new instance of the <see cref="InheritDocSchemaFilter" /> class.
    /// </summary>
    /// <param name="options"><see cref="SwaggerGenOptions"/>.</param>
    /// <param name="excludedTypes">Excluded types.</param>
    public InheritDocSchemaFilter(SwaggerGenOptions options, params Type[] excludedTypes)
    {
        this.excludedTypes = excludedTypes;

        // find the XPathDocument arguments from all XmlCommentSchemaFilters
        documents = options.SchemaFilterDescriptors.Where(x => x.Type == typeof(XmlCommentsSchemaFilter))
                                                   .Select(x => x.Arguments.Single())
                                                   .Cast<XPathDocument>()
                                                   .ToList();

        inheritedDocs = documents.SelectMany(doc =>
        {
            var inheritedElements = new List<(string, string)>();
            foreach (XPathNavigator member in doc.CreateNavigator().Select("doc/members/member/inheritdoc"))
            {
                var cref = member.GetAttribute("cref", "");
                member.MoveToParent();
                var parentCref = member.GetAttribute("cref", "");
                if (!string.IsNullOrWhiteSpace(parentCref))
                    cref = parentCref;
                inheritedElements.Add((member.GetAttribute("name", ""), cref));
            }

            return inheritedElements;
        }).ToDictionary(x => x.Item1, x => x.Item2);
    }

    /// <summary>
    /// Apply filter.
    /// </summary>
    /// <param name="schema"><see cref="OpenApiSchema"/>.</param>
    /// <param name="context"><see cref="SchemaFilterContext"/>.</param>
    public void Apply(OpenApiSchema schema, SchemaFilterContext context)
    {
        if (excludedTypes.Length != 0 && excludedTypes.Contains(context.Type)) return;

        // Try to apply a description for inherited types.
        var memberName = XmlCommentsNodeNameHelper.GetMemberNameForType(context.Type);
        if (string.IsNullOrEmpty(schema.Description) && inheritedDocs.TryGetValue(memberName, out var cref))
        {
            var target = GetTargetRecursive(context.Type, cref);

            var targetXmlNode = GetMemberXmlNode(XmlCommentsNodeNameHelper.GetMemberNameForType(target));
            var summaryNode = targetXmlNode?.SelectSingleNode(SummaryTag);

            if (summaryNode != null)
            {
                schema.Description = XmlCommentsTextHelper.Humanize(summaryNode.InnerXml);
            }
        }

        // Handle parameters such as in form data
        if (context.ParameterInfo != null && context.MemberInfo != null)
        {
            ApplyPropertyComments(schema, context.MemberInfo);
        }

        if (schema.Properties == null) return;

        // Add the summary and examples for the properties.
        foreach (var entry in schema.Properties)
        {
            var memberInfo = ((TypeInfo)context.Type).GetAllMembers()?.FirstOrDefault(p => p.Name.Equals(entry.Key, StringComparison.OrdinalIgnoreCase));
            if (memberInfo != null)
            {
                ApplyPropertyComments(entry.Value, memberInfo);
            }
        }
    }

    private void ApplyPropertyComments(OpenApiSchema propertySchema, MemberInfo memberInfo)
    {
        var memberName = XmlCommentsNodeNameHelper.GetMemberNameForFieldOrProperty(memberInfo);

        if (excludedTypes.Length != 0 && excludedTypes.Contains(((PropertyInfo)memberInfo).PropertyType)) return;
        if (!inheritedDocs.TryGetValue(memberName, out string? cref)) return;

        var target = GetTargetRecursive(memberInfo, cref);

        var targetXmlNode = GetMemberXmlNode(XmlCommentsNodeNameHelper.GetMemberNameForFieldOrProperty(target));
        if (targetXmlNode == null) return;

        var summaryNode = targetXmlNode.SelectSingleNode(SummaryTag);
        if (string.IsNullOrEmpty(propertySchema.Description) && summaryNode != null)
        {
            propertySchema.Description = XmlCommentsTextHelper.Humanize(summaryNode.InnerXml);
        }

        var exampleNode = targetXmlNode.SelectSingleNode(ExampleTag);
        if (propertySchema.Example is null && exampleNode != null)
        {
            propertySchema.Example = new OpenApiString(XmlCommentsTextHelper.Humanize(exampleNode.InnerXml));
        }
    }

    private XPathNavigator? GetMemberXmlNode(string memberName)
    {
        var path = $"/doc/members/member[@name='{memberName}']";

        foreach (var document in documents)
        {
            var node = document.CreateNavigator().SelectSingleNode(path);
            if (node != null) return node;
        }

        return null;
    }

    private static MemberInfo? GetTarget(MemberInfo memberInfo, string cref)
    {
        var type = memberInfo.DeclaringType ?? memberInfo.ReflectedType;
        if (type == null) return null;

        // Find all matching members in all interfaces and the base class.
        var targets = type.GetInterfaces()
            .Append(type.BaseType)
            .SelectMany(
                x => x?.FindMembers(
                    memberInfo.MemberType,
                    BindingFlags.Instance | BindingFlags.Public,
                    (info, criteria) => info.Name == memberInfo.Name,
                    null) ?? [])
            .ToList();

        // Try to find the target, if one is declared.
        if (!string.IsNullOrEmpty(cref))
        {
            var crefTarget = targets.SingleOrDefault(t => XmlCommentsNodeNameHelper.GetMemberNameForFieldOrProperty(t) == cref);
            if (crefTarget != null) return crefTarget;
        }

        // We use the last since that will be our base class or the "nearest" implemented interface.
        return targets.LastOrDefault();
    }

    private MemberInfo? GetTargetRecursive(MemberInfo memberInfo, string cref)
    {
        var target = GetTarget(memberInfo, cref);
        if (target == null) return null;

        var targetMemberName = XmlCommentsNodeNameHelper.GetMemberNameForFieldOrProperty(target);

        return inheritedDocs.TryGetValue(targetMemberName, out var value) ? GetTarget(target, value) : target;
    }

    private Type? GetTargetRecursive(Type type, string cref)
    {
        var target = GetTarget(type, cref);
        if (target == null) return null;

        var targetMemberName = XmlCommentsNodeNameHelper.GetMemberNameForType(target);

        return inheritedDocs.TryGetValue(targetMemberName, out var value) ? GetTarget(target, value) : target;
    }

    private static Type? GetTarget(Type type, string cref)
    {
        var targets = type.GetInterfaces();
        if (type.BaseType is not null && type.BaseType != typeof(object))
            targets = [.. targets, type.BaseType];

        // Try to find the target, if one is declared.
        if (!string.IsNullOrEmpty(cref))
        {
            var crefTarget = targets.SingleOrDefault(t => XmlCommentsNodeNameHelper.GetMemberNameForType(t) == cref);
            if (crefTarget != null) return crefTarget;
        }

        // We use the last since that will be our base class or the "nearest" implemented interface.
        return targets.LastOrDefault();
    }
}
