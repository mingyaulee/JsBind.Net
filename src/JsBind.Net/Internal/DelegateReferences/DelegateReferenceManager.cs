using System;
using System.Collections.Generic;
using System.Linq;
using JsBind.Net.Configurations;
using JsBind.Net.Internal.Extensions;
using JsBind.Net.Internal.References;
using JsBind.Net.InvokeOptions;

namespace JsBind.Net.Internal.DelegateReferences
{
    /// <summary>
    /// Handles creation of delegate reference and invocation of delegate.
    /// </summary>
    internal static class DelegateReferenceManager
    {
        private static readonly Dictionary<Guid, CapturedDelegateReference> delegateReferences = new();

        /// <summary>
        /// Gets or creates an instance of <see cref="DelegateReference" /> representing the delegate.
        /// </summary>
        /// <param name="delegateObject">The delegate.</param>
        /// <param name="jsRuntime">The JS runtime adapter.</param>
        /// <returns>An instance of <see cref="DelegateReference" /> representing the delegate.</returns>
        public static DelegateReference GetOrCreateDelegateReference(Delegate delegateObject, IJsRuntimeAdapter jsRuntime)
        {
            var existingCapturedDelegateReference = GetCapturedDelegateReference(delegateObject, jsRuntime);
            if (existingCapturedDelegateReference is not null)
            {
                return existingCapturedDelegateReference.DelegateReference;
            }

            var delegateReferenceId = Guid.NewGuid();
            ProcessDelegateArgumentTypesAndReturnType(delegateObject.GetType(), out var argumentBindings, out var isAsync);
            var delegateReference = new DelegateReference(delegateReferenceId.ToString(), argumentBindings, isAsync);
            var capturedDelegateReference = new CapturedDelegateReference(delegateReference, delegateObject, jsRuntime);
            delegateReferences.Add(delegateReferenceId, capturedDelegateReference);
            return delegateReference;
        }

        /// <summary>
        /// Gets all instances of <see cref="CapturedDelegateReference" /> in the session.
        /// </summary>
        /// <param name="jsRuntime">The JS runtime instance to identify session.</param>
        /// <returns>All instances of <see cref="CapturedDelegateReference" /> in the session.</returns>
        public static IEnumerable<CapturedDelegateReference> GetCapturedDelegateReferences(IJsRuntimeAdapter jsRuntime)
        {
            return delegateReferences.Values
                .Where(delegateReference => delegateReference.JsRuntime.IsJsRuntimeEqual(jsRuntime))
                .ToList();
        }

        /// <summary>
        /// Gets an instance of <see cref="CapturedDelegateReference" /> representing the captured delegate.
        /// </summary>
        /// <param name="delegateObject">The delegate.</param>
        /// <param name="jsRuntime">The JS runtime instance to identify session.</param>
        /// <returns>An instance of <see cref="CapturedDelegateReference" /> representing the captured delegate.</returns>
        public static CapturedDelegateReference? GetCapturedDelegateReference(Delegate delegateObject, IJsRuntimeAdapter jsRuntime)
        {
            return delegateReferences.Values
                .FirstOrDefault(delegateReference =>
                    delegateReference.DelegateObject.Equals(delegateObject) &&
                    delegateReference.JsRuntime.IsJsRuntimeEqual(jsRuntime)
                );
        }

        /// <summary>
        /// Gets an instance of <see cref="CapturedDelegateReference" /> representing the captured delegate.
        /// </summary>
        /// <param name="delegateId">The delegate identifier.</param>
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
        /// Removes the captured delegate.
        /// </summary>
        /// <param name="delegateId">The delegate identifier.</param>
        public static void RemoveCapturedDelegateReference(Guid delegateId)
        {
            delegateReferences.Remove(delegateId);
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
