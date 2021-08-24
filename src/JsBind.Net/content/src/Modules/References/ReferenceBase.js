/**
 * @typedef {import("./ReferenceType.js").ReferenceTypeEnumValue} ReferenceTypeEnumValue
 */

export default class ReferenceBase {
  /** @type {boolean} */
  __isJsBindReference;
  /** @type {ReferenceTypeEnumValue} */
  __referenceType;
}

/**
 * Checks if a value is a ReferenceBase.
 * @param {any} value
 * @returns {value is ReferenceBase}
 */
export function IsReferenceBase(value) {
  return value &&
    typeof value === "object" &&
    /** @type {ReferenceBase} */(value).__isJsBindReference === true;
}