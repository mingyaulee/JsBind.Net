using System.Collections;
using System.Collections.Generic;

namespace JsBind.Net.InvokeOptions;

/// <summary>
/// A collection of function arguments.
/// </summary>
public class FunctionArgumentCollection : IEnumerable<object?>
{
    internal readonly IEnumerable<object?> EnumerableValue;
    internal readonly InvokeOption InvokeOption;

    internal FunctionArgumentCollection(IEnumerable<object?> enumerable, InvokeOption invokeOption)
    {
        EnumerableValue = enumerable;
        InvokeOption = invokeOption;
    }

    /// <inheritdoc />
    IEnumerator IEnumerable.GetEnumerator() => EnumerableValue.GetEnumerator();

    /// <inheritdoc />
    public IEnumerator<object?> GetEnumerator() => EnumerableValue.GetEnumerator();
}
