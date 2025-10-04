import AccessPaths from "./AccessPaths.js";
import DelegateReferenceHandler from "./DelegateReferenceHandler.js";
import JsObjectHandler from "./JsObjectHandler.js";
import ObjectBindingHandler from "./ObjectBindingHandler.js";
import InvokeResult from "./InvokeResult.js";
import DelegateReferenceReviver from "./JsonRevivers/DelegateReferenceReviver.js";
import ObjectBindingConfigurationReviver from "./JsonRevivers/ObjectBindingConfigurationReviver.js";
import ObjectReferenceReviver from "./JsonRevivers/ObjectReferenceReviver.js";

/**
 * @typedef {import("./InvokeOptions/InvokeOption.js").default} InvokeOption
 * @typedef {import("./InvokeOptions/CompareObjectOption.js").default} CompareObjectOption
 * @typedef {import("./InvokeOptions/ConvertObjectTypeOption.js").default} ConvertObjectTypeOption
 * @typedef {import("./InvokeOptions/DisposeDelegateOption.js").default} DisposeDelegateOption
 * @typedef {import("./InvokeOptions/DisposeObjectOption.js").default} DisposeObjectOption
 * @typedef {import("./InvokeOptions/GetPropertyOption.js").default} GetPropertyOption
 * @typedef {import("./InvokeOptions/InvokeFunctionOption.js").default} InvokeFunctionOption
 * @typedef {import("./InvokeOptions/SetPropertyOption.js").default} SetPropertyOption
 */

function attachDotNetRevivers() {
  if (globalThis.DotNet === undefined) {
    console.error("DotNet should be imported before JsBind is imported.");
    return;
  }
  globalThis.DotNet.attachReviver(DelegateReferenceReviver.revive.bind(DelegateReferenceReviver));
  globalThis.DotNet.attachReviver(ObjectBindingConfigurationReviver.revive.bind(ObjectBindingConfigurationReviver));
  globalThis.DotNet.attachReviver(ObjectReferenceReviver.revive.bind(ObjectReferenceReviver));
}

export default class JsBindNet {
  constructor() {
    attachDotNetRevivers();
  }

  /**
   * Compare objects.
   * @param {CompareObjectOption} compareObjectOption
   * @returns {InvokeResult}
   */
  CompareObject(compareObjectOption) {
    const obj1 = JsObjectHandler.getObjectFromAccessPath(compareObjectOption.object1AccessPath);
    const obj2 = JsObjectHandler.getObjectFromAccessPath(compareObjectOption.object2AccessPath);
    return this._getReturnValue(obj1 === obj2, compareObjectOption);
  }

  /**
   * Convert object type.
   * @param {ConvertObjectTypeOption} convertObjectTypeOption
   * @returns {InvokeResult}
   */
  ConvertObjectType(convertObjectTypeOption) {
    const obj = JsObjectHandler.getObjectFromAccessPath(convertObjectTypeOption.accessPath);
    return this._getReturnValue(obj, convertObjectTypeOption);
  }

  /**
   * Dispose delegate reference.
   * @param {DisposeDelegateOption} disposeDelegateOption
   */
  DisposeDelegate(disposeDelegateOption) {
    DelegateReferenceHandler.removeDelegateReference(disposeDelegateOption.delegateId);
  }

  /**
   * Dispose object reference.
   * @param {DisposeObjectOption} disposeObjectOption
   */
  DisposeObject(disposeObjectOption) {
    JsObjectHandler.removeObjectReference(disposeObjectOption.referenceId);
  }

  /**
   * Dispose session.
   */
  DisposeSession() {
    JsObjectHandler.clearObjectReferences();
    DelegateReferenceHandler.clearDelegateReferences();
  }

  /**
   * Get the property value of an object.
   * @param {GetPropertyOption} getPropertyOption
   * @returns {InvokeResult}
   */
  GetProperty(getPropertyOption) {
    const targetObject = JsObjectHandler.getObjectFromAccessPath(getPropertyOption.accessPath);
    if (targetObject === undefined || targetObject === null) {
      return this._getErrorReturnValue(`Target object '${getPropertyOption.accessPath}' is null or undefined.`);
    }
    const returnValue = targetObject[getPropertyOption.propertyName];
    getPropertyOption.getReturnValueAccessPath = function () {
      return AccessPaths.combine(getPropertyOption.accessPath, getPropertyOption.propertyName);
    };
    return this._getReturnValue(returnValue, getPropertyOption);
  }

