using System.Text.Json.Serialization;
using JsBind.Net;

namespace TestBindings.Server
{
    [BindDeclaredProperties]
    public class Location : ObjectBindingBase
    {
        [JsonPropertyName("href")]
        public string Href { get; set; }
    }
}
