using System;

namespace JsBind.Net
{
    /// <summary>
    /// Exception thrown by JsBind.
    /// </summary>
    public sealed class JsBindException : Exception
    {
        private string? jsBindStackTrace;
        private readonly string? previousStackTrace;

        /// <inheritdoc />
        public override string? StackTrace => GetStackTrace();

        /// <summary>
        /// Creates a new instance of <see cref="JsBindException" />.
        /// </summary>
        /// <param name="message">The error message.</param>
        /// <param name="previousStackTrace">The previous stack trace.</param>
        public JsBindException(string? message, string? previousStackTrace) : base(message)
        {
            this.previousStackTrace = previousStackTrace;
        }

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
