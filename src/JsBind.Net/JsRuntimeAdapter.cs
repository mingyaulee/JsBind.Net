using System;
using System.Threading.Tasks;
using JsBind.Net.Configurations;
using JsBind.Net.Internal;
using JsBind.Net.Internal.Extensions;
using JsBind.Net.InvokeOptions;
using Microsoft.JSInterop;

namespace JsBind.Net
{
    /// <summary>
    /// Adapter for <see cref="IJSRuntime" />.
    /// </summary>
    public class JsRuntimeAdapter : IJsRuntimeAdapter
    {
        private readonly IJSRuntime? jsRuntime;
        private readonly IJsBindOptions? jsBindOptions;

        /// <summary>
        /// Creates a new instance of <see cref="JsRuntimeAdapter" /> to be used for binding interop.
        /// </summary>
        /// <param name="jsRuntime">The JS runtime instance.</param>
        /// <param name="jsBindOptions">The JSBind options.</param>
        public JsRuntimeAdapter(IJSRuntime jsRuntime, IJsBindOptions jsBindOptions)
        {
            this.jsRuntime = jsRuntime;
            this.jsBindOptions = jsBindOptions;
        }

        /// <summary>
        /// Creates a new instance of <see cref="JsRuntimeAdapter" /> to be used for binding interop.
        /// </summary>
        protected JsRuntimeAdapter()
        {
        }

        /// <summary>
        /// Gets the underlying JS runtime instance.
        /// </summary>
        public virtual IJSRuntime JsRuntime => jsRuntime ?? throw new InvalidOperationException("Inheriting classed must override the JSRuntime property.");

        /// <summary>
        /// Gets the JsBind options.
        /// </summary>
        public virtual IJsBindOptions JsBindOptions => jsBindOptions ?? throw new InvalidOperationException("Inheriting classed must override the JsBindOptions property.");

        /// <inheritdoc />
        public TValue? Invoke<TValue>(string identifier, InvokeOptionWithReturnValue invokeOption)
        {
            return (TValue?)InvokeInternal<TValue>(identifier, invokeOption, typeof(TValue));
        }

        /// <inheritdoc />
        public async ValueTask<TValue?> InvokeAsync<TValue>(string identifier, InvokeOptionWithReturnValue invokeOption)
        {
            return (TValue?)await InvokeAsyncInternal<TValue>(identifier, invokeOption, typeof(TValue)).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public void InvokeVoid(string identifier, InvokeOption invokeOption)
        {
            invokeOption.JsRuntime = this;
            InvokeResult? invokeResult;
            if (JsBindOptions.UseInProcessJsRuntime)
            {
                invokeResult = ((IJSInProcessRuntime)JsRuntime).Invoke<InvokeResult?>(identifier, invokeOption);
            }
            else if (invokeOption is DisposeObjectOption || invokeOption is DisposeDelegateOption)
            {
                // If we are disposing without using in process JS runtime, invoke asynchronously but do not wait for it.
#pragma warning disable CA2012 // Use ValueTasks correctly
                JsRuntime.InvokeAsync<InvokeResult?>(identifier, invokeOption);
#pragma warning restore CA2012 // Use ValueTasks correctly
                return;
            }
            else
            {
                invokeResult = JsRuntime.InvokeAsync<InvokeResult?>(identifier, invokeOption).AsTask().ConfigureAwait(false).GetAwaiter().GetResult();
            }

            if (invokeResult is not null && invokeResult.IsError && invokeResult.ErrorMessage is not null)
            {
                throw new JsBindException(invokeResult.ErrorMessage, invokeResult.StackTrace);
            }
        }

        /// <inheritdoc />
        public async ValueTask InvokeVoidAsync(string identifier, InvokeOption invokeOption)
        {
            invokeOption.JsRuntime = this;
            var invokeResult = await JsRuntime.InvokeAsync<InvokeResult?>(identifier, invokeOption).ConfigureAwait(false);
            if (invokeResult is not null && invokeResult.IsError && invokeResult.ErrorMessage is not null)
            {
                throw new JsBindException(invokeResult.ErrorMessage, invokeResult.StackTrace);
            }
        }

        /// <inheritdoc />
        public bool IsJsRuntimeEqual(IJsRuntimeAdapter other)
        {
            if (other is JsRuntimeAdapter otherJsRuntime)
            {
                return JsRuntime.Equals(otherJsRuntime.JsRuntime);
            }

            return false;
        }

        private object? InvokeInternal<TValue>(string identifier, InvokeOptionWithReturnValue invokeOption, Type type)
        {
            invokeOption.ReturnValueBinding = type.GetBindingConfiguration(JsBindOptions.BindingConfigurationProvider);
            invokeOption.JsRuntime = this;
            InvokeResult<TValue>? invokeResult;
            if (JsBindOptions.UseInProcessJsRuntime)
            {
                invokeResult = ((IJSInProcessRuntime)JsRuntime).Invoke<InvokeResult<TValue>?>(identifier, invokeOption);
            }
            else
            {
                invokeResult = JsRuntime.InvokeAsync<InvokeResult<TValue>?>(identifier, invokeOption).AsTask().ConfigureAwait(false).GetAwaiter().GetResult();
            }

            if (invokeResult is not null && invokeResult.IsError && invokeResult.ErrorMessage is not null)
            {
                throw new JsBindException(invokeResult.ErrorMessage, invokeResult.StackTrace);
            }

            return GetInvokeResultObject(invokeResult, type);
        }

        private async ValueTask<object?> InvokeAsyncInternal<TValue>(string identifier, InvokeOptionWithReturnValue invokeOption, Type type)
        {
            invokeOption.ReturnValueBinding = type.GetBindingConfiguration(JsBindOptions.BindingConfigurationProvider);
            invokeOption.JsRuntime = this;
            var invokeResult = await JsRuntime.InvokeAsync<InvokeResult<TValue>?>(identifier, invokeOption).ConfigureAwait(false);
            if (invokeResult is not null && invokeResult.IsError && invokeResult.ErrorMessage is not null)
            {
                throw new JsBindException(invokeResult.ErrorMessage, invokeResult.StackTrace);
            }

            return GetInvokeResultObject(invokeResult, type);
        }

        private object? GetInvokeResultObject<TValue>(InvokeResult<TValue>? invokeResult, Type type)
        {
            var invokeResultObject = invokeResult is null ? type.GetDefaultValue() : invokeResult.Value;
            if (invokeResult is null || invokeResult.ProxyJsRuntime is null)
            {
                return invokeResultObject;
            }

            invokeResult.ProxyJsRuntime.SetJsRuntime(this);
            return invokeResultObject;
        }
    }
}
