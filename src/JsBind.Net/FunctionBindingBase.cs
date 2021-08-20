using System.Threading.Tasks;
using JsBind.Net.InvokeOptions;

namespace JsBind.Net
{
    /// <summary>
    /// Base JavaScript function binding.
    /// </summary>
    public class FunctionBindingBase : BindingBase<FunctionBindingBase>
    {
        /// <summary>
        /// Invokes the JavaScript function synchronously.
        /// </summary>
        /// <param name="args">JSON-serializable arguments.</param>
        /// <returns>An instance of <typeparamref name="TValue" /> obtained by JSON-deserializing the return value.</returns>
        protected TValue? Invoke<TValue>(params object?[] args)
            => JsRuntime.Invoke<TValue>(InvokeFunctionOption.Identifier, new InvokeFunctionOption<TValue>(InternalGetAccessPath(), null, args));

        /// <summary>
        /// Invokes the JavaScript function asynchronously.
        /// </summary>
        /// <param name="args">JSON-serializable arguments.</param>
        /// <returns>An instance of <typeparamref name="TValue" /> obtained by JSON-deserializing the return value.</returns>
        protected ValueTask<TValue?> InvokeAsync<TValue>(params object?[] args)
            => JsRuntime.InvokeAsync<TValue>(InvokeFunctionOption.AsyncIdentifier, new InvokeFunctionOption<TValue>(InternalGetAccessPath(), null, args));

        /// <summary>
        /// Invokes the JavaScript function synchronously.
        /// </summary>
        /// <param name="args">JSON-serializable arguments.</param>
        protected void InvokeVoid(params object?[] args)
            => JsRuntime.InvokeVoid(InvokeFunctionOption.Identifier, new InvokeFunctionOption(InternalGetAccessPath(), null, args));

        /// <summary>
        /// Invokes the JavaScript function asynchronously.
        /// </summary>
        /// <param name="args">JSON-serializable arguments.</param>
        /// <returns>A <see cref="ValueTask" /> that represents the asynchronous invocation operation.</returns>
        protected ValueTask InvokeVoidAsync(params object?[] args)
            => JsRuntime.InvokeVoidAsync(InvokeFunctionOption.AsyncIdentifier, new InvokeFunctionOption(InternalGetAccessPath(), null, args));
    }
}
