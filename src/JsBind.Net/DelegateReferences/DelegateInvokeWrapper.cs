using System.Collections.Generic;
using System.ComponentModel;
using System.Text.Json;
using System.Text.Json.Serialization;
using JsBind.Net.Internal.JsonConverters;

namespace JsBind.Net.DelegateReferences;

/// <summary>
/// A wrapper for deserializing delegate invocation arguments.
/// </summary>
[JsonConverter(typeof(DelegateInvokeConverter))]
[EditorBrowsable(EditorBrowsableState.Never)]
public class DelegateInvokeWrapper
{
    /// <summary>
    /// The arguments for invoking the delegate.
    /// </summary>
    public IEnumerable<JsonElement>? Args { get; set; }

    /// <summary>
    /// The JSON serialization options to be used.
    /// </summary>
    public JsonSerializerOptions? JsonSerializerOptions { get; set; }
}
