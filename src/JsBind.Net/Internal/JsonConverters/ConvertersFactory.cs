using System.Collections.Generic;
using System.Text.Json;

namespace JsBind.Net.Internal.JsonConverters
{
    internal static class ConvertersFactory
    {
        public static void AddReadConverters(JsonSerializerOptions options, JsonSerializerOptions cloneOptions, IList<BindingBase?> references)
        {
            cloneOptions.Converters.Add(new ReadObjectReferenceConverterFactory(references, options));
            cloneOptions.Converters.Add(new ReadDelegateReferenceConverter(references, options));
        }

        public static void AddWriteConverters(JsonSerializerOptions cloneOptions, IJsRuntimeAdapter jsRuntime)
        {
            cloneOptions.Converters.Add(new WriteObjectReferenceConverterFactory());
            cloneOptions.Converters.Add(new WriteDelegateReferenceConverter(jsRuntime));
        }
    }
}
