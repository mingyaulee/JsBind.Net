using System.Text.Json.Serialization;
using JsBind.Net.Internal.JsonConverters;

namespace JsBind.Net.InvokeOptions
{
    /// <summary>
    /// Invoke option to set property.
    /// </summary>
    public interface ISetPropertyOption
    {
        /// <summary>
        /// The access path to the target object.
        /// </summary>
        string? AccessPath { get; set; }

        /// <summary>
        /// The name of the target property.
        /// </summary>
        string? PropertyName { get; set; }

        /// <summary>
        /// The value of the target property.
        /// </summary>
        object? PropertyValue { get; set; }
    }

    /// <summary>
    /// Invoke option to set property value. In sync with Modules\InvokeOptions\SetPropertyOption.js
    /// </summary>
    internal class SetPropertyOption : InvokeOption, ISetPropertyOption
    {
        /// <summary>
        /// Fully qualified function name for setting property value.
        /// </summary>
        public const string Identifier = "JsBindNet.SetProperty";

        /// <inheritdoc />
        [JsonPropertyName("accessPath")]
        public string? AccessPath { get; set; }

        /// <inheritdoc />
        [JsonPropertyName("propertyName")]
        public string? PropertyName { get; set; }

        /// <inheritdoc />
        [JsonPropertyName("propertyValue")]
        [JsonConverter(typeof(InvokeArgumentConverter))]
        public InvokeArgument InvokeArgument { get; set; }

        /// <inheritdoc />
        [JsonIgnore]
        public object? PropertyValue { get => InvokeArgument.ArgumentValue; set => InvokeArgument = new(value, this); }

        public SetPropertyOption(string? accessPath, string? propertyName, object? propertyValue)
        {
            AccessPath = accessPath;
            PropertyName = propertyName;
            InvokeArgument = new(propertyValue, this);
        }
    }
}