  /**
   * Set the property value of an object.
   * @param {SetPropertyOption} setPropertyOption
   * @returns {InvokeResult}
   */
  SetProperty(setPropertyOption) {
    const targetObject = JsObjectHandler.getObjectFromAccessPath(setPropertyOption.accessPath);
    if (targetObject === undefined || targetObject === null) {
      return this._getErrorReturnValue(`Target object '${setPropertyOption.accessPath}' is null or undefined.`);
    }
    targetObject[setPropertyOption.propertyName] = setPropertyOption.propertyValue;
    return null;
  }

  /**
   * Invoke a function on an object.
   * @param {InvokeFunctionOption} invokeFunctionOption
   * @returns {InvokeResult}
   */
  InvokeFunction(invokeFunctionOption) {
    try {
      const returnValue = this._invokeFunctionInternal(invokeFunctionOption);
      if (invokeFunctionOption.hasReturnValue) {
        invokeFunctionOption.getReturnValueAccessPath = function () {
          return AccessPaths.fromReferenceId(invokeFunctionOption.returnValueReferenceId);
        }
      }
      return this._getReturnValue(returnValue, invokeFunctionOption);
    } catch (error) {
      console.error(error);
      return this._getErrorReturnValue(error.message, error.stack);
    }
  }

  /**
   * Invoke a function on an object asynchronously.
   * @param {InvokeFunctionOption} invokeFunctionOption
   * @returns {Promise<InvokeResult>}
   */
  async InvokeFunctionAsync(invokeFunctionOption) {
    try {
      let returnValue = this._invokeFunctionInternal(invokeFunctionOption);
      if (returnValue instanceof Promise) {
        returnValue = await returnValue;
      }
      if (invokeFunctionOption.hasReturnValue) {
        invokeFunctionOption.getReturnValueAccessPath = function () {
          return AccessPaths.fromReferenceId(this.returnValueReferenceId);
        };
      }
      return this._getReturnValue(returnValue, invokeFunctionOption);
    } catch (error) {
      console.error(error);
      return this._getErrorReturnValue(error.message, error.stack);
    }
  }

  /**
   * Get the object references.
   * @returns {object} The object references.
   */
  getObjectReferences() { return JsObjectHandler.getObjectReferences(); }

  /**
   * Get the count of the object references.
   * @returns {number} The count of the object references.
   */
  getObjectReferencesCount() { return JsObjectHandler.getObjectReferencesCount(); }

  /**
   * Get the delegate references.
   * @returns {object} The delegate references.
   */
  getDelegateReferences() { return DelegateReferenceHandler.getDelegateReferences(); }

  /**
   * Get the count of the delegate references.
   * @returns {number} The count of the delegate references.
   */
  getDelegateReferencesCount() { return DelegateReferenceHandler.getDelegateReferencesCount(); }

  /**
   * Get the return value wrapped in InvokeResult.
   * @param {any} returnValue
   * @param {InvokeOption} invokeOption
   * @returns {InvokeResult}
   */
  _getReturnValue(returnValue, invokeOption) {
    if (invokeOption.hasReturnValue) {
      if (returnValue === null || returnValue === undefined) {
        return new InvokeResult(null);
      }

      if (invokeOption.returnValueIsReference) {
        JsObjectHandler.addObjectReference(invokeOption.returnValueReferenceId, returnValue);
      }

      const returnValueAccessPath = invokeOption.getReturnValueAccessPath ? invokeOption.getReturnValueAccessPath() : null;
      const value = ObjectBindingHandler.getValueFromBinding(returnValue, invokeOption.returnValueBinding, returnValueAccessPath);
      return new InvokeResult(value);
    }
    return null;
  }

  /**
   * Get the error wrapped in InvokeResult.
   * @param {string} errorMessage
   * @param {string} [stackTrace]
   * @returns {InvokeResult}
   */
  _getErrorReturnValue(errorMessage, stackTrace) {
    return new InvokeResult(null, true, errorMessage, stackTrace);
  }

  /**
   * Invoke a function on an object.
   * @param {InvokeFunctionOption} invokeFunctionOption
   */
  _invokeFunctionInternal(invokeFunctionOption) {
    const targetObject = JsObjectHandler.getObjectFromAccessPath(invokeFunctionOption.accessPath);
    if (targetObject === undefined || targetObject === null) {
      throw new Error("Target object is null or undefined.");
    }

    const targetFunction = invokeFunctionOption.functionName ? targetObject[invokeFunctionOption.functionName] : targetObject;
    if (!targetFunction || typeof(targetFunction) !== "function") {
      if (invokeFunctionOption.functionName) {
        throw new Error(`Member ${invokeFunctionOption.functionName} on target object is not a function.`);
      } else {
        throw new Error(`Target object is not a function.`);
      }
    }

    return targetFunction.apply(targetObject, invokeFunctionOption.functionArguments);
  }
}