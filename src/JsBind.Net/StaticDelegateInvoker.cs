using System;
using System.ComponentModel;
using System.Threading.Tasks;
using JsBind.Net.DelegateReferences;
using JsBind.Net.Internal.DelegateReferences;
using Microsoft.JSInterop;

namespace JsBind.Net;

/// <summary>
/// Static methods to invoke delegate from JavaScript.
/// </summary>
[EditorBrowsable(EditorBrowsableState.Never)]
public static class StaticDelegateInvoker
{
    /// <summary>
    /// Invoke a delegate reference from JavaScript.
    /// </summary>
    /// <param name="delegateId">The delegate identifier.</param>
    /// <param name="invokeWrapper">Contains the arguments to invoke the delegate.</param>
    /// <returns>The result of the delegate invocation.</returns>
    [JSInvokable("InvokeDelegateFromJs")]
    public static object? InvokeDelegateFromJs(Guid delegateId, DelegateInvokeWrapper? invokeWrapper)
    {
        var capturedDelegateReference = DelegateReferenceManager.GetCapturedDelegateReference(delegateId);
        return capturedDelegateReference.InvokeDelegateFromJs(invokeWrapper);
    }

    /// <summary>
    /// Invoke a delegate reference from JavaScript.
    /// </summary>
    /// <param name="delegateId">The delegate identifier.</param>
    /// <param name="invokeWrapper">Contains the arguments to invoke the delegate.</param>
    /// <returns>The result of the delegate invocation.</returns>
    [JSInvokable("InvokeDelegateFromJsAsync")]
    public static ValueTask<object?> InvokeDelegateFromJsAsync(Guid delegateId, DelegateInvokeWrapper? invokeWrapper)
    {
        var capturedDelegateReference = DelegateReferenceManager.GetCapturedDelegateReference(delegateId);
        return capturedDelegateReference.InvokeDelegateFromJsAsync(invokeWrapper);
    }
}
