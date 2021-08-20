﻿import JsObjectHandler from "./JsObjectHandler.js";
import ObjectBindingHandler from "./ObjectBindingHandler.js";

/**
 * @typedef {import("./References/DelegateReference.js").default} DelegateReference
 * @typedef {import("./References/DelegateReference.js").DotNetObjectReference} DotNetObjectReference
 */

/**
 * @typedef {Object} ProxyFunction
 * @property {DotNetDelegateProxy} delegateProxy
 */

/**
 * A DotNet delegate proxy to be invoked in JS.
 */
export default class DotNetDelegateProxy {
  /** @type {DelegateReference} */
  delegateReference;
  /** @type {ProxyFunction} A function, when invoked executes the DotNet delegate. */
  proxyFunction;

  /**
   * Creates a new instance of the DotNetDelegateProxy class.
   * @param {DelegateReference} delegateReference The delegate reference.
   */
  constructor(delegateReference) {
    this.delegateReference = delegateReference;
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
      throw new Error(invokeResult.ErrorMessage);
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
    let invokeResult = await delegateInvoker.invokeMethodAsync("InvokeDelegateFromJsAsync", invokeArgs);
    if (invokeResult && !invokeResult.hasOwnProperty("isError") && invokeResult.hasOwnProperty("result")) {
      // unwrap from a Task object
      invokeResult = invokeResult.result;
    }

    if (invokeResult && invokeResult.isError && invokeResult.errorMessage) {
      throw new Error(invokeResult.ErrorMessage);
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
      accessPath = JsObjectHandler.getAccessPathFromReferenceId(referenceId);
      JsObjectHandler.addObjectReference(referenceId, invokeArgs);
    }

    return invokeArgs.map((invokeArg, index) => {
      const invokeArgAccessPath = accessPath ? JsObjectHandler.combineAccessPaths(accessPath, index.toString()) : null;
      return ObjectBindingHandler.getValueFromBinding(invokeArg, bindings[index], invokeArgAccessPath);
    });
  }
}

/**
 * Checks if a value is a ProxyFunction.
 * @param {any} value
 * @returns {value is ProxyFunction}
 */
 export function IsProxyFunction(value) {
  return value && /** @type {ProxyFunction} */(value).delegateProxy?.proxyFunction === value;
}