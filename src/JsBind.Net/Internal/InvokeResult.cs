using System.Text.Json.Serialization;
using JsBind.Net.Internal.JsonConverters;

namespace JsBind.Net.Internal
{
    /// <summary>
    /// Base class of invocation result from JavaScript.
    /// </summary>
    internal class InvokeResult
    {
        [JsonPropertyName("isError")]
        public bool IsError { get; set; }

        [JsonPropertyName("errorMessage")]
        public string? ErrorMessage { get; set; }
    }

    /// <summary>
    /// Base class of invocation result with value from JavaScript.
    /// </summary>
    internal abstract class InvokeResultWithValue : InvokeResult
    {
        [JsonIgnore]
        public ProxyJsRuntimeAdapter? ProxyJsRuntime { get; set; }
    }

    /// <summary>
    /// Invoke result with value of type <typeparamref name="TValue" />.
    /// </summary>
    /// <typeparam name="TValue">The type of value.</typeparam>
    [JsonConverter(typeof(InvokeResultConverterFactory))]
    internal class InvokeResult<TValue> : InvokeResultWithValue
    {
        [JsonPropertyName("value")]
        public TValue? Value { get; set; }
    }

    /// <summary>
    /// A class for deserialization without InvokeResultConverter.
    /// </summary>
    /// <typeparam name="TValue">The type of value.</typeparam>
    internal class InvokeResultWithValue<TValue> : InvokeResult<TValue>
    {
    }
}
