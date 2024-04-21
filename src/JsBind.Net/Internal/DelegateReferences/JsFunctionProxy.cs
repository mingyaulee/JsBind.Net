﻿using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using JsBind.Net.Internal.Extensions;

namespace JsBind.Net.Internal.DelegateReferences
{
    /// <summary>
    /// A JS function proxy to be invoked in DotNet.
    /// </summary>
    [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.NonPublicMethods)]
    internal class JsFunctionProxy : FunctionBindingBase
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

#pragma warning disable S3011 // Reflection should not be used to increase accessibility of classes, methods, or fields
        static IReadOnlyDictionary<string, MethodInfo> InvokeMethods = typeof(JsFunctionProxy)
            .GetMethods(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly)
            .ToDictionary(methodInfo => methodInfo.Name + (methodInfo.IsGenericMethod ? methodInfo.GetGenericArguments().Length : string.Empty), methodInfo => methodInfo);
#pragma warning restore S3011 // Reflection should not be used to increase accessibility of classes, methods, or fields

#pragma warning disable S1144 // Unused private types or members should be removed
#pragma warning disable IDE0051 // Remove unused private members
        private void InvokeFunctionVoid() => InvokeVoid();
        private void InvokeFunctionVoid<T1>(T1 arg1) => InvokeVoid(arg1);
        private void InvokeFunctionVoid<T1, T2>(T1 arg1, T2 arg2) => InvokeVoid(arg1, arg2);
        private void InvokeFunctionVoid<T1, T2, T3>(T1 arg1, T2 arg2, T3 arg3) => InvokeVoid(arg1, arg2, arg3);
        private void InvokeFunctionVoid<T1, T2, T3, T4>(T1 arg1, T2 arg2, T3 arg3, T4 arg4) => InvokeVoid(arg1, arg2, arg3, arg4);
        private void InvokeFunctionVoid<T1, T2, T3, T4, T5>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5) => InvokeVoid(arg1, arg2, arg3, arg4, arg5);
        private void InvokeFunctionVoid<T1, T2, T3, T4, T5, T6>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6) => InvokeVoid(arg1, arg2, arg3, arg4, arg5, arg6);
        private void InvokeFunctionVoid<T1, T2, T3, T4, T5, T6, T7>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7) => InvokeVoid(arg1, arg2, arg3, arg4, arg5, arg6, arg7);
        private void InvokeFunctionVoid<T1, T2, T3, T4, T5, T6, T7, T8>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8) => InvokeVoid(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8);
        private void InvokeFunctionVoid<T1, T2, T3, T4, T5, T6, T7, T8, T9>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9) => InvokeVoid(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9);
        private void InvokeFunctionVoid<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10) => InvokeVoid(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10);
        private void InvokeFunctionVoid<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11) => InvokeVoid(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11);
        private void InvokeFunctionVoid<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12) => InvokeVoid(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12);
        private void InvokeFunctionVoid<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13) => InvokeVoid(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13);
        private void InvokeFunctionVoid<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14) => InvokeVoid(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14);
        private void InvokeFunctionVoid<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15) => InvokeVoid(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15);
        private void InvokeFunctionVoid<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15, T16 arg16) => InvokeVoid(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15, arg16);
        private ValueTask InvokeFunctionVoidAsync() => InvokeVoidAsync();
        private ValueTask InvokeFunctionVoidAsync<T1>(T1 arg1) => InvokeVoidAsync(arg1);
        private ValueTask InvokeFunctionVoidAsync<T1, T2>(T1 arg1, T2 arg2) => InvokeVoidAsync(arg1, arg2);
        private ValueTask InvokeFunctionVoidAsync<T1, T2, T3>(T1 arg1, T2 arg2, T3 arg3) => InvokeVoidAsync(arg1, arg2, arg3);
        private ValueTask InvokeFunctionVoidAsync<T1, T2, T3, T4>(T1 arg1, T2 arg2, T3 arg3, T4 arg4) => InvokeVoidAsync(arg1, arg2, arg3, arg4);
        private ValueTask InvokeFunctionVoidAsync<T1, T2, T3, T4, T5>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5) => InvokeVoidAsync(arg1, arg2, arg3, arg4, arg5);
        private ValueTask InvokeFunctionVoidAsync<T1, T2, T3, T4, T5, T6>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6) => InvokeVoidAsync(arg1, arg2, arg3, arg4, arg5, arg6);
        private ValueTask InvokeFunctionVoidAsync<T1, T2, T3, T4, T5, T6, T7>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7) => InvokeVoidAsync(arg1, arg2, arg3, arg4, arg5, arg6, arg7);
        private ValueTask InvokeFunctionVoidAsync<T1, T2, T3, T4, T5, T6, T7, T8>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8) => InvokeVoidAsync(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8);
        private ValueTask InvokeFunctionVoidAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9) => InvokeVoidAsync(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9);
        private ValueTask InvokeFunctionVoidAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10) => InvokeVoidAsync(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10);
        private ValueTask InvokeFunctionVoidAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11) => InvokeVoidAsync(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11);
        private ValueTask InvokeFunctionVoidAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12) => InvokeVoidAsync(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12);
        private ValueTask InvokeFunctionVoidAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13) => InvokeVoidAsync(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13);
        private ValueTask InvokeFunctionVoidAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14) => InvokeVoidAsync(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14);
        private ValueTask InvokeFunctionVoidAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15) => InvokeVoidAsync(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15);
        private ValueTask InvokeFunctionVoidAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15, T16 arg16) => InvokeVoidAsync(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15, arg16);
        private Task InvokeFunctionVoidAsTaskAsync() => InvokeVoidAsync().AsTask();
        private Task InvokeFunctionVoidAsTaskAsync<T1>(T1 arg1) => InvokeVoidAsync(arg1).AsTask();
        private Task InvokeFunctionVoidAsTaskAsync<T1, T2>(T1 arg1, T2 arg2) => InvokeVoidAsync(arg1, arg2).AsTask();
        private Task InvokeFunctionVoidAsTaskAsync<T1, T2, T3>(T1 arg1, T2 arg2, T3 arg3) => InvokeVoidAsync(arg1, arg2, arg3).AsTask();
        private Task InvokeFunctionVoidAsTaskAsync<T1, T2, T3, T4>(T1 arg1, T2 arg2, T3 arg3, T4 arg4) => InvokeVoidAsync(arg1, arg2, arg3, arg4).AsTask();
        private Task InvokeFunctionVoidAsTaskAsync<T1, T2, T3, T4, T5>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5) => InvokeVoidAsync(arg1, arg2, arg3, arg4, arg5).AsTask();
        private Task InvokeFunctionVoidAsTaskAsync<T1, T2, T3, T4, T5, T6>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6) => InvokeVoidAsync(arg1, arg2, arg3, arg4, arg5, arg6).AsTask();
        private Task InvokeFunctionVoidAsTaskAsync<T1, T2, T3, T4, T5, T6, T7>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7) => InvokeVoidAsync(arg1, arg2, arg3, arg4, arg5, arg6, arg7).AsTask();
        private Task InvokeFunctionVoidAsTaskAsync<T1, T2, T3, T4, T5, T6, T7, T8>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8) => InvokeVoidAsync(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8).AsTask();
        private Task InvokeFunctionVoidAsTaskAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9) => InvokeVoidAsync(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9).AsTask();
        private Task InvokeFunctionVoidAsTaskAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10) => InvokeVoidAsync(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10).AsTask();
        private Task InvokeFunctionVoidAsTaskAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11) => InvokeVoidAsync(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11).AsTask();
        private Task InvokeFunctionVoidAsTaskAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12) => InvokeVoidAsync(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12).AsTask();
        private Task InvokeFunctionVoidAsTaskAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13) => InvokeVoidAsync(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13).AsTask();
        private Task InvokeFunctionVoidAsTaskAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14) => InvokeVoidAsync(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14).AsTask();
        private Task InvokeFunctionVoidAsTaskAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15) => InvokeVoidAsync(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15).AsTask();
        private Task InvokeFunctionVoidAsTaskAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15, T16 arg16) => InvokeVoidAsync(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15, arg16).AsTask();
        private TResult? InvokeFunction<TResult>() => Invoke<TResult>();
        private TResult? InvokeFunction<T1, TResult>(T1 arg1) => Invoke<TResult>(arg1);
        private TResult? InvokeFunction<T1, T2, TResult>(T1 arg1, T2 arg2) => Invoke<TResult>(arg1, arg2);
        private TResult? InvokeFunction<T1, T2, T3, TResult>(T1 arg1, T2 arg2, T3 arg3) => Invoke<TResult>(arg1, arg2, arg3);
        private TResult? InvokeFunction<T1, T2, T3, T4, TResult>(T1 arg1, T2 arg2, T3 arg3, T4 arg4) => Invoke<TResult>(arg1, arg2, arg3, arg4);
        private TResult? InvokeFunction<T1, T2, T3, T4, T5, TResult>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5) => Invoke<TResult>(arg1, arg2, arg3, arg4, arg5);
        private TResult? InvokeFunction<T1, T2, T3, T4, T5, T6, TResult>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6) => Invoke<TResult>(arg1, arg2, arg3, arg4, arg5, arg6);
        private TResult? InvokeFunction<T1, T2, T3, T4, T5, T6, T7, TResult>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7) => Invoke<TResult>(arg1, arg2, arg3, arg4, arg5, arg6, arg7);
        private TResult? InvokeFunction<T1, T2, T3, T4, T5, T6, T7, T8, TResult>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8) => Invoke<TResult>(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8);
        private TResult? InvokeFunction<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9) => Invoke<TResult>(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9);
        private TResult? InvokeFunction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10) => Invoke<TResult>(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10);
        private TResult? InvokeFunction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TResult>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11) => Invoke<TResult>(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11);
        private TResult? InvokeFunction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TResult>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12) => Invoke<TResult>(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12);
        private TResult? InvokeFunction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13) => Invoke<TResult>(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13);
        private TResult? InvokeFunction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TResult>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14) => Invoke<TResult>(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14);
        private TResult? InvokeFunction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TResult>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15) => Invoke<TResult>(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15);
        private TResult? InvokeFunction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TResult>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15, T16 arg16) => Invoke<TResult>(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15, arg16);
        private ValueTask<TResult?> InvokeFunctionAsync<TResult>() => InvokeAsync<TResult>();
        private ValueTask<TResult?> InvokeFunctionAsync<T1, TResult>(T1 arg1) => InvokeAsync<TResult>(arg1);
        private ValueTask<TResult?> InvokeFunctionAsync<T1, T2, TResult>(T1 arg1, T2 arg2) => InvokeAsync<TResult>(arg1, arg2);
        private ValueTask<TResult?> InvokeFunctionAsync<T1, T2, T3, TResult>(T1 arg1, T2 arg2, T3 arg3) => InvokeAsync<TResult>(arg1, arg2, arg3);
        private ValueTask<TResult?> InvokeFunctionAsync<T1, T2, T3, T4, TResult>(T1 arg1, T2 arg2, T3 arg3, T4 arg4) => InvokeAsync<TResult>(arg1, arg2, arg3, arg4);
        private ValueTask<TResult?> InvokeFunctionAsync<T1, T2, T3, T4, T5, TResult>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5) => InvokeAsync<TResult>(arg1, arg2, arg3, arg4, arg5);
        private ValueTask<TResult?> InvokeFunctionAsync<T1, T2, T3, T4, T5, T6, TResult>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6) => InvokeAsync<TResult>(arg1, arg2, arg3, arg4, arg5, arg6);
        private ValueTask<TResult?> InvokeFunctionAsync<T1, T2, T3, T4, T5, T6, T7, TResult>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7) => InvokeAsync<TResult>(arg1, arg2, arg3, arg4, arg5, arg6, arg7);
        private ValueTask<TResult?> InvokeFunctionAsync<T1, T2, T3, T4, T5, T6, T7, T8, TResult>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8) => InvokeAsync<TResult>(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8);
        private ValueTask<TResult?> InvokeFunctionAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9) => InvokeAsync<TResult>(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9);
        private ValueTask<TResult?> InvokeFunctionAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10) => InvokeAsync<TResult>(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10);
        private ValueTask<TResult?> InvokeFunctionAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TResult>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11) => InvokeAsync<TResult>(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11);
        private ValueTask<TResult?> InvokeFunctionAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TResult>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12) => InvokeAsync<TResult>(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12);
        private ValueTask<TResult?> InvokeFunctionAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13) => InvokeAsync<TResult>(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13);
        private ValueTask<TResult?> InvokeFunctionAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TResult>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14) => InvokeAsync<TResult>(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14);
        private ValueTask<TResult?> InvokeFunctionAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TResult>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15) => InvokeAsync<TResult>(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15);
        private ValueTask<TResult?> InvokeFunctionAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TResult>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15, T16 arg16) => InvokeAsync<TResult>(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15, arg16);
        private Task<TResult?> InvokeFunctionAsTaskAsync<TResult>() => InvokeAsync<TResult>().AsTask();
        private Task<TResult?> InvokeFunctionAsTaskAsync<T1, TResult>(T1 arg1) => InvokeAsync<TResult>(arg1).AsTask();
        private Task<TResult?> InvokeFunctionAsTaskAsync<T1, T2, TResult>(T1 arg1, T2 arg2) => InvokeAsync<TResult>(arg1, arg2).AsTask();
        private Task<TResult?> InvokeFunctionAsTaskAsync<T1, T2, T3, TResult>(T1 arg1, T2 arg2, T3 arg3) => InvokeAsync<TResult>(arg1, arg2, arg3).AsTask();
        private Task<TResult?> InvokeFunctionAsTaskAsync<T1, T2, T3, T4, TResult>(T1 arg1, T2 arg2, T3 arg3, T4 arg4) => InvokeAsync<TResult>(arg1, arg2, arg3, arg4).AsTask();
        private Task<TResult?> InvokeFunctionAsTaskAsync<T1, T2, T3, T4, T5, TResult>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5) => InvokeAsync<TResult>(arg1, arg2, arg3, arg4, arg5).AsTask();
        private Task<TResult?> InvokeFunctionAsTaskAsync<T1, T2, T3, T4, T5, T6, TResult>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6) => InvokeAsync<TResult>(arg1, arg2, arg3, arg4, arg5, arg6).AsTask();
        private Task<TResult?> InvokeFunctionAsTaskAsync<T1, T2, T3, T4, T5, T6, T7, TResult>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7) => InvokeAsync<TResult>(arg1, arg2, arg3, arg4, arg5, arg6, arg7).AsTask();
        private Task<TResult?> InvokeFunctionAsTaskAsync<T1, T2, T3, T4, T5, T6, T7, T8, TResult>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8) => InvokeAsync<TResult>(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8).AsTask();
        private Task<TResult?> InvokeFunctionAsTaskAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9) => InvokeAsync<TResult>(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9).AsTask();
        private Task<TResult?> InvokeFunctionAsTaskAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10) => InvokeAsync<TResult>(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10).AsTask();
        private Task<TResult?> InvokeFunctionAsTaskAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TResult>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11) => InvokeAsync<TResult>(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11).AsTask();
        private Task<TResult?> InvokeFunctionAsTaskAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TResult>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12) => InvokeAsync<TResult>(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12).AsTask();
        private Task<TResult?> InvokeFunctionAsTaskAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13) => InvokeAsync<TResult>(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13).AsTask();
        private Task<TResult?> InvokeFunctionAsTaskAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TResult>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14) => InvokeAsync<TResult>(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14).AsTask();
        private Task<TResult?> InvokeFunctionAsTaskAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TResult>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15) => InvokeAsync<TResult>(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15).AsTask();
        private Task<TResult?> InvokeFunctionAsTaskAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TResult>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15, T16 arg16) => InvokeAsync<TResult>(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15, arg16).AsTask();
#pragma warning restore IDE0051 // Remove unused private members
#pragma warning restore S1144 // Unused private types or members should be removed
    }
}
