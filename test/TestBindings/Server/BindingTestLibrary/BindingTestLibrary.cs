using System;
using System.Threading.Tasks;
using JsBind.Net;

namespace TestBindings.Server.BindingTestLibrary
{
    public class BindingTestLibrary : ObjectBindingBase<BindingTestLibrary>
    {
        public BindingTestLibrary(IJsRuntimeAdapter jsRuntime)
        {
            SetAccessPath("BindingTestLibrary");
            Initialize(jsRuntime);
        }

        public ValueTask<TestBoundClass> GetTestObjectReviverInstanceFromFunction() => InvokeAsync<TestBoundClass>("getTestObjectReviverInstanceFromFunction");
        public ValueTask<TestBoundClass> GetTestObjectReviverInstanceFromProperty() => GetPropertyAsync<TestBoundClass>("testObjectReviverInstanceFromProperty");
        public ValueTask<bool> IsObjectReferenceRevived(TestBoundClass obj) => InvokeAsync<bool>("isObjectReferenceRevived", obj);
        public ValueTask<bool> IsNestedObjectReferenceRevived(object obj) => InvokeAsync<bool>("isNestedObjectReferenceRevived", obj);

        public ValueTask<bool> IsDelegateReferenceRevived(Delegate del) => InvokeAsync<bool>("isDelegateReferenceRevived", del);
        public ValueTask<bool> IsNestedDelegateReferenceRevived(object obj) => InvokeAsync<bool>("isNestedDelegateReferenceRevived", obj);
        public ValueTask<bool> AreDelegateReferencesEqual(Delegate del1, Delegate del2) => InvokeAsync<bool>("areDelegateReferencesEqual", del1, del2);

        public ValueTask TestInvokeDelegate(Delegate del) => InvokeVoidAsync("testInvokeDelegate", del);
        public ValueTask<TResult> TestInvokeDelegate<TResult>(Delegate del) => InvokeAsync<TResult>("testInvokeDelegate", del);
        public ValueTask TestInvokeDelegateAsync(Delegate del) => InvokeVoidAsync("testInvokeDelegateAsync", del);
        public ValueTask<TResult> TestInvokeDelegateAsync<TResult>(Delegate del) => InvokeAsync<TResult>("testInvokeDelegateAsync", del);

        public ValueTask<Func<ValueTask<bool>>> GetFunctionDelegate() => InvokeAsync<Func<ValueTask<bool>>>("getFunctionDelegate");
        public ValueTask<NestedDelegateClass> GetNestedActionDelegate() => InvokeAsync<NestedDelegateClass>("getNestedActionDelegate");
        public ValueTask<Func<int, Task<int>>> GetPrimitiveFunctionDelegate() => InvokeAsync<Func<int, Task<int>>>("getMirrorFunctionDelegate");
        public ValueTask<Func<Window, Task<Window>>> GetReferenceFunctionDelegate() => InvokeAsync<Func<Window, Task<Window>>>("getMirrorFunctionDelegate");
    }
}
