namespace JsBind.Net.InvokeOptions
{
    /// <summary>
    /// A wrapper for invoke argument.
    /// </summary>
    public class InvokeArgument
    {
        internal readonly object? ArgumentValue;
        internal readonly InvokeOption InvokeOption;

        internal InvokeArgument(object? argumentValue, InvokeOption invokeOption)
        {
            ArgumentValue = argumentValue;
            InvokeOption = invokeOption;
        }
    }
}
