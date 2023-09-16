export default class InvokeResult {
  /** @type {any} */
  value;
  /** @type {boolean} */
  isError;
  /** @type {string} */
  errorMessage;
  /** @type {string} */
  stackTrace;

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