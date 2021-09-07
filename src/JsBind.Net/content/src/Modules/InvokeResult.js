export default class InvokeResult {
  /**
   * Creates a new instance of InvokeResult.
   * @param {any} value
   * @param {boolean} [isError]
   * @param {string} [errorMessage]
   * @param {string} [stackTrace]
   */
  constructor(value, isError, errorMessage, stackTrace) {
    this.value = value;
    this.isError = isError;
    this.errorMessage = errorMessage;
    this.stackTrace = stackTrace;
  }
}