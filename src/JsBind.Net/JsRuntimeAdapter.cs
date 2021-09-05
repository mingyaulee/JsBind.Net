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
    /// Adapter for IJSRuntime
    /// </summary>
    public class JsRuntimeAdapter : IJsRuntimeAdapter
    {
        private readonly IJSRuntime jsRuntime;
        private readonly IJsBindOptions jsBindOptions;

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
            if (jsBindOptions.UseInProcessJsRuntime)
            {
                invokeResult = ((IJSInProcessRuntime)jsRuntime).Invoke<InvokeResult?>(identifier, invokeOption);
            }
            else if (invokeOption is DisposeObjectOption || invokeOption is DisposeDelegateOption)
            {
                // If we are disposing without using in process JS runtime, invoke asynchronously but do not wait for it.
#pragma warning disable CA2012 // Use ValueTasks correctly
                jsRuntime.InvokeAsync<InvokeResult?>(identifier, invokeOption);
#pragma warning restore CA2012 // Use ValueTasks correctly
                return;
            }
            else
            {
                invokeResult = jsRuntime.InvokeAsync<InvokeResult?>(identifier, invokeOption).AsTask().ConfigureAwait(false).GetAwaiter().GetResult();
            }

            if (invokeResult is not null && invokeResult.IsError && invokeResult.ErrorMessage is not null)
            {
                throw new JSException(invokeResult.ErrorMessage);
            }
        }

        /// <inheritdoc />
        public async ValueTask InvokeVoidAsync(string identifier, InvokeOption invokeOption)
        {
            invokeOption.JsRuntime = this;
            var invokeResult = await jsRuntime.InvokeAsync<InvokeResult?>(identifier, invokeOption).ConfigureAwait(false);
            if (invokeResult is not null && invokeResult.IsError && invokeResult.ErrorMessage is not null)
            {
                throw new JSException(invokeResult.ErrorMessage);
            }
        }

        private object? InvokeInternal<TValue>(string identifier, InvokeOptionWithReturnValue invokeOption, Type type)
        {
            invokeOption.ReturnValueBinding = type.GetBindingConfiguration(jsBindOptions.BindingConfigurationProvider);
            invokeOption.JsRuntime = this;
            InvokeResult<TValue>? invokeResult;
            if (jsBindOptions.UseInProcessJsRuntime)
            {
                invokeResult = ((IJSInProcessRuntime)jsRuntime).Invoke<InvokeResult<TValue>?>(identifier, invokeOption);
            }
            else
            {
                invokeResult = jsRuntime.InvokeAsync<InvokeResult<TValue>?>(identifier, invokeOption).AsTask().ConfigureAwait(false).GetAwaiter().GetResult();
            }

            if (invokeResult is not null && invokeResult.IsError && invokeResult.ErrorMessage is not null)
            {
                throw new JSException(invokeResult.ErrorMessage);
            }

            return GetInvokeResultObject(invokeResult, type);
        }

        private async ValueTask<object?> InvokeAsyncInternal<TValue>(string identifier, InvokeOptionWithReturnValue invokeOption, Type type)
        {
            invokeOption.ReturnValueBinding = type.GetBindingConfiguration(jsBindOptions.BindingConfigurationProvider);
            invokeOption.JsRuntime = this;
            var invokeResult = await jsRuntime.InvokeAsync<InvokeResult<TValue>?>(identifier, invokeOption).ConfigureAwait(false);
            if (invokeResult is not null && invokeResult.IsError && invokeResult.ErrorMessage is not null)
            {
                throw new JSException(invokeResult.ErrorMessage);
            }

            return GetInvokeResultObject(invokeResult, type);
        }

        private object? GetInvokeResultObject<TValue>(InvokeResult<TValue>? invokeResult, Type type)
        {
            var invokeResultObject = invokeResult is null ? type.GetDefaultValue() : invokeResult.Value;
            if (invokeResult is null || invokeResult.References is null)
            {
                return invokeResultObject;
            }


            foreach (var reference in invokeResult.References)
            {
                if (reference is null)
                {
                    continue;
                }

                reference.InternalInitialize(this);
            }

            return invokeResultObject;
        }
    }
}
