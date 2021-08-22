using System;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using JsBind.Net.InvokeOptions;

namespace JsBind.Net
{
    /// <summary>
    /// Base JavaScript binding class.
    /// </summary>
    public abstract class BindingBase : IAsyncDisposable
    {
        private bool isInitialized;
        private IJsRuntimeAdapter? jsRuntime;
        private string? accessPath;

        /// <summary>
        /// Do not use this property. It is public for deserialization only.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("__jsBindAccessPath")]
        [Obsolete("Do not use this property. It is public for deserialization only.")]
        public string? __JsBindAccessPath { get => null; private set => accessPath = value; }

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
        protected virtual string AccessPath => accessPath ?? throw new InvalidOperationException("Object access path is not set.");

        internal void InternalInitialize(IJsRuntimeAdapter jsRuntime)
        {
            Initialize(jsRuntime);
        }

        internal string? InternalGetAccessPath() => accessPath;

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
            isInitialized = true;
            this.jsRuntime = jsRuntime;
        }

        /// <summary>
        /// Disposes this instance of object.
        /// </summary>
        /// <returns>A <see cref="ValueTask" /> that represents the asynchronous invocation operation.</returns>
        public async ValueTask DisposeAsync()
        {
            if (!string.IsNullOrEmpty(accessPath) && jsRuntime != null)
            {
                if (AccessPaths.IsReferenceId(accessPath))
                {
                    var referenceId = AccessPaths.GetReferenceId(accessPath) ?? throw new InvalidOperationException($"Reference ID in access path '{accessPath}' is invalid.");
                    await jsRuntime.InvokeVoidAsync(DisposeObjectOption.Identifier, new DisposeObjectOption(referenceId.ToString())).ConfigureAwait(false);
                }
                jsRuntime = null;
                accessPath = null;
            }
        }
    }

    /// <summary>
    /// Base JavaScript binding class with equality comparer.
    /// </summary>
    public abstract class BindingBase<T> : BindingBase, IEquatable<T>
        where T : BindingBase
    {
        /// <inheritdoc />
        public override bool Equals(object? obj)
        {
            return Equals(obj as T);
        }

        /// <inheritdoc />
        public virtual bool Equals(T? other)
        {
            return Equals(this as T, other);
        }

        /// <summary>
        /// Determines whether the current object is the same instance as the other object in JavaScript.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns><c>true</c> if the current object is the same instance as the other object in JavaScript.</returns>
        public virtual bool InstanceEquals(T? other)
        {
            if (Equals(this as T, other))
            {
                return true;
            }

            return JsRuntime.Invoke<bool>(CompareObjectOption.Identifier, new CompareObjectOption(InternalGetAccessPath(), other?.InternalGetAccessPath()));
        }

        /// <summary>
        /// Determines whether the current object is the same instance as the other object in JavaScript.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns><c>true</c> if the current object is the same instance as the other object in JavaScript.</returns>
        public virtual async ValueTask<bool> InstanceEqualsAsync(T? other)
        {
            if (Equals(this as T, other))
            {
                return true;
            }

            return await JsRuntime.InvokeAsync<bool>(CompareObjectOption.Identifier, new CompareObjectOption(InternalGetAccessPath(), other?.InternalGetAccessPath())).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return GetHashCode(this as T);
        }

        private static bool Equals(T? x, T? y)
        {
            return x?.InternalGetAccessPath() == y?.InternalGetAccessPath();
        }

        private static int GetHashCode(T? obj)
        {
            return obj?.InternalGetAccessPath()?.GetHashCode() ?? "globalThis".GetHashCode();
        }
    }
}
