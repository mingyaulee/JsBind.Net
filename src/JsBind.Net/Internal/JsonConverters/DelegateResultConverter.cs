using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using JsBind.Net.Internal.DelegateReferences;

namespace JsBind.Net.Internal.JsonConverters
{
    /// <summary>
    /// This converter injects additional converters for writing references in invocation result after invoking delegate from JavaScript.
    /// </summary>
    internal class DelegateResultConverter : JsonConverter<DelegateResultWrapper?>
    {
        public override DelegateResultWrapper? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            => throw new NotImplementedException();

        public override void Write(Utf8JsonWriter writer, DelegateResultWrapper? value, JsonSerializerOptions options)
        {
            if (value is null)
            {
                writer.WriteNullValue();
                return;
            }

            var jsRuntime = value.JsRuntime ?? throw new InvalidOperationException("DelegateResultWrapper should have JS runtime adapter.");
            var cloneOptions = new JsonSerializerOptions(options);
            ConvertersFactory.AddWriteConverters(cloneOptions, jsRuntime);
            JsonSerializer.Serialize(writer, value.Result, cloneOptions);
        }
    }
}
