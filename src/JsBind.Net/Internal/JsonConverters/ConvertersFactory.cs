using System.Text.Json;

namespace JsBind.Net.Internal.JsonConverters
{
    internal static class ConvertersFactory
    {
        public static void AddReadConverters(JsonSerializerOptions options, JsonSerializerOptions cloneOptions, IJsRuntimeAdapter jsRuntime)
        {
            cloneOptions.Converters.Add(new ReadJsRuntimeConverter(jsRuntime));
            cloneOptions.Converters.Add(new ReadDelegateReferenceConverter(jsRuntime, options));
        }

        public static void AddWriteConverters(JsonSerializerOptions cloneOptions, IJsRuntimeAdapter jsRuntime)
        {
            cloneOptions.Converters.Add(new WriteObjectReferenceConverterFactory());
            cloneOptions.Converters.Add(new WriteDelegateReferenceConverter(jsRuntime));
        }
    }
}
