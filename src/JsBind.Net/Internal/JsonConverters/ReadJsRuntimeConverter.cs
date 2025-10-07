using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace JsBind.Net.Internal.JsonConverters;

/// <summary>
/// Returns the existing JS runtime during deserialization.
/// </summary>
internal class ReadJsRuntimeConverter(IJsRuntimeAdapter jsRuntime) : JsonConverter<IJsRuntimeAdapter?>
{
    private readonly IJsRuntimeAdapter jsRuntime = jsRuntime;

    public override bool CanConvert(Type typeToConvert)
        => typeof(IJsRuntimeAdapter) == typeToConvert;

    public override IJsRuntimeAdapter? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        // From JavaScript the value is set to 0, so we just need to read the integer and discard it to move the reader position to the next token
        _ = reader.GetInt32();
        return jsRuntime;
    }

    public override void Write(Utf8JsonWriter writer, IJsRuntimeAdapter? value, JsonSerializerOptions options)
        => throw new NotImplementedException();
}
