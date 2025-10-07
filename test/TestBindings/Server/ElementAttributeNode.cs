using System.Text.Json.Serialization;
using JsBind.Net;

namespace TestBindings.Server;

[BindDeclaredProperties]
public class ElementAttributeNode : ObjectBindingBase
{
    // Property that is loaded when initialized from JSON deserializer
    [JsonPropertyName("value")]
    public string Value { get; set; }
}
