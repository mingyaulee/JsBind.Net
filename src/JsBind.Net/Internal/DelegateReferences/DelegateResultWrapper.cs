using System.Text.Json.Serialization;
using JsBind.Net.Internal.JsonConverters;

namespace JsBind.Net.Internal.DelegateReferences;

/// <summary>
/// A wrapper for serializing a delegate invocation result.
/// </summary>
[JsonConverter(typeof(DelegateResultConverter))]
internal class DelegateResultWrapper
{
    public IJsRuntimeAdapter? JsRuntime { get; set; }
    public object? Result { get; set; }
}
