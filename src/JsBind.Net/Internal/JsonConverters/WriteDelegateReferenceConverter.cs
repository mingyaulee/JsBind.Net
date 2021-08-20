using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using JsBind.Net.Internal.DelegateReferences;

namespace JsBind.Net.Internal.JsonConverters
{
    /// <summary>
    /// Writes delegate as delegate reference to be revived in JavaScript.
    /// </summary>
    internal class WriteDelegateReferenceConverter : JsonConverter<Delegate?>
    {
        private readonly IJsRuntimeAdapter jsRuntime;

        public WriteDelegateReferenceConverter(IJsRuntimeAdapter jsRuntime)
        {
            this.jsRuntime = jsRuntime;
        }

        public override bool CanConvert(Type typeToConvert)
        {
            return typeof(Delegate).IsAssignableFrom(typeToConvert);
        }

        public override Delegate? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }

        public override void Write(Utf8JsonWriter writer, Delegate? value, JsonSerializerOptions options)
        {
            if (value is null)
            {
                writer.WriteNullValue();
                return;
            }

            var delegateReference = DelegateReferenceManager.GetDelegateReference(value, jsRuntime);
            JsonSerializer.Serialize(writer, delegateReference, options);
        }
    }
}
