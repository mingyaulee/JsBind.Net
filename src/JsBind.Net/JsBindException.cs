using System;
using System.Runtime.Serialization;

namespace JsBind.Net
{
    /// <summary>
    /// Exception thrown by JsBind.
    /// </summary>
    [Serializable]
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

#if NET8_0_OR_GREATER
        [Obsolete("Serialization support is not required for custom exceptions", DiagnosticId = "SYSLIB0051")] // add this attribute to the serialization ctor
#endif
        private JsBindException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            jsBindStackTrace = info.GetString("JsBindStackTrace");
            previousStackTrace = info.GetString("PreviousStackTrace");
        }

        /// <inheritdoc />
#if NET8_0_OR_GREATER
        [Obsolete("Serialization support is not required for custom exceptions", DiagnosticId = "SYSLIB0051")] // add this attribute to the serialization ctor
#endif
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("JsBindStackTrace", jsBindStackTrace, typeof(string));
            info.AddValue("PreviousStackTrace", previousStackTrace, typeof(string));
        }
    }
}
