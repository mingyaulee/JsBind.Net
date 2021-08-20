using System;
using System.Text.Json.Serialization;

namespace JsBind.Net.Internal.References
{
    /// <summary>
    /// Delegate reference object from JavaScript.
    /// </summary>
    internal class JsDelegateReference
    {
        [JsonPropertyName("delegateId")]
        public Guid? DelegateId { get; set; }

        [JsonPropertyName("accessPath")]
        public string? AccessPath { get; set; }
    }
}
