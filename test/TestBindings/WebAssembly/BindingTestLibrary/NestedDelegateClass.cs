using System;
using System.Text.Json.Serialization;

namespace TestBindings.WebAssembly.BindingTestLibrary;

public class NestedDelegateClass
{
    [JsonPropertyName("action")]
    public Action NestedAction { get; set; }
}
