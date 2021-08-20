using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using JsBind.Net.Internal.DelegateReferences;
using JsBind.Net.InvokeOptions;
using Microsoft.JSInterop;

namespace JsBind.Net.Internal.References
{
    /// <summary>
    /// Delegate refence class to be serialized and passed to JavaScript.
    /// </summary>
    internal class DelegateReference : ReferenceBase
    {
        private string? argumentsReferenceId;

        public override ReferenceType ReferenceType => ReferenceType.Delegate;

        [JsonPropertyName("delegateId")]
        public string? DelegateId { get; }

        [JsonPropertyName("argumentBindings")]
        public IEnumerable<ObjectBindingConfiguration?>? ArgumentBindings { get; }

        [JsonPropertyName("storeArgumentsAsReference")]
        public bool StoreArgumentsAsReference => ArgumentBindings is not null && ArgumentBindings.Any();

        [JsonPropertyName("argumentsReferenceId")]
        public string? ArgumentsReferenceId
        {
            get
            {
                if (StoreArgumentsAsReference && argumentsReferenceId is null)
                {
                    argumentsReferenceId = Guid.NewGuid().ToString();
                }
                return argumentsReferenceId;
            }
        }

        [JsonPropertyName("isAsync")]
        public bool IsAsync { get; }

        [JsonPropertyName("delegateInvoker")]
        public DotNetObjectReference<CapturedDelegateReference>? DelegateInvoker { get; set; }

        public DelegateReference(string? delegateId, IEnumerable<ObjectBindingConfiguration?>? argumentBindings, bool isAsync)
        {
            DelegateId = delegateId;
            ArgumentBindings = argumentBindings;
            IsAsync = isAsync;
        }
    }
}
