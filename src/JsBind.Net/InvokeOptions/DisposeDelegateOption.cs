using System.Text.Json.Serialization;

namespace JsBind.Net.InvokeOptions;

/// <summary>
/// Invoke option to dispose delegate. In sync with Modules\InvokeOptions\DisposeDelegateOption.js
/// </summary>
internal class DisposeDelegateOption(string? delegateId) : InvokeOption
{
    /// <summary>
    /// Fully qualified function name for disposing delegate.
    /// </summary>
    public const string Identifier = "JsBindNet.DisposeDelegate";

    [JsonPropertyName("delegateId")]
    public string? DelegateId { get; set; } = delegateId;
}
