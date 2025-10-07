using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using JsBind.Net.DelegateReferences;

namespace JsBind.Net.Internal.JsonConverters;

/// <summary>
/// This converter injects additional converters for reading references from JavaScript before delegate invocation.
/// </summary>
internal class DelegateInvokeConverter : JsonConverter<DelegateInvokeWrapper?>
{
    public override DelegateInvokeWrapper? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var invokeWrapper = new DelegateInvokeWrapper()
        {
            Args = JsonSerializer.Deserialize<IEnumerable<JsonElement>?>(ref reader, options),
            JsonSerializerOptions = options
        };
        return invokeWrapper;
    }

    public override void Write(Utf8JsonWriter writer, DelegateInvokeWrapper? value, JsonSerializerOptions options)
        => throw new NotImplementedException();
}
