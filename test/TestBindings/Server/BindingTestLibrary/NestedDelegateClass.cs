using System;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace TestBindings.Server.BindingTestLibrary
{
    public class NestedDelegateClass
    {
        [JsonPropertyName("action")]
        public Func<Task> NestedAction { get; set; }
    }
}
