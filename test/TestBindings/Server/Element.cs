using System.Text.Json.Serialization;
using System.Threading.Tasks;
using JsBind.Net;

namespace TestBindings.Server;

[BindDeclaredProperties]
public class Element : ObjectBindingBase
{
    // Properties that are loaded when initialized from JSON deserializer
    [JsonPropertyName("id")]
    public string Id { get; set; }

    [JsonPropertyName("attributes")]
    public ElementAttributeMap Attributes { get; set; }

    // Property that is loaded everytime it is called (needs to be changed to async method)
    public ValueTask<string> GetTagName() => GetPropertyAsync<string>("tagName");

    // Invoke function on this object (needs to be changed to async method)
    public ValueTask<string> GetAttribute(string attributeName) => InvokeAsync<string>("getAttribute", attributeName);
}
