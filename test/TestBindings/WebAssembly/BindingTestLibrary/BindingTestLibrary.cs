﻿using System;
using System.Threading.Tasks;
using JsBind.Net;

namespace TestBindings.WebAssembly.BindingTestLibrary
{
    public class BindingTestLibrary : ObjectBindingBase<BindingTestLibrary>
    {
        public BindingTestLibrary(IJsRuntimeAdapter jsRuntime)
        {
            SetAccessPath("BindingTestLibrary");
            Initialize(jsRuntime);
        }

        public TestBoundClass GetTestObjectReviverInstanceFromFunction() => Invoke<TestBoundClass>("getTestObjectReviverInstanceFromFunction");
        public TestBoundClass TestObjectReviverInstanceFromProperty => GetProperty<TestBoundClass>("testObjectReviverInstanceFromProperty");
        public bool IsObjectReferenceRevived(TestBoundClass obj) => Invoke<bool>("isObjectReferenceRevived", obj);
        public bool IsNestedObjectReferenceRevived(object obj) => Invoke<bool>("isNestedObjectReferenceRevived", obj);

        public bool IsDelegateReferenceRevived(Delegate del) => Invoke<bool>("isDelegateReferenceRevived", del);
        public bool IsNestedDelegateReferenceRevived(object obj) => Invoke<bool>("isNestedDelegateReferenceRevived", obj);
        public bool AreDelegateReferencesEqual(Delegate del1, Delegate del2) => Invoke<bool>("areDelegateReferencesEqual", del1, del2);

        public void TestInvokeDelegate(Delegate del) => InvokeVoid("testInvokeDelegate", del);
        public TResult TestInvokeDelegate<TResult>(Delegate del) => Invoke<TResult>("testInvokeDelegate", del);
        public ValueTask TestInvokeDelegateAsync(Delegate del) => InvokeVoidAsync("testInvokeDelegateAsync", del);
        public ValueTask<TResult> TestInvokeDelegateAsync<TResult>(Delegate del) => InvokeAsync<TResult>("testInvokeDelegateAsync", del);

        public Func<bool> GetFunctionDelegate() => Invoke<Func<bool>>("getFunctionDelegate");
        public NestedDelegateClass GetNestedActionDelegate() => Invoke<NestedDelegateClass>("getNestedActionDelegate");
        public Func<int, int> GetPrimitiveFunctionDelegate() => Invoke<Func<int, int>>("getMirrorFunctionDelegate");
        public Func<Window, Window> GetReferenceFunctionDelegate() => Invoke<Func<Window, Window>>("getMirrorFunctionDelegate");
    }
}
