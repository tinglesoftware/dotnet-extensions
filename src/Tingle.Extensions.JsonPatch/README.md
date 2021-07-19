# Tingle.Extensions.JsonPatch

The primary goal of this library is to provide functionalities to perform [JsonPatch](https://tools.ietf.org/html/rfc6902) operations on documents using `System.Text.Json` library. We'll show how to do this in some examples below.

Let us first define a class representing a customer with orders.

```cs
class Customer
{
    public string Name { get; set; }
    public List<Order> Orders { get; set; }
    public string AlternateName { get; set; }
}

class Order
{
    public string Item { get; set; }
    public int Quantity { get; set;}
}
```

An instantiated `Customer` object would then look like this:

```cs
{
    "name": "John",
    "alternateName": null,
    "orders":
    [
        {
            "item": "Toy car",
            "quantity": 1
        },
        {
            "item": "C# In A Nuthshell Book",
            "quantity": 1
        }
    ]
}
```

A JSON Patch document has an array of operations. Each operation identifies a particular type of change. Examples of such changes include adding an array element or replacing a property value.

Let us create `JsonPatchDocument<Customer>` instance to demonstrate the various patching functionalities we provide.

```cs
var patchDoc= new JsonPatchDocument<Customer>();
// Define operations here...
```

By default, the case transform type is `LowerCase` by default. Other options available are `UpperCase`, `CamelCase` and `OriginalCase`. These can be set via the constructor of the `JsonPatchDocument<T>`. For our example purposes we'll go with the default casing. Now let us see the supported patch operations.

## Add Operation

Let us set the `Name` of the customer and add an object to the end of the `orders` array.

```cs
var order = new Order
{
    Item = "Car tracker",
    Quantity = 10
};

var patchDoc= new JsonPatchDocument<Customer>();
patchDoc.Add(x => x.Name, "Ben")
        .Add(y => y.Orders, order);
```

## Remove Operation

Let us set the `Name` to null and delete `orders[0]`

```cs
var patchDoc= new JsonPatchDocument<Customer>();
patchDoc.Remove(x => x.Name, null)
        .Remove(y => y.Orders, 0);
```

## Replace Operation

This is the same as a `Remove` operation followed by an `Add`. Let us show how to do this below:

```cs
var order = new Order
{
    Item = "Air Fryer",
    Quantity = 1
};

var patchDoc= new JsonPatchDocument<Customer>();
patchDoc.Replace(x => x.Name, null)
        .Replace(y => y.Orders, order, 0);
```

The `Replace` operation can also be used to replace items in a dictionary by the given key.

## Move Operation

Let us Move `orders[1]` to before `orders[0]` and set `AlternateName` from the `Name` value.

```cs
var patchDoc= new JsonPatchDocument<Customer>();
patchDoc.Move(x => x.Orders, 0, y => y.Orders, 1) // swap the orders
        .Move(x => x.Name, y => y.AlternateName); // set AlternateName to Name while leaving Name as null
```

## Copy Operation

This operation is fundamentally the same as `Move` without the final `Remove` step.

Let us in the example below copy the value of `Name` to the `AlternateName` and insert a copy of `orders[1]` before `orders[0]`.

```cs
var patchDoc= new JsonPatchDocument<Customer>();
patchDoc.Copy(x => x.Orders, 1, y => y.Orders, 0)
        .Copy(x => x.Name, y => y.AlternateName);
```

## Test Operation

This operation is is commonly used to prevent an update when there's a concurrency conflict.

The following sample patch document has no effect if the initial value of `Name` is "John", because the test fails:

```cs
var patchDoc= new JsonPatchDocument<Customer>();
patchDoc.Test(x => x.Name, "Andrew")
        .Add(x => x.Name, "Ben");
```

The instantiated patch document can then be serialized or deserialized using the `JsonSerializer` in the `System.Text.Json` library.
