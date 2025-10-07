using System.Text.Json.Serialization;
using JsBind.Net;

namespace TestBindings.WebAssembly;

[BindDeclaredProperties]
public class Location : ObjectBindingBase
{
    [JsonPropertyName("href")]
    public string Href { get; set; }
}
