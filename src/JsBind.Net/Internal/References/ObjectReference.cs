using System.Text.Json.Serialization;

namespace JsBind.Net.Internal.References
{
    /// <summary>
    /// Object refence class to be serialized and passed to JavaScript.
    /// </summary>
    internal class ObjectReference : ReferenceBase
    {
        public override ReferenceType ReferenceType => ReferenceType.Object;

        [JsonPropertyName("accessPath")]
        public string? AccessPath { get; }

        public ObjectReference(string? accessPath)
        {
            AccessPath = accessPath;
        }
    }
}
