using System.Reflection;
using System.Threading.Tasks;
using JsBind.Net.Internal.Extensions;

namespace JsBind.Net.Internal.DelegateReferences
{
    /// <summary>
    /// Helper to unwrap result object from <see cref="Task" /> and <see cref="ValueTask" />.
    /// </summary>
    internal static class AsyncResultHelper
    {
#pragma warning disable S3011 // Reflection should not be used to increase accessibility of classes, methods, or fields
        private static readonly MethodInfo GetObjectFromValueTaskMethodInfo = typeof(AsyncResultHelper).GetMethod(nameof(GetObjectFromValueTask), BindingFlags.NonPublic | BindingFlags.Static)!;
        private static readonly MethodInfo GetObjectFromTaskMethodInfo = typeof(AsyncResultHelper).GetMethod(nameof(GetObjectFromTask), BindingFlags.NonPublic | BindingFlags.Static)!;
#pragma warning restore S3011 // Reflection should not be used to increase accessibility of classes, methods, or fields

        /// <summary>
        /// Gets the result object and waits if the result is <see cref="Task" /> or <see cref="ValueTask" />.
        /// </summary>
        /// <param name="result">The result.</param>
        /// <returns>The result of the delegate invocation.</returns>
        public static async ValueTask<object?> GetAsyncResultObject(object? result)
        {
            if (result is null)
            {
                return null;
            }

            var resultType = result.GetType();
            if (resultType.IsGenericType && resultType.IsTaskOrValueTask())
            {
                Task<object> objectTask;
                if (typeof(Task).IsAssignableFrom(resultType))
                {
                    objectTask=(Task<object>)GetObjectFromTaskMethodInfo.MakeGenericMethod(resultType.GetGenericArguments()[0]).Invoke(null, new[] { result })!;
                }
                else
                {
                    objectTask = (Task<object>)GetObjectFromValueTaskMethodInfo.MakeGenericMethod(resultType.GetGenericArguments()[0]).Invoke(null, new[] { result })!;
                }

                return await objectTask.ConfigureAwait(false);
            }

            if (result is Task task)
            {
                await task.ConfigureAwait(false);
                return null;
            }
            else if (result is ValueTask valueTask)
            {
                await valueTask.AsTask().ConfigureAwait(false);
                return null;
            }

            return result;
        }

        private static async Task<object?> GetObjectFromValueTask<TResult>(ValueTask<TResult> resultTask)
        {
            return await resultTask.AsTask().ConfigureAwait(false);
        }

        private static async Task<object?> GetObjectFromTask<TResult>(Task<TResult> resultTask)
        {
            return await resultTask.ConfigureAwait(false);
        }
    }
}
