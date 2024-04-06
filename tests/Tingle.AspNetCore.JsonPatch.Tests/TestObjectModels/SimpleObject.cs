﻿namespace Tingle.AspNetCore.JsonPatch;

public class SimpleObject
{
    public List<int>? IntegerList { get; set; }
    public IList<int>? IntegerIList { get; set; }
    public int IntegerValue { get; set; }
    public int AnotherIntegerValue { get; set; }
    public string? StringProperty { get; set; }
    public string? AnotherStringProperty { get; set; }
    public decimal DecimalValue { get; set; }
    public double DoubleValue { get; set; }
    public float FloatValue { get; set; }
    public Guid GuidValue { get; set; }
}
