using System.Text.Json.Serialization;

namespace JsBind.Net.Internal.References
{
    /// <summary>
    /// Base refence class to be serialized and passed to JavaScript.
    /// </summary>
    internal abstract class ReferenceBase
    {
        [JsonPropertyName("__isJsBindReference")]
        public bool IsReference { get; } = true;

        [JsonPropertyName("__referenceType")]
        public abstract ReferenceType ReferenceType { get; }
    }
}
