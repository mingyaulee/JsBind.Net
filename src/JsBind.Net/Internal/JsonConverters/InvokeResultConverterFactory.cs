using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace JsBind.Net.Internal.JsonConverters
{
    /// <summary>
    /// This converter injects creates a generic instance of <see cref="InvokeResultConverter{TValue}" />.
    /// </summary>
    internal class InvokeResultConverterFactory : JsonConverterFactory
    {
        public override bool CanConvert(Type typeToConvert)
        {
            return typeof(InvokeResultWithValue).IsAssignableFrom(typeToConvert);
        }

        public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options)
        {
            var converterType = typeof(InvokeResultConverter<>).MakeGenericType(typeToConvert.GetGenericArguments()[0]);
            return (JsonConverter)Activator.CreateInstance(converterType)!;
        }
    }
}
