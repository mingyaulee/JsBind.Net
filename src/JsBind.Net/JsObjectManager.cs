using System;
using System.Collections;
using System.Linq;
using System.Threading.Tasks;
using JsBind.Net.Internal.DelegateReferences;
using JsBind.Net.InvokeOptions;

namespace JsBind.Net
{
    /// <summary>
    /// JavaScript object manager.
    /// </summary>
    public static class JsObjectManager
    {
        /// <summary>
        /// Disposes the object reference in JavaScript synchronously, if any.
        /// </summary>
        /// <param name="obj">The object reference to dispose.</param>
        public static void DisposeObjectReference(object? obj)
        {
            if (obj is BindingBase bindingBase)
            {
                bindingBase.Dispose();
            }
            else if (obj is IEnumerable enumerable)
            {
                var arrayItem = enumerable.OfType<BindingBase>()
                    .FirstOrDefault(item => item.InternalGetAccessPath() is not null);
                if (IsArrayItemRootObjectReference(arrayItem) && IsDisposableRootObjectReference(arrayItem, out var jsRuntime, out var referenceId))
                {
                    jsRuntime!.InvokeVoid(DisposeObjectOption.Identifier, new DisposeObjectOption(referenceId));
                }
            }
        }

        /// <summary>
        /// Disposes the object reference in JavaScript asynchronously, if any.
        /// </summary>
        /// <param name="obj">The object reference to dispose.</param>
        /// <returns>A <see cref="ValueTask" /> that represents the asynchronous dispose operation.</returns>
        public static async ValueTask DisposeObjectReferenceAsync(object? obj)
        {
            if (obj is BindingBase bindingBase)
            {
                await bindingBase.DisposeAsync().ConfigureAwait(false);
            }
            else if (obj is IEnumerable enumerable)
            {
                var arrayItem = enumerable.OfType<BindingBase>()
                    .FirstOrDefault(item => item.InternalGetAccessPath() is not null);
                if (IsArrayItemRootObjectReference(arrayItem) && IsDisposableRootObjectReference(arrayItem, out var jsRuntime, out var referenceId))
                {
                    await jsRuntime!.InvokeVoidAsync(DisposeObjectOption.Identifier, new DisposeObjectOption(referenceId)).ConfigureAwait(false);
                }
            }
        }

        /// <summary>
        /// Disposes the root object reference in JavaScript synchronously, if any.
        /// </summary>
        /// <param name="obj">The object reference to dispose.</param>
        public static void DisposeRootObjectReference(object? obj)
        {
            if (obj is BindingBase bindingBase && IsDisposableRootObjectReference(bindingBase, out var jsRuntime, out var referenceId))
            {
                jsRuntime!.InvokeVoid(DisposeObjectOption.Identifier, new DisposeObjectOption(referenceId));
            }
            else if (obj is IEnumerable enumerable)
            {
                var arrayItem = enumerable.Cast<object>().FirstOrDefault();
                DisposeRootObjectReference(arrayItem);
            }
        }

