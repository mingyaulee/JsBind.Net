using System.Threading.Tasks;
using JsBind.Net.InvokeOptions;

namespace JsBind.Net
{
    /// <summary>
    /// Adapter for IJSRuntime
    /// </summary>
    public interface IJsRuntimeAdapter
    {
        /// <summary>
        /// Invokes the specified JavaScript function synchronously.
        /// </summary>
        /// <typeparam name="TValue">The JSON-serializable return type.</typeparam>
        /// <param name="identifier">The identifier of the invocation function.</param>
        /// <param name="invokeOption">The option for invocation.</param>
        /// <returns>An instance of <typeparamref name="TValue" /> obtained by JSON-deserializing the return value.</returns>
        TValue? Invoke<TValue>(string identifier, InvokeOptionWithReturnValue invokeOption);

        /// <summary>
        /// Invokes the specified JavaScript function asynchronously.
        /// </summary>
        /// <typeparam name="TValue">The JSON-serializable return type.</typeparam>
        /// <param name="identifier">The identifier of the invocation function.</param>
        /// <param name="invokeOption">The option for invocation.</param>
        /// <returns>An instance of <typeparamref name="TValue" /> obtained by JSON-deserializing the return value.</returns>
        ValueTask<TValue?> InvokeAsync<TValue>(string identifier, InvokeOptionWithReturnValue invokeOption);

        /// <summary>
        /// Invokes the specified JavaScript function synchronously.
        /// </summary>
        /// <param name="identifier">The identifier of the invocation function.</param>
        /// <param name="invokeOption">The option for invocation.</param>
        void InvokeVoid(string identifier, InvokeOption invokeOption);

        /// <summary>
        /// Invokes the specified JavaScript function asynchronously.
        /// </summary>
        /// <param name="identifier">The identifier of the invocation function.</param>
        /// <param name="invokeOption">The option for invocation.</param>
        /// <returns>A <see cref="ValueTask" /> that represents the asynchronous invocation operation.</returns>
        ValueTask InvokeVoidAsync(string identifier, InvokeOption invokeOption);

        /// <summary>
        /// Checks if the instance of JS runtime is equal to the instance of JS runtime for the other adapter.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        bool IsJsRuntimeEqual(IJsRuntimeAdapter other);
    }
}
