using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Threading.Tasks;
using JsBind.Net.Internal.Extensions;
using JsBind.Net.Internal.JsonConverters;
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
        private readonly MethodInfo invokeMethod;

        public CapturedDelegateReference(DelegateReference delegateReference, Delegate delegateObject, IJsRuntimeAdapter jsRuntime)
        {
            DelegateReference = delegateReference;
            DelegateObject = delegateObject;
            this.jsRuntime = jsRuntime;
            invokeMethod = DelegateObject.GetType().GetMethod("Invoke") ?? throw new InvalidOperationException("Delegate must have invoke method.");
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
                var returnValue = InvokeDelegateInternal(invokeArgs);
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
                var returnValue = await AsyncResultHelper.GetAsyncResultObject(InvokeDelegateInternal(invokeArgs)).ConfigureAwait(false);
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

        /// <summary>
        /// Invokes a delegate instance with the arguments.
        /// </summary>
        /// <param name="args">The arguments to invoke the delegate.</param>
        /// <returns>The result of the delegate invocation.</returns>
        private object? InvokeDelegateInternal(object?[] args)
        {
            return invokeMethod.Invoke(DelegateObject, args);
        }

        private object?[] GetInvokeArgs(DelegateInvokeWrapper? invokeWrapper)
        {
            if (invokeWrapper is null)
            {
                throw new InvalidOperationException("Delegate invoke wrapper should not be null.");
            }

            var argumentTypes = invokeMethod.GetParameters().Select(p => p.ParameterType).ToArray();
            if (invokeWrapper.Args is null || !invokeWrapper.Args.Any())
            {
                return argumentTypes.Select(argumentType => argumentType.GetDefaultValue()).ToArray();
            }


            var options = invokeWrapper.JsonSerializerOptions!;
            var cloneOptions = new JsonSerializerOptions(options);
            var references = new List<BindingBase?>();
            ConvertersFactory.AddReadConverters(options, cloneOptions, references);
            var args = invokeWrapper.Args.Cast<object?>().ToArray();
            var processedArgs = argumentTypes.Select((argumentType, index) =>
            {
                if (index < args.Length)
                {
                    // make sure the object type is matching
                    return ProcessInvokeArg(args[index], argumentType, cloneOptions);
                }

                // fill missing arguments with their default values
                return argumentType.GetDefaultValue();
            }).ToArray();

            foreach (var reference in references)
            {
                if (reference is not null)
                {
                    reference.InternalInitialize(jsRuntime);
                }
            }

            return processedArgs;
        }

        private static object? ProcessInvokeArg(object? invokeArg, Type argumentType, JsonSerializerOptions options)
        {
            if (invokeArg is JsonElement jsonElement)
            {
                var json = jsonElement.GetRawText();
                try
                {
                    return JsonSerializer.Deserialize(json, argumentType, options);
                }
                catch (JsonException jsonException)
                {
                    throw new JsonException($"Error when deserializing JSON: {json}", jsonException);
                }
            }

            return invokeArg;
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
                Result = new InvokeResultWithValue<object>()
                {
                    Value = result
                }
            };
        }

        private DelegateResultWrapper? GetErrorReturnValue(Exception exception)
        {
            return new()
            {
                JsRuntime = jsRuntime,
                Result = new InvokeResultWithValue<object>()
                {
                    IsError = true,
                    ErrorMessage = exception.Message
                }
            };
        }
    }
}
