using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace JsBind.Net.Internal.JsonConverters;

/// <summary>
/// This converter injects creates a generic instance of <see cref="WriteObjectReferenceConverter{T}" />.
/// </summary>
internal class WriteObjectReferenceConverterFactory : JsonConverterFactory
{
    /// <summary>
    /// Binding type to serialize as a plain object instead of a JavaScript object reference.
    /// </summary>
    public Type? SkipConvertType { get; set; }

    public override bool CanConvert(Type typeToConvert)
    {
        if (typeToConvert == SkipConvertType)
        {
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
