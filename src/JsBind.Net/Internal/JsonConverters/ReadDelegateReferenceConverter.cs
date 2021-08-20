using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using JsBind.Net.Internal.DelegateReferences;
using JsBind.Net.Internal.References;

namespace JsBind.Net.Internal.JsonConverters
{
    /// <summary>
    /// Reads the delegate reference object from JavaScript and convert it into a DotNet delegate and keeps track of all references for initialization.
    /// </summary>
    internal class ReadDelegateReferenceConverter : JsonConverter<Delegate?>
    {
        private readonly IList<BindingBase?> references;
        private readonly JsonSerializerOptions jsonSerializerOptions;

        public ReadDelegateReferenceConverter(IList<BindingBase?> references, JsonSerializerOptions jsonSerializerOptions)
        {
            this.references = references;
            this.jsonSerializerOptions = jsonSerializerOptions;
        }

        public override bool CanConvert(Type typeToConvert)
        {
            return typeof(Delegate).IsAssignableFrom(typeToConvert);
        }

        public override Delegate? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var jsDelegateReference = JsonSerializer.Deserialize<JsDelegateReference?>(ref reader, jsonSerializerOptions);
            if (jsDelegateReference?.DelegateId is not null && jsDelegateReference.DelegateId != Guid.Empty)
            {
                return DelegateReferenceManager.GetCapturedDelegateReference(jsDelegateReference.DelegateId.Value).DelegateObject;
            }

            if (!string.IsNullOrEmpty(jsDelegateReference?.AccessPath))
            {
                var functionBinding = new JsFunctionProxy(jsDelegateReference.AccessPath);
                references.Add(functionBinding);
                return functionBinding.GetDelegate(typeToConvert);
            }

            return null;
        }

        public override void Write(Utf8JsonWriter writer, Delegate? value, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }
    }
}
