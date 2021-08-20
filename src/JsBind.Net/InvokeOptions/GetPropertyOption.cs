using System.Text.Json.Serialization;

namespace JsBind.Net.InvokeOptions
{
    /// <summary>
    /// Invoke option to get property value. In sync with JsBindScripts\Modules\InvokeOptions\GetPropertyOption.js
    /// </summary>
    internal class GetPropertyOption : InvokeOptionWithReturnValue
    {
        /// <summary>
        /// Fully qualified function name for getting property value.
        /// </summary>
        public const string Identifier = "JsBindNet.GetProperty";

        [JsonPropertyName("accessPath")]
        public string? AccessPath { get; set; }

        [JsonPropertyName("propertyName")]
        public string? PropertyName { get; set; }

        public GetPropertyOption(string? accessPath, string? propertyName)
        {
            AccessPath = accessPath;
            PropertyName = propertyName;
        }
    }
}
