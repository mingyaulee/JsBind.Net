export default class JsBindError extends Error {
  /**
   * Creates a new instance of JsBindError.
   * @param {string} message
   * @param {string} [stackTrace]
   */
  constructor(message, stackTrace) {
    super(message);

    // Check if the stack trace is in the message
    let currentStackTrace = "";
    if (this.stack) {
      currentStackTrace = this.stack;
    } else {
      let stackTraceIndex = this.message.indexOf(message) + message.length;
      if (this.message.length > stackTraceIndex) {
        currentStackTrace = this.message.substring(stackTraceIndex);
        this.message = this.message.substring(0, stackTraceIndex);
      }
    }

    stackTrace = stackTrace || "";
    currentStackTrace = currentStackTrace ? "JavaScript stack trace: \n" + currentStackTrace : "";
    if (stackTrace && currentStackTrace) {
      this.stack = stackTrace + "\n\n" + currentStackTrace;
    } else {
      this.stack = stackTrace || currentStackTrace;
    }
  }
}