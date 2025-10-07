using System.Text.Json.Serialization;
using JsBind.Net;

namespace TestBindings.Server;

[BindDeclaredProperties]
public class WindowWithLocation : Window
{
    [JsonPropertyName("location")]
    public Location Location { get; set; }
}
