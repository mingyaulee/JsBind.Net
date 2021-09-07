using System;
using System.Threading.Tasks;
using JsBind.Net.InvokeOptions;

namespace JsBind.Net.Internal
{
    /// <summary>
    /// Proxy adapter for JS runtime. Used for deserialization when the real JS runtime is not available yet.
    /// </summary>
    internal class ProxyJsRuntimeAdapter : IJsRuntimeAdapter
    {
        private IJsRuntimeAdapter? jsRuntime;
        public IJsRuntimeAdapter JsRuntime
        {
            get
            {
                return jsRuntime ?? throw new InvalidOperationException("Proxy JsRuntime has not been set.");
            }
            set
            {
                jsRuntime = value;
            }
        }

        /// <inheritdoc />
        public TValue? Invoke<TValue>(string identifier, InvokeOptionWithReturnValue invokeOption)
            => JsRuntime.Invoke<TValue>(identifier, invokeOption);

        /// <inheritdoc />
        public ValueTask<TValue?> InvokeAsync<TValue>(string identifier, InvokeOptionWithReturnValue invokeOption)
            => JsRuntime.InvokeAsync<TValue>(identifier, invokeOption);

        /// <inheritdoc />
        public void InvokeVoid(string identifier, InvokeOption invokeOption)
            => JsRuntime.InvokeVoid(identifier, invokeOption);

        /// <inheritdoc />
        public ValueTask InvokeVoidAsync(string identifier, InvokeOption invokeOption)
            => JsRuntime.InvokeVoidAsync(identifier, invokeOption);
    }
}
