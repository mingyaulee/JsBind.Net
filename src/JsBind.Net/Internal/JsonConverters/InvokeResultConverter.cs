using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace JsBind.Net.Internal.JsonConverters
{
    /// <summary>
    /// This converter injects additional converters for reading references in invoke result after invoking in JavaScript.
    /// </summary>
    internal class InvokeResultConverter<TValue> : JsonConverter<InvokeResult<TValue>?>
    {
        public override bool CanConvert(Type typeToConvert)
        {
            return typeof(InvokeResultWithValue).IsAssignableFrom(typeToConvert);
        }

        public override InvokeResult<TValue>? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var cloneOptions = new JsonSerializerOptions(options);
            var proxyJsRuntime = new ProxyJsRuntimeAdapter();
            ConvertersFactory.AddReadConverters(options, cloneOptions, proxyJsRuntime);
            var invokeResult = JsonSerializer.Deserialize<InvokeResultWithValue<TValue>>(ref reader, cloneOptions);
            if (invokeResult is not null)
            {
                invokeResult.ProxyJsRuntime = proxyJsRuntime;
            }
            return invokeResult;
        }

        public override void Write(Utf8JsonWriter writer, InvokeResult<TValue>? value, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }
    }
}
