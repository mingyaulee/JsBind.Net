using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace JsBind.Net.Internal.JsonConverters
{
    /// <summary>
    /// Reads the object reference from JavaScript and keeps track of all references for initialization.
    /// </summary>
    internal class ReadObjectReferenceConverter<T> : JsonConverter<T?>
        where T : BindingBase
    {
        private readonly IList<BindingBase?> references;
        private readonly JsonSerializerOptions jsonSerializerOptions;

        public ReadObjectReferenceConverter(IList<BindingBase?> references, JsonSerializerOptions jsonSerializerOptions)
        {
            this.references = references;
            this.jsonSerializerOptions = jsonSerializerOptions;
        }

        public override bool CanConvert(Type typeToConvert)
        {
            return typeof(IObjectBindingBase).IsAssignableFrom(typeToConvert);
        }

        public override T? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var objectBindingBase = (T?)JsonSerializer.Deserialize(ref reader, typeToConvert, jsonSerializerOptions);
            references.Add(objectBindingBase);
            return objectBindingBase;
        }

        public override void Write(Utf8JsonWriter writer, T? value, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }
    }
}
