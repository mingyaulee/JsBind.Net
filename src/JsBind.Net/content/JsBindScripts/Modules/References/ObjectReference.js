import ReferenceBase, { IsReferenceBase } from "./ReferenceBase.js";
import ReferenceType from "./ReferenceType.js";

export default class ObjectReference extends ReferenceBase {
  /** @type {string} */
  accessPath;
}

/**
 * Checks if a value is an ObjectReference.
 * @param {any} value
 * @returns {value is ObjectReference}
 */
export function IsObjectReference(value) {
  return IsReferenceBase(value) && value.__referenceType === ReferenceType.Object;
}