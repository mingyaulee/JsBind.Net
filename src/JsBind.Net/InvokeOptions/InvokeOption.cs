using System.Text.Json.Serialization;

namespace JsBind.Net.InvokeOptions
{
    /// <summary>
    /// Base invoke option without return value. In sync with Modules\InvokeOptions\InvokeOption.js
    /// </summary>
    public abstract class InvokeOption
    {
        internal IJsRuntimeAdapter? JsRuntime { get; set; }

        /// <summary>
        /// Indicates whether this invocation has return value.
        /// </summary>
        [JsonPropertyName("hasReturnValue")]
        public virtual bool HasReturnValue => false;

        /// <summary>
        /// Indicates whether this invocation's return value is reference.
        /// </summary>
        [JsonPropertyName("returnValueIsReference")]
        public virtual bool ReturnValueIsReference => false;

        /// <summary>
        /// The identifier to be used as a key for the JavaScript object returned.
        /// </summary>
        [JsonPropertyName("returnValueReferenceId")]
        public virtual string? ReturnValueReferenceId => null;
    }

    /// <summary>
    /// Base invoke option with return value.
    /// </summary>
    public abstract class InvokeOptionWithReturnValue : InvokeOption
    {
        /// <inheritdoc />
        public override bool HasReturnValue => true;

        /// <summary>
        /// The object binding configuration for the return value.
        /// </summary>
        [JsonPropertyName("returnValueBinding")]
        public ObjectBindingConfiguration? ReturnValueBinding { get; set; }
    }
}
