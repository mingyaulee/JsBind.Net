﻿export default class InvokeResult {
  /**
   * Creates a new instance of InvokeResult.
   * @param {any} value
   * @param {boolean} [isError]
   * @param {string} [errorMessage]
   */
  constructor(value, isError, errorMessage) {
    this.value = value;
    this.isError = isError;
    this.errorMessage = errorMessage;
  }
}