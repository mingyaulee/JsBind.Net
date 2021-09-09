import AccessPaths from "./AccessPaths.js";
import { IsProxyFunction } from "./DotNetDelegateProxy.js";

const AccessPathPropertyName = "__jsBindAccessPath";
const JsRuntimePropertyName = "__jsBindJsRuntime";

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
 * Check if the value should be returned without binding.
 * @param {any} value
 * @param {ObjectBindingConfiguration} binding
 * @param {string} [accessPath]
 * @returns {boolean}
 */
function shouldReturnValueWithoutBinding(value, binding, accessPath) {
  if (!shouldProcessObject(value) || !binding) {
    return true;
  }

  if (!shouldProcessBinding(binding)) {
    if (binding.isBindingBase) {
      value[AccessPathPropertyName] = accessPath;
      value[JsRuntimePropertyName] = 0;
    }
    return true;
  }

  return false;
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

  if (shouldReturnValueWithoutBinding(value, binding, accessPath)) {
    return value;
  }

  if (binding.arrayItemBinding) {
    return getArrayValueFromBinding(value, binding.arrayItemBinding, accessPath);
  }

  const includeAllProperties = binding.include && binding.include.some(includeProperty => includeProperty === "*");
  const excludeProperties = binding.exclude?.map(excludeProperty => excludeProperty.toUpperCase());
  const getPropertyBinding = (propertyName) => {
    return binding.propertyBindings?.[propertyName.toUpperCase()];
  }

  const boundValue = {
  };

  if (binding.isBindingBase) {
    boundValue[AccessPathPropertyName] = accessPath;
    boundValue[JsRuntimePropertyName] = 0;
  }

  if (binding.include && !includeAllProperties) {
    // Fast path: The include properties are known
    binding.include.forEach(property => {
      boundValue[property] = getValueFromBinding(value[property], getPropertyBinding(property), AccessPaths.combine(accessPath, property));
    });
    return boundValue;
  }

  // Slow path: Include all properties or only the exclude properties are known
  getObjectKeys(value).forEach(property => {
    if (includeAllProperties) {
      boundValue[property] = getValueFromBinding(value[property], getPropertyBinding(property), AccessPaths.combine(accessPath, property));
    } else if (excludeProperties) {
      const upperCasePropertyName = property.toUpperCase();
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
