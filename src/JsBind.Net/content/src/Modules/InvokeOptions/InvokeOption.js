/**
 * @typedef {import("./ObjectBindingConfiguration.js").default} ObjectBindingConfiguration
 */

export default class InvokeOption {
  /** @type {boolean} */
  hasReturnValue;
  /** @type {boolean} */
  returnValueIsReference;
  /** @type {string} */
  returnValueReferenceId;
  /** @type {ObjectBindingConfiguration} */
  returnValueBinding;
  /** @type {function(): string} */
  getReturnValueAccessPath;
}