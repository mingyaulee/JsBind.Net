using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace JsBind.Net.Internal.JsonConverters
{
    /// <summary>
    /// This converter injects creates a generic instance of <see cref="ReadObjectReferenceConverter{T}" />.
    /// </summary>
    internal class ReadObjectReferenceConverterFactory : JsonConverterFactory
    {
        private readonly IList<BindingBase?> references;
        private readonly JsonSerializerOptions jsonSerializerOptions;

        public ReadObjectReferenceConverterFactory(IList<BindingBase?> references, JsonSerializerOptions jsonSerializerOptions)
        {
            this.references = references;
            this.jsonSerializerOptions = jsonSerializerOptions;
        }

        public override bool CanConvert(Type typeToConvert)
        {
            return typeof(ObjectBindingBase).IsAssignableFrom(typeToConvert);
        }

        public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options)
        {
            var converterType = typeof(ReadObjectReferenceConverter<>).MakeGenericType(typeToConvert);
            return (JsonConverter)Activator.CreateInstance(converterType, references, jsonSerializerOptions)!;
        }
    }
}
