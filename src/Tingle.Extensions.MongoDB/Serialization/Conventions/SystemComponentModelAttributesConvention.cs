using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.Conventions;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;

namespace Tingle.Extensions.MongoDB.Serialization.Conventions;

/// <summary>
/// A convention that maps attributes in the <c>System.ComponentModel.DataAnnotations</c>
/// to equivalents in the <c>MongoDB.Bson.Serialization.Attributes</c>.
/// For example:
/// <list type="bullet">
/// <item><see cref="KeyAttribute"/> instead of <see cref="BsonIdAttribute"/></item>
/// <item><see cref="NotMappedAttribute"/> instead of <see cref="BsonIgnoreAttribute"/></item>
/// </list>
/// </summary>
public class SystemComponentModelAttributesConvention : ConventionBase, IClassMapConvention
{
    /// <inheritdoc/>
    public void Apply(BsonClassMap classMap)
    {
        // Handle KeyAttribute
        if (classMap.IdMemberMap is null)
        {
            var flags = BindingFlags.Public | BindingFlags.Instance | BindingFlags.GetProperty | BindingFlags.SetProperty;
#pragma warning disable IL2075 // 'this' argument does not satisfy 'DynamicallyAccessedMembersAttribute' in call to target method. The return value of the source method does not have matching annotations.
            var properties = classMap.ClassType.GetProperties(flags);
#pragma warning restore IL2075 // 'this' argument does not satisfy 'DynamicallyAccessedMembersAttribute' in call to target method. The return value of the source method does not have matching annotations.
            var props = properties.Where(p => p.GetCustomAttribute<KeyAttribute>() != null).ToList();
            if (props.Count > 1)
            {
                throw new InvalidOperationException("Cannot pull key from more than one property");
            }

            var target = props.SingleOrDefault();
            if (target is not null)
            {
                classMap.MapIdProperty(target.Name);
            }
        }

        // Handle NotMappedAttribute
        foreach (var memberMap in classMap.DeclaredMemberMaps.ToList())
        {
            var attr = memberMap.MemberInfo.GetCustomAttributes<NotMappedAttribute>().FirstOrDefault();
            if (attr is not null)
            {
                classMap.UnmapMember(memberMap.MemberInfo);
            }
        }
    }
}
