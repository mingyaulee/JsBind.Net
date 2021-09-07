import ReferenceBase, { IsReferenceBase } from "./ReferenceBase.js";
import ReferenceType from "./ReferenceType.js";

/**
 * @typedef {import("../InvokeOptions/ObjectBindingConfiguration.js").default} ObjectBindingConfiguration
 * @typedef {import("../InvokeResult.js").default} InvokeResult
 */

/**
 * @typedef {object} TaskInvokeResult
 * @property {InvokeResult} result
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
}

/**
 * Checks if a value is a DelegateReference.
 * @param {any} value
 * @returns {value is DelegateReference}
 */
export function IsDelegateReference(value) {
  return IsReferenceBase(value) && value.__referenceType === ReferenceType.Delegate;
}