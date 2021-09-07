using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using JsBind.Net.InvokeOptions;

namespace JsBind.Net.Internal.References
{
    /// <summary>
    /// Delegate refence class to be serialized and passed to JavaScript.
    /// </summary>
    internal class DelegateReference : ReferenceBase
    {
        private IEnumerable<string?>? argumentsReferenceIds;

        public override ReferenceType ReferenceType => ReferenceType.Delegate;

        [JsonPropertyName("delegateId")]
        public string? DelegateId { get; }

        [JsonPropertyName("argumentBindings")]
        public IEnumerable<ObjectBindingConfiguration?>? ArgumentBindings { get; }

        [JsonPropertyName("storeArgumentsAsReferences")]
        public IEnumerable<bool>? StoreArgumentsAsReferences => ArgumentBindings is not null ? ArgumentBindings.Select(binding => binding is not null) : null;

        [JsonPropertyName("argumentsReferenceIds")]
        public IEnumerable<string?>? ArgumentsReferenceIds
        {
            get
            {
                if (StoreArgumentsAsReferences is not null && argumentsReferenceIds is null)
                {
                    argumentsReferenceIds = StoreArgumentsAsReferences.Select(storeArgumentAsReference => storeArgumentAsReference ? Guid.NewGuid().ToString() : null).ToList();
                }
                return argumentsReferenceIds;
            }
        }

        [JsonPropertyName("isAsync")]
        public bool IsAsync { get; }

        public DelegateReference(string? delegateId, IEnumerable<ObjectBindingConfiguration?>? argumentBindings, bool isAsync)
        {
            DelegateId = delegateId;
            ArgumentBindings = argumentBindings;
            IsAsync = isAsync;
        }
    }
}
