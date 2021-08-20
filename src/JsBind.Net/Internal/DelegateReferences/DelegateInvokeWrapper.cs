using System.Collections.Generic;
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
        public object?[]? Args { get; set; }
        public IEnumerable<BindingBase?>? References { get; set; }
    }
}
