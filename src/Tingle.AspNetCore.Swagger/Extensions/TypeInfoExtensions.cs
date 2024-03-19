namespace System.Reflection;

internal static class TypeInfoExtensions
{
    public static IEnumerable<ConstructorInfo> GetAllConstructors(this TypeInfo typeInfo)
        => GetAll(typeInfo, ti => ti.DeclaredConstructors);

    public static IEnumerable<EventInfo> GetAllEvents(this TypeInfo typeInfo)
        => GetAll(typeInfo, ti => ti.DeclaredEvents);

    public static IEnumerable<FieldInfo> GetAllFields(this TypeInfo typeInfo)
        => GetAll(typeInfo, ti => ti.DeclaredFields);

    public static IEnumerable<MemberInfo> GetAllMembers(this TypeInfo typeInfo)
        => GetAll(typeInfo, ti => ti.DeclaredMembers);

    public static IEnumerable<MethodInfo> GetAllMethods(this TypeInfo typeInfo)
        => GetAll(typeInfo, ti => ti.DeclaredMethods);

    public static IEnumerable<TypeInfo> GetAllNestedTypes(this TypeInfo typeInfo)
        => GetAll(typeInfo, ti => ti.DeclaredNestedTypes);

    public static IEnumerable<PropertyInfo> GetAllProperties(this TypeInfo typeInfo)
        => GetAll(typeInfo, ti => ti.DeclaredProperties);

    private static IEnumerable<T> GetAll<T>(TypeInfo typeInfo, Func<TypeInfo, IEnumerable<T>> accessor)
    {
        var ti = typeInfo;
        while (ti is not null)
        {
            foreach (var t in accessor(ti))
            {
                yield return t;
            }

            ti = ti.BaseType?.GetTypeInfo();
        }
    }
}
