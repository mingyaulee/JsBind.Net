using System;
using System.Collections.Generic;
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
            var references = new List<BindingBase?>();
            cloneOptions.Converters.Add(new ReadObjectReferenceConverterFactory(references, options));
            cloneOptions.Converters.Add(new ReadDelegateReferenceConverter(references, options));
            var invokeResult = JsonSerializer.Deserialize<InvokeResultWithValue<TValue>>(ref reader, cloneOptions);
            if (invokeResult is not null)
            {
                invokeResult.References = references;
            }
            return invokeResult;
        }

        public override void Write(Utf8JsonWriter writer, InvokeResult<TValue>? value, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }
    }
}