        /// <summary>
        /// Disposes the root object reference in JavaScript asynchronously, if any.
        /// </summary>
        /// <param name="obj">The object reference to dispose.</param>
        /// <returns>A <see cref="ValueTask" /> that represents the asynchronous dispose operation.</returns>
        public static async ValueTask DisposeRootObjectReferenceAsync(object? obj)
        {
            if (obj is BindingBase bindingBase && IsDisposableRootObjectReference(bindingBase, out var jsRuntime, out var referenceId))
            {
                await jsRuntime!.InvokeVoidAsync(DisposeObjectOption.Identifier, new DisposeObjectOption(referenceId)).ConfigureAwait(false);
            }
            else if (obj is IEnumerable enumerable)
            {
                var arrayItem = enumerable.Cast<object>().FirstOrDefault();
                await DisposeRootObjectReferenceAsync(arrayItem).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Disposes the delegate reference in JavaScript synchronously, if any.
        /// </summary>
        /// <param name="delegateObject">The delegate reference to dispose.</param>
        /// <param name="jsRuntime">The JS runtime instance to identify session.</param>
        public static void DisposeDelegateReference(Delegate delegateObject, IJsRuntimeAdapter jsRuntime)
        {
            if (TryDisposeDelegateReferenceInDotNet(delegateObject, jsRuntime, out var capturedDelegateReference))
            {
                capturedDelegateReference!.Dispose();
            }
        }

        /// <summary>
        /// Disposes the delegate reference in JavaScript asynchronously, if any.
        /// </summary>
        /// <param name="delegateObject">The delegate reference to dispose.</param>
        /// <param name="jsRuntime">The JS runtime instance to identify session.</param>
        /// <returns>A <see cref="ValueTask" /> that represents the asynchronous dispose operation.</returns>
        public static async ValueTask DisposeDelegateReferenceAsync(Delegate delegateObject, IJsRuntimeAdapter jsRuntime)
        {
            if (TryDisposeDelegateReferenceInDotNet(delegateObject, jsRuntime, out var capturedDelegateReference))
            {
                await capturedDelegateReference!.DisposeAsync().ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Disposes all references in the session.
        /// </summary>
        /// <param name="jsRuntime">The JS runtime instance to identify session.</param>
        /// <param name="disposeJsReferences">If <c>true</c>, disposes all references in JavaScript.</param>
        public static void DisposeSession(IJsRuntimeAdapter jsRuntime, bool disposeJsReferences = false)
        {
            DisposeSessionDelegateReferencesInDotNet(jsRuntime);

            if (disposeJsReferences)
            {
                jsRuntime.InvokeVoid(DisposeSessionOption.Identifier, new DisposeSessionOption());
            }
        }

        /// <summary>
        /// Disposes all references in the session.
        /// </summary>
        /// <param name="jsRuntime">The JS runtime instance to identify session.</param>
        /// <param name="disposeJsReferences">If <c>true</c>, disposes all references in JavaScript.</param>
        /// <returns>A <see cref="ValueTask" /> that represents the asynchronous dispose operation.</returns>
        public static async ValueTask DisposeSessionAsync(IJsRuntimeAdapter jsRuntime, bool disposeJsReferences = false)
        {
            DisposeSessionDelegateReferencesInDotNet(jsRuntime);

            if (disposeJsReferences)
            {
                await jsRuntime.InvokeVoidAsync(DisposeSessionOption.Identifier, new DisposeSessionOption()).ConfigureAwait(false);
            }
        }

        private static bool IsArrayItemRootObjectReference(BindingBase? arrayItem)
        {
            return AccessPaths.Split(arrayItem?.InternalGetAccessPath())?.Length == 2;
        }

        private static bool IsDisposableRootObjectReference(BindingBase? bindingBase, out IJsRuntimeAdapter? jsRuntime, out string? referenceId)
        {
            jsRuntime = bindingBase?.InternalGetJsRuntime();
            referenceId = null;
            if (bindingBase is null || jsRuntime is null)
            {
                return false;
            }

            var rootAccessPath = AccessPaths.Split(bindingBase.InternalGetAccessPath())?.FirstOrDefault();
            if (rootAccessPath is null || !AccessPaths.IsReferenceId(rootAccessPath))
            {
                return false;
            }

            referenceId = AccessPaths.GetReferenceId(rootAccessPath).ToString();
            return true;
        }

        private static bool TryDisposeDelegateReferenceInDotNet(Delegate delegateObject, IJsRuntimeAdapter jsRuntime, out CapturedDelegateReference? capturedDelegateReference)
        {
            capturedDelegateReference = DelegateReferenceManager.GetCapturedDelegateReference(delegateObject, jsRuntime);
            if (capturedDelegateReference is null)
            {
                return false;
            }

            if (Guid.TryParse(capturedDelegateReference.DelegateReference.DelegateId, out var delegateId))
            {
                DelegateReferenceManager.RemoveCapturedDelegateReference(delegateId);
            }

            return true;
        }

        private static void DisposeSessionDelegateReferencesInDotNet(IJsRuntimeAdapter jsRuntime)
        {
            var capturedDelegateReferences = DelegateReferenceManager.GetCapturedDelegateReferences(jsRuntime);
            foreach (var capturedDelegateReference in capturedDelegateReferences)
            {
                if (Guid.TryParse(capturedDelegateReference.DelegateReference.DelegateId, out var delegateId))
                {
                    DelegateReferenceManager.RemoveCapturedDelegateReference(delegateId);
                }
            }
        }
    }
}
