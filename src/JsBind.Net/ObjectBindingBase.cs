﻿using System.Collections.Generic;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using JsBind.Net.InvokeOptions;

namespace JsBind.Net
{
    /// <summary>
    /// Base JavaScript object binding.
    /// </summary>
    public abstract class ObjectBindingBase : BindingBase
    {
        /// <summary>
        /// Contains any additional data within this object instance.
        /// </summary>
        [JsonExtensionData]
        public IDictionary<string, object>? AdditionalData { get; set; }

        /// <summary>
        /// Gets the property from the object synchronously.
        /// </summary>
        /// <param name="propertyName">The property name to get.</param>
        /// <returns>An instance of <typeparamref name="TValue" /> obtained by JSON-deserializing the return value.</returns>
        protected TValue? GetProperty<TValue>(string? propertyName)
            => JsRuntime.Invoke<TValue>(GetPropertyOption.Identifier, new GetPropertyOption(InternalGetAccessPath(), propertyName));

        /// <summary>
        /// Gets the property from the object asynchronously.
        /// </summary>
        /// <param name="propertyName">The property name to get.</param>
        /// <returns>An instance of <typeparamref name="TValue" /> obtained by JSON-deserializing the return value.</returns>
        protected ValueTask<TValue?> GetPropertyAsync<TValue>(string? propertyName)
            => JsRuntime.InvokeAsync<TValue>(GetPropertyOption.Identifier, new GetPropertyOption(InternalGetAccessPath(), propertyName));

        /// <summary>
        /// Sets the property value of the object synchronously.
        /// </summary>
        /// <param name="propertyName">The property name to set.</param>
        /// <param name="propertyValue">The property value to set.</param>
        protected void SetProperty(string? propertyName, object? propertyValue)
            => JsRuntime.InvokeVoid(SetPropertyOption.Identifier, new SetPropertyOption(InternalGetAccessPath(), propertyName, propertyValue));

        /// <summary>
        /// Sets the property value of the object asynchronously.
        /// </summary>
        /// <param name="propertyName">The property name to set.</param>
        /// <param name="propertyValue">The property value to set.</param>
        /// <returns>A <see cref="ValueTask" /> that represents the asynchronous invocation operation.</returns>
        protected ValueTask SetPropertyAsync(string? propertyName, object? propertyValue)
            => JsRuntime.InvokeVoidAsync(SetPropertyOption.Identifier, new SetPropertyOption(InternalGetAccessPath(), propertyName, propertyValue));

        /// <summary>
        /// Invokes the specified JavaScript function synchronously.
        /// </summary>
        /// <param name="functionName">The function name to invoke.</param>
        /// <param name="args">JSON-serializable arguments.</param>
        /// <returns>An instance of <typeparamref name="TValue" /> obtained by JSON-deserializing the return value.</returns>
        protected TValue? Invoke<TValue>(string? functionName, params object?[] args)
            => JsRuntime.Invoke<TValue>(InvokeFunctionOption.Identifier, new InvokeFunctionOption<TValue>(InternalGetAccessPath(), functionName, args));

        /// <summary>
        /// Invokes the specified JavaScript function asynchronously.
        /// </summary>
        /// <param name="functionName">The function name to invoke.</param>
        /// <param name="args">JSON-serializable arguments.</param>
        /// <returns>An instance of <typeparamref name="TValue" /> obtained by JSON-deserializing the return value.</returns>
        protected ValueTask<TValue?> InvokeAsync<TValue>(string? functionName, params object?[] args)
            => JsRuntime.InvokeAsync<TValue>(InvokeFunctionOption.AsyncIdentifier, new InvokeFunctionOption<TValue>(InternalGetAccessPath(), functionName, args));

        /// <summary>
        /// Invokes the specified JavaScript function synchronously.
        /// </summary>
        /// <param name="functionName">The function name to invoke.</param>
        /// <param name="args">JSON-serializable arguments.</param>
        protected void InvokeVoid(string? functionName, params object?[] args)
            => JsRuntime.InvokeVoid(InvokeFunctionOption.Identifier, new InvokeFunctionOption(InternalGetAccessPath(), functionName, args));

        /// <summary>
        /// Invokes the specified JavaScript function asynchronously.
        /// </summary>
        /// <param name="functionName">The function name to invoke.</param>
        /// <param name="args">JSON-serializable arguments.</param>
        /// <returns>A <see cref="ValueTask" /> that represents the asynchronous invocation operation.</returns>
        protected ValueTask InvokeVoidAsync(string? functionName, params object?[] args)
            => JsRuntime.InvokeVoidAsync(InvokeFunctionOption.AsyncIdentifier, new InvokeFunctionOption(InternalGetAccessPath(), functionName, args));

        /// <summary>
        /// Converts the object to a different type synchronously.
        /// </summary>
        /// <returns>An instance of <typeparamref name="TValue" /> obtained by JSON-deserializing the return value.</returns>
        protected TValue? ConvertToType<TValue>()
            => JsRuntime.Invoke<TValue>(ConvertObjectTypeOption.Identifier, new ConvertObjectTypeOption(InternalGetAccessPath()));

        /// <summary>
        /// Converts the object to a different type asynchronously.
        /// </summary>
        /// <returns>An instance of <typeparamref name="TValue" /> obtained by JSON-deserializing the return value.</returns>
        protected ValueTask<TValue?> ConvertToTypeAsync<TValue>()
            => JsRuntime.InvokeAsync<TValue>(ConvertObjectTypeOption.Identifier, new ConvertObjectTypeOption(InternalGetAccessPath()));
    }
}
