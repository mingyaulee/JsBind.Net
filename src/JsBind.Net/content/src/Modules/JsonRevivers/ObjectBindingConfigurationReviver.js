import { IsObjectBindingConfiguration } from "../InvokeOptions/ObjectBindingConfiguration.js";

/**
 * @typedef {import("../InvokeOptions/ObjectBindingConfiguration.js").default} ObjectBindingConfiguration
 */

/**
 * @callback FoundBindingCallback
 * @param {ObjectBindingConfiguration}
 */

class ObjectBindingConfigurationReviverClass {
  constructor() {
    /** @type {Object<string, ObjectBindingConfiguration>} */
    this.references = {};
    /** @type {Object<string, FoundBindingCallback[]>} */
    this.referenceCallbacks = {};
  }

  /**
   * Revives reference binding configuration.
   * @param {any} key
   * @param {any} value
   */
  revive(key, value) {
    if (IsObjectBindingConfiguration(value)) {
      if (value.id) {
        this.references[value.id] = value;
        if (this.referenceCallbacks.hasOwnProperty(value.id)) {
          this.referenceCallbacks[value.id].forEach(callback => callback(value));
          this.referenceCallbacks[value.id] = [];
          try {
            delete this.referenceCallbacks[value.id];
          } catch { }
        }
      } else if (value.referenceId) {
        if (this.references.hasOwnProperty(value.referenceId)) {
          return this.references[value.referenceId];
        } else {
          if (!this.referenceCallbacks.hasOwnProperty(value.referenceId)) {
            this.referenceCallbacks[value.referenceId] = [];
          }
          this.referenceCallbacks[value.referenceId].push(binding => {
            value.include = binding.include;
            value.exclude = binding.exclude;
            value.propertyBindings = binding.propertyBindings;
            value.isBindingBase = binding.isBindingBase;
            value.arrayItemBinding = binding.arrayItemBinding;
          });
        }
      }
    }

    return value;
  }
}

const ObjectBindingConfigurationReviver = new ObjectBindingConfigurationReviverClass();
export default ObjectBindingConfigurationReviver;