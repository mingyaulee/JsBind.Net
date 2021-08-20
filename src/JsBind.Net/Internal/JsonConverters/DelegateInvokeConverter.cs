using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using JsBind.Net.Internal.DelegateReferences;

namespace JsBind.Net.Internal.JsonConverters
{
    /// <summary>
    /// This converter injects additional converters for reading references from JavaScript before delegate invocation.
    /// </summary>
    internal class DelegateInvokeConverter : JsonConverter<DelegateInvokeWrapper?>
    {
        public override DelegateInvokeWrapper? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var cloneOptions = new JsonSerializerOptions(options);
            var references = new List<BindingBase?>();
            cloneOptions.Converters.Add(new ReadObjectReferenceConverterFactory(references, options));
            cloneOptions.Converters.Add(new ReadDelegateReferenceConverter(references, options));
            var invokeWrapper = new DelegateInvokeWrapper()
            {
                Args = JsonSerializer.Deserialize<object?[]?>(ref reader, cloneOptions),
                References = references
            };
            return invokeWrapper;
        }

        public override void Write(Utf8JsonWriter writer, DelegateInvokeWrapper? value, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }
    }
}
