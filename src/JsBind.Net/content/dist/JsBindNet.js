(function () {
  'use strict';

  const ReferenceIdPrefix = "#";
  const AccessPathSeparator = ".";

  class AccessPathsClass {
    /**
     * Checks if the is a reference identifier.
     * @param {string} accessPath
     * @returns {boolean}
     */
    isReferenceId(accessPath) {
      return !!accessPath && accessPath.startsWith(ReferenceIdPrefix);
    }

    /**
     * Gets the reference identifier from the access path.
     * @param {string} accessPath
     * @returns {string}
     */
    getReferenceId(accessPath) {
      return accessPath?.substring(ReferenceIdPrefix.length);
    }

    /**
     * Get access path from the specified reference identifier.
     * @param {string} referenceId The object reference identifier.
     * @returns {string} The access path.
     */
    fromReferenceId(referenceId) {
      return ReferenceIdPrefix + referenceId;
    }

    /**
     * Combine multiple access paths.
     * @param {string[]} accessPaths
     * @returns {string}
     */
    combine(...accessPaths) {
      return accessPaths?.filter(accessPath => typeof accessPath === "string" && accessPath.length)?.join(AccessPathSeparator);
    }

    /**
     * Splits the access path based on the access path separator.
     * @param {string} accessPath
     * @returns {string[]}
     */
    split(accessPath) {
      return accessPath?.split(AccessPathSeparator);
    }
  }

  const AccessPaths = new AccessPathsClass();

  class JsObjectHandlerClass {
    constructor() {
      this._objectReferences = {};
      this._objectReferencesCount = 0;
    }

    /**
     * Get an object from access path.
     * @param {string} accessPath The access path.
     * @returns {any} The object reference.
     */
    getObjectFromAccessPath(accessPath) {
      if (!accessPath) {
        return globalThis;
      }

      let targetObject = globalThis;
      const accessPaths = AccessPaths.split(accessPath);

      for (let i = 0; i < accessPaths.length; i++) {
        if (i === 0 && AccessPaths.isReferenceId(accessPaths[i])) {
          const referenceId = AccessPaths.getReferenceId(accessPaths[i]);
          targetObject = this._objectReferences[referenceId];
        } else {
          targetObject = targetObject?.[accessPaths[i]];
        }
      }

      return targetObject;
    }

    /**
     * Add an object reference with the specified identifier.
     * @param {string} referenceId The object reference identifier.
     * @param {any} objectReference The object reference.
     */
    addObjectReference(referenceId, objectReference) {
      if (!this._objectReferences[referenceId]) {
        this._objectReferencesCount++;
      }
      this._objectReferences[referenceId] = objectReference;
    }

    /**
     * Remove object reference.
     * @param {string} referenceId The object reference identifier.
     */
    removeObjectReference(referenceId) {
      if (this._objectReferences[referenceId]) {
        this._objectReferencesCount--;
        this._objectReferences[referenceId] = null;
        try {
          delete this._objectReferences[referenceId];
        } catch { }
      }
    }

    /**
     * Get the object references.
     * @returns {object} The object references.
     */
    getObjectReferences() { return this._objectReferences; }

    /**
     * Get the count of the object references.
     * @returns {number} The count of the object references.
     */
    getObjectReferencesCount() { return this._objectReferencesCount; }
  }

  const JsObjectHandler = new JsObjectHandlerClass();

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
      if (binding.setAccessPath) {
        value[AccessPathPropertyName] = accessPath;
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

    const includeProperties = binding.include?.map(includeProperty => includeProperty.toUpperCase());
    const excludeProperties = binding.exclude?.map(excludeProperty => excludeProperty.toUpperCase());
    const getPropertyBinding = (propertyName) => {
      return binding.propertyBindings?.[propertyName.toUpperCase()];
    };

    const boundValue = {
    };

    if (binding.setAccessPath) {
      boundValue[AccessPathPropertyName] = accessPath;
    }

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

  /**
   * @typedef {import("./References/DelegateReference.js").default} DelegateReference
   * @typedef {import("./References/DelegateReference.js").DotNetObjectReference} DotNetObjectReference
   * @typedef {import("./References/DelegateReference.js").TaskInvokeResult} TaskInvokeResult
   * @typedef {import("./InvokeResult.js").default} InvokeResult
   */

  /**
   * @typedef {Object} ProxyFunction
   * @property {DotNetDelegateProxy} delegateProxy
   */

  /**
   * Checks if an object is TaskInvokeResult.
   * @param {any} obj
   * @returns {obj is TaskInvokeResult}
   */
  function isTaskInvokeResult(obj) {
    return obj && !obj.hasOwnProperty("isError") && obj.hasOwnProperty("result");
  }

  /**
   * Unwrap the result if it is contained in a Task object.
   * @param {InvokeResult | TaskInvokeResult} asyncResult
   * @returns {InvokeResult}
   */
  function unwrapAsyncResult(asyncResult) {
    if (isTaskInvokeResult(asyncResult)) {
      // unwrap from a Task object
      return asyncResult.result;
    }
    return asyncResult;
  }

  /**
   * A DotNet delegate proxy to be invoked in JS.
   */
  class DotNetDelegateProxy {
    /**
     * Creates a new instance of the DotNetDelegateProxy class.
     * @param {DelegateReference} delegateReference The delegate reference.
     */
    constructor(delegateReference) {
      this.delegateReference = delegateReference;
      /** @type {ProxyFunction} A function, when invoked executes the DotNet delegate. */
      this.proxyFunction = this._dynamicInvoke.bind(this);
      this.proxyFunction.delegateProxy = this;
    }

    /**
     * Dynamically invoke the DotNet delegate synchronously.
     * @param  {any[]} invokeArgs JSON-serializable arguments.
     * @returns {object} An object obtained by JSON-deserializing the return value.
     */
    _dynamicInvoke(...invokeArgs) {
      const processedInvokeArgs = this._processInvokeArgs(invokeArgs);
      if (this.delegateReference.isAsync) {
        return this._invokeDelegateAsyncInternal(this.delegateReference.delegateInvoker, processedInvokeArgs);
      } else {
        return this._invokeDelegateInternal(this.delegateReference.delegateInvoker, processedInvokeArgs);
      }
    }

    /**
     * Invoke the DotNet delegate synchronously.
     * @param {DotNetObjectReference} delegateInvoker
     * @param {any[]} invokeArgs
     * @returns {any} 
     */
    _invokeDelegateInternal(delegateInvoker, invokeArgs) {
      const invokeResult = delegateInvoker.invokeMethod("InvokeDelegateFromJs", invokeArgs);
      if (invokeResult && invokeResult.isError && invokeResult.errorMessage) {
        throw new Error(invokeResult.errorMessage);
      }

      return invokeResult?.value;
    }

    /**
     * Invoke the DotNet delegate asynchronously.
     * @param {DotNetObjectReference} delegateInvoker
     * @param {any[]} invokeArgs
     * @returns {Promise<any>} 
     */
    async _invokeDelegateAsyncInternal(delegateInvoker, invokeArgs) {
      let invokeAsyncResult = await delegateInvoker.invokeMethodAsync("InvokeDelegateFromJsAsync", invokeArgs);
      let invokeResult = unwrapAsyncResult(invokeAsyncResult);

      if (invokeResult && invokeResult.isError && invokeResult.errorMessage) {
        throw new Error(invokeResult.errorMessage);
      }

      return invokeResult?.value;
    }

    /**
     * Process delegate invocation arguments.
     * @param {any[]} invokeArgs
     */
    _processInvokeArgs(invokeArgs) {
      const bindings = this.delegateReference.argumentBindings;
      if (!invokeArgs || !invokeArgs.length || !bindings.length) {
        return invokeArgs;
      }

      let accessPath = null;
      if (this.delegateReference.storeArgumentsAsReference){
        const referenceId = this.delegateReference.argumentsReferenceId;
        accessPath = AccessPaths.fromReferenceId(referenceId);
        JsObjectHandler.addObjectReference(referenceId, invokeArgs);
      }

      return invokeArgs.map((invokeArg, index) => {
        const invokeArgAccessPath = accessPath ? AccessPaths.combine(accessPath, index.toString()) : null;
        return ObjectBindingHandler.getValueFromBinding(invokeArg, bindings[index], invokeArgAccessPath);
      });
    }
  }

  /**
   * Checks if a value is a ProxyFunction.
   * @param {any} value
   * @returns {value is ProxyFunction}
   */
   function IsProxyFunction(value) {
    return value && /** @type {ProxyFunction} */(value).delegateProxy?.proxyFunction === value;
  }

  /**
   * @typedef {import("./References/DelegateReference.js").default} DelegateReference
   */

  class DelegateReferenceHandlerClass {
    constructor() {
      /** @type {Object<string, DotNetDelegateProxy>} */
      this._delegateReferences = {};
    }

    /**
     * Get or create the delegate proxy from the delegate reference.
     * @param {DelegateReference} delegateReference
     * @returns {DotNetDelegateProxy}
     */
    getOrCreateDelegateProxy(delegateReference) {
      if (this._delegateReferences[delegateReference.delegateId]) {
        return this._delegateReferences[delegateReference.delegateId];
      }

      const delegateProxy = new DotNetDelegateProxy(delegateReference);
      this._delegateReferences[delegateReference.delegateId] = delegateProxy;
      return delegateProxy;
    }

    /**
     * Remove the delegate reference by delegate ID.
     * @param {string} delegateId 
     */
    removeDelegateReference(delegateId) {
      if (this._delegateReferences[delegateId]) {
        this._delegateReferences[delegateId] = null;
        try {
          delete this._delegateReferences[delegateId];
        } catch { }
      }
    }
  }

  const DelegateReferenceHandler = new DelegateReferenceHandlerClass();

  class InvokeResult {
    /**
     * Creates a new instance of InvokeResult.
     * @param {any} value
     * @param {boolean} [isError]
     * @param {string} [errorMessage]
     */
    constructor(value, isError, errorMessage) {
      this.value = value;
      this.isError = isError;
      this.errorMessage = errorMessage;
    }
  }

  /**
   * @typedef {import("./ReferenceType.js").ReferenceTypeEnumValue} ReferenceTypeEnumValue
   */

  /**
   * Checks if a value is a ReferenceBase.
   * @param {any} value
   * @returns {value is ReferenceBase}
   */
  function IsReferenceBase(value) {
    return value &&
      typeof value === "object" &&
      /** @type {ReferenceBase} */(value).__isJsBindReference === true;
  }

  /** 
   * @typedef {"Object" | "Delegate"} ReferenceTypeEnumValue
   */

  /**
   * @typedef ReferenceTypeEnum
   * @property {ReferenceTypeEnumValue} Object
   * @property {ReferenceTypeEnumValue} Delegate
   */

  /**
   * @type {ReferenceTypeEnum}
   */
  const ReferenceType = {
    Object: "Object",
    Delegate: "Delegate"
  };

  /**
   * Checks if a value is a DelegateReference.
   * @param {any} value
   * @returns {value is DelegateReference}
   */
  function IsDelegateReference(value) {
    return IsReferenceBase(value) && value.__referenceType === ReferenceType.Delegate;
  }

  class DelegateReferenceReviverClass {
    /**
     * Converts a DelegateReference JSON object to a proxy function.
     * @param {any} key
     * @param {any} value
     */
    revive(key, value) {
      if (IsDelegateReference(value)) {
        return DelegateReferenceHandler.getOrCreateDelegateProxy(value).proxyFunction;
      }

      return value;
    }
  }

  const DelegateReferenceReviver = new DelegateReferenceReviverClass();

  /**
   * Checks if a value is an ObjectReference.
   * @param {any} value
   * @returns {value is ObjectReference}
   */
  function IsObjectReference(value) {
    return IsReferenceBase(value) && value.__referenceType === ReferenceType.Object;
  }

  class ObjectReferenceReviverClass {
    /**
     * Converts an ObjectReference JSON object to an object.
     * @param {any} key
     * @param {any} value
     */
    revive(key, value) {
      if (IsObjectReference(value)) {
        return JsObjectHandler.getObjectFromAccessPath(value.accessPath);
      }

      return value;
    }
  }

  const ObjectReferenceReviver = new ObjectReferenceReviverClass();

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
    if (typeof globalThis.DotNet === "undefined") {
      setTimeout(attachDotNetRevivers, 10);
      return;
    }
    globalThis.DotNet.attachReviver(DelegateReferenceReviver.revive);
    globalThis.DotNet.attachReviver(ObjectReferenceReviver.revive);
  }

  class JsBindNet {
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
     * Get the property value of an object.
     * @param {GetPropertyOption} getPropertyOption
     * @returns {InvokeResult}
     */
    GetProperty(getPropertyOption) {
      const targetObject = JsObjectHandler.getObjectFromAccessPath(getPropertyOption.accessPath);
      if (targetObject === undefined || targetObject === null) {
        return this._getErrorReturnValue("Target object is null or undefined.");
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
        return this._getErrorReturnValue("Target object is null or undefined.");
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
          };
        }
        return this._getReturnValue(returnValue, invokeFunctionOption);
      } catch (error) {
        console.error(error);
        return this._getErrorReturnValue(error.message);
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
        return this._getErrorReturnValue(error.message);
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
     * @returns {InvokeResult}
     */
    _getErrorReturnValue(errorMessage) {
      return new InvokeResult(null, true, errorMessage);
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
      if (!targetFunction || !(targetFunction instanceof Function)) {
        if (invokeFunctionOption.functionName) {
          throw new Error(`Member ${invokeFunctionOption.functionName} on target object is not a function.`);
        } else {
          throw new Error(`Target object is not a function.`);
        }
      }

      return targetFunction.apply(targetObject, invokeFunctionOption.functionArguments);
    }
  }

  globalThis.JsBindNet = new JsBindNet();

}());
