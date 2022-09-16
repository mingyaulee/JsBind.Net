using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace JsBind.Net.Internal.JsonConverters
{
    /// <summary>
    /// This converter injects creates a generic instance of <see cref="WriteObjectReferenceConverter{T}" />.
    /// </summary>
    internal class WriteObjectReferenceConverterFactory : JsonConverterFactory
    {
        /// <summary>
        /// This is a workaround to tell the converter factory to skip the next CanConvert check just once.
        /// So the default object serializer will be used instead of this converter.
        /// </summary>
        public bool SkipConvertOnce { get; set; }

        /// <summary>
        /// This is a workaround to tell the converter factory to skip the next CanConvert check just once for this type.
        /// So the default object serializer will be used instead of this converter.
        /// </summary>
        public Type? SkipConvertType { get; set; }

        public override bool CanConvert(Type typeToConvert)
        {
            if (SkipConvertOnce && typeToConvert == SkipConvertType)
            {
                SkipConvertOnce = false;
                SkipConvertType = null;
                return false;
            }

            return typeof(BindingBase).IsAssignableFrom(typeToConvert);
        }

        public override JsonConverter? CreateConverter(Type typeToConvert, JsonSerializerOptions options)
        {
            var converterType = typeof(WriteObjectReferenceConverter<>).MakeGenericType(typeToConvert);
            return (JsonConverter)Activator.CreateInstance(converterType)!;
        }
    }
}
