﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JsBind.Net.Configurations;
using JsBind.Net.Internal.Extensions;
using JsBind.Net.Internal.References;
using JsBind.Net.InvokeOptions;
using Microsoft.JSInterop;

namespace JsBind.Net.Internal.DelegateReferences
{
    /// <summary>
    /// Handles creation of delegate reference and invocation of delegate.
    /// </summary>
    internal static class DelegateReferenceManager
    {
        private static readonly Dictionary<Guid, CapturedDelegateReference> delegateReferences = new();

        /// <summary>
        /// Invoke a delegate from JavaScript.
        /// </summary>
        /// <param name="capturedDelegateReference">The delegate.</param>
        /// <param name="args">The arguments to invoke the delegate.</param>
        /// <returns>The result of the delegate invocation.</returns>
        public static object? InvokeDelegateFromJs(CapturedDelegateReference capturedDelegateReference, object?[]? args)
        {
            return InvokeDelegateInternal(capturedDelegateReference.DelegateObject, args);
        }

        /// <summary>
        /// Invoke a delegate from JavaScript asynchronously.
        /// </summary>
        /// <param name="capturedDelegateReference">The delegate.</param>
        /// <param name="args">The arguments to invoke the delegate.</param>
        /// <returns>The result of the delegate invocation.</returns>
        public static ValueTask<object?> InvokeDelegateFromJsAsync(CapturedDelegateReference capturedDelegateReference, object?[]? args)
        {
            var result = InvokeDelegateInternal(capturedDelegateReference.DelegateObject, args);
            return AsyncResultHelper.GetAsyncResultObject(result);
        }

        /// <summary>
        /// Gets an instance of <see cref="DelegateReference" /> representing the delegate.
        /// </summary>
        /// <param name="delegateObject">The delegate.</param>
        /// <param name="jsRuntime">The JS runtime adapter.</param>
        /// <returns>An instance of <see cref="DelegateReference" /> representing the delegate.</returns>
        public static DelegateReference GetDelegateReference(Delegate delegateObject, IJsRuntimeAdapter jsRuntime)
        {
            var existingCapturedDelegateReference = delegateReferences.Values.FirstOrDefault(delegateReference => delegateReference.DelegateObject.Equals(delegateObject));
            if (existingCapturedDelegateReference is not null)
            {
                return existingCapturedDelegateReference.DelegateReference;
            }
            else
            {
                var delegateReferenceId = Guid.NewGuid();
                ProcessDelegateArgumentTypesAndReturnType(delegateObject.GetType(), out var argumentBindings, out var isAsync);
                var delegateReference = new DelegateReference(delegateReferenceId.ToString(), argumentBindings, isAsync);
                var capturedDelegateReference = new CapturedDelegateReference(delegateReference, delegateObject, jsRuntime);
                delegateReference.DelegateInvoker = DotNetObjectReference.Create(capturedDelegateReference);
                delegateReferences.Add(delegateReferenceId, capturedDelegateReference);
                return delegateReference;
            }
        }

        /// <summary>
        /// Gets an instance of <see cref="CapturedDelegateReference" /> representing the captured delegate.
        /// </summary>
        /// <param name="delegateId">The delegate.</param>
        /// <returns>An instance of <see cref="CapturedDelegateReference" /> representing the captured delegate.</returns>
        public static CapturedDelegateReference GetCapturedDelegateReference(Guid delegateId)
        {
            if (delegateReferences.TryGetValue(delegateId, out var capturedDelegateReference))
            {
                return capturedDelegateReference;
            }

            throw new InvalidOperationException($"Delegate with ID '{delegateId}' does not exist.");
        }

        /// <summary>
        /// Invokes a delegate instance with the arguments.
        /// </summary>
        /// <param name="delegateInstance">The delegate.</param>
        /// <param name="args">The arguments to invoke the delegate.</param>
        /// <returns>The result of the delegate invocation.</returns>
        private static object? InvokeDelegateInternal(Delegate delegateInstance, object?[]? args)
        {
            var invokeMethod = delegateInstance.GetType().GetMethod("Invoke");
            if (invokeMethod is null)
            {
                throw new InvalidOperationException("Delegate must have invoke method.");
            }

            var argumentTypes = invokeMethod.GetParameters().Select(p => p.ParameterType).ToArray();
            object?[] processedArgs;
            if (args is null)
            {
                processedArgs = Array.Empty<object>();
            }
            else
            {
                processedArgs = args;
                if (args.Length < argumentTypes.Length)
                {
                    processedArgs = argumentTypes.Select((argumentType, index) =>
                    {
                        if (index < args.Length)
                        {
                            return args[index];
                        }
                        return argumentType.GetDefaultValue();
                    }).ToArray();
                }
            }
            return invokeMethod.Invoke(delegateInstance, processedArgs);
        }

        private static void ProcessDelegateArgumentTypesAndReturnType(Type delegateType, out IEnumerable<ObjectBindingConfiguration?>? argumentBindings, out bool isAsync)
        {
            var jsBindOptions = IJsBindOptions.Instance;
            isAsync = !jsBindOptions.UseInProcessJsRuntime;
            argumentBindings = null;

            if (!delegateType.IsGenericType)
            {
                return;
            }

            var argumentTypes = delegateType.GetGenericArguments().AsEnumerable();
            var isAction = delegateType.GetGenericTypeDefinition().Name.StartsWith(nameof(Action));
            if (!isAction)
            {
                if (!argumentTypes.Any())
                {
                    throw new InvalidOperationException("Delegate type without any generic type argument is invalid.");
                }
                var returnType = argumentTypes.Last();
                if (!isAsync && returnType.IsTaskOrValueTask())
                {
                    isAsync = true;
                }
                argumentTypes = argumentTypes.Take(argumentTypes.Count() - 1);
            }

            argumentBindings = argumentTypes.Select(argumentType => argumentType.GetBindingConfiguration(jsBindOptions.BindingConfigurationProvider)).ToList();
        }
    }
}
