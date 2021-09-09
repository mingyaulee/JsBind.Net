using System.Text.Json.Serialization;
using JsBind.Net;

namespace TestBindings.WebAssembly
{
    [BindDeclaredProperties]
    public class Element : ObjectBindingBase
    {
        // Properties that are loaded when initialized from JSON deserializer
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("attributes")]
        public ElementAttributeMap Attributes { get; set; }

        // Property that is loaded everytime it is called
        public string TagName => GetProperty<string>("tagName");

        // Invoke function on this object
        public string GetAttribute(string attributeName) => Invoke<string>("getAttribute", attributeName);
    }
}
