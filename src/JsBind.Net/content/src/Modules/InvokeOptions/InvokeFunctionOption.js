import InvokeOption from "./InvokeOption.js";

export default class InvokeFunctionOption extends InvokeOption {
  /** @type {string} */
  accessPath;
  /** @type {string} */
  functionName;
  /** @type {any[]} */
  functionArguments;
}