using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using JsBind.Net.Internal.Extensions;
using JsBind.Net.Internal.JsonConverters;

namespace JsBind.Net.InvokeOptions
{
    /// <summary>
    /// Invoke option to invoke function.
    /// </summary>
    public interface IInvokeFunctionOption
    {
        /// <summary>
        /// The access path to the target object.
        /// </summary>
        string? AccessPath { get; set; }

        /// <summary>
        /// The name of the target function, null if the target object is a function.
        /// </summary>
        string? FunctionName { get; set; }

        /// <summary>
        /// The collection of arguments to invoke the target function.
        /// </summary>
        FunctionArgumentCollection? FunctionArguments { get; set; }
    }

    /// <summary>
    /// Invoke option to invoke function without return value. In sync with Modules\InvokeOptions\InvokeFunctionOption.js
    /// </summary>
    internal class InvokeFunctionOption : InvokeOption, IInvokeFunctionOption
    {
        /// <summary>
        /// Fully qualified function name for invoking function synchronously.
        /// </summary>
        public const string Identifier = "JsBindNet.InvokeFunction";

        /// <summary>
        /// Fully qualified function name for invoking function asynchronously.
        /// </summary>
        public const string AsyncIdentifier = "JsBindNet.InvokeFunctionAsync";

        /// <inheritdoc />
        [JsonPropertyName("accessPath")]
        public string? AccessPath { get; set; }

        /// <inheritdoc />
        [JsonPropertyName("functionName")]
        public string? FunctionName { get; set; }

        /// <inheritdoc />
        [JsonPropertyName("functionArguments")]
        [JsonConverter(typeof(FunctionArgumentCollectionConverter))]
        public FunctionArgumentCollection? FunctionArguments { get; set; }

        public InvokeFunctionOption(string? accessPath, string? functionName, IEnumerable<object?>? functionArguments)
        {
            AccessPath = accessPath;
            FunctionName = functionName;
            FunctionArguments = functionArguments is null ? null : new FunctionArgumentCollection(functionArguments, this);
        }
    }

    /// <summary>
    /// Invoke option to invoke function with return value. In sync with Modules\InvokeOptions\InvokeFunctionOption.js
    /// </summary>
    internal class InvokeFunctionOption<TValue> : InvokeOptionWithReturnValue, IInvokeFunctionOption
    {
        private string? returnValueReferenceId;

        /// <inheritdoc />
        [JsonPropertyName("returnValueIsReference")]
        public override bool ReturnValueIsReference => typeof(IObjectBindingBase).IsAssignableFrom(typeof(TValue)) || typeof(TValue).IsIterableType() || typeof(Delegate).IsAssignableFrom(typeof(TValue));

        /// <inheritdoc />
        [JsonPropertyName("returnValueReferenceId")]
        public override string? ReturnValueReferenceId
        {
            get
            {
                if (ReturnValueIsReference && returnValueReferenceId is null)
                {
                    returnValueReferenceId = Guid.NewGuid().ToString();
                }
                return returnValueReferenceId;
            }
        }

        /// <inheritdoc />
        [JsonPropertyName("accessPath")]
        public string? AccessPath { get; set; }

        /// <inheritdoc />
        [JsonPropertyName("functionName")]
        public string? FunctionName { get; set; }

        /// <inheritdoc />
        [JsonPropertyName("functionArguments")]
        [JsonConverter(typeof(FunctionArgumentCollectionConverter))]
        public FunctionArgumentCollection? FunctionArguments { get; set; }

        public InvokeFunctionOption(string? accessPath, string? functionName, IEnumerable<object?>? functionArguments)
        {
            AccessPath = accessPath;
            FunctionName = functionName;
            FunctionArguments = functionArguments is null ? null : new FunctionArgumentCollection(functionArguments, this);
        }
    }
}
