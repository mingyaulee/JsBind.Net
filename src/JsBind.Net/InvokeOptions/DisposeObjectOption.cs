using System.Text.Json.Serialization;

namespace JsBind.Net.InvokeOptions
{
    /// <summary>
    /// Invoke option to dispose object. In sync with Modules\InvokeOptions\DisposeObjectOption.js
    /// </summary>
    internal class DisposeObjectOption : InvokeOption
    {
        /// <summary>
        /// Fully qualified function name for disposing object.
        /// </summary>
        public const string Identifier = "JsBindNet.DisposeObject";

        [JsonPropertyName("referenceId")]
        public string? ReferenceId { get; set; }

        public DisposeObjectOption(string? referenceId)
        {
            ReferenceId = referenceId;
        }
    }
}
