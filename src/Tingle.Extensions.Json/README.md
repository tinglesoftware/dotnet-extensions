# Tingle.Extensions.Json

This library provides various serialization functionalities that aren't in the `System.Text.Json` library but are in the counterpart `Newtonsoft.Json` library.

The various functionalities will be described using examples below.

Some of the classes

## Cloning and Copying

```cs
class TestType1
{
   public int Value1 { get; set; }
   public string Value2 { get; set; }
}

class TestType2 : TestType1
{
   public DateTimeOffset? Value3 { get; set; }
}

class TestType3
{
    public int Value1 { get; set; }
    public string Value2 { get; set; }
    public DateTimeOffset? Value3 { get; set; }
}
```
### Cloning
You can use it to create an object clone via the JSON serializer. An example is shown below:

```Program.cs
static void Main(string[] args)
{
    var tt1 = new TestType1 { Value1 = 13, Value2 = "cake1" };
    var tt2 = tt1.JsonClone();
}
```

### Copying
You can use it to convert one object to another via the JSON serializer. An example is shown below:

```Program.cs
static void Main(string[] args)
{
    var tt2 = new TestType2 { Value1 = 13, Value2 = "cake1" };
    var tt3 = tt2.JsonConvertTo<TestType3>();
}
```

## JsonSerializerOptions Extensions

`JsonSerializerOptions` is used with `System.Text.Json.JsonSerializer` to perform various serialization tasks. The following are some extensions in the library.

### Version Converter

Version class represents the version number of an assembly, operating system, or the common language runtime. This class cannot be inherited. 

The example below will show how to deserialize this to/from a string.

```cs
class TestModel
{
   public Version Deployed { get; set; }
}
```

```Program.cs
static void Main(string[] args)
{
    var src_json = "{\"deployed\":\"1.13.4\"}";
    var options = new JsonSerializerOptions
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    }.AddConverterForVersion();
    var model = JsonSerializer.Deserialize<TestModel>(src_json, options);
    var dst_json = JsonSerializer.Serialize(model, options);
}
```

### Timespan Converter

A Timespan represents a time interval. The example below will show how to deserialize this to/from a string.

```cs
class TestModel
{
   public TimeSpan Duration { get; set; }
}
```

```Program.cs
static void Main(string[] args)
{
    var src_json = "{\"duration\":\"00:00:00.2880000\"}";
    var options = new JsonSerializerOptions
    {
       PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    }.AddConverterForTimeSpan();
    var model = JsonSerializer.Deserialize<TestModel>(src_json, options);
    var dst_json = JsonSerializer.Serialize(model, options);
}
```





