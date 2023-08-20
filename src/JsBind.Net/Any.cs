using System;
using System.Threading.Tasks;

namespace JsBind.Net
{
    /// <summary>
    /// JavaScript dynamic binding class.
    /// </summary>
    public class Any : ObjectBindingBase
    {
        /// <summary>
        /// Gets a dynamic member of the current object.
        /// </summary>
        /// <param name="memberName">The member name.</param>
        /// <returns>An instance of <see cref="Any" /> representing the member value.</returns>
        public Any this[string memberName]
        {
            get
            {
                var dynamicTypeObject = new Any();
                if (AccessPath is null)
                {
                    throw new InvalidOperationException("Access path should not be null.");
                }
                dynamicTypeObject.SetAccessPath(AccessPaths.Combine(AccessPath, memberName)!);

                var jsRuntime = InternalGetJsRuntime();
                if (jsRuntime is not null)
                {
                    dynamicTypeObject.Initialize(jsRuntime);
                }

                return dynamicTypeObject;
            }
        }

        /// <summary>
        /// Gets the value of this object in the specified type synchronously.
        /// </summary>
        /// <returns>An instance of <typeparamref name="TValue" /> obtained by JSON-deserializing the return value.</returns>
        public TValue? GetValue<TValue>()
            => ConvertToType<TValue>();

        /// <summary>
        /// Gets the value of this object in the specified type asynchronously.
        /// </summary>
        /// <returns>An instance of <typeparamref name="TValue" /> obtained by JSON-deserializing the return value.</returns>
        public ValueTask<TValue?> GetValueAsync<TValue>()
            => ConvertToTypeAsync<TValue>();

        /// <summary>
        /// Gets the property value from the object synchronously.
        /// </summary>
        /// <param name="propertyName">The property name to get.</param>
        /// <returns>An instance of <typeparamref name="TValue" /> obtained by JSON-deserializing the return value.</returns>
        public TValue? GetPropertyValue<TValue>(string? propertyName)
            => GetProperty<TValue>(propertyName);

        /// <summary>
        /// Gets the property value from the object asynchronously.
        /// </summary>
        /// <param name="propertyName">The property name to get.</param>
        /// <returns>An instance of <typeparamref name="TValue" /> obtained by JSON-deserializing the return value.</returns>
        public ValueTask<TValue?> GetPropertyValueAsync<TValue>(string? propertyName)
            => GetPropertyAsync<TValue>(propertyName);

        /// <summary>
        /// Sets the property value of the object synchronously.
        /// </summary>
        /// <param name="propertyName">The property name to set.</param>
        /// <param name="propertyValue">The property value to set.</param>
        public void SetPropertyValue(string? propertyName, object? propertyValue)
            => SetProperty(propertyName, propertyValue);

        /// <summary>
        /// Sets the property value of the object asynchronously.
        /// </summary>
        /// <param name="propertyName">The property name to set.</param>
        /// <param name="propertyValue">The property value to set.</param>
        /// <returns>A <see cref="ValueTask" /> that represents the asynchronous invocation operation.</returns>
        public ValueTask SetPropertyValueAsync(string? propertyName, object? propertyValue)
            => SetPropertyAsync(propertyName, propertyValue);

        /// <summary>
        /// Invokes the specified JavaScript function synchronously.
        /// </summary>
        /// <param name="functionName">The function name to invoke.</param>
        /// <param name="args">JSON-serializable arguments.</param>
        /// <returns>An instance of <typeparamref name="TValue" /> obtained by JSON-deserializing the return value.</returns>
        public TValue? InvokeFunction<TValue>(string? functionName, params object?[] args)
            => Invoke<TValue>(functionName, args);

        /// <summary>
        /// Invokes the specified JavaScript function asynchronously.
        /// </summary>
        /// <param name="functionName">The function name to invoke.</param>
        /// <param name="args">JSON-serializable arguments.</param>
        /// <returns>An instance of <typeparamref name="TValue" /> obtained by JSON-deserializing the return value.</returns>
        public ValueTask<TValue?> InvokeFunctionAsync<TValue>(string? functionName, params object?[] args)
            => InvokeAsync<TValue>(functionName, args);

        /// <summary>
        /// Invokes the specified JavaScript function synchronously.
        /// </summary>
        /// <param name="functionName">The function name to invoke.</param>
        /// <param name="args">JSON-serializable arguments.</param>
        public void InvokeFunctionVoid(string? functionName, params object?[] args)
            => InvokeVoid(functionName, args);

        /// <summary>
        /// Invokes the specified JavaScript function asynchronously.
        /// </summary>
        /// <param name="functionName">The function name to invoke.</param>
        /// <param name="args">JSON-serializable arguments.</param>
        /// <returns>A <see cref="ValueTask" /> that represents the asynchronous invocation operation.</returns>
        public ValueTask InvokeFunctionVoidAsync(string? functionName, params object?[] args)
            => InvokeVoidAsync(functionName, args);

        /// <summary>
        /// Converts an object binding to a dynamic binding object of type <see cref="Any" />.
        /// </summary>
        /// <param name="obj">The object binding.</param>
        /// <returns>The dynamic binding object.</returns>
        public static Any From(ObjectBindingBase obj)
        {
            var accessPath = obj.InternalGetAccessPath() ??
                throw new InvalidOperationException("Object access path should not be null.");

            var dynamicTypeObject = new Any();
            dynamicTypeObject.SetAccessPath(accessPath);
            
            var jsRuntime = obj.InternalGetJsRuntime();
            if (jsRuntime is not null)
            {
                dynamicTypeObject.Initialize(jsRuntime);
            }

            return dynamicTypeObject;
        }

        /// <summary>
        /// Creates a dynamic binding object of type <see cref="Any" /> with the specified access path.
        /// </summary>
        /// <param name="accessPath">The access path.</param>
        /// <returns>The dynamic binding object.</returns>
        public static Any From(string? accessPath)
        {
            return From(accessPath, null);
        }

        /// <summary>
        /// Creates a dynamic binding object of type <see cref="Any" /> with the specified access path and JS runtime.
        /// </summary>
        /// <param name="accessPath">The access path.</param>
        /// <param name="jsRuntime">The JS runtime.</param>
        /// <returns>The dynamic binding object.</returns>
        public static Any From(string? accessPath, IJsRuntimeAdapter? jsRuntime)
        {
            ArgumentNullException.ThrowIfNull(accessPath);

            var dynamicTypeObject = new Any();
            dynamicTypeObject.SetAccessPath(accessPath);
            
            if (jsRuntime is not null)
            {
                dynamicTypeObject.Initialize(jsRuntime);
            }
            
            return dynamicTypeObject;
        }
    }
}
