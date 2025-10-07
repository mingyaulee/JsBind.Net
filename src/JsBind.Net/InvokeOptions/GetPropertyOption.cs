using System.Text.Json.Serialization;

namespace JsBind.Net.InvokeOptions
{
    /// <summary>
    /// Invoke option to get property.
    /// </summary>
    public interface IGetPropertyOption
    {
        /// <summary>
        /// The access path to the target object.
        /// </summary>
        string? AccessPath { get; set; }

        /// <summary>
        /// The name of the target property.
        /// </summary>
        string? PropertyName { get; set; }
    }

    /// <summary>
    /// Invoke option to get property value. In sync with Modules\InvokeOptions\GetPropertyOption.js
    /// </summary>
    internal class GetPropertyOption(string? accessPath, string? propertyName) : InvokeOptionWithReturnValue, IGetPropertyOption
    {
        /// <summary>
        /// Fully qualified function name for getting property value.
        /// </summary>
        public const string Identifier = "JsBindNet.GetProperty";

        /// <inheritdoc />
        [JsonPropertyName("accessPath")]
        public string? AccessPath { get; set; } = accessPath;

        /// <inheritdoc />
        [JsonPropertyName("propertyName")]
        public string? PropertyName { get; set; } = propertyName;
    }
}
