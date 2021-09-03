using System.Text.Json.Serialization;
using JsBind.Net;

namespace TestBindings.WebAssembly
{
    [BindDeclaredProperties]
    public class WindowWithCustomProperty : Window
    {
        [JsonPropertyName("customProperty")]
        public string CustomProperty { get; set; }
    }
}
