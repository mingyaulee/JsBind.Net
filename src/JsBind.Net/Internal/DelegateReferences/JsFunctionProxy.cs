using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using JsBind.Net.Internal.Extensions;

namespace JsBind.Net.Internal.DelegateReferences
{
    /// <summary>
    /// A JS function proxy to be invoked in DotNet.
    /// </summary>
    internal partial class JsFunctionProxy : FunctionBindingBase
    {
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
            var invokeMethod = delegateType.GetMethod("Invoke") ?? throw new InvalidOperationException("Delegate must have invoke method.");
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

            var isVoid = returnType == typeof(void);
            var isTask = isAsync && typeof(Task).IsAssignableFrom(invokeMethod.ReturnType);
            var method = "InvokeFunction" +
                (isVoid ? "Void" : string.Empty) +
                (isTask ? "AsTask" : string.Empty) +
                (isAsync ? "Async" : string.Empty);

            var genericParameterTypes = invokeMethod.GetParameters().Select(p => p.ParameterType).Concat(isVoid ? Enumerable.Empty<Type>() : new[] { returnType }).ToArray();
            MethodInfo methodInfo;
            if (genericParameterTypes.Length > 0)
            {
                methodInfo = InvokeMethods[method + genericParameterTypes.Length].MakeGenericMethod(genericParameterTypes);
            }
            else
            {
                methodInfo = InvokeMethods[method];
            }
            return Delegate.CreateDelegate(delegateType, this, methodInfo, true)!;
        }
    }
}
