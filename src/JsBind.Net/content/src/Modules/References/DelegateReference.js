import ReferenceBase, { IsReferenceBase } from "./ReferenceBase.js";
import ReferenceType from "./ReferenceType.js";

/**
 * @typedef {import("../InvokeOptions/ObjectBindingConfiguration.js").default} ObjectBindingConfiguration
 * @typedef {import("../InvokeResult.js").default} InvokeResult
 */

/**
 * @callback InvokeMethodFunction
 * @param {string} methodName The method name.
 * @param {any[]} args The arguments.
 * @returns {InvokeResult}
 */

/**
 * @typedef {object} TaskInvokeResult
 * @property {InvokeResult} result
 */

/**
 * @callback InvokeMethodAsyncFunction
 * @param {string} methodName The method name.
 * @param {any[]} args The arguments.
 * @returns {Promise<InvokeResult> | Promise<TaskInvokeResult>}
 */

/**
 * @typedef {object} DotNetObjectReference 
 * @property {InvokeMethodFunction} invokeMethod
 * @property {InvokeMethodAsyncFunction} invokeMethodAsync
 */

export default class DelegateReference extends ReferenceBase {
  /** @type {string} */
  delegateId;
  /** @type {ObjectBindingConfiguration[]} */
  argumentBindings;
  /** @type {boolean[]} */
  storeArgumentsAsReferences;
  /** @type {string[]} */
  argumentsReferenceIds;
  /** @type {boolean} */
  isAsync;
  /** @type {DotNetObjectReference} */
  delegateInvoker;
}

/**
 * Checks if a value is a DelegateReference.
 * @param {any} value
 * @returns {value is DelegateReference}
 */
export function IsDelegateReference(value) {
  return IsReferenceBase(value) && value.__referenceType === ReferenceType.Delegate;
}