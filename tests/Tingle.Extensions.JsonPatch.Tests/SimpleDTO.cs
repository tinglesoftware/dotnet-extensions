﻿using System;
using System.Collections.Generic;

namespace Tingle.Extensions.JsonPatch.Tests
{
    public class SimpleDTO
    {
        public List<int>? IntegerList { get; set; }
        public IList<int>? IntegerGenericList { get; set; }
        public int IntegerValue { get; set; }
        public string? StringProperty { get; set; }
        public string? AnotherStringProperty { get; set; }
        public decimal DecimalValue { get; set; }

        public double DoubleValue { get; set; }

        public float FloatValue { get; set; }

        public Guid GuidValue { get; set; }
    }
}
