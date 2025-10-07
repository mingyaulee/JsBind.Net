using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using JsBind.Net.InvokeOptions;

namespace JsBind.Net.Internal.JsonConverters
{
    /// <summary>
    /// This converter injects additional converters for writing references in invoke argument before invoking in JavaScript.
    /// </summary>
    internal class InvokeArgumentConverter : JsonConverter<InvokeArgument?>
    {
        public override InvokeArgument? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            => throw new NotImplementedException();

        public override void Write(Utf8JsonWriter writer, InvokeArgument? value, JsonSerializerOptions options)
        {
            if (value is null)
            {
                writer.WriteNullValue();
                return;
            }

            var jsRuntime = value.InvokeOption.JsRuntime ?? throw new InvalidOperationException("InvokeOption should have JS runtime adapter.");
            var cloneOptions = new JsonSerializerOptions(options);
            ConvertersFactory.AddWriteConverters(cloneOptions, jsRuntime);
            JsonSerializer.Serialize(writer, value.ArgumentValue, cloneOptions);
        }
    }
}
