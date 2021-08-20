using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using JsBind.Net.Internal.References;

namespace JsBind.Net.Internal.JsonConverters
{
    /// <summary>
    /// Writes binding base as object reference to be revived in JavaScript.
    /// </summary>
    internal class WriteObjectReferenceConverter : JsonConverter<BindingBase?>
    {
        public override bool CanConvert(Type typeToConvert)
        {
            return typeof(BindingBase).IsAssignableFrom(typeToConvert);
        }

        public override BindingBase? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }

        public override void Write(Utf8JsonWriter writer, BindingBase? value, JsonSerializerOptions options)
        {
            if (value is null)
            {
                writer.WriteNullValue();
                return;
            }

            var objectReference = new ObjectReference(value.InternalGetAccessPath());
            JsonSerializer.Serialize(writer, objectReference, options);
        }
    }
}
