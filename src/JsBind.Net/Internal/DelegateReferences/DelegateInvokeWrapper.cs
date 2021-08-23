using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using JsBind.Net.Internal.JsonConverters;

namespace JsBind.Net.Internal.DelegateReferences
{
    /// <summary>
    /// A wrapper for deserializing delegate invocation arguments.
    /// </summary>
    [JsonConverter(typeof(DelegateInvokeConverter))]
    internal class DelegateInvokeWrapper
    {
        public IEnumerable<JsonElement>? Args { get; set; }
        public JsonSerializerOptions? JsonSerializerOptions { get; set; }
    }
}
