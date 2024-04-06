namespace Tingle.AspNetCore.JsonPatch.Internal;

internal class Customer(string name, int age)
{
    private readonly string _name = name;
    private readonly int _age = age;
}
