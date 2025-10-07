using System.Text.Json.Serialization;
using JsBind.Net;

namespace TestBindings.WebAssembly;

[BindDeclaredProperties]
public class ElementAttributeMap : ObjectBindingBase
{
    // Property that is loaded when initialized from JSON deserializer
    [JsonPropertyName("id")]
    public ElementAttributeNode Id { get; set; }
}
