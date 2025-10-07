using System;

namespace JsBind.Net
{
    /// <summary>
    /// Exception thrown by JsBind.
    /// </summary>
    /// <remarks>
    /// Creates a new instance of <see cref="JsBindException" />.
    /// </remarks>
    /// <param name="message">The error message.</param>
    /// <param name="previousStackTrace">The previous stack trace.</param>
    public sealed class JsBindException(string? message, string? previousStackTrace) : Exception(message)
    {
        private string? jsBindStackTrace;
        private readonly string? previousStackTrace = previousStackTrace;

        /// <inheritdoc />
        public override string? StackTrace => GetStackTrace();

        private string? GetStackTrace()
        {
            if (jsBindStackTrace is not null)
            {
                return jsBindStackTrace;
            }

            var currentStackTrace = base.StackTrace;
            if (!string.IsNullOrEmpty(previousStackTrace) && !string.IsNullOrEmpty(currentStackTrace))
            {
                jsBindStackTrace = $"{previousStackTrace}{Environment.NewLine}{Environment.NewLine}DotNet stack trace: {Environment.NewLine}{currentStackTrace}";
            }
            else
            {
                jsBindStackTrace = previousStackTrace + currentStackTrace;
            }

            return jsBindStackTrace;
        }
    }
}
