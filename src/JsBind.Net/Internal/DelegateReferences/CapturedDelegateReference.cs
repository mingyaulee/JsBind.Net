using System;
using System.Threading.Tasks;
using JsBind.Net.Internal.References;
using JsBind.Net.InvokeOptions;
using Microsoft.JSInterop;

namespace JsBind.Net.Internal.DelegateReferences
{
    /// <summary>
    /// Contains the delegate object and the delegate reference and is passed to JavaScript as <see cref="DotNetObjectReference" /> to be invoked from JavaScript.
    /// </summary>
    internal class CapturedDelegateReference : IAsyncDisposable
    {
        private readonly IJsRuntimeAdapter jsRuntime;

        public CapturedDelegateReference(DelegateReference delegateReference, Delegate delegateObject, IJsRuntimeAdapter jsRuntime)
        {
            DelegateReference = delegateReference;
            DelegateObject = delegateObject;
            this.jsRuntime = jsRuntime;
        }

        public DelegateReference DelegateReference { get; }
        public Delegate DelegateObject { get; }

        /// <summary>
        /// Invoke a delegate reference from JavaScript.
        /// </summary>
        /// <param name="invokeWrapper">Contains the arguments to invoke the delegate.</param>
        /// <returns>The result of the delegate invocation.</returns>
        [JSInvokable("InvokeDelegateFromJs")]
        public object? InvokeDelegateFromJs(DelegateInvokeWrapper? invokeWrapper)
        {
            try
            {
                var invokeArgs = GetInvokeArgs(invokeWrapper);
                var returnValue = DelegateReferenceManager.InvokeDelegateFromJs(this, invokeArgs);
                return GetReturnValue(returnValue);
            }
            catch (Exception exception)
            {
                return GetErrorReturnValue(exception);
            }
        }

        /// <summary>
        /// Invoke a delegate reference from JavaScript.
        /// </summary>
        /// <param name="invokeWrapper">Contains the arguments to invoke the delegate.</param>
        /// <returns>The result of the delegate invocation.</returns>
        [JSInvokable("InvokeDelegateFromJsAsync")]
        public async ValueTask<object?> InvokeDelegateFromJsAsync(DelegateInvokeWrapper? invokeWrapper)
        {
            try
            {
                var invokeArgs = GetInvokeArgs(invokeWrapper);
                var returnValue = await DelegateReferenceManager.InvokeDelegateFromJsAsync(this, invokeArgs).ConfigureAwait(false);
                return GetReturnValue(returnValue);
            }
            catch (Exception exception)
            {
                return GetErrorReturnValue(exception);
            }
        }

        public async ValueTask DisposeAsync()
        {
            DelegateReference.DelegateInvoker?.Dispose();
            await jsRuntime.InvokeVoidAsync(DisposeDelegateOption.Identifier, new DisposeDelegateOption(DelegateReference.DelegateId)).ConfigureAwait(false);
        }

        private object?[]? GetInvokeArgs(DelegateInvokeWrapper? invokeWrapper)
        {
            if (invokeWrapper?.References is null)
            {
                return invokeWrapper?.Args;
            }

            foreach (var reference in invokeWrapper.References)
            {
                if (reference is null)
                {
                    continue;
                }

                reference.InternalInitialize(jsRuntime);
            }

            return invokeWrapper.Args;
        }

        private DelegateResultWrapper? GetReturnValue(object? result)
        {
            if (result is null)
            {
                return null;
            }

            return new()
            {
                JsRuntime = jsRuntime,
                Result = new
                {
                    isError = false,
                    value = result,
                }
            };
        }

        private DelegateResultWrapper? GetErrorReturnValue(Exception exception)
        {
            return new()
            {
                JsRuntime = jsRuntime,
                Result = new
                {
                    isError = true,
                    errorMessage = exception.Message
                }
            };
        }
    }
}
