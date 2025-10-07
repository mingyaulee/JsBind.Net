using System.Text.Json.Serialization;

namespace JsBind.Net.InvokeOptions
{
    /// <summary>
    /// Invoke option to convert object type.
    /// </summary>
    public interface IConvertObjectTypeOption
    {
        /// <summary>
        /// The access path to the target object.
        /// </summary>
        string? AccessPath { get; set; }
    }

    /// <summary>
    /// Invoke option to convert object type. In sync with Modules\InvokeOptions\ConvertObjectTypeOption.js
    /// </summary>
    internal class ConvertObjectTypeOption(string? accessPath) : InvokeOptionWithReturnValue, IConvertObjectTypeOption
    {
        /// <summary>
        /// Fully qualified function name for converting object type.
        /// </summary>
        public const string Identifier = "JsBindNet.ConvertObjectType";

        /// <inheritdoc />
        [JsonPropertyName("accessPath")]
        public string? AccessPath { get; set; } = accessPath;
    }
}
