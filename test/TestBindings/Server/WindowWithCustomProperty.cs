using System.Text.Json.Serialization;
using JsBind.Net;

namespace TestBindings.Server;

[BindDeclaredProperties]
public class WindowWithCustomProperty : Window
{
    [JsonPropertyName("customProperty")]
    public string CustomProperty { get; set; }
}
