using System;
using System.ComponentModel;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using JsBind.Net.InvokeOptions;

namespace JsBind.Net
{
    /// <summary>
    /// Base JavaScript binding class.
    /// </summary>
    public abstract class BindingBase : IDisposable, IAsyncDisposable
    {
        private bool isInitialized;
        private IJsRuntimeAdapter? jsRuntime;
        private string? accessPath;

        /// <summary>
        /// Do not use this property. It is public for deserialization only.
        /// </summary>
        [JsonPropertyName("__jsBindAccessPath")]
        [Obsolete("Do not use this property. It is public for deserialization only.")]
        [EditorBrowsable(EditorBrowsableState.Never)]
#pragma warning disable S2376 // Write-only properties should not be used
        public string? __JsBindAccessPath { set => accessPath = value; }
#pragma warning restore S2376 // Write-only properties should not be used

        /// <summary>
        /// Do not use this property. It is public for deserialization only.
        /// </summary>
        [JsonPropertyName("__jsBindJsRuntime")]
        [Obsolete("Do not use this property. It is public for deserialization only.")]
        [EditorBrowsable(EditorBrowsableState.Never)]
#pragma warning disable S2376 // Write-only properties should not be used
        public IJsRuntimeAdapter? __JsBindJsRuntime { set => Initialize(value); }
#pragma warning restore S2376 // Write-only properties should not be used

        /// <summary>
        /// Checks whether the binding instance has been initialized.
        /// </summary>
        protected virtual bool IsInitialized => isInitialized;

        /// <summary>
        /// Gets the JS runtime adapter instance.
        /// </summary>
        protected IJsRuntimeAdapter JsRuntime => jsRuntime ?? throw new InvalidOperationException("Object is not initialized with JsRuntime.");

        /// <summary>
        /// Gets the access path of the binding instance.
        /// </summary>
        protected virtual string? AccessPath => accessPath;


        internal string? InternalGetAccessPath() => accessPath;
        internal IJsRuntimeAdapter? InternalGetJsRuntime() => jsRuntime;

        /// <summary>
        /// Sets the access path of the binding instance.
        /// </summary>
        /// <param name="accessPath">The access path.</param>
        protected virtual void SetAccessPath(string accessPath)
        {
            this.accessPath = accessPath;
        }

        /// <summary>
        /// Initialized the binding instance with JS runtime adapter.
        /// </summary>
        /// <param name="jsRuntime">The JS runtime adapter.</param>
        protected virtual void Initialize(IJsRuntimeAdapter? jsRuntime)
        {
            if (jsRuntime is not null)
            {
                isInitialized = true;
                this.jsRuntime = jsRuntime;
            }
        }

        #region Dispose methods

        /// <summary>
        /// Disposes this instance of object in JavaScript synchronously, if any.
        /// </summary>
        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Disposes this instance of object in JavaScript asynchronously, if any.
        /// </summary>
        /// <returns>A <see cref="ValueTask" /> that represents the asynchronous dispose operation.</returns>
        public async ValueTask DisposeAsync()
        {
            await DisposeAsyncCore();

            Dispose(disposing: false);
#pragma warning disable CA1816 // Dispose methods should call SuppressFinalize
            GC.SuppressFinalize(this);
#pragma warning restore CA1816 // Dispose methods should call SuppressFinalize
        }

        /// <summary>
        /// Finalizer to dispose JavaScript reference, if any.
        /// </summary>
        ~BindingBase()
        {
            Dispose(disposing: false);
        }

        /// <summary>
        /// Disposes this instance of object in JavaScript, if any.
        /// </summary>
        /// <param name="disposing">Disposing from <see cref="IDisposable.Dispose" />.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!string.IsNullOrEmpty(accessPath) && jsRuntime != null && AccessPaths.IsReferenceId(accessPath))
            {
                var referenceId = AccessPaths.GetReferenceId(accessPath) ?? throw new InvalidOperationException($"Reference ID in access path '{accessPath}' is invalid.");
                if (disposing)
                {
                    jsRuntime.InvokeVoid(DisposeObjectOption.Identifier, new DisposeObjectOption(referenceId.ToString()));
                }
                else
                {
#pragma warning disable CA2012 // Use ValueTasks correctly
                    jsRuntime.InvokeVoidAsync(DisposeObjectOption.Identifier, new DisposeObjectOption(referenceId.ToString()));
#pragma warning restore CA2012 // Use ValueTasks correctly
                }
            }

            jsRuntime = null;
            accessPath = null;
        }

        /// <summary>
        /// Disposes this instance of object in JavaScript asynchronously, if any.
        /// </summary>
        /// <returns>A <see cref="ValueTask" /> that represents the asynchronous dispose operation.</returns>
        protected virtual async ValueTask DisposeAsyncCore()
        {
            if (!string.IsNullOrEmpty(accessPath) && jsRuntime != null && AccessPaths.IsReferenceId(accessPath))
            {
                var referenceId = AccessPaths.GetReferenceId(accessPath) ?? throw new InvalidOperationException($"Reference ID in access path '{accessPath}' is invalid.");
                await jsRuntime.InvokeVoidAsync(DisposeObjectOption.Identifier, new DisposeObjectOption(referenceId.ToString())).ConfigureAwait(false);
            }

            jsRuntime = null;
            accessPath = null;
        }

        #endregion Dispose methods

        #region Equality check methods

        /// <inheritdoc />
        public override bool Equals(object? obj)
        {
            return Equals(this, obj as BindingBase);
        }

        /// <summary>
        /// Determines whether the current object is the same instance as the other object in JavaScript.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns><c>true</c> if the current object is the same instance as the other object in JavaScript.</returns>
        public virtual bool InstanceEquals(object? other)
        {
            var otherBindingBase = other as BindingBase;
            if (Equals(this, otherBindingBase))
            {
                return true;
            }

            return GetJsRuntimeToCompare(otherBindingBase).Invoke<bool>(CompareObjectOption.Identifier, new CompareObjectOption(InternalGetAccessPath(), otherBindingBase?.InternalGetAccessPath()));
        }

        /// <summary>
        /// Determines whether the current object is the same instance as the other object in JavaScript.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns><c>true</c> if the current object is the same instance as the other object in JavaScript.</returns>
        public virtual async ValueTask<bool> InstanceEqualsAsync(object? other)
        {
            var otherBindingBase = other as BindingBase;
            if (Equals(this, otherBindingBase))
            {
                return true;
            }            

            return await GetJsRuntimeToCompare(otherBindingBase).InvokeAsync<bool>(CompareObjectOption.Identifier, new CompareObjectOption(InternalGetAccessPath(), otherBindingBase?.InternalGetAccessPath())).ConfigureAwait(false);
        }

        private IJsRuntimeAdapter GetJsRuntimeToCompare(BindingBase? other)
        {
            if (isInitialized)
            {
                return JsRuntime;
            }
            else if (other is not null && other.isInitialized)
            {
                return other.JsRuntime;
            }
            else
            {
                throw new InvalidOperationException("Unable to get JsRuntime from either one of the objects to compare equality.");
            }
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return GetHashCode(this);
        }

        private static bool Equals(BindingBase? x, BindingBase? y)
        {
            return x?.InternalGetAccessPath() == y?.InternalGetAccessPath();
        }

        private static int GetHashCode(BindingBase? obj)
        {
            return obj?.InternalGetAccessPath()?.GetHashCode() ?? "globalThis".GetHashCode();
        }

        #endregion Equality check methods
    }
}
