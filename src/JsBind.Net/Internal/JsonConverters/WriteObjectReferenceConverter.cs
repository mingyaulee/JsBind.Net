using System;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using JsBind.Net.Internal.References;

namespace JsBind.Net.Internal.JsonConverters;

/// <summary>
/// Writes binding base as object reference to be revived in JavaScript.
/// </summary>
internal class WriteObjectReferenceConverter<T> : JsonConverter<T>
    where T : BindingBase
{
    public override bool CanConvert(Type typeToConvert)
        => typeof(BindingBase).IsAssignableFrom(typeToConvert);

    public override T? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        => throw new NotImplementedException();

    public override void Write(Utf8JsonWriter writer, T? value, JsonSerializerOptions options)
    {
        if (value is null)
        {
            writer.WriteNullValue();
            return;
        }

        var accessPath = value.InternalGetAccessPath();
        if (!string.IsNullOrEmpty(accessPath))
        {
            var objectReference = new ObjectReference(value.InternalGetAccessPath());
            JsonSerializer.Serialize(writer, objectReference, options);
        }
        else
        {
#pragma warning disable CA1869 // Cache and reuse 'JsonSerializerOptions' instances
            var cloneOptions = new JsonSerializerOptions(options);
#pragma warning restore CA1869 // Cache and reuse 'JsonSerializerOptions' instances
            cloneOptions.Converters.Remove(cloneOptions.Converters.OfType<WriteObjectReferenceConverterFactory>().Single());
            cloneOptions.Converters.Add(new WriteObjectReferenceConverterFactory()
            {
                SkipConvertOnce = true,
                SkipConvertType = typeof(T)
            });
            JsonSerializer.Serialize(writer, (object?)value, cloneOptions);
        }
    }
}
