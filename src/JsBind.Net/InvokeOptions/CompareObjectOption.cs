using System.Text.Json.Serialization;

namespace JsBind.Net.InvokeOptions
{
    /// <summary>
    /// Invoke option to compare objects. In sync with Modules\InvokeOptions\CompareObjectOptions.js
    /// </summary>
    internal class CompareObjectOption : InvokeOptionWithReturnValue
    {
        /// <summary>
        /// Fully qualified function name for comparing object.
        /// </summary>
        public const string Identifier = "JsBindNet.CompareObject";

        [JsonPropertyName("object1AccessPath")]
        public string? Object1AccessPath { get; }

        [JsonPropertyName("object2AccessPath")]
        public string? Object2AccessPath { get; }

        public CompareObjectOption(string? object1AccessPath, string? object2AccessPath)
        {
            Object1AccessPath = object1AccessPath;
            Object2AccessPath = object2AccessPath;
        }
    }
}
