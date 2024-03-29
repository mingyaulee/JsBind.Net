﻿using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using JsBind.Net.InvokeOptions;

namespace JsBind.Net.Internal.JsonConverters
{
    /// <summary>
    /// This converter injects additional converters for writing references in function arguments before invoking function in JavaScript.
    /// </summary>
    internal class FunctionArgumentCollectionConverter : JsonConverter<FunctionArgumentCollection?>
    {
        public override FunctionArgumentCollection? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }

        public override void Write(Utf8JsonWriter writer, FunctionArgumentCollection? value, JsonSerializerOptions options)
        {
            if (value is null)
            {
                writer.WriteNullValue();
                return;
            }

            var jsRuntime = value.InvokeOption.JsRuntime ?? throw new InvalidOperationException("InvokeOption should have JS runtime adapter.");
            var cloneOptions = new JsonSerializerOptions(options);
            ConvertersFactory.AddWriteConverters(cloneOptions, jsRuntime);
            JsonSerializer.Serialize(writer, value.EnumerableValue, cloneOptions);
        }
    }
}
