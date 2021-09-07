import AccessPaths from "./AccessPaths.js";
import JsBindError from "./JsBindError.js";
import JsObjectHandler from "./JsObjectHandler.js";
import ObjectBindingHandler from "./ObjectBindingHandler.js";

/**
 * @typedef {import("./References/DelegateReference.js").default} DelegateReference
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
export default class DotNetDelegateProxy {
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
      return this._invokeDelegateAsyncInternal(this.delegateReference.delegateId, processedInvokeArgs);
    } else {
      return this._invokeDelegateInternal(this.delegateReference.delegateId, processedInvokeArgs);
    }
  }

  /**
   * Invoke the DotNet delegate synchronously.
   * @param {string} delegateId
   * @param {any[]} invokeArgs
   * @returns {any} 
   */
  _invokeDelegateInternal(delegateId, invokeArgs) {
    const invokeResult = globalThis.DotNet.invokeMethod("JsBind.Net", "InvokeDelegateFromJs", delegateId, invokeArgs);
    if (invokeResult && invokeResult.isError && invokeResult.errorMessage) {
      throw new JsBindError(invokeResult.errorMessage, invokeResult.stackTrace);
    }

    return invokeResult?.value;
  }

  /**
   * Invoke the DotNet delegate asynchronously.
   * @param {string} delegateId
   * @param {any[]} invokeArgs
   * @returns {Promise<any>} 
   */
  async _invokeDelegateAsyncInternal(delegateId, invokeArgs) {
    let invokeAsyncResult = await globalThis.DotNet.invokeMethodAsync("JsBind.Net", "InvokeDelegateFromJsAsync", delegateId, invokeArgs);
    let invokeResult = unwrapAsyncResult(invokeAsyncResult);

    if (invokeResult && invokeResult.isError && invokeResult.errorMessage) {
      throw new JsBindError(invokeResult.errorMessage, invokeResult.stackTrace);
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

    return invokeArgs.map((invokeArg, index) => {
      let invokeArgAccessPath = null;

      if (this.delegateReference.storeArgumentsAsReferences[index]) {
        const referenceId = this.delegateReference.argumentsReferenceIds[index];
        invokeArgAccessPath = AccessPaths.fromReferenceId(referenceId);
        JsObjectHandler.addObjectReference(referenceId, invokeArg);
      }

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