# Tingle.Extensions.AnyOf

This library provides generic classes that can hold a value of one of the supported types. It uses implicit conversion operators to seamlessly accept or return any of the possible types.  
This is used to represent polymorphic request parameters, i.e parameters that can be of different types, typically a string and an options class.

There are two classes in this library namely `AnyOf<T1,T2>` and `AnyOf<T1,T2,T3>`
.

## `AnyOf<T1,T2>`  
This class holds a value of one of two different types.  

### Example 1 
A connection string can either be a string or a credentials object. 

```csharp
using System;

//example goes here

```  


## `AnyOf<T1,T2,T3>`  
This generic class can hold a value of on of three different types.  

### Example 2 

Suppose the connection string in _Example 1_ supports three different types: 

```csharp
using System;

//example goes here

```
 
 

