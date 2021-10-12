export default class ObjectBindingConfiguration {
  /** @type {boolean} */
  __isObjectBindingConfiguration;
  /** @type {string} */
  id;
  /** @type {string} */
  referenceId;
  /** @type {string[]} */
  include;
  /** @type {string[]} */
  exclude;
  /** @type {Object<string, ObjectBindingConfiguration>} */
  propertyBindings;
  /** @type {boolean} */
  isBindingBase;
  /** @type {ObjectBindingConfiguration} */
  arrayItemBinding;
}

/**
 * Checks if a value is a ObjectBindingConfiguration.
 * @param {any} value
 * @returns {value is ObjectBindingConfiguration}
 */
export function IsObjectBindingConfiguration(value) {
  return value &&
    typeof value === "object" &&
    /** @type {ObjectBindingConfiguration} */(value).__isObjectBindingConfiguration === true;
}