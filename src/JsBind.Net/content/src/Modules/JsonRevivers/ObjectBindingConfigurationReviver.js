import { IsObjectBindingConfiguration } from "../InvokeOptions/ObjectBindingConfiguration.js";

/**
 * @typedef {import("../InvokeOptions/ObjectBindingConfiguration.js").default} ObjectBindingConfiguration
 */

/**
 * @callback FoundBindingCallback
 * @param {ObjectBindingConfiguration} binding
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
   * @param {any} _key
   * @param {any} value
   */
  revive(_key, value) {
    if (IsObjectBindingConfiguration(value)) {
      if (value.id) {
        this.foundObject(value);
      } else if (value.referenceId) {
        this.trackReference(value);
      }
    }

    return value;
  }

  /**
   * Called when an object with id is found.
   * @param {ObjectBindingConfiguration} obj 
   */
  foundObject(obj) {
    this.references[obj.id] = obj;
    if (this.referenceCallbacks.hasOwnProperty(obj.id)) {
      this.referenceCallbacks[obj.id].forEach(callback => callback(obj));
      this.referenceCallbacks[obj.id] = [];
      try {
        delete this.referenceCallbacks[obj.id];
      } catch { }
    }
  }

  /**
   * Track a reference object with reference id to be initialized when the referenced object is found.
   * @param {any} referenceObject 
   */
  trackReference(referenceObject) {
    const foundBinding = (/** @type {ObjectBindingConfiguration} */ binding) => {
      referenceObject.include = binding.include;
      referenceObject.exclude = binding.exclude;
      referenceObject.propertyBindings = binding.propertyBindings;
      referenceObject.isBindingBase = binding.isBindingBase;
      referenceObject.arrayItemBinding = binding.arrayItemBinding;
    };
    if (this.references.hasOwnProperty(referenceObject.referenceId)) {
      foundBinding(this.references[referenceObject.referenceId]);
    } else {
      if (!this.referenceCallbacks.hasOwnProperty(referenceObject.referenceId)) {
        this.referenceCallbacks[referenceObject.referenceId] = [];
      }
      this.referenceCallbacks[referenceObject.referenceId].push(foundBinding);
    }
  }
}

const ObjectBindingConfigurationReviver = new ObjectBindingConfigurationReviverClass();
export default ObjectBindingConfigurationReviver;