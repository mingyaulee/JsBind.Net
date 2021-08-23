﻿using System.Text.Json.Serialization;
using JsBind.Net;

namespace TestBindings.WebAssembly.BindingTestLibrary
{
    public class TestBoundClass : ObjectBindingBase<TestBoundClass>
    {
        [JsonPropertyName("isTestClass")]
        public bool IsTestClass { get; set; }
        public double RandomValue { get; set; }
    }
}
