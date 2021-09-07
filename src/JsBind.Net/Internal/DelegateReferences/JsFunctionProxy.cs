using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using JsBind.Net.Internal.Extensions;

namespace JsBind.Net.Internal.DelegateReferences
{
    /// <summary>
    /// A JS function proxy to be invoked in DotNet.
    /// </summary>
    internal class JsFunctionProxy : FunctionBindingBase
    {
#pragma warning disable S3011 // Reflection should not be used to increase accessibility of classes, methods, or fields
        private static readonly MethodInfo dynamicInvokeMethodInfo = typeof(JsFunctionProxy).GetMethod(nameof(DynamicInvoke), BindingFlags.NonPublic | BindingFlags.Instance)!;
        private static readonly MethodInfo dynamicInvokeAsyncMethodInfo = typeof(JsFunctionProxy).GetMethod(nameof(DynamicInvokeAsync), BindingFlags.NonPublic | BindingFlags.Instance)!;
#pragma warning restore S3011 // Reflection should not be used to increase accessibility of classes, methods, or fields

        /// <summary>
        /// Creates a new instance of the <see cref="JsFunctionProxy" /> class.
        /// </summary>
        /// <param name="jsRuntime">The JS runtime.</param>
        /// <param name="accessPath">The access path.</param>
        public JsFunctionProxy(IJsRuntimeAdapter jsRuntime, string accessPath)
        {
            SetAccessPath(accessPath);
            Initialize(jsRuntime);
        }

        /// <summary>
        /// Gets the proxy delegate to invoke the JavaScript function.
        /// </summary>
        /// <param name="delegateType">The delegate type.</param>
        /// <returns>A delegate matching the signature of the delegate type, when invoked executes the JavaScript function.</returns>
        public Delegate GetDelegate(Type delegateType)
        {
            // Generates a lambda expression based on the delegate type.
            // Action generates () => DynamicInvoke(new object[])
            // Action<int> generates (arg0) => DynamicInvoke(new object[] { (object)arg0 })
            // Func<int, int> generates (arg0) => (int)DynamicInvoke(new object[] { (object)arg0 })
            // Action<int, string> generates (arg0, arg1) => DynamicInvoke(new object[] { (object)arg0, (object)arg1 })
            // Func<int, string, int> generates (arg0, arg1) => DynamicInvoke(new object[] { (object)arg0, (object)arg1 })

            var invokeMethod = delegateType.GetMethod("Invoke");
            if (invokeMethod is null)
            {
                throw new InvalidOperationException("Delegate must have invoke method.");
            }
            var isAsync = invokeMethod.ReturnType.IsTaskOrValueTask();

            Type? returnType;
            if (isAsync)
            {
                if (invokeMethod.ReturnType.IsGenericType)
                {
                    returnType = invokeMethod.ReturnType.GetGenericArguments()[0];
                }
                else
                {
                    returnType = typeof(void);
                }
            }
            else
            {
                returnType = invokeMethod.ReturnType;
            }

            if (returnType == typeof(void))
            {
                returnType = typeof(object);
            }

            MethodInfo methodCallInfo;
            if (isAsync)
            {
                methodCallInfo = dynamicInvokeAsyncMethodInfo.MakeGenericMethod(returnType);
            }
            else
            {
                methodCallInfo = dynamicInvokeMethodInfo.MakeGenericMethod(returnType);
            }

            var invokeParameters = invokeMethod.GetParameters();
            var parameterExpressions = invokeParameters.Select((invokeParameter, index) => Expression.Parameter(invokeParameter.ParameterType, $"arg{index}")).ToArray();
            var newArrayInitExpression = Expression.NewArrayInit(typeof(object), parameterExpressions.Select(parameterExpression => Expression.Convert(parameterExpression, typeof(object))));
            var methodCallExpression = Expression.Call(Expression.Constant(this), methodCallInfo, newArrayInitExpression);

            Expression expression = methodCallExpression;
            if (isAsync && typeof(Task).IsAssignableFrom(invokeMethod.ReturnType))
            {
                var asTaskMethodInfo = methodCallInfo.ReturnType.GetMethod(nameof(ValueTask.AsTask))!;
                expression = Expression.Call(expression, asTaskMethodInfo);
            }

            if (invokeMethod.ReturnType != typeof(void))
            {
                expression = Expression.Convert(expression, invokeMethod.ReturnType);
            }

            return Expression.Lambda(delegateType, expression, parameterExpressions).Compile();
        }

        /// <summary>
        /// Dynamically invoke the JavaScript function synchronously.
        /// </summary>
        /// <typeparam name="TValue">The JSON-serializable return type.</typeparam>
        /// <param name="arguments">JSON-serializable arguments.</param>
        /// <returns>An instance of TResult obtained by JSON-deserializing the return value.</returns>
        private TValue? DynamicInvoke<TValue>(object[] arguments)
            => Invoke<TValue>(arguments);

        /// <summary>
        /// Dynamically invoke the JavaScript function asynchronously.
        /// </summary>
        /// <typeparam name="TValue">The JSON-serializable return type.</typeparam>
        /// <param name="arguments">JSON-serializable arguments.</param>
        /// <returns>An instance of TResult obtained by JSON-deserializing the return value.</returns>
        private ValueTask<TValue?> DynamicInvokeAsync<TValue>(object[] arguments)
            => InvokeAsync<TValue>(arguments);
    }
}
