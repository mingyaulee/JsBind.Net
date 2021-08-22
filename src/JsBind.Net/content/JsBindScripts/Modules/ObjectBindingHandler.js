import AccessPaths from "./AccessPaths.js";
import { IsProxyFunction } from "./DotNetDelegateProxy.js";

const AccessPathPropertyName = "__jsBindAccessPath";

/**
 * @typedef {import("./InvokeOptions/ObjectBindingConfiguration.js").default} ObjectBindingConfiguration
 * @typedef {import("./DotNetDelegateProxy.js").default} DotNetDelegateProxy
 */

/**
 * Checks if the binding configuration should be processed.
 * @param {ObjectBindingConfiguration} binding
 * @returns {Boolean}
 */
function shouldProcessBinding(binding) {
  return !!binding.arrayItemBinding || (binding.include && binding.include.length > 0) || (binding.exclude && binding.exclude.length > 0);
}

/**
 * Checks if the object should be processed.
 * @param {any} obj 
 * @returns {Boolean}
 */
function shouldProcessObject(obj) {
  return obj && typeof obj === "object";
}

/**
 * Get all the keys in the object.
 * @param {any} value
 * @returns {string[]}
 */
function getObjectKeys(value) {
  const objectPrototype = Object.getPrototypeOf(value);
  if (objectPrototype) {
    return [...Object.keys(value), ...getObjectKeys(objectPrototype)];
  }
  return Object.keys(value);
}

/**
 * Get the array value from array item binding.
 * @param {any} value
 * @param {ObjectBindingConfiguration} arrayItemBinding
 * @param {string} [accessPath]
 * @returns {any[]}
 */
function getArrayValueFromBinding(value, arrayItemBinding, accessPath) {
  if (value && typeof value[Symbol.iterator] === "function") {
    return [...value].map((arrayItem, index) => {
      const arrayItemAccessPath = AccessPaths.combine(accessPath, index.toString());
      return getValueFromBinding(arrayItem, arrayItemBinding, arrayItemAccessPath);
    });
  }
  return [];
}

/**
 * Get the object for a JS function.
 * @param {any} value
 * @param {string} [accessPath]
 * @returns {any}
 */
function getFunctionValue(value, accessPath) {
  if (IsProxyFunction(value)) {
    return {
      delegateId: value.delegateProxy.delegateReference.delegateId
    };
  }

  return {
    accessPath: accessPath
  };
}

/**
 * Get the value based on the binding configuration.
 * @param {any} value
 * @param {ObjectBindingConfiguration} binding
 * @param {string} [accessPath]
 * @returns {any}
 */
function getValueFromBinding(value, binding, accessPath) {
  if (value instanceof Function) {
    return getFunctionValue(value, accessPath);
  }

  if (!shouldProcessObject(value)) {
    return value;
  }

  if (!binding || !shouldProcessBinding(binding)) {
    value[AccessPathPropertyName] = accessPath;
    return value;
  }

  if (binding.arrayItemBinding) {
    return getArrayValueFromBinding(value, binding.arrayItemBinding, accessPath);
  }

  const includeProperties = binding.include?.map(includeProperty => includeProperty.toUpperCase());
  const excludeProperties = binding.exclude?.map(excludeProperty => excludeProperty.toUpperCase());
  const getPropertyBinding = (propertyName) => {
    return binding.propertyBindings?.[propertyName.toUpperCase()];
  }

  const boundValue = {
    [AccessPathPropertyName]: accessPath
  };
  getObjectKeys(value).forEach(property => {
    const upperCasePropertyName = property.toUpperCase();
    if (includeProperties) {
      if (includeProperties.some(includeProperty => includeProperty === upperCasePropertyName || includeProperty === "*")) {
        boundValue[property] = getValueFromBinding(value[property], getPropertyBinding(property), AccessPaths.combine(accessPath, property));
      }
    } else if (excludeProperties) {
      if (excludeProperties.every(excludeProperty => excludeProperty !== upperCasePropertyName)) {
        boundValue[property] = getValueFromBinding(value[property], getPropertyBinding(property), AccessPaths.combine(accessPath, property));
      }
    }
  });
  return boundValue;
}

class ObjectBindingHandlerClass {
  /**
   * Get the value based on the binding configuration.
   * @param {any} value
   * @param {ObjectBindingConfiguration} binding
   * @param {string} [accessPath]
   * @returns {any}
   */
  getValueFromBinding(value, binding, accessPath) { return getValueFromBinding(value, binding, accessPath); }
}

const ObjectBindingHandler = new ObjectBindingHandlerClass();
export default ObjectBindingHandler;
