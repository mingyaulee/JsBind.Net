using System;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using JsBind.Net;

namespace TestBindings.Server.BindingTestLibrary
{
    public class NestedDelegateClass : ObjectBindingBase
    {
        [JsonPropertyName("action")]
        public Func<Task> NestedAction { get; set; }
    }
}
